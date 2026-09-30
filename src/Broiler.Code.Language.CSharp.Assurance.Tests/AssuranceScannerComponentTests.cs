using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Language.CSharp.Assurance.Tests;

/// <summary>
/// What the scanner does beyond the owning component, for the shapes the other
/// components have and it does not: CRLF checkouts of literals that span
/// lines, directives, repeated names, top-level statements, initializers that
/// run code, and comments of every kind.
///
/// Each is invisible on the owning component's tree, which
/// <see cref="AssuranceScannerTests"/> holds this scanner to; each is asserted
/// here on the shape that needs it.
/// </summary>
public sealed class AssuranceScannerComponentTests
{
    private static IReadOnlyList<AssuranceScannedUnit> Units(
        string text, AssuranceExemptionPredicate predicate = AssuranceExemptionPredicate.OwningComponent) =>
        new CSharpAssuranceScanner(null, predicate).Scan(text, "x.cs");

    private static AssuranceScannedUnit Unit(string text, string name, AssuranceExemptionPredicate predicate = AssuranceExemptionPredicate.OwningComponent) =>
        Units(text, predicate).Single(unit => unit.Name == name);

    /// <summary>
    /// A CRLF working tree of an LF blob gives every unit, and the file, the
    /// fingerprint the blob gives, even where a verbatim or raw string spans
    /// lines and so holds the line break inside a token.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Crlf_Checkout_Of_A_Literal_That_Spans_Lines_Fingerprints_As_The_Lf_Blob()
    {
        string lf =
            "namespace N;\n" +
            "\n" +
            "public static class A\n" +
            "{\n" +
            "    public const string S = @\"x\n" +
            "y\";\n" +
            "\n" +
            "    public static string R() => \"\"\"\n" +
            "        first\n" +
            "        second\n" +
            "        \"\"\";\n" +
            "\n" +
            "    public static string I(int n) => $@\"{n}\n" +
            "tail\";\n" +
            "\n" +
            "    public static int T(System.Collections.Generic.List<(int child, int inline,\n" +
            "            double offX)> list) => list.Count + 1;\n" +
            "}\n";
        string crlf = lf.Replace("\n", "\r\n", StringComparison.Ordinal);
        var files = new CSharpAssuranceFileScanner();

        Assert.Equal(
            Units(lf).Select(static unit => (unit.Name, unit.Fingerprint)),
            Units(crlf).Select(static unit => (unit.Name, unit.Fingerprint)));
        Assert.Equal(files.ScanFile(lf, "x.cs").FileFingerprint, files.ScanFile(crlf, "x.cs").FileFingerprint);
        Assert.Equal(CSharpAssuranceScanner.FingerprintOfFile(lf, "x.cs"), CSharpAssuranceScanner.FingerprintOfFile(crlf, "x.cs"));

        // A lone CR is a line break too, and so is hashed as one.
        Assert.Equal(Unit(lf, "N.A.S").Fingerprint, Unit(lf.Replace("@\"x\n", "@\"x\r", StringComparison.Ordinal), "N.A.S").Fingerprint);

        // A parameter type written across lines is named on one line, so the
        // name does not carry the checkout's line ending either.
        const string Tuple = "N.A.T(System.Collections.Generic.List<(int child, int inline, double offX)>)";
        Assert.Equal(Tuple, Unit(lf, Tuple).Name);
        Assert.Equal(Tuple, Unit(crlf, Tuple).Name);
    }

