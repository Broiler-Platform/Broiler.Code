using System;
using System.Collections.Generic;
using System.Linq;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// One code unit, with what the owning component would say about it and
/// whether a block may be inserted above it.
/// </summary>
public sealed record AssuranceCandidate
{
    /// <summary>The unit as the file scanner reported it.</summary>
    public required AssuranceFileUnit Source { get; init; }

    /// <summary>The block the owning component attaches, read strictly. Null when there is none, or it does not parse.</summary>
    public AssuranceAnnotation? Annotation { get; init; }

    /// <summary>Why the attached <c>// Broiler-AI:</c> line did not parse, in the owning component's words.</summary>
    public string? AnnotationProblem { get; init; }

    /// <summary>Exempt by the predicate, or by an <c>EXEMPT=</c> written in the source.</summary>
    public bool IsExempt { get; init; }

    /// <summary>The exemption case, <c>DeclaredInSource</c> for <c>EXEMPT=</c>, or <c>None</c>.</summary>
    public string Exemption { get; init; } = "None";

    /// <summary>The review state, as the owning component's state machine resolves it.</summary>
    public AssuranceUnitState State { get; init; }

    /// <summary>True when a block may be inserted above the unit now.</summary>
    public bool Insertable { get; init; }

    /// <summary>
    /// One of the <see cref="AssuranceCandidates"/> reason constants: why the
    /// unit is not insertable, or <see cref="AssuranceCandidates.None"/>.
    /// </summary>
    public string Reason { get; init; } = AssuranceCandidates.None;

    /// <summary>A sentence explaining <see cref="Reason"/>, or null when it needs none.</summary>
    public string? Detail { get; init; }

    public AssuranceScannedUnit Unit => Source.Unit;

    public bool IsRelevant => !IsExempt;

    public bool IsAnnotated => Annotation is not null;
}

/// <summary>
/// Decides, per unit, what a tool may do next: nothing (exempt or annotated),
/// insert a block, or ask a human to repair what is already there.
///
/// The questions are answered from the parser's trivia and the owning
/// component's own block grammar, so "annotated" here means what it means to
/// that component. A block that does not parse leaves the unit unannotated
/// there, and inserting a second block above a broken one would give a reviewer
/// two blocks to reconcile, so such a unit is not insertable either.
/// </summary>
public static class AssuranceCandidates
{
    /// <summary>A block may be inserted.</summary>
    public const string None = "none";

    /// <summary>The unit is exempt; it takes no block.</summary>
    public const string Exempt = "exempt";

    /// <summary>The unit already carries a block that parses.</summary>
    public const string Annotated = "annotated";

    /// <summary>Something other than whitespace stands before the declaration on its line.</summary>
    public const string NotOwnLine = "not-own-line";

    /// <summary>The unit's leading trivia holds a <c>// Broiler-AI:</c> line that does not parse.</summary>
    public const string MalformedBlock = "malformed-block";

    /// <summary>The unit's leading trivia holds a human or criterion line with no AI line.</summary>
    public const string HalfBlock = "half-block";

    /// <summary>An assurance comment sits inside the declaration's header, below its first token.</summary>
    public const string BelowDeclaration = "below-declaration";

    /// <summary>
    /// The file breaks lines on characters the annotation line model does not,
    /// so no line number from the parser can be written by.
    /// </summary>
    public const string LineModelMismatch = "line-model-mismatch";

    /// <summary>Classifies every unit of one scanned file, in scan order.</summary>
    public static IReadOnlyList<AssuranceCandidate> Classify(AssuranceLines lines, AssuranceScannedFile scan)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(scan);

        bool linesAgree = scan.LineCount == lines.Count;
        var candidates = new List<AssuranceCandidate>(scan.Units.Count);

        foreach (AssuranceFileUnit source in scan.Units)
        {
            AssuranceScannedUnit unit = source.Unit;
            AssuranceAnnotation? annotation = null;
            string? problem = null;

            if (linesAgree && source.AnnotationLine is { } aiLine)
                AssuranceAnnotation.TryParseStrict(lines, aiLine, out annotation, out problem);

            bool declaredExempt = annotation?.ExemptReason is not null;
            bool exempt = unit.IsExempt || declaredExempt;
            var candidate = new AssuranceCandidate
            {
                Source = source,
                Annotation = annotation,
                AnnotationProblem = problem,
                IsExempt = exempt,
                Exemption = declaredExempt ? AssuranceVocabulary.DeclaredInSource : unit.Exemption,
                State = AssuranceStateMachine.Resolve(annotation, unit.IsExempt, unit.Fingerprint),
            };

            (string reason, string? detail) = ReasonFor(source, candidate, scan, linesAgree);
            candidates.Add(candidate with
            {
                Reason = reason,
                Detail = detail,
                Insertable = reason == None,
            });
        }

        return candidates;
    }

    private static (string Reason, string? Detail) ReasonFor(
        AssuranceFileUnit source, AssuranceCandidate candidate, AssuranceScannedFile scan, bool linesAgree)
    {
        if (candidate.IsExempt)
            return (Exempt, $"exempt: {candidate.Exemption}");

        if (candidate.Annotation is { } annotation)
            return (Annotated, $"annotated at line {annotation.AiLine + 1}");

        if (!linesAgree)
        {
            return (LineModelMismatch,
                "the file breaks lines on U+0085, U+2028 or U+2029, which the parser counts as line " +
                "breaks and the annotation line model does not");
        }

        if (!source.StartsOwnLine)
            return (NotOwnLine, "the declaration does not start its own line");

        if (source.AnnotationLine is { } aiLine)
            return (MalformedBlock, $"the block at line {aiLine + 1} does not parse: {candidate.AnnotationProblem}");

        if (source.LeadingAssuranceLines.Count > 0)
        {
            return (HalfBlock,
                $"line {source.LeadingAssuranceLines[0] + 1} carries an assurance line with no " +
                $"'{AssuranceVocabulary.AiMarker}' line above it");
        }

        int declaration = source.Unit.DeclarationLine;
        int inside = scan.AssuranceCommentLines.FirstOrDefault(
            line => line > declaration && line <= source.HeaderEndLine, -1);
        if (inside >= 0)
        {
            return (BelowDeclaration,
                $"line {inside + 1} carries an assurance comment inside the declaration, below its first line");
        }

        return (None, null);
    }
}
