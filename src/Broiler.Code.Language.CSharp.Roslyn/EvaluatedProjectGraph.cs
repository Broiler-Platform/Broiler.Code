// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           15
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    High
// Criteria:         15/2
// Resource impact:  5/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Linq;

namespace Broiler.Code.Language.CSharp.Roslyn;

/// <summary>
/// The target-specific evaluated graph a semantic service needs before it can
/// say anything true.
///
/// Roslyn alone is not a language service. A compilation with no metadata
/// references reports every framework type as missing, so the question for a
/// constrained host was never "can Roslyn run there" but "can this get there".
/// Phase 0 measured it: 168 references and 5.8 MiB for a two-project solution.
///
/// It is serializable by construction — plain strings and lists — because it
/// crosses a process boundary from a trusted worker, and may cross a network to
/// a remote one.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=03E10E
// Broiler-Falsified-If: HasReferences or Describe misreports the graph, for example HasReferences is true while MetadataReferencePaths is empty
// Broiler-Human:        PENDING
public sealed record EvaluatedProjectGraph
{
    /// <summary>The project this graph describes, workspace-relative.</summary>
    public required string ProjectPath { get; init; }

    /// <summary>
    /// Which target framework was evaluated. A multi-targeting project has one
    /// graph per framework and they genuinely differ — a diagnostic from the
    /// wrong one is a diagnostic about code the user is not building.
    /// </summary>
    public required string TargetFramework { get; init; }

    /// <summary>Absolute paths of the compile inputs, in evaluated order.</summary>
    public IReadOnlyList<string> CompilePaths { get; init; } = [];

    public IReadOnlyList<string> MetadataReferencePaths { get; init; } = [];

    /// <summary>
    /// Preprocessor symbols. These are what make target-specific results
    /// target-specific, so a service that guessed them would report errors in
    /// code the selected framework never compiles.
    /// </summary>
    public IReadOnlyList<string> PreprocessorSymbols { get; init; } = [];

    public string? LanguageVersion { get; init; }

    /// <summary>The project's nullable context, verbatim.</summary>
    public string? NullableContext { get; init; }

    /// <summary>
    /// Compile inputs a generator produced. Recorded separately because they
    /// exist only after a build, and a diagnostic pointing at one has to be
    /// presented as generated rather than as a file the user can edit.
    /// </summary>
    public IReadOnlyList<string> GeneratedCompilePaths { get; init; } = [];

    /// <summary>
    /// The build that produced this graph, when it came from one. Diagnostics
    /// carry it so a stale build cannot replace newer live results.
    /// </summary>
    public string? BuildId { get; init; }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C629E5
    // Broiler-Falsified-If: HasReferences is true for a graph whose MetadataReferencePaths list is empty
    // Broiler-Human:        PENDING
    public bool HasReferences => MetadataReferencePaths.Count > 0;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A1F8C5
    // Broiler-Falsified-If: the description gives a source count other than CompilePaths.Count or a reference count other than MetadataReferencePaths.Count
    // Broiler-Human:        PENDING
    public string Describe() =>
        $"{ProjectPath} [{TargetFramework}]: {CompilePaths.Count} sources, " +
        $"{MetadataReferencePaths.Count} references, {PreprocessorSymbols.Count} defines";
}

/// <summary>
/// Why a project has no evaluated graph. The service reports one of these
/// rather than guessing: a compilation assembled from assumptions produces
/// diagnostics that look authoritative and are not.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=819D09
// Broiler-Human:        PENDING
public enum GraphUnavailableReason
{
    None = 0,

    /// <summary>The workspace has not been trusted, so nothing was evaluated.</summary>
    WorkspaceNotTrusted,

    /// <summary>Evaluation ran and failed.</summary>
    EvaluationFailed,

    /// <summary>The project uses a construct the declared model cannot model.</summary>
    UnsupportedProject,

    /// <summary>No worker is reachable.</summary>
    WorkerUnavailable,
}

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=442659
// Broiler-Falsified-If: the diagnostic built from a reason carries another reason's code or a project path other than the one the record names
// Broiler-Human:        PENDING
public sealed record GraphUnavailable(
    string ProjectPath,
    GraphUnavailableReason Reason,
    string Message)
{
    /// <summary>
    /// The user-facing diagnostic. It names the project and the reason, because
    /// "no IntelliSense" with no explanation is the failure mode this exists to
    /// avoid.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=16EF14
    // Broiler-Falsified-If: a WorkerUnavailable reason is given BRC1000, BRC1001 or BRC1002, the codes reserved for the other reasons
    // Broiler-Human:        PENDING
    public CodeProjectDiagnostic ToDiagnostic() => new(
        Reason switch
        {
            GraphUnavailableReason.WorkspaceNotTrusted => "BRC1000",
            GraphUnavailableReason.EvaluationFailed => "BRC1001",
            GraphUnavailableReason.UnsupportedProject => "BRC1002",
            _ => "BRC1003",
        },
        ProjectPath,
        Message);
}

/// <summary>
/// A diagnostic with no source span — a project or toolchain problem. It stays
/// visible in the Problems pane rather than being dropped for having nowhere to
/// put a squiggle.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3B3594
// Broiler-Human:        PENDING
public sealed record CodeProjectDiagnostic(string Code, string ProjectPath, string Message);

