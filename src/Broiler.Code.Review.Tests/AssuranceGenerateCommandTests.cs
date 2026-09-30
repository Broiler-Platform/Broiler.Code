using System.Text;
using System.Text.Json;
using Broiler.Code.Review.Cli.Assurance;
using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Review.Tests;

/// <summary>
/// <c>assurance generate</c>, <c>check</c> and <c>status</c> through the
/// command, over files on disk: what is written, what is kept byte for byte,
/// and what is refused.
/// </summary>
public sealed class AssuranceGenerateCommandTests
{
    private const string Config =
        """
        {
          "schema": 1,
          "component": "Probe",
          "projects": [ "src/Probe/Probe.csproj" ],
          "spdx": { "copyright": [ "2026 Broiler Platform contributors" ], "license": "Apache-2.0" }
        }
        """;

    private const string Counter =
        "namespace Probe;\n" +
        "\n" +
        "// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=TBF\n" +
        "// Broiler-Human:        PENDING\n" +
        "public sealed class Counter\n" +
        "{\n" +
        "    public int Count { get; set; }\n" +
        "}\n";

    private static TemporaryComponent Component(string? config = Config)
    {
        var component = new TemporaryComponent();
        component.Project("src/Probe/Probe.csproj", assemblyName: "Probe");
        if (config is not null)
            component.Write("assurance.config.json", config);

        return component;
    }

    [Theory(Timeout = 600000)]
    [InlineData(null, "does not exist. A component opts in to written annotations by committing that file; generate writes nothing without it.")]
    [InlineData("""{ "mode": "external", "projects": [ "src/Probe/Probe.csproj" ], "spdx": { "copyright": [ "x" ], "license": "MIT" } }""", "\"mode\": \"external\"")]
    [InlineData("""{ "projects": [ "src/Probe/Probe.csproj" ] }""", "$.spdx is required")]
    public void Generate_Needs_An_Owned_Configuration_With_Spdx_Lines(string? config, string expected)
    {
        using TemporaryComponent component = Component(config);
        component.Write("src/Probe/Counter.cs", Counter);

        (int exit, _, string error) = component.Run("generate", "--root", component.Root);

        Assert.Equal(AssuranceCommand.UsageError, exit);
        Assert.Contains(expected, error, StringComparison.Ordinal);
        Assert.Equal(Counter, component.Read("src/Probe/Counter.cs"));
    }

    /// <summary>
    /// A byte-order mark stays first with the header after it; a CRLF file
    /// gets CRLF header lines; a mixed file keeps every ending it had; and a
    /// second run writes nothing.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Generate_Keeps_Byte_Order_Marks_And_Line_Endings_And_Is_A_Fixed_Point()
    {
        using TemporaryComponent component = Component();
        string crlf = Counter.Replace("\n", "\r\n", StringComparison.Ordinal);
        string mixed =
            "namespace Probe;\n\n" +
            "// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=TBF\r\n" +
            "// Broiler-Human:        PENDING\n" +
            "public enum Mode\r\n{\r\n    On,\n    Off,\r\n}";
        component.Write("src/Probe/Counter.cs", crlf, byteOrderMark: true);
        component.Write("src/Probe/Mode.cs", mixed);

        (int exit, string output, string error) = component.Run("generate", "--root", component.Root);
        Assert.True(exit == 0, error);
        Assert.Contains("5 written, 0 failed.", output, StringComparison.Ordinal);

        byte[] counter = component.ReadBytes("src/Probe/Counter.cs");
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, counter.Take(3));
        string counterText = Encoding.UTF8.GetString(counter, 3, counter.Length - 3);
        Assert.StartsWith("// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\r\n", counterText, StringComparison.Ordinal);
        Assert.Contains("// GENERATED - DO NOT EDIT MANUALLY\r\n\r\nnamespace Probe;\r\n", counterText, StringComparison.Ordinal);
        Assert.DoesNotContain("\n", counterText.Replace("\r\n", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal);

        string modeText = component.Read("src/Probe/Mode.cs");
        string mode = AssurancePlanning.Fingerprint(mixed, ".Mode");
        Assert.EndsWith(
            "// GENERATED - DO NOT EDIT MANUALLY\n\n" + mixed.Replace("Fingerprint=TBF", "Fingerprint=" + mode, StringComparison.Ordinal),
            modeText,
            StringComparison.Ordinal);

        Dictionary<string, byte[]> before = Snapshot(component);
        (int again, string againOutput, _) = component.Run("generate", "--root", component.Root);
        Assert.Equal(0, again);
        Assert.Contains("0 artefacts to write", againOutput, StringComparison.Ordinal);
        Assert.Equal(before, Snapshot(component));

        (int check, string checkOutput, _) = component.Run("check", "--root", component.Root);
        Assert.Equal(0, check);
        Assert.Equal("Probe: no violations.\n", checkOutput.Replace("\r\n", "\n", StringComparison.Ordinal));
    }

