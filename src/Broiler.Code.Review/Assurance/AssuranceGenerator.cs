using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Broiler.Code.Review.Assurance;

/// <summary>Which of the generated artefacts one is.</summary>
public enum AssuranceArtefactKind
{
    /// <summary>A covered source file: its header and its annotation blocks.</summary>
    Source = 0,

    /// <summary>The component report, <c>CODE-ASSURANCE.md</c> by default.</summary>
    Report,

    /// <summary>The unit manifest, <c>assurance.manifest.json</c> by default.</summary>
    Manifest,

    /// <summary>The per-unit decision record, <c>HUMAN_REVIEW.md</c> by default.</summary>
    HumanReview,
}

/// <summary>One generated artefact: what is there now, and what the generator would write.</summary>
/// <param name="RelativePath">Root-relative, forward slashes.</param>
/// <param name="Kind">Which artefact it is.</param>
/// <param name="Exists">Whether the file is there at all.</param>
/// <param name="Current">Its text now; empty when it does not exist.</param>
/// <param name="Desired">Its text as the generator would write it.</param>
public sealed record AssuranceArtefact(
    string RelativePath, AssuranceArtefactKind Kind, bool Exists, string Current, string Desired)
{
    /// <summary>True when the artefact is already what the generator would write.</summary>
    public bool IsCurrent => Exists && string.Equals(Current, Desired, StringComparison.Ordinal);
}

/// <summary>One covered file through the generator: its units before and after.</summary>
/// <param name="Source">The file as read.</param>
/// <param name="Scan">The scan of the text as read.</param>
/// <param name="Before">Its units as read. Empty rules are reported against these, with these line numbers.</param>
/// <param name="Desired">Its text as the generator would write it; the text as read when it refused.</param>
/// <param name="FileFingerprint">The file fingerprint of <paramref name="Desired"/>.</param>
/// <param name="After">Its units in <paramref name="Desired"/>, in the same order as <paramref name="Before"/>.</param>
/// <param name="Indexable">
/// False when the parser and the line model count lines differently, so no
/// block could be read by line number. Such a file is left as it is.
/// </param>
public sealed record AssurancePlannedFile(
    AssuranceSource Source,
    AssuranceScannedFile Scan,
    IReadOnlyList<AssuranceCorpusUnit> Before,
    string Desired,
    string FileFingerprint,
    IReadOnlyList<AssuranceCorpusUnit> After,
    bool Indexable);

/// <summary>
/// What the generator would leave behind: every artefact, the files and units
/// before and after, and every reason it refused.
/// </summary>
/// <param name="Context">The component-level facts the prose was written from.</param>
/// <param name="Artefacts">
/// One per covered file in path order, then the report, the manifest and the
/// human-review record: the owning component's order.
/// </param>
/// <param name="Files">The covered files, in path order.</param>
/// <param name="UnitsBefore">Every unit as read.</param>
/// <param name="UnitsAfter">Every unit as the generator would leave it. The component artefacts describe these.</param>
/// <param name="Problems">
/// Why the generator refused, per file. A plan with problems must not be
/// applied: the refused files are carried through unchanged, and writing the
/// rest would leave a record that disagrees with them.
/// </param>
public sealed record AssurancePlan(
    AssuranceReportContext Context,
    IReadOnlyList<AssuranceArtefact> Artefacts,
    IReadOnlyList<AssurancePlannedFile> Files,
    IReadOnlyList<AssuranceCorpusUnit> UnitsBefore,
    IReadOnlyList<AssuranceCorpusUnit> UnitsAfter,
    IReadOnlyList<AssuranceViolation> Problems)
{
    /// <summary>The artefacts the generator would change or create.</summary>
    public IEnumerable<AssuranceArtefact> Changes => Artefacts.Where(static artefact => !artefact.IsCurrent);
}

/// <summary>
/// The generator, as a pure function from the component's text to what every
/// generated artefact should contain.
///
/// The check calls the same function and compares, so what the check accepts is
/// by construction what the generator writes. Per file: refresh every attached
/// block (fill the fingerprint, move a decision the code has outrun to
/// <c>STALE</c>, re-align the three lines), rescan, remove the old header,
/// and write a new one counting the refreshed states. Counting the states this
/// pass is about to produce, rather than the ones it found, is what makes one
/// run a fixed point. Then the report, the manifest and the human-review
/// record, from every file's units as the pass leaves them.
///
/// The format is the owning component's, byte for byte, and so is every
/// transition, except that a bare alias is bound only to the version the
/// machine line records (see <see cref="AssuranceHumanLine"/>). Where that
/// generator would throw (an undefined human line, an invented reviewer) or
/// delete a comment it cannot prove it wrote, this refuses the file and says
/// why, or leaves the comment where it is; nothing is written while any
/// refusal stands.
/// </summary>
public static class AssuranceGenerator
{
    /// <summary>The line a generated report, record or manifest carries, and a hand-written one does not.</summary>
    public const string GeneratedNotice = "GENERATED - DO NOT EDIT MANUALLY";

