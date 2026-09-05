using Broiler.Code.Language.CSharp.Roslyn;
using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Language.CSharp.Tests;

/// <summary>
/// The assurance model with a language service composed, which is the level at
/// which the editor may rewrite a generated file header.
///
/// The claim under test is narrow and is the one the whole feature rests on:
/// this build counts a real annotated file exactly the way the component that
/// owns the format counted it, and it proves that by reproducing the header that
/// component wrote before it changes a byte.
/// </summary>
public sealed class AssuranceDocumentWithScannerTests
{
    private static AssuranceDocument Read(string text) =>
        AssuranceDocument.Read(text, new CSharpAssuranceScanner(), "VmArtifactDescriptor.cs");

    /// <summary>
    /// Every number in the file's own header, recomputed and compared one at a
    /// time so a failure names which of them moved.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Summary_Is_The_One_The_File_States()
    {
        AssuranceSummary summary = Read(AssuranceFixture.Descriptor).Summary!.Value;

        Assert.Equal(4, summary.Relevant);
        Assert.Equal(4, summary.Annotated);
        Assert.Equal(9, summary.Exempt);
        Assert.Equal(0, summary.Verified);
        Assert.Equal(4, summary.Unverified);
        Assert.Equal("Low", summary.MaxIpRisk);
        Assert.Equal("Low", summary.MaxSecurityRisk);
        Assert.Equal(0, summary.Criteria);
        Assert.Equal(0, summary.CriteriaRequired);
        Assert.Equal(3, summary.MaxResources);
    }

    /// <summary>
    /// The whole header, byte for byte.
    ///
    /// This is the licence to rewrite it. Sixteen lines have to come back
    /// identical — the padding of every label, the two bare comment markers, the
    /// rule under the banner — because the thing being demonstrated is not that
    /// the counts are right but that this build writes the same file that
    /// component would.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Header_Is_Reproduced_Exactly()
    {
        Assert.True(Read(AssuranceFixture.Descriptor).BannerIsReproducible);
    }

