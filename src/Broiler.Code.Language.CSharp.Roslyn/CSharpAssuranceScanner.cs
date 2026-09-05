using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Broiler.Code.Review.Assurance;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.Code.Language.CSharp.Roslyn;

/// <summary>
/// Finds the code units of a C# file and fingerprints them the way the component
/// that owns the Broiler Code Assurance format does.
///
/// This is a deliberate second implementation of somebody else's algorithm, and
/// that is worth stating plainly because a second implementation is usually the
/// wrong answer. The first lives inside the annotated component's own
/// architecture-test assembly, which is a test project in another repository:
/// this editor cannot reference it, and the component's own decision record
/// explains why it will not be extracted into a shared tool until two products
/// need it.
///
/// So the two must agree by construction, and every part of this file exists to
/// make disagreement visible rather than silent. The parse options are part of
/// the algorithm and are copied whole — a file with a conditional region parses
/// differently under the defaults, and its tokens would quietly leave the hash.
/// The fingerprint is <see cref="SyntaxToken.Text"/> joined by one space, which
/// is what keeps every comment, the generated header and the annotation itself
/// out of the value they describe; without that the generator could never reach
/// a fixed point. And the exemption predicate is ported case by case in the same
/// order, because the reason a unit is exempt is reported to a reviewer and a
/// plausible wrong reason is worse than none.
///
/// Nothing here decides a review. It reports what is, and the two things a
/// mistake in it can cost are a unit shown under the wrong heading and a file
/// header this editor then declines to recount — never an approval.
/// </summary>
public sealed class CSharpAssuranceScanner : IAssuranceUnitScanner
{
    /// <summary>
    /// The preprocessor symbols the owning component parses under.
    ///
    /// Part of the algorithm rather than a detail of it. Code inside a region
    /// whose symbol is undefined parses as disabled text, and disabled text
    /// carries no tokens — so a scanner using the defaults would hash shipping
    /// code as though it were absent and would agree with nobody.
    /// </summary>
    private static readonly string[] PreprocessorSymbols =
    [
        "NET",
        "NETCOREAPP",
        "NETCOREAPP1_0_OR_GREATER",
        "NETCOREAPP1_1_OR_GREATER",
        "NETCOREAPP2_0_OR_GREATER",
        "NETCOREAPP2_1_OR_GREATER",
        "NETCOREAPP2_2_OR_GREATER",
        "NETCOREAPP3_0_OR_GREATER",
        "NETCOREAPP3_1_OR_GREATER",
        "NET5_0_OR_GREATER",
        "NET6_0_OR_GREATER",
        "NET7_0_OR_GREATER",
        "NET8_0_OR_GREATER",
        "NET9_0_OR_GREATER",
        "NET10_0_OR_GREATER",
        "NET10_0",
        "DEBUG",
        "RELEASE",
        "TRACE",
    ];

    private static readonly CSharpParseOptions ParseOptions = CSharpParseOptions.Default
        .WithLanguageVersion(LanguageVersion.Latest)
        .WithPreprocessorSymbols(PreprocessorSymbols);

    /// <inheritdoc/>
    public IReadOnlyList<AssuranceScannedUnit> Scan(string text, string path)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(path);

        SyntaxTree tree = CSharpSyntaxTree.ParseText(text, ParseOptions, path: path);
        SyntaxNode root = tree.GetRoot();

        var units = new List<AssuranceScannedUnit>();
        foreach (MemberDeclarationSyntax declaration in root
            .DescendantNodes()
            .OfType<MemberDeclarationSyntax>()
            .Where(IsCodeUnit))
        {
            FileLinePositionSpan span = tree.GetLineSpan(declaration.Span);
            AssuranceExemption exemption = ExemptionFor(declaration);

            units.Add(new AssuranceScannedUnit(
                NameOf(declaration),
                MemberNameOf(declaration) + ParametersOf(declaration),
                span.StartLinePosition.Line,
                span.EndLinePosition.Line,
                exemption != AssuranceExemption.None,
                exemption.ToString(),
                Fingerprint(declaration)));
        }