    /// <summary>Computes the plan. Reads nothing but its arguments and writes nothing.</summary>
    public static AssurancePlan Plan(AssuranceCorpus corpus, IAssuranceFileScanner scanner, AssuranceComponentConfig config)
    {
        ArgumentNullException.ThrowIfNull(corpus);
        ArgumentNullException.ThrowIfNull(scanner);
        ArgumentNullException.ThrowIfNull(config);

        List<AssuranceSource> sources = [.. corpus.Files.OrderBy(static file => file.RelativePath, StringComparer.Ordinal)];
        (string generate, string check) = AssuranceReportContext.CommandsFor(config);
        var context = new AssuranceReportContext(
            config.Component,
            config.Artefacts,
            generate,
            check,
            [.. sources.Select(static source => source.RelativePath)],
            corpus.Excluded,
            corpus.Assemblies,
            config.ClosedToEscapeHatch)
        {
            SeparateRecords = corpus.SeparateRecords,
        };

        var problems = new List<AssuranceViolation>();
        var files = new List<AssurancePlannedFile>(sources.Count);
        var artefacts = new List<AssuranceArtefact>(sources.Count + 3);

        foreach (AssuranceSource source in sources)
        {
            AssurancePlannedFile file = PlanFile(source, scanner, config, problems);
            files.Add(file);
            artefacts.Add(new AssuranceArtefact(source.RelativePath, AssuranceArtefactKind.Source, true, source.Text, file.Desired));
        }

        List<AssuranceCorpusUnit> before = [.. files.SelectMany(static file => file.Before)];
        List<AssuranceCorpusUnit> after = [.. files.SelectMany(static file => file.After)];

        artefacts.Add(Artefact(corpus, config.Artefacts.Report, AssuranceArtefactKind.Report,
            AssuranceComponentReport.Render(context, after)));

        artefacts.Add(Artefact(corpus, config.Artefacts.Manifest, AssuranceArtefactKind.Manifest,
            AssuranceManifest.Render(
                config.ManifestComment ?? AssuranceManifest.Comment(generate, check, corpus.Assemblies.Count),
                files.Select(static file => new AssuranceManifestFile(file.Source.RelativePath, file.FileFingerprint)),
                after)));

        artefacts.Add(Artefact(corpus, config.Artefacts.HumanReview, AssuranceArtefactKind.HumanReview,
            AssuranceHumanReviewRecord.Render(context, after)));

        return new AssurancePlan(context, artefacts, files, before, after, problems);
    }

