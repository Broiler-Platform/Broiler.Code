// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           0
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    High
// Criteria:         15/13
// Resource impact:  5/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

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
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=BBF1D1
// Broiler-Falsified-If: a criterion, exemption reason or Spec value containing a claim phrase passes VocabularyProblems with no problem
// Broiler-Human:        PENDING
public static partial class AssuranceRules
{
    /// <summary>
    /// The words of a review claim, matched case-insensitively. Bare
    /// <c>reviewed</c> is deliberately absent: "since it was reviewed" describes
    /// what a fingerprint is for and is not a claim that anything was.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=C1DD35
    // Broiler-Falsified-If: a term of the owning component's J9 list is missing here, so a generated line using it passes this tool's check
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=DE9342
    // Broiler-Falsified-If: the list lacks High or Critical, so a block at that level passes without a criterion line
    // Broiler-Human:        PENDING
    public static readonly IReadOnlyList<string> SecurityRequiringACriterion = ["High", "Critical"];

    /// <summary>
    /// The security values below High, whose blocks the rubric writes without a
    /// criterion: below High, the observation that would prove a unit wrong is
    /// rarely more than the unit restated.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=8B2E2B
    // Broiler-Falsified-If: the list holds High or Critical, so prune takes the criterion off a block at the top of the vocabulary and insert refuses one there
    // Broiler-Human:        PENDING
    public static readonly IReadOnlyList<string> SecurityWritingNoCriterion = ["None", "Low", "Medium"];

    /// <summary>
    /// Every problem with a falsification criterion, or nothing when it is
    /// well formed or absent.
    ///
    /// A criterion names one observation that would make the unit wrong. It is
    /// prose: one of this format's own fields written there (<c>Security=Low</c>,
    /// <c>Fingerprint=...</c>) would be an unchecked claim sitting where a
    /// reader takes it for a checked one, so it is refused. The owning
    /// component refuses any <c>Key=Value</c>, which also refuses the
    /// observation a criterion for network code is made of
    /// (<c>a cookie with SameSite=None is sent</c>); this refuses the format's
    /// fields only. Comparisons written with <c>==</c>, <c>!=</c>, <c>&lt;=</c>
    /// or <c>&gt;=</c> are prose. A criterion also never says that somebody
    /// looked (<see cref="ReviewClaimsIn"/>).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=1B72D8
    // Broiler-Falsified-If: a criterion stating one of the format's field names followed by an equals sign and a value yields no problem
    // Broiler-Human:        PENDING
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