    /// <summary>
    /// A component that keeps a hand-written review record keeps it: the
    /// generator refuses and writes nothing, until the configuration points
    /// the record elsewhere or the run says to adopt it.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Generate_Refuses_To_Replace_A_Hand_Written_Record_Unless_Told_To_Adopt_It()
    {
        using TemporaryComponent component = Component();
        component.Write("src/Probe/Counter.cs", Counter);
        component.Write("HUMAN_REVIEW.md", "# Human Review\n\nApproved for preview by a person, 2026-07-01.\n");

        (int exit, _, string error) = component.Run("generate", "--root", component.Root);

        Assert.Equal(AssuranceCommand.Refused, exit);
        Assert.Contains(
            "HUMAN_REVIEW.md exists and was not written by the generator (it carries no 'GENERATED - DO NOT EDIT MANUALLY' line)",
            error,
            StringComparison.Ordinal);
        Assert.Contains("nothing was written.", error, StringComparison.Ordinal);
        Assert.Equal(Counter, component.Read("src/Probe/Counter.cs"));
        Assert.False(File.Exists(component.PathOf("CODE-ASSURANCE.md")));

        (int checkExit, string checkOutput, _) = component.Run("check", "--root", component.Root);
        Assert.Equal(AssuranceCommand.Refused, checkExit);
        Assert.Contains("HUMAN_REVIEW.md was not written by the generator", checkOutput, StringComparison.Ordinal);

        (int adopted, _, _) = component.Run("generate", "--root", component.Root, "--adopt");
        Assert.Equal(0, adopted);
        Assert.StartsWith("# Human Review: Probe\n\nGENERATED - DO NOT EDIT MANUALLY.", component.Read("HUMAN_REVIEW.md"), StringComparison.Ordinal);
    }

    [Fact(Timeout = 600000)]
    public void A_Dry_Run_Writes_Nothing()
    {
        using TemporaryComponent component = Component();
        component.Write("src/Probe/Counter.cs", Counter);

        (int exit, string output, _) = component.Run("generate", "--root", component.Root, "--dry-run");

        Assert.Equal(0, exit);
        Assert.Contains("  would write src/Probe/Counter.cs\n", output.Replace("\r\n", "\n", StringComparison.Ordinal), StringComparison.Ordinal);
        Assert.Contains("  would write CODE-ASSURANCE.md (new)", output, StringComparison.Ordinal);
        Assert.Equal(Counter, component.Read("src/Probe/Counter.cs"));
        Assert.False(File.Exists(component.PathOf("assurance.manifest.json")));
    }

    /// <summary>
    /// A violation is a GitHub workflow command on its file and line, with a
    /// multi-line message escaped, and the JSON report says the same.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Check_Reports_Workflow_Commands_And_Exits_One()
    {
        using TemporaryComponent component = Component();
        component.Write("src/Probe/Counter.cs", Counter.Replace("// Broiler-AI:", "// Broiler-AI: ", StringComparison.Ordinal) +
            "public static class Loose { public static int Twice(int x) => x * 2; }\n");

        (int exit, string output, string error) = component.Run("check", "--root", component.Root, "--json", "-");

        Assert.Equal(AssuranceCommand.Refused, exit);
        Assert.Contains(
            "::error file=src/Probe/Counter.cs,line=1::J5 src/Probe/Counter.cs(1) is not what the generator would write.%0A  on disk:   namespace Probe;%0A",
            error,
            StringComparison.Ordinal);
        Assert.Contains(
            "::error file=src/Probe/Counter.cs,line=5::J3 src/Probe/Counter.cs(5): Probe.Counter still records the placeholder TBF",
            error,
            StringComparison.Ordinal);
        Assert.Contains(
            "::error file=src/Probe/Counter.cs,line=9::J1 src/Probe/Counter.cs(9): Probe.Loose is relevant and carries no assurance annotation",
            error,
            StringComparison.Ordinal);
        Assert.Contains("Probe: 8 violations: J1 2, J3 1, J5 4, J7 1.", error, StringComparison.Ordinal);

        using JsonDocument report = JsonDocument.Parse(output);
        Assert.Equal(8, report.RootElement.GetProperty("violationCount").GetInt32());
        Assert.Equal(2, report.RootElement.GetProperty("rules").GetProperty("J1").GetInt32());
    }

    [Fact(Timeout = 600000)]
    public void Check_Release_Also_Reports_Unresolved_Units()
    {
        using TemporaryComponent component = Component();
        component.Write("src/Probe/Counter.cs", Counter);
        Assert.Equal(0, component.Run("generate", "--root", component.Root).Exit);

        (int exit, string output, _) = component.Run("check", "--root", component.Root, "--release");

        Assert.Equal(AssuranceCommand.Refused, exit);
        Assert.Contains(
            "::error file=src/Probe/Counter.cs,line=22::J11 src/Probe/Counter.cs(22): Probe.Counter is HUMAN_PENDING and its human line reads 'PENDING'",
            output,
            StringComparison.Ordinal);
    }

