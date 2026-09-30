// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           0
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    High
// Criteria:         11/5
// Resource impact:  4/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Broiler.Code.Review;

/// <summary>One file's contribution to a coverage report.</summary>
/// <param name="Path">Workspace-relative path, forward slashes.</param>
/// <param name="Component">The component the file was grouped under.</param>
/// <param name="State">The evaluated state.</param>
/// <param name="Reviewer">Who recorded it, empty when nobody has.</param>
/// <param name="ReviewedAt">When, null when nobody has.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=88F1E2
// Broiler-Human:        PENDING
public sealed record ReviewedFile(
    string Path,
    string Component,
    ReviewState State,
    string Reviewer = "",
    DateTimeOffset? ReviewedAt = null);

/// <summary>
/// Counts for one component or for a whole workspace.
///
/// The four buckets are exhaustive and disjoint, so they sum to
/// <see cref="Total"/>. That is not decoration: a coverage number whose parts do
/// not add up invites the reader to assume the flattering reading, and this
/// number exists precisely to stop the platform flattering itself.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=59DEC7
// Broiler-Falsified-If: VerifiedPercent counts files in StaleApprovals, Flagged or Unreviewed toward its figure, so the coverage gate passes below its minimum
// Broiler-Human:        PENDING
public sealed record ReviewCoverageTotals(
    string Name,
    int Total,
    int Verified,
    int StaleApprovals,
    int Flagged,
    int Unreviewed,
    int OpenNotes)
{
    /// <summary>
    /// The share of files a human approved and that have not changed since.
    ///
    /// A stale approval is deliberately not counted. It is the honest reading —
    /// nobody has confirmed the current content — and it is the one that keeps
    /// the number meaningful as the codebase moves, rather than letting it ratchet
    /// upward and stay there.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=B17075
    // Broiler-Falsified-If: a file counted in StaleApprovals raises VerifiedPercent
    // Broiler-Human:        PENDING
    public double VerifiedPercent => Total == 0 ? 0 : Verified * 100.0 / Total;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9B09E3
    // Broiler-Falsified-If: StalePercent is not zero when Total is 0
    // Broiler-Human:        PENDING
    public double StalePercent => Total == 0 ? 0 : StaleApprovals * 100.0 / Total;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9A7CC7
    // Broiler-Falsified-If: UnreviewedPercent is not zero when Total is 0
    // Broiler-Human:        PENDING
    public double UnreviewedPercent => Total == 0 ? 0 : Unreviewed * 100.0 / Total;

    /// <summary>Files reviewed and found wanting: an open question or a demanded change.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=549B26
    // Broiler-Falsified-If: FlaggedPercent is not zero when Total is 0
    // Broiler-Human:        PENDING
    public double FlaggedPercent => Total == 0 ? 0 : Flagged * 100.0 / Total;

    /// <summary>One decimal place, invariant, for a report line or a badge.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1E201D
    // Broiler-Falsified-If: a value is written with a comma as the decimal separator when the current culture is German
    // Broiler-Human:        PENDING
    public string FormatPercent(double value) =>
        value.ToString("0.0", CultureInfo.InvariantCulture) + " %";
}

/// <summary>
/// The counterpart to Test262 and WPT.
///
/// Those suites measure what a machine can prove about the platform. This
/// measures what a human has actually looked at, which is the other half of the
/// claim Broiler makes about itself and the half that has never been countable.
/// Put beside each other they say something neither says alone:
///
/// <code>
/// Correctness            Human verification
/// Test262   99.99 %      Source review   83.4 %
/// WPT       83 %
/// </code>
///
/// The number is only worth publishing if it is hard to inflate, which is why
/// <see cref="ReviewCoverageTotals.VerifiedPercent"/> excludes stale approvals
/// and why <see cref="FileReview.WithDecision"/> will not record an approval
/// against content its caller did not supply.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=A88676
// Broiler-Falsified-If: a file listed with a Reviewed state that is Stale or Unknown raises the VerifiedPercent of the totals returned
// Broiler-Human:        PENDING
public static class ReviewCoverage
{
    /// <summary>
    /// Groups files by the component each already carries and counts them.
    ///
    /// The grouping is decided when a <see cref="ReviewedFile"/> is built, by
    /// <see cref="ComponentOf"/>, which splits on the repository's own shape — a
    /// submodule directory, or the project directory under <c>src/</c> — because
    /// that is the unit the platform is described in and the unit a reviewer
    /// owns.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5566D5
    // Broiler-Falsified-If: the per-component totals do not add up to the Overall totals for the same files
    // Broiler-Human:        PENDING
    public static IReadOnlyList<ReviewCoverageTotals> ByComponent(IEnumerable<ReviewedFile> files)
    {
        ArgumentNullException.ThrowIfNull(files);

        return [.. files
            .GroupBy(file => file.Component, StringComparer.Ordinal)
            .Select(group => Count(group.Key, group))

            // Worst first. A report sorted alphabetically buries the component
            // that needs attention among the ones that do not.
            .OrderBy(totals => totals.VerifiedPercent)
            .ThenBy(totals => totals.Name, StringComparer.Ordinal)];
    }

    /// <summary>The totals across every file, under the name given.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=62601C
    // Broiler-Falsified-If: a file in the input is left out of Total, which raises VerifiedPercent
    // Broiler-Human:        PENDING
    public static ReviewCoverageTotals Overall(IEnumerable<ReviewedFile> files, string name = "Broiler Platform")
    {
        ArgumentNullException.ThrowIfNull(files);
        return Count(name, files);
    }

    /// <summary>
    /// The component a workspace-relative path belongs to.
    ///
    /// <c>Broiler.JS/src/Broiler.JS/Runtime/JsObject.cs</c> is Broiler.JS;
    /// <c>src/Broiler.Code.Core/Shell/CodeShell.cs</c> is Broiler.Code.Core.
    /// Anything else is grouped under its first segment, so a file in an
    /// unexpected place is still counted somewhere rather than silently dropped
    /// from the denominator.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=17A954
    // Broiler-Falsified-If: a path under src/ is grouped under src rather than under its project directory
    // Broiler-Human:        PENDING
    public static string ComponentOf(string relativePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);

        string[] segments = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return "(root)";

        // Under src/ the component is the project directory, which is the level
        // that owns a .csproj and therefore the level a reviewer works at.
        if (segments[0] == "src")
            return segments.Length > 1 ? segments[1] : "src";

        return segments[0];
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=982D8D
    // Broiler-Falsified-If: a file whose state is Reviewed with Stale or Unknown freshness is added to the count behind VerifiedPercent
    // Broiler-Human:        PENDING
    private static ReviewCoverageTotals Count(string name, IEnumerable<ReviewedFile> files)
    {
        int total = 0, verified = 0, stale = 0, flagged = 0, unreviewed = 0, openNotes = 0;

        foreach (ReviewedFile file in files)
        {
            total++;
            openNotes += file.State.OpenNotes;

            if (file.State.IsVerified)
                verified++;
            else if (file.State.IsStaleApproval)
                stale++;
            else if (file.State.Status is ReviewStatus.Question or ReviewStatus.NeedsChange)
                flagged++;
            else
                // InReview and Unknown land here with Unreviewed. Neither is an
                // approval, and a bucket per intermediate state would make the
                // report harder to read without changing what it says.
                unreviewed++;
        }

        return new ReviewCoverageTotals(name, total, verified, stale, flagged, unreviewed, openNotes);
    }
}
