using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// Finding, removing and writing the generated block at the top of a covered
/// file, and recognizing a forged one anywhere else in it.
///
/// The removal follows the owning component's generator: a file whose first
/// line is an SPDX line has its leading <c>//</c> run removed through the
/// <c>GENERATED</c> marker and one blank line, and then every further leading
/// comment run that reads like a summary block. Where that generator would
/// delete something it cannot prove it wrote, this refuses instead:
///
/// <list type="bullet">
/// <item>a line between the SPDX lines and the marker that is neither a header
/// line nor summary vocabulary, such as a licence sentence someone put
/// there;</item>
/// <item>a copyright line in the old header that the configuration no longer
/// states for the file, because dropping one would silently re-attribute the
/// code;</item>
/// <item>an SPDX run with no marker at all, unless every line of it is a line
/// the new header carries anyway. The owning component deletes such a run
/// whole, which for third-party code is a licence notice.</item>
/// </list>
///
/// A refusal is a message for a human, and the file is left as it was.
/// </summary>
public static class AssuranceHeader
{
    /// <summary>
    /// The owning component's summary vocabulary, matched case-insensitively
    /// anywhere in a comment line. Its <c>reviewer</c> and <c>approved</c> are
    /// what makes it strict: in code that talks about reviews they match
    /// ordinary comments.
    /// </summary>
    public static readonly IReadOnlyList<string> StrictVocabulary =
    [
        "broiler code assurance",
        "generated - do not edit manually",
        "relevant units:", "annotated:", "exempt:", "human-reviewed",
        "ip risk", "security risk", "criteria:", "resource impact", "unverified:",
        "human reviewed", "reviewer", "reviewstate", "humanreviewed", "approved",
        "eligible for release",
    ];

    /// <summary>
    /// What a comment must open with, after its slashes and spaces, to be a
    /// summary line under the narrow vocabulary: the banner, the marker, or one
    /// of the header's row labels.
    /// </summary>
    private static readonly string[] NarrowOpenings =
    [
        AssuranceBanner.Banner[3..],
        AssuranceBanner.GeneratedMarker[3..],
        .. AssuranceBanner.RowLabels,
    ];

    /// <summary>
    /// True for a comment line that reads like part of a generated summary,
    /// under <paramref name="vocabulary"/>. A line that is not a <c>//</c>
    /// comment never is: a summary is written in comments, and a string that
    /// happens to contain a row label is not one.
    /// </summary>
    public static bool IsSummaryLine(string line, AssuranceForgeryVocabulary vocabulary)
    {
        ArgumentNullException.ThrowIfNull(line);

        string content = line.Trim();
        if (!content.StartsWith("//", StringComparison.Ordinal))
            return false;

        if (vocabulary == AssuranceForgeryVocabulary.Strict)
            return StrictVocabulary.Any(term => content.Contains(term, StringComparison.OrdinalIgnoreCase));

        string body = content.TrimStart('/').TrimStart();
        return NarrowOpenings.Any(opening => body.StartsWith(opening, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// How many assurance banners a text carries, at any indentation and in any
    /// case. A generated file carries exactly one.
    /// </summary>
    public static int BannerCount(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var lines = new AssuranceLines(text);
        int count = 0;
        for (int index = 0; index < lines.Count; index++)
        {
            if (string.Equals(lines[index].Trim(), AssuranceBanner.Banner, StringComparison.OrdinalIgnoreCase))
                count++;
        }

        return count;
    }

    /// <summary>
    /// A violation when <paramref name="text"/> carries more than one banner,
    /// in the owning component's words; otherwise null.
    /// </summary>
    public static AssuranceViolation? DuplicateBanners(string path, string text)
    {
        ArgumentNullException.ThrowIfNull(path);

        int count = BannerCount(text);
        return count <= 1
            ? null
            : new AssuranceViolation(
                "J5",
                path,
                null,
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"{path} carries {count} '{AssuranceBanner.Banner[3..]}' banners; exactly one block is generated and every other one is a forgery"));
    }

    /// <summary>
    /// A violation naming the first summary line below the generated marker,
    /// and how many there are; otherwise null. Below the header, the generator
    /// neither writes nor strips anything, so a summary block there survives
    /// every generation and only this finds it.
    /// </summary>
    public static AssuranceViolation? ForgedSummary(string path, string text, AssuranceForgeryVocabulary vocabulary)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(text);

        var lines = new AssuranceLines(text);
        int header = -1;
        for (int index = 0; index < lines.Count; index++)
        {
            if (string.Equals(lines[index].Trim(), AssuranceBanner.GeneratedMarker, StringComparison.Ordinal))
            {
                header = index;
                break;
            }
        }

        int first = -1;
        int count = 0;
        for (int index = header + 1; index < lines.Count; index++)
        {
            if (!IsSummaryLine(lines[index], vocabulary))
                continue;

            if (first < 0)
                first = index;

            count++;
        }

        return first < 0
            ? null
            : new AssuranceViolation(
                "J5",
                path,
                first + 1,
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"{path}({first + 1}) carries the assurance summary line '{lines[first].Trim()}' below the generated header, and {count} such line(s) sit there; the generated block is the only one a file may carry"));
    }

