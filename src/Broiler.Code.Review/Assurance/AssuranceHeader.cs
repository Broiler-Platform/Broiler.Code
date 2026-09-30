// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           0
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    High
// Criteria:         20/15
// Resource impact:  5/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// Finding, removing and writing the generated block at the top of a covered
/// file, and recognizing a forged one anywhere else in it.
///
/// The removal follows the owning component's generator: a file whose first
/// line is an SPDX line has its leading <c>//</c> run removed through the
/// <c>GENERATED</c> marker and one blank line. That generator then removes
/// every further leading comment run that reads like a summary block; this one
/// removes a further run only when it is a copy of the generated block itself
/// (every line one the generator writes, through a marker of its own). Where
/// that generator would delete something it cannot prove it wrote, this
/// leaves it or refuses instead:
///
/// <list type="bullet">
/// <item>a comment run under the header that merely uses the summary's
/// words — a licence notice mentioning an OSI-approved licence, a comment
/// opening with <c>Exempt:</c>, a documentation comment, a block — is left
/// where it is, and the check reports it if it reads as a summary;</item>
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
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=6ED41D
// Broiler-Falsified-If: Strip deletes a copyright line that the configuration does not state for the file without returning a refusal
// Broiler-Human:        PENDING
public static class AssuranceHeader
{
    /// <summary>
    /// The nine row labels in the narrow vocabulary's form: lower case, a hyphen
    /// read as a space, without the colon.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=4415CB
    // Broiler-Falsified-If: a normalized label keeps its trailing colon or its upper case, so a comment opening 'relevant units : 3' below the header is not reported
    // Broiler-Human:        PENDING
    private static readonly string[] NormalizedRowLabels =
        [.. AssuranceBanner.RowLabels.Select(static label => label.TrimEnd(':').Replace('-', ' ').ToLowerInvariant())];
    /// <summary>
    /// The owning component's summary vocabulary, matched case-insensitively
    /// anywhere in a comment line. Its <c>reviewer</c> and <c>approved</c> are
    /// what makes it strict: in code that talks about reviews they match
    /// ordinary comments.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DAF119
    // Broiler-Falsified-If: a term holds an upper-case letter, so IsSummaryComment, which compares terms ordinally against lower-cased text, never matches it
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AA4C2E
    // Broiler-Falsified-If: an opening keeps the leading slashes and space of the banner or the marker, so a line whose comment body opens with the banner text is not accepted by IsSummaryLine
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=35AD38
    // Broiler-Falsified-If: under the narrow vocabulary a '//' line whose body opens with 'unverified:' in lower case is not accepted as a summary line
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=AF95F5
    // Broiler-Falsified-If: an indented or lower-case copy of the banner line is not counted
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9E099B
    // Broiler-Falsified-If: a text carrying two banner lines returns null
    // Broiler-Human:        PENDING
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
    /// True for one line of comment text that reads like part of a generated
    /// summary, wherever the comment is and whatever delimits it: a
    /// <c>//</c>, <c>///</c> or <c>/* */</c> comment, or disabled text.
    ///
    /// The text is compared after its delimiters are removed, its whitespace
    /// collapsed and its case folded, so rewording by spacing, tabs, case or a
    /// hyphen does not hide it. Under the narrow vocabulary it is a summary
    /// line when it mentions the banner or the marker anywhere, or when it
    /// opens with one of the nine row labels followed by a colon (a space may
    /// stand for the hyphen in <c>Human-reviewed</c>, and spaces may stand
    /// before the colon). The strict vocabulary adds the owning component's
    /// words anywhere in the line.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E77B11
    // Broiler-Falsified-If: a comment 'Relevant-Units : 3', with a hyphen and a space before the colon, is not reported under the narrow vocabulary
    // Broiler-Human:        PENDING
    public static bool IsSummaryComment(string comment, AssuranceForgeryVocabulary vocabulary)
    {
        ArgumentNullException.ThrowIfNull(comment);

        string content = CommentContent(comment);
        if (content.Length == 0)
            return false;

        if (vocabulary == AssuranceForgeryVocabulary.Strict &&
            StrictVocabulary.Any(term => content.Contains(term, StringComparison.Ordinal)))
        {
            return true;
        }

        // Every run of anything but a letter or a digit read as one space, so
        // "Broiler-Code  Assurance" and "GENERATED: do not edit, manually" are
        // the phrases they imitate.
        string words = WordsOf(content);
        if (words.Contains("broiler code assurance", StringComparison.Ordinal) ||
            words.Contains("generated do not edit manually", StringComparison.Ordinal))
        {
            return true;
        }

        string labelled = content.Replace('-', ' ');
        foreach (string label in NormalizedRowLabels)
        {
            if (!labelled.StartsWith(label, StringComparison.Ordinal))
                continue;

            if (labelled[label.Length..].TrimStart().StartsWith(':'))
                return true;
        }

        return false;
    }

