using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Review.Tests;

/// <summary>
/// The assurance model with no language service composed — the state every host
/// this shell is meant to run on will be in, and most of them permanently.
///
/// What it can do at this level is the whole of what a human writes: read the
/// annotation blocks that are already in the file, show what they record, and
/// set the one line a reviewer owns. What it cannot do is count the file's units
/// or compute a fingerprint, and the tests below are mostly about it declining
/// to pretend otherwise.
/// </summary>
public sealed class AssuranceDocumentTests
{
    private const string Source = """
// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           1
// Human-reviewed:   0/2
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  2/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Sample;

/// <summary>A thing.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AAAAAA
// Broiler-Human:        PENDING
public sealed class Thing
{
    /// <summary>Does the work.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=BBBBBB
    // Broiler-Falsified-If: the caller can observe a partially applied change
    // Broiler-Human:        PENDING
    public int Work(int value)
    {
        return value + 1;
    }
}
""";

    /// <summary>
    /// Finding the blocks needs no parser, because a comment is a comment. This
    /// is what makes the pane useful on a host that could never carry Roslyn.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Units_Are_The_Annotated_Declarations()
    {
        AssuranceDocument document = AssuranceDocument.Read(Source);

        Assert.Equal(2, document.Units.Count);
        Assert.False(document.HasUnitScanner);
        Assert.True(document.HasBanner);
        Assert.Contains("class Thing", document.Units[0].DisplayName, StringComparison.Ordinal);
        Assert.Contains("Work", document.Units[1].DisplayName, StringComparison.Ordinal);
    }

    /// <summary>
    /// Every field of the machine's line reaches the pane, including the
    /// criterion, which is the sentence a reviewer of a high-security unit is
    /// meant to argue with.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void An_Annotation_Reports_What_It_States()
    {
        AssuranceAnnotation annotation = AssuranceDocument.Read(Source).Units[1].Annotation!;

        Assert.Equal("AI", annotation.Field("Origin"));
        Assert.Equal("High", annotation.Field("Security"));
        Assert.Equal("2", annotation.Field("Resources"));
        Assert.Equal("BBBBBB", annotation.RecordedFingerprint);
        Assert.Equal("the caller can observe a partially applied change", annotation.Criterion);
        Assert.True(annotation.HumanIsPending);
        Assert.Null(annotation.Reviewer);
    }

    /// <summary>
    /// A caret anywhere in a member reports that member, and the member's own
    /// annotation lines count as inside it — a reviewer clicking the line they
    /// are about to sign is looking at the right unit.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Caret_Finds_The_Unit_It_Is_In()
    {
        AssuranceDocument document = AssuranceDocument.Read(Source);
        AssuranceUnit method = document.Units[1];

        Assert.Same(method, document.UnitAt(method.DeclarationLine));
        Assert.Same(method, document.UnitAt(method.Annotation!.HumanLine));
        Assert.Same(method, document.UnitAt(method.DeclarationLine + 2));
        Assert.Null(document.UnitAt(0));
    }

