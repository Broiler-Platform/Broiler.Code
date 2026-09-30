// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           3
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    High
// Criteria:         9/3
// Resource impact:  7/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Storage;

namespace Broiler.Code.Core.Templates;

// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=C99AEE
// Broiler-Human:        PENDING
public enum ProjectTemplateKind
{
    ConsoleApplication,
    ClassLibrary,
}

// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=A50916
// Broiler-Human:        PENDING
public sealed record TemplateFile(string RelativePath, string Content);

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=7B4872
// Broiler-Human:        PENDING
public sealed record TemplateResult(
    bool Succeeded,
    IReadOnlyList<TemplateFile> Files,
    string? Message = null)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=2858F7
    // Broiler-Falsified-If: a result made by Fail reports Succeeded true or carries files that WriteAsync would write
    // Broiler-Human:        PENDING
    public static TemplateResult Fail(string message) => new(false, [], message);
}

/// <summary>
/// Creates solutions, projects, and source files.
///
/// Everything it emits is a standard SDK file that the .NET CLI would accept and
/// a person would recognise. It writes no Broiler-specific metadata, no marker
/// file, and no property that only this IDE understands: a workspace created
/// here has to remain a workspace anyone can open with any tool, including
/// after Broiler Code is uninstalled.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=3AC707
// Broiler-Falsified-If: WriteAsync replaces a file that already existed at a planned path when the call began instead of refusing the plan
// Broiler-Human:        PENDING
public sealed class CodeTemplateService
{
    private readonly IWorkspaceStorage _storage;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=398525
    // Broiler-Human:        PENDING
    public CodeTemplateService(IWorkspaceStorage storage) =>
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));

    /// <summary>
    /// The files a new solution consists of, without writing them, so a caller
    /// can preview or test the output.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AD8446
    // Broiler-Falsified-If: a project path containing a double quote or an angle bracket reaches the .slnx text without being turned into an entity
    // Broiler-Human:        PENDING
    public static TemplateResult PlanSolution(string solutionName, IReadOnlyList<string> projectPaths)
    {
        if (!IsValidIdentifier(solutionName))
            return TemplateResult.Fail($"'{solutionName}' is not a usable solution name.");

        var builder = new System.Text.StringBuilder();
        builder.Append("<Solution>\n");
        builder.Append("  <Configurations>\n");
        builder.Append("    <BuildType Name=\"Debug\" />\n");
        builder.Append("    <BuildType Name=\"Release\" />\n");
        builder.Append("  </Configurations>\n");
        if (projectPaths.Count > 0)
        {
            builder.Append("  <Folder Name=\"/src/\">\n");
            foreach (string path in projectPaths)
                builder.Append("    <Project Path=\"").Append(Escape(path)).Append("\" />\n");
            builder.Append("  </Folder>\n");
        }

        builder.Append("</Solution>\n");

        return new TemplateResult(true, [new TemplateFile($"{solutionName}.slnx", builder.ToString())]);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=687E2D
    // Broiler-Falsified-If: a project reference or target framework containing a double quote or an angle bracket adds markup to the generated project file
    // Broiler-Human:        PENDING
    public static TemplateResult PlanProject(
        string projectName,
        ProjectTemplateKind kind,
        string targetFramework = "net10.0",
        IReadOnlyList<string>? projectReferences = null)
    {
        if (!IsValidIdentifier(projectName))
            return TemplateResult.Fail($"'{projectName}' is not a usable project name.");

        var files = new List<TemplateFile>();
        string directory = $"src/{projectName}";

        var project = new System.Text.StringBuilder();
        project.Append("<Project Sdk=\"Microsoft.NET.Sdk\">\n\n");
        project.Append("  <PropertyGroup>\n");
        if (kind == ProjectTemplateKind.ConsoleApplication)
            project.Append("    <OutputType>Exe</OutputType>\n");
        project.Append("    <TargetFramework>").Append(Escape(targetFramework)).Append("</TargetFramework>\n");
        project.Append("    <ImplicitUsings>enable</ImplicitUsings>\n");
        project.Append("    <Nullable>enable</Nullable>\n");
        project.Append("  </PropertyGroup>\n");

        if (projectReferences is { Count: > 0 })
        {
            project.Append("\n  <ItemGroup>\n");
            foreach (string reference in projectReferences)
            {
                project.Append("    <ProjectReference Include=\"")
                    .Append(Escape(reference.Replace('/', '\\')))
                    .Append("\" />\n");
            }

            project.Append("  </ItemGroup>\n");
        }

        project.Append("\n</Project>\n");
        files.Add(new TemplateFile($"{directory}/{projectName}.csproj", project.ToString()));

        files.Add(kind == ProjectTemplateKind.ConsoleApplication
            ? new TemplateFile(
                $"{directory}/Program.cs",
                "Console.WriteLine(\"Hello from " + projectName + "\");\n")
            : new TemplateFile(
                $"{directory}/Class1.cs",
                $"namespace {projectName};\n\npublic class Class1\n{{\n}}\n"));

        return new TemplateResult(true, files);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7243AF
    // Broiler-Falsified-If: a namespace name containing a semicolon or a brace is written into the generated source verbatim, so the file declares code beyond the requested type
    // Broiler-Human:        PENDING
    public static TemplateResult PlanSourceFile(string relativePath, string? namespaceName, string typeName)
    {
        if (!IsValidIdentifier(typeName))
            return TemplateResult.Fail($"'{typeName}' is not a usable type name.");

        string body = namespaceName is { Length: > 0 }
            ? $"namespace {namespaceName};\n\npublic class {typeName}\n{{\n}}\n"
            : $"public class {typeName}\n{{\n}}\n";
        return new TemplateResult(true, [new TemplateFile(relativePath, body)]);
    }

    /// <summary>
    /// Writes a plan. Refuses to overwrite: a template that clobbers an
    /// existing file turns "new project" into data loss, and the caller has
    /// more context than this does about what the user meant.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=B2F7A2
    // Broiler-Falsified-If: a plan whose later file is refused by storage returns a failure after its earlier files were already written
    // Broiler-Human:        PENDING
    public async ValueTask<TemplateResult> WriteAsync(
        TemplateResult plan, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (!plan.Succeeded)
            return plan;

        foreach (TemplateFile file in plan.Files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StorageResult<StorageEntry> existing = await _storage
                .StatAsync(file.RelativePath, cancellationToken).ConfigureAwait(false);
            if (existing.Succeeded)
                return TemplateResult.Fail($"'{file.RelativePath}' already exists.");
        }

        foreach (TemplateFile file in plan.Files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StorageResult<string> write = await _storage.WriteTextAsync(
                file.RelativePath,
                file.Content,
                TextEncodingInfo.Utf8NoBom,
                expectedRevision: null,
                cancellationToken).ConfigureAwait(false);
            if (!write.Succeeded)
                return TemplateResult.Fail($"'{file.RelativePath}': {write.Failure!.Message}");
        }

        return plan;
    }

    /// <summary>
    /// Adds a project reference to an existing project, going through the
    /// lossless provider so the rest of the file is untouched.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=BA3BFC
    // Broiler-Falsified-If: a project file that is not well-formed XML makes the call throw instead of returning a failed TemplateResult
    // Broiler-Human:        PENDING
    public async ValueTask<TemplateResult> AddProjectReferenceAsync(
        string projectRelativePath, string referenceInclude, CancellationToken cancellationToken = default)
    {
        StorageResult<StorageTextContent> read = await _storage
            .ReadTextAsync(projectRelativePath, cancellationToken).ConfigureAwait(false);
        if (!read.Succeeded)
            return TemplateResult.Fail(read.Failure!.Message);

        Workspaces.Projects.DeclaredProjectFile project =
            Workspaces.Projects.DeclaredProjectFile.Parse(projectRelativePath, read.Value!.Text);
        Workspaces.Projects.ProjectEditResult edit = project.AddProjectReference(referenceInclude);
        if (!edit.Accepted)
            return TemplateResult.Fail($"{edit.Code}: {edit.Message}");

        StorageResult<string> write = await _storage.WriteTextAsync(
            projectRelativePath,
            edit.NewText!,
            read.Value.Encoding,
            read.Value.ExternalRevision,
            cancellationToken).ConfigureAwait(false);

        return write.Succeeded
            ? new TemplateResult(true, [new TemplateFile(projectRelativePath, edit.NewText!)])
            : TemplateResult.Fail(write.Failure!.Message);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DA3C0F
    // Broiler-Falsified-If: a name ending in a dot or with a digit after a dot is accepted, although storage refuses the directory or C# refuses the namespace it becomes
    // Broiler-Human:        PENDING
    private static bool IsValidIdentifier(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Conservative on purpose: the name becomes a directory, a file name,
        // an assembly name, and a namespace, and the intersection of what all
        // four accept is narrower than any one of them.
        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '.')
                return false;
        }

        return char.IsLetter(name[0]) || name[0] == '_';
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6F0057
    // Broiler-Falsified-If: a double quote, an ampersand or an angle bracket survives unescaped in the returned attribute text
    // Broiler-Human:        PENDING
    private static string Escape(string value) => value
        .Replace("&", "&amp;", StringComparison.Ordinal)
        .Replace("<", "&lt;", StringComparison.Ordinal)
        .Replace(">", "&gt;", StringComparison.Ordinal)
        .Replace("\"", "&quot;", StringComparison.Ordinal);
}
