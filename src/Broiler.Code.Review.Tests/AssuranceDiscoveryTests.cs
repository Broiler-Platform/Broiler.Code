using System.Text.Json;
using Broiler.Code.Review.Assurance;
using Broiler.Code.Review.Cli.Assurance;

namespace Broiler.Code.Review.Tests;

/// <summary>
/// Which files a component's assurance covers, and what <c>assurance list</c>
/// reports about them.
///
/// Components carry nested checkouts of other components, untracked stale
/// copies of them, and tracked build output. Each of those would add files that
/// are not this component's, and a tool that annotated them would be writing
/// into somebody else's source.
/// </summary>
public sealed class AssuranceDiscoveryTests
{
    private const string Relevant = "namespace N;\npublic sealed class C\n{\n    public void Run() { System.Console.WriteLine(); }\n}\n";

    [Fact(Timeout = 600000)]
    public void Build_Output_Nested_Checkouts_And_Exclusions_Are_Not_Covered()
    {
        using var component = new TemporaryComponent();
        component.Project("src/A/A.csproj", assemblyName: "Custom.A");
        component.Write("src/A/X.cs", Relevant);
        component.Write("src/A/Sub/Y.cs", Relevant);
        component.Write("src/A/bin/Release/Z.cs", Relevant);
        component.Write("src/A/Deep/obj/Debug/W.AssemblyAttributes.cs", Relevant);
        component.Write("src/A/Nested/.git", "gitdir: ../../.git/modules/Nested\n");
        component.Write("src/A/Nested/N.cs", Relevant);
        component.Folder("src/A/Stale/.git");
        component.Write("src/A/Stale/S.cs", Relevant);
        component.Write("src/A/Gen/X.g.cs", Relevant);
        component.Project("src/B/B.csproj");
        component.Write("src/B/Q.cs", Relevant);

        AssuranceComponentConfig config = AssuranceComponentConfig.Parse(
            """{ "projects": [ "src/A/A.csproj" ], "exclude": [ { "glob": "**/*.g.cs", "reason": "regenerated" } ] }""");

        ComponentSourceSet set = ComponentSources.Discover(component.Root, config);

        Assert.Equal(["src/A/Sub/Y.cs", "src/A/X.cs"], set.Files.Select(file => file.RelativePath));
        Assert.All(set.Files, file => Assert.Equal("Custom.A", file.Project.AssemblyName));
        Assert.Equal([new ComponentExcludedFile("src/A/Gen/X.g.cs", "regenerated")], set.Excluded);
    }

    /// <summary>
    /// A configured project inside another's directory is walked once, for
    /// itself, so its files are neither covered twice nor credited to the
    /// wrong assembly.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Nested_Project_Owns_Its_Own_Files()
    {
        using var component = new TemporaryComponent();
        component.Project("src/A/A.csproj");
        component.Project("src/A/Inner/Inner.csproj");
        component.Write("src/A/X.cs", Relevant);
        component.Write("src/A/Inner/I.cs", Relevant);

        ComponentSourceSet set = ComponentSources.Discover(
            component.Root,
            AssuranceComponentConfig.Parse("""{ "projects": [ "src/A/A.csproj", "src/A/Inner/Inner.csproj" ] }"""));

        Assert.Equal(
            [("src/A/Inner/I.cs", "Inner"), ("src/A/X.cs", "A")],
            set.Files.Select(file => (file.RelativePath, file.Project.AssemblyName)));
    }

    [Fact(Timeout = 600000)]
    public void A_Project_Inside_A_Nested_Checkout_Is_Refused()
    {
        using var component = new TemporaryComponent();
        component.Folder("Broiler.Graphics/.git");
        component.Project("Broiler.Graphics/src/G/G.csproj");

        ComponentSourceException exception = Assert.Throws<ComponentSourceException>(() =>
            ComponentSources.Discover(
                component.Root,
                AssuranceComponentConfig.Parse("""{ "projects": [ "Broiler.Graphics/src/G/G.csproj" ] }""")));

        Assert.Contains("'Broiler.Graphics', which holds a .git entry", exception.Message, StringComparison.Ordinal);
    }

