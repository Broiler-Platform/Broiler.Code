using System;
using System.Collections.Generic;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// One declaration the assurance system has an opinion about, placed in the file
/// on screen.
///
/// Lines are zero-based and inclusive, matching the editor and
/// <see cref="NoteAnchoring"/> rather than the owning component's report, which
/// counts from one because a person reads it. Converting once, here, is cheaper
/// than being wrong about it in the three places that consume this.
/// </summary>
public sealed record AssuranceUnit
{
    /// <summary>
    /// The qualified, signature-shaped name: namespace, containing types, the
    /// member and its parameter list. It is how the owning component's manifest
    /// addresses a unit, and it is what a note's symbol anchor records.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>A short name for a tree row, without the qualifier.</summary>
    public required string DisplayName { get; init; }

    /// <summary>First line of the unit's extent, annotation block included.</summary>
    public required int StartLine { get; init; }

    /// <summary>Last line of the unit's extent, inclusive.</summary>
    public required int EndLine { get; init; }

    /// <summary>The line the declaration itself starts on.</summary>
    public required int DeclarationLine { get; init; }

    /// <summary>
    /// True when the unit is exempt, whether by the scanner's predicate or by a
    /// reason written into the source.
    /// </summary>
    public bool IsExempt { get; init; }

    /// <summary>Why it is exempt, in the owning component's words. "None" when it is not.</summary>
    public string Exemption { get; init; } = "None";

    /// <summary>
    /// The unit's fingerprint now, or null when nothing composed into this build
    /// can compute one.
    ///
    /// Null is not an error and is not a zero. It is the difference between "the
    /// approval on this unit has lapsed" and "this build cannot tell you whether
    /// it has", and the pane says which.
    /// </summary>
    public string? Fingerprint { get; init; }

    /// <summary>The annotation block above the declaration, when it carries one.</summary>
    public AssuranceAnnotation? Annotation { get; init; }

    /// <summary>What the annotation currently says about this unit.</summary>
    public AssuranceUnitState State { get; init; } = AssuranceUnitState.New;

    /// <summary>True when the unit counts towards the file's relevant total.</summary>
    public bool IsRelevant => !IsExempt;

    /// <summary>True when a reviewer can record a decision on this unit here.</summary>
    public bool IsWritable => Annotation is not null && !IsExempt;

    /// <summary>Whether <paramref name="line"/> falls inside the unit's extent.</summary>
    public bool Contains(int line) => line >= StartLine && line <= EndLine;
}

/// <summary>
/// One declaration as a language service found it, before the annotation above
/// it has been read.
/// </summary>
/// <param name="Name">The qualified, signature-shaped name.</param>
/// <param name="DisplayName">The member's own name, for a tree row.</param>
/// <param name="DeclarationLine">Zero-based line the declaration starts on, attributes included.</param>
/// <param name="EndLine">Zero-based last line of the declaration, inclusive.</param>
/// <param name="IsExempt">Whether the exemption predicate matched.</param>
/// <param name="Exemption">Why, in the owning component's words.</param>
/// <param name="Fingerprint">The unit's fingerprint now.</param>
public readonly record struct AssuranceScannedUnit(
    string Name,
    string DisplayName,
    int DeclarationLine,
    int EndLine,
    bool IsExempt,
    string Exemption,
    string Fingerprint);

/// <summary>
/// Finds the code units of a source file and fingerprints them.
///
/// A seam rather than an implementation, for the reason the whole repository is
/// laid out the way it is: doing this exactly needs a real C# parser, a real
/// parser means Roslyn, and Roslyn must not be in the closure of a browser or
/// Android host. A head that composes one gets exact unit boundaries, the
/// exemption predicate and real fingerprints; a head that composes none still
/// gets everything the annotation text alone can support, and the pane says
/// which of the two it is looking at rather than pretending.
///
/// Implementations must agree with the owning component token for token. They do
/// not get to be approximately right: a fingerprint that is nearly correct
/// reports every reviewed unit in the repository as stale.
/// </summary>
public interface IAssuranceUnitScanner
{
    /// <summary>
    /// The units of one file, in document order.
    ///
    /// <paramref name="path"/> is the file's own path, which some parsers record
    /// in the tree. It never reaches a fingerprint — the fingerprint covers the
    /// declaration's tokens and nothing else — so a caller with no path may pass
    /// an empty string.
    /// </summary>
    IReadOnlyList<AssuranceScannedUnit> Scan(string text, string path);
}
