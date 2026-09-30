// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           9
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         8/6
// Resource impact:  0/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.Code.Review;

/// <summary>
/// What a reviewer recorded about a file.
///
/// These are the reviewer's own words about their own work, and nothing else
/// writes them. The one state a reviewer cannot record is <c>Stale</c>: that is
/// derived from the file's content and is reported separately by
/// <see cref="ReviewState"/>, because a status a tool can silently change is a
/// status a reviewer cannot be held to.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=864469
// Broiler-Falsified-If: the zero member is not Unreviewed, so a status left at its default reads as a recorded decision
// Broiler-Human:        PENDING
public enum ReviewStatus
{
    /// <summary>Nobody has looked at this file yet. The default for every file with no record.</summary>
    Unreviewed = 0,

    /// <summary>
    /// Someone has started and has not finished. Distinct from Unreviewed so a
    /// review that spans several sittings does not have to be restarted, and so
    /// two reviewers can see that a file is already being read.
    /// </summary>
    InReview,

    /// <summary>
    /// A human read this file and understood it. This is the state the platform's
    /// "AI-generated code is human reviewed" claim rests on, and the only one
    /// <see cref="ReviewState.IsVerified"/> counts.
    ///
    /// Recording it, like every status other than <see cref="Unreviewed"/>,
    /// requires a reviewer and a timestamp — <see cref="FileReview.WithDecision"/>
    /// takes both, and the shell will not offer the command without a reviewer
    /// name.
    /// </summary>
    Reviewed,

    /// <summary>
    /// Read, but something about it is not understood. The file is not approved
    /// and the open question is in the notes.
    /// </summary>
    Question,

    /// <summary>
    /// Read, and something is wrong. Stronger than Question: the reviewer is not
    /// asking, they are asserting that the code has to change.
    /// </summary>
    NeedsChange,
}

/// <summary>
/// How a file's recorded status relates to the bytes on disk right now.
///
/// This is the distinction that makes the whole record worth keeping. "Somebody
/// looked at this file once" decays to nothing as a codebase moves; "this exact
/// content was reviewed" does not. The comparison is over content, not over a
/// commit, so a rebase, a cherry-pick, a squash, or a branch switch does not
/// invalidate a review that still describes the code — and a change that is
/// reverted becomes current again, which is the honest answer.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=5B69A0
// Broiler-Falsified-If: the zero member is Current, so a Reviewed state whose freshness was left at its default reports IsVerified as true
// Broiler-Human:        PENDING
public enum ReviewFreshness
{
    /// <summary>No review has been recorded, so there is nothing to be stale.</summary>
    NotReviewed = 0,

    /// <summary>The content hash matches what was reviewed. The record still describes this file.</summary>
    Current,

    /// <summary>
    /// The file changed after it was reviewed. The recorded status is kept and
    /// reported alongside this, because "reviewed, then modified" is more useful
    /// than "unreviewed": it says a reviewer's earlier reading exists and needs
    /// re-confirming, not that the file has never been read.
    /// </summary>
    Stale,

    /// <summary>
    /// A record exists but its content could not be compared — the file is gone,
    /// or unreadable. Reported rather than assumed current, because assuming
    /// would let a deleted-and-restored file keep an approval it never earned.
    /// </summary>
    Unknown,
}

/// <summary>
/// A file's recorded status together with how it relates to the current bytes.
///
/// The two halves are kept apart on purpose. Coverage counts and the explorer
/// badge both need to say "reviewed, but modified afterwards" as its own thing,
/// and folding it into a single enum would mean overwriting what a reviewer
/// actually said.
/// </summary>
/// <param name="Status">What the reviewer recorded.</param>
/// <param name="Freshness">How that record relates to the file as it is now.</param>
/// <param name="OpenNotes">Notes still awaiting an answer, which keep a file visible after it is marked reviewed.</param>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=5F4BB4
// Broiler-Falsified-If: a state with Status Reviewed and Freshness Stale, Unknown or NotReviewed reports IsVerified as true
// Broiler-Human:        PENDING
public readonly record struct ReviewState(
    ReviewStatus Status,
    ReviewFreshness Freshness,
    int OpenNotes = 0)
{
    /// <summary>A file with no record at all.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=1109EB
    // Broiler-Falsified-If: ReviewState.None reports IsVerified, IsStaleApproval or IsAttested as true
    // Broiler-Human:        PENDING
    public static ReviewState None => new(ReviewStatus.Unreviewed, ReviewFreshness.NotReviewed);

    /// <summary>
    /// True only for a file a human approved and that has not changed since.
    /// This is what the coverage number counts, and the deliberately strict
    /// reading: a stale approval is not an approval.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=6E7763
    // Broiler-Falsified-If: a Question, InReview or NeedsChange state with Current freshness reports IsVerified as true
    // Broiler-Human:        PENDING
    public bool IsVerified =>
        Status == ReviewStatus.Reviewed && Freshness == ReviewFreshness.Current;

    /// <summary>True when a reviewer approved this content but it has since changed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=995F9F
    // Broiler-Falsified-If: a Reviewed state with Stale freshness reports IsStaleApproval as false, so the stale check passes a changed file
    // Broiler-Human:        PENDING
    public bool IsStaleApproval =>
        Status == ReviewStatus.Reviewed && Freshness == ReviewFreshness.Stale;

    /// <summary>
    /// True when a human has recorded an opinion of any kind. A file with an
    /// open question has been looked at even though it is not approved, and
    /// reporting it as untouched would understate the work done.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A13D2B
    // Broiler-Falsified-If: an Unreviewed state reports IsAttested as true
    // Broiler-Human:        PENDING
    public bool IsAttested => Status != ReviewStatus.Unreviewed;

    /// <summary>
    /// A short label for a tree row or a report line. Deliberately plain text:
    /// it is read in a terminal, in a Markdown table, and in a GitHub annotation
    /// as often as in the editor.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D6D311
    // Broiler-Falsified-If: a Reviewed state with Stale or Unknown freshness is labelled plain reviewed
    // Broiler-Human:        PENDING
    public string ToDisplayString() => (Status, Freshness) switch
    {
        (ReviewStatus.Unreviewed, _) => "needs review",
        (_, ReviewFreshness.Unknown) => "review state unknown",
        (ReviewStatus.Reviewed, ReviewFreshness.Stale) => "reviewed, then modified",
        (ReviewStatus.Reviewed, _) => "reviewed",
        (ReviewStatus.InReview, ReviewFreshness.Stale) => "in review, then modified",
        (ReviewStatus.InReview, _) => "in review",
        (ReviewStatus.Question, ReviewFreshness.Stale) => "open question, then modified",
        (ReviewStatus.Question, _) => "open question",
        (ReviewStatus.NeedsChange, ReviewFreshness.Stale) => "needs change, then modified",
        (ReviewStatus.NeedsChange, _) => "needs change",

        // Unreachable for the declared members, and required: a switch
        // expression over an enum is not exhaustive to the compiler, because a
        // cast can produce a value no member names. Reporting it rather than
        // throwing keeps a corrupt record readable in the pane that is supposed
        // to help someone fix it.
        _ => "review state unknown",
    };
}