    [Fact(Timeout = 600000)]
    public void A_Missing_Project_Is_Refused()
    {
        using var component = new TemporaryComponent();

        Assert.Throws<ComponentSourceException>(() => ComponentSources.Discover(
            component.Root, AssuranceComponentConfig.Parse("""{ "projects": [ "src/A/A.csproj" ] }""")));
    }

    /// <summary>
    /// Without a configuration the product projects are guessed, and the run
    /// says so: a guess is fine for looking, and is why writing needs the file.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Without_A_Config_Test_Projects_Are_Not_Guessed_As_Products()
    {
        using var component = new TemporaryComponent();
        component.Project("src/A/A.csproj");
        component.Project("src/A.Tests/A.Tests.csproj");
        component.Project("src/Checks/Checks.csproj", isTest: true);
        component.Project("tests/Harness/Harness.csproj");

        ComponentSourceSet set = ComponentSources.Discover(component.Root, config: null);

        Assert.Equal(["src/A/A.csproj"], set.Projects.Select(project => project.RelativePath));
        Assert.Contains(set.Notes, note => note.Contains("guessed", StringComparison.Ordinal));
    }

    [Fact(Timeout = 600000)]
    public void A_Files_List_Restricts_The_Run()
    {
        using var component = new TemporaryComponent();
        component.Project("src/A/A.csproj");
        component.Write("src/A/X.cs", Relevant);
        component.Write("src/A/Y.cs", Relevant);

        ComponentSourceSet set = ComponentSources.Discover(
            component.Root, AssuranceComponentConfig.Parse("""{ "projects": [ "src/A/A.csproj" ] }"""));
        ComponentSourceSet restricted = ComponentSources.Restrict(
            set, ["# changed files", "src\\A\\Y.cs", "./README.md", ""], out IReadOnlyList<string> unknown);

        Assert.Equal(["src/A/Y.cs"], restricted.Files.Select(file => file.RelativePath));
        Assert.Equal(["README.md"], unknown);
    }