    /// <summary>
    /// A violation naming the first summary comment line below the generated
    /// marker, and how many there are; otherwise null. Below the header, the
    /// generator neither writes nor strips anything, so a summary block there
    /// survives every generation and only this finds it.
    ///
    /// The comments are read from the parser's trivia, so a row label inside a
    /// string is not one, and one inside a block comment, a documentation
    /// comment or disabled text is.
    /// </summary>
    /// <param name="path">The file, for the message.</param>
    /// <param name="text">The file's text, to find its header.</param>
    /// <param name="comments">Every comment line of the file, as the scanner reports them.</param>
    /// <param name="vocabulary">What marks a line as a summary line.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=F41069
    // Broiler-Falsified-If: a row-label comment inside a documentation comment below the generated marker returns null
    // Broiler-Human:        PENDING
    public static AssuranceViolation? ForgedSummary(
        string path, string text, IReadOnlyList<AssuranceCommentLine> comments, AssuranceForgeryVocabulary vocabulary)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(comments);

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

        AssuranceCommentLine? first = null;
        int count = 0;
        foreach (AssuranceCommentLine comment in comments)
        {
            if (comment.Line <= header || !IsSummaryComment(comment.Text, vocabulary))
                continue;

            first ??= comment;
            count++;
        }

        return first is not { } found
            ? null
            : new AssuranceViolation(
                "J5",
                path,
                found.Line + 1,
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"{path}({found.Line + 1}) carries the assurance summary line '{found.Text.Trim()}' below the generated header, and {count} such line(s) sit there; the generated block is the only one a file may carry"));
    }

    /// <summary>
    /// A comment line without its delimiters, whitespace collapsed to single
    /// spaces, in lower case: <c>"  //\tHuman-reviewed : 3/3"</c> is
    /// <c>"human-reviewed : 3/3"</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=54B271
    // Broiler-Falsified-If: a block-comment line ' * Exempt: 3 */' comes back still holding its asterisks or closing delimiter, so the label check misses it
    // Broiler-Human:        PENDING
    private static string CommentContent(string comment)
    {
        string text = comment.Trim();
        if (text.EndsWith("*/", StringComparison.Ordinal))
            text = text[..^2];

        text = text.TrimStart('/', '*', '!').Trim();

        var content = new StringBuilder(text.Length);
        bool space = false;
        foreach (char character in text)
        {
            if (char.IsWhiteSpace(character))
            {
                space = content.Length > 0;
                continue;
            }

            if (space)
                content.Append(' ');

            space = false;
            content.Append(char.ToLowerInvariant(character));
        }

        return content.ToString();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9B9031
    // Broiler-Falsified-If: a run of two or more characters that are neither letters nor digits between two words comes back as more than one space
    // Broiler-Human:        PENDING
    private static string WordsOf(string content)
    {
        var words = new StringBuilder(content.Length);
        bool gap = false;
        foreach (char character in content)
        {
            if (char.IsLetterOrDigit(character))
            {
                if (gap && words.Length > 0)
                    words.Append(' ');

                gap = false;
                words.Append(character);
            }
            else
            {
                gap = true;
            }
        }

        return words.ToString();
    }

    /// <summary>
    /// The generated header of a text, from its first line through the marker,
    /// or nothing when it has none. This is the generated part of a source
    /// file, and all of it the review-claim rule reads there.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A8E833
    // Broiler-Falsified-If: a text that opens with a complete generated header is returned without its marker line
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=64DCF0
    // Broiler-Falsified-If: a second header under the first, whose copyright line names a holder the configuration does not state, is removed without a refusal
    // Broiler-Human:        PENDING
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
                    $"above it: state exactly the run's lines in spdx or in an spdxOverrides entry for this file, " +
                    $"so that the generated header replaces the run, or exclude the file.");
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

        // A copy of the generated block pasted directly under the real one
        // would otherwise be carried through verbatim by every generation, so
        // it goes too — but only a copy: every line of the run through its own
        // marker is a line the generator writes. A run that merely uses the
        // summary's words is somebody's comment. The owning component deletes
        // that as well, which on a second run took away a licence notice or a
        // documentation comment, and the assurance block under it, that the
        // first run had kept; it stays, and the check reports it if it reads
        // as a summary.
        while (LeadingRunIsGeneratedCopy(lines))
            RemoveLeadingCommentRun(lines);

        return null;
    }

    /// <summary>
    /// Inserts <paramref name="header"/> at the top of the file. Line
    /// <c>i</c> takes the ending of the old header's line <c>i</c> when there
    /// was one, and otherwise the ending of the line it lands above, so a file
    /// keeps its own line endings and a regeneration moves none of them.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=74ACD7
    // Broiler-Falsified-If: a CRLF file whose old header was stripped gets LF endings on the new header's lines
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FBBD80
    // Broiler-Falsified-If: a documentation line, or the machine line of an assurance block, counts as header-shaped
    // Broiler-Human:        PENDING
    private static bool IsHeaderShaped(string line) =>
        line.StartsWith(AssuranceBanner.SpdxCopyrightPrefix, StringComparison.Ordinal) ||
        line.StartsWith(AssuranceBanner.SpdxLicensePrefix, StringComparison.Ordinal) ||
        line is "//" or AssuranceBanner.Banner or AssuranceBanner.BannerRule or AssuranceBanner.GeneratedMarker ||
        AssuranceBanner.RowLabels.Any(label => line.StartsWith("// " + label, StringComparison.Ordinal));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A3504D
    // Broiler-Falsified-If: a leading run broken by a blank line is counted past that blank line
    // Broiler-Human:        PENDING
    private static int LeadingCommentRun(AssuranceLines lines)
    {
        int run = 0;
        while (run < lines.Count && lines[run].StartsWith("//", StringComparison.Ordinal))
            run++;

        return run;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=385A7F
    // Broiler-Falsified-If: a marker on a line after the leading comment run is returned
    // Broiler-Human:        PENDING
    private static int MarkerIn(AssuranceLines lines, int run)
    {
        for (int index = 0; index < run; index++)
        {
            if (string.Equals(lines[index], AssuranceBanner.GeneratedMarker, StringComparison.Ordinal))
                return index;
        }

        return -1;
    }

    /// <summary>
    /// True when the file opens with a copy of the generated block: a
    /// <c>//</c> run holding the marker, every line of it up to that marker a
    /// line the generator writes. A <c>///</c> line or an assurance block line
    /// is neither, so a run holding one is never a copy.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=BCE73F
    // Broiler-Falsified-If: a leading run that holds a documentation line or an assurance block line before its own marker is reported as a copy
    // Broiler-Human:        PENDING
    private static bool LeadingRunIsGeneratedCopy(AssuranceLines lines)
    {
        int marker = MarkerIn(lines, LeadingCommentRun(lines));
        if (marker < 0)
            return false;

        for (int index = 0; index <= marker; index++)
        {
            if (!IsHeaderShaped(lines[index]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Removes the leading <c>//</c> run through its own marker, and one blank
    /// line after it: the owning component's rule for a forged block, applied
    /// only to a copy of the generated one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F419FE
    // Broiler-Falsified-If: more than one blank line after the copy's marker is removed
    // Broiler-Human:        PENDING
    private static void RemoveLeadingCommentRun(AssuranceLines lines)
    {
        int run = LeadingCommentRun(lines);
        int marker = MarkerIn(lines, run);
        if (marker < 0)
            return;

        int through = marker + 1;

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FDFB4C
    // Broiler-Falsified-If: removing as many lines as the text holds leaves no line at all instead of one empty line
    // Broiler-Human:        PENDING
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
