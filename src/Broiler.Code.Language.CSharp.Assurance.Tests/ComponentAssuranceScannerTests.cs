using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Language.CSharp.Assurance.Tests;

/// <summary>
/// The editor's scanner reads a file the way the file's own component does:
/// under the nearest <c>assurance.config.json</c> at or above it, within the
/// granted root. Without that, a component that watches its named values would
/// see every one of them as a unit owing a block the moment it is opened.
/// </summary>
public sealed class ComponentAssuranceScannerTests : IDisposable
{
    private const string Values =
        "namespace Probe;\n" +
        "\n" +
        "public static class Native\n" +
        "{\n" +
        "    public const int EGL_NONE = 0x3038;\n" +
        "}\n";

    private const string Watched =
        """{ "schema": 1, "projects": [ "src/Probe/Probe.csproj" ], "exemptionPredicate": "owning-component", "namedValues": "watched" }""";

    private readonly string _root = Path.Combine(Path.GetTempPath(), "broiler-component-scanner-" + Guid.NewGuid().ToString("N"));

    public ComponentAssuranceScannerTests() => Directory.CreateDirectory(_root);

    public void Dispose() => Directory.Delete(_root, recursive: true);

    private void Write(string relativePath, string text)
    {
        string path = Path.Combine(_root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text);
    }

    private static AssuranceScannedUnit Value(IAssuranceUnitScanner scanner, string path) =>
        scanner.Scan(Values, path).Single(static unit => unit.Name == "Probe.Native.EGL_NONE");

    /// <summary>
    /// A file in a component that watches its named values sees one as exempt,
    /// as that component's command-line runs do; a file of the same text in
    /// another component, or under none, sees it as the owning component does.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Each_File_Is_Scanned_As_Its_Nearest_Configuration_Says()
    {
        Write("assurance.config.json", """{ "schema": 1, "projects": [ "Other/Other.csproj" ] }""");
        Write("Native/assurance.config.json", Watched);
        var scanner = new CSharpComponentAssuranceScanner(_root);

        AssuranceScannedUnit native = Value(scanner, "Native/src/Probe/Values.cs");
        Assert.True(native.IsExempt);
        Assert.Equal("NamedValue", native.Exemption);
        Assert.Equal(Path.Combine(_root, "Native", "assurance.config.json"), scanner.ConfigurationFor("Native/src/Probe/Values.cs"));

        AssuranceScannedUnit other = Value(scanner, "Other/Values.cs");
        Assert.False(other.IsExempt);
        Assert.Equal(Path.Combine(_root, "assurance.config.json"), scanner.ConfigurationFor("Other/Values.cs"));

        // The value is the same unit either way: only the exemption differs.
        Assert.Equal(native.Fingerprint, other.Fingerprint);
        Assert.Equal(new CSharpAssuranceScanner().Scan(Values, "x.cs").Select(static unit => unit.Fingerprint),
            scanner.Scan(Values, "Native/x.cs").Select(static unit => unit.Fingerprint));
    }

    /// <summary>
    /// The root is what the user granted, and nothing above it is read: a
    /// configuration there, or a path that climbs out, leaves the default.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Nothing_Outside_The_Root_Is_Read()
    {
        Write("assurance.config.json", Watched);
        Directory.CreateDirectory(Path.Combine(_root, "src"));
        var scanner = new CSharpComponentAssuranceScanner(Path.Combine(_root, "src"));

        Assert.Null(scanner.ConfigurationFor("Probe/Values.cs"));
        Assert.False(Value(scanner, "Probe/Values.cs").IsExempt);
        Assert.Null(scanner.ConfigurationFor("../Values.cs"));
        Assert.Null(scanner.ConfigurationFor(Path.Combine(_root, "Values.cs")));
        Assert.Null(scanner.ConfigurationFor(string.Empty));
    }

    /// <summary>
    /// A configuration the command-line tool would refuse leaves the default,
    /// rather than an editor that throws; one edited while the editor runs is
    /// read again.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void An_Unreadable_Configuration_Leaves_The_Default_And_An_Edited_One_Is_Read_Again()
    {
        Write("assurance.config.json", """{ "schema": 1, "projects": [ "a.csproj" ], "namedValues": "maybe" }""");
        var scanner = new CSharpComponentAssuranceScanner(_root);

        Assert.False(Value(scanner, "Values.cs").IsExempt);

        Write("assurance.config.json", Watched);
        File.SetLastWriteTimeUtc(Path.Combine(_root, "assurance.config.json"), DateTime.UtcNow.AddMinutes(1));

        Assert.True(Value(scanner, "Values.cs").IsExempt);
    }
}
