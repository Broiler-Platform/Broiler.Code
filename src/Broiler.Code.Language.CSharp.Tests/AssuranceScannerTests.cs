using Broiler.Code.Language.CSharp.Roslyn;
using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Language.CSharp.Tests;

/// <summary>
/// The assurance scanner against values the component that owns the format has
/// already published.
///
/// This is the test that matters most in the feature, because the scanner is a
/// second implementation of an algorithm defined elsewhere. Agreement cannot be
/// asserted by construction — the first implementation lives in another
/// repository's test assembly and cannot be referenced — so it is asserted
/// against evidence instead: the fixture below is a file from
/// <c>Broiler.VM</c>, and every fingerprint asserted here is the value that
/// component's own generator wrote into that file and into its
/// <c>assurance.manifest.json</c>.
///
/// A failure here does not mean the numbers moved. It means this editor and that
/// component have stopped agreeing about what a unit is or what its fingerprint
/// covers, and the editor is then the one that is wrong — which is why the
/// expected values are written out as literals rather than computed.
/// </summary>
public sealed class AssuranceScannerTests
{


    /// <summary>
    /// Every fingerprint the fixture states, recomputed.
    ///
    /// The four annotated units carry their value in the source, so this asserts
    /// that this scanner reproduces what the owning component's generator wrote —
    /// which is the whole claim the scanner makes.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Scanner_Reproduces_The_Fingerprints_In_The_Source()
    {
        IReadOnlyList<AssuranceScannedUnit> units = new CSharpAssuranceScanner()
            .Scan(AssuranceFixture.Descriptor, "VmArtifactDescriptor.cs");

        Assert.Equal("06FA02", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor"));
        Assert.Equal("4A3BFD", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.IsWellFormed"));
        Assert.Equal(
            "90F94D",
            Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.Equals(VmArtifactDescriptor)"));
        Assert.Equal(
            "A85631",
            Fingerprint(
                units,
                "Broiler.VM.VmArtifactDescriptor.operator !=(VmArtifactDescriptor, VmArtifactDescriptor)"));
    }

    /// <summary>
    /// The nine exempt units, whose fingerprints the source does not state
    /// because they carry no annotation.
    ///
    /// Their values come from that component's <c>assurance.manifest.json</c>,
    /// which records one row per unit whether it is exempt or not. They are
    /// asserted because an exempt unit still has a fingerprint, and a scanner
    /// that only agreed about the annotated ones would be agreeing about a third
    /// of the file.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Scanner_Reproduces_The_Fingerprints_The_Manifest_Records()
    {
        IReadOnlyList<AssuranceScannedUnit> units = new CSharpAssuranceScanner()
            .Scan(AssuranceFixture.Descriptor, "VmArtifactDescriptor.cs");

        Assert.Equal(
            "29A93E",
            Fingerprint(
                units,
                "Broiler.VM.VmArtifactDescriptor.VmArtifactDescriptor(VmProfileId, uint, " +
                "VmFeatureManifestId, VmLimitVector, VmCallerIdentity)"));
        Assert.Equal("CCA6CF", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.ProfileId"));
        Assert.Equal("551767", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.FormatVersion"));
        Assert.Equal("2545D0", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.FeatureManifestId"));
        Assert.Equal("0F125A", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.RequestedLimits"));
        Assert.Equal("D3C23E", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.CallerIdentity"));
        Assert.Equal("230EED", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.Equals(object?)"));
        Assert.Equal("464772", Fingerprint(units, "Broiler.VM.VmArtifactDescriptor.GetHashCode()"));
        Assert.Equal(
            "E46DBD",
            Fingerprint(
                units,
                "Broiler.VM.VmArtifactDescriptor.operator ==(VmArtifactDescriptor, VmArtifactDescriptor)"));
    }

    /// <summary>
    /// A fingerprint covers a declaration's tokens and no trivia at all.
    ///
    /// This is the property the whole scheme rests on: the annotation sits above
    /// the declaration it describes, so if it reached the value, writing a
    /// reviewer's name would change the fingerprint that names what they
    /// reviewed and no generation could ever settle. Asserted by rewriting every
    /// comment in the file and finding every fingerprint unmoved.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Rewriting_Every_Comment_Moves_No_Fingerprint()
    {
        var scanner = new CSharpAssuranceScanner();

        string reviewed = AssuranceFixture.Descriptor
            .Replace("// Broiler-Human:        PENDING", "// Broiler-Human:        EB", StringComparison.Ordinal)
            .Replace("/// <inheritdoc/>", "/// <summary>Something else entirely.</summary>", StringComparison.Ordinal)
            .Replace("// Human-reviewed:   0/4", "// Human-reviewed:   4/4", StringComparison.Ordinal);

        IReadOnlyList<AssuranceScannedUnit> before = scanner.Scan(AssuranceFixture.Descriptor, "x.cs");
        IReadOnlyList<AssuranceScannedUnit> after = scanner.Scan(reviewed, "x.cs");

        Assert.Equal(
            before.Select(unit => (unit.Name, unit.Fingerprint)),
            after.Select(unit => (unit.Name, unit.Fingerprint)));
    }

    /// <summary>
    /// Line endings move no fingerprint either, which is what lets a review
    /// survive a file being rewritten by a tool that normalizes them.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Line_Endings_Move_No_Fingerprint()
    {
        var scanner = new CSharpAssuranceScanner();
        string lf = AssuranceFixture.Descriptor.Replace("\r\n", "\n", StringComparison.Ordinal);
        string crlf = lf.Replace("\n", "\r\n", StringComparison.Ordinal);

        Assert.Equal(
            scanner.Scan(lf, "x.cs").Select(unit => unit.Fingerprint),
            scanner.Scan(crlf, "x.cs").Select(unit => unit.Fingerprint));
    }

    /// <summary>
    /// The exemption predicate, against the count the file's own generated
    /// header states: thirteen units, four relevant and nine exempt.
    ///
    /// The header is evidence rather than an assumption — that component wrote
    /// it — so this asserts the predicate against a number somebody else
    /// computed. Which units land on which side is asserted too, because two
    /// mistakes that cancel would leave the totals right.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Predicate_Agrees_With_The_Generated_Header()
    {
        IReadOnlyList<AssuranceScannedUnit> units = new CSharpAssuranceScanner()
            .Scan(AssuranceFixture.Descriptor, "VmArtifactDescriptor.cs");

        Assert.Equal(13, units.Count);
        Assert.Equal(4, units.Count(unit => !unit.IsExempt));
        Assert.Equal(9, units.Count(unit => unit.IsExempt));

        Assert.Equal(
            [
                "Broiler.VM.VmArtifactDescriptor",
                "Broiler.VM.VmArtifactDescriptor.Equals(VmArtifactDescriptor)",
                "Broiler.VM.VmArtifactDescriptor.IsWellFormed",
                "Broiler.VM.VmArtifactDescriptor.operator !=(VmArtifactDescriptor, VmArtifactDescriptor)",
            ],
            units.Where(unit => !unit.IsExempt).Select(unit => unit.Name).OrderBy(name => name, StringComparer.Ordinal));
    }

    /// <summary>
    /// The reason a unit is exempt, not only that it is.
    ///
    /// The reason reaches a reviewer, so a plausible wrong one is worse than
    /// none: "this is an auto-property" and "the compiler writes this" are
    /// different statements about the same declaration and only one of them is
    /// true.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void An_Exemption_Reports_The_Case_That_Covered_It()
    {
        IReadOnlyList<AssuranceScannedUnit> units = new CSharpAssuranceScanner()
            .Scan(AssuranceFixture.Descriptor, "VmArtifactDescriptor.cs");

        Assert.Equal(
            "TrivialPropertyOrAccessor",
            Exemption(units, "Broiler.VM.VmArtifactDescriptor.ProfileId"));
        Assert.Equal(
            "ParameterAssigningConstructor",
            Exemption(
                units,
                "Broiler.VM.VmArtifactDescriptor.VmArtifactDescriptor(VmProfileId, uint, " +
                "VmFeatureManifestId, VmLimitVector, VmCallerIdentity)"));
        Assert.Equal(
            "DelegatingOverrideOrOperator",
            Exemption(units, "Broiler.VM.VmArtifactDescriptor.Equals(object?)"));
        Assert.Equal(
            "DelegatingOverrideOrOperator",
            Exemption(
                units,
                "Broiler.VM.VmArtifactDescriptor.operator ==(VmArtifactDescriptor, VmArtifactDescriptor)"));
    }

    /// <summary>
    /// Inequality is relevant and equality is exempt, which looks like an
    /// inconsistency and is the point.
    ///
    /// <c>left.Equals(right)</c> hands the question on unchanged;
    /// <c>!left.Equals(right)</c> answers the opposite one, and which way round
    /// an inequality operator is is the whole of what it says. The owning
    /// component records that a round of adversarial review found every
    /// <c>operator !=</c> exempt and the negation therefore unreviewable.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Negation_Is_Not_Delegation()
    {
        IReadOnlyList<AssuranceScannedUnit> units = new CSharpAssuranceScanner()
            .Scan(AssuranceFixture.Descriptor, "VmArtifactDescriptor.cs");

        Assert.True(Unit(units, "Broiler.VM.VmArtifactDescriptor.operator ==(VmArtifactDescriptor, VmArtifactDescriptor)").IsExempt);
        Assert.False(Unit(units, "Broiler.VM.VmArtifactDescriptor.operator !=(VmArtifactDescriptor, VmArtifactDescriptor)").IsExempt);
    }

    /// <summary>
    /// A type declaration's fingerprint covers its header only, so editing a
    /// member does not move the type's own value.
    ///
    /// Without it every edit anywhere in a class would expire the review of the
    /// class, and a reviewer's approval of a type header would be worth nothing
    /// on any type anybody was still working on.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Type_Is_Fingerprinted_By_Its_Header_Alone()
    {
        var scanner = new CSharpAssuranceScanner();
        string edited = AssuranceFixture.Descriptor.Replace("FormatVersion >= 1", "FormatVersion >= 2", StringComparison.Ordinal);

        IReadOnlyList<AssuranceScannedUnit> after = scanner.Scan(edited, "x.cs");

        Assert.Equal("06FA02", Fingerprint(after, "Broiler.VM.VmArtifactDescriptor"));
        Assert.NotEqual("4A3BFD", Fingerprint(after, "Broiler.VM.VmArtifactDescriptor.IsWellFormed"));
    }

    /// <summary>
    /// The line span a unit reports is what puts the caret on the right unit, so
    /// it is asserted rather than assumed. The struct's span covers its members;
    /// a member's does not reach beyond itself.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Unit_Reports_The_Lines_It_Occupies()
    {
        IReadOnlyList<AssuranceScannedUnit> units = new CSharpAssuranceScanner()
            .Scan(AssuranceFixture.Descriptor, "VmArtifactDescriptor.cs");

        AssuranceScannedUnit type = Unit(units, "Broiler.VM.VmArtifactDescriptor");
        AssuranceScannedUnit member = Unit(units, "Broiler.VM.VmArtifactDescriptor.IsWellFormed");

        Assert.True(type.DeclarationLine < member.DeclarationLine);
        Assert.True(type.EndLine > member.EndLine);
        Assert.True(member.EndLine >= member.DeclarationLine);
    }

    private static AssuranceScannedUnit Unit(IReadOnlyList<AssuranceScannedUnit> units, string name) =>
        units.Single(unit => unit.Name == name);

    private static string Fingerprint(IReadOnlyList<AssuranceScannedUnit> units, string name) =>
        Unit(units, name).Fingerprint;

    private static string Exemption(IReadOnlyList<AssuranceScannedUnit> units, string name) =>
        Unit(units, name).Exemption;
}
