using System;
using System.Collections.Generic;
using System.Linq;
using Broiler.Code.Review.Assurance;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Broiler.Code.Language.CSharp.Assurance;

/// <summary>
/// The whole-file scan the command-line tool needs: every unit with its place
/// on the line and the assurance comments the parser attaches to it, the file
/// fingerprint, the directives, every assurance comment in the file, and every
/// line of every comment.
///
/// Units, names, exemptions and fingerprints come from
/// <see cref="CSharpAssuranceScanner"/>, so the two scanners cannot disagree
/// about them. What this class adds is read from trivia. The block a unit
/// carries is the first <c>// Broiler-AI:</c> single-line comment in the
/// declaration's leading trivia, which is exactly how the owning component
/// attaches one; a scan of the lines above the declaration would also find
/// markers inside raw strings and disabled code.
/// </summary>
public sealed class CSharpAssuranceFileScanner : IAssuranceFileScanner
{
    private static readonly string[] Markers =
    [
        AssuranceVocabulary.AiMarker,
        AssuranceVocabulary.FalsifiedIfMarker,
        AssuranceVocabulary.HumanMarker,
    ];

    private readonly CSharpParseOptions _parseOptions;
    private readonly AssuranceExemptionPredicate _predicate;

    /// <summary>
    /// A file scanner parsing under <paramref name="preprocessorSymbols"/>, or
    /// under <see cref="CSharpAssuranceScanner.DefaultPreprocessorSymbols"/> when
    /// that is null, applying <paramref name="predicate"/>.
    /// </summary>
    public CSharpAssuranceFileScanner(
        IEnumerable<string>? preprocessorSymbols = null,
        AssuranceExemptionPredicate predicate = AssuranceExemptionPredicate.OwningComponent)
    {
        _parseOptions = CSharpAssuranceScanner.OptionsFor(
            preprocessorSymbols ?? CSharpAssuranceScanner.DefaultPreprocessorSymbols);
        _predicate = predicate;
    }

    /// <inheritdoc/>
    public AssuranceScannedFile ScanFile(string text, string path)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(path);

        SyntaxTree tree = CSharpSyntaxTree.ParseText(text, _parseOptions, path: path);
        SyntaxNode root = tree.GetRoot();
        SourceText source = tree.GetText();

        var units = new List<AssuranceFileUnit>();
        foreach (ScannedDeclaration found in CSharpAssuranceScanner.Units(tree, _predicate))
        {
            MemberDeclarationSyntax declaration = found.Declaration;
            AssuranceScannedUnit unit = found.Unit;
            TextLine line = source.Lines[unit.DeclarationLine];
            string before = source.ToString(TextSpan.FromBounds(line.Start, declaration.SpanStart));
            bool ownLine = IsBlank(before);

            int? annotationLine = null;
            var leading = new List<int>();
            foreach (SyntaxTrivia trivia in declaration.GetLeadingTrivia())
            {
                if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    continue;

                string comment = trivia.ToString().TrimStart();
                if (!OpensWithMarker(comment))
                    continue;

                int at = LineOf(tree, trivia.Span);
                leading.Add(at);

                if (annotationLine is null &&
                    comment.StartsWith(AssuranceVocabulary.AiMarker, StringComparison.Ordinal))
                {
                    annotationLine = at;
                }
            }

            units.Add(new AssuranceFileUnit(
                unit,
                ownLine ? before : IndentOf(source.ToString(line.Span)),
                ownLine,
                annotationLine,
                leading,
                LineOf(tree, HeaderEnd(declaration).Span)));
        }

        // The same expression the owning component uses for its directive rule,
        // so the two report the same directives.
        var directives = root
            .DescendantNodesAndSelf(descendIntoTrivia: true)
            .OfType<DirectiveTriviaSyntax>()
            .Select(directive => new AssuranceDirective(
                LineOf(tree, directive.Span), directive.ToString().Trim()))
            .ToList();

        var commentLines = new SortedSet<int>();
        var comments = new List<AssuranceCommentLine>();
        foreach (SyntaxTrivia trivia in root.DescendantTrivia(descendIntoTrivia: true))
        {
            if (IsCommentLike(trivia))
                comments.AddRange(LinesOf(source, trivia));

            if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || !OpensWithMarker(trivia.ToString()))
                continue;

