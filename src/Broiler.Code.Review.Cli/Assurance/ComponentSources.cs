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
internal sealed record ComponentProject(string RelativePath, string Directory, string AssemblyName);

/// <summary>One covered source file.</summary>
internal sealed record ComponentSourceFile(string RelativePath, string FullPath, ComponentProject Project);

/// <summary>A file under a covered project that the configuration leaves out.</summary>
internal sealed record ComponentExcludedFile(string RelativePath, string Reason);

/// <summary>What discovery found.</summary>
/// <param name="Projects">The covered projects.</param>
/// <param name="Files">The covered files, ordered by path (ordinal).</param>
/// <param name="Excluded">Files under a covered project that an exclusion removed.</param>
/// <param name="Notes">Things a person running the tool should know, such as a heuristic project list.</param>
internal sealed record ComponentSourceSet(
    IReadOnlyList<ComponentProject> Projects,
    IReadOnlyList<ComponentSourceFile> Files,
    IReadOnlyList<ComponentExcludedFile> Excluded,
    IReadOnlyList<string> Notes);

/// <summary>Why discovery could not produce a source set.</summary>
internal sealed class ComponentSourceException : Exception
{
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
/// Three things are left out that the owning component does not need to leave
/// out, because it has none of them:
/// <list type="bullet">
/// <item><c>bin</c> and <c>obj</c> at any depth, not only at the project root.
/// Some components have stale <c>obj/**/*.AssemblyAttributes.cs</c> files
/// tracked in git.</item>
/// <item>Any directory holding a <c>.git</c> entry. That is a nested checkout
/// of another component, or an untracked stale copy of one; its files belong to
/// that component.</item>
/// <item>A directory that is itself another covered project's directory. It is
/// walked once, for that project, so no file is covered twice.</item>
/// </list>
/// </summary>
internal static class ComponentSources
{
    private static readonly string[] BuildOutput = ["bin", "obj"];

    /// <summary>
    /// Discovers the covered files under <paramref name="root"/>. With a
    /// configuration, its project list is used as it is. Without one, product
    /// projects are guessed, and <see cref="ComponentSourceSet.Notes"/> says so.
    /// </summary>
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

        var projects = new List<ComponentProject>();
        foreach (string projectPath in projectPaths)
        {
            string full = Path.GetFullPath(Path.Combine(fullRoot, projectPath));
            if (!File.Exists(full))
                throw new ComponentSourceException($"the project '{projectPath}' does not exist under {fullRoot}");

            string directory = Relative(fullRoot, Path.GetDirectoryName(full)!);
            string? checkout = NestedCheckoutOn(fullRoot, directory);
            if (checkout is not null)
            {
                throw new ComponentSourceException(
                    $"the project '{projectPath}' lies inside '{checkout}', which holds a .git entry: " +
                    "that is another component's checkout, not a product project of this one");
            }

            projects.Add(new ComponentProject(Relative(fullRoot, full), directory, AssemblyNameOf(full)));
        }

        var projectDirectories = new HashSet<string>(
            projects.Select(static project => project.Directory), StringComparer.OrdinalIgnoreCase);

        var files = new Dictionary<string, ComponentSourceFile>(StringComparer.Ordinal);
        var excluded = new List<ComponentExcludedFile>();

        foreach (ComponentProject project in projects)
        {
            string start = project.Directory.Length == 0
                ? fullRoot
                : Path.Combine(fullRoot, project.Directory.Replace('/', Path.DirectorySeparatorChar));

            foreach (string file in Walk(fullRoot, start, projectDirectories))
            {
                string relative = Relative(fullRoot, file);
                if (config?.ExclusionFor(relative) is { } exclusion)
                {
                    excluded.Add(new ComponentExcludedFile(relative, exclusion.Reason));
                    continue;
                }

                files.TryAdd(relative, new ComponentSourceFile(relative, file, project));
            }
        }

        return new ComponentSourceSet(
            projects,
            [.. files.Values.OrderBy(static file => file.RelativePath, StringComparer.Ordinal)],
            [.. excluded.OrderBy(static file => file.RelativePath, StringComparer.Ordinal)],
            notes);
    }

