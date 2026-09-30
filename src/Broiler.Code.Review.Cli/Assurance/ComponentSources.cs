// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   31
// Annotated:        31/31
// Exempt:           0
// Human-reviewed:   0/31
// IP risk:          Low
// Security risk:    High
// Criteria:         21/21
// Resource impact:  8/10 max
// Unverified:       31
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Review.Cli.Assurance;

/// <summary>One product project whose sources are covered.</summary>
/// <param name="RelativePath">The .csproj, relative to the component root, forward slashes.</param>
/// <param name="Directory">The project's directory, relative to the root; empty for the root itself.</param>
/// <param name="AssemblyName">The assembly the project builds.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A1D735
// Broiler-Human:        PENDING
internal sealed record ComponentProject(string RelativePath, string Directory, string AssemblyName);

/// <summary>One covered source file.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=90C302
// Broiler-Human:        PENDING
internal sealed record ComponentSourceFile(string RelativePath, string FullPath, ComponentProject Project);

/// <summary>
/// A file under a covered project, or compiled into one, that is not covered,
/// and why. A path ending in <c>/</c> is a directory the tool does not enter.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=94945A
// Broiler-Human:        PENDING
internal sealed record ComponentExcludedFile(string RelativePath, string Reason);

/// <summary>A path a <c>--files</c> list or an insert names that is not a covered file, and why.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=E0644A
// Broiler-Human:        PENDING
internal sealed record ComponentUnknownPath(string Path, string Reason);

/// <summary>What discovery found.</summary>
/// <param name="Projects">The covered projects.</param>
/// <param name="Files">The covered files, ordered by path (ordinal).</param>
/// <param name="Excluded">
/// Everything under a covered project, or compiled into one, that is not
/// covered: files an exclusion removed, and files and directories discovery
/// did not enter. Ordered by path.
/// </param>
/// <param name="Notes">Things a person running the tool should know, such as a heuristic project list.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=25DB23
// Broiler-Human:        PENDING
internal sealed record ComponentSourceSet(
    IReadOnlyList<ComponentProject> Projects,
    IReadOnlyList<ComponentSourceFile> Files,
    IReadOnlyList<ComponentExcludedFile> Excluded,
    IReadOnlyList<string> Notes);

/// <summary>Why discovery could not produce a source set.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=70DEE0
// Broiler-Human:        PENDING
internal sealed class ComponentSourceException : Exception
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E52F4F
    // Broiler-Human:        PENDING
    public ComponentSourceException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Finds the covered source files of one component.
///
/// The set is every <c>*.cs</c> under each product project's directory, as the
/// owning component defines it: a directory walk, not an MSBuild evaluation.
/// To that walk this adds the files a project compiles in from elsewhere with
/// a <c>&lt;Compile Include&gt;</c> it states literally (in the project file, a
/// <c>Directory.Build.props</c> or <c>.targets</c> above it, or a file either
/// imports), which the owning component has none of and the walk cannot see.
///
/// Some things are not entered, and every one of them is listed as not
/// covered with its reason, so that the record never claims a file it left
/// out:
/// <list type="bullet">
/// <item><c>bin</c> and <c>obj</c> below a project's root, unless the
/// configuration asks for the owning component's rule. Some components track
/// stale <c>obj/**/*.AssemblyAttributes.cs</c> files. A project's own
/// <c>bin</c> and <c>obj</c> are its build output and are left out without a
/// word, as the SDK and the owning component leave them out.</item>
/// <item>Any directory holding a <c>.git</c> entry. That is a nested checkout
/// of another component, or an untracked stale copy of one; its files belong to
/// that component.</item>
/// <item>A directory or file that is a junction or symbolic link. Following
/// one would read and write through it, and it can lead out of the component.</item>
/// </list>
/// A directory that is itself another covered project's directory is walked
/// once, for that project, so no file is covered twice.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=71E8E4
// Broiler-Falsified-If: a .cs file reached through a junction, or inside a directory holding a .git entry, is returned among the covered files
// Broiler-Human:        PENDING
internal static class ComponentSources
{
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=1F5999
    // Broiler-Falsified-If: a .cs file under an obj directory directly below a project root is returned as covered
    // Broiler-Human:        PENDING
    private static readonly string[] BuildOutput = ["bin", "obj"];