    /// <summary>
    /// Code under a <c>#else</c> is disabled text under this parse and compiled
    /// under the other configuration. It is in its method's fingerprint, and
    /// in the file's, so rewriting it moves both; rewrapping it moves neither,
    /// because it is hashed by its tokens as the rest is.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Code_Under_A_Directive_Is_In_Its_Units_Fingerprint_And_The_Files()
    {
        const string Method =
            "namespace N;\n" +
            "public static class C\n" +
            "{\n" +
            "    public static int Scale(int value, int factor)\n" +
            "    {\n" +
            "#if DEBUG\n" +
            "        return value * factor;\n" +
            "#else\n" +
            "        return value * factor;\n" +
            "#endif\n" +
            "    }\n" +
            "}\n";

        string rewritten = Method.Replace(
            "#else\n        return value * factor;\n",
            "#else\n        System.IO.File.Delete(\"C:/important\"); return int.MaxValue;\n",
            StringComparison.Ordinal);
        string rewrapped = Method.Replace(
            "#else\n        return value * factor;\n",
            "#else\n        return value\n            * factor; // a comment\n",
            StringComparison.Ordinal);
        string flipped = Method.Replace("#if DEBUG", "#if !DEBUG", StringComparison.Ordinal);
        var files = new CSharpAssuranceFileScanner();

        string original = Unit(Method, "N.C.Scale(int, int)").Fingerprint;
        Assert.NotEqual(original, Unit(rewritten, "N.C.Scale(int, int)").Fingerprint);
        Assert.NotEqual(original, Unit(flipped, "N.C.Scale(int, int)").Fingerprint);
        Assert.Equal(original, Unit(rewrapped, "N.C.Scale(int, int)").Fingerprint);
        Assert.NotEqual(files.ScanFile(Method, "x.cs").FileFingerprint, files.ScanFile(rewritten, "x.cs").FileFingerprint);

        // The directive above a unit is not the unit's own; the class's
        // header and every other unit keep their values.
        Assert.Equal(Unit(Method, "N.C").Fingerprint, Unit(rewritten, "N.C").Fingerprint);
    }

    /// <summary>
    /// No two units of a file share a name. The three shapes that gave two
    /// units one name — overloaded indexers, members of <c>Foo</c> and
    /// <c>Foo&lt;T&gt;</c>, and a partial type declared twice — each get a name
    /// of their own; a name that was unique keeps the owning component's form.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Names_Are_Unique_Within_A_File_And_Otherwise_The_Owning_Components()
    {
        const string Text =
            "namespace N;\n" +
            "public partial class P\n" +
            "{\n" +
            "    public int this[int i] => i + 1;\n" +
            "    public int this[string s, int i] => s.Length + i;\n" +
            "    public int Only { get { return 1 + 1; } }\n" +
            "}\n" +
            "public partial class P\n" +
            "{\n" +
            "}\n" +
            "public static class Box\n" +
            "{\n" +
            "    public static int Kind() => 1 + 1;\n" +
            "}\n" +
            "public static class Box<T>\n" +
            "{\n" +
            "    public static int Kind() => 2 + 2;\n" +
            "}\n";

        IReadOnlyList<AssuranceScannedUnit> units = Units(Text);

        Assert.Equal(
            [
                "N.P", "N.P.this[int]", "N.P.this[string, int]", "N.P.Only", "N.P#2",
                "N.Box", "N.Box.Kind()", "N.Box<T>", "N.Box<T>.Kind()",
            ],
            units.Select(static unit => unit.Name));
        Assert.Equal(units.Count, units.Select(static unit => unit.Name).Distinct(StringComparer.Ordinal).Count());

        // The file scan names them the same way.
        Assert.Equal(units, new CSharpAssuranceFileScanner().ScanFile(Text, "x.cs").Units.Select(static unit => unit.Unit));
    }

    /// <summary>
    /// A file of top-level statements is one relevant unit, its local
    /// functions included, where it used to be no unit at all.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Top_Level_Statements_Are_One_Relevant_Unit()
    {
        const string Program =
            "using System.IO;\n" +
            "\n" +
            "string target = args[0];\n" +
            "Wipe(target);\n" +
            "\n" +
            "static void Wipe(string path)\n" +
            "{\n" +
            "    File.Delete(path);\n" +
            "}\n";

        AssuranceScannedUnit unit = Assert.Single(Units(Program));
        Assert.Equal(AssuranceVocabulary.TopLevelStatements, unit.Name);
        Assert.False(unit.IsExempt);
        Assert.Equal("top-level statements", unit.Kind);
        Assert.Equal(2, unit.DeclarationLine);
        Assert.Equal(8, unit.EndLine);

        string edited = Program.Replace("File.Delete(path);", "File.Delete(path + \".bak\");", StringComparison.Ordinal);
        Assert.NotEqual(unit.Fingerprint, Assert.Single(Units(edited)).Fingerprint);

        AssuranceFileUnit scanned = Assert.Single(new CSharpAssuranceFileScanner().ScanFile(Program, "x.cs").Units);
        Assert.True(scanned.StartsOwnLine);
        Assert.Equal(2, scanned.HeaderEndLine);
    }