    /// <summary>
    /// The generated header of a text, from its first line through the marker,
    /// or nothing when it has none. This is the generated part of a source
    /// file, and all of it the review-claim rule reads there.
    /// </summary>
    public static IReadOnlyList<string> GeneratedHeaderLines(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var lines = new AssuranceLines(text);
        if (!lines[0].StartsWith(AssuranceBanner.SpdxCopyrightPrefix, StringComparison.Ordinal))
            return [];

        for (int index = 0; index < lines.Count && lines[index].StartsWith("//", StringComparison.Ordinal); index++)
        {
            if (string.Equals(lines[index], AssuranceBanner.GeneratedMarker, StringComparison.Ordinal))
                return [.. Enumerable.Range(0, index + 1).Select(line => lines[line])];
        }

        return [];
    }

    /// <summary>
    /// Removes the generated header and any forged summary runs directly under
    /// it, in place. Returns null, or why nothing was removed.
    /// </summary>
    /// <param name="lines">The file's lines. Unchanged when a problem is returned.</param>
    /// <param name="spdxLines">The SPDX lines the new header will carry for this file.</param>
    /// <param name="vocabulary">What marks a leading comment run as a summary block.</param>
    /// <param name="path">The file, for messages.</param>
    /// <param name="separators">
    /// The line endings of the header lines removed, in order, so that the new
    /// header can end each of its lines the way the old one did.
    /// </param>
    public static string? Strip(
        AssuranceLines lines,
        IReadOnlyList<string> spdxLines,
        AssuranceForgeryVocabulary vocabulary,
        string path,
        out IReadOnlyList<string> separators)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(spdxLines);
        ArgumentNullException.ThrowIfNull(path);

        separators = [];
        if (!lines[0].StartsWith(AssuranceBanner.SpdxPrefix, StringComparison.Ordinal))
            return null;

        int run = LeadingCommentRun(lines);
        int marker = MarkerIn(lines, run);
        int through;

        if (marker >= 0)
        {
            for (int index = 0; index <= marker; index++)
            {
                string line = lines[index];
                if (IsHeaderShaped(line) || IsSummaryLine(line, vocabulary))
                    continue;

                return string.Create(
                    CultureInfo.InvariantCulture,
                    $"{path}({index + 1}) sits inside the generated header and is not a line the generator writes: " +
                    $"'{line}'. The generator will not delete it; move it below the header, or delete it.");
            }

            for (int index = 0; index <= marker; index++)
            {
                string line = lines[index];
                if (line.StartsWith(AssuranceBanner.SpdxCopyrightPrefix, StringComparison.Ordinal) &&
                    !spdxLines.Contains(line, StringComparer.Ordinal))
                {
                    return string.Create(
                        CultureInfo.InvariantCulture,
                        $"{path}({index + 1}) carries '{line}' in its generated header, and " +
                        $"{AssuranceComponentConfig.FileName} does not state that line for this file. The generator " +
                        $"will not drop a copyright line: state it in spdx or spdxOverrides, or delete it by hand.");
                }
            }

            through = marker + 1;
        }
        else
        {
            for (int index = 0; index < run; index++)
            {
                if (spdxLines.Contains(lines[index], StringComparer.Ordinal))
                    continue;

                return string.Create(
                    CultureInfo.InvariantCulture,
                    $"{path} opens with a comment run the generator did not write (line {index + 1}: " +
                    $"'{lines[index]}'). It will neither delete that run nor stack a second licence header " +
                    $"above it: state its SPDX lines in spdx or spdxOverrides and delete the run, or exclude the file.");
            }

            through = run;
        }