/// <summary>Supplies evaluated graphs, from wherever they are produced.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=523A3B
// Broiler-Falsified-If: an implementation returns true with a graph whose TargetFramework differs from the non-null framework that was requested
// Broiler-Human:        PENDING
public interface IEvaluatedGraphSource
{
    /// <summary>
    /// The graph for a project and target framework, or the reason there is
    /// none. Never returns a partially-guessed graph.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=64F59D
    // Broiler-Falsified-If: an implementation returns false while leaving unavailable null, so the language service fails while building its Unavailable result
    // Broiler-Human:        PENDING
    bool TryGetGraph(
        string projectPath,
        string? targetFramework,
        out EvaluatedProjectGraph? graph,
        out GraphUnavailable? unavailable);

    /// <summary>Frameworks a project declares, for the selection the UI offers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E93BB3
    // Broiler-Falsified-If: an implementation lists a framework for which TryGetGraph with that framework then returns false
    // Broiler-Human:        PENDING
    IReadOnlyList<string> GetTargetFrameworks(string projectPath);
}

/// <summary>
/// The source used before a workspace is trusted, and the default one.
///
/// Opening a workspace does not imply trust, so this evaluates nothing and
/// reports why. An IDE that quietly evaluated on open would run the project's
/// MSBuild logic — arbitrary code — before the user agreed to anything.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=15E1EF
// Broiler-Falsified-If: the source used before trust answers any project with a graph, so its compile and reference paths are read before the user trusted the workspace
// Broiler-Human:        PENDING
public sealed class UntrustedGraphSource : IEvaluatedGraphSource
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E44D11
    // Broiler-Falsified-If: for some project path TryGetGraph returns true or a non-null graph, or reports a reason other than WorkspaceNotTrusted
    // Broiler-Human:        PENDING
    public bool TryGetGraph(
        string projectPath,
        string? targetFramework,
        out EvaluatedProjectGraph? graph,
        out GraphUnavailable? unavailable)
    {
        graph = null;
        unavailable = new GraphUnavailable(
            projectPath,
            GraphUnavailableReason.WorkspaceNotTrusted,
            "This workspace has not been trusted, so its projects have not been evaluated. " +
            "Editing and syntax colouring work; semantic diagnostics need an explicit trust decision.");
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CBA7B8
    // Broiler-Falsified-If: a project in an untrusted workspace is listed with a target framework
    // Broiler-Human:        PENDING
    public IReadOnlyList<string> GetTargetFrameworks(string projectPath) => [];
}

/// <summary>
/// A source backed by graphs a trusted worker already produced. The service
/// holds no evaluation logic of its own — evaluation runs out of process, and
/// this only remembers what came back.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=66240A
// Broiler-Falsified-If: a query naming one target framework is answered with the graph cached for another framework of the same project
// Broiler-Human:        PENDING
public sealed class CachedGraphSource : IEvaluatedGraphSource
{
    private readonly Dictionary<string, List<EvaluatedProjectGraph>> _byProject =
        new(StringComparer.OrdinalIgnoreCase);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0C223B
    // Broiler-Falsified-If: adding a graph for a framework already cached under a different letter case leaves two graphs for that framework
    // Broiler-Human:        PENDING
    public void Add(EvaluatedProjectGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);
        if (!_byProject.TryGetValue(graph.ProjectPath, out List<EvaluatedProjectGraph>? graphs))
            _byProject[graph.ProjectPath] = graphs = [];

        graphs.RemoveAll(existing =>
            string.Equals(existing.TargetFramework, graph.TargetFramework, StringComparison.OrdinalIgnoreCase));
        graphs.Add(graph);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=17827E
    // Broiler-Falsified-If: after Add replaces the graph of the first framework added for a project, a query with no framework returns another framework's graph
    // Broiler-Human:        PENDING
    public bool TryGetGraph(
        string projectPath,
        string? targetFramework,
        out EvaluatedProjectGraph? graph,
        out GraphUnavailable? unavailable)
    {
        graph = null;
        unavailable = null;

        if (!_byProject.TryGetValue(projectPath, out List<EvaluatedProjectGraph>? graphs) || graphs.Count == 0)
        {
            unavailable = new GraphUnavailable(
                projectPath,
                GraphUnavailableReason.EvaluationFailed,
                $"'{projectPath}' has not been evaluated yet.");
            return false;
        }

        // With no framework named, the first declared one is used and the UI
        // shows which. Picking silently is what makes multi-targeting confusing.
        graph = targetFramework is null
            ? graphs[0]
            : graphs.FirstOrDefault(candidate =>
                string.Equals(candidate.TargetFramework, targetFramework, StringComparison.OrdinalIgnoreCase));

        if (graph is null)
        {
            unavailable = new GraphUnavailable(
                projectPath,
                GraphUnavailableReason.EvaluationFailed,
                $"'{projectPath}' has no evaluated graph for '{targetFramework}'.");
            return false;
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=B7F02B
    // Broiler-Falsified-If: a project with no cached graph returns a non-empty list
    // Broiler-Human:        PENDING
    public IReadOnlyList<string> GetTargetFrameworks(string projectPath) =>
        _byProject.TryGetValue(projectPath, out List<EvaluatedProjectGraph>? graphs)
            ? [.. graphs.Select(graph => graph.TargetFramework)]
            : [];
}