    /// <summary>
    /// Recording a review writes the reviewer's name and nothing else.
    ///
    /// The fingerprint is left for the owning component's generator, which is
    /// the shape that component expects from a person and the reason this editor
    /// cannot manufacture a completed approval. Asserted on the exact bytes,
    /// because the column the name lands in is part of the format.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Approving_Writes_The_Reviewer_And_No_Fingerprint()
    {
        AssuranceDocument document = AssuranceDocument.Read(Source);
        AssuranceEditResult result = document.Approve(document.Units[1], "EB");

        Assert.True(result.Succeeded);
        Assert.Contains("    // Broiler-Human:        EB\n", result.Text, StringComparison.Ordinal);
        Assert.DoesNotContain("EB; Fingerprint", result.Text, StringComparison.Ordinal);

        // The other unit is untouched, and so is every other line in the file.
        Assert.Contains("// Broiler-Human:        PENDING", result.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// Only the one line changes. A review that reformatted the file would
    /// invalidate every other reviewer's content hash on the way past, and would
    /// bury the one line that matters in a whole-file diff.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Approving_Changes_One_Line_And_No_Other()
    {
        AssuranceDocument document = AssuranceDocument.Read(Source);
        string after = document.Approve(document.Units[1], "EB").Text;

        string[] before = Source.Split('\n');
        string[] now = after.Split('\n');

        Assert.Equal(before.Length, now.Length);
        Assert.Single(before.Where((line, index) => !string.Equals(line, now[index], StringComparison.Ordinal)));
    }

    /// <summary>
    /// A file that uses CRLF gets its line back with CRLF, and every untouched
    /// line keeps whatever it had. A rewrite that normalized line endings would
    /// be a whole-file change dressed as a one-line one.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Rewrite_Keeps_The_Line_Endings_It_Found()
    {
        string crlf = Source.Replace("\n", "\r\n", StringComparison.Ordinal);
        AssuranceDocument document = AssuranceDocument.Read(crlf);

        string after = document.Approve(document.Units[1], "EB").Text;

        Assert.Contains("    // Broiler-Human:        EB\r\n", after, StringComparison.Ordinal);

        // Every ending is still CRLF: no line feed survives without its carriage
        // return, which is the only way a rewrite could have normalized one.
        Assert.Equal(crlf.Length, after.Length + "PENDING".Length - "EB".Length);
        Assert.DoesNotContain('\n', after.Replace("\r\n", string.Empty, StringComparison.Ordinal));
    }

    /// <summary>
    /// Withdrawing puts the reserved word back, which is the way out of an
    /// approval recorded on the wrong declaration.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Withdrawing_Restores_Pending()
    {
        AssuranceDocument approved = AssuranceDocument.Read(
            AssuranceDocument.Read(Source).Approve(AssuranceDocument.Read(Source).Units[1], "EB").Text);

        AssuranceEditResult result = approved.Withdraw(approved.Units[1]);

        Assert.True(result.Succeeded);
        Assert.Equal(2, Occurrences(result.Text, "// Broiler-Human:        PENDING"));
    }

    /// <summary>
    /// A name with no reviewer behind it is refused, in the model rather than
    /// only in the shell — the same rule, and for the same reason, as
    /// <see cref="FileReview.WithDecision"/> refusing an unnamed decision.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void An_Approval_Without_A_Name_Is_Refused()
    {
        AssuranceDocument document = AssuranceDocument.Read(Source);

        AssuranceEditResult result = document.Approve(document.Units[1], "   ");

        Assert.Equal(AssuranceEditOutcome.NoReviewer, result.Outcome);
        Assert.Equal(Source, result.Text);
        Assert.Contains("not evidence", result.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// A name carrying either of the format's delimiters is refused rather than
    /// written and mangled.
    ///
    /// The body is split on <c>;</c>, staleness is later recorded as
    /// <c>Previous=name@fingerprint</c>, and a first part containing <c>=</c> is
    /// read as a field rather than as a reviewer — so a name carrying any of the
    /// three comes back as something else, or as a body the owning component's
    /// generator refuses to touch. Refusing here is the only answer that does not
    /// quietly corrupt somebody's attestation.
    /// </summary>
    [Theory]
    [InlineData("Enrico; Bartky")]
    [InlineData("enrico@example.com")]
    [InlineData("name=value")]
    [InlineData("PENDING")]
    [InlineData("STALE")]
    public void A_Name_The_Format_Cannot_Carry_Is_Refused(string reviewer)
    {
        AssuranceDocument document = AssuranceDocument.Read(Source);

        AssuranceEditResult result = document.Approve(document.Units[1], reviewer);

        Assert.Equal(AssuranceEditOutcome.NoReviewer, result.Outcome);
        Assert.Equal(Source, result.Text);
    }

    /// <summary>
    /// A name that is merely unusual is written. There is no roster of permitted
    /// reviewers in the format and this refuses nobody on the grounds of who they
    /// are — only on the grounds that the line would not survive the round trip.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void An_Ordinary_Name_With_A_Space_Is_Written()
    {
        AssuranceDocument document = AssuranceDocument.Read(Source);

        AssuranceEditResult result = document.Approve(document.Units[1], "Enrico Bartky");

        Assert.True(result.Succeeded);
        Assert.Contains("// Broiler-Human:        Enrico Bartky", result.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// Signing the same unit twice is refused rather than rewritten. A commit
    /// whose only content is a line being replaced by itself asks a reviewer to
    /// read a diff with nothing in it.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Recording_The_Same_Decision_Twice_Is_Refused()
    {
        AssuranceDocument document = AssuranceDocument.Read(
            AssuranceDocument.Read(Source).Approve(AssuranceDocument.Read(Source).Units[1], "EB").Text);

        AssuranceEditResult result = document.Approve(document.Units[1], "EB");

        Assert.Equal(AssuranceEditOutcome.NothingToDo, result.Outcome);
    }

    /// <summary>
    /// A reviewer re-signing their own line keeps the assessment they recorded
    /// beside their name; a different reviewer's is dropped, because it was their
    /// reading and not this one's.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Reviewers_Own_Assessment_Survives_Their_Re_Signature()
    {
        string assessed = Source.Replace(
            "    // Broiler-Human:        PENDING",
            "    // Broiler-Human:        EB; Security=Critical; Fingerprint=BBBBBB",
            StringComparison.Ordinal);

        AssuranceDocument document = AssuranceDocument.Read(assessed);
        AssuranceUnit unit = document.Units[1];
        AssuranceAnnotation annotation = unit.Annotation!;

        Assert.Equal("EB", annotation.Reviewer);
        Assert.Equal("BBBBBB", annotation.HumanFingerprint);
        Assert.Equal(["Security=Critical"], annotation.HumanAssessment);

        // With no scanner the freshness of that fingerprint is unknown, so the
        // same reviewer signing again leaves the line exactly as it is rather
        // than rebuilding it — rebuilding would drop the very field that records
        // which version they approved.
        Assert.Equal("EB; Security=Critical; Fingerprint=BBBBBB", AssuranceDocument.ApprovalBody(unit, "EB"));

        // A different reviewer's signature replaces the line outright: the
        // assessment was somebody else's reading, and so was the approval.
        Assert.Equal("RV", AssuranceDocument.ApprovalBody(unit, "RV"));
    }

    /// <summary>
    /// Signing a unit whose own approval is already recorded is refused rather
    /// than rewritten.
    ///
    /// The failure this guards against is the sharp one: rebuilding the line from
    /// the reviewer's name alone would delete the <c>Fingerprint=</c> that records
    /// which version they approved, so pressing the command twice would withdraw
    /// the approval it had just recorded.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Signing_A_Unit_That_Already_Records_This_Reviewer_Changes_Nothing()
    {
        string assessed = Source.Replace(
            "    // Broiler-Human:        PENDING",
            "    // Broiler-Human:        EB; Fingerprint=BBBBBB",
            StringComparison.Ordinal);

        AssuranceDocument document = AssuranceDocument.Read(assessed);

        AssuranceEditResult result = document.Approve(document.Units[1], "EB");

        Assert.Equal(AssuranceEditOutcome.NothingToDo, result.Outcome);
        Assert.Equal(assessed, result.Text);
    }

    /// <summary>
    /// Without a scanner the file header is never rewritten, and the model says
    /// so rather than writing a header from counts it cannot take.
    ///
    /// This is the safety property that makes the two levels of confidence safe
    /// to ship together: the licence to rewrite the header is the ability to
    /// reproduce it, and a build that cannot compute a fingerprint cannot count
    /// verified units, so it never gets that licence.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void Without_A_Scanner_The_Header_Is_Left_Alone()
    {
        AssuranceDocument document = AssuranceDocument.Read(Source);

        Assert.Null(document.Summary);
        Assert.False(document.BannerIsReproducible);

        AssuranceEditResult result = document.Approve(document.Units[1], "EB");

        Assert.True(result.Succeeded);
        Assert.False(result.HeaderUpdated);
        Assert.Contains("// Human-reviewed:   0/2", result.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// A file with no annotation in it is not an error and not an empty pane's
    /// fault — it is most files. The model reports nothing to show and refuses
    /// nothing loudly.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_File_With_No_Annotations_Reports_Nothing()
    {
        AssuranceDocument document = AssuranceDocument.Read("namespace Sample;\n\npublic sealed class Plain\n{\n}\n");

        Assert.Empty(document.Units);
        Assert.False(document.HasBanner);
        Assert.False(document.IsAnnotated);
        Assert.Null(document.UnitAt(2));
    }

    /// <summary>
    /// A block whose human line is missing is not an annotation. Treating it as
    /// one would offer a reviewer somewhere to write that the file has no room
    /// for, and the write would land on whatever line followed.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Block_Without_A_Human_Line_Is_Not_An_Annotation()
    {
        AssuranceDocument document = AssuranceDocument.Read(
            "// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AAAAAA\n" +
            "public sealed class Thing\n{\n}\n");

        Assert.Empty(document.Units);
    }

    /// <summary>
    /// The block is still this declaration's when a doc comment, an attribute, a
    /// block comment or a preprocessor line sits between them.
    ///
    /// The component that owns the format reads the block out of the
    /// declaration's leading trivia, which holds all four. This build has no
    /// parser and scans lines instead, so every one of them has to be skipped
    /// explicitly — a <c>#region</c> in the wrong place would otherwise report an
    /// approved declaration as never assessed.
    /// </summary>
    [Theory]
    [InlineData("    /// <summary>Doc.</summary>")]
    [InlineData("    [System.Obsolete]")]
    [InlineData("    /* a block comment */")]
    [InlineData("    #region Work")]
    [InlineData("")]
    public void The_Block_Survives_What_Can_Sit_Between_It_And_The_Declaration(string between)
    {
        const string Anchor = "    // Broiler-Human:        PENDING\n    public int Work(int value)";
        string source = Source.Replace(
            Anchor,
            "    // Broiler-Human:        PENDING\n" + between + "\n    public int Work(int value)",
            StringComparison.Ordinal);

        Assert.NotEqual(Source, source);

        AssuranceDocument document = AssuranceDocument.Read(source);

        Assert.Equal(2, document.Units.Count);
        Assert.NotNull(document.Units[1].Annotation);
    }

    private static int Occurrences(string text, string value)
    {
        int count = 0;
        int index = text.IndexOf(value, StringComparison.Ordinal);
        while (index >= 0)
        {
            count++;
            index = text.IndexOf(value, index + value.Length, StringComparison.Ordinal);
        }

        return count;
    }
}