    /// <summary>
    /// The list's default is the work left: relevant units carrying no block,
    /// each with what a tool needs to write one. Exempt and annotated units are
    /// counted, and listed only when asked for.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_List_Reports_Unannotated_Relevant_Units_With_Their_Placement()
    {
        using var component = new TemporaryComponent();
        component.Project("src/A/A.csproj");
        component.Write("assurance.config.json", """{ "component": "Probe", "projects": [ "src/A/A.csproj" ] }""");
        component.Write("src/A/X.cs",
            "namespace N;\n" +
            "// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=TBF\n" +
            "// Broiler-Human:        PENDING\n" +
            "public sealed class C\n" +
            "{\n" +
            "    /// <summary>Runs.</summary>\n" +
            "    [System.Obsolete]\n" +
            "    public void Run()\n" +
            "    {\n" +
            "        System.Console.WriteLine();\n" +
            "    }\n" +
            "\n" +
            "    public int Auto { get; set; }\n" +
            "    public int A; public void B() { System.Console.WriteLine(); }\n" +
            "}\n");

        (int exit, string output, _) = component.Run("list", "--root", component.Root, "--json", "-");

        Assert.Equal(0, exit);
        using JsonDocument report = JsonDocument.Parse(output);
        JsonElement root = report.RootElement;
        Assert.Equal("Probe", root.GetProperty("component").GetString());

        JsonElement units = root.GetProperty("units");
        Assert.Equal(2, units.GetArrayLength());

        JsonElement run = units[0];
        Assert.Equal("src/A/X.cs", run.GetProperty("file").GetString());
        Assert.Equal("N.C.Run()", run.GetProperty("unit").GetString());
        Assert.Equal("Run()", run.GetProperty("displayName").GetString());
        Assert.Equal("method", run.GetProperty("kind").GetString());
        Assert.Equal(7, run.GetProperty("line").GetInt32());
        Assert.Equal(5, run.GetProperty("column").GetInt32());
        Assert.Equal("    ", run.GetProperty("indent").GetString());
        Assert.Equal(7, run.GetProperty("extent").GetProperty("startLine").GetInt32());
        Assert.Equal(11, run.GetProperty("extent").GetProperty("endLine").GetInt32());
        Assert.Equal(6, run.GetProperty("fingerprint").GetString()!.Length);
        Assert.True(run.GetProperty("insertable").GetBoolean());
        Assert.Equal("none", run.GetProperty("reason").GetString());
        Assert.Equal("NEW", run.GetProperty("state").GetString());

        JsonElement second = units[1];
        Assert.Equal("N.C.B()", second.GetProperty("unit").GetString());
        Assert.False(second.GetProperty("insertable").GetBoolean());
        Assert.Equal("not-own-line", second.GetProperty("reason").GetString());

        JsonElement file = root.GetProperty("files")[0];
        Assert.Equal(3, file.GetProperty("relevant").GetInt32());
        Assert.Equal(1, file.GetProperty("annotated").GetInt32());
        Assert.Equal(2, file.GetProperty("unannotated").GetInt32());
        Assert.Equal(2, file.GetProperty("exempt").GetInt32());
        Assert.Equal(1, file.GetProperty("insertable").GetInt32());
        Assert.Equal(2, file.GetProperty("listed").GetInt32());

        (_, string all, _) = component.Run("list", "--root", component.Root, "--json", "-", "--all-units");
        using JsonDocument everything = JsonDocument.Parse(all);
        Assert.Equal(5, everything.RootElement.GetProperty("units").GetArrayLength());
        Assert.Contains(
            everything.RootElement.GetProperty("units").EnumerateArray(),
            unit => unit.GetProperty("unit").GetString() == "N.C" &&
                unit.GetProperty("reason").GetString() == "annotated" &&
                unit.GetProperty("state").GetString() == "AI_ASSESSED");
    }

    [Fact(Timeout = 600000)]
    public void The_Text_List_Names_Each_Unit_And_Why_It_Cannot_Take_A_Block()
    {
        using var component = new TemporaryComponent();
        component.Project("src/A/A.csproj");
        component.Write("assurance.config.json", """{ "component": "Probe", "projects": [ "src/A/A.csproj" ] }""");
        component.Write("src/A/X.cs",
            "namespace N;\n" +
            "public sealed class C\n" +
            "{\n" +
            "    // Broiler-Human:        PENDING\n" +
            "    public void Run() { System.Console.WriteLine(); }\n" +
            "}\n");

        (int exit, string output, _) = component.Run("list", "--root", component.Root);

        Assert.Equal(0, exit);
        Assert.StartsWith(
            "Probe: 1 files, 2 units, 2 relevant, 0 annotated, 2 unannotated, 1 insertable",
            output,
            StringComparison.Ordinal);
        Assert.Contains("src/A/X.cs  relevant 2  annotated 0  unannotated 2  insertable 1", output, StringComparison.Ordinal);
        Assert.Contains(
            "N.C.Run()  [NEW] half-block: line 4 carries an assurance line with no '// Broiler-AI:' line above it",
            output,
            StringComparison.Ordinal);
    }

    [Fact(Timeout = 600000)]
    public void An_Unknown_Option_Is_A_Usage_Error()
    {
        using var component = new TemporaryComponent();

        (int exit, _, string error) = component.Run("list", "--root", component.Root, "--everything");

        Assert.Equal(2, exit);
        Assert.Contains("'--everything' is not an option of 'assurance list'", error, StringComparison.Ordinal);
    }
}
