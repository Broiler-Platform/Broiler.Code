using System.Text.Json;
using Broiler.Code.Language.CSharp.Assurance;
using Broiler.Code.Review.Assurance;
using Broiler.Code.Review.Cli.Assurance;

namespace Broiler.Code.Review.Tests;

/// <summary>
/// A Broiler.VM checkout, when one is beside this repository or at the
/// platform's usual place. Its own architecture tests generate and gate its
/// assurance record; this tool must agree with them byte for byte.
/// </summary>
internal static class VmCheckout
{
    public static string? Root { get; } = Find();

    private static string? Find()
    {
        var candidates = new List<string>();
        if (Environment.GetEnvironmentVariable("BROILER_VM_ROOT") is { Length: > 0 } configured)
            candidates.Add(configured);

        // The sibling of this repository's root, which is where the platform
        // checks its components out side by side.
        for (DirectoryInfo? directory = new(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Broiler.Code.Tests.slnx")))
            {
                if (directory.Parent is { } parent)
                    candidates.Add(Path.Combine(parent.FullName, "Broiler.VM"));

                break;
            }
        }

        candidates.Add(@"D:\Broiler\Broiler.VM");

        return candidates.FirstOrDefault(static root =>
            File.Exists(Path.Combine(root, "Broiler.VM.slnx")) &&
            File.Exists(Path.Combine(root, "assurance.manifest.json")));
    }
}

/// <summary>A fact that runs only where a Broiler.VM checkout is.</summary>
public sealed class VmCheckoutFactAttribute : FactAttribute
{
    public VmCheckoutFactAttribute()
    {
        Timeout = 600000;
        if (VmCheckout.Root is null)
            Skip = "No Broiler.VM checkout beside this repository, at D:\\Broiler\\Broiler.VM or in BROILER_VM_ROOT.";
    }
}

/// <summary>
/// The golden test: over Broiler.VM, whose own generator wrote every header,
/// block and manifest entry, this tool's plan changes nothing.
///
/// External mode, sources only: VM's report and record are its own prose,
/// held by its own tests, and are not compared. Everything else is, byte for
/// byte: every covered file's header and blocks, and the manifest's
/// <c>files</c> and <c>units</c> arrays. Every rule runs as well, with VM's
/// own settings: its directive ban, its strict forgery vocabulary, its ADR
/// directory and its assembly closed to <c>EXEMPT=</c>.
/// </summary>
public sealed class AssuranceVmGoldenTests
{
    private const string ExternalConfig =
        """
        {
          "schema": 1,
          "component": "Broiler.VM",
          "mode": "external",
          "projects": [
            "src/Broiler.VM.Abstractions/Broiler.VM.Abstractions.csproj",
            "src/Broiler.VM.Binary/Broiler.VM.Binary.csproj",
            "src/Broiler.VM.Emitter.Bytecode/Broiler.VM.Emitter.Bytecode.csproj",
            "src/Broiler.VM.Profile.JavaScript/Broiler.VM.Profile.JavaScript.csproj",
            "src/Broiler.VM.Profile.JavaScript.Compiler/Broiler.VM.Profile.JavaScript.Compiler.csproj",
            "src/Broiler.VM.Profile.JavaScript.Format/Broiler.VM.Profile.JavaScript.Format.csproj",
            "src/Broiler.VM.Profile.MachineCode/Broiler.VM.Profile.MachineCode.csproj",
            "src/Broiler.VM.Profile.WebAssembly/Broiler.VM.Profile.WebAssembly.csproj",
            "src/Broiler.VM.Runtime/Broiler.VM.Runtime.csproj",
            "src/Broiler.VM.Ubc/Broiler.VM.Ubc.csproj"
          ],
          "spdx": { "copyright": [ "2026 Broiler Platform contributors" ], "license": "Apache-2.0" },
          "forbidDirectives": true,
          "forgeryVocabulary": "strict",
          "closedToEscapeHatch": [ "Broiler.VM.Binary" ],
          "regenerateCommand": "BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release"
        }
        """;

    [VmCheckoutFact]
    public void Check_Sources_Only_Finds_No_Difference_From_Broiler_VM()
    {
        string root = VmCheckout.Root!;
        using var scratch = new TemporaryComponent();
        scratch.Write("vm.assurance.config.json", ExternalConfig);

        (int exit, string output, string error) = scratch.Run(
            "check", "--root", root, "--config", scratch.PathOf("vm.assurance.config.json"), "--sources-only", "--json", "-");

        using JsonDocument report = JsonDocument.Parse(output);
        Assert.True(exit == 0, error);
        Assert.Equal(0, report.RootElement.GetProperty("violationCount").GetInt32());
    }

    /// <summary>
    /// The same comparison stated directly, so that a failure names the file,
    /// the unit or the array that differs rather than a count.
    /// </summary>
    [VmCheckoutFact]
    public void Every_Header_Fingerprint_And_Manifest_Entry_Is_Broiler_VMs_Own()
    {
        string root = VmCheckout.Root!;
        AssuranceComponentConfig config = AssuranceComponentConfig.Parse(ExternalConfig);
        ComponentCorpus loaded = ComponentCorpus.Load(root, config);
        AssurancePlan plan = AssuranceGenerator.Plan(loaded.Corpus, new CSharpAssuranceFileScanner(config.PreprocessorSymbols), config);

        Assert.Empty(loaded.Problems);
        Assert.Empty(plan.Problems);

        // Every covered file: header, blocks and all, byte for byte.
        Assert.All(
            plan.Artefacts.Where(static artefact => artefact.Kind == AssuranceArtefactKind.Source),
            artefact => Assert.True(artefact.IsCurrent, AssuranceGenerator.Describe(artefact, "(golden)")));

        // The manifest's arrays, byte for byte, and as many entries as VM's.
        AssuranceArtefact manifest = plan.Artefacts.Single(static artefact => artefact.Kind == AssuranceArtefactKind.Manifest);
        Assert.Equal(AssuranceManifest.ArraysOf(manifest.Current), AssuranceManifest.ArraysOf(manifest.Desired));

        using JsonDocument recorded = JsonDocument.Parse(manifest.Current);
        Assert.Equal(recorded.RootElement.GetProperty("files").GetArrayLength(), plan.Files.Count);
        Assert.Equal(recorded.RootElement.GetProperty("units").GetArrayLength(), plan.UnitsAfter.Count);

        // Every fingerprint an AI line records is the one this scanner computes.
        Assert.All(
            plan.UnitsBefore.Where(static unit => unit.Annotation is { ExemptReason: null }),
            static unit => Assert.Equal(unit.Fingerprint, unit.Annotation!.RecordedFingerprint));

        // Nothing would be written but the three artefacts whose prose is VM's:
        // the two documents and the manifest's $comment.
        Assert.Equal(
            [config.Artefacts.Report, config.Artefacts.HumanReview, config.Artefacts.Manifest],
            plan.Changes.Select(static artefact => artefact.RelativePath).Order(StringComparer.Ordinal));
    }
}
