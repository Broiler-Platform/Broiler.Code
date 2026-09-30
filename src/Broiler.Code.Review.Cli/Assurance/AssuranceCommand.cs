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
/// what <c>generate</c> would write and applies the rules, and <c>status</c>
/// summarizes.
///
/// Exit codes: 0 done (or, for <c>check</c>, nothing wrong), 1 something was
/// refused or violated, 2 a usage or configuration error, before anything was
/// written.
/// </summary>
internal static partial class AssuranceCommand
{
    public const int Done = 0;

    public const int Refused = 1;

    public const int UsageError = 2;

    private static readonly string[] ListOptions = ["--root", "--files", "--json"];

    private static readonly string[] ListFlags = ["--all-units"];

    private static readonly string[] InsertOptions = ["--root", "--assessments", "--json"];

    private static readonly string[] InsertFlags = ["--dry-run"];

    private static readonly string[] GenerateOptions = ["--root"];

    private static readonly string[] GenerateFlags = ["--dry-run", "--adopt"];

    private static readonly string[] CheckOptions = ["--root", "--config", "--json"];

    private static readonly string[] CheckFlags = ["--release", "--sources-only"];

    private static readonly string[] StatusOptions = ["--root", "--config"];

    public static int Run(IReadOnlyList<string> args, TextWriter output, TextWriter error)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(error);