        return units;
    }

    /// <summary>
    /// The fingerprint of one declaration: SHA-256 over its token texts joined by
    /// a single space, uppercase hex, first six characters.
    ///
    /// <see cref="SyntaxToken.Text"/> and not <c>ToFullString</c>, which is the
    /// whole mechanism. Text is a token's own characters and never the trivia
    /// around it, so every comment is outside the value — the assurance
    /// annotation included. A fingerprint that covered its own annotation could
    /// never be written down, because writing it would change it.
    ///
    /// Nothing inside a token is canonicalized either. <c>1_000</c> and
    /// <c>1000</c> are different fingerprints on purpose: how a literal is
    /// spelled is part of the source, and the conservative answer is the useful
    /// one for a question about whether code changed.
    /// </summary>
    public static string Fingerprint(SyntaxNode declaration)
    {
        ArgumentNullException.ThrowIfNull(declaration);

        byte[] digest = SHA256.HashData(Encoding.UTF8.GetBytes(TokenStream(declaration)));
        return Convert.ToHexString(digest)[..AssuranceVocabulary.FingerprintWidth];
    }

    /// <summary>
    /// The exact string a fingerprint is taken over.
    ///
    /// Exposed because a reviewer told only that six hex characters moved has
    /// been told nothing they can act on. What changed is answerable from this,
    /// and a pane that can show it is worth more than one that cannot.
    /// </summary>
    public static string TokenStream(SyntaxNode declaration)
    {
        ArgumentNullException.ThrowIfNull(declaration);

        return string.Join(" ", Tokens(declaration).Select(static token => token.Text));
    }

    /// <summary>
    /// A whole file's fingerprint, over the compilation unit.
    ///
    /// The root is not a type declaration, so it takes the whole-node branch —
    /// which includes the end-of-file token. That token's text is empty and its
    /// separator is not, so every file's stream ends with a space. It looks like
    /// a defect and is load-bearing: drop it and no file fingerprint matches.
    /// </summary>
    public static string FingerprintOfFile(string text, string path)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(path);

        return Fingerprint(CSharpSyntaxTree.ParseText(text, ParseOptions, path: path).GetRoot());
    }

    /// <summary>
    /// Which tokens a declaration contributes.
    ///
    /// A type declaration contributes only its header, because its members are
    /// units with fingerprints of their own and counting them twice would make
    /// every edit anywhere in a class move the class's own value. An enum is not
    /// a type declaration in this sense and contributes everything: its members
    /// are its content, and the vocabulary is the reviewable thing.
    /// </summary>
    private static IEnumerable<SyntaxToken> Tokens(SyntaxNode declaration) => declaration switch
    {
        TypeDeclarationSyntax type => HeaderTokens(type),
        _ => declaration.DescendantTokens(),
    };

    /// <summary>
    /// Everything before the opening brace. A record declared without a body has
    /// no brace to stop at, so the whole declaration is its header — which is
    /// correct, and is why this takes while rather than searching for a brace it
    /// would then fail to find.
    /// </summary>
    private static IEnumerable<SyntaxToken> HeaderTokens(TypeDeclarationSyntax type) =>
        type.ChildNodesAndTokens()
            .TakeWhile(static child => !child.IsKind(SyntaxKind.OpenBraceToken))
            .SelectMany(static child => child.IsToken
                ? Enumerable.Repeat(child.AsToken(), 1)
                : child.AsNode()!.DescendantTokens());

    /// <summary>
    /// What counts as a code unit: a whitelist, not a pattern.
    ///
    /// A declaration this does not name is in no unit at all, so it has no
    /// fingerprint and no record of any kind — which is why the list reaches
    /// bodiless members, fields, enum members and type declarations rather than
    /// only the things with executable bodies. A local function is not here and
    /// needs no entry: it is a statement, and its tokens are already inside the
    /// member that declares it.
    ///
    /// A namespace is deliberately absent. It declares nothing, and its leading
    /// trivia is where the generated file header lives.
    /// </summary>
    private static bool IsCodeUnit(MemberDeclarationSyntax member) => member switch
    {
        MethodDeclarationSyntax or
        ConstructorDeclarationSyntax or
        DestructorDeclarationSyntax or
        OperatorDeclarationSyntax or
        ConversionOperatorDeclarationSyntax or
        PropertyDeclarationSyntax or
        IndexerDeclarationSyntax or
        EventDeclarationSyntax => true,
        BaseFieldDeclarationSyntax => true,
        EnumMemberDeclarationSyntax => true,
        DelegateDeclarationSyntax => true,
        BaseTypeDeclarationSyntax => true,
        _ => false,
    };

    /// <summary>
    /// The eight exemption cases, tried in the order the owning component writes
    /// them.
    ///
    /// The order carries no policy — the cases are close to disjoint — but it is
    /// fixed, so the reason reported for a unit is the same one that component
    /// reports. A reviewer told "exempt: field declaring storage" when the other
    /// tool says "compiler-supplied record member" has been given two answers to
    /// one question.
    ///
    /// The source's own <c>EXEMPT=</c> escape hatch is not here. It lives on the
    /// annotation, which this scanner does not read: the model above it applies
    /// that reason after attaching the block, so there is one place that knows
    /// about annotations and one that knows about syntax.
    /// </summary>
    private static AssuranceExemption ExemptionFor(MemberDeclarationSyntax declaration)
    {
        // Case 6 — inside a marker type. A property of where the member lives
        // rather than of what it says, so it is answered first.
        if (ContainingTypes(declaration).Any(static type =>
                string.Equals(type.Identifier.ValueText, "AssemblyMarker", StringComparison.Ordinal)))
        {
            return AssuranceExemption.InsideAssemblyMarker;
        }

        // Case 8 — one member of an enum. Answered before case 4 because a
        // member of an enum is written by the source, and case 4 is about what
        // the compiler writes: reporting it as compiler-supplied would be a
        // false reason on a true answer.
        if (declaration is EnumMemberDeclarationSyntax)
            return AssuranceExemption.EnumMemberOfADeclaredVocabulary;

        // Case 7 — a field that is not a fixed value. It declares storage, and
        // the members that write it are reviewed. FieldDeclarationSyntax and not
        // the base type: an event field declares a broadcast point rather than
        // storage, and stays relevant.
        if (declaration is FieldDeclarationSyntax field && !IsFixedValue(field))
            return AssuranceExemption.FieldDeclaringStorage;

        // Case 4 — a member of a record or an enum that the compiler writes. A
        // type or a delegate declared inside a record is not one of those, and
        // without the first line a whole nested type header would be exempt.
        if (declaration is not BaseTypeDeclarationSyntax and not DelegateDeclarationSyntax &&
            ContainingTypes(declaration).FirstOrDefault() is RecordDeclarationSyntax or EnumDeclarationSyntax &&
            !SuppliesAnImplementation(declaration))
        {
            return AssuranceExemption.CompilerSuppliedRecordOrEnumMember;
        }

        // Case 1 — an auto-property, or accessors that only return or assign the
        // corresponding member.
        if (declaration is BasePropertyDeclarationSyntax property && IsTrivialProperty(property))
            return AssuranceExemption.TrivialPropertyOrAccessor;

        // Case 2 — a constructor that only assigns its parameters.
        if (declaration is ConstructorDeclarationSyntax constructor && AssignsParametersOnly(constructor))
            return AssuranceExemption.ParameterAssigningConstructor;

        // Case 3 — an expression body that is the corresponding member, a
        // forwarding call, a constant, or a throw.
        if (ArrowBody(declaration) is { } arrow &&
            (IsCorrespondingMemberAccess(arrow, SimpleNameOf(declaration)) ||
             IsDelegationToOwnMember(arrow, declaration) ||
             IsConstant(arrow) ||
             IsThrowNew(arrow)))
        {
            return AssuranceExemption.TrivialExpressionBodiedMember;
        }

        // Case 5 — ToString, GetHashCode, Equals or an operator that only hands
        // its question to another member. The qualifier is the case: an Equals
        // that compares fields itself is a decision about equality.
        if (IsOverrideOrOperator(declaration) && OnlyDelegates(declaration))
            return AssuranceExemption.DelegatingOverrideOrOperator;

        return AssuranceExemption.None;
    }

    /// <summary>
    /// A field whose value is the reviewable thing: const, or static readonly,
    /// and stating a value.
    /// </summary>
    private static bool IsFixedValue(FieldDeclarationSyntax field)
    {
        bool isConstant = field.Modifiers.Any(SyntaxKind.ConstKeyword);
        bool isStaticReadOnly = field.Modifiers.Any(SyntaxKind.StaticKeyword) &&
            field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);

        return (isConstant || isStaticReadOnly) &&
            field.Declaration.Variables.Any(static variable => variable.Initializer is not null);
    }

    private static bool IsTrivialProperty(BasePropertyDeclarationSyntax property)
    {
        string? name = SimpleNameOf(property);

        if (property is PropertyDeclarationSyntax { ExpressionBody: { } arrow })
            return IsCorrespondingMemberAccess(arrow.Expression, name);

        if (property.AccessorList is null)
            return false;

        return property.AccessorList.Accessors.All(accessor =>
        {
            // No body at all is compiler-supplied: an auto-property.
            if (accessor.Body is null && accessor.ExpressionBody is null)
                return true;

            if (accessor.ExpressionBody is { } body)
            {
                return IsCorrespondingMemberAccess(body.Expression, name) ||
                    IsFieldAssignmentFromValue(body.Expression, name);
            }

            return accessor.Body!.Statements is [var only] && only switch
            {
                ReturnStatementSyntax { Expression: { } returned } =>
                    IsCorrespondingMemberAccess(returned, name),
                ExpressionStatementSyntax statement =>
                    IsFieldAssignmentFromValue(statement.Expression, name),
                _ => false,
            };
        });
    }

    /// <summary>
    /// A member access naming the member that corresponds to
    /// <paramref name="name"/>.
    ///
    /// The correspondence is the case rather than a refinement of it. A property
    /// that returns <em>some</em> field is trivial only if it returns
    /// <em>its own</em>; re-pointing a property at a different field is a
    /// decision, and a decision is what this system exists to put in front of a
    /// person.
    /// </summary>
    private static bool IsCorrespondingMemberAccess(ExpressionSyntax expression, string? name) =>
        name is not null &&
        IsSingleMemberAccess(expression) &&
        Corresponds(AssignedMemberName(expression), name);

    private static bool IsFieldAssignmentFromValue(ExpressionSyntax expression, string? name) =>
        expression is AssignmentExpressionSyntax assignment &&
        assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) &&
        IsCorrespondingMemberAccess(assignment.Left, name) &&
        assignment.Right is IdentifierNameSyntax { Identifier.ValueText: "value" };

    /// <summary>
    /// The simple name a declaration publishes, or null. An operator, a
    /// conversion, an indexer, a constructor and a destructor have none, so
    /// nothing corresponds to them and every borderline shape on them is
    /// answered relevant.
    /// </summary>
    private static string? SimpleNameOf(MemberDeclarationSyntax declaration) => declaration switch
    {
        MethodDeclarationSyntax method => method.Identifier.ValueText,
        PropertyDeclarationSyntax property => property.Identifier.ValueText,
        EventDeclarationSyntax @event => @event.Identifier.ValueText,
        _ => null,
    };

    /// <summary>
    /// A constructor assigning each parameter, at most once, to the member that
    /// corresponds to it.
    ///
    /// Each parameter at most once, and each to its own member: a body that
    /// permutes two assignments is exactly the change this case would otherwise
    /// stop anyone from checking.
    /// </summary>
    private static bool AssignsParametersOnly(ConstructorDeclarationSyntax constructor)
    {
        // A chained constructor runs code this predicate is not looking at. An
        // argument-free hop runs nothing worth reviewing.
        if (constructor.Initializer is { ArgumentList.Arguments.Count: > 0 })
            return false;

        IReadOnlyList<ExpressionSyntax>? assignments;
        if (constructor.ExpressionBody is { } arrow)
        {
            assignments = [arrow.Expression];
        }
        else if (constructor.Body is null)
        {
            return false;
        }
        else
        {
            assignments = OnlyExpressions(constructor.Body);
        }

        if (assignments is null)
            return false;

        var assigned = new HashSet<string>(StringComparer.Ordinal);
        foreach (ExpressionSyntax expression in assignments)
        {
            if (expression is not AssignmentExpressionSyntax assignment ||
                !assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) ||
                !IsSingleMemberAccess(assignment.Left) ||
                assignment.Right is not IdentifierNameSyntax right)
            {
                return false;
            }

            ParameterSyntax? parameter = constructor.ParameterList.Parameters.FirstOrDefault(parameter =>
                string.Equals(
                    parameter.Identifier.ValueText, right.Identifier.ValueText, StringComparison.Ordinal));

            if (parameter is null ||
                !Corresponds(AssignedMemberName(assignment.Left), parameter.Identifier.ValueText) ||
                !assigned.Add(parameter.Identifier.ValueText))
            {
                return false;
            }
        }

        return true;

        static IReadOnlyList<ExpressionSyntax>? OnlyExpressions(BlockSyntax body)
        {
            var expressions = new List<ExpressionSyntax>();
            foreach (StatementSyntax statement in body.Statements)
            {
                if (statement is not ExpressionStatementSyntax expression)
                    return null;

                expressions.Add(expression.Expression);
            }

            return expressions;
        }
    }

    /// <summary>The last identifier of <c>X</c>, <c>this.X</c> or <c>A.B.X</c>.</summary>
    private static string AssignedMemberName(ExpressionSyntax left) => Unwrap(left) switch
    {
        IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
        MemberAccessExpressionSyntax { Name: IdentifierNameSyntax name } => name.Identifier.ValueText,
        _ => string.Empty,
    };

    /// <summary>
    /// Whether a member name and a parameter name are the same name under the
    /// convention: a leading underscore is decoration and the leading capital is
    /// a casing rule. Anything the convention does not cover is answered
    /// relevant.
    /// </summary>
    private static bool Corresponds(string member, string parameter) =>
        member.Length > 0 &&
        parameter.Length > 0 &&
        string.Equals(
            member.TrimStart('_'),
            parameter.TrimStart('_'),
            StringComparison.OrdinalIgnoreCase);

    /// <summary>A name, a <c>this</c>, or a dotted chain of them. No call, no operator, no argument.</summary>
    private static bool IsSingleMemberAccess(ExpressionSyntax expression) => Unwrap(expression) switch
    {
        IdentifierNameSyntax => true,
        ThisExpressionSyntax => true,
        MemberAccessExpressionSyntax member =>
            IsSingleMemberAccess(member.Expression) && member.Name is IdentifierNameSyntax,
        _ => false,
    };

    /// <summary>
    /// A call to another member of the same type, forwarding only arguments the
    /// caller chose.
    ///
    /// The argument rule is the case. Anything a member <em>supplies</em> — a
    /// literal, an enum member, a field — is a decision that member is making,
    /// and a delegation that supplies a value is reviewed like any other. Only a
    /// member that hands its own arguments on unchanged decides nothing.
    /// </summary>
    private static bool IsDelegationToOwnMember(ExpressionSyntax expression, SyntaxNode declaration)
    {
        if (Unwrap(expression) is not InvocationExpressionSyntax invocation)
            return false;

        if (!invocation.ArgumentList.Arguments.All(argument =>
                IsForwardedParameter(argument.Expression, declaration)))
        {
            return false;
        }

        string? ownType = ContainingTypes(declaration).FirstOrDefault()?.Identifier.ValueText;

        return invocation.Expression switch
        {
            IdentifierNameSyntax or GenericNameSyntax => true,
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } => true,
            MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax qualifier } =>
                ownType is not null &&
                string.Equals(qualifier.Identifier.ValueText, ownType, StringComparison.Ordinal),
            _ => false,
        };
    }

    private static bool IsForwardedParameter(ExpressionSyntax expression, SyntaxNode declaration)
    {
        ExpressionSyntax unwrapped = Unwrap(expression);

        if (unwrapped is ThisExpressionSyntax)
            return true;

        if (unwrapped is not IdentifierNameSyntax identifier)
            return false;

        SeparatedSyntaxList<ParameterSyntax> parameters = declaration switch
        {
            BaseMethodDeclarationSyntax method => method.ParameterList.Parameters,
            IndexerDeclarationSyntax indexer => indexer.ParameterList.Parameters,

            // A property or event accessor has one implicit parameter, called value.
            _ => default,
        };

        if (parameters.Count == 0)
        {
            return declaration is BasePropertyDeclarationSyntax &&
                string.Equals(identifier.Identifier.ValueText, "value", StringComparison.Ordinal);
        }

        return parameters.Any(parameter => string.Equals(
            parameter.Identifier.ValueText, identifier.Identifier.ValueText, StringComparison.Ordinal));
    }

    private static bool IsConstant(ExpressionSyntax expression) => Unwrap(expression) switch
    {
        LiteralExpressionSyntax => true,
        PrefixUnaryExpressionSyntax unary when unary.IsKind(SyntaxKind.UnaryMinusExpression) ||
            unary.IsKind(SyntaxKind.UnaryPlusExpression) => IsConstant(unary.Operand),
        InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" } } => true,
        DefaultExpressionSyntax => true,
        _ => false,
    };

    private static bool IsThrowNew(ExpressionSyntax expression) =>
        Unwrap(expression) is ThrowExpressionSyntax { Expression: BaseObjectCreationExpressionSyntax };

    private static bool IsOverrideOrOperator(MemberDeclarationSyntax declaration) => declaration switch
    {
        MethodDeclarationSyntax method =>
            method.Identifier.ValueText is "ToString" or "GetHashCode" or "Equals",
        OperatorDeclarationSyntax => true,
        ConversionOperatorDeclarationSyntax => true,
        _ => false,
    };

    /// <summary>
    /// One expression built only from calls, names, literals, type tests and the
    /// two short-circuit operators, containing at least one call.
    ///
    /// A whitelist, because the question is what the expression may <em>not</em>
    /// contain. Logical negation is deliberately absent: <c>!Equals(other)</c> is
    /// not a delegation to Equals, it is the opposite decision, and which way
    /// round it is is the whole content of an inequality operator.
    /// </summary>
    private static bool OnlyDelegates(MemberDeclarationSyntax declaration)
    {
        ExpressionSyntax? expression = ArrowBody(declaration) ?? SingleReturnedExpression(declaration);
        if (expression is null)
            return false;

        bool permitted = expression.DescendantNodesAndSelf().All(static node => node switch
        {
            InvocationExpressionSyntax => true,
            MemberAccessExpressionSyntax => true,
            ArgumentListSyntax or ArgumentSyntax => true,
            ParenthesizedExpressionSyntax => true,
            ThisExpressionSyntax or BaseExpressionSyntax => true,
            LiteralExpressionSyntax => true,
            IsPatternExpressionSyntax => true,
            DeclarationPatternSyntax or SingleVariableDesignationSyntax => true,
            BinaryExpressionSyntax binary =>
                binary.IsKind(SyntaxKind.LogicalAndExpression) || binary.IsKind(SyntaxKind.LogicalOrExpression),
            TypeSyntax => true,
            TypeArgumentListSyntax => true,
            _ => false,
        });

        return permitted && expression.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Any();
    }

    private static ExpressionSyntax? ArrowBody(MemberDeclarationSyntax declaration) => declaration switch
    {
        MethodDeclarationSyntax method => method.ExpressionBody?.Expression,
        ConstructorDeclarationSyntax constructor => constructor.ExpressionBody?.Expression,
        DestructorDeclarationSyntax destructor => destructor.ExpressionBody?.Expression,
        OperatorDeclarationSyntax @operator => @operator.ExpressionBody?.Expression,
        ConversionOperatorDeclarationSyntax conversion => conversion.ExpressionBody?.Expression,
        PropertyDeclarationSyntax property => property.ExpressionBody?.Expression,
        IndexerDeclarationSyntax indexer => indexer.ExpressionBody?.Expression,
        _ => null,
    };

    private static ExpressionSyntax? SingleReturnedExpression(MemberDeclarationSyntax declaration) =>
        declaration switch
        {
            MethodDeclarationSyntax { Body.Statements: [ReturnStatementSyntax { Expression: { } only }] } => only,
            OperatorDeclarationSyntax { Body.Statements: [ReturnStatementSyntax { Expression: { } only }] } => only,
            ConversionOperatorDeclarationSyntax
            { Body.Statements: [ReturnStatementSyntax { Expression: { } only }] } => only,
            _ => null,
        };

    /// <summary>True when the source, rather than the compiler, says what this member does.</summary>
    private static bool SuppliesAnImplementation(MemberDeclarationSyntax declaration)
    {
        if (ArrowBody(declaration) is not null)
            return true;

        // An initializer is an implementation the source supplies, so a constant
        // inside a record is not one of the members the compiler writes.
        if (declaration is FieldDeclarationSyntax field)
            return field.Declaration.Variables.Any(static variable => variable.Initializer is not null);

        // The compiler writes no event into a record, so an event field inside
        // one is the source's own, initializer or not.
        if (declaration is EventFieldDeclarationSyntax)
            return true;

        if (declaration is BaseMethodDeclarationSyntax { Body: not null })
            return true;

        return declaration is BasePropertyDeclarationSyntax { AccessorList: { } accessors } &&
            accessors.Accessors.Any(static accessor =>
                accessor.Body is not null || accessor.ExpressionBody is not null);
    }

    private static ExpressionSyntax Unwrap(ExpressionSyntax expression) =>
        expression is ParenthesizedExpressionSyntax parenthesized
            ? Unwrap(parenthesized.Expression)
            : expression;

    private static IEnumerable<BaseTypeDeclarationSyntax> ContainingTypes(SyntaxNode node) =>
        node.Ancestors().OfType<BaseTypeDeclarationSyntax>();

    /// <summary>
    /// The qualified, signature-shaped name: namespace, containing types
    /// outermost first, then the member and its parameters.
    ///
    /// The qualifier joins only the parts that exist. A top-level type has no
    /// containing type, and joining an empty one in produces a name with a hole
    /// in it that nothing can be matched against by eye.
    /// </summary>
    private static string NameOf(MemberDeclarationSyntax declaration)
    {
        string owner = string.Join(
            ".",
            ContainingTypes(declaration).Reverse().Select(static type => type.Identifier.ValueText));

        string? space = declaration.Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Select(static @namespace => @namespace.Name.ToString())
            .FirstOrDefault();

        string qualified = string.Join(
            ".",
            new[] { space, owner }.Where(static part => !string.IsNullOrEmpty(part)));

        return $"{qualified}.{MemberNameOf(declaration)}{ParametersOf(declaration)}";
    }

    /// <summary>
    /// The member part of the name, without the qualifier.
    ///
    /// An explicit interface implementation carries its specifier, because it is
    /// a different member: two declarations in one type whose names differed only
    /// by the specifier would otherwise be indistinguishable in a report and
    /// unaddressable in a manifest.
    /// </summary>
    private static string MemberNameOf(MemberDeclarationSyntax declaration) => declaration switch
    {
        MethodDeclarationSyntax method =>
            Explicit(method.ExplicitInterfaceSpecifier) +
            method.Identifier.ValueText + Generics(method.TypeParameterList),
        ConstructorDeclarationSyntax constructor => constructor.Identifier.ValueText,
        DestructorDeclarationSyntax destructor => "~" + destructor.Identifier.ValueText,
        OperatorDeclarationSyntax @operator => "operator " + @operator.OperatorToken.ValueText,
        ConversionOperatorDeclarationSyntax conversion => "operator " + conversion.Type,
        PropertyDeclarationSyntax property =>
            Explicit(property.ExplicitInterfaceSpecifier) + property.Identifier.ValueText,
        IndexerDeclarationSyntax indexer => Explicit(indexer.ExplicitInterfaceSpecifier) + "this[]",
        EventDeclarationSyntax @event =>
            Explicit(@event.ExplicitInterfaceSpecifier) + @event.Identifier.ValueText,
        BaseFieldDeclarationSyntax field => string.Join(
            ", ",
            field.Declaration.Variables.Select(static variable => variable.Identifier.ValueText)),
        EnumMemberDeclarationSyntax entry => entry.Identifier.ValueText,
        DelegateDeclarationSyntax @delegate =>
            @delegate.Identifier.ValueText + Generics(@delegate.TypeParameterList),
        TypeDeclarationSyntax type => type.Identifier.ValueText + Generics(type.TypeParameterList),
        BaseTypeDeclarationSyntax type => type.Identifier.ValueText,
        _ => "<member>",
    };

    /// <summary>
    /// The parameter list, rendered only for a method-shaped declaration.
    ///
    /// Modifiers are part of it because they are part of the signature: two
    /// overloads differing only by <c>out</c> are two units, and a name that
    /// dropped the keyword would make one of them unreachable.
    /// </summary>
    private static string ParametersOf(MemberDeclarationSyntax declaration) =>
        declaration is BaseMethodDeclarationSyntax method
            ? "(" + string.Join(
                ", ",
                method.ParameterList.Parameters.Select(static parameter => string.Concat(
                    parameter.Modifiers.Select(static modifier => modifier.ValueText + " ")) +
                    (parameter.Type?.ToString() ?? "?"))) + ")"
            : string.Empty;

    private static string Generics(TypeParameterListSyntax? parameters) => parameters is null
        ? string.Empty
        : "<" + string.Join(",", parameters.Parameters.Select(static p => p.Identifier.ValueText)) + ">";

    private static string Explicit(ExplicitInterfaceSpecifierSyntax? specifier) =>
        specifier is null ? string.Empty : specifier.Name.ToString() + ".";

    /// <summary>
    /// Which of the eight cases, if any, covers a declaration. The names are the
    /// owning component's, so a reader can hold the two against each other one
    /// line at a time.
    /// </summary>
    private enum AssuranceExemption
    {
        None = 0,
        TrivialPropertyOrAccessor,
        ParameterAssigningConstructor,
        TrivialExpressionBodiedMember,
        CompilerSuppliedRecordOrEnumMember,
        DelegatingOverrideOrOperator,
        InsideAssemblyMarker,
        FieldDeclaringStorage,
        EnumMemberOfADeclaredVocabulary,
    }
}