            // First thing on its line only. A trailing comment after code is not
            // a block line to anything that reads blocks.
            TextLine line = source.Lines.GetLineFromPosition(trivia.SpanStart);
            if (IsBlank(source.ToString(TextSpan.FromBounds(line.Start, trivia.SpanStart))))
                commentLines.Add(line.LineNumber);
        }

        return new AssuranceScannedFile(
            units,
            CSharpAssuranceScanner.Fingerprint(root),
            directives,
            [.. commentLines],
            source.Lines.Count)
        {
            CommentLines = comments,
        };
    }

    /// <summary>
    /// Trivia a reader takes for prose: every kind of comment, documentation
    /// comments included, and disabled text, which reads like code but is
    /// shown to nobody as code. A forged summary can be written in any of them.
    /// </summary>
    private static bool IsCommentLike(SyntaxTrivia trivia) =>
        trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
        trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
        trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
        trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia) ||
        trivia.IsKind(SyntaxKind.DisabledTextTrivia);

    /// <summary>One entry per physical line the trivia spans, with that line's part of it.</summary>
    private static IEnumerable<AssuranceCommentLine> LinesOf(SourceText source, SyntaxTrivia trivia)
    {
        TextSpan span = trivia.FullSpan;
        int first = source.Lines.GetLineFromPosition(span.Start).LineNumber;
        int last = source.Lines.GetLineFromPosition(Math.Max(span.Start, span.End - 1)).LineNumber;

        for (int number = first; number <= last; number++)
        {
            TextLine line = source.Lines[number];
            int start = Math.Max(line.Start, span.Start);
            int end = Math.Min(line.End, span.End);
            if (end > start)
                yield return new AssuranceCommentLine(number, source.ToString(TextSpan.FromBounds(start, end)));
        }
    }

    /// <summary>
    /// The token where a declaration's header ends and its content begins. An
    /// assurance comment above this token and below the declaration's first
    /// token is inside the header, where no declaration picks it up. The
    /// top-level unit has no header: its block goes above its first statement.
    /// </summary>
    private static SyntaxToken HeaderEnd(MemberDeclarationSyntax declaration)
    {
        SyntaxToken end = declaration switch
        {
            GlobalStatementSyntax statement => statement.GetFirstToken(),
            TypeDeclarationSyntax type => Present(type.OpenBraceToken) ?? type.SemicolonToken,
            EnumDeclarationSyntax @enum => @enum.OpenBraceToken,
            BaseMethodDeclarationSyntax method =>
                method.Body?.OpenBraceToken ?? method.ExpressionBody?.ArrowToken ?? method.SemicolonToken,
            PropertyDeclarationSyntax property =>
                property.AccessorList?.OpenBraceToken ?? property.ExpressionBody?.ArrowToken ?? default(SyntaxToken),
            IndexerDeclarationSyntax indexer =>
                indexer.AccessorList?.OpenBraceToken ?? indexer.ExpressionBody?.ArrowToken ?? default(SyntaxToken),
            EventDeclarationSyntax @event => @event.AccessorList?.OpenBraceToken ?? @event.SemicolonToken,
            BaseFieldDeclarationSyntax field =>
                field.Declaration.Variables.FirstOrDefault()?.Initializer?.EqualsToken ?? field.SemicolonToken,
            DelegateDeclarationSyntax @delegate => @delegate.SemicolonToken,
            EnumMemberDeclarationSyntax member => member.EqualsValue?.EqualsToken ?? member.Identifier,
            _ => default,
        };

        return end.IsKind(SyntaxKind.None) || end.IsMissing ? declaration.GetLastToken() : end;
    }

    private static SyntaxToken? Present(SyntaxToken token) =>
        token.IsKind(SyntaxKind.None) || token.IsMissing ? null : token;

    private static bool OpensWithMarker(string comment)
    {
        foreach (string marker in Markers)
        {
            if (comment.StartsWith(marker, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private static int LineOf(SyntaxTree tree, TextSpan span) =>
        tree.GetLineSpan(span).StartLinePosition.Line;

    private static bool IsBlank(string text)
    {
        foreach (char character in text)
        {
            if (!char.IsWhiteSpace(character))
                return false;
        }

        return true;
    }

    private static string IndentOf(string line) => AssuranceLines.IndentOf(line);
}
