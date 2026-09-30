using System;
using System.Collections.Generic;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// The literals of the Broiler Code Assurance annotation, spelled the way the
/// component that owns the format writes them.
///
/// This editor is not the authority on the format. The authority is the
/// generator that lives inside the annotated component's own architecture-test
/// assembly, and every constant here exists to be byte-compatible with it: a
/// value written one space out of place is a line that component's parser
/// rejects, and a rejected line is a review that never happened.
///
/// The vocabularies are ordered weakest-claim-last, because that is the order
/// the generated file header takes its "worst of" from. Reordering them would
/// silently change what a file reports about itself.
/// </summary>
public static class AssuranceVocabulary
{
    /// <summary>The machine's assessment line.</summary>
    public const string AiMarker = "// Broiler-AI:";

    /// <summary>
    /// The optional middle line: one sentence naming what would prove the unit
    /// wrong. Required by the owning component whenever the unit's security
    /// risk is High or Critical, carried verbatim here and never rewritten.
    /// </summary>
    public const string FalsifiedIfMarker = "// Broiler-Falsified-If:";

    /// <summary>The human's line. The only line in the block a person writes.</summary>
    public const string HumanMarker = "// Broiler-Human:";

    /// <summary>
    /// The column every value in the block starts at, as a width the markers
    /// are padded to.
    ///
    /// Derived from the longest marker rather than written as 24, which is what
    /// the owning component does. A constant would be the same number today and
    /// would drift the moment a marker changed length there.
    /// </summary>
    public static readonly int LabelWidth = FalsifiedIfMarker.Length;

    /// <summary>No human has recorded anything about this unit.</summary>
    public const string Pending = "PENDING";

    /// <summary>
    /// A human approved a version that is no longer the one on disk. Matched by
    /// prefix rather than equality, because the written form carries what was
    /// approved: <c>STALE; Previous=EB@06FA02</c>.
    /// </summary>
    public const string Stale = "STALE";

    /// <summary>
    /// The placeholder a fingerprint field carries until the owning component's
    /// generator fills it in.
    /// </summary>
    public const string ToBeFilled = "TBF";

    /// <summary>The number of hex characters a fingerprint carries.</summary>
    public const int FingerprintWidth = 6;

    /// <summary>The field that names the fingerprint, on either line.</summary>
    public const string FingerprintField = "Fingerprint";

    /// <summary>The field that exempts a unit outright, ahead of every predicate.</summary>
    public const string ExemptField = "EXEMPT";

    /// <summary>
    /// The name of the one unit a file's top-level statements form together.
    /// Angle brackets because no declaration can be called that: it is the
    /// compiler's generated entry point, spelled so a reader sees what it is.
    /// </summary>
    public const string TopLevelStatements = "<top-level statements>";

    /// <summary>Where a unit's code came from.</summary>
    public static readonly string[] OriginValues =
        ["Original", "AI", "Specification", "Derived", "Ported", "ThirdParty"];

    /// <summary>Intellectual-property risk, weakest claim last.</summary>
    public static readonly string[] IpRiskValues =
        ["None", "Low", "Medium", "High", "Unknown"];

    /// <summary>Security risk, weakest claim last.</summary>
    public static readonly string[] SecurityRiskValues =
        ["None", "Low", "Medium", "High", "Critical"];

    /// <summary>The exemption a unit takes from an <c>EXEMPT=</c> field in its own source.</summary>
    public const string DeclaredInSource = "DeclaredInSource";

    /// <summary>
    /// Every exemption case, in the order the owning component declares them and
    /// its report tables list them. <c>None</c>, which is not an exemption, is
    /// left out.
    /// </summary>
    public static readonly string[] ExemptionCases =
    [
        "TrivialPropertyOrAccessor",
        "ParameterAssigningConstructor",
        "TrivialExpressionBodiedMember",
        "CompilerSuppliedRecordOrEnumMember",
        "DelegatingOverrideOrOperator",
        "InsideAssemblyMarker",
        "FieldDeclaringStorage",
        "EnumMemberOfADeclaredVocabulary",
        DeclaredInSource,
    ];

    /// <summary>
    /// Every state the state machine can resolve, in the order the owning
    /// component declares them and its report tables list them.
    /// </summary>
    public static readonly AssuranceUnitState[] States =
    [
        AssuranceUnitState.New,
        AssuranceUnitState.AiAssessed,
        AssuranceUnitState.HumanPending,
        AssuranceUnitState.HumanApprovedPendingFingerprint,
        AssuranceUnitState.Verified,
        AssuranceUnitState.Stale,
        AssuranceUnitState.Exempt,
    ];

    /// <summary>The fields a full assessment carries. <c>Spec</c> is the one defined optional.</summary>
    public static readonly string[] RequiredFields =
        ["Origin", "IP", "Security", "Resources", FingerprintField];

    /// <summary>
    /// The fields a reviewer may state on their own line, beside their name.
    ///
    /// A reviewer who disagrees with the machine's risk assessment records their
    /// own here rather than editing the AI line, so the disagreement survives
    /// the next generation instead of being overwritten by it.
    /// </summary>
    public static readonly string[] HumanFieldMarkers =
        [FingerprintField + "=", "IP=", "Security=", "Resources="];