    /// <summary>
    /// Discovers the covered files under <paramref name="root"/>. With a
    /// configuration, its project list is used as it is. Without one, product
    /// projects are guessed, and <see cref="ComponentSourceSet.Notes"/> says so.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=1AB32E
    // Broiler-Falsified-If: a configured project that lies inside a nested checkout is walked and its files covered instead of refused
    // Broiler-Human:        PENDING
    public static ComponentSourceSet Discover(string root, AssuranceComponentConfig? config)
    {
        ArgumentNullException.ThrowIfNull(root);

        string fullRoot = Path.GetFullPath(root);
        var notes = new List<string>();

        IReadOnlyList<string> projectPaths;
        if (config is not null)
        {
            projectPaths = config.Projects;
        }
        else
        {
            projectPaths = GuessProjects(fullRoot);
            notes.Add(
                $"no {AssuranceComponentConfig.FileName}: the product projects were guessed " +
                $"({(projectPaths.Count == 0 ? "none found" : string.Join(", ", projectPaths))}). " +
                "Write the file to state them.");
        }

        var projects = new List<(ComponentProject Project, string FullPath)>();
        foreach (string projectPath in projectPaths)
        {
            string full = Path.GetFullPath(Path.Combine(fullRoot, projectPath));
            if (!File.Exists(full))
                throw new ComponentSourceException($"the project '{projectPath}' does not exist under {fullRoot}");

            string directory = Relative(fullRoot, Path.GetDirectoryName(full)!);
            if (Unenterable(fullRoot, directory) is { } barrier)
            {
                throw new ComponentSourceException(
                    $"the project '{projectPath}' lies inside '{barrier.Directory}', which {barrier.What}: " +
                    (barrier.IsLink
                        ? "the tool reads and writes nothing through a link"
                        : "that is another component's checkout, not a product project of this one"));
            }

            projects.Add((new ComponentProject(Relative(fullRoot, full), directory, AssemblyNameOf(fullRoot, full)), full));
        }

        if (config is not null)
            CheckClosedAssemblies(config, projects.Select(static project => project.Project));

        var projectDirectories = new HashSet<string>(
            projects.Select(static project => project.Project.Directory), StringComparer.OrdinalIgnoreCase);

        var files = new Dictionary<string, ComponentSourceFile>(StringComparer.Ordinal);
        var excluded = new Dictionary<string, ComponentExcludedFile>(StringComparer.Ordinal);
        bool anyDepth = config?.ExcludeBuildOutputAtAnyDepth ?? true;

        void Exclude(string relative, string reason) => excluded.TryAdd(relative, new ComponentExcludedFile(relative, reason));

        foreach ((ComponentProject project, _) in projects)
        {
            string start = project.Directory.Length == 0
                ? fullRoot
                : Path.Combine(fullRoot, project.Directory.Replace('/', Path.DirectorySeparatorChar));

            foreach (WalkEntry entry in Walk(fullRoot, start, projectDirectories, anyDepth))
            {
                if (entry.Skipped is { } reason)
                {
                    Exclude(entry.RelativePath, reason);
                    continue;
                }

                if (config?.ExclusionFor(entry.RelativePath) is { } exclusion)
                {
                    Exclude(entry.RelativePath, exclusion.Reason);
                    continue;
                }

                files.TryAdd(entry.RelativePath, new ComponentSourceFile(entry.RelativePath, entry.FullPath, project));
            }
        }

        // Compile items a project states outside its own directory, walked
        // after every project so a file some project's walk covers stays that
        // project's.
        foreach ((ComponentProject project, string projectFile) in projects)
        {
            foreach (CompileItem item in CompileItems(fullRoot, projectFile, project))
            {
                if (item.Unresolved)
                {
                    Exclude(item.Include,
                        $"a <Compile Include> in {item.DeclaredIn} that this tool cannot evaluate, so what it compiles " +
                        $"into {project.AssemblyName} is not known here");
                    continue;
                }

                string relative = Relative(fullRoot, item.FullPath);
                string origin = $"compiled into {project.AssemblyName} by <Compile Include=\"{item.Include}\"> in {item.DeclaredIn}";

                if (relative.StartsWith("../", StringComparison.Ordinal) || relative == ".." || Path.IsPathRooted(relative))
                {
                    Exclude(relative, $"{origin}, and outside the component root, where this tool reads and writes nothing");
                    continue;
                }

                if (files.ContainsKey(relative) || excluded.ContainsKey(relative))
                    continue;

                if (Unenterable(fullRoot, Path.GetDirectoryName(relative)!.Replace('\\', '/')) is { } barrier)
                {
                    Exclude(relative, $"{origin}, and inside '{barrier.Directory}', which {barrier.What}");
                    continue;
                }

                if (IsLink(item.FullPath))
                {
                    Exclude(relative, $"{origin}, and a symbolic link, which the tool does not write through");
                    continue;
                }

                if (config?.ExclusionFor(relative) is { } exclusion)
                {
                    Exclude(relative, exclusion.Reason);
                    continue;
                }

                files.Add(relative, new ComponentSourceFile(relative, item.FullPath, project));
            }
        }

        return new ComponentSourceSet(
            [.. projects.Select(static project => project.Project)],
            [.. files.Values.OrderBy(static file => file.RelativePath, StringComparer.Ordinal)],
            [.. excluded.Values.OrderBy(static file => file.RelativePath, StringComparer.Ordinal)],
            notes);
    }

