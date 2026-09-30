using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Review.Tests;

/// <summary>
/// The strict block parse and the vocabulary checks, against the owning
/// component's own answers.
///
/// A tool deciding whether a unit is annotated has to give that component's
/// answer, not the editor's lenient one. Every problem string asserted here is
/// that component's, word for word, because a tool reporting the same fault in
/// other words leaves a reader wondering whether it is the same fault.
/// </summary>
public sealed class AssuranceRulesTests
{
    private static AssuranceAnnotation? Parse(string text, out string? problem)
    {
        AssuranceAnnotation.TryParseStrict(new AssuranceLines(text), 0, out AssuranceAnnotation? annotation, out problem);
        return annotation;
    }

    [Fact(Timeout = 600000)]
    public void Keys_And_Values_Are_Trimmed_Around_The_Equals_Sign()
    {
        AssuranceAnnotation annotation = Parse(
            "// Broiler-AI: Origin = AI ;IP=Low; Security=Low; Resources=3; Fingerprint=TBF\n// Broiler-Human: PENDING",
            out _)!;

        Assert.Equal("AI", annotation.Field("Origin"));
        Assert.Equal("Low", annotation.Field("IP"));
        Assert.Empty(AssuranceRules.VocabularyProblems(annotation));
    }

    [Theory(Timeout = 600000)]
    [InlineData("// Broiler-AI: Origin=AI; Low\n// Broiler-Human: PENDING", "line 1 has a field with no '=': 'Low'")]
    [InlineData("// Broiler-AI: ;\n// Broiler-Human: PENDING", "line 1 carries no fields")]
    [InlineData("// Broiler-AI: Origin=AI", "line 1 has no '// Broiler-Human:' line under it")]
    [InlineData("// Broiler-AI: Origin=AI\n\n// Broiler-Human: PENDING", "line 1 is not immediately followed by a '// Broiler-Human:' line")]
    [InlineData(
        "// Broiler-AI: Origin=AI\n// Broiler-Falsified-If: a\n// Broiler-Falsified-If: b\n// Broiler-Human: PENDING",
        "line 3 carries a second '// Broiler-Falsified-If:' line, and a falsification criterion is one line")]
    public void A_Block_The_Owning_Component_Refuses_Is_Refused_With_Its_Reason(string text, string expected)
    {
        Assert.Null(Parse(text, out string? problem));
        Assert.Equal(expected, problem);
    }

    /// <summary>
    /// An empty criterion line still counts as a criterion line, as it does in
    /// the header's Criteria row there, and is reported as saying nothing.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void An_Empty_Criterion_Line_Is_A_Line_That_Says_Nothing()
    {
        AssuranceAnnotation annotation = Parse(
            "// Broiler-AI: Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=TBF\n" +
            "// Broiler-Falsified-If:\n" +
            "// Broiler-Human: PENDING",
            out _)!;

        Assert.True(annotation.HasCriterionLine);
        Assert.False(annotation.HasCriterion);
        Assert.Equal(["// Broiler-Falsified-If: carries no criterion"], AssuranceRules.VocabularyProblems(annotation));
    }

    [Fact(Timeout = 600000)]
    public void Every_Field_Is_Held_To_Its_Vocabulary()
    {
        AssuranceAnnotation annotation = Parse(
            "// Broiler-AI: Origin=Human; IP=Some; Security=Severe; Resources=11; Fingerprint=abc123; Spec=; Extra=1\n" +
            "// Broiler-Human: PENDING",
            out _)!;

        Assert.Equal(
            [
                "Origin=Human is outside its vocabulary (Original, AI, Specification, Derived, Ported, ThirdParty)",
                "IP=Some is outside its vocabulary (None, Low, Medium, High, Unknown)",
                "Security=Severe is outside its vocabulary (None, Low, Medium, High, Critical)",
                "Resources=11 is not an integer 0 to 10",
                "Fingerprint=abc123 is neither TBF nor six uppercase hex characters",
                "Spec= is empty",
                "Extra=1 is not a field this system defines",
            ],
            AssuranceRules.VocabularyProblems(annotation));
    }

    [Fact(Timeout = 600000)]
    public void A_Missing_Required_Field_Is_Named()
    {
        AssuranceAnnotation annotation = Parse("// Broiler-AI: Origin=AI\n// Broiler-Human: PENDING", out _)!;

        Assert.Equal(
            ["no IP field", "no Security field", "no Resources field", "no Fingerprint field"],
            AssuranceRules.VocabularyProblems(annotation));
    }

    [Fact(Timeout = 600000)]
    public void An_Exemption_Is_Its_Own_Field_And_Carries_A_Reason()
    {
        Assert.Equal(
            ["EXEMPT carries no reason"],
            AssuranceRules.VocabularyProblems(Parse("// Broiler-AI: EXEMPT=\n// Broiler-Human: PENDING", out _)!));
        Assert.Equal(
            ["EXEMPT is stated beside other fields; an exemption is not an assessment"],
            AssuranceRules.VocabularyProblems(Parse("// Broiler-AI: EXEMPT=x; IP=Low\n// Broiler-Human: PENDING", out _)!));
    }

    /// <summary>
    /// A criterion is prose. Comparisons written with ==, !=, &lt;= and &gt;=
    /// are prose too; an identifier followed by one = and a value is a field.
    /// </summary>
    [Theory(Timeout = 600000)]
    [InlineData("the count == 0 after a reset, or x != y, or a <= b")]
    [InlineData("a negative value reaches the running total")]
    public void Prose_Is_A_Criterion(string criterion) =>
        Assert.Empty(AssuranceRules.CriterionProblems(criterion));

    [Fact(Timeout = 600000)]
    public void A_Field_On_A_Criterion_Is_Data_And_Refused()
    {
        Assert.Equal(
            ["// Broiler-Falsified-If: states the field Security=Low, and a falsification criterion is prose, not data"],
            AssuranceRules.CriterionProblems("it is really Security=Low"));
    }

    /// <summary>
    /// Review words are matched as substrings, so a criterion cannot say that
    /// anyone looked, however it is spelled.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void A_Criterion_Never_Claims_A_Review()
    {
        Assert.Equal(
            [
                "// Broiler-Falsified-If: claims a review by saying 'verified', and a falsification criterion states what would make the unit wrong, never that anyone read it",
                "// Broiler-Falsified-If: claims a review by saying 'signed off', and a falsification criterion states what would make the unit wrong, never that anyone read it",
            ],
            AssuranceRules.CriterionProblems("Unverified input is returned; this was Signed Off"));
    }

    [Theory(Timeout = 600000)]
    [InlineData("0", true)]
    [InlineData("10", true)]
    [InlineData("+5", true)]
    [InlineData("11", false)]
    [InlineData("-1", false)]
    [InlineData("five", false)]
    public void Resources_Is_An_Integer_From_Zero_To_Ten(string value, bool valid) =>
        Assert.Equal(valid, AssuranceRules.TryParseResources(value, out _));

    [Fact(Timeout = 600000)]
    public void A_Stale_Line_Keeps_The_Previous_Reviewer()
    {
        AssuranceAnnotation annotation = Parse(
            "// Broiler-AI: Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=06FA02\n" +
            "// Broiler-Human: STALE; Previous=EB@112233",
            out _)!;

        Assert.Equal(("EB", "112233"), annotation.Previous);
    }
}