    /// <summary>
    /// True for a value the owning component would accept as a fingerprint:
    /// exactly six characters, each an uppercase hex digit.
    ///
    /// Uppercase only, deliberately. The value is produced by
    /// <c>Convert.ToHexString</c> there, and a lowercase spelling is the
    /// signature of a reimplementation that used a different formatter.
    /// </summary>
    public static bool IsWellFormedFingerprint(string? value) =>
        value is { Length: FingerprintWidth } &&
        AllHex(value);

    /// <summary>The longest alias a human line may carry.</summary>
    public const int MaxAliasLength = 64;

    /// <summary>
    /// Words that say something about a unit's state, or say that nobody is
    /// there, rather than name a person. Matched case-insensitively against
    /// every word of an alias, words being separated by a space, <c>.</c>,
    /// <c>_</c>, <c>-</c> or <c>'</c>.
    /// </summary>
    private static readonly HashSet<string> PlaceholderWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "PENDING", "STALE", "TBF", "TBD", "TBA", "TBC", "TODO", "FIXME", "XXX", "WIP", "LATER",
        "NONE", "NOBODY", "NOONE", "NOT", "NO", "NA", "NIL", "NULL", "UNKNOWN", "UNASSIGNED",
        "ANONYMOUS", "SOMEONE", "SOMEBODY", "PLACEHOLDER", "MISSING", "SKIP", "SKIPPED", "EXEMPT",
        "REVIEW", "REVIEWS", "REVIEWED", "REVIEWING", "REVIEWER", "UNREVIEWED", "NEEDS", "AWAITING", "WAITING",
        "YES", "OK", "OKAY", "LGTM", "DONE", "APPROVE", "APPROVED", "ACCEPTED", "VERIFIED", "CHECKED",
    };

    /// <summary>
    /// True when <paramref name="value"/> is an alias: something a human line
    /// can name as the person who decided.
    ///
    /// The owning component calls an alias "a bare token" and accepts anything
    /// without an <c>=</c> in its place, which makes <c>NOT REVIEWED</c>,
    /// <c>Pending</c> and <c>PENDING</c> followed by a zero-width space into
    /// reviewers — and the generator then binds each of them to the code as a
    /// sealed approval nobody wrote. So an alias here is a name in shape:
    ///
    /// <list type="bullet">
    /// <item>It opens with a letter and is at most <see cref="MaxAliasLength"/>
    /// characters long.</item>
    /// <item>It holds letters, digits, combining marks, <c>.</c>, <c>_</c>,
    /// <c>-</c> and <c>'</c>, and single spaces between words, because the
    /// editor signs with a person's configured name. Nothing else: <c>;</c>
    /// separates the parts of the line, <c>=</c> is what a field looks like,
    /// <c>@</c> separates the name from the fingerprint in
    /// <c>Previous=name@fingerprint</c>, and a format character such as a
    /// zero-width space occupies no room and names nobody anyone can see.</item>
    /// <item>No word of it is one of the placeholder words: the two reserved
    /// states, and the words a line says "nobody yet" or "fine" with.</item>
    /// </list>
    ///
    /// There is still no roster of permitted reviewers, so nothing is refused
    /// on the grounds of who someone is.
    /// </summary>
    public static bool IsAlias(string? value)
    {
        if (value is null || value.Length is 0 or > MaxAliasLength || !char.IsLetter(value[0]) || value[^1] == ' ')
            return false;

        char previous = '\0';
        foreach (char character in value)
        {
            bool allowed = character == ' '
                ? previous != ' '
                : char.IsLetterOrDigit(character) || character is '.' or '_' or '-' or '\'' ||
                  System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character) is
                      System.Globalization.UnicodeCategory.NonSpacingMark or
                      System.Globalization.UnicodeCategory.SpacingCombiningMark;

            if (!allowed)
                return false;

            previous = character;
        }

        foreach (string word in value.Split([' ', '.', '_', '-', '\''], StringSplitOptions.RemoveEmptyEntries))
        {
            if (PlaceholderWords.Contains(word))
                return false;
        }

        return true;
    }

    /// <summary>
    /// True when a name may stand as a reviewer on a human line: when it is an
    /// alias (<see cref="IsAlias"/>) once trimmed.
    ///
    /// The editor and the generator hold a name to the same rule. A name the
    /// editor wrote and the generator then refused would be a review nobody can
    /// seal; a name the generator accepted and the editor refused would be one
    /// a person could only type by hand. Refusing costs one message naming the
    /// name.
    /// </summary>
    public static bool IsWritableReviewer(string? reviewer) =>
        reviewer is not null && IsAlias(reviewer.Trim());

    private static bool AllHex(string value)
    {
        foreach (char character in value)
        {
            if (character is not (>= '0' and <= '9' or >= 'A' and <= 'F'))
                return false;
        }

        return true;
    }
}
