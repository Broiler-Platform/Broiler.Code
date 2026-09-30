// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           1
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    High
// Criteria:         4/2
// Resource impact:  3/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Review;

namespace Broiler.Code.Core.Review;

/// <summary>
/// Reports the workspace's current git commit, for the provenance field of a
/// review record.
///
/// It shells out to <c>git</c> rather than reading <c>.git</c> directly. Reading
/// the directory means implementing packed refs, worktrees, submodule gitdir
/// files, and alternates — all to answer a question git answers in one command —
/// and getting any of them wrong would write a wrong SHA into a record meant to
/// be evidence.
///
/// Every failure is null, and null is an ordinary answer here. A workspace that
/// is not a repository, a host with no git on PATH, a scratch directory, a
/// detached checkout: none of them stop a review being recorded, because
/// staleness is decided by content and this field is provenance. The whole class
/// degrading to <see cref="NoRevisionProvider"/> costs exactly one convenience.
///
/// It lives in Core rather than in a platform head because both desktop heads
/// want it and neither should own it. A host whose platform has no process API
/// simply never constructs one.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=25855C
// Broiler-Falsified-If: git output other than a single hexadecimal object name of 7 to 64 digits is returned as the current revision
// Broiler-Human:        PENDING
public sealed class GitRevisionProvider : IRevisionProvider
{
    private readonly string _workingDirectory;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5E3AD9
    // Broiler-Falsified-If: a null working directory is accepted and the first revision lookup starts git in the editor's own current directory
    // Broiler-Human:        PENDING
    public GitRevisionProvider(string workingDirectory) =>
        _workingDirectory = workingDirectory ?? throw new ArgumentNullException(nameof(workingDirectory));

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=71534B
    // Broiler-Falsified-If: rev-parse output with a warning line before the object name is returned as the current revision
    // Broiler-Human:        PENDING
    public async ValueTask<string?> GetCurrentRevisionAsync(CancellationToken cancellationToken = default)
    {
        string? revision = await GitCommand
            .RunAsync(_workingDirectory, "rev-parse HEAD", cancellationToken)
            .ConfigureAwait(false);

        // A SHA and nothing else. Anything git prints that is not one — a hint,
        // a warning, a localized message — is discarded rather than written into
        // a record as though it were a revision.
        return revision is not null && IsHex(revision) ? revision : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=723854
    // Broiler-Falsified-If: a string shorter than 7 or longer than 64 characters, or one holding a character outside 0-9, a-f and A-F, is accepted
    // Broiler-Human:        PENDING
    private static bool IsHex(string value)
    {
        if (value.Length is < 7 or > 64)
            return false;

        foreach (char c in value)
        {
            if (!char.IsAsciiHexDigit(c))
                return false;
        }

        return true;
    }
}
