// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           5
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    Critical
// Criteria:         11/7
// Resource impact:  9/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Broiler.Code.Language.CSharp.Roslyn;

/// <summary>
/// Whether the user has agreed to let a workspace's project logic run.
///
/// MSBuild evaluation executes code: SDK targets, imported props, and anything
/// the project author wrote. Opening a folder cannot imply consent to that, so
/// evaluation is gated on an explicit, scoped decision and an untrusted
/// workspace stays in declared, non-evaluating mode.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=7D49B0
// Broiler-Falsified-If: Untrusted is not the zero value, so a default-initialised trust setting lets evaluation run
// Broiler-Human:        PENDING
public enum WorkspaceTrust
{
    Untrusted = 0,
    Trusted,
}

/// <summary>
/// Design-time project evaluation, out of process.
///
/// It runs the SDK's own structured query — <c>-getItem</c> and
/// <c>-getProperty</c> — rather than loading MSBuild into the IDE. Two reasons,
/// both load-bearing: evaluation runs arbitrary project code, which does not
/// belong in the process holding the user's unsaved edits; and the output is
/// JSON, so nothing here parses localized build text.
///
/// No analyzer or source generator runs as part of this. The query stops at
/// evaluation; generators run during a build, inside the worker's trust
/// boundary, and their outputs reach the language service as recorded inputs
/// rather than by being executed here.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=9; Fingerprint=797B08
// Broiler-Falsified-If: an evaluator whose Trust is Untrusted, or any value other than Trusted, starts a dotnet process
// Broiler-Human:        PENDING
public sealed class DesignTimeEvaluator
{
    private readonly string _workspaceRoot;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7B19AE
    // Broiler-Falsified-If: an empty or whitespace workspace root is accepted and the process's current directory becomes the containment root
    // Broiler-Human:        PENDING
    public DesignTimeEvaluator(string workspaceRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);
        _workspaceRoot = Path.GetFullPath(workspaceRoot);
    }

    public WorkspaceTrust Trust { get; set; } = WorkspaceTrust.Untrusted;

    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Evaluates one project for one target framework. Returns the reason
    /// rather than a partial graph when it cannot: a compilation assembled from
    /// assumptions produces diagnostics that look authoritative and are not.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=9; Fingerprint=7B6211
    // Broiler-Falsified-If: an evaluation whose dotnet build never exits keeps running past TimeoutSeconds, because only the caller's cancellation token ends the wait
    // Broiler-Human:        PENDING
    public async ValueTask<(EvaluatedProjectGraph? Graph, GraphUnavailable? Unavailable)> EvaluateAsync(
        string projectRelativePath,
        string? targetFramework,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectRelativePath);

        if (Trust != WorkspaceTrust.Trusted)
        {
            return (null, new GraphUnavailable(
                projectRelativePath,
                GraphUnavailableReason.WorkspaceNotTrusted,
                "This workspace has not been trusted, so its projects have not been evaluated. " +
                "Editing and syntax colouring work; semantic diagnostics need an explicit trust decision."));
        }

        string projectPath = Path.GetFullPath(Path.Combine(_workspaceRoot, projectRelativePath));
        if (!IsInsideWorkspace(projectPath) || !File.Exists(projectPath))
        {
            return (null, new GraphUnavailable(
                projectRelativePath,
                GraphUnavailableReason.UnsupportedProject,
                $"'{projectRelativePath}' is not a project inside the granted workspace roots."));
        }

        var startInfo = new ProcessStartInfo("dotnet")
        {
            // The project's own directory, so the CLI applies the workspace's
            // global.json exactly as a build would. Resolving the SDK from the
            // IDE's own location would evaluate against a version the user did
            // not select.
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        foreach (string argument in new[]
        {
            "build", projectPath, "-t:ResolveReferences", "--nologo",
            "-getItem:Compile", "-getItem:ReferencePath",
            "-getProperty:DefineConstants", "-getProperty:LangVersion",
            "-getProperty:Nullable", "-getProperty:TargetFramework",
        })
        {
            startInfo.ArgumentList.Add(argument);
        }

        if (targetFramework is { Length: > 0 })
            startInfo.ArgumentList.Add($"-p:TargetFramework={targetFramework}");

        startInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        startInfo.Environment["MSBUILDDISABLENODEREUSE"] = "1";

        using var process = new Process { StartInfo = startInfo };
        try
        {
            process.Start();
        }
        catch (Exception exception) when (exception is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return (null, new GraphUnavailable(
                projectRelativePath, GraphUnavailableReason.WorkerUnavailable, exception.Message));
        }

        Task<string> output = process.StandardOutput.ReadToEndAsync(cancellationToken);
        Task<string> error = process.StandardError.ReadToEndAsync(cancellationToken);

        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // A superseded evaluation must not keep an SDK process alive; the
            // whole tree goes, because MSBuild starts worker nodes.
            TryKill(process);
            throw;
        }

        if (process.ExitCode != 0)
        {
            return (null, new GraphUnavailable(
                projectRelativePath,
                GraphUnavailableReason.EvaluationFailed,
                $"Evaluating '{projectRelativePath}' failed with exit code {process.ExitCode}. " +
                $"Display-only output: {Tail(await error.ConfigureAwait(false), 400)}"));
        }

        try
        {
            return (Parse(projectRelativePath, targetFramework, await output.ConfigureAwait(false)), null);
        }
        catch (JsonException exception)
        {
            return (null, new GraphUnavailable(
                projectRelativePath,
                GraphUnavailableReason.EvaluationFailed,
                $"The evaluation output for '{projectRelativePath}' could not be read: {exception.Message}"));
        }
    }

    /// <summary>
    /// Reads MSBuild's structured output. Public so a worker on another machine
    /// can produce the JSON and this side can still parse it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=2A2F09
    // Broiler-Falsified-If: well-formed JSON of another shape, such as a Compile item whose FullPath is a number or an Items.Compile that is not an array, throws InvalidOperationException, which the JsonException handler in EvaluateAsync does not catch
    // Broiler-Human:        PENDING
    public static EvaluatedProjectGraph Parse(
        string projectPath, string? requestedFramework, string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        var compiles = new List<string>();
        var references = new List<string>();
        var generated = new List<string>();
        if (root.TryGetProperty("Items", out JsonElement items))
        {
            foreach (string path in FullPaths(items, "Compile"))
            {
                // A compile input under obj/ came from a generator, not from
                // the user. A diagnostic on one has to be presented as
                // generated rather than as a file they can edit.
                //
                // Both separators are checked rather than the local one: this
                // parses output that may have been produced by a worker on
                // another platform.
                if (IsUnderIntermediateOutput(path))
                    generated.Add(path);
                else
                    compiles.Add(path);
            }

            references.AddRange(FullPaths(items, "ReferencePath"));
        }

        string? defines = null;
        string? languageVersion = null;
        string? nullable = null;
        string? evaluatedFramework = null;
        if (root.TryGetProperty("Properties", out JsonElement properties))
        {
            defines = GetString(properties, "DefineConstants");
            languageVersion = GetString(properties, "LangVersion");
            nullable = GetString(properties, "Nullable");
            evaluatedFramework = GetString(properties, "TargetFramework");
        }

        return new EvaluatedProjectGraph
        {
            ProjectPath = projectPath,
            TargetFramework = evaluatedFramework ?? requestedFramework ?? string.Empty,
            CompilePaths = compiles,
            GeneratedCompilePaths = generated,
            MetadataReferencePaths = references,
            PreprocessorSymbols = defines?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [],
            LanguageVersion = languageVersion,
            NullableContext = nullable,
        };
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8D22F7
    // Broiler-Falsified-If: a user source whose absolute path has any ancestor directory named obj, such as a workspace under /home/u/obj/repo, is classified as generated
    // Broiler-Human:        PENDING
    private static bool IsUnderIntermediateOutput(string path)
    {
        string normalized = path.Replace('\\', '/');
        return normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith("obj/", StringComparison.OrdinalIgnoreCase);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A7C384
    // Broiler-Falsified-If: on a case-sensitive file system a project in a sibling directory whose name differs from the workspace root's only in letter case passes the containment check
    // Broiler-Human:        PENDING
    private bool IsInsideWorkspace(string candidate)
    {
        string prefix = _workspaceRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return candidate.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=EDB0B7
    // Broiler-Falsified-If: an entry with no FullPath, or an empty one, is yielded as a path
    // Broiler-Human:        PENDING
    private static IEnumerable<string> FullPaths(JsonElement items, string itemName)
    {
        if (!items.TryGetProperty(itemName, out JsonElement element))
            yield break;

        foreach (JsonElement entry in element.EnumerateArray())
        {
            if (entry.TryGetProperty("FullPath", out JsonElement path) &&
                path.GetString() is { Length: > 0 } value)
            {
                yield return value;
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=05E96E
    // Broiler-Falsified-If: an absent property is returned as an empty string rather than null, so TargetFramework no longer falls back to the requested framework
    // Broiler-Human:        PENDING
    private static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out JsonElement value) ? value.GetString() : null;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=19C51F
    // Broiler-Falsified-If: a text longer than the limit keeps its first characters instead of its last
    // Broiler-Human:        PENDING
    private static string Tail(string text, int limit) =>
        text.Length <= limit ? text : text[^limit..];

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=C7D5EB
    // Broiler-Falsified-If: a process tree that cannot be fully terminated makes Kill throw AggregateException or Win32Exception, which escapes TryKill and replaces the cancellation EvaluateAsync rethrows
    // Broiler-Human:        PENDING
    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException)
        {
        }
    }
}
