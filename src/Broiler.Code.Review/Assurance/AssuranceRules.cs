using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// The checks the owning component applies to one annotation block, ported with
/// its problem strings word for word.
///
/// They are here rather than in the command-line tool because they are part of
/// the format, not of any one tool. A block that fails them is one the owning
/// component reports, so a tool that writes a block must run them over what it
/// is about to write.
/// </summary>
public static partial class AssuranceRules
{
    /// <summary>
    /// The words of a review claim, matched case-insensitively. Bare
    /// <c>reviewed</c> is deliberately absent: "since it was reviewed" describes
    /// what a fingerprint is for and is not a claim that anything was.
    /// </summary>
    public static readonly IReadOnlyList<string> ReviewClaimTerms =
    [
        "verified",
        "approved",
        "approval",
        "reviewer",
        "reviewed by",
        "reviewed-by",
        "human reviewed",
        "human-reviewed",
        "humanreviewed",
        "eligible for release",
        "signed off",
        "sign-off",
        "certified",
        "attested",
    ];

    /// <summary>The security values that oblige a unit to carry a falsification criterion.</summary>
    public static readonly IReadOnlyList<string> SecurityRequiringACriterion = ["High", "Critical"];

    /// <summary>
    /// Every problem with a falsification criterion, or nothing when it is
    /// well formed or absent.
    ///
    /// A criterion names one observation that would make the unit wrong. It is
    /// prose: a <c>Key=Value</c> pair there would be an unchecked claim sitting
    /// where a reader takes it for a checked one, so it is refused. The test is
    /// narrow on purpose, so comparisons written with <c>==</c>, <c>!=</c>,
    /// <c>&lt;=</c> or <c>&gt;=</c> are still prose. A criterion also never says
    /// that somebody looked; the review terms are matched as substrings, which
    /// is stricter than the whole-word match used on generated text.
    /// </summary>
    public static IEnumerable<string> CriterionProblems(string? criterion)
    {
        if (criterion is null)
            yield break;

        if (criterion.Length == 0)
        {
            yield return $"{AssuranceVocabulary.FalsifiedIfMarker} carries no criterion";
            yield break;
        }

        Match field = FieldOnACriterion().Match(criterion);
        if (field.Success)
        {
            yield return $"{AssuranceVocabulary.FalsifiedIfMarker} states the field {field.Value}, " +
                "and a falsification criterion is prose, not data";
        }

        string lowered = criterion.ToLowerInvariant();
        foreach (string term in ReviewClaimTerms)
        {
            if (lowered.Contains(term, StringComparison.Ordinal))
            {
                yield return $"{AssuranceVocabulary.FalsifiedIfMarker} claims a review by saying '{term}', " +
                    "and a falsification criterion states what would make the unit wrong, never that anyone read it";
            }
        }
    }

    /// <summary>
    /// Every problem with a parsed block's values, criterion first. Empty means
    /// well formed. The block must have been read with
    /// <see cref="AssuranceAnnotation.TryParseStrict"/>; the lenient parse keeps
    /// shapes this would misreport.
    /// </summary>
    public static IEnumerable<string> VocabularyProblems(AssuranceAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        foreach (string problem in CriterionProblems(annotation.HasCriterionLine ? annotation.Criterion : null))
            yield return problem;

        if (annotation.ExemptReason is { } reason)
        {
            if (reason.Length == 0)
                yield return "EXEMPT carries no reason";

            if (annotation.Fields.Count > 1)
                yield return "EXEMPT is stated beside other fields; an exemption is not an assessment";

            yield break;
        }

        foreach (string required in AssuranceVocabulary.RequiredFields)
        {
            if (annotation.Field(required) is null)
                yield return $"no {required} field";
        }

        foreach (AssuranceField field in annotation.Fields)
        {
            string? problem = field.Key switch
            {
                "Origin" => Closed(field.Value, AssuranceVocabulary.OriginValues),
                "IP" => Closed(field.Value, AssuranceVocabulary.IpRiskValues),
                "Security" => Closed(field.Value, AssuranceVocabulary.SecurityRiskValues),
                "Resources" => TryParseResources(field.Value, out _) ? null : "is not an integer 0 to 10",
                "Fingerprint" => string.Equals(field.Value, AssuranceVocabulary.ToBeFilled, StringComparison.Ordinal) ||
                    AssuranceVocabulary.IsWellFormedFingerprint(field.Value)
                    ? null
                    : $"is neither {AssuranceVocabulary.ToBeFilled} nor six uppercase hex characters",
                "Spec" => field.Value.Length == 0 ? "is empty" : null,
                _ => "is not a field this system defines",
            };

            if (problem is not null)
                yield return $"{field.Key}={field.Value} {problem}";
        }

        static string? Closed(string value, string[] allowed) =>
            allowed.Contains(value, StringComparer.Ordinal)
                ? null
                : $"is outside its vocabulary ({string.Join(", ", allowed)})";
    }

    /// <summary>
    /// True when a block that is not an exemption states a security risk that
    /// obliges it to carry a criterion.
    /// </summary>
    public static bool RequiresFalsificationCriterion(AssuranceAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        return annotation.ExemptReason is null &&
            annotation.Field("Security") is { } security &&
            SecurityRequiringACriterion.Contains(security, StringComparer.Ordinal);
    }

    /// <summary>
    /// Reads a <c>Resources</c> value: an integer from 0 to 10.
    ///
    /// The owning component calls <c>int.TryParse</c> with the current culture.
    /// This uses the invariant culture with the same number style, so a sign or
    /// surrounding whitespace is accepted as it is there, and the answer does not
    /// depend on the machine's locale.
    /// </summary>
    public static bool TryParseResources(string? value, out int resources)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out resources) &&
            resources is >= 0 and <= 10)
        {
            return true;
        }

        resources = 0;
        return false;
    }

    /// <summary>
    /// A <c>Key=Value</c> field: an identifier, an <c>=</c> that is not part of
    /// a comparison operator, and a value. The owning component's expression.
    /// </summary>
    [GeneratedRegex(@"(?<![=!<>])\b[A-Za-z_][A-Za-z0-9_.-]*\s*=\s*(?![=])\S+")]
    private static partial Regex FieldOnACriterion();
}
