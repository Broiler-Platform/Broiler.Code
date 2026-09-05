using System;

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

    /// <summary>Where a unit's code came from.</summary>
    public static readonly string[] OriginValues =
        ["Original", "AI", "Specification", "Derived", "Ported", "ThirdParty"];

    /// <summary>Intellectual-property risk, weakest claim last.</summary>
    public static readonly string[] IpRiskValues =
        ["None", "Low", "Medium", "High", "Unknown"];

    /// <summary>Security risk, weakest claim last.</summary>
    public static readonly string[] SecurityRiskValues =
        ["None", "Low", "Medium", "High", "Critical"];

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

    /// <summary>
    /// True when a name may stand as a reviewer on a human line.
    ///
    /// The owning component keeps no list of permitted reviewers, so there is no
    /// roster to check a name against and this refuses nothing on the grounds of
    /// who someone is. What it refuses is a name the format could not carry back:
    ///
    /// <list type="bullet">
    /// <item><c>;</c> separates the parts of the line, so a name containing one
    /// would come back as a different name with a field after it.</item>
    /// <item><c>=</c> is what a field looks like, and that component reads a
    /// first part containing one as a field rather than as a reviewer — its
    /// generator then refuses to touch the line at all.</item>
    /// <item><c>@</c> separates the name from the fingerprint when staleness is
    /// later recorded as <c>Previous=name@fingerprint</c>.</item>
    /// <item>A line break, which would end the comment.</item>
    /// <item>Either reserved word, which names a state rather than a person.</item>
    /// <item>Nothing visible. A zero-width space is not whitespace to
    /// <c>char.IsWhiteSpace</c>, so a body made only of format characters would
    /// pass an emptiness check and be sealed into an approval attributed to a
    /// name nobody can see, read out or type again.</item>
    /// </list>
    ///
    /// Refusing costs one message naming the name. Writing one costs a line that
    /// component's generator throws on, which is a review nobody can seal.
    /// </summary>
    public static bool IsWritableReviewer(string? reviewer)
    {
        if (string.IsNullOrWhiteSpace(reviewer))
            return false;

        string trimmed = reviewer.Trim();
        if (trimmed.AsSpan().IndexOfAny(";=@") >= 0)
            return false;

        if (trimmed.AsSpan().IndexOfAny('\r', '\n') >= 0)
            return false;

        if (!HasVisibleCharacter(trimmed))
            return false;

        return !trimmed.StartsWith(Pending, StringComparison.Ordinal) &&
            !trimmed.StartsWith(Stale, StringComparison.Ordinal);
    }

    private static bool HasVisibleCharacter(string value)
    {
        foreach (char character in value)
        {
            if (char.IsWhiteSpace(character) || char.IsControl(character))
                continue;

            // Format characters — zero-width spaces, joiners, the byte-order
            // mark — occupy no space and name nobody.
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character) ==
                System.Globalization.UnicodeCategory.Format)
            {
                continue;
            }

            return true;
        }

        return false;
    }

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