        if (args.Count == 0 || args[0] is "-h" or "--help" or "help")
        {
            output.Write(Usage);
            return args.Count == 0 ? UsageError : Done;
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

    private static int List(Options options, TextWriter output, TextWriter error)
    {
        string root = RootOf(options);
        AssuranceComponentConfig? config = LoadConfig(root);
        ComponentSourceSet set = ComponentSources.Discover(root, config);
        var notes = new List<string>(set.Notes);

        if (options.Value("--files") is { } listFile)
        {
            if (!File.Exists(listFile))
                throw new UsageException($"--files: '{listFile}' does not exist");

            set = ComponentSources.Restrict(set, File.ReadAllLines(listFile), out IReadOnlyList<string> unknown);
            foreach (string path in unknown)
                notes.Add($"--files names '{path}', which is not a covered file");
        }

        var scanner = new CSharpAssuranceFileScanner(config?.PreprocessorSymbols);
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

        if (options.Value("--json") is { } jsonPath)
        {
            WriteOut(jsonPath, AssuranceJson.List(component, set, files, allUnits, notes), output, error);
            return Done;
        }

        WriteListText(component, set, files, allUnits, output);
        return Done;
    }

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
                string status = candidate.Insertable
                    ? "insertable"
                    : candidate.Detail is null ? candidate.Reason : $"{candidate.Reason}: {candidate.Detail}";

                output.WriteLine(
                    $"  {where,-9} {unit.Kind,-12} {unit.Fingerprint}  {unit.Name}  [{AssuranceStateMachine.Name(candidate.State)}] {status}");
            }
        }

        foreach (ComponentExcludedFile excluded in set.Excluded)
            output.WriteLine($"{excluded.RelativePath}  not covered: {excluded.Reason}");
    }

    private static int Insert(Options options, TextWriter output, TextWriter error)
    {
        string root = RootOf(options);
        string assessmentsPath = options.Value("--assessments")
            ?? throw new UsageException("insert needs --assessments <file.json>");

        AssuranceComponentConfig config = OwnedConfig(root, "insert");

        if (!File.Exists(assessmentsPath))
            throw new UsageException($"--assessments: '{assessmentsPath}' does not exist");

        AssessmentInput input = AssuranceJson.ReadAssessments(File.ReadAllText(assessmentsPath));
        ComponentSourceSet set = ComponentSources.Discover(root, config);
        bool dryRun = options.Has("--dry-run");

        var covered = set.Files.ToDictionary(static file => file.RelativePath, StringComparer.Ordinal);
        var excluded = set.Excluded.ToDictionary(static file => file.RelativePath, StringComparer.Ordinal);
        var closed = new HashSet<string>(config.ClosedToEscapeHatch, StringComparer.Ordinal);
        var scanner = new CSharpAssuranceFileScanner(config.PreprocessorSymbols);

        var results = new List<AssuranceInsertEntryResult>(input.Refused);
        foreach (IGrouping<string, AssuranceAssessment> group in input.Entries
            .GroupBy(static entry => entry.File, StringComparer.Ordinal)
            .OrderBy(static group => group.Key, StringComparer.Ordinal))
        {
            List<AssuranceAssessment> entries = [.. group.OrderBy(static entry => entry.Index)];

            if (!covered.TryGetValue(group.Key, out ComponentSourceFile? file))
            {
                string why = excluded.TryGetValue(group.Key, out ComponentExcludedFile? exclusion)
                    ? $"the file is excluded from coverage ({exclusion.Reason})"
                    : "not a covered file; 'assurance list' prints the covered files and their paths";
                results.AddRange(entries.Select(entry => new AssuranceInsertEntryResult(entry, false, why)));
                continue;
            }

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
        string? jsonPath = options.Value("--json");
        WriteInsertText(results, dryRun, jsonPath == "-" ? error : output);

        if (jsonPath is not null)
            WriteOut(jsonPath, AssuranceJson.InsertReport(results, dryRun), output, error);

        return results.All(static result => result.Applied) ? Done : Refused;
    }

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

    private static AssuranceComponentConfig? LoadConfig(string root)
    {
        string path = Path.Combine(root, AssuranceComponentConfig.FileName);
        return File.Exists(path) ? AssuranceComponentConfig.Parse(File.ReadAllText(path)) : null;
    }

    private static string ComponentName(string root, AssuranceComponentConfig? config) =>
        config is { Component.Length: > 0 }
            ? config.Component
            : Path.GetFileName(Path.TrimEndingDirectorySeparator(Path.GetFullPath(root)));

    private static string RootOf(Options options)
    {
        string root = options.Value("--root") ?? Directory.GetCurrentDirectory();
        if (!Directory.Exists(root))
            throw new UsageException($"--root: '{root}' is not a directory");

        return root;
    }

    private static void WriteOut(string path, string content, TextWriter output, TextWriter error)
    {
        if (path == "-")
        {
            output.Write(content);
            return;
        }

        string? directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (directory is { Length: > 0 })
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, content);
        error.WriteLine($"broiler-review assurance: wrote {path}");
    }

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

    private static string Invariant(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);

    private const string Usage =
        """
        Usage:
          broiler-review assurance list   --root <dir> [--files <list>] [--json <out>|-] [--all-units]
          broiler-review assurance insert --root <dir> --assessments <file.json> [--dry-run] [--json <out>|-]
          broiler-review assurance generate --root <dir> [--dry-run] [--adopt]
          broiler-review assurance check  --root <dir> [--config <file>] [--release] [--sources-only] [--json <out>|-]
          broiler-review assurance status --root <dir> [--config <file>]

        list    Relevant units that carry no annotation block, with the file, line, column, indent,
                qualified name, kind, fingerprint and extent a tool needs to write one, and whether
                one can be inserted there now (and if not, why). --all-units lists every unit.
                --files restricts the run to the root-relative paths in a file, one per line.
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
        status  A short summary of units, annotations and states.

        Exit codes: 0 done, 1 something was refused or check found a violation,
        2 usage or configuration error.

        """;

    private sealed record Options(IReadOnlyDictionary<string, string> Values, IReadOnlySet<string> Flags)
    {
        public string? Value(string name) => Values.TryGetValue(name, out string? value) ? value : null;

        public bool Has(string name) => Flags.Contains(name);
    }

    private sealed class UsageException : Exception
    {
        public UsageException(string message)
            : base(message)
        {
        }
    }
}
