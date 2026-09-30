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
// Criteria:         13/9
// Resource impact:  8/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Review.Cli.Assurance;

/// <summary>
/// <c>generate</c>, <c>check</c> and <c>status</c>: the generator, the gate
/// that compares the tree with it, and a summary for a person.
///
/// All three compute the same plan. <c>generate</c> writes it; <c>check</c>
/// compares it with the disk and applies the rules; <c>status</c> counts. So
/// what the check accepts is what the generator writes, and there is no second
/// implementation of either to drift.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=CCE5C4
// Broiler-Falsified-If: generate writes a source file or artefact in a run that also reported a refusal
// Broiler-Human:        PENDING
internal static partial class AssuranceCommand
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=929E74
    // Broiler-Falsified-If: a run with a refusal, or with an existing report the generator did not write and no --adopt, still writes a file
    // Broiler-Human:        PENDING
    private static int Generate(Options options, TextWriter output, TextWriter error)
    {
        string root = RootOf(options);
        AssuranceComponentConfig config = OwnedConfig(root, "generate");
        RequireSpdx(config);

        ComponentCorpus loaded = ComponentCorpus.Load(root, config);
        AssurancePlan plan = AssuranceGenerator.Plan(loaded.Corpus, ScannerFor(config), config);
        string component = ComponentName(root, config);

        // Any refusal stops the whole run, and every refusal is reported in
        // the same run, so fixing one does not uncover the next. A refused
        // file is carried through unchanged, so writing the rest would leave a
        // report and a manifest that describe a tree the files do not match.
        List<AssuranceViolation> refusals = [.. loaded.Problems, .. plan.Problems];
        List<AssuranceArtefact> changes = [.. plan.Changes];

        List<AssuranceArtefact> handWritten = options.Has("--adopt")
            ? []
            : [.. changes.Where(static artefact =>
                artefact.Kind != AssuranceArtefactKind.Source &&
                artefact.Exists &&
                artefact.Current.Length > 0 &&
                !AssuranceGenerator.IsGenerated(artefact.Current))];

        if (refusals.Count > 0)
        {
            error.WriteLine($"broiler-review assurance: the generator refused {Plural(refusals.Count, "time")}:");
            foreach (AssuranceViolation refusal in refusals)
                error.WriteLine($"  {refusal.Rule} {Indent(refusal.Message)}");
        }

        foreach (AssuranceArtefact artefact in handWritten)
        {
            error.WriteLine(
                $"broiler-review assurance: {artefact.RelativePath} exists and was not written by the generator " +
                $"(it carries no '{AssuranceGenerator.GeneratedNotice}' line). Point " +
                $"\"artefacts.{AssuranceGenerator.ConfigKey(artefact.Kind)}\" in {AssuranceComponentConfig.FileName} " +
                "at another file to keep it, or pass --adopt to replace it.");
        }

        if (refusals.Count > 0 || handWritten.Count > 0)
        {
            error.WriteLine("broiler-review assurance: nothing was written.");
            return Refused;
        }

        bool dryRun = options.Has("--dry-run");
        int sources = changes.Count(static artefact => artefact.Kind == AssuranceArtefactKind.Source);
        output.WriteLine(Invariant(
            $"{component}: {plan.Files.Count} covered files; {changes.Count} artefacts ") +
            (dryRun ? "would change" : "to write") +
            Invariant($" ({sources} source files, {changes.Count - sources} component artefacts)"));

        if (dryRun)
        {
            foreach (AssuranceArtefact artefact in changes)
                output.WriteLine($"  would write {artefact.RelativePath}{(artefact.Exists ? string.Empty : " (new)")}");

            output.WriteLine("Dry run: nothing was written.");
            return Done;
        }

        // Sources first: the component artefacts describe them, and a failure
        // part way leaves the record stale, which check then reports, rather
        // than a record describing files that were never written. A write that
        // fails (the file changed on disk meanwhile, or could not be written)
        // does not undo the writes before it; the next run writes the rest.
        var failed = new List<string>();
        foreach (AssuranceArtefact artefact in changes.OrderBy(static artefact => artefact.Kind))
        {
            if (loaded.TryWrite(artefact, out string? problem))
            {
                output.WriteLine($"  wrote {artefact.RelativePath}");
                continue;
            }

            failed.Add($"{artefact.RelativePath} {problem}");
        }

        foreach (string failure in failed)
            error.WriteLine($"broiler-review assurance: {failure}");

        output.WriteLine(Invariant($"{changes.Count - failed.Count} written, {failed.Count} failed."));
        return failed.Count == 0 ? Done : Refused;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=39CE19
    // Broiler-Falsified-If: a run whose only problem is a covered file that could not be read as UTF-8 exits 0
    // Broiler-Human:        PENDING
    private static int Check(Options options, TextWriter output, TextWriter error)
    {
        string root = RootOf(options);
        string? jsonPath = options.Value("--json");
        PrepareOut(jsonPath);

        string? prefix = options.Value("--annotation-prefix");
        int? limit = null;
        if (options.Value("--annotation-limit") is { } text)
        {
            if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
                throw new UsageException($"--annotation-limit: '{text}' is not a whole number");

            limit = value;
        }

        AssuranceComponentConfig config = ReadableConfig(root, options, "check");
        RequireSpdx(config);

        ComponentCorpus loaded = ComponentCorpus.Load(root, config);
        AssurancePlan plan = AssuranceGenerator.Plan(loaded.Corpus, ScannerFor(config), config);

        bool release = options.Has("--release");
        bool sourcesOnly = options.Has("--sources-only");
        var checkOptions = new AssuranceCheckOptions(
            release, sourcesOnly, ComponentCorpus.AdrRecords(root, config.AdrDirectory));

        List<AssuranceViolation> violations = [.. loaded.Problems, .. AssuranceChecks.Run(plan, config, checkOptions)];
        string component = ComponentName(root, config);

        // With the JSON report on standard output, the annotations move to
        // standard error so that the output stays one parseable document.
        TextWriter annotations = jsonPath == "-" ? error : output;
        var shown = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (AssuranceViolation violation in violations)
        {
            int count = shown.GetValueOrDefault(violation.Rule) + 1;
            shown[violation.Rule] = count;
            if (limit is null || count <= limit)
                annotations.WriteLine(WorkflowCommand(violation, prefix));
        }

        // A runner shows a handful of annotations per step, and a component
        // that has just adopted the scheme has thousands; the count says what
        // the lines left out, and the JSON report holds every one.
        foreach ((string rule, int count) in shown.OrderBy(static pair => RuleOrder(pair.Key)))
        {
            if (limit is { } most && count > most)
                annotations.WriteLine(Invariant($"{component}: {count - most} further {rule} violations are not shown as annotations (--annotation-limit {most})."));
        }

        annotations.WriteLine(Summary(component, violations, release, sourcesOnly));

        if (jsonPath is not null &&
            !WriteOut(jsonPath, AssuranceJson.CheckReport(component, release, sourcesOnly, violations), output, error))
        {
            return Refused;
        }

        return violations.Count == 0 ? Done : Refused;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=87451D
    // Broiler-Falsified-If: a unit whose human line still reads PENDING is included in the count printed for units a person has decided
    // Broiler-Human:        PENDING
    private static int Status(Options options, TextWriter output, TextWriter error)
    {
        string root = RootOf(options);
        AssuranceComponentConfig config = ReadableConfig(root, options, "status");
        RequireSpdx(config);

        ComponentCorpus loaded = ComponentCorpus.Load(root, config);
        AssurancePlan plan = AssuranceGenerator.Plan(loaded.Corpus, ScannerFor(config), config);
        IReadOnlyList<AssuranceCorpusUnit> units = plan.UnitsBefore;
        AssuranceSummary summary = AssuranceSummary.Of(units);

        AssuranceAnnotation[] assessed = [.. units
            .Where(static unit => unit.IsRelevant && unit.Annotation is { ExemptReason: null })
            .Select(static unit => unit.Annotation!)];

        int pending = assessed.Count(static annotation => annotation.HumanIsPending);
        int stale = units.Count(static unit => unit.IsRelevant && unit.State == AssuranceUnitState.Stale);

        output.WriteLine(
            Invariant($"{ComponentName(root, config)}: {plan.Files.Count} covered files ({loaded.Set.Excluded.Count} not covered), ") +
            Invariant($"{units.Count} code units, {summary.Relevant} relevant, {summary.Exempt} exempt"));
        output.WriteLine($"  Annotated:       {Portion(summary.Annotated, summary.Relevant)}");
        output.WriteLine($"  Human-reviewed:  {Portion(summary.Verified, summary.Relevant)}");
        output.WriteLine(Invariant($"  Human line:      {pending} PENDING, {stale} STALE"));
        output.WriteLine("  States:          " + string.Join(", ", AssuranceVocabulary.States.Select(state =>
            Invariant($"{AssuranceStateMachine.Name(state)} {units.Count(unit => unit.State == state)}"))));
        output.WriteLine("  Security:        " + string.Join(", ", AssuranceVocabulary.SecurityRiskValues.Select(value =>
            Invariant($"{value} {assessed.Count(annotation => annotation.Field("Security") == value)}"))) +
            Invariant($", not assessed {summary.Relevant - summary.Annotated}"));

        int changes = plan.Changes.Count();
        int refusals = loaded.Problems.Count + plan.Problems.Count;
        output.WriteLine(refusals > 0
            ? Invariant($"  Generate:        refuses ({refusals} problems; 'check' names them)")
            : changes == 0
                ? "  Generate:        nothing to write"
                : Invariant($"  Generate:        would write {changes} artefacts"));

        return Done;
    }

    /// <summary>The configuration at the root, which a write command requires, in owned mode.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=444F24
    // Broiler-Falsified-If: a component with no assurance.config.json at its root, or one whose mode is external, gets a configuration back and a write command proceeds
    // Broiler-Human:        PENDING
    private static AssuranceComponentConfig OwnedConfig(string root, string command)
    {
        // The configuration's presence at the component root is the opt-in to
        // writing. Without it nothing is written, so pointing the tool at a
        // component cannot change that component's sources.
        AssuranceComponentConfig config = LoadConfig(root)
            ?? throw new AssuranceConfigException(
                $"{Path.Combine(root, AssuranceComponentConfig.FileName)} does not exist. A component opts in " +
                $"to written annotations by committing that file; {command} writes nothing without it.");

        if (config.Mode == AssuranceMode.External)
        {
            throw new AssuranceConfigException(
                $"{AssuranceComponentConfig.FileName} says \"mode\": \"external\": another tool writes this " +
                "component's annotations, and this one only reads them.");
        }

        return config;
    }

    /// <summary>
    /// The configuration a read-only command uses: <c>--config</c>, else the
    /// one at the root. A read changes nothing, so the configuration may live
    /// outside a component that has not opted in, such as one whose own tests
    /// generate its record.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=9F9308
    // Broiler-Falsified-If: a --config path that does not exist falls back to the configuration at the root instead of raising a usage error
    // Broiler-Human:        PENDING
    private static AssuranceComponentConfig ReadableConfig(string root, Options options, string command)
    {
        if (options.Value("--config") is { } path)
        {
            if (!File.Exists(path))
                throw new UsageException($"--config: '{path}' does not exist");

            return Named(AssuranceComponentConfig.Parse(File.ReadAllText(path)), root);
        }

        return LoadConfig(root)
            ?? throw new AssuranceConfigException(
                $"{Path.Combine(root, AssuranceComponentConfig.FileName)} does not exist; {command} needs it, " +
                "or --config naming one.");
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=77AB52
    // Broiler-Falsified-If: a configuration without an spdx object reaches generate or check without an AssuranceConfigException
    // Broiler-Human:        PENDING
    private static void RequireSpdx(AssuranceComponentConfig config)
    {
        if (config.Spdx is null)
        {
            throw new AssuranceConfigException(
                $"{AssuranceComponentConfig.FileName}: $.spdx is required to generate or check headers: " +
                "{ \"copyright\": [ \"<year> <holder>\" ], \"license\": \"<SPDX expression>\" }");
        }
    }

    /// <summary>
    /// A GitHub workflow command, so each violation lands on its file in a
    /// pull request's diff. Its message may span lines; they are escaped as
    /// the runner requires. A remedy follows the message as a <c>Run:</c>
    /// line. <paramref name="prefix"/> goes before the file's root-relative
    /// path, for a workflow that runs from a directory above the component.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=0CF246
    // Broiler-Falsified-If: a violation whose message spans two lines is emitted as two output lines, so the second one can start a workflow command of its own
    // Broiler-Human:        PENDING
    internal static string WorkflowCommand(AssuranceViolation violation, string? prefix = null)
    {
        var properties = new List<string>();
        if (violation.File is { } file)
        {
            string located = prefix is { Length: > 0 } ? prefix.Replace('\\', '/').TrimEnd('/') + "/" + file : file;
            properties.Add("file=" + EscapeProperty(located));
        }

        if (violation.Line is { } line)
            properties.Add("line=" + line.ToString(CultureInfo.InvariantCulture));

        string remedy = violation.Remedy is null ? string.Empty : $"\n  Run: {violation.Remedy}";
        string head = properties.Count == 0 ? "::error" : "::error " + string.Join(',', properties);
        return $"{head}::{EscapeData(violation.Rule + " " + violation.Message + remedy)}";
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C0E98F
    // Broiler-Falsified-If: a message holding a carriage return or line feed is emitted with the raw character instead of %0D or %0A
    // Broiler-Human:        PENDING
    private static string EscapeData(string value) => value
        .Replace("%", "%25", StringComparison.Ordinal)
        .Replace("\r", "%0D", StringComparison.Ordinal)
        .Replace("\n", "%0A", StringComparison.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=DC0025
    // Broiler-Falsified-If: a file path containing a comma or a colon is emitted unescaped, so it ends the file property and starts another
    // Broiler-Human:        PENDING
    private static string EscapeProperty(string value) => EscapeData(value)
        .Replace(":", "%3A", StringComparison.Ordinal)
        .Replace(",", "%2C", StringComparison.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=00E3A4
    // Broiler-Falsified-If: a run with at least one violation prints a summary line saying no violations
    // Broiler-Human:        PENDING
    private static string Summary(
        string component, IReadOnlyList<AssuranceViolation> violations, bool release, bool sourcesOnly)
    {
        string mode = (release, sourcesOnly) switch
        {
            (true, true) => " (release, sources only)",
            (true, false) => " (release)",
            (false, true) => " (sources only)",
            _ => string.Empty,
        };

        if (violations.Count == 0)
            return $"{component}: no violations{mode}.";

        var counts = new StringBuilder();
        foreach (IGrouping<string, AssuranceViolation> rule in violations
            .GroupBy(static violation => violation.Rule, StringComparer.Ordinal)
            .OrderBy(static group => RuleOrder(group.Key)))
        {
            if (counts.Length > 0)
                counts.Append(", ");

            counts.Append(Invariant($"{rule.Key} {rule.Count()}"));
        }

        return Invariant($"{component}: {Plural(violations.Count, "violation")}{mode}: {counts}.");
    }

    /// <summary>J1 before J10, and anything that is not a J rule first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DCC118
    // Broiler-Falsified-If: J10 sorts before J2 in the per-rule counts
    // Broiler-Human:        PENDING
    private static int RuleOrder(string rule) =>
        rule.Length > 1 && rule[0] == 'J' &&
        int.TryParse(rule.AsSpan(1), NumberStyles.None, CultureInfo.InvariantCulture, out int number)
            ? number
            : 0;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=34CBEC
    // Broiler-Falsified-If: Portion(1999, 2000) prints 100% for a share that falls short of the whole
    // Broiler-Human:        PENDING
    private static string Portion(int part, int whole) => whole == 0
        ? part.ToString(CultureInfo.InvariantCulture)
        : Invariant($"{part} of {whole} ({(int)Math.Round(100.0 * part / whole)}%)");

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=49799F
    // Broiler-Human:        PENDING
    private static string Plural(int count, string noun) =>
        Invariant($"{count} {noun}{(count == 1 ? string.Empty : "s")}");

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=EDD234
    // Broiler-Human:        PENDING
    private static string Indent(string message) => message.Replace("\n", "\n    ", StringComparison.Ordinal);
}
