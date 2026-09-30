// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           6
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         8/6
// Resource impact:  8/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Review.Cli.Assurance;

/// <summary>
/// One component-level artefact as it is on disk, with what it takes to write
/// it back in the same style.
/// </summary>
/// <param name="RelativePath">Root-relative, forward slashes.</param>
/// <param name="FullPath">Where it is.</param>
/// <param name="Source">The file as read, or null when it does not exist.</param>
/// <param name="NewLine">
/// The ending its lines use: the first one it has, or LF for a new file. The
/// generator renders LF; a checkout that converts to CRLF gets CRLF back, and
/// the comparison is made in LF, so neither kind of checkout reports every line
/// of a current file as different.
/// </param>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=FFA75F
// Broiler-Falsified-If: a text the generator rendered in LF is written with LF lines to a file whose lines end in CRLF
// Broiler-Human:        PENDING
internal sealed record ComponentArtefact(string RelativePath, string FullPath, AssuranceSourceText? Source, string NewLine)
{
    /// <summary><paramref name="desired"/>, which the generator rendered in LF, in this file's line endings.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=20C81C
    // Broiler-Falsified-If: a text the generator rendered in LF is written with LF lines to a file whose lines end in CRLF
    // Broiler-Human:        PENDING
    public string Restyle(string desired) =>
        NewLine == "\r\n" ? desired.Replace("\n", "\r\n", StringComparison.Ordinal) : desired;
}

/// <summary>
/// A component read from disk for the generator: the covered files as
/// <see cref="AssuranceSource"/>, the component-level artefacts, and every
/// file that could not be read, which the generator refuses over.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=ABFA27
// Broiler-Falsified-If: an artefact path whose directory is a junction leading outside the component root is written through it
// Broiler-Human:        PENDING
internal sealed class ComponentCorpus
{
    private ComponentCorpus(
        AssuranceCorpus corpus,
        ComponentSourceSet set,
        IReadOnlyDictionary<string, (ComponentSourceFile File, AssuranceSourceText Text)> sources,
        IReadOnlyDictionary<string, ComponentArtefact> artefacts,
        IReadOnlyList<AssuranceViolation> problems)
    {
        Corpus = corpus;
        Set = set;
        Sources = sources;
        Artefacts = artefacts;
        Problems = problems;
    }

    public AssuranceCorpus Corpus { get; }

    public ComponentSourceSet Set { get; }

    /// <summary>The covered files that were read, by root-relative path.</summary>
    public IReadOnlyDictionary<string, (ComponentSourceFile File, AssuranceSourceText Text)> Sources { get; }

    /// <summary>The three component-level artefacts, by root-relative path.</summary>
    public IReadOnlyDictionary<string, ComponentArtefact> Artefacts { get; }

    /// <summary>Files that could not be read as UTF-8. They are missing from <see cref="Corpus"/>.</summary>
    public IReadOnlyList<AssuranceViolation> Problems { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=265A60
    // Broiler-Falsified-If: a covered file that is not valid UTF-8 is left out of the corpus with no IO entry in Problems, so generate writes the rest and check reports nothing about it
    // Broiler-Human:        PENDING
    public static ComponentCorpus Load(string root, AssuranceComponentConfig config)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(config);

        ComponentSourceSet set = ComponentSources.Discover(root, config);
        var problems = new List<AssuranceViolation>();
        var sources = new Dictionary<string, (ComponentSourceFile, AssuranceSourceText)>(StringComparer.Ordinal);
        var files = new List<AssuranceSource>(set.Files.Count);

        foreach (ComponentSourceFile file in set.Files)
        {
            if (!AssuranceSourceText.TryRead(file.FullPath, out AssuranceSourceText? text, out string? problem))
            {
                problems.Add(new AssuranceViolation("IO", file.RelativePath, null, $"{file.RelativePath} {problem}"));
                continue;
            }

            sources[file.RelativePath] = (file, text!);
            files.Add(new AssuranceSource(file.RelativePath, file.Project.AssemblyName, text!.Text));
        }

        var artefacts = new Dictionary<string, ComponentArtefact>(StringComparer.Ordinal);
        var current = new Dictionary<string, string>(StringComparer.Ordinal);
        string fullRoot = Path.GetFullPath(root);

        foreach (string path in new[] { config.Artefacts.Report, config.Artefacts.Manifest, config.Artefacts.HumanReview })
        {
            string full = Path.Combine(fullRoot, path.Replace('/', Path.DirectorySeparatorChar));
            CheckWritable(fullRoot, path, full);

            if (!File.Exists(full))
            {
                artefacts[path] = new ComponentArtefact(path, full, null, "\n");
                continue;
            }

            if (!AssuranceSourceText.TryRead(full, out AssuranceSourceText? text, out string? problem))
            {
                problems.Add(new AssuranceViolation("IO", path, null, $"{path} {problem}"));
                artefacts[path] = new ComponentArtefact(path, full, null, "\n");
                continue;
            }

            string newLine = new AssuranceLines(text!.Text).NewLine;
            artefacts[path] = new ComponentArtefact(path, full, text, newLine);
            current[path] = newLine == "\r\n"
                ? text.Text.Replace("\r\n", "\n", StringComparison.Ordinal)
                : text.Text;
        }

        var corpus = new AssuranceCorpus(
            files,
            [.. set.Excluded.Select(static excluded => new AssuranceExcludedSource(excluded.RelativePath, excluded.Reason))],
            [.. set.Projects.Select(static project => project.AssemblyName).Distinct(StringComparer.Ordinal)],
            current)
        {
            SeparateRecords = SeparateRecords(fullRoot, config.Artefacts),
        };

        return new ComponentCorpus(corpus, set, sources, artefacts, problems);
    }

