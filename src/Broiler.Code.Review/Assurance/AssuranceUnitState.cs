// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           8
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    High
// Criteria:         6/4
// Resource impact:  2/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// What the annotation on one code unit currently says, computed from the two
/// lines and never stored.
///
/// Seven of these are the owning component's own states and carry its names, so
/// that the pane, that component's report and its release gate cannot describe
/// the same declaration differently. The eighth is this editor's, and exists
/// because this editor can be composed without the thing that computes a
/// fingerprint.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=842B64
// Broiler-Falsified-If: the enum's zero value is a state other than New, so a state that was never resolved reads as more than unassessed
// Broiler-Human:        PENDING
public enum AssuranceUnitState
{
    /// <summary>A relevant unit carrying no machine line at all.</summary>
    New = 0,

    /// <summary>Assessed, with the fingerprint still to be filled by the generator.</summary>
    AiAssessed,

    /// <summary>Assessed with a real fingerprint, and no human decision.</summary>
    HumanPending,

    /// <summary>A human named themselves and left the fingerprint for the generator.</summary>
    HumanApprovedPendingFingerprint,

    /// <summary>A human approved exactly the version that is here now.</summary>
    Verified,

    /// <summary>A human approved a version that is no longer the one here.</summary>
    Stale,

    /// <summary>Exempt by the scanner's predicate, or by an explicit reason in the source.</summary>
    Exempt,

    /// <summary>
    /// A human approved a specific version and this build cannot tell whether it
    /// is still the one on screen, because nothing composed here computes a
    /// fingerprint.
    ///
    /// Reported rather than guessed. Guessing downwards would tell a reviewer
    /// their colleague's approval had lapsed on no evidence, and guessing upwards
    /// would show an approval as current over code that has since moved. The
    /// review model above this makes the same distinction between Stale and
    /// Unknown, and for the same reason.
    /// </summary>
    Unknown,
}

/// <summary>
/// The state machine, as one function of the two lines and the unit's current
/// fingerprint.
///
/// The order of the guards is part of the answer and is preserved from the
/// owning component rather than rearranged into something tidier. A line already
/// reading <c>STALE</c> is stale before anything is compared, which is what makes
/// the transition the policy forbids — stale straight back to verified —
/// unreachable by anything automatic: the only thing that clears it is a human
/// replacing the body with their own name.
///
/// Nothing here can produce <see cref="AssuranceUnitState.Verified"/> from a
/// source that does not already name a reviewer. That is the same rule as
/// <c>FileReview.WithDecision</c> hashing its own content: the one way to make
/// this record worthless is to let something approve code nobody read.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=8008E0
// Broiler-Falsified-If: a human line already reading STALE resolves to a state that does not block release, whatever fingerprint the unit now has
// Broiler-Human:        PENDING
public static class AssuranceStateMachine
{
    /// <param name="currentFingerprint">
    /// The unit's fingerprint now, or null when this build cannot compute one.
    /// Null is answered with <see cref="AssuranceUnitState.Unknown"/> at the one
    /// step that needs it, and changes no other answer.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D5E6D4
    // Broiler-Falsified-If: a human line naming a person whose recorded fingerprint differs from currentFingerprint resolves to a state that does not block release
    // Broiler-Human:        PENDING
    public static AssuranceUnitState Resolve(
        AssuranceAnnotation? annotation,
        bool isExemptByPredicate,
        string? currentFingerprint)
    {
        if (isExemptByPredicate || annotation?.ExemptReason is not null)
            return AssuranceUnitState.Exempt;

        if (annotation is null)
            return AssuranceUnitState.New;

        if (annotation.HumanIsStale)
            return AssuranceUnitState.Stale;

        string? recorded = annotation.RecordedFingerprint;
        if (recorded is null ||
            string.Equals(recorded, AssuranceVocabulary.ToBeFilled, StringComparison.Ordinal))
        {
            return AssuranceUnitState.AiAssessed;
        }

        if (annotation.HumanIsPending || annotation.Reviewer is null)
            return AssuranceUnitState.HumanPending;

        string? approved = annotation.HumanFingerprint;
        if (approved is null ||
            string.Equals(approved, AssuranceVocabulary.ToBeFilled, StringComparison.Ordinal))
        {
            return AssuranceUnitState.HumanApprovedPendingFingerprint;
        }

        if (currentFingerprint is null)
            return AssuranceUnitState.Unknown;

        return string.Equals(approved, currentFingerprint, StringComparison.Ordinal)
            ? AssuranceUnitState.Verified
            : AssuranceUnitState.Stale;
    }

    /// <summary>
    /// The release-blocking states. Only Verified and Exempt are not.
    ///
    /// <see cref="AssuranceUnitState.Unknown"/> blocks, because a state this
    /// build could not establish is not an approval anybody has checked.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=90C958
    // Broiler-Falsified-If: a state other than the current signed one and Exempt, such as Unknown or Stale, is reported as not blocking release
    // Broiler-Human:        PENDING
    public static bool BlocksRelease(AssuranceUnitState state) =>
        state is not (AssuranceUnitState.Verified or AssuranceUnitState.Exempt);

    /// <summary>
    /// The name the policy writes, so a pane and that policy read the same.
    ///
    /// <see cref="AssuranceUnitState.Unknown"/> has no policy name — it is this
    /// build admitting a limit, not a state the format defines — and is spelled
    /// in lower case so it cannot be mistaken for one in a report.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=873D59
    // Broiler-Falsified-If: two different states map to the same policy name, so a report row for one counts units in the other
    // Broiler-Human:        PENDING
    public static string Name(AssuranceUnitState state) => state switch
    {
        AssuranceUnitState.New => "NEW",
        AssuranceUnitState.AiAssessed => "AI_ASSESSED",
        AssuranceUnitState.HumanPending => "HUMAN_PENDING",
        AssuranceUnitState.HumanApprovedPendingFingerprint => "HUMAN_APPROVED_PENDING_FINGERPRINT",
        AssuranceUnitState.Verified => "VERIFIED",
        AssuranceUnitState.Stale => "STALE",
        AssuranceUnitState.Exempt => "EXEMPT",
        _ => "fingerprint unknown",
    };

    /// <summary>
    /// A sentence for the pane. Plain words rather than the policy's constants,
    /// which are for a report and read as shouting in a tree row.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=832FB5
    // Broiler-Falsified-If: a Stale or Unknown state is shown in the pane with the same text as the current signed state
    // Broiler-Human:        PENDING
    public static string ToDisplayString(AssuranceUnitState state) => state switch
    {
        AssuranceUnitState.New => "not assessed",
        AssuranceUnitState.AiAssessed => "assessed, fingerprint not yet generated",
        AssuranceUnitState.HumanPending => "needs human review",
        AssuranceUnitState.HumanApprovedPendingFingerprint => "approved, fingerprint to be generated",
        AssuranceUnitState.Verified => "human reviewed",
        AssuranceUnitState.Stale => "reviewed, then modified",
        AssuranceUnitState.Exempt => "exempt",
        _ => "approved, freshness unknown in this build",
    };
}
