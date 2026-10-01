// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   35
// Annotated:        35/35
// Exempt:           0
// Human-reviewed:   0/35
// IP risk:          Low
// Security risk:    High
// Criteria:         17/12
// Resource impact:  6/10 max
// Unverified:       35
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Broiler.Code.Language.CSharp.Assurance;
using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Review.Cli.Assurance;

/// <summary>
/// <c>broiler-review assurance</c>: the per-declaration assurance annotations
/// of one component.
///
/// <c>list</c> reports which relevant units carry no block yet, with what a
/// tool needs to write one. <c>insert</c> writes machine assessments above
/// those units, and cannot write anything on the human line but
/// <c>PENDING</c>: its input has no field for it. <c>generate</c> writes the
/// headers and the component artefacts, <c>check</c> compares the tree with
/// what <c>generate</c> would write and applies the rules, <c>status</c>
/// summarizes, and <c>prune</c> removes the annotation lines the rubric no
/// longer asks for.
///
/// Exit codes: 0 done (or, for <c>check</c>, nothing wrong), 1 something was
/// refused or violated (or, after a command's own writes, its JSON report
/// could not be written), 2 a usage or configuration error, before anything
/// was written.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=CCE5C4
// Broiler-Falsified-If: an insert entry naming a path outside the root, or a file the component does not cover, gets a block written into that file
// Broiler-Human:        PENDING
internal static partial class AssuranceCommand
{
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=734E8F
    // Broiler-Falsified-If: Done equals Refused or UsageError, so the exit code cannot tell a clean check from one that found a violation
    // Broiler-Human:        PENDING
    public const int Done = 0;

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=7C5B04
    // Broiler-Falsified-If: Refused is zero, so a check that found a violation exits as a success and the gate passes
    // Broiler-Human:        PENDING
    public const int Refused = 1;

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=8285EA
    // Broiler-Falsified-If: UsageError is zero, so a check given an unknown option exits as a success having applied no rule
    // Broiler-Human:        PENDING
    public const int UsageError = 2;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A71BA9
    // Broiler-Human:        PENDING
    private static readonly string[] ListOptions = ["--root", "--files", "--json"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=D1764A
    // Broiler-Human:        PENDING
    private static readonly string[] ListFlags = ["--all-units", "--strict"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=6F1CA6
    // Broiler-Human:        PENDING
    private static readonly string[] InsertOptions = ["--root", "--assessments", "--json"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=7499AB
    // Broiler-Human:        PENDING
    private static readonly string[] InsertFlags = ["--dry-run"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=4F6E0B
    // Broiler-Human:        PENDING
    private static readonly string[] GenerateOptions = ["--root"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=74287F
    // Broiler-Human:        PENDING
    private static readonly string[] GenerateFlags = ["--dry-run", "--adopt"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=DA0CC7
    // Broiler-Human:        PENDING
    private static readonly string[] CheckOptions = ["--root", "--config", "--json", "--annotation-prefix", "--annotation-limit"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C4F71A
    // Broiler-Human:        PENDING
    private static readonly string[] CheckFlags = ["--release", "--sources-only"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=6FCA8D
    // Broiler-Human:        PENDING
    private static readonly string[] StatusOptions = ["--root", "--config"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BA8517
    // Broiler-Human:        PENDING
    private static readonly string[] PruneOptions = ["--root", "--json"];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=786581
    // Broiler-Human:        PENDING
    private static readonly string[] PruneFlags = ["--dry-run"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=02034E
    // Broiler-Falsified-If: an unknown subcommand, or an option the subcommand does not accept, exits 0
    // Broiler-Human:        PENDING
    public static int Run(IReadOnlyList<string> args, TextWriter output, TextWriter error)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(error);

        if (args.Count == 0)
        {
            error.Write(Usage);
            return UsageError;
        }

        if (args[0] is "-h" or "--help" or "help")
        {
            output.Write(Usage);
            return Done;
        }

        try
        {
            return args[0] switch
            {
                "list" => List(Parse(args, ListOptions, ListFlags), output, error),
                "insert" => Insert(Parse(args, InsertOptions, InsertFlags), output, error),
                "generate" => Generate(Parse(args, GenerateOptions, GenerateFlags), output, error),
                "check" => Check(Parse(args, CheckOptions, CheckFlags), output, error),
                "status" => Status(Parse(args, StatusOptions, []), output, error),
                "prune" => Prune(Parse(args, PruneOptions, PruneFlags), output, error),
                _ => throw new UsageException($"unknown assurance command '{args[0]}'"),
            };
        }
        catch (UsageException exception)
        {
            error.WriteLine($"broiler-review assurance: {exception.Message}");
            error.Write(Usage);
            return UsageError;
        }
        catch (Exception exception) when (exception is AssuranceConfigException or ComponentSourceException or AssuranceInputException)
        {
            error.WriteLine($"broiler-review assurance: {exception.Message}");
            return UsageError;
        }
    }

    /// <summary>The file scanner a configuration asks for: its symbols, its exemption predicate and its named values.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8CC1F8
    // Broiler-Falsified-If: with no configuration the scanner is built with a predicate other than the strict one, so a unit the strict predicate counts as relevant is listed as exempt
    // Broiler-Human:        PENDING
    internal static CSharpAssuranceFileScanner ScannerFor(AssuranceComponentConfig? config) =>
        config is null
            ? new CSharpAssuranceFileScanner(null, AssuranceExemptionPredicate.Strict)
            : new CSharpAssuranceFileScanner(config.PreprocessorSymbols, config.ExemptionPredicate, config.NamedValues);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=2ADA2B
    // Broiler-Falsified-If: a run in which a covered file could not be read, or a --strict run whose --files list names an uncovered path, exits 0
    // Broiler-Human:        PENDING
    private static int List(Options options, TextWriter output, TextWriter error)
    {
        string root = RootOf(options);
        string? jsonPath = options.Value("--json");
        PrepareOut(jsonPath);

        AssuranceComponentConfig? config = LoadConfig(root);
        ComponentSourceSet set = ComponentSources.Discover(root, config);
        var notes = new List<string>(set.Notes);
        IReadOnlyList<ComponentUnknownPath>? unknown = null;

        if (options.Value("--files") is { } listFile)
        {
            if (!File.Exists(listFile))
                throw new UsageException($"--files: '{listFile}' does not exist");

            set = ComponentSources.Restrict(set, root, File.ReadAllLines(listFile), out unknown);
            foreach (ComponentUnknownPath path in unknown)
                notes.Add($"--files names '{path.Path}', which {path.Reason}");
        }

        CSharpAssuranceFileScanner scanner = ScannerFor(config);
        var files = new List<ListedFile>(set.Files.Count);
        foreach (ComponentSourceFile file in set.Files)
        {
            if (!AssuranceSourceText.TryRead(file.FullPath, out AssuranceSourceText? source, out string? problem))
            {
                files.Add(new ListedFile(file, [], $"the file {problem}"));
                continue;
            }

            AssuranceScannedFile scan = scanner.ScanFile(source!.Text, file.RelativePath);
            files.Add(new ListedFile(file, AssuranceCandidates.Classify(new AssuranceLines(source.Text), scan), null));
        }

        string component = ComponentName(root, config);
        bool allUnits = options.Has("--all-units");

        foreach (string note in notes)
            error.WriteLine($"broiler-review assurance: {note}");

        // A run that could not read a covered file has not listed everything
        // there is, and one told to be strict about its --files list has not
        // listed what it was asked for: neither is a clean "nothing left".
        bool unreadable = files.Any(static file => file.Problem is not null);
        bool strayed = options.Has("--strict") && unknown is { Count: > 0 };
        int exit = unreadable || strayed ? Refused : Done;

        if (jsonPath is not null)
            return WriteOut(jsonPath, AssuranceJson.List(component, set, files, allUnits, notes, unknown ?? []), output, error) ? exit : Refused;

        WriteListText(component, set, files, allUnits, output);
        return exit;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8A1CEC
    // Broiler-Falsified-If: a file whose read failed is left out of the text report instead of being printed with its problem
    // Broiler-Human:        PENDING
    private static void WriteListText(
        string component, ComponentSourceSet set, IReadOnlyList<ListedFile> files, bool allUnits, TextWriter output)
    {
        ListTotals totals = ListTotals.Of(files, allUnits);
        output.WriteLine(
            Invariant($"{component}: {totals.Files} files, {totals.Units} units, {totals.Relevant} relevant, ") +
            Invariant($"{totals.Annotated} annotated, {totals.Unannotated} unannotated, {totals.Insertable} insertable"));

        foreach (ListedFile file in files)
        {
            ListTotals counts = ListTotals.Of([file], allUnits);
            if (file.Problem is not null)
            {
                output.WriteLine($"{file.File.RelativePath}  unreadable: {file.Problem}");
                continue;
            }

            if (counts.Listed == 0)
                continue;

            output.WriteLine(
                Invariant($"{file.File.RelativePath}  relevant {counts.Relevant}  annotated {counts.Annotated}  ") +
                Invariant($"unannotated {counts.Unannotated}  insertable {counts.Insertable}"));

            foreach (AssuranceCandidate candidate in AssuranceJson.Listed(file, allUnits))
            {
                AssuranceScannedUnit unit = candidate.Unit;
                string where = Invariant($"{unit.DeclarationLine + 1}:{unit.DeclarationColumn + 1}");

                // A detail that already opens with its reason says it once.
                string status = candidate.Insertable
                    ? "insertable"
                    : candidate.Detail is null
                        ? candidate.Reason
                        : candidate.Detail.StartsWith(candidate.Reason, StringComparison.Ordinal)
                            ? candidate.Detail
                            : $"{candidate.Reason}: {candidate.Detail}";

                output.WriteLine(
                    $"  {where,-9} {unit.Kind,-12} {unit.Fingerprint}  {unit.Name}  [{AssuranceStateMachine.Name(candidate.State)}] {status}");
            }
        }

        foreach (ComponentExcludedFile excluded in set.Excluded)
            output.WriteLine($"{excluded.RelativePath}  not covered: {excluded.Reason}");
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=AAA6E4
    // Broiler-Falsified-If: with --dry-run, a file with applied entries is still written to disk
    // Broiler-Human:        PENDING
    private static int Insert(Options options, TextWriter output, TextWriter error)
    {
        string root = RootOf(options);
        string assessmentsPath = options.Value("--assessments")
            ?? throw new UsageException("insert needs --assessments <file.json>");

        string? jsonPath = options.Value("--json");
        PrepareOut(jsonPath);

        AssuranceComponentConfig config = OwnedConfig(root, "insert");

        if (!File.Exists(assessmentsPath))
            throw new UsageException($"--assessments: '{assessmentsPath}' does not exist");

        AssessmentInput input = AssuranceJson.ReadAssessments(File.ReadAllText(assessmentsPath));
        ComponentSourceSet set = ComponentSources.Discover(root, config);
        bool dryRun = options.Has("--dry-run");

        var closed = new HashSet<string>(config.ClosedToEscapeHatch, StringComparer.Ordinal);
        CSharpAssuranceFileScanner scanner = ScannerFor(config);
        var results = new List<AssuranceInsertEntryResult>(input.Refused);

        // Each entry's file is read the way a --files list is: relative or
        // absolute under the root, with either slash, and (on Windows) in any
        // case. An entry keeps the covered file's own spelling from here on.
        var located = new List<(AssuranceAssessment Entry, ComponentSourceFile? File, string? Why)>();
        foreach (AssuranceAssessment entry in input.Entries)
        {
            ComponentSourceFile? file = ComponentSources.Find(set, root, entry.File, out ComponentUnknownPath? unknown);
            located.Add(file is null
                ? (entry, null, unknown!.Reason)
                : (entry with { File = file.RelativePath }, file, null));
        }

        foreach ((AssuranceAssessment entry, _, string? why) in located.Where(static entry => entry.File is null))
            results.Add(new AssuranceInsertEntryResult(entry, false, $"the file {why}"));

        foreach (IGrouping<ComponentSourceFile, (AssuranceAssessment Entry, ComponentSourceFile? File, string? Why)> group in located
            .Where(static entry => entry.File is not null)
            .GroupBy(static entry => entry.File!)
            .OrderBy(static group => group.Key.RelativePath, StringComparer.Ordinal))
        {
            ComponentSourceFile file = group.Key;
            List<AssuranceAssessment> entries = [.. group.Select(static entry => entry.Entry).OrderBy(static entry => entry.Index)];

            if (!AssuranceSourceText.TryRead(file.FullPath, out AssuranceSourceText? source, out string? problem))
            {
                results.AddRange(entries.Select(entry =>
                    new AssuranceInsertEntryResult(entry, false, $"the file {problem}")));
                continue;
            }

            AssuranceInsertFileResult applied = AssuranceInsertion.Apply(
                source!.Text,
                file.RelativePath,
                scanner,
                entries,
                closed.Contains(file.Project.AssemblyName),
                file.Project.AssemblyName);

            if (dryRun)
            {
                results.AddRange(applied.Entries.Select(static entry => entry.Applied
                    ? entry with { Message = entry.Message + " (dry run: not written)" }
                    : entry));
                continue;
            }

            if (applied.Changed && !source.TryWrite(file.FullPath, applied.Text, out string? writeProblem))
            {
                results.AddRange(applied.Entries.Select(entry => entry.Applied
                    ? entry with { Applied = false, Message = $"the file {writeProblem}", Line = null }
                    : entry));
                continue;
            }

            results.AddRange(applied.Entries);
        }

        results.Sort(static (left, right) => left.Entry.Index.CompareTo(right.Entry.Index));

        // With the JSON report on standard output, the text report moves to
        // standard error so that the output stays one parseable document.
        WriteInsertText(results, dryRun, jsonPath == "-" ? error : output);

        int exit = results.All(static result => result.Applied) ? Done : Refused;
        if (jsonPath is not null && !WriteOut(jsonPath, AssuranceJson.InsertReport(results, dryRun), output, error))
        {
            if (!dryRun && results.Any(static result => result.Applied))
                error.WriteLine("broiler-review assurance: the blocks reported above as inserted were written to the sources.");

            return Refused;
        }

        return exit;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7211F1
    // Broiler-Falsified-If: an entry whose file could not be written is counted as applied in the summary line
    // Broiler-Human:        PENDING
    private static void WriteInsertText(IReadOnlyList<AssuranceInsertEntryResult> results, bool dryRun, TextWriter output)
    {
        foreach (AssuranceInsertEntryResult result in results)
        {
            string number = result.Entry.Index.ToString(CultureInfo.InvariantCulture);
            if (result.Applied)
            {
                string verb = dryRun ? "would insert" : "inserted";
                output.WriteLine(Invariant(
                    $"#{number} {verb}  {result.Entry.File}:{result.Line}  {result.Entry.Unit}"));
            }
            else
            {
                output.WriteLine(
                    $"#{number} refused  {result.Entry.File}  {result.Entry.Unit}: {result.Message}");
            }
        }

        int applied = results.Count(static result => result.Applied);
        output.WriteLine(
            Invariant($"{applied} {(dryRun ? "would be applied" : "applied")}, {results.Count - applied} refused") +
            (dryRun ? " (dry run: nothing was written)" : string.Empty));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=D4F931
    // Broiler-Falsified-If: an assurance.config.json at the root that fails to parse comes back as null instead of stopping the run with a configuration error
    // Broiler-Human:        PENDING
    private static AssuranceComponentConfig? LoadConfig(string root)
    {
        string path = Path.Combine(root, AssuranceComponentConfig.FileName);
        return File.Exists(path) ? Named(AssuranceComponentConfig.Parse(File.ReadAllText(path)), root) : null;
    }

    /// <summary>
    /// The configuration with its documented default name: a configuration
    /// that names no component is the component its root directory is, so the
    /// generated titles never come out as "#  Code Assurance".
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9595FA
    // Broiler-Falsified-If: a configuration that names no component comes back with an empty component name
    // Broiler-Human:        PENDING
    private static AssuranceComponentConfig Named(AssuranceComponentConfig config, string root) =>
        config.Component.Length > 0 ? config : config with { Component = ComponentName(root, null) };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=51D934
    // Broiler-Falsified-If: a root given with a trailing directory separator yields an empty component name
    // Broiler-Human:        PENDING
    private static string ComponentName(string root, AssuranceComponentConfig? config) =>
        config is { Component.Length: > 0 }
            ? config.Component
            : Path.GetFileName(Path.TrimEndingDirectorySeparator(Path.GetFullPath(root)));

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=9BD9C8
    // Broiler-Falsified-If: a --root that names a file or a missing path is returned instead of raising a usage error
    // Broiler-Human:        PENDING
    private static string RootOf(Options options)
    {
        string root = options.Value("--root") ?? Directory.GetCurrentDirectory();
        if (!Directory.Exists(root))
            throw new UsageException($"--root: '{root}' is not a directory");

        return root;
    }

    /// <summary>
    /// Refuses a <c>--json</c> target that cannot be written, before the
    /// command reads or writes anything else, so that a report path mistake
    /// never leaves an insert applied with no report of it. A file that did
    /// not exist is created to find out, and removed again.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AC1291
    // Broiler-Falsified-If: probing an existing --json target truncates it or deletes it
    // Broiler-Human:        PENDING
    private static void PrepareOut(string? path)
    {
        if (path is null or "-")
            return;

        try
        {
            string full = Path.GetFullPath(path);
            if (Directory.Exists(full))
                throw new UsageException($"--json: '{path}' is a directory");

            if (Path.GetDirectoryName(full) is { Length: > 0 } directory)
                Directory.CreateDirectory(directory);

            bool existed = File.Exists(full);
            using (new FileStream(full, FileMode.OpenOrCreate, FileAccess.Write))
            {
            }

            if (!existed)
                File.Delete(full);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            throw new UsageException($"--json: '{path}' cannot be written: {exception.Message}");
        }
    }

    /// <summary>Writes a report, and says so; false, with the reason on standard error, when it cannot.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E2CFD9
    // Broiler-Falsified-If: a report that could not be written returns true and prints that it was written
    // Broiler-Human:        PENDING
    private static bool WriteOut(string path, string content, TextWriter output, TextWriter error)
    {
        if (path == "-")
        {
            output.Write(content);
            return true;
        }

        try
        {
            string? directory = Path.GetDirectoryName(Path.GetFullPath(path));
            if (directory is { Length: > 0 })
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, content);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            error.WriteLine($"broiler-review assurance: could not write {path}: {exception.Message}");
            return false;
        }

        error.WriteLine($"broiler-review assurance: wrote {path}");
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=D4FBB8
    // Broiler-Falsified-If: a flag the subcommand accepts, such as --release for check, is dropped instead of set, so the run applies fewer rules than it was asked to
    // Broiler-Human:        PENDING
    private static Options Parse(IReadOnlyList<string> args, string[] valued, string[] flags)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var set = new HashSet<string>(StringComparer.Ordinal);

        for (int index = 1; index < args.Count; index++)
        {
            string name = args[index];
            if (valued.Contains(name, StringComparer.Ordinal))
            {
                if (index + 1 >= args.Count)
                    throw new UsageException($"{name} needs a value");

                if (!values.TryAdd(name, args[++index]))
                    throw new UsageException($"{name} is given twice");
            }
            else if (flags.Contains(name, StringComparer.Ordinal))
            {
                set.Add(name);
            }
            else
            {
                throw new UsageException($"'{name}' is not an option of 'assurance {args[0]}'");
            }
        }

        return new Options(values, set);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=434BE1
    // Broiler-Human:        PENDING
    private static string Invariant(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);

    // Broiler-AI:           Origin=AI; IP=Low; Security=None; Resources=0; Fingerprint=C5BB3F
    // Broiler-Human:        PENDING
    private const string Usage =
        """
        Usage:
          broiler-review assurance list   --root <dir> [--files <list>] [--strict] [--json <out>|-] [--all-units]
          broiler-review assurance insert --root <dir> --assessments <file.json> [--dry-run] [--json <out>|-]
          broiler-review assurance generate --root <dir> [--dry-run] [--adopt]
          broiler-review assurance check  --root <dir> [--config <file>] [--release] [--sources-only] [--json <out>|-]
                                          [--annotation-prefix <path>] [--annotation-limit <n>]
          broiler-review assurance status --root <dir> [--config <file>]
          broiler-review assurance prune  --root <dir> [--dry-run] [--json <out>|-]

        list    Relevant units that carry no annotation block, with the file, line, column, indent,
                qualified name, kind, fingerprint and extent a tool needs to write one, and whether
                one can be inserted there now (and if not, why). --all-units lists every unit.
                --files restricts the run to the paths in a file, one per line ('#' starts a
                comment): root-relative or absolute under --root, either slash, any case on Windows.
                A path that is not a covered file is reported (unknownFiles in JSON), and with
                --strict fails the run. A covered file that cannot be read always does.
        insert  Writes machine assessments above those units:
                  { "schema": 1, "assessments": [ { "file", "unit", "fingerprint", "origin",
                    "spec"?, "ip", "security", "resources", "falsifiedIf"? } ] }
                or { "file", "unit", "fingerprint"?, "exempt": "<reason>" } for the EXEMPT form.
                Each block records Fingerprint=TBF and a human line of PENDING. The input has no
                field for the human line. Needs assurance.config.json at the root.
        generate  Rewrites every covered file's generated header and annotation blocks (fills
                Fingerprint=TBF, moves an outrun decision to STALE) and the report, manifest and
                human-review record. Writes only what changed, keeping each file's byte-order
                mark and line endings. Needs assurance.config.json at the root, refuses in
                "mode": "external", and will not replace a report or record it did not write
                unless --adopt is given.
        check   Computes what generate would write and reports every difference, and every
                annotation rule the tree breaks, as ::error lines (J1-J7, J9, J10; J11 with
                --release). --sources-only compares only the covered files and the manifest's
                arrays, for a component whose own tooling owns the prose. --config reads the
                configuration from elsewhere, for a component that has none.
                --annotation-prefix puts a path before every file= (the component's directory,
                when the workflow runs from a parent); --annotation-limit caps the ::error lines
                per rule and counts the rest.
        status  A short summary of units, annotations and states.
        prune   Removes the annotation lines the rubric no longer asks for: the whole block above
                a unit the exemption predicate exempts (a named value, once "namedValues" is
                "watched"), and the Broiler-Falsified-If line of a block assessed Security=None,
                Low or Medium. Only where the human line reads exactly PENDING: a block naming a
                reviewer or reading STALE is left in place and reported. Whole lines only; each
                written file is scanned again and must keep every fingerprint. Run generate
                afterwards. Needs assurance.config.json at the root and refuses in external mode.

        Exit codes: 0 done, 1 something was refused, check found a violation, list could
        not read a covered file (or, with --strict, was named a path that is not one),
        prune left a block in place or could not read, verify or write a file, or a JSON
        report could not be written after the command's own writes; 2 usage or
        configuration error, before anything was written.

        """;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=9CF15D
    // Broiler-Human:        PENDING
    private sealed record Options(IReadOnlyDictionary<string, string> Values, IReadOnlySet<string> Flags)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=1E731F
        // Broiler-Human:        PENDING
        public string? Value(string name) => Values.TryGetValue(name, out string? value) ? value : null;

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3C9931
        // Broiler-Human:        PENDING
        public bool Has(string name) => Flags.Contains(name);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DA32AF
    // Broiler-Human:        PENDING
    private sealed class UsageException : Exception
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=9AC89E
        // Broiler-Human:        PENDING
        public UsageException(string message)
            : base(message)
        {
        }
    }
}
