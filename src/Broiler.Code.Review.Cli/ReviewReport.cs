// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         5/3
// Resource impact:  5/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Workspaces.Storage;

namespace Broiler.Code.Review.Cli;

/// <summary>
/// Evaluates every source file in a repository against its review record.
///
/// This is the same evaluation the editor does, over every file rather than the
/// one on screen, and it deliberately shares
/// <see cref="ReviewStateEvaluator"/> with it. Two implementations of "is this
/// review still current?" would eventually disagree, and the first anyone would
/// hear of it is a CI run contradicting the badge in the editor.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=B86628
// Broiler-Falsified-If: a recorded file whose text the storage refuses to read, such as a symbolic link, is reported as reviewed and unchanged
// Broiler-Human:        PENDING
public static class ReviewReport
{
    /// <summary>Evaluates <paramref name="files"/> against the records under <paramref name="root"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=2DB7D3
    // Broiler-Falsified-If: a recorded file whose text the storage refuses to read, such as a symbolic link, is reported as reviewed and unchanged
    // Broiler-Human:        PENDING
    public static async ValueTask<IReadOnlyList<ReviewedFile>> EvaluateAsync(
        string root,
        IReadOnlyList<string> files,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(files);

        var storage = new FileSystemWorkspaceStorage(root);
        var store = new ReviewStore(storage);
        ReviewStore.ReviewRecordSet all =
            await store.ReadAllAsync(cancellationToken).ConfigureAwait(false);

        // A record that cannot be parsed — most often one a merge left conflict
        // markers in — would otherwise make its file look unreviewed and quietly
        // lower the published number. Reported on stderr so the run's own output
        // stays machine-readable, and loudly, because a silently wrong coverage
        // figure is worse than none.
        foreach (StorageFailure failure in all.Unreadable)
            Console.Error.WriteLine($"::warning::{failure.Message}");

        IReadOnlyDictionary<string, FileReview> records = all.Reviews;

        // A second index, keyed the way the file list is folded. The editor
        // writes a record at whatever path the file had when it was opened, and
        // that may be a nested checkout — Broiler.HTML/Broiler.CSS/… — while the
        // denominator keeps only the canonical Broiler.CSS/… path. Without this
        // the two spaces never meet, and a real review recorded through a nested
        // checkout reads as unreviewed.
        var byIdentity = new Dictionary<string, FileReview>(StringComparer.OrdinalIgnoreCase);
        foreach ((string path, FileReview record) in records)
            byIdentity.TryAdd(SourceInventory.IdentityOf(path), record);

        var evaluated = new List<ReviewedFile>(files.Count);
        foreach (string file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!records.TryGetValue(file, out FileReview? review) &&
                !byIdentity.TryGetValue(SourceInventory.IdentityOf(file), out review))
            {
                evaluated.Add(new ReviewedFile(file, ReviewCoverage.ComponentOf(file), ReviewState.None));
                continue;
            }

            // Read through the same storage the editor writes through, so the
            // path rules — normalization, grant containment — are the ones the
            // record was written under.
            StorageResult<StorageTextContent> read = await storage
                .ReadTextAsync(file, cancellationToken).ConfigureAwait(false);

            ReviewState state = ReviewStateEvaluator.Evaluate(
                review, read.Succeeded ? read.Value!.Text : null);

            evaluated.Add(new ReviewedFile(
                file, ReviewCoverage.ComponentOf(file), state, review.Reviewer, review.ReviewedAt));
        }

