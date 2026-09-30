// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           1
// Human-reviewed:   0/4
// IP risk:          None
// Security risk:    Medium
// Criteria:         4/0
// Resource impact:  3/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Threading;
using System.Threading.Tasks;

namespace Broiler.Code.Review;

/// <summary>
/// Supplies the source revision a review was recorded at.
///
/// This is an interface rather than a call to git because nothing in the review
/// model depends on the answer. Staleness is decided by content
/// (<see cref="ReviewContentHash"/>); the revision is provenance — it answers
/// "which commit was this read at?" for an auditor, and lets a reviewer pull up
/// the exact diff. A host that cannot answer returns null and loses nothing but
/// that convenience.
///
/// Keeping it out of the model is also what lets this assembly run where git
/// does not: a browser or Android host has a workspace and no repository, and a
/// review recorded there is worth exactly as much as one recorded on a desktop.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1474A1
// Broiler-Falsified-If: a workspace that is not a repository makes GetCurrentRevisionAsync throw instead of returning null, so a decision cannot be recorded there
// Broiler-Human:        PENDING
public interface IRevisionProvider
{
    /// <summary>
    /// The current revision, or null when there is none to report. Null is an
    /// ordinary answer — an untracked file, a workspace that is not a
    /// repository, a host with no git — and never an error.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9089C4
    // Broiler-Falsified-If: an implementation returns text that is not a revision, such as an error message from git, where the contract calls for null
    // Broiler-Human:        PENDING
    ValueTask<string?> GetCurrentRevisionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// The provider for a host that has no revision to report. Used by default, so
/// a review can always be recorded and the revision field is simply absent.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=FEA77C
// Broiler-Falsified-If: GetCurrentRevisionAsync returns a non-null value or throws
// Broiler-Human:        PENDING
public sealed class NoRevisionProvider : IRevisionProvider
{
    public static NoRevisionProvider Instance { get; } = new();

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A20867
    // Broiler-Falsified-If: GetCurrentRevisionAsync returns a non-null value or throws
    // Broiler-Human:        PENDING
    public ValueTask<string?> GetCurrentRevisionAsync(CancellationToken cancellationToken = default) =>
        ValueTask.FromResult<string?>(null);
}
