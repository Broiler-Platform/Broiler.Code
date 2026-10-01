// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   27
// Annotated:        27/27
// Exempt:           0
// Human-reviewed:   0/27
// IP risk:          Low
// Security risk:    High
// Criteria:         26/17
// Resource impact:  1/10 max
// Unverified:       27
//
// GENERATED - DO NOT EDIT MANUALLY

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
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7D094C
// Broiler-Falsified-If: a name made of a placeholder word with an invisible combining mark appended, such as PENDING followed by a variation selector, is accepted as an alias
// Broiler-Human:        PENDING
public static class AssuranceVocabulary
{
    /// <summary>The machine's assessment line.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=0D2482
    // Broiler-Falsified-If: the marker differs by a byte from the one the owning component writes, so a block this tool writes is not attached to its unit there
    // Broiler-Human:        PENDING
    public const string AiMarker = "// Broiler-AI:";

    /// <summary>
    /// The optional middle line: one sentence naming what would prove the unit
    /// wrong. Required by the owning component whenever the unit's security
    /// risk is High or Critical, carried verbatim here and never rewritten.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=08E4C4
    // Broiler-Falsified-If: the marker differs by a byte from the owning component's, so a criterion line this tool writes is not read as one there
    // Broiler-Human:        PENDING
    public const string FalsifiedIfMarker = "// Broiler-Falsified-If:";

    /// <summary>The human's line. The only line in the block a person writes.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=BBB9E0
    // Broiler-Falsified-If: the marker differs by a byte from the owning component's, so a human line the editor writes is not read as one there
    // Broiler-Human:        PENDING
    public const string HumanMarker = "// Broiler-Human:";

    /// <summary>
    /// The column every value in the block starts at, as a width the markers
    /// are padded to.
    ///
    /// Derived from the longest marker rather than written as 24, which is what
    /// the owning component does. A constant would be the same number today and
    /// would drift the moment a marker changed length there.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=279106
    // Broiler-Falsified-If: the width differs from the owning component's padding of 24 columns, so every block this tool writes differs from the one the generator writes
    // Broiler-Human:        PENDING
    public static readonly int LabelWidth = FalsifiedIfMarker.Length;

    /// <summary>No human has recorded anything about this unit.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=CC1510
    // Broiler-Falsified-If: a human line reading exactly PENDING is read as naming a person
    // Broiler-Human:        PENDING
    public const string Pending = "PENDING";

    /// <summary>
    /// A human approved a version that is no longer the one on disk. Matched by
    /// prefix rather than equality, because the written form carries what was
    /// approved: <c>STALE; Previous=EB@06FA02</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=3B0062
    // Broiler-Falsified-If: a human line opening with STALE and a previous name is not recognised as stale and is read as naming a person
    // Broiler-Human:        PENDING
    public const string Stale = "STALE";

    /// <summary>
    /// The placeholder a fingerprint field carries until the owning component's
    /// generator fills it in.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=11804D
    // Broiler-Falsified-If: the placeholder differs from the owning component's, so a human line left for the generator is read as carrying a real fingerprint
    // Broiler-Human:        PENDING
    public const string ToBeFilled = "TBF";

    /// <summary>The number of hex characters a fingerprint carries.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=AD2165
    // Broiler-Falsified-If: a hex value of five or seven characters is accepted as a well-formed fingerprint
    // Broiler-Human:        PENDING
    public const int FingerprintWidth = 6;

    /// <summary>The field that names the fingerprint, on either line.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=6E00F2
    // Broiler-Falsified-If: the field name differs from the owning component's, so a fingerprint stated on a human line is not found and the line reads as left for the generator
    // Broiler-Human:        PENDING
    public const string FingerprintField = "Fingerprint";

    /// <summary>The field that exempts a unit outright, ahead of every predicate.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=BE996E
    // Broiler-Falsified-If: the field name differs from the owning component's, so a unit this tool reads as exempt blocks release there, or the reverse
    // Broiler-Human:        PENDING
    public const string ExemptField = "EXEMPT";

    /// <summary>
    /// The name of the one unit a file's top-level statements form together.
    /// Angle brackets because no declaration can be called that: it is the
    /// compiler's generated entry point, spelled so a reader sees what it is.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C9243E
    // Broiler-Falsified-If: a declaration in source can carry this name, so it and a file's top-level statements share one unit name
    // Broiler-Human:        PENDING
    public const string TopLevelStatements = "<top-level statements>";

