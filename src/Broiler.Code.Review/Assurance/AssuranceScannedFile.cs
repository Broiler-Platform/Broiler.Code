using System.Collections.Generic;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// One preprocessor directive, as the parser saw it.
/// </summary>
/// <param name="Line">Zero-based line the directive starts on.</param>
/// <param name="Text">The directive, trimmed: <c>#if DEBUG</c>, <c>#nullable enable</c>.</param>
public readonly record struct AssuranceDirective(int Line, string Text);

/// <summary>
/// One code unit, with what a tool needs to write text next to it.
///
/// The editor's seam, <see cref="IAssuranceUnitScanner"/>, gives name, lines and
/// fingerprint. A tool that inserts annotation blocks also needs to know where
/// the declaration sits on its line, and which assurance comments the parser
/// already attaches to it. Those questions are answered from syntax trivia here,
/// not by scanning lines, so that a marker inside a string literal is never
/// taken for a comment.
/// </summary>
/// <param name="Unit">The unit as <see cref="IAssuranceUnitScanner"/> reports it.</param>
/// <param name="Indent">
/// The whitespace in front of the declaration's first token, exactly as written.
/// When the declaration does not start its own line, this is the indent of the
/// line it is on.
/// </param>
/// <param name="StartsOwnLine">
/// True when nothing but whitespace stands before the declaration's first token
/// on its line. A block can only be inserted above such a declaration.
/// </param>
/// <param name="AnnotationLine">
/// Zero-based line of the first single-line comment in the declaration's leading
/// trivia that opens with <c>// Broiler-AI:</c>, or null. This is the block the
/// owning component attaches to the unit. Whether it parses is a separate
/// question.
/// </param>
/// <param name="LeadingAssuranceLines">
/// Zero-based lines of every single-line comment in the declaration's leading
/// trivia that opens with one of the three markers, in order.
/// </param>
/// <param name="HeaderEndLine">
/// Zero-based line of the token where the declaration's header ends: the opening
/// brace, the <c>=&gt;</c>, the initializer's <c>=</c> or the closing
/// <c>;</c>. An assurance comment between <see cref="AssuranceScannedUnit.DeclarationLine"/>
/// and this line sits inside the declaration, below its first token, where
/// nothing attaches it.
/// </param>
public sealed record AssuranceFileUnit(
    AssuranceScannedUnit Unit,
    string Indent,
    bool StartsOwnLine,
    int? AnnotationLine,
    IReadOnlyList<int> LeadingAssuranceLines,
    int HeaderEndLine);

/// <summary>
/// Everything the assurance tooling reads out of one parsed file.
/// </summary>
/// <param name="Units">The code units, in document order (a type before its members).</param>
/// <param name="FileFingerprint">The fingerprint over the whole compilation unit.</param>
/// <param name="Directives">Every preprocessor directive, in document order.</param>
/// <param name="AssuranceCommentLines">
/// Zero-based lines holding a single-line comment that opens with one of the
/// three markers and is the first thing on its line. Found as comment trivia, so
/// the same text inside a string or disabled code is not counted.
/// </param>
/// <param name="LineCount">
/// How many lines the parser counts. The parser also breaks lines on U+0085,
/// U+2028 and U+2029, and <see cref="AssuranceLines"/> does not. When the two
/// counts differ, the parser's line numbers do not index <see cref="AssuranceLines"/>,
/// and nothing may be written into the file by line number.
/// </param>
public sealed record AssuranceScannedFile(
    IReadOnlyList<AssuranceFileUnit> Units,
    string FileFingerprint,
    IReadOnlyList<AssuranceDirective> Directives,
    IReadOnlyList<int> AssuranceCommentLines,
    int LineCount);

/// <summary>
/// Scans a whole file for the command-line tool.
///
/// A second seam beside <see cref="IAssuranceUnitScanner"/> rather than a wider
/// version of it, so that the editor's seam and its implementations stay as they
/// are. Implementations must agree with the owning component token for token,
/// exactly as that interface's must.
/// </summary>
public interface IAssuranceFileScanner
{
    /// <summary>
    /// Parses <paramref name="text"/> (already decoded, with no byte-order mark)
    /// and reports its units, fingerprint, directives and assurance comments.
    /// </summary>
    AssuranceScannedFile ScanFile(string text, string path);
}