    /// <summary>
    /// Signing a pending unit changes one line and leaves every number in the
    /// header where it was — which is the right answer, not a missed update.
    ///
    /// A name with no fingerprint beside it is a human saying they stand behind
    /// the declaration and the generator not yet having sealed which version.
    /// That is not a reviewed unit, so <c>Human-reviewed</c> does not move and the
    /// unit still blocks a release. A header that counted it would be claiming
    /// something no artefact supports.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Signing_A_Pending_Unit_Moves_No_Count()
    {
        AssuranceDocument document = Read(AssuranceFixture.Descriptor);
        AssuranceUnit unit = document.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!;

        AssuranceEditResult result = document.Approve(unit, "EB");

        Assert.True(result.Succeeded);
        Assert.False(result.HeaderUpdated);

        Assert.Contains("    // Broiler-Human:        EB\n", result.Text, StringComparison.Ordinal);
        Assert.Contains("// Human-reviewed:   0/4", result.Text, StringComparison.Ordinal);
        Assert.Contains("// Unverified:       4", result.Text, StringComparison.Ordinal);
        Assert.Contains("// Relevant units:   4", result.Text, StringComparison.Ordinal);
        Assert.Contains("// Exempt:           9", result.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// Withdrawing a sealed approval does move the counts, and the header follows
    /// in the same edit.
    ///
    /// This is the case the recount exists for. A file left saying one of its four
    /// declarations is human-reviewed, immediately after the only person who said
    /// so took it back, contradicts its own annotations — and it is the generated
    /// half of the file, which nobody reads expecting to be out of date.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Withdrawing_A_Sealed_Approval_Recounts_The_Header()
    {
        Assert.Contains("// Human-reviewed:   1/4", Sealed(), StringComparison.Ordinal);

        AssuranceDocument document = Read(Sealed());
        AssuranceUnit unit = document.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!;
        Assert.True(document.BannerIsReproducible);

        AssuranceEditResult result = document.Withdraw(unit);

        Assert.True(result.Succeeded);
        Assert.True(result.HeaderUpdated);
        Assert.Contains("// Human-reviewed:   0/4", result.Text, StringComparison.Ordinal);
        Assert.Contains("// Unverified:       4", result.Text, StringComparison.Ordinal);

        // The rest of the header is untouched: withdrawing a review changes what
        // a human said, not what the code is.
        Assert.Contains("// Relevant units:   4", result.Text, StringComparison.Ordinal);
        Assert.Contains("// Exempt:           9", result.Text, StringComparison.Ordinal);
        Assert.Contains("// Resource impact:  3/10 max", result.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// Recounting the header rewrites its numbers and not its line endings.
    ///
    /// A file whose generated header and body disagree about how a line ends is
    /// exactly the shape this repository is documented as holding, and the
    /// obvious implementation — remove the block, insert the new one — gives all
    /// sixteen lines whichever ending happens to be left. The result is a
    /// whole-block diff to record one reviewer taking their name off one
    /// declaration.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Recounting_The_Header_Keeps_Each_Line_Ending_It_Found()
    {
        // Header CRLF, body LF: the mix the naive rewrite normalizes away.
        string mixed = Sealed().Replace("\r\n", "\n", StringComparison.Ordinal);
        int split = mixed.IndexOf(AssuranceBanner.GeneratedMarker, StringComparison.Ordinal) +
            AssuranceBanner.GeneratedMarker.Length;
        mixed = mixed[..split].Replace("\n", "\r\n", StringComparison.Ordinal) + mixed[split..];

        int carriageReturnsBefore = mixed.Count(character => character == '\r');
        Assert.Equal(15, carriageReturnsBefore);

        AssuranceDocument document = Read(mixed);
        Assert.True(document.BannerIsReproducible);

        AssuranceEditResult result = document.Withdraw(
            document.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!);

        Assert.True(result.HeaderUpdated);
        Assert.Contains("// Human-reviewed:   0/4", result.Text, StringComparison.Ordinal);

        // Every carriage return the file had, and not one more.
        Assert.Equal(carriageReturnsBefore, result.Text.Count(character => character == '\r'));
    }

    /// <summary>
    /// The fixture with one unit sealed the way the owning component's generator
    /// seals it — a reviewer's name and the fingerprint of what they approved —
    /// and its header already recounted to match.
    /// </summary>
    private static string Sealed() => AssuranceFixture.Descriptor
        .Replace(
            "    // Broiler-Human:        PENDING\n    public bool IsWellFormed",
            "    // Broiler-Human:        EB; Fingerprint=4A3BFD\n    public bool IsWellFormed",
            StringComparison.Ordinal)
        .Replace("// Human-reviewed:   0/4", "// Human-reviewed:   1/4", StringComparison.Ordinal)
        .Replace("// Unverified:       4", "// Unverified:       3", StringComparison.Ordinal);

    /// <summary>
    /// A unit whose human line names a reviewer and the version they approved is
    /// verified, and the header says so.
    ///
    /// This is the state the owning component's generator produces from the bare
    /// name this editor writes, so the fixture is edited to what that generator
    /// would have made of it. What is asserted is that this build then counts the
    /// result the same way.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Sealed_Approval_Counts_As_Reviewed()
    {
        string sealedOff = AssuranceFixture.Descriptor.Replace(
            """
                // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4A3BFD
                // Broiler-Human:        PENDING
            """,
            """
                // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4A3BFD
                // Broiler-Human:        EB; Fingerprint=4A3BFD
            """,
            StringComparison.Ordinal);

        AssuranceDocument document = Read(sealedOff);
        AssuranceUnit unit = document.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!;

        Assert.Equal(AssuranceUnitState.Verified, unit.State);
        Assert.Equal(1, document.Summary!.Value.Verified);
        Assert.Equal(3, document.Summary!.Value.Unverified);
    }

    /// <summary>
    /// Signing a unit that already records this reviewer's sealed approval is
    /// refused, rather than rebuilt from the name alone.
    ///
    /// This is the sharpest failure the feature could have had: rebuilding the
    /// line drops the <c>Fingerprint=</c> that records which version was
    /// approved, so pressing Sign on an already-reviewed declaration would
    /// withdraw the approval and recount the header downwards — while the status
    /// line said a signature had been recorded.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Signing_An_Already_Verified_Unit_Is_Refused()
    {
        AssuranceDocument document = Read(Sealed());
        AssuranceUnit unit = document.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!;
        Assert.Equal(AssuranceUnitState.Verified, unit.State);

        AssuranceEditResult result = document.Approve(unit, "EB");

        Assert.Equal(AssuranceEditOutcome.NothingToDo, result.Outcome);
        Assert.Equal(Sealed(), result.Text);
        Assert.Contains("// Human-reviewed:   1/4", result.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// Signing a unit whose approval has gone stale replaces the lapsed
    /// fingerprint with a bare name, which is what re-reviewing means: the
    /// generator then seals the version that is here now.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Signing_A_Stale_Unit_Clears_The_Lapsed_Fingerprint()
    {
        string moved = Sealed().Replace("FormatVersion >= 1", "FormatVersion >= 2", StringComparison.Ordinal);

        AssuranceDocument document = Read(moved);
        AssuranceUnit unit = document.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!;
        Assert.Equal(AssuranceUnitState.Stale, unit.State);

        AssuranceEditResult result = document.Approve(unit, "EB");

        Assert.True(result.Succeeded);
        Assert.Contains("    // Broiler-Human:        EB\n", result.Text, StringComparison.Ordinal);
        Assert.DoesNotContain("EB; Fingerprint=4A3BFD", result.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// Editing the code under a sealed approval makes it stale, and the pane says
    /// so from the fingerprint rather than from a commit.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Changing_The_Code_Under_An_Approval_Makes_It_Stale()
    {
        string sealedOff = AssuranceFixture.Descriptor
            .Replace(
                "    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4A3BFD\n" +
                "    // Broiler-Human:        PENDING",
                "    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4A3BFD\n" +
                "    // Broiler-Human:        EB; Fingerprint=4A3BFD",
                StringComparison.Ordinal)
            .Replace("FormatVersion >= 1", "FormatVersion >= 2", StringComparison.Ordinal);

        AssuranceUnit unit = Read(sealedOff).UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!;

        Assert.Equal(AssuranceUnitState.Stale, unit.State);
    }

    /// <summary>
    /// A header this build cannot reproduce is left alone, and the review is
    /// still recorded.
    ///
    /// The failure this guards against is the one worth guarding against: a
    /// header written from counts that disagree with the owning component's would
    /// look generated and be wrong, which is worse than a header that is merely
    /// out of date. The reviewer's own line is the part that must not be lost, so
    /// it is written either way.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Header_That_Does_Not_Reproduce_Is_Not_Rewritten()
    {
        string tampered = AssuranceFixture.Descriptor.Replace(
            "// Exempt:           9", "// Exempt:           7", StringComparison.Ordinal);

        AssuranceDocument document = Read(tampered);
        Assert.False(document.BannerIsReproducible);

        AssuranceEditResult result = document.Approve(
            document.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!, "EB");

        Assert.True(result.Succeeded);
        Assert.False(result.HeaderUpdated);
        Assert.Contains("// Exempt:           7", result.Text, StringComparison.Ordinal);
        Assert.Contains("    // Broiler-Human:        EB\n", result.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// An exempt unit is refused, because the assurance system expects no human
    /// line to move on one and a reviewer who signed nine of them would have
    /// recorded nine attestations the format does not count.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void An_Exempt_Unit_Cannot_Be_Signed()
    {
        AssuranceDocument document = Read(AssuranceFixture.Descriptor);
        AssuranceUnit exempt = document.UnitNamed("Broiler.VM.VmArtifactDescriptor.GetHashCode()")!;

        AssuranceEditResult result = document.Approve(exempt, "EB");

        Assert.Equal(AssuranceEditOutcome.Exempt, result.Outcome);
        Assert.Contains("exempt", result.Message, StringComparison.Ordinal);
        Assert.Equal(AssuranceFixture.Descriptor, result.Text);
    }

    /// <summary>
    /// With a scanner the caret lands on the innermost unit, so a click inside a
    /// method reports the method rather than the type that contains it.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Caret_Finds_The_Innermost_Unit()
    {
        AssuranceDocument document = Read(AssuranceFixture.Descriptor);
        AssuranceUnit method = document.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!;
        AssuranceUnit type = document.UnitNamed("Broiler.VM.VmArtifactDescriptor")!;

        Assert.Equal(method, document.UnitAt(method.DeclarationLine));
        Assert.Equal(type, document.UnitAt(type.DeclarationLine));
    }

    /// <summary>
    /// Generating twice changes nothing the second time.
    ///
    /// The owning component asserts the same property of its own generator, and
    /// for the same reason: a header derived from the file it is written into
    /// must not depend on itself, or no run ever settles. It holds here because
    /// no fingerprint sees a comment.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Recorded_Review_Is_Stable_Under_A_Second_Pass()
    {
        AssuranceDocument first = Read(AssuranceFixture.Descriptor);
        string once = first.Approve(
            first.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!, "EB").Text;

        AssuranceDocument second = Read(once);
        AssuranceEditResult again = second.Approve(
            second.UnitNamed("Broiler.VM.VmArtifactDescriptor.IsWellFormed")!, "EB");

        Assert.Equal(AssuranceEditOutcome.NothingToDo, again.Outcome);
        Assert.Equal(once, again.Text);
        Assert.True(second.BannerIsReproducible);
    }
}
