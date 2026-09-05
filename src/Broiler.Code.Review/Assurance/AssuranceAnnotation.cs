using System;
using System.Collections.Generic;
using System.Text;

namespace Broiler.Code.Review.Assurance;

/// <summary>One <c>Key=Value</c> pair on the machine's assessment line.</summary>
public readonly record struct AssuranceField(string Key, string Value);

/// <summary>
/// The two or three comment lines that sit above an annotated declaration,
/// parsed.
///
/// The block is written as well as read, because writing it is the point: the
/// whole of what a human records about a unit is the body of the
/// <c>Broiler-Human</c> line, and an editor that could show that line but not
/// set it would leave the reviewer typing the format by hand — which is how a
/// line the owning component refuses gets committed.
///
/// Two fields spelled <c>Fingerprint</c> live in this block and they mean
/// opposite things. The one on the machine's line is maintained by the owning
/// component's generator and is the unit's current value after a generation. The
/// one on the human's line is the version a person approved, and comparing it
/// against the unit's value now is the entire definition of a stale review. They
/// are exposed separately and never folded together.
/// </summary>
public sealed class AssuranceAnnotation
{
    private AssuranceAnnotation(
        int aiLine,
        int? falsifiedIfLine,
        int humanLine,
        string indent,
        IReadOnlyList<AssuranceField> fields,
        string criterion,
        string humanBody)
    {
        AiLine = aiLine;
        FalsifiedIfLine = falsifiedIfLine;
        HumanLine = humanLine;
        Indent = indent;
        Fields = fields;
        Criterion = criterion;
        HumanBody = humanBody;
    }

    /// <summary>Zero-based index of the machine's line within the file's lines.</summary>
    public int AiLine { get; }

    /// <summary>Zero-based index of the falsification criterion, when the block carries one.</summary>
    public int? FalsifiedIfLine { get; }

    /// <summary>Zero-based index of the human's line.</summary>
    public int HumanLine { get; }

    /// <summary>The indent every line of the block is re-emitted at.</summary>
    public string Indent { get; }

    /// <summary>The machine's fields, in the order the source wrote them.</summary>
    public IReadOnlyList<AssuranceField> Fields { get; }

    /// <summary>The falsification criterion, or empty when the block carries none.</summary>
    public string Criterion { get; }

    /// <summary>Everything after the human marker, trimmed.</summary>
    public string HumanBody { get; }

    /// <summary>The first line of the block, for a caller measuring its extent.</summary>
    public int FirstLine => AiLine;

    /// <summary>The last line of the block.</summary>
    public int LastLine => HumanLine;

    /// <summary>True when the block carries a falsification criterion.</summary>
    public bool HasCriterion => Criterion.Length > 0;

    /// <summary>The fingerprint the generator last stamped onto the machine's line.</summary>
    public string? RecordedFingerprint => Field(AssuranceVocabulary.FingerprintField);

    /// <summary>The reason this unit is exempt in the source, when it says so outright.</summary>
    public string? ExemptReason => Field(AssuranceVocabulary.ExemptField);

    /// <summary>True when nobody has recorded anything.</summary>
    public bool HumanIsPending =>
        string.Equals(HumanBody, AssuranceVocabulary.Pending, StringComparison.Ordinal);

    /// <summary>
    /// True when the line already records that the code moved out from under a
    /// review. Matched by prefix, because the written form carries who approved
    /// what: <c>STALE; Previous=EB@06FA02</c>.
    /// </summary>
    public bool HumanIsStale =>
        HumanBody.StartsWith(AssuranceVocabulary.Stale, StringComparison.Ordinal);

    /// <summary>
    /// Who recorded the review, or null when nobody has.
    ///
    /// Null for both reserved bodies. A stale line names the previous reviewer
    /// inside <c>Previous=</c>, and that person is not the reviewer of the code
    /// as it stands now — reporting them here would credit an approval that has
    /// already lapsed.
    /// </summary>
    public string? Reviewer
    {
        get
        {
            if (HumanIsPending || HumanIsStale || HumanBody.Length == 0)
                return null;

            int semicolon = HumanBody.IndexOf(';', StringComparison.Ordinal);
            string name = (semicolon < 0 ? HumanBody : HumanBody[..semicolon]).Trim();
            return name.Length == 0 ? null : name;
        }
    }

    /// <summary>The version the reviewer approved, or null when they left it to the generator.</summary>
    public string? HumanFingerprint
    {
        get
        {
            foreach (string part in HumanBody.Split(';', StringSplitOptions.TrimEntries))
            {
                if (part.StartsWith(AssuranceVocabulary.FingerprintField + "=", StringComparison.Ordinal))
                    return part[(AssuranceVocabulary.FingerprintField.Length + 1)..];
            }

            return null;
        }
    }