    /// <summary>
    /// True when <paramref name="text"/> says it was generated: one of its
    /// first ten lines opens with <see cref="GeneratedNotice"/>, after
    /// whitespace and, for the manifest's <c>$comment</c>, a quote. A
    /// component-level artefact without it was written by a person, and the
    /// generator does not replace it unless told to adopt it.
    /// </summary>
    public static bool IsGenerated(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var lines = new AssuranceLines(text);
        for (int index = 0; index < lines.Count && index < 10; index++)
        {
            if (lines[index].Trim().TrimStart('"').StartsWith(GeneratedNotice, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    /// <summary>The <c>artefacts</c> property of the configuration that names an artefact of this kind.</summary>
    public static string ConfigKey(AssuranceArtefactKind kind) => kind switch
    {
        AssuranceArtefactKind.Report => "report",
        AssuranceArtefactKind.Manifest => "manifest",
        AssuranceArtefactKind.HumanReview => "humanReview",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "A source file is not named in the configuration's artefacts."),
    };

    /// <summary>
    /// The first line where an artefact and its regeneration part company, in
    /// the owning component's words, naming the command that fixes it.
    ///
    /// An artefact that does not exist is compared as an empty text, as the
    /// owning component compares it, so the message is that component's (line
    /// 1, nothing on disk); a line saying that the file is missing follows,
    /// because an empty "on disk" does not say it.
    /// </summary>
    public static string Describe(AssuranceArtefact artefact, string generateCommand)
    {
        ArgumentNullException.ThrowIfNull(artefact);
        ArgumentNullException.ThrowIfNull(generateCommand);

        return artefact.Exists
            ? Describe(artefact.RelativePath, artefact.Current, artefact.Desired, 0, generateCommand)
            : Describe(artefact.RelativePath, string.Empty, artefact.Desired, 0, generateCommand,
                $"{artefact.RelativePath} does not exist, and the generator would write it.");
    }

    /// <summary>
    /// <see cref="Describe(AssuranceArtefact, string)"/> over two texts, with
    /// <paramref name="lineOffset"/> added to the line number reported, for a
    /// comparison of part of a file, and <paramref name="note"/>, when given,
    /// as a line of its own before the command.
    /// </summary>
    internal static string Describe(
        string path, string current, string desired, int lineOffset, string generateCommand, string? note = null)
    {
        var onDisk = new AssuranceLines(current);
        var generated = new AssuranceLines(desired);
        string noted = note is null ? string.Empty : $"\n  {note}";

        for (int line = 0; line < Math.Max(onDisk.Count, generated.Count); line++)
        {
            string left = line < onDisk.Count ? onDisk[line] : "<end of file>";
            string right = line < generated.Count ? generated[line] : "<end of file>";

            if (!string.Equals(left, right, StringComparison.Ordinal))
            {
                return string.Create(
                    CultureInfo.InvariantCulture,
                    $"{path}({line + 1 + lineOffset}) is not what the generator would write." +
                    $"\n  on disk:   {left}" +
                    $"\n  generated: {right}{noted}" +
                    $"\n  Run: {generateCommand}");
            }
        }

        // Every line reads the same, so the lines end differently. The owning
        // component says "differs in length only" here, which is true and does
        // not say what to look at.
        return $"{path} differs from what the generator would write in its line endings only\n  Run: {generateCommand}";
    }

    /// <summary>The 1-based line of the first difference between two texts, or null.</summary>
    internal static int? FirstDifference(string current, string desired)
    {
        var onDisk = new AssuranceLines(current);
        var generated = new AssuranceLines(desired);

        for (int line = 0; line < Math.Max(onDisk.Count, generated.Count); line++)
        {
            if (line >= onDisk.Count || line >= generated.Count ||
                !string.Equals(onDisk[line], generated[line], StringComparison.Ordinal))
            {
                return line + 1;
            }
        }

        return null;
    }

    private static AssuranceArtefact Artefact(
        AssuranceCorpus corpus, string path, AssuranceArtefactKind kind, string desired)
    {
        bool exists = corpus.Artefacts.TryGetValue(path, out string? current);
        return new AssuranceArtefact(path, kind, exists, current ?? string.Empty, desired);
    }

    private static AssurancePlannedFile PlanFile(
        AssuranceSource source,
        IAssuranceFileScanner scanner,
        AssuranceComponentConfig config,
        List<AssuranceViolation> problems)
    {
        string path = source.RelativePath;
        string text = source.Text;

        AssuranceScannedFile scan = scanner.ScanFile(text, path);
        var lines = new AssuranceLines(text);
        IReadOnlyList<AssuranceCorpusUnit> before = Units(source, lines, scan);

        AssurancePlannedFile Unchanged(bool indexable = true) =>
            new(source, scan, before, text, scan.FileFingerprint, before, indexable);

        if (scan.LineCount != lines.Count)
        {
            problems.Add(new AssuranceViolation(
                "IO",
                path,
                null,
                $"{path} breaks lines on U+0085, U+2028 or U+2029, which the parser counts as line breaks and the " +
                "annotation line model does not, so no block in it can be read or written by line number. " +
                "Replace those characters with escapes."));
            return Unchanged(indexable: false);
        }

        if (config.SpdxFor(path) is not { } spdx)
        {
            problems.Add(new AssuranceViolation(
                "J5",
                path,
                null,
                $"{AssuranceComponentConfig.FileName} states no SPDX lines for {path}: set \"spdx\", or an " +
                "\"spdxOverrides\" entry that matches it."));
            return Unchanged();
        }

        string? refreshed = Refresh(text, before, problems);
        if (refreshed is null)
            return Unchanged();

        // The header counts the states this pass leaves, so it is computed from
        // the refreshed text. The old header is still in it, which moves nothing:
        // a comment is trivia, in no unit and no fingerprint.
        IReadOnlyList<AssuranceCorpusUnit> counted = string.Equals(refreshed, text, StringComparison.Ordinal)
            ? before
            : Units(source, new AssuranceLines(refreshed), scanner.ScanFile(refreshed, path));

        var generated = new AssuranceLines(refreshed);
        string? refusal = AssuranceHeader.Strip(
            generated, AssuranceBanner.SpdxLines(spdx), config.ForgeryVocabulary, path, out IReadOnlyList<string> separators);

        if (refusal is not null)
        {
            problems.Add(new AssuranceViolation("J5", path, 1, refusal));
            return Unchanged();
        }

        AssuranceHeader.Insert(
            generated,
            [.. AssuranceBanner.Render(AssuranceSummary.Of(counted), spdx), string.Empty],
            separators);

        string desired = generated.Render();
        if (string.Equals(desired, text, StringComparison.Ordinal))
            return new AssurancePlannedFile(source, scan, before, text, scan.FileFingerprint, before, true);

        AssuranceScannedFile rescanned = scanner.ScanFile(desired, path);
        var desiredLines = new AssuranceLines(desired);
        IReadOnlyList<AssuranceCorpusUnit> after = Units(source, desiredLines, rescanned);

        // What the generator writes is comments. If the code reads differently
        // afterwards, something was written somewhere it must not be, and the
        // file is refused rather than written. And the header must describe the
        // file it heads: if what it counted is not what the written file holds
        // (a block went with a removed comment run, say), it would publish
        // figures for a file that no longer exists.
        string? change = ChangedCode(scan, rescanned, desiredLines);
        if (change is null && AssuranceSummary.Of(after) != AssuranceSummary.Of(counted))
            change = "the header would count annotations the written file does not carry";

        if (change is not null)
        {
            problems.Add(new AssuranceViolation(
                "J5",
                path,
                null,
                $"{path} would not scan the same after generation ({change}); the generator refused it."));
            return Unchanged();
        }

        return new AssurancePlannedFile(source, scan, before, desired, rescanned.FileFingerprint, after, true);
    }

    /// <summary>
    /// The text with every attached block brought up to date, one line for
    /// one line so every line number stays valid; null when a human line is
    /// one the generator must not rewrite, with the reason added to
    /// <paramref name="problems"/>.
    /// </summary>
    private static string? Refresh(string text, IReadOnlyList<AssuranceCorpusUnit> units, List<AssuranceViolation> problems)
    {
        var lines = new AssuranceLines(text);
        bool refused = false;

        foreach (AssuranceCorpusUnit unit in units)
        {
            if (unit.Annotation is not { } annotation)
                continue;

            string indent = AssuranceLines.IndentOf(lines[annotation.AiLine]);
            string fingerprint = unit.Fingerprint;

            lines.Replace(annotation.AiLine, AssuranceAnnotation.RenderAiLine(indent, RefreshedFields(annotation, fingerprint)));

            if (annotation.FalsifiedIfLine is { } criterion)
                lines.Replace(criterion, AssuranceAnnotation.RenderFalsifiedIfLine(indent, annotation.Criterion));

            string body;
            try
            {
                body = AssuranceHumanLine.Refreshed(annotation, fingerprint, unit.Where);
                AssuranceHumanLine.RefuseInventedApproval(unit.Where, annotation.HumanBody, body);
            }
            catch (AssuranceRefusalException refusal)
            {
                problems.Add(new AssuranceViolation("J4", unit.File, annotation.HumanLine + 1, refusal.Message));
                refused = true;
                continue;
            }

            lines.Replace(annotation.HumanLine, AssuranceAnnotation.RenderHumanLine(indent, body));
        }

        return refused ? null : lines.Render();
    }

    /// <summary>
    /// The machine line's fields with every <c>Fingerprint</c> set to the
    /// unit's current value, whatever it held (<c>TBF</c> or an outdated
    /// value). Every other field is an assessment, carried through in source
    /// order. A block with no Fingerprint field does not get one: it stays
    /// assessed but unbound until someone adds the field.
    /// </summary>
    public static IEnumerable<AssuranceField> RefreshedFields(AssuranceAnnotation annotation, string currentFingerprint)
    {
        ArgumentNullException.ThrowIfNull(annotation);
        ArgumentNullException.ThrowIfNull(currentFingerprint);

        return annotation.Fields.Select(field =>
            string.Equals(field.Key, AssuranceVocabulary.FingerprintField, StringComparison.Ordinal)
                ? field with { Value = currentFingerprint }
                : field);
    }

    private static IReadOnlyList<AssuranceCorpusUnit> Units(
        AssuranceSource source, AssuranceLines lines, AssuranceScannedFile scan) =>
        [.. AssuranceCandidates.Classify(lines, scan)
            .Select(candidate => new AssuranceCorpusUnit(source.RelativePath, source.Assembly, candidate))];

    private static string? ChangedCode(AssuranceScannedFile before, AssuranceScannedFile after, AssuranceLines lines)
    {
        if (after.LineCount != lines.Count)
            return "the parser and the line model count different lines";

        if (!string.Equals(before.FileFingerprint, after.FileFingerprint, StringComparison.Ordinal))
            return $"the file fingerprint moved from {before.FileFingerprint} to {after.FileFingerprint}";

        if (before.Units.Count != after.Units.Count)
            return "the number of code units changed";

        for (int index = 0; index < before.Units.Count; index++)
        {
            AssuranceScannedUnit was = before.Units[index].Unit;
            AssuranceScannedUnit now = after.Units[index].Unit;
            if (!string.Equals(was.Name, now.Name, StringComparison.Ordinal) ||
                !string.Equals(was.Fingerprint, now.Fingerprint, StringComparison.Ordinal) ||
                !string.Equals(was.Exemption, now.Exemption, StringComparison.Ordinal))
            {
                return $"unit {was.Name} changed";
            }
        }

        return null;
    }
}
