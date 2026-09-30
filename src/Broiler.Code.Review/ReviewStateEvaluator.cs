// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  4/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.Code.Review;

/// <summary>
/// Turns a recorded review and the file's current content into the state the
/// product actually shows.
///
/// The whole value of the record is here. "Somebody looked at this file once" is
/// worth nothing on a codebase that moves; "this exact content was read by this
/// person on this date, and here is whether it has changed since" is a claim
/// that survives contact with development. Everything else in this assembly
/// exists to make this function answerable.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=96A140
// Broiler-Falsified-If: a record with status Reviewed reports Current freshness for text whose normalized form differs from the text it was hashed from
// Broiler-Human:        PENDING
public static class ReviewStateEvaluator
{
    /// <summary>
    /// Evaluates a record against the content on disk.
    ///
    /// <paramref name="content"/> may be null when the file could not be read —
    /// it was deleted, or the record names a path that no longer exists. That is
    /// reported as <see cref="ReviewFreshness.Unknown"/> rather than being
    /// treated as unchanged, because a deleted-and-restored file must not keep
    /// an approval it never earned.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=9D7EDE
    // Broiler-Falsified-If: a record with status Reviewed evaluated with null content reports IsVerified as true
    // Broiler-Human:        PENDING
    public static ReviewState Evaluate(FileReview? review, string? content)
    {
        if (review is null || review.Status == ReviewStatus.Unreviewed)
        {
            // A file nobody approved can still carry open questions — somebody
            // read it, wrote down what they did not understand, and did not
            // record a status. Reporting that as an untouched file would
            // understate the work done and hide the question.
            int pending = review?.OpenNoteCount ?? 0;
            return new ReviewState(ReviewStatus.Unreviewed, ReviewFreshness.NotReviewed, pending);
        }

        return new ReviewState(review.Status, FreshnessOf(review, content), review.OpenNoteCount);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=9ED4E9
    // Broiler-Falsified-If: a record whose reviewedContentHash is absent or carries an unknown algorithm prefix evaluates as Current or Stale instead of Unknown
    // Broiler-Human:        PENDING
    private static ReviewFreshness FreshnessOf(FileReview review, string? content)
    {
        if (content is null)
            return ReviewFreshness.Unknown;

        // A record written by a build that hashed differently cannot be
        // compared. Saying "unknown" keeps the reviewer's name and date visible
        // and asks for a re-confirmation; saying "stale" would blame the
        // reviewer for a tool change and teach them to ignore the warning.
        if (!ReviewContentHash.IsKnownAlgorithm(review.ReviewedContentHash))
            return ReviewFreshness.Unknown;

        return ReviewContentHash.Matches(review.ReviewedContentHash, content)
            ? ReviewFreshness.Current
            : ReviewFreshness.Stale;
    }
}
