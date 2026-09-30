using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// The rule that no generated text may say a unit is reviewed, approved or
/// releasable while the annotations say no such thing (the owning component's
/// J9), ported with its algorithm and its quirks.
///
/// A review word in generated text is honest in exactly two ways: the first
/// number after it is the count the annotations give for it
/// (<c>| VERIFIED | 0 |</c>, <c>// Human-reviewed:   0/12</c>), or a negation
/// stands before it in the same clause (<c>it is not an approval</c>). Only
/// the first whole-word occurrence of each term on a line is judged, at most
/// one violation is reported per line, and a clause ends at <c>:</c>,
/// <c>;</c> or <c>.</c>.
///
/// It runs over the generator's output, which this tool writes, so on a
/// correct build it finds nothing but text a human put there: an alias or an
/// exemption reason that happens to contain one of the words is echoed into the
/// artefacts and reported, as it is by the owning component.
/// </summary>
public static class AssuranceReviewClaims
{
    private static readonly string[] Negations =
        ["no", "not", "nothing", "never", "none", "neither", "nobody", "absence", "unverified"];

    private static readonly char[] ClauseSeparators = [':', ';', '.'];

    /// <summary>
    /// Every line of <paramref name="lines"/> stating a review the annotations
    /// in <paramref name="scope"/> do not hold.
    /// </summary>
    /// <param name="where">The artefact, for messages.</param>
    /// <param name="lines">Its generated text: the header of a source file, or the whole of anything else.</param>
    /// <param name="scope">The units the text speaks for: the file's own for a source header, all of them otherwise.</param>
    public static IReadOnlyList<AssuranceViolation> Violations(
        string where, IReadOnlyList<string> lines, IReadOnlyList<AssuranceCorpusUnit> scope)
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(scope);

        var violations = new List<AssuranceViolation>();
        for (int line = 0; line < lines.Count; line++)
        {
            string content = lines[line];
            string lowered = content.ToLowerInvariant();

            foreach (string term in AssuranceRules.ReviewClaimTerms)
            {
                int at = WholeWord(lowered, term);
                if (at < 0 || IsSupported(lowered, at, term, scope))
                    continue;

                string count = Supported(term, scope)?.ToString(CultureInfo.InvariantCulture) ?? "none is defined for it";
                violations.Add(new AssuranceViolation(
                    "J9",
                    where,
                    line + 1,
                    string.Create(
                        CultureInfo.InvariantCulture,
                        $"{where}({line + 1}) says '{content.Trim()}', and the annotations hold no such state: the term " +
                        $"'{term}' is stated with neither the count the annotations give ({count}) nor a negation before it")));
                break;
            }
        }

        return violations;
    }

    /// <summary>
    /// Where <paramref name="term"/> first stands as a whole word that is not
    /// the tail of a dotted name, or -1. <c>VmVerifiedArtifact</c> and
    /// <c>State.Verified</c> are names, not claims.
    /// </summary>
    private static int WholeWord(string lowered, string term)
    {
        for (int index = lowered.IndexOf(term, StringComparison.Ordinal);
             index >= 0;
             index = index + 1 <= lowered.Length ? lowered.IndexOf(term, index + 1, StringComparison.Ordinal) : -1)
        {
            bool before = index == 0 || !char.IsAsciiLetter(lowered[index - 1]);
            bool after = index + term.Length >= lowered.Length || !char.IsAsciiLetter(lowered[index + term.Length]);

            if (before && after && (index == 0 || lowered[index - 1] != '.'))
                return index;
        }

        return -1;
    }

    private static bool IsSupported(string lowered, int at, string term, IReadOnlyList<AssuranceCorpusUnit> scope)
    {
        if (Negations.Any(negation => IsWordBefore(lowered, negation, at)))
            return true;

        if (Supported(term, scope) is not { } expected)
            return false;

        return FirstNumberAfter(lowered, at + term.Length) == expected;
    }

    /// <summary>A negation as a whole word in the clause that holds <paramref name="at"/>, before it.</summary>
    private static bool IsWordBefore(string lowered, string word, int at)
    {
        int clause = 0;
        for (int index = 0; index < at && index < lowered.Length; index++)
        {
            if (ClauseSeparators.Contains(lowered[index]))
                clause = index + 1;
        }

        for (int index = lowered.IndexOf(word, clause, StringComparison.Ordinal);
             index >= 0 && index < at;
             index = lowered.IndexOf(word, index + 1, StringComparison.Ordinal))
        {
            bool before = index == 0 || !char.IsAsciiLetter(lowered[index - 1]);
            bool after = index + word.Length >= lowered.Length || !char.IsAsciiLetter(lowered[index + word.Length]);
            if (before && after)
                return true;
        }

        return false;
    }

    /// <summary>The count the annotations give for a term, or null where the term names no countable state.</summary>
    private static int? Supported(string term, IReadOnlyList<AssuranceCorpusUnit> scope) => term switch
    {
        "verified" or "human reviewed" or "human-reviewed" or "humanreviewed" =>
            scope.Count(static unit => unit.IsRelevant && unit.State == AssuranceUnitState.Verified),
        "approved" =>
            scope.Count(static unit => unit.State == AssuranceUnitState.HumanApprovedPendingFingerprint),
        _ => null,
    };

    private static int? FirstNumberAfter(string line, int index)
    {
        while (index < line.Length && !char.IsAsciiDigit(line[index]))
            index++;

        if (index >= line.Length)
            return null;

        int end = index;
        while (end < line.Length && char.IsAsciiDigit(line[end]))
            end++;

        return int.TryParse(line.AsSpan(index, end - index), NumberStyles.None, CultureInfo.InvariantCulture, out int value)
            ? value
            : null;
    }
}
