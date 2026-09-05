using System;
using System.Collections.Generic;
using System.Globalization;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// The nine numbers the generated file header reports.
/// </summary>
/// <param name="Relevant">Units in the file that are not exempt.</param>
/// <param name="Annotated">Relevant units carrying an assessment.</param>
/// <param name="Exempt">Units in the file that are exempt. Relevant + Exempt is every unit.</param>
/// <param name="Verified">Relevant units a human approved against the version that is here.</param>
/// <param name="Unverified">Relevant units in a state that blocks a release.</param>
/// <param name="MaxIpRisk">The weakest IP claim any assessment makes, or null when none does.</param>
/// <param name="MaxSecurityRisk">The weakest security claim any assessment makes, or null.</param>
/// <param name="Criteria">Units carrying a falsification criterion.</param>
/// <param name="CriteriaRequired">Units whose security risk demands one.</param>
/// <param name="MaxResources">The largest resource score any assessment states, or null.</param>
public readonly record struct AssuranceSummary(
    int Relevant,
    int Annotated,
    int Exempt,
    int Verified,
    int Unverified,
    string? MaxIpRisk,
    string? MaxSecurityRisk,
    int Criteria,
    int CriteriaRequired,
    int? MaxResources);

/// <summary>
/// The generated block at the top of an annotated file: the licence header, the
/// counts, and the line saying not to edit any of it by hand.
///
/// This editor renders that block for one reason only — to compare. Recording a
/// review changes two of its numbers, so a file left with the old ones would
/// contradict its own annotations until something else ran; but a block written
/// from counts that disagree with the owning component's would be worse than a
/// stale one, because it would look authoritative and be wrong.
///
/// So the block is only ever rewritten when this build can first reproduce the
/// block that is already there, byte for byte, from its own reading of the file.
/// That check is the whole safety argument, and it fails closed: a build that
/// cannot compute fingerprints cannot count verified units, cannot reproduce the
/// header, and therefore never touches it. See
/// <see cref="AssuranceDocument.BannerIsReproducible"/>.
/// </summary>
public static class AssuranceBanner
{
    public const string SpdxCopyrightPrefix = "// SPDX-FileCopyrightText:";

    public const string GeneratedMarker = "// GENERATED - DO NOT EDIT MANUALLY";

    public const string Banner = "// Broiler Code Assurance";

    public const string BannerRule = "// ----------------------";

    /// <summary>The width every label is padded to, so every value starts in the same column.</summary>
    public const int LabelWidth = 18;

    /// <summary>What a row reports when nothing in the file states a value for it.</summary>
    public const string NotAssessed = "not assessed";

    /// <summary>
    /// One row: two slashes, a space, the label padded, then the value. Every
    /// value therefore starts at the same column, which is the only reason the
    /// block reads as a table.
    /// </summary>
    public static string Row(string label, string value)
    {
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(value);

        return string.Concat("// ", label.PadRight(LabelWidth), value);
    }

    /// <summary>
    /// The block as it would be written for <paramref name="summary"/>.
    ///
    /// No row is ever left out. A count of zero renders as a zero and an absent
    /// assessment renders as <see cref="NotAssessed"/>, because a row that
    /// disappears when it has nothing to say makes the reader work out whether
    /// the value was zero or the tool forgot.
    /// </summary>
    public static IReadOnlyList<string> Render(AssuranceSummary summary, string copyright, string licence)
    {
        ArgumentNullException.ThrowIfNull(copyright);
        ArgumentNullException.ThrowIfNull(licence);

        return
        [
            copyright,
            licence,
            "//",
            Banner,
            BannerRule,
            Row("Relevant units:", Count(summary.Relevant)),
            Row("Annotated:", Fraction(summary.Annotated, summary.Relevant)),
            Row("Exempt:", Count(summary.Exempt)),
            Row("Human-reviewed:", Fraction(summary.Verified, summary.Relevant)),
            Row("IP risk:", summary.MaxIpRisk ?? NotAssessed),
            Row("Security risk:", summary.MaxSecurityRisk ?? NotAssessed),
            Row("Criteria:", Fraction(summary.Criteria, summary.CriteriaRequired)),
            Row(
                "Resource impact:",
                summary.MaxResources is { } score
                    ? string.Create(CultureInfo.InvariantCulture, $"{score}/10 max")
                    : NotAssessed),
            Row("Unverified:", Count(summary.Unverified)),
            "//",
            GeneratedMarker,
        ];
    }

    /// <summary>
    /// How many leading lines of <paramref name="lines"/> form a generated
    /// block, or zero when the file carries none.
    ///
    /// The same rule the owning component uses to find its own header: the file
    /// must open with the copyright line, and the block runs to the first line
    /// that is exactly the generated marker. A file opening with any other
    /// comment is left alone, because a header this editor did not recognize is
    /// a header it must not delete.
    /// </summary>
    public static int Length(AssuranceLines lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        if (lines.Count == 0 ||
            !lines[0].StartsWith(SpdxCopyrightPrefix, StringComparison.Ordinal))
        {
            return 0;
        }

        for (int line = 0; line < lines.Count && lines[line].StartsWith("//", StringComparison.Ordinal); line++)
        {
            if (string.Equals(lines[line], GeneratedMarker, StringComparison.Ordinal))
                return line + 1;
        }

        return 0;
    }

    /// <summary>
    /// The weakest claim any assessment makes for <paramref name="field"/>,
    /// ranked by position in <paramref name="vocabulary"/>.
    ///
    /// The vocabularies end with their weakest claim, so "worst" is the highest
    /// index rather than the lowest — an unknown IP risk outranks a high one,
    /// because not knowing is a weaker position than knowing it is bad. A value
    /// the vocabulary does not name is ignored rather than ranked, so a typo
    /// cannot silently become the file's headline number.
    /// </summary>
    public static string? Worst(
        IEnumerable<AssuranceAnnotation> assessed, string field, IReadOnlyList<string> vocabulary)
    {
        ArgumentNullException.ThrowIfNull(assessed);
        ArgumentNullException.ThrowIfNull(vocabulary);

        int worst = -1;
        foreach (AssuranceAnnotation annotation in assessed)
        {
            string? value = annotation.Field(field);
            if (value is null)
                continue;

            for (int rank = 0; rank < vocabulary.Count; rank++)
            {
                if (string.Equals(vocabulary[rank], value, StringComparison.Ordinal) && rank > worst)
                    worst = rank;
            }
        }

        return worst < 0 ? null : vocabulary[worst];
    }

    private static string Count(int value) =>
        value.ToString(CultureInfo.InvariantCulture);

    private static string Fraction(int numerator, int denominator) =>
        string.Create(CultureInfo.InvariantCulture, $"{numerator}/{denominator}");
}