    /// <summary>
    /// Keeps only the files a list names, one root-relative path per line. Blank
    /// lines and lines starting with <c>#</c> are ignored. A named path that is
    /// not covered is reported in <paramref name="unknown"/> rather than failing
    /// the run, because a changed-files list from a pull request names tests and
    /// documents too.
    /// </summary>
    public static ComponentSourceSet Restrict(
        ComponentSourceSet set, IEnumerable<string> listed, out IReadOnlyList<string> unknown)
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentNullException.ThrowIfNull(listed);

        var wanted = new HashSet<string>(StringComparer.Ordinal);
        foreach (string line in listed)
        {
            string path = line.Trim().Replace('\\', '/');
            if (path.StartsWith("./", StringComparison.Ordinal))
                path = path[2..];

            if (path.Length > 0 && !path.StartsWith('#'))
                wanted.Add(path);
        }

        var covered = set.Files.Where(file => wanted.Contains(file.RelativePath)).ToList();
        unknown = [.. wanted
            .Where(path => !set.Files.Any(file => string.Equals(file.RelativePath, path, StringComparison.Ordinal)))
            .OrderBy(static path => path, StringComparer.Ordinal)];

        return set with { Files = covered };
    }

    /// <summary>
    /// The assembly a project builds: its last unconditional <c>AssemblyName</c>,
    /// or the file name without <c>.csproj</c>. A value that refers to an MSBuild
    /// property cannot be evaluated here and falls back to the file name too.
    /// </summary>
    internal static string AssemblyNameOf(string projectPath)
    {
        string fallback = Path.GetFileNameWithoutExtension(projectPath);

        XDocument document;
        try
        {
            document = XDocument.Load(projectPath);
        }
        catch (XmlException)
        {
            return fallback;
        }

        XElement? declared = document.Descendants()
            .Where(static element => element.Name.LocalName == "AssemblyName")
            .Where(static element => element.Attribute("Condition") is null &&
                element.Parent?.Attribute("Condition") is null)
            .LastOrDefault();

        string? value = declared?.Value.Trim();
        return string.IsNullOrEmpty(value) || value.Contains("$(", StringComparison.Ordinal) ? fallback : value;
    }

    private static IEnumerable<string> Walk(string root, string directory, HashSet<string> projectDirectories)
    {
        var pending = new Stack<string>();
        pending.Push(directory);

        while (pending.Count > 0)
        {
            string current = pending.Pop();

            foreach (string file in Directory.EnumerateFiles(current)
                .Where(static file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)))
            {
                yield return file;
            }

            foreach (string sub in Directory.EnumerateDirectories(current))
            {
                string name = Path.GetFileName(sub);
                if (BuildOutput.Contains(name, StringComparer.OrdinalIgnoreCase))
                    continue;

                if (IsNestedCheckout(sub) || IsLink(sub))
                    continue;

                // Another covered project's directory is walked for that project.
                if (projectDirectories.Contains(Relative(root, sub)))
                    continue;

                pending.Push(sub);
            }
        }
    }

    /// <summary>
    /// The first directory below the root, down to and including
    /// <paramref name="relativeDirectory"/>, that holds a <c>.git</c> entry, or
    /// null. The root's own <c>.git</c> is the component's and does not count.
    /// </summary>
    private static string? NestedCheckoutOn(string root, string relativeDirectory)
    {
        if (relativeDirectory.Length == 0)
            return null;

        string current = root;
        string walked = string.Empty;
        foreach (string segment in relativeDirectory.Split('/'))
        {
            current = Path.Combine(current, segment);
            walked = walked.Length == 0 ? segment : walked + "/" + segment;
            if (IsNestedCheckout(current))
                return walked;
        }

        return null;
    }

    private static bool IsNestedCheckout(string directory)
    {
        string git = Path.Combine(directory, ".git");
        return Directory.Exists(git) || File.Exists(git);
    }

    private static bool IsLink(string directory)
    {
        try
        {
            return new DirectoryInfo(directory).Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
        catch (IOException)
        {
            return true;
        }
    }

    /// <summary>
    /// A guess at the product projects, used only when there is no
    /// configuration: every project that is not a test, a benchmark, a sample or
    /// a diagnostic, outside nested checkouts and build output.
    /// </summary>
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

    private static string Relative(string root, string path)
    {
        string relative = Path.GetRelativePath(root, path).Replace('\\', '/');
        return relative == "." ? string.Empty : relative;
    }
}