        if (through < lines.Count && lines[through].Trim().Length == 0)
            through++;

        var removed = new List<string>(through);
        for (int index = 0; index < through; index++)
            removed.Add(lines.SeparatorOf(index));

        RemoveLeading(lines, through);
        separators = removed;

        // A summary block pasted directly under the real one would otherwise be
        // carried through verbatim by every generation.
        while (LeadingRunIsSummary(lines, vocabulary))
            RemoveLeadingCommentRun(lines);

        return null;
    }

    /// <summary>
    /// Inserts <paramref name="header"/> at the top of the file. Line
    /// <c>i</c> takes the ending of the old header's line <c>i</c> when there
    /// was one, and otherwise the ending of the line it lands above, so a file
    /// keeps its own line endings and a regeneration moves none of them.
    /// </summary>
    public static void Insert(AssuranceLines lines, IReadOnlyList<string> header, IReadOnlyList<string> separators)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(separators);

        string fallback = lines.SeparatorOf(0) is { Length: > 0 } own ? own : lines.NewLine;
        for (int index = 0; index < header.Count; index++)
        {
            string separator = index < separators.Count && separators[index].Length > 0 ? separators[index] : fallback;
            lines.Insert(index, [header[index]], separator);
        }
    }

    /// <summary>
    /// A line the generator writes into a header: an SPDX line, a bare
    /// <c>//</c>, the banner, its rule, the marker, or a row with one of the
    /// nine labels (whatever value it states).
    /// </summary>
    private static bool IsHeaderShaped(string line) =>
        line.StartsWith(AssuranceBanner.SpdxCopyrightPrefix, StringComparison.Ordinal) ||
        line.StartsWith(AssuranceBanner.SpdxLicensePrefix, StringComparison.Ordinal) ||
        line is "//" or AssuranceBanner.Banner or AssuranceBanner.BannerRule or AssuranceBanner.GeneratedMarker ||
        AssuranceBanner.RowLabels.Any(label => line.StartsWith("// " + label, StringComparison.Ordinal));

    private static int LeadingCommentRun(AssuranceLines lines)
    {
        int run = 0;
        while (run < lines.Count && lines[run].StartsWith("//", StringComparison.Ordinal))
            run++;

        return run;
    }

    private static int MarkerIn(AssuranceLines lines, int run)
    {
        for (int index = 0; index < run; index++)
        {
            if (string.Equals(lines[index], AssuranceBanner.GeneratedMarker, StringComparison.Ordinal))
                return index;
        }

        return -1;
    }

    private static bool LeadingRunIsSummary(AssuranceLines lines, AssuranceForgeryVocabulary vocabulary)
    {
        int run = LeadingCommentRun(lines);
        for (int index = 0; index < run; index++)
        {
            if (IsSummaryLine(lines[index], vocabulary))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Removes the leading <c>//</c> run through its own marker when it has
    /// one, else whole, and one blank line after it: the owning component's
    /// rule for a forged block.
    /// </summary>
    private static void RemoveLeadingCommentRun(AssuranceLines lines)
    {
        int run = LeadingCommentRun(lines);
        if (run == 0)
            return;

        int marker = MarkerIn(lines, run);
        int through = marker >= 0 ? marker + 1 : run;

        if (through < lines.Count && lines[through].Trim().Length == 0)
            through++;

        RemoveLeading(lines, through);
    }

    /// <summary>
    /// Removes the first <paramref name="count"/> lines. When that is all of
    /// them, the last line is kept and emptied instead, so the text still ends
    /// the way the line model says every text ends: in a line with no
    /// terminator.
    /// </summary>
    private static void RemoveLeading(AssuranceLines lines, int count)
    {
        if (count < lines.Count)
        {
            lines.RemoveRange(0, count);
            return;
        }

        lines.RemoveRange(0, lines.Count - 1);
        lines.Replace(0, string.Empty);
    }
}