    /// <summary>
    /// A checkout that converts the generated Markdown and JSON to CRLF is not
    /// reported as stale, and a regeneration writes them back in CRLF.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Generated_Artefacts_Keep_The_Line_Endings_A_Checkout_Gave_Them()
    {
        using TemporaryComponent component = Component();
        component.Write("src/Probe/Counter.cs", Counter);
        Assert.Equal(0, component.Run("generate", "--root", component.Root).Exit);

        foreach (string artefact in new[] { "CODE-ASSURANCE.md", "HUMAN_REVIEW.md", "assurance.manifest.json" })
            component.Write(artefact, component.Read(artefact).Replace("\n", "\r\n", StringComparison.Ordinal));

        Assert.Equal(0, component.Run("check", "--root", component.Root).Exit);

        component.Write("src/Probe/Counter.cs", component.Read("src/Probe/Counter.cs").Replace(
            "    public int Count { get; set; }\n", "    public int Count { get; set; }\n\n    public int Limit;\n", StringComparison.Ordinal));
        Assert.Equal(0, component.Run("generate", "--root", component.Root).Exit);

        string report = component.Read("CODE-ASSURANCE.md");
        Assert.Contains("| Code units | 3 |\r\n", report, StringComparison.Ordinal);
        Assert.DoesNotContain("\n", report.Replace("\r\n", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal);
        Assert.Equal(0, component.Run("check", "--root", component.Root).Exit);
    }

    [Fact(Timeout = 600000)]
    public void Status_Summarizes_The_Tree_As_It_Is()
    {
        using TemporaryComponent component = Component();
        component.Write("src/Probe/Counter.cs", Counter + "public static class Loose { }\n");

        (int exit, string output, _) = component.Run("status", "--root", component.Root);

        Assert.Equal(0, exit);
        Assert.Equal(
            "Probe: 1 covered files (0 not covered), 3 code units, 2 relevant, 1 exempt\n" +
            "  Annotated:       1 of 2 (50%)\n" +
            "  Human-reviewed:  0 of 2 (0%)\n" +
            "  Human line:      1 PENDING, 0 STALE\n" +
            "  States:          NEW 1, AI_ASSESSED 1, HUMAN_PENDING 0, HUMAN_APPROVED_PENDING_FINGERPRINT 0, VERIFIED 0, STALE 0, EXEMPT 1\n" +
            "  Security:        None 0, Low 1, Medium 0, High 0, Critical 0, not assessed 1\n" +
            "  Generate:        would write 4 artefacts\n",
            output.Replace("\r\n", "\n", StringComparison.Ordinal));
    }

    /// <summary>A read-only command may take its configuration from outside the component.</summary>
    [Fact(Timeout = 600000)]
    public void Check_Reads_A_Configuration_Named_On_The_Command_Line()
    {
        using TemporaryComponent component = Component(config: null);
        component.Write("src/Probe/Counter.cs", Counter);
        component.Write("elsewhere/assurance.json", Config);

        (int missing, _, string error) = component.Run("check", "--root", component.Root);
        Assert.Equal(AssuranceCommand.UsageError, missing);
        Assert.Contains("check needs it, or --config naming one.", error, StringComparison.Ordinal);

        (int exit, string output, _) = component.Run(
            "check", "--root", component.Root, "--config", component.PathOf("elsewhere/assurance.json"));
        Assert.Equal(AssuranceCommand.Refused, exit);
        Assert.Contains("Probe: 6 violations: J3 1, J5 4, J7 1.", output, StringComparison.Ordinal);
    }

    private static Dictionary<string, byte[]> Snapshot(TemporaryComponent component) =>
        Directory.EnumerateFiles(component.Root, "*", SearchOption.AllDirectories)
            .ToDictionary(path => Path.GetRelativePath(component.Root, path), File.ReadAllBytes);

    /// <summary>The workflow-command escaping the runner requires.</summary>
    [Fact(Timeout = 600000)]
    public void A_Workflow_Command_Escapes_What_The_Runner_Would_Misread()
    {
        Assert.Equal(
            "::error file=a%3Ab%2Cc.cs,line=3::J2 50%25 done%0Anext",
            AssuranceCommand.WorkflowCommand(new AssuranceViolation("J2", "a:b,c.cs", 3, "50% done\nnext")));
        Assert.Equal("::error::J7 whole", AssuranceCommand.WorkflowCommand(new AssuranceViolation("J7", null, null, "whole")));
    }
}
