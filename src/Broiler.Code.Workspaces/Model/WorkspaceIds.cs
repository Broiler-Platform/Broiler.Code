// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           6
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    High
// Criteria:         5/5
// Resource impact:  0/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Globalization;

namespace Broiler.Code.Workspaces.Model;

/// <summary>
/// A stable runtime identity for a workspace item, independent of its current
/// path.
///
/// Paths are not identity. A rename inside the IDE must keep a document's open
/// tab, undo history, and per-user state attached to it, and none of that
/// survives keying on a path. The ID is minted once when the item enters the
/// workspace and never changes while it is there.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=E62C7B
// Broiler-Falsified-If: two items minted by one factory compare equal, so StorageFor returns one item's grant for the other
// Broiler-Human:        PENDING
public readonly record struct WorkspaceItemId(long Value)
{
    public static WorkspaceItemId None => default;

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=5E896F
    // Broiler-Falsified-If: IsNone is false for WorkspaceItemId.None, so OpenGrantedDocumentAsync gives every new granted document the ID 0 and one grant replaces another
    // Broiler-Human:        PENDING
    public bool IsNone => Value == 0;

    public override string ToString() =>
        Value.ToString(CultureInfo.InvariantCulture);
}

/// <summary>
/// Mints identities. Values are unique within one workspace instance and
/// monotonic, so an ID is never reused for a different item after a remove.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=481E1F
// Broiler-Falsified-If: two calls to Next on one factory from different threads return the same ID
// Broiler-Human:        PENDING
public sealed class WorkspaceIdFactory
{
    private long _next;

    /// <summary>
    /// Starts minting above <paramref name="highestSeen"/>, used when restoring
    /// persisted per-user state so restored IDs cannot collide with new ones.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=F3F3D9
    // Broiler-Falsified-If: a factory created with a positive highestSeen N returns N itself or a lower positive value from its first Next
    // Broiler-Human:        PENDING
    public WorkspaceIdFactory(long highestSeen = 0) => _next = Math.Max(0, highestSeen);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=FDFD03
    // Broiler-Falsified-If: Next returns a value it already returned when called from two threads at once
    // Broiler-Human:        PENDING
    public WorkspaceItemId Next() => new(System.Threading.Interlocked.Increment(ref _next));
}

/// <summary>
/// How an external rename is matched back to an existing identity.
///
/// The IDE can only claim identity across an external rename when the storage
/// provider gives it something durable to match on. Guessing from content
/// similarity would silently merge two files, so an unmatched rename is
/// reported rather than resolved.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7046D4
// Broiler-Human:        PENDING
public enum ExternalIdentityMatch
{
    /// <summary>The provider exposes a durable file ID and it matched.</summary>
    DurableFileId,

    /// <summary>The user confirmed the match.</summary>
    UserConfirmed,

    /// <summary>No durable ID and no confirmation; the item is treated as new.</summary>
    Unmatched,
}