    /// <summary>
    /// Refuses an artefact path the generator must not write: one that passes
    /// through a junction or symbolic link (which can lead out of the root), a
    /// <c>.git</c> directory, or a directory holding a <c>.git</c> entry (another
    /// component's checkout), or that is itself a link. The configuration has
    /// already refused a rooted path and a <c>..</c> segment; these are the
    /// questions only the disk can answer.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E072E0
    // Broiler-Falsified-If: an artefact path that is itself a symbolic link passes without a ComponentSourceException
    // Broiler-Human:        PENDING
    private static void CheckWritable(string root, string relative, string full)
    {
        string? directory = Path.GetDirectoryName(relative.Replace('/', Path.DirectorySeparatorChar))?.Replace('\\', '/');
        if (directory is { Length: > 0 } && ComponentSources.Unenterable(root, directory) is { } barrier)
        {
            throw new ComponentSourceException(
                $"{AssuranceComponentConfig.FileName}: the artefact '{relative}' lies inside '{barrier.Directory}', which " +
                $"{barrier.What}; the generator writes nothing there");
        }

        if (ComponentSources.IsLink(full))
        {
            throw new ComponentSourceException(
                $"{AssuranceComponentConfig.FileName}: the artefact '{relative}' is a symbolic link; the generator " +
                "writes nothing through a link");
        }
    }

    /// <summary>
    /// A hand-written review record at an artefact's default path while the
    /// configuration writes that artefact elsewhere: a component that keeps a
    /// signed <c>HUMAN_REVIEW.md</c> and points the per-unit record at another
    /// file. A file there the generator wrote is not one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=410865
    // Broiler-Falsified-If: a hand-written HUMAN_REVIEW.md at the default path is left out of the result while the configuration points the per-unit record at another file
    // Broiler-Human:        PENDING
    private static IReadOnlyList<string> SeparateRecords(string root, AssuranceArtefactPaths paths)
    {
        var configured = new[] { paths.Report, paths.HumanReview, paths.Manifest };
        var records = new List<string>();

        foreach (string path in new[] { new AssuranceArtefactPaths().HumanReview, new AssuranceArtefactPaths().Report })
        {
            if (configured.Contains(path, StringComparer.OrdinalIgnoreCase))
                continue;

            string full = Path.Combine(root, path);
            if (File.Exists(full) && AssuranceSourceText.TryRead(full, out AssuranceSourceText? text, out _) &&
                !AssuranceGenerator.IsGenerated(text!.Text))
            {
                records.Add(path);
            }
        }

        return records;
    }

    /// <summary>
    /// The four-digit numbers of the records in the component's ADR directory:
    /// the part of each <c>*.md</c> file name before its first <c>-</c>. Empty
    /// when the directory does not exist, so that every citation is reported.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=036BD4
    // Broiler-Falsified-If: an ADR number resolves when the ADR directory holds no *.md file whose name begins with that number followed by a hyphen
    // Broiler-Human:        PENDING
    public static IReadOnlySet<string> AdrRecords(string root, string directory)
    {
        string full = Path.Combine(Path.GetFullPath(root), directory.Replace('/', Path.DirectorySeparatorChar));
        if (!Directory.Exists(full))
            return new HashSet<string>(StringComparer.Ordinal);

        return Directory.EnumerateFiles(full, "*.md")
            .Select(static path => Path.GetFileName(path).Split('-')[0])
            .ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>
    /// Writes one changed artefact, keeping its byte-order mark and line
    /// endings, and refusing when it changed on disk since it was read (or,
    /// for a new one, appeared).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=3D2205
    // Broiler-Falsified-If: a new artefact file that appeared on disk after Load is overwritten instead of refused
    // Broiler-Human:        PENDING
    public bool TryWrite(AssuranceArtefact artefact, out string? problem)
    {
        ArgumentNullException.ThrowIfNull(artefact);

        if (artefact.Kind == AssuranceArtefactKind.Source)
        {
            (ComponentSourceFile file, AssuranceSourceText text) = Sources[artefact.RelativePath];
            return text.TryWrite(file.FullPath, artefact.Desired, out problem);
        }

        ComponentArtefact target = Artefacts[artefact.RelativePath];
        if (target.Source is not null)
            return target.Source.TryWrite(target.FullPath, target.Restyle(artefact.Desired), out problem);

        problem = null;
        try
        {
            if (File.Exists(target.FullPath))
            {
                problem = "appeared on disk while the generator ran; nothing was written to it";
                return false;
            }

            string? directory = Path.GetDirectoryName(target.FullPath);
            if (directory is { Length: > 0 })
                Directory.CreateDirectory(directory);

            File.WriteAllBytes(target.FullPath, AssuranceSourceText.Encode(target.Restyle(artefact.Desired), false));
            return true;
        }
        catch (IOException exception)
        {
            problem = string.Create(CultureInfo.InvariantCulture, $"could not be written: {exception.Message}");
            return false;
        }
        catch (UnauthorizedAccessException exception)
        {
            problem = string.Create(CultureInfo.InvariantCulture, $"could not be written: {exception.Message}");
            return false;
        }
    }
}
