using System.Xml.Linq;

namespace Broiler.Code.Core.Tests;

/// <summary>
/// The dependency rules ADR 0021 and the architecture document state. They are
/// asserted against local project files and restored component assemblies — a
/// comment saying "no Roslyn here" does not stop a reference being added.
/// </summary>
public sealed class CodeEditorArchitectureTests
{
    [Fact(Timeout = 600000)]
    public void The_Abstraction_References_Only_Ui_And_Graphics()
    {
        // Inspect the assembly restored from NuGet; component source is not in this checkout.
        string[] references = [.. System.Reflection.Assembly.Load("Broiler.UI.CodeEditor")
            .GetReferencedAssemblies()
            .Select(reference => reference.Name!)
            .Where(name => name.StartsWith("Broiler.", StringComparison.Ordinal))
            .OrderBy(name => name, StringComparer.Ordinal)];

        Assert.Equal(["Broiler.Graphics", "Broiler.UI"], references);
    }

    [Fact(Timeout = 600000)]
    public void The_Abstraction_Depends_On_No_Language_Service_Or_Product_Assembly()
    {
        string[] references = [.. System.Reflection.Assembly.Load("Broiler.UI.CodeEditor")
            .GetReferencedAssemblies().Select(reference => reference.Name!)];

        Assert.DoesNotContain(references, reference =>
            reference.Contains("Roslyn", StringComparison.OrdinalIgnoreCase) ||
            reference.Contains("CodeAnalysis", StringComparison.OrdinalIgnoreCase) ||
            reference.Contains("Broiler.Code", StringComparison.Ordinal) ||
            reference.Contains("Standard", StringComparison.Ordinal) ||
            reference.Contains("Windows", StringComparison.OrdinalIgnoreCase) ||
            reference.Contains("Android", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// The text layer is the base of both the editor and the later workspace
    /// model, so it must stay free of UI and of a language service. A reference
    /// added here would reach every host.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Text_Layer_Has_No_Dependencies_At_All()
    {
        XDocument project = Load("src", "Broiler.Code.Workspaces", "Broiler.Code.Workspaces.csproj");

        Assert.Empty(project.Descendants("PackageReference"));
        Assert.Empty(project.Descendants("ProjectReference"));
    }

    /// <summary>
    /// Core is the only assembly that knows about both sides. That is the seam,
    /// and it must not acquire a third role: it composes control
    /// <em>abstractions</em> and the workspace, never a Standard implementation
    /// and never a platform head. A reference to either would make the shell
    /// impossible to host anywhere the implementation does not exist.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Core_Composes_Only_Abstractions_And_The_Workspace()
    {
        string[] references = References("src", "Broiler.Code.Core", "Broiler.Code.Core.csproj");

        // Listed exactly rather than by pattern: an exact list is what catches
        // a reference added without thinking about which side of the seam it
        // puts Core on.
        Assert.Equal(
        [
            // The review model sits below Core on the same side of the seam as
            // the workspace: no UI, no Roslyn, no platform. Its own rule is
            // asserted by The_Review_Model_Depends_Only_On_The_Workspace below.
            "../Broiler.Code.Review/Broiler.Code.Review.csproj",
            "../Broiler.Code.Workspaces/Broiler.Code.Workspaces.csproj",
        ],
            references);

        string[] packages = [.. Load("src", "Broiler.Code.Core", "Broiler.Code.Core.csproj")
            .Descendants("PackageReference")
            .Select(reference => (string)reference.Attribute("Include")!)
            .OrderBy(name => name, StringComparer.Ordinal)];
        Assert.Equal(
        [
            "Broiler.UI.Button", "Broiler.UI.CodeEditor", "Broiler.UI.ComboBox",
            "Broiler.UI.Edit", "Broiler.UI.Label", "Broiler.UI.Menu", "Broiler.UI.Panel",
            "Broiler.UI.Splitter", "Broiler.UI.TabView", "Broiler.UI.Toolbar", "Broiler.UI.TreeView",
        ], packages);
    }

    /// <summary>
    /// The review model is evidence about the source, so it is held to the same
    /// rule as the text layer: it may know about a workspace and nothing else.
    ///
    /// Asserted rather than described because the two things most likely to be
    /// added to it are the two that would break it — a UI reference, so a pane
    /// can render a status directly, and a git package, so staleness can be
    /// decided from a commit. The first would stop CI computing coverage with no
    /// display; the second would make a review expire on a rebase.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Review_Model_Depends_Only_On_The_Workspace()
    {
        XDocument project = Load("src", "Broiler.Code.Review", "Broiler.Code.Review.csproj");

        Assert.Empty(project.Descendants("PackageReference"));
        Assert.Equal(
            ["../Broiler.Code.Workspaces/Broiler.Code.Workspaces.csproj"],
            References(project));
    }

    private static string[] References(params string[] parts) => References(Load(parts));

    private static string[] References(XDocument project) => [.. project
        .Descendants("ProjectReference")
        .Select(reference => ((string?)reference.Attribute("Include"))?.Replace('\\', '/'))
        .Where(reference => reference is not null)
        .Cast<string>()
        .OrderBy(reference => reference, StringComparer.Ordinal)];

    private static XDocument Load(params string[] parts) => XDocument.Load(RepositoryPath(parts));

    private static string RepositoryPath(params string[] parts)
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Broiler.Code.Tests.slnx")) &&
                File.Exists(Path.Combine(directory.FullName, "Directory.Build.props")))
            {
                return Path.Combine([directory.FullName, .. parts]);
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root not found.");
    }
}