        foreach (string term in ReviewClaimsIn(criterion))
        {
            yield return $"{AssuranceVocabulary.FalsifiedIfMarker} claims a review by saying '{term}', " +
                "and a falsification criterion states what would make the unit wrong, never that anyone read it";
        }
    }

    /// <summary>
    /// Every problem with the reason of an <c>EXEMPT=</c> field beyond its
    /// shape: a reason says why a unit needs no review, and never that it had
    /// one. The report prints every reason, so a claim here would be printed
    /// there as though the record held it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=320DC4
    // Broiler-Falsified-If: an exemption reason containing a claim phrase yields no problem
    // Broiler-Human:        PENDING
    public static IEnumerable<string> ExemptionReasonProblems(string? reason)
    {
        if (reason is null)
            yield break;

        foreach (string term in ReviewClaimsIn(reason))
        {
            yield return $"EXEMPT={reason} claims a review by saying '{term}', and an exemption states why a unit " +
                "needs no review, never that it had one";
        }
    }

    /// <summary>
    /// Every problem with a <c>Spec=</c> value beyond its shape: it cites what
    /// the unit implements, and never who read it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=19E322
    // Broiler-Falsified-If: a Spec value containing a claim phrase yields no problem
    // Broiler-Human:        PENDING
    public static IEnumerable<string> SpecProblems(string? spec)
    {
        if (spec is null)
            yield break;

        foreach (string term in ReviewClaimsIn(spec))
        {
            yield return $"Spec={spec} claims a review by saying '{term}', and a Spec cites what the unit " +
                "implements, never who read it";
        }
    }

    /// <summary>
    /// The review claims a piece of assessment prose makes, each once, in lower
    /// case, in the order they appear: <c>verified</c>, <c>approve</c> and its
    /// forms, <c>reviewer</c>, <c>reviewed by</c>, <c>human review</c> and its
    /// forms, <c>eligible for release</c>, <c>signed off</c> and
    /// <c>sign-off</c>, <c>certified</c>, <c>attested</c>, <c>LGTM</c>,
    /// <c>looks good to me</c> and <c>checked by</c>.
    ///
    /// Matched as whole words, so <c>unverified</c> and <c>verification</c>
    /// are not claims: a criterion about code that verifies a certificate has
    /// to be able to say so. The owning component matches its terms as
    /// substrings, which refuses those too. Every term of its list is one of
    /// these, so a claim it finds standing as a word is found here as well.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=0C28B4
    // Broiler-Falsified-If: a claim phrase the expression matches is left out of the returned list, so the text carrying it is accepted
    // Broiler-Human:        PENDING
    public static IReadOnlyList<string> ReviewClaimsIn(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var terms = new List<string>();
        foreach (Match match in ReviewClaim().Matches(text))
        {
            string term = WhitespaceRun().Replace(match.Value.ToLowerInvariant(), " ");
            if (!terms.Contains(term, StringComparer.Ordinal))
                terms.Add(term);
        }

        return terms;
    }

    /// <summary>
    /// Every problem with a parsed block's values, criterion first. Empty means
    /// well formed. The block must have been read with
    /// <see cref="AssuranceAnnotation.TryParseStrict"/>; the lenient parse keeps
    /// shapes this would misreport.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=703D05
    // Broiler-Falsified-If: a non-exempt block missing one of the five required fields yields no problem
    // Broiler-Human:        PENDING
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

            foreach (string problem in ExemptionReasonProblems(reason))
                yield return problem;

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

        foreach (string problem in SpecProblems(annotation.Field("Spec")))
            yield return problem;

        static string? Closed(string value, string[] allowed) =>
            allowed.Contains(value, StringComparer.Ordinal)
                ? null
                : $"is outside its vocabulary ({string.Join(", ", allowed)})";
    }

    /// <summary>
    /// True when a block that is not an exemption states a security risk that
    /// obliges it to carry a criterion.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1B1FA3
    // Broiler-Falsified-If: a non-exempt block whose security value is High or Critical returns false
    // Broiler-Human:        PENDING
    public static bool RequiresFalsificationCriterion(AssuranceAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        return annotation.ExemptReason is null &&
            annotation.Field("Security") is { } security &&
            SecurityRequiringACriterion.Contains(security, StringComparer.Ordinal);
    }

    /// <summary>
    /// True when a block that is not an exemption is assessed below High and
    /// carries a criterion line all the same.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C3A4FA
    // Broiler-Falsified-If: a block that states an exemption, or one assessed High, is answered true, so prune takes its criterion line
    // Broiler-Human:        PENDING
    public static bool CarriesCriterionBelowHigh(AssuranceAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        return annotation.ExemptReason is null &&
            annotation.HasCriterionLine &&
            annotation.Field("Security") is { } security &&
            SecurityWritingNoCriterion.Contains(security, StringComparer.Ordinal);
    }

    /// <summary>
    /// Reads a <c>Resources</c> value: an integer from 0 to 10.
    ///
    /// The owning component calls <c>int.TryParse</c> with the current culture.
    /// This uses the invariant culture with the same number style, so a sign or
    /// surrounding whitespace is accepted as it is there, and the answer does not
    /// depend on the machine's locale.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2E1379
    // Broiler-Falsified-If: a value of 11 or -1 is accepted, or the answer for 7 changes with the machine's current culture
    // Broiler-Human:        PENDING
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
    /// One of this format's fields written as <c>Key=Value</c>: a field name of
    /// the machine or the human line, in any case, an <c>=</c> that is not part
    /// of a comparison operator, and a value. The owning component's
    /// expression with its identifier narrowed to those names.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D5C99A
    // Broiler-Falsified-If: a format field name followed by an equals sign and a value, written in lower case or with spaces around the sign, is not matched
    // Broiler-Human:        PENDING
    [GeneratedRegex(
        @"(?<![=!<>\w.-])\b(?:Origin|Spec|IP|Security|Resources|Fingerprint|EXEMPT|Previous)\s*=\s*(?![=])\S+",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex FieldOnACriterion();

    /// <summary>A review claim as a whole word or phrase. See <see cref="ReviewClaimsIn"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=897B3A
    // Broiler-Falsified-If: a claim phrase written in mixed case, or with a tab between its two words, is not matched
    // Broiler-Human:        PENDING
    [GeneratedRegex(
        @"\b(?:verified|approv(?:e|es|ed|al|als|ing)|reviewers?|reviewed[\s-]+by|human[\s-]*review(?:ed|s)?|eligible\s+for\s+release|sign(?:ed)?[\s-]*off|certified|attested|lgtm|looks\s+good\s+to\s+me|checked[\s-]+by)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ReviewClaim();

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=3; Fingerprint=799B4C
    // Broiler-Falsified-If: a tab or line break inside a matched claim phrase survives normalisation, so one claim is listed under two spellings
    // Broiler-Human:        PENDING
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();
}