    /// <summary>
    /// The strict predicate exempts storage and trivial members, not code: an
    /// initializer that calls or holds a lambda, a throw that computes its
    /// message, and anything inside a type named <c>AssemblyMarker</c> are
    /// relevant under it. The owning component's predicate is unchanged.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Strict_Predicate_Does_Not_Exempt_Code_That_Runs()
    {
        const string Text =
            "namespace N;\n" +
            "public sealed class Gadget\n" +
            "{\n" +
            "    private readonly int _seed = Wipe(\"C:/x\");\n" +
            "    private static System.Func<int> s_hook = () => { System.IO.File.Delete(\"C:/y\"); return 0; };\n" +
            "    public int Auto { get; } = Wipe(\"C:/z\");\n" +
            "    public int Throws => throw new System.InvalidOperationException(Describe());\n" +
            "    private int _count = 0;\n" +
            "    private readonly System.Collections.Generic.List<int> _items = new();\n" +
            "    private System.DateTimeKind _kind = System.DateTimeKind.Utc | System.DateTimeKind.Local;\n" +
            "    public int Plain { get; set; } = -1;\n" +
            "    public int Fails => throw new System.InvalidOperationException(\"no\");\n" +
            "    public static class AssemblyMarker\n" +
            "    {\n" +
            "        public static int Wipe(string p) { System.IO.File.Delete(p); return 0; }\n" +
            "    }\n" +
            "    private static int Wipe(string p) { System.IO.File.Delete(p); return 0; }\n" +
            "    private static string Describe() => System.IO.File.ReadAllText(\"C:/secret\");\n" +
            "}\n";

        string[] relevant = ["N.Gadget._seed", "N.Gadget.s_hook", "N.Gadget.Auto", "N.Gadget.Throws", "N.Gadget.AssemblyMarker.Wipe(string)"];
        string[] stillExempt = ["N.Gadget._count", "N.Gadget._items", "N.Gadget._kind", "N.Gadget.Plain", "N.Gadget.Fails"];

        foreach (string name in relevant)
        {
            Assert.True(Unit(Text, name).IsExempt, $"{name} is exempt under the owning component's predicate");
            Assert.False(Unit(Text, name, AssuranceExemptionPredicate.Strict).IsExempt, $"{name} is relevant strictly");
        }

        foreach (string name in stillExempt)
        {
            AssuranceScannedUnit owning = Unit(Text, name);
            AssuranceScannedUnit strict = Unit(Text, name, AssuranceExemptionPredicate.Strict);
            Assert.True(strict.IsExempt, $"{name} is exempt strictly");
            Assert.Equal(owning.Exemption, strict.Exemption);
        }
    }

    /// <summary>
    /// Every comment line is reported, whatever delimits it — the lines a
    /// forged summary can be written in — and a string that looks like one is
    /// not a comment.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Every_Line_Of_Every_Comment_Is_Reported()
    {
        const string Text =
            "namespace N;\n" +
            "/* Broiler Code Assurance\n" +
            "   Human-reviewed: 3/3 */\n" +
            "/// <summary>Doc.</summary>\n" +
            "public class C // trailing\n" +
            "{\n" +
            "    const string S = \"// Annotated: 1/1\";\n" +
            "#if NEVER\n" +
            "    // Unverified: 0\n" +
            "#endif\n" +
            "}\n";

        AssuranceScannedFile file = new CSharpAssuranceFileScanner().ScanFile(Text, "x.cs");

        Assert.Equal(
            [
                (1, "/* Broiler Code Assurance"),
                (2, "   Human-reviewed: 3/3 */"),
                (3, "/// <summary>Doc.</summary>"),
                (4, "// trailing"),
                (8, "    // Unverified: 0"),
            ],
            file.CommentLines.Select(static line => (line.Line, line.Text)));
    }
}