    /// <summary>
    /// The reviewer's own assessment fields, in source order, without their name
    /// and without the fingerprint.
    ///
    /// Carried so that rewriting the line preserves them. A reviewer who
    /// recorded that they read the security risk as higher than the machine did
    /// has made a statement about the code; dropping it while writing their name
    /// back would erase it silently.
    /// </summary>
    public IReadOnlyList<string> HumanAssessment
    {
        get
        {
            if (Reviewer is null)
                return [];

            var assessment = new List<string>();
            bool first = true;
            foreach (string part in HumanBody.Split(';', StringSplitOptions.TrimEntries))
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                if (part.Length > 0 &&
                    !part.StartsWith(AssuranceVocabulary.FingerprintField + "=", StringComparison.Ordinal))
                {
                    assessment.Add(part);
                }
            }

            return assessment;
        }
    }

    /// <summary>
    /// Reads the block whose machine line is at <paramref name="aiLine"/>.
    ///
    /// The human line must be there. A block without one records an assessment
    /// nobody can answer, and treating it as an annotation would let the pane
    /// offer a reviewer somewhere to write that this file has no room for.
    /// </summary>
    public static bool TryParse(AssuranceLines lines, int aiLine, out AssuranceAnnotation? annotation)
    {
        ArgumentNullException.ThrowIfNull(lines);
        annotation = null;

        if (aiLine < 0 || aiLine >= lines.Count)
            return false;

        string ai = lines[aiLine];
        if (!ai.TrimStart().StartsWith(AssuranceVocabulary.AiMarker, StringComparison.Ordinal))
            return false;

        int next = aiLine + 1;
        int? criterionLine = null;
        string criterion = string.Empty;

        if (next < lines.Count &&
            lines[next].TrimStart().StartsWith(AssuranceVocabulary.FalsifiedIfMarker, StringComparison.Ordinal))
        {
            criterionLine = next;
            criterion = Body(lines[next], AssuranceVocabulary.FalsifiedIfMarker);
            next++;
        }

        if (next >= lines.Count ||
            !lines[next].TrimStart().StartsWith(AssuranceVocabulary.HumanMarker, StringComparison.Ordinal))
        {
            return false;
        }

        annotation = new AssuranceAnnotation(
            aiLine,
            criterionLine,
            next,
            AssuranceLines.IndentOf(ai),
            ParseFields(Body(ai, AssuranceVocabulary.AiMarker)),
            criterion,
            Body(lines[next], AssuranceVocabulary.HumanMarker));

        return true;
    }

    /// <summary>
    /// Renders one line of a block: the indent, the marker padded to the shared
    /// width, one space, and the value.
    ///
    /// This is the owning component's own expression, and matching it exactly is
    /// what keeps a line this editor writes indistinguishable from one that
    /// component wrote. An empty value renders as the bare marker with no
    /// trailing space, because a trailing space is whitespace nobody can see and
    /// every diff can.
    /// </summary>
    public static string RenderLine(string indent, string marker, string value)
    {
        ArgumentNullException.ThrowIfNull(indent);
        ArgumentNullException.ThrowIfNull(marker);
        ArgumentNullException.ThrowIfNull(value);

        return value.Length == 0
            ? indent + marker
            : indent + marker.PadRight(AssuranceVocabulary.LabelWidth) + " " + value;
    }

    /// <summary>A machine field by key, or null.</summary>
    public string? Field(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        foreach (AssuranceField field in Fields)
        {
            if (string.Equals(field.Key, key, StringComparison.Ordinal))
                return field.Value;
        }

        return null;
    }

    /// <summary>The machine's line as it would be written now.</summary>
    public string RenderAiLine()
    {
        var builder = new StringBuilder();
        for (int index = 0; index < Fields.Count; index++)
        {
            if (index > 0)
                builder.Append("; ");

            builder.Append(Fields[index].Key);
            if (Fields[index].Value.Length > 0)
                builder.Append('=').Append(Fields[index].Value);
        }

        return RenderLine(Indent, AssuranceVocabulary.AiMarker, builder.ToString());
    }

    /// <summary>The human's line carrying <paramref name="body"/>.</summary>
    public string RenderHumanLine(string body) =>
        RenderLine(Indent, AssuranceVocabulary.HumanMarker, body);

    private static string Body(string line, string marker)
    {
        string trimmed = line.TrimStart();
        return trimmed[marker.Length..].Trim();
    }

    private static IReadOnlyList<AssuranceField> ParseFields(string body)
    {
        if (body.Length == 0)
            return [];

        var fields = new List<AssuranceField>();
        foreach (string part in body.Split(';', StringSplitOptions.TrimEntries))
        {
            if (part.Length == 0)
                continue;

            int equals = part.IndexOf('=', StringComparison.Ordinal);

            // A part with no '=' is not a field this format defines. It is kept
            // with an empty value rather than dropped, so re-rendering the line
            // gives back what was there instead of quietly deleting something
            // the owning component would have reported as a problem.
            fields.Add(equals < 0
                ? new AssuranceField(part, string.Empty)
                : new AssuranceField(part[..equals], part[(equals + 1)..]));
        }

        return fields;
    }
}