        return evaluated;
    }

    /// <summary>
    /// Renders the report as Markdown, for a job summary or a pull-request
    /// comment.
    ///
    /// The overall row comes first and the component table is worst-first, which
    /// is the order somebody deciding what to review next reads it in.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=11E880
    // Broiler-Falsified-If: the four rows of the overall table do not add up to the file total its opening sentence states
    // Broiler-Human:        PENDING
    public static string ToMarkdown(IReadOnlyList<ReviewedFile> files)
    {
        ArgumentNullException.ThrowIfNull(files);

        ReviewCoverageTotals overall = ReviewCoverage.Overall(files);
        var text = new StringBuilder();

        text.AppendLine("## Human review coverage");
        text.AppendLine();
        text.AppendLine(Invariant($"**{overall.FormatPercent(overall.VerifiedPercent)}** of {overall.Total} source files are human reviewed against their current content."));
        text.AppendLine();
        text.AppendLine("| | Files | Share |");
        text.AppendLine("| --- | ---: | ---: |");
        text.AppendLine(Invariant($"| Human reviewed | {overall.Verified} | {overall.FormatPercent(overall.VerifiedPercent)} |"));
        text.AppendLine(Invariant($"| Modified since review | {overall.StaleApprovals} | {overall.FormatPercent(overall.StalePercent)} |"));
        text.AppendLine(Invariant($"| Question or needs change | {overall.Flagged} | {overall.FormatPercent(overall.FlaggedPercent)} |"));
        text.AppendLine(Invariant($"| Needs review | {overall.Unreviewed} | {overall.FormatPercent(overall.UnreviewedPercent)} |"));
        text.AppendLine();

        if (overall.OpenNotes > 0)
        {
            text.AppendLine(Invariant($"{overall.OpenNotes} review notes are still open."));
            text.AppendLine();
        }

        text.AppendLine("### By component");
        text.AppendLine();
        text.AppendLine("| Component | Reviewed | Stale | Needs review | Total | Coverage |");
        text.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: |");

        foreach (ReviewCoverageTotals component in ReviewCoverage.ByComponent(files))
        {
            text.AppendLine(Invariant(
                $"| {component.Name} | {component.Verified} | {component.StaleApprovals} | {component.Unreviewed} | {component.Total} | {component.FormatPercent(component.VerifiedPercent)} |"));
        }

        text.AppendLine();
        text.AppendLine(
            "Stale approvals are counted separately and are **not** included in the coverage " +
            "percentage: a review of content that has since changed has not been confirmed by anyone.");

        return text.ToString();
    }

    /// <summary>
    /// Renders the report as JSON, for anything that wants to track the number
    /// over time rather than read it once.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=40B3FF
    // Broiler-Falsified-If: a component listed in the Markdown table is missing from the JSON components array
    // Broiler-Human:        PENDING
    public static string ToJson(IReadOnlyList<ReviewedFile> files)
    {
        ArgumentNullException.ThrowIfNull(files);

        ReviewCoverageTotals overall = ReviewCoverage.Overall(files);
        var buffer = new MemoryStream();
        using (var writer = new System.Text.Json.Utf8JsonWriter(
            buffer, new System.Text.Json.JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartObject();
            WriteTotals(writer, "overall", overall);

            writer.WriteStartArray("components");
            foreach (ReviewCoverageTotals component in ReviewCoverage.ByComponent(files))
            {
                writer.WriteStartObject();
                WriteTotalsBody(writer, component);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.ToArray()) + "\n";
    }

    /// <summary>
    /// The files a run should complain about: an approval whose content has
    /// changed, and a file somebody said needs a change.
    ///
    /// Files that were never reviewed are deliberately absent. A repository
    /// adopting this starts with all of them, and a check that fails on every
    /// unreviewed file would be turned off on the first run and never turned
    /// back on. The coverage number is how those are reported; this is for
    /// things that changed under a human's earlier answer.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=ED3B11
    // Broiler-Falsified-If: a file whose record says reviewed but whose content hash no longer matches is missing from the returned list
    // Broiler-Human:        PENDING
    public static IReadOnlyList<ReviewedFile> Regressions(IReadOnlyList<ReviewedFile> files)
    {
        ArgumentNullException.ThrowIfNull(files);

        var found = new List<ReviewedFile>();
        foreach (ReviewedFile file in files)
        {
            if (file.State.IsStaleApproval || file.State.Status == ReviewStatus.NeedsChange)
                found.Add(file);
        }

        return found;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=747CD1
    // Broiler-Human:        PENDING
    private static void WriteTotals(
        System.Text.Json.Utf8JsonWriter writer, string name, ReviewCoverageTotals totals)
    {
        writer.WriteStartObject(name);
        WriteTotalsBody(writer, totals);
        writer.WriteEndObject();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=51A2C4
    // Broiler-Human:        PENDING
    private static void WriteTotalsBody(
        System.Text.Json.Utf8JsonWriter writer, ReviewCoverageTotals totals)
    {
        writer.WriteString("name", totals.Name);
        writer.WriteNumber("total", totals.Total);
        writer.WriteNumber("verified", totals.Verified);
        writer.WriteNumber("staleApprovals", totals.StaleApprovals);
        writer.WriteNumber("flagged", totals.Flagged);
        writer.WriteNumber("unreviewed", totals.Unreviewed);
        writer.WriteNumber("openNotes", totals.OpenNotes);
        writer.WriteNumber("verifiedPercent", Math.Round(totals.VerifiedPercent, 2));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=434BE1
    // Broiler-Human:        PENDING
    private static string Invariant(FormattableString text) =>
        text.ToString(CultureInfo.InvariantCulture);
}