    /// <summary>
    /// Keeps only the files a list names, one path per line. Blank lines and
    /// lines starting with <c>#</c> are ignored, and each path is read the way
    /// <see cref="Normalize"/> reads it. A named path that is not a covered file
    /// is reported in <paramref name="unknown"/>, with why, rather than failing
    /// the run, because a changed-files list from a pull request names tests
    /// and documents too; <c>list --strict</c> fails on one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=8689F7
    // Broiler-Falsified-If: a --files list whose only line names an uncovered path returns a set that still holds covered files
    // Broiler-Human:        PENDING
    public static ComponentSourceSet Restrict(
        ComponentSourceSet set, string root, IEnumerable<string> listed, out IReadOnlyList<ComponentUnknownPath> unknown)
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(listed);

        var wanted = new HashSet<string>(StringComparer.Ordinal);
        var missing = new List<ComponentUnknownPath>();
        var named = new HashSet<string>(StringComparer.Ordinal);

        foreach (string line in listed)
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                continue;

            if (Find(set, root, trimmed, out ComponentUnknownPath? problem) is { } file)
            {
                wanted.Add(file.RelativePath);
                continue;
            }

            if (named.Add(problem!.Path))
                missing.Add(problem);
        }

        unknown = [.. missing.OrderBy(static path => path.Path, StringComparer.Ordinal)];
        return set with
        {
            Files = [.. set.Files.Where(file => wanted.Contains(file.RelativePath))],
            Excluded = [.. set.Excluded.Where(file => named.Contains(file.RelativePath))],
        };
    }

    /// <summary>
    /// The covered file <paramref name="path"/> names, or null with why not.
    ///
    /// The path is normalized first (<see cref="Normalize"/>). On Windows,
    /// whose file system does not tell cases apart, a path that differs from a
    /// covered file only in case names that file; elsewhere it names nothing,
    /// and the reason suggests the file it would have named.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=8A8F8C
    // Broiler-Falsified-If: on Linux a path that differs from a covered file only in letter case returns that file
    // Broiler-Human:        PENDING
    public static ComponentSourceFile? Find(ComponentSourceSet set, string root, string path, out ComponentUnknownPath? problem)
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(path);

        problem = null;
        string relative = Normalize(root, path);

        ComponentSourceFile? exact = set.Files.FirstOrDefault(file => string.Equals(file.RelativePath, relative, StringComparison.Ordinal));
        if (exact is not null)
            return exact;

        ComponentSourceFile? folded = set.Files.FirstOrDefault(file =>
            string.Equals(file.RelativePath, relative, StringComparison.OrdinalIgnoreCase));
        if (folded is not null && OperatingSystem.IsWindows())
            return folded;

        ComponentExcludedFile? exclusion = set.Excluded.FirstOrDefault(file =>
            string.Equals(file.RelativePath, relative, StringComparison.Ordinal));

        string reason = relative.StartsWith("../", StringComparison.Ordinal) || Path.IsPathRooted(relative)
            ? "is outside the component root"
            : exclusion is not null
                ? $"is not covered: {exclusion.Reason}"
                : folded is not null
                    ? $"is not a covered file; the covered file '{folded.RelativePath}' differs from it in case only"
                    : File.Exists(Path.Combine(Path.GetFullPath(root), relative))
                        ? "is not a covered file; 'assurance list' prints the covered files and their paths"
                        : "does not exist under the component root";

        problem = new ComponentUnknownPath(relative, reason);
        return null;
    }

    /// <summary>
    /// A path as a list or an assessment names a file, read as a root-relative
    /// path with forward slashes: an absolute path under the root is made
    /// relative to it, <c>\</c> is read as <c>/</c>, and empty and <c>.</c>
    /// segments are dropped and <c>..</c> resolved. A path outside the root
    /// comes back starting with <c>../</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=AC07D0
    // Broiler-Falsified-If: a path such as src/../../x.cs comes back without a leading ../, so it is taken for a file inside the root
    // Broiler-Human:        PENDING
    public static string Normalize(string root, string path)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(path);

        string candidate = path.Trim();
        if (Path.IsPathRooted(candidate))
            candidate = Path.GetRelativePath(Path.GetFullPath(root), Path.GetFullPath(candidate));

        var segments = new List<string>();
        foreach (string segment in candidate.Replace('\\', '/').Split('/'))
        {
            if (segment.Length == 0 || segment == ".")
                continue;

            if (segment == ".." && segments.Count > 0 && segments[^1] != "..")
                segments.RemoveAt(segments.Count - 1);
            else
                segments.Add(segment);
        }

        return string.Join('/', segments);
    }

    /// <summary>
    /// The assembly a project builds: its unconditional <c>AssemblyName</c>,
    /// or the file name without <c>.csproj</c> when it states none.
    ///
    /// Refused, rather than guessed, when the name cannot be read off the
    /// file: an <c>AssemblyName</c> under a condition, one that refers to an
    /// MSBuild property other than <c>$(MSBuildProjectName)</c>, and one set
    /// in a <c>Directory.Build.props</c> or <c>.targets</c> above the project.
    /// The name decides which assemblies are closed to the escape hatch, and a
    /// silent fallback there closes nothing.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=B1A16D
    // Broiler-Falsified-If: a project whose only AssemblyName sits in a PropertyGroup with a Condition returns that name instead of refusing
    // Broiler-Human:        PENDING
    internal static string AssemblyNameOf(string root, string projectPath)
    {
        string fallback = Path.GetFileNameWithoutExtension(projectPath);
        string project = Relative(root, projectPath);

        foreach (string imported in DirectoryBuildFiles(root, Path.GetDirectoryName(projectPath)!))
        {
            if (Load(imported)?.Descendants().Any(static element => element.Name.LocalName == "AssemblyName") == true)
            {
                throw new ComponentSourceException(
                    $"{Relative(root, imported)} sets AssemblyName for '{project}', and this tool reads the " +
                    "assembly name only from the project file itself");
            }
        }

        if (Load(projectPath) is not { } document)
            return fallback;

        List<XElement> declared = [.. document.Descendants().Where(static element => element.Name.LocalName == "AssemblyName")];
        if (declared.FirstOrDefault(static element => element.AncestorsAndSelf().Any(static node => node.Attribute("Condition") is not null)) is { } conditional)
        {
            throw new ComponentSourceException(
                $"'{project}' sets AssemblyName to '{conditional.Value.Trim()}' under a condition; this tool reads " +
                "only an unconditional AssemblyName and will not guess which one builds");
        }

        string? value = declared.LastOrDefault()?.Value.Trim();
        if (string.IsNullOrEmpty(value) || value == "$(MSBuildProjectName)")
            return fallback;

        if (value.Contains("$(", StringComparison.Ordinal))
        {
            throw new ComponentSourceException(
                $"'{project}' sets AssemblyName to '{value}', which refers to an MSBuild property this tool does not evaluate");
        }

        return value;
    }

    /// <summary>
    /// Refuses a configuration that closes an assembly no configured project
    /// builds. A misspelt name would otherwise close nothing while the report
    /// said it was closed.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=9AE3E7
    // Broiler-Falsified-If: a configuration whose closedToEscapeHatch names an assembly no configured project builds passes without an exception
    // Broiler-Human:        PENDING
    private static void CheckClosedAssemblies(AssuranceComponentConfig config, IEnumerable<ComponentProject> projects)
    {
        var built = new SortedSet<string>(projects.Select(static project => project.AssemblyName), StringComparer.Ordinal);
        foreach (string assembly in config.ClosedToEscapeHatch)
        {
            if (!built.Contains(assembly))
            {
                throw new ComponentSourceException(
                    $"{AssuranceComponentConfig.FileName}: $.closedToEscapeHatch names '{assembly}', which no configured " +
                    $"project builds; they build {string.Join(", ", built)}");
            }
        }
    }

    /// <summary>One file the walk found, or one path it did not enter, with why.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=2ECFF9
    // Broiler-Human:        PENDING
    private readonly record struct WalkEntry(string RelativePath, string FullPath, string? Skipped);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=E07ED9
    // Broiler-Falsified-If: a subdirectory that is a junction is descended into and its .cs files are yielded with no skip reason
    // Broiler-Human:        PENDING
    private static IEnumerable<WalkEntry> Walk(string root, string start, HashSet<string> projectDirectories, bool anyDepth)
    {
        // Each directory with the build output directory it lies in, below the
        // project's root, when it lies in one: its files are listed, not covered.
        var pending = new Stack<(string Directory, string? BuildOutput)>();
        pending.Push((start, null));

        while (pending.Count > 0)
        {
            (string current, string? output) = pending.Pop();

            foreach (string file in Directory.EnumerateFiles(current)
                .Where(static file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                .Order(StringComparer.Ordinal))
            {
                string relative = Relative(root, file);
                if (IsLink(file))
                    yield return new WalkEntry(relative, file, "a symbolic link, which the tool does not write through");
                else if (output is not null)
                    yield return new WalkEntry(relative, file, BuildOutputReason(output));
                else
                    yield return new WalkEntry(relative, file, null);
            }

            foreach (string sub in Directory.EnumerateDirectories(current).Order(StringComparer.Ordinal))
            {
                string name = Path.GetFileName(sub);
                string relative = Relative(root, sub);

                // Git's own directory: no source of anybody's is in it.
                if (string.Equals(name, ".git", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (IsNestedCheckout(sub))
                {
                    yield return new WalkEntry(relative + "/", sub,
                        "a nested checkout: it holds a .git entry, so its files belong to another component");
                    continue;
                }

                if (IsLink(sub))
                {
                    yield return new WalkEntry(relative + "/", sub,
                        "a junction or symbolic link, which the tool does not follow");
                    continue;
                }

                // Another covered project's directory is walked for that project.
                if (projectDirectories.Contains(relative))
                    continue;

                bool buildOutput = BuildOutput.Contains(name, StringComparer.OrdinalIgnoreCase);
                if (buildOutput && string.Equals(current, start, StringComparison.Ordinal))
                    continue;

                pending.Push((sub, output ?? (buildOutput && anyDepth ? relative : null)));
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9469BB
    // Broiler-Human:        PENDING
    private static string BuildOutputReason(string directory) =>
        $"inside the build output directory '{directory}/', which is left out at any depth " +
        "(\"excludeBuildOutputAtAnyDepth\": false covers it)";

    /// <summary>
    /// The first directory below the root, down to and including
    /// <paramref name="relativeDirectory"/>, that discovery does not enter: one
    /// holding a <c>.git</c> entry, or a link. Null when there is none. The
    /// root's own <c>.git</c> is the component's and does not count.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=1C607B
    // Broiler-Falsified-If: on Windows a directory segment spelt .git. (which the file system opens as .git) returns null instead of a .git barrier
    // Broiler-Human:        PENDING
    internal static (string Directory, string What, bool IsLink)? Unenterable(string root, string relativeDirectory)
    {
        if (relativeDirectory.Length == 0)
            return null;

        string current = root;
        string walked = string.Empty;
        foreach (string segment in relativeDirectory.Split('/'))
        {
            current = Path.Combine(current, segment);
            walked = walked.Length == 0 ? segment : walked + "/" + segment;

            if (string.Equals(segment, ".git", StringComparison.OrdinalIgnoreCase))
                return (walked, "is a .git directory", false);

            if (IsLink(current))
                return (walked, "is a junction or symbolic link", true);

            if (IsNestedCheckout(current))
                return (walked, "holds a .git entry", false);
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=057A2F
    // Broiler-Falsified-If: a directory whose .git entry is a file, as in a submodule checkout, is reported as not a nested checkout
    // Broiler-Human:        PENDING
    private static bool IsNestedCheckout(string directory)
    {
        string git = Path.Combine(directory, ".git");
        return Directory.Exists(git) || File.Exists(git);
    }

    /// <summary>True for a junction or symbolic link, file or directory, and for anything that cannot be asked.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=895528
    // Broiler-Falsified-If: a directory junction, or a path whose attributes cannot be read, returns false
    // Broiler-Human:        PENDING
    internal static bool IsLink(string path)
    {
        try
        {
            FileSystemInfo info = Directory.Exists(path) ? new DirectoryInfo(path) : new FileInfo(path);
            return info.Exists && (info.Attributes.HasFlag(FileAttributes.ReparsePoint) || info.LinkTarget is not null);
        }
        catch (IOException)
        {
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return true;
        }
    }

    /// <summary>One <c>&lt;Compile Include&gt;</c> value, resolved to a file or not resolvable.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=4B4CEA
    // Broiler-Human:        PENDING
    private sealed record CompileItem(string Include, string DeclaredIn, string FullPath, bool Unresolved);

    /// <summary>
    /// The <c>*.cs</c> files a project's literal <c>&lt;Compile Include&gt;</c>
    /// items name outside the project's own directory, and every item whose
    /// value this tool cannot evaluate. Wildcards are expanded. A
    /// <c>Condition</c> is ignored and a <c>Remove</c> is not applied: the bias
    /// is to cover what may be compiled, not only what certainly is.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=C2DC5E
    // Broiler-Falsified-If: an include using an MSBuild property other than the two it expands is resolved or dropped instead of yielded as unresolved
    // Broiler-Human:        PENDING
    private static IEnumerable<CompileItem> CompileItems(string root, string projectFile, ComponentProject project)
    {
        string projectDirectory = Path.GetDirectoryName(projectFile)!;

        foreach (string file in ProjectFiles(root, projectFile))
        {
            if (Load(file) is not { } document)
                continue;

            string declaredIn = Relative(root, file);
            string declaringDirectory = Path.GetDirectoryName(file)! + Path.DirectorySeparatorChar;

            foreach (XElement element in document.Descendants().Where(static element => element.Name.LocalName == "Compile"))
            {
                if (element.Attribute("Include")?.Value is not { } includes)
                    continue;

                foreach (string include in includes.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    string expanded = include
                        .Replace("$(MSBuildThisFileDirectory)", declaringDirectory, StringComparison.OrdinalIgnoreCase)
                        .Replace("$(MSBuildProjectDirectory)", projectDirectory, StringComparison.OrdinalIgnoreCase);

                    if (expanded.Contains("$(", StringComparison.Ordinal) || expanded.Contains("@(", StringComparison.Ordinal) ||
                        expanded.Contains("%(", StringComparison.Ordinal))
                    {
                        yield return new CompileItem(include, declaredIn, string.Empty, Unresolved: true);
                        continue;
                    }

                    foreach (string resolved in Resolve(projectDirectory, expanded))
                    {
                        if (!resolved.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || IsUnder(resolved, projectDirectory))
                            continue;

                        yield return new CompileItem(include, declaredIn, resolved, Unresolved: false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// The project file, the <c>Directory.Build.props</c> and <c>.targets</c>
    /// files from its directory up to the root, and every file those import by
    /// a literal path inside the root, each once.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=1476CB
    // Broiler-Falsified-If: an Import whose literal path leads outside the component root is loaded
    // Broiler-Human:        PENDING
    private static IEnumerable<string> ProjectFiles(string root, string projectFile)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pending = new Queue<string>([projectFile, .. DirectoryBuildFiles(root, Path.GetDirectoryName(projectFile)!)]);

        while (pending.Count > 0)
        {
            string file = pending.Dequeue();
            if (!seen.Add(file) || !File.Exists(file))
                continue;

            yield return file;

            if (Load(file) is not { } document)
                continue;

            string directory = Path.GetDirectoryName(file)! + Path.DirectorySeparatorChar;
            foreach (XElement import in document.Descendants().Where(static element => element.Name.LocalName == "Import"))
            {
                string? target = import.Attribute("Project")?.Value
                    .Replace("$(MSBuildThisFileDirectory)", directory, StringComparison.OrdinalIgnoreCase);
                if (target is null || target.Contains("$(", StringComparison.Ordinal) || target.IndexOfAny(['*', '?']) >= 0)
                    continue;

                string full = Path.GetFullPath(Path.Combine(directory, target.Replace('\\', Path.DirectorySeparatorChar)));
                if (IsUnder(full, root))
                    pending.Enqueue(full);
            }
        }
    }

    /// <summary>The <c>Directory.Build.props</c> and <c>.targets</c> files from <paramref name="directory"/> up to the root.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F30897
    // Broiler-Falsified-If: a Directory.Build.props in a directory above the component root is returned
    // Broiler-Human:        PENDING
    private static IEnumerable<string> DirectoryBuildFiles(string root, string directory)
    {
        for (DirectoryInfo? current = new(directory); current is not null; current = current.Parent)
        {
            foreach (string name in new[] { "Directory.Build.props", "Directory.Build.targets" })
            {
                string candidate = Path.Combine(current.FullName, name);
                if (File.Exists(candidate))
                    yield return candidate;
            }

            if (string.Equals(
                    Path.TrimEndingDirectorySeparator(current.FullName),
                    Path.TrimEndingDirectorySeparator(root),
                    StringComparison.OrdinalIgnoreCase))
            {
                yield break;
            }
        }
    }

    /// <summary>The files one include names, relative to the project directory, with its wildcards expanded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=0A7161
    // Broiler-Falsified-If: an include such as ../../../../**/*.cs, whose wildcard base lies outside the component root, has that base directory enumerated
    // Broiler-Human:        PENDING
    private static IEnumerable<string> Resolve(string projectDirectory, string include)
    {
        string[] segments = include.Replace('\\', '/').Split('/');
        int wildcard = Array.FindIndex(segments, static segment => segment.IndexOfAny(['*', '?']) >= 0);

        if (wildcard < 0)
        {
            string file = Path.GetFullPath(Path.Combine(projectDirectory, include.Replace('\\', Path.DirectorySeparatorChar)));
            return File.Exists(file) ? [file] : [];
        }

        string baseDirectory = Path.GetFullPath(Path.Combine(
            projectDirectory, string.Join(Path.DirectorySeparatorChar, segments[..wildcard])));
        string pattern = string.Join('/', segments[wildcard..]);

        if (!Directory.Exists(baseDirectory) || IsUnder(baseDirectory, projectDirectory) ||
            !AssuranceGlob.TryParse(pattern, out AssuranceGlob? glob, out _))
        {
            return [];
        }

        bool recursive = pattern.Contains('/', StringComparison.Ordinal);
        var found = new List<string>();
        var pending = new Stack<string>([baseDirectory]);
        while (pending.Count > 0)
        {
            string current = pending.Pop();
            foreach (string file in Directory.EnumerateFiles(current))
            {
                if (glob!.IsMatch(Path.GetRelativePath(baseDirectory, file).Replace('\\', '/')))
                    found.Add(file);
            }

            if (!recursive)
                continue;

            foreach (string sub in Directory.EnumerateDirectories(current))
            {
                if (!string.Equals(Path.GetFileName(sub), ".git", StringComparison.OrdinalIgnoreCase) && !IsLink(sub))
                    pending.Push(sub);
            }
        }

        return found;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=49023D
    // Broiler-Falsified-If: a path in a sibling directory whose name begins with the directory name, such as root2/x.cs against root, counts as under it
    // Broiler-Human:        PENDING
    private static bool IsUnder(string path, string directory)
    {
        string relative = Path.GetRelativePath(directory, path);
        return relative == "." ||
            (!relative.StartsWith("..", StringComparison.Ordinal) && !Path.IsPathRooted(relative));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=177EB9
    // Broiler-Falsified-If: a project file declaring an external entity has it resolved, so text from a file outside the component appears in an element value
    // Broiler-Human:        PENDING
    private static XDocument? Load(string path)
    {
        try
        {
            return XDocument.Load(path);
        }
        catch (XmlException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    /// <summary>
    /// A guess at the product projects, used only when there is no
    /// configuration: every project that is not a test, a benchmark, a sample or
    /// a diagnostic, outside nested checkouts and build output.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=EDE036
    // Broiler-Falsified-If: a .csproj inside a directory holding a .git entry is returned as a product project
    // Broiler-Human:        PENDING
    private static IReadOnlyList<string> GuessProjects(string root)
    {
        var found = new List<string>();
        var pending = new Stack<string>();
        pending.Push(root);

        while (pending.Count > 0)
        {
            string current = pending.Pop();
            foreach (string project in Directory.EnumerateFiles(current, "*.csproj"))
            {
                string relative = Relative(root, project);
                if (!LooksLikeProduct(relative, project))
                    continue;

                found.Add(relative);
            }

            foreach (string sub in Directory.EnumerateDirectories(current))
            {
                string name = Path.GetFileName(sub);
                if (name.StartsWith('.') || BuildOutput.Contains(name, StringComparer.OrdinalIgnoreCase) ||
                    IsNestedCheckout(sub) || IsLink(sub))
                {
                    continue;
                }

                pending.Push(sub);
            }
        }

        found.Sort(StringComparer.Ordinal);
        return found;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=DDA2BD
    // Broiler-Falsified-If: a project that declares IsTestProject true is reported as a product
    // Broiler-Human:        PENDING
    private static bool LooksLikeProduct(string relative, string fullPath)
    {
        string[] notProduct = ["tests", "test", "samples", "sample", "benchmarks", "benchmark", "diagnostics", "tools"];
        string[] segments = relative.Split('/');
        if (segments[..^1].Any(segment => notProduct.Contains(segment, StringComparer.OrdinalIgnoreCase)))
            return false;

        string name = Path.GetFileNameWithoutExtension(fullPath);
        string[] suffixes = [".Tests", ".Test", ".Benchmarks", ".Benchmark", ".Samples", ".Sample", ".Diagnostic", ".Diagnostics"];
        if (suffixes.Any(suffix => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
            return false;

        try
        {
            return !XDocument.Load(fullPath).Descendants()
                .Any(static element => element.Name.LocalName == "IsTestProject" &&
                    string.Equals(element.Value.Trim(), "true", StringComparison.OrdinalIgnoreCase));
        }
        catch (XmlException)
        {
            return false;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=75666A
    // Broiler-Falsified-If: a path outside the root comes back neither rooted nor starting with ../, so Discover takes it for a file inside the root
    // Broiler-Human:        PENDING
    private static string Relative(string root, string path)
    {
        string relative = Path.GetRelativePath(root, path).Replace('\\', '/');
        return relative == "." ? string.Empty : relative;
    }
}