    /// <summary>Where a unit's code came from.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=640430
    // Broiler-Falsified-If: a value outside the six origins the owning component defines is accepted, or one of those six is refused
    // Broiler-Human:        PENDING
    public static readonly string[] OriginValues =
        ["Original", "AI", "Specification", "Derived", "Ported", "ThirdParty"];

    /// <summary>Intellectual-property risk, weakest claim last.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A48232
    // Broiler-Falsified-If: the values are reordered, so a file header's worst value is milder than one its units state
    // Broiler-Human:        PENDING
    public static readonly string[] IpRiskValues =
        ["None", "Low", "Medium", "High", "Unknown"];

    /// <summary>Security risk, weakest claim last.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A0A725
    // Broiler-Falsified-If: the values are reordered, so a file header's worst value is milder than one its units state
    // Broiler-Human:        PENDING
    public static readonly string[] SecurityRiskValues =
        ["None", "Low", "Medium", "High", "Critical"];

    /// <summary>The exemption a unit takes from an <c>EXEMPT=</c> field in its own source.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B2B7FA
    // Broiler-Falsified-If: a unit exempted by its own source is reported under a case name other than the one the owning component's report lists
    // Broiler-Human:        PENDING
    public const string DeclaredInSource = "DeclaredInSource";

    /// <summary>
    /// The exemption a named value takes in a component that watches named
    /// values: this tool's case, not the owning component's, and the only one
    /// a configuration switches on.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7E70E7
    // Broiler-Human:        PENDING
    public const string NamedValue = "NamedValue";

    /// <summary>
    /// Every exemption case, in the order the owning component declares them and
    /// its report tables list them, with this tool's <see cref="NamedValue"/>
    /// after them and before <see cref="DeclaredInSource"/>. <c>None</c>, which
    /// is not an exemption, is left out.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=EA965A
    // Broiler-Falsified-If: an exemption case the scanner can report is missing, so the report's exemption table omits the units under it
    // Broiler-Human:        PENDING
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
        NamedValue,
        DeclaredInSource,
    ];

    /// <summary>
    /// Every state the state machine can resolve, in the order the owning
    /// component declares them and its report tables list them.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D24B18
    // Broiler-Falsified-If: a state other than Unknown that the state machine can resolve is missing, so the report's state table omits the units in it
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=1896A9
    // Broiler-Falsified-If: a non-exempt block missing its fingerprint or security field passes the vocabulary check
    // Broiler-Human:        PENDING
    public static readonly string[] RequiredFields =
        ["Origin", "IP", "Security", "Resources", FingerprintField];

    /// <summary>
    /// The fields a reviewer may state on their own line, beside their name.
    ///
    /// A reviewer who disagrees with the machine's risk assessment records their
    /// own here rather than editing the AI line, so the disagreement survives
    /// the next generation instead of being overwritten by it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=D911D4
    // Broiler-Falsified-If: a human line carrying a part after the name that opens with none of the four field prefixes, such as a second name, is read as a defined shape
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=6A4AEF
    // Broiler-Falsified-If: a lowercase hex value, or one of a length other than six, is accepted as a fingerprint
    // Broiler-Human:        PENDING
    public static bool IsWellFormedFingerprint(string? value) =>
        value is { Length: FingerprintWidth } &&
        AllHex(value);

    /// <summary>The longest alias a human line may carry.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=AA3F31
    // Broiler-Falsified-If: a name of 65 characters is accepted as an alias
    // Broiler-Human:        PENDING
    public const int MaxAliasLength = 64;

    /// <summary>
    /// Words that say something about a unit's state, or say that nobody is
    /// there, rather than name a person. Matched case-insensitively against
    /// every word of an alias, words being separated by a space, <c>.</c>,
    /// <c>_</c>, <c>-</c> or <c>'</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=B3440B
    // Broiler-Falsified-If: a name whose only word is NOBODY, TODO or PENDING in mixed case is accepted as an alias
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=083C05
    // Broiler-Falsified-If: a name made of a placeholder word with an invisible combining mark appended, such as PENDING followed by a variation selector, is accepted as an alias
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=9610D6
    // Broiler-Falsified-If: a name that the alias rule refuses once trimmed is accepted for writing on a human line
    // Broiler-Human:        PENDING
    public static bool IsWritableReviewer(string? reviewer) =>
        reviewer is not null && IsAlias(reviewer.Trim());

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=850147
    // Broiler-Falsified-If: a lowercase hex digit or a non-ASCII digit is accepted
    // Broiler-Human:        PENDING
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
