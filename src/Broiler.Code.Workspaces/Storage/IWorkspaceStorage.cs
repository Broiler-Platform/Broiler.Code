// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           18
// Human-reviewed:   0/15
// IP risk:          None
// Security risk:    High
// Criteria:         12/7
// Resource impact:  5/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Broiler.Code.Workspaces.Storage;

/// <summary>
/// What a storage provider can actually do.
///
/// The workspace asks rather than assumes. A desktop filesystem renames
/// atomically and exposes durable file IDs; Android's Storage Access Framework
/// gives a document tree with different rename semantics; a browser has
/// directory handles on some engines and nothing but import/export on others.
/// Code that assumes the desktop shape works until it reaches a device, so the
/// capability is a value the workspace reads and honours.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=42E7DB
// Broiler-Falsified-If: two members share a bit, so a provider that reports one capability is read as reporting another
// Broiler-Human:        PENDING
[Flags]
public enum StorageCapabilities
{
    None = 0,

    Read = 1,

    Write = 2,

    /// <summary>Create and delete entries.</summary>
    Modify = 4,

    /// <summary>Rename without a read-write-delete round trip.</summary>
    Rename = 8,

    /// <summary>
    /// Writes land atomically — a crash mid-save leaves the old file, never a
    /// truncated one.
    /// </summary>
    AtomicWrite = 16,

    /// <summary>Exposes an identity that survives a rename outside the IDE.</summary>
    DurableFileIds = 32,

    /// <summary>Reports external changes without polling.</summary>
    ChangeNotification = 64,

    /// <summary>Paths differing only in case denote the same entry.</summary>
    CaseInsensitivePaths = 128,
}

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B7F9CF
// Broiler-Falsified-If: two members share a value, so an OutsideGrant refusal is read by the caller as another kind of failure
// Broiler-Human:        PENDING
public enum StorageFailureKind
{
    None = 0,
    NotFound,
    PermissionDenied,

    /// <summary>The entry changed since it was read.</summary>
    Conflict,

    /// <summary>The path escapes the granted roots.</summary>
    OutsideGrant,

    /// <summary>The capability needed for the operation is not available.</summary>
    Unsupported,

    /// <summary>The provider failed for a reason it could not classify.</summary>
    Unknown,
}

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=7843F7
// Broiler-Human:        PENDING
public sealed record StorageFailure(StorageFailureKind Kind, string Message);

/// <summary>
/// The result of a storage operation. Failures are values rather than
/// exceptions: Save All has to report which of twenty files failed and why
/// while still saving the rest, and an exception per file makes that awkward
/// enough that callers start swallowing them.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B8DC4E
// Broiler-Falsified-If: Ok yields a result with Succeeded false or a Failure set, or Fail yields one with Succeeded true
// Broiler-Human:        PENDING
public readonly record struct StorageResult<T>(bool Succeeded, T? Value, StorageFailure? Failure)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=FE52E2
    // Broiler-Falsified-If: Ok yields a result with Succeeded false or a Failure set
    // Broiler-Human:        PENDING
    public static StorageResult<T> Ok(T value) => new(true, value, null);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=284A56
    // Broiler-Falsified-If: Fail yields a result with Succeeded true or a Failure of another kind
    // Broiler-Human:        PENDING
    public static StorageResult<T> Fail(StorageFailureKind kind, string message) =>
        new(false, default, new StorageFailure(kind, message));
}

/// <summary>Text plus what is needed to write it back unchanged.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=20243D
// Broiler-Human:        PENDING
public sealed record StorageTextContent(
    string Text,
    Model.TextEncodingInfo Encoding,
    string? ExternalRevision,
    string? DurableFileId);

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=65CBE8
// Broiler-Human:        PENDING
public sealed record StorageEntry(
    string RelativePath,
    bool IsDirectory,
    long SizeBytes,
    string? ExternalRevision,
    string? DurableFileId);

/// <summary>
/// Asynchronous, capability-based access to the granted workspace roots.
///
/// Asynchronous throughout because two of the four target hosts have no
/// synchronous option: Android's SAF and the browser's file handles are both
/// promise-shaped, and a synchronous contract would either block their UI
/// thread or need a second contract later.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=5; Fingerprint=DB7ACF
// Broiler-Falsified-If: an implementation reads, writes, lists or deletes an entry outside GrantedRoots for a path with .. segments or one that names a link
// Broiler-Human:        PENDING
public interface IWorkspaceStorage
{
    StorageCapabilities Capabilities { get; }

    /// <summary>Roots the user granted. Nothing outside them is reachable.</summary>
    IReadOnlyList<string> GrantedRoots { get; }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=5; Fingerprint=CD76AB
    // Broiler-Falsified-If: an implementation returns the text of an entry outside the granted roots for a path with .. segments or one that names a link
    // Broiler-Human:        PENDING
    ValueTask<StorageResult<StorageTextContent>> ReadTextAsync(
        string relativePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes text. <paramref name="expectedRevision"/> is the revision the
    /// caller last saw; a mismatch fails with
    /// <see cref="StorageFailureKind.Conflict"/> rather than overwriting.
    /// Passing null writes unconditionally, which only a caller that has
    /// already resolved the conflict should do.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=7949B7
    // Broiler-Falsified-If: a write whose expectedRevision differs from the entry's current revision replaces the entry instead of failing with Conflict
    // Broiler-Human:        PENDING
    ValueTask<StorageResult<string>> WriteTextAsync(
        string relativePath,
        string text,
        Model.TextEncodingInfo encoding,
        string? expectedRevision,
        CancellationToken cancellationToken = default);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=A8D97A
    // Broiler-Falsified-If: an implementation lists an entry that lies outside the granted roots, such as the target of a link
    // Broiler-Human:        PENDING
    ValueTask<StorageResult<IReadOnlyList<StorageEntry>>> ListAsync(
        string relativeDirectory, CancellationToken cancellationToken = default);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=542CE2
    // Broiler-Falsified-If: an implementation reports the size or revision of an entry outside the granted roots instead of failing with OutsideGrant
    // Broiler-Human:        PENDING
    ValueTask<StorageResult<StorageEntry>> StatAsync(
        string relativePath, CancellationToken cancellationToken = default);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=5; Fingerprint=944079
    // Broiler-Falsified-If: an implementation removes an entry outside the granted roots, such as files under a link inside a deleted directory
    // Broiler-Human:        PENDING
    ValueTask<StorageResult<bool>> DeleteAsync(
        string relativePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renames, if <see cref="StorageCapabilities.Rename"/> is present. A
    /// provider without it fails with
    /// <see cref="StorageFailureKind.Unsupported"/> so the caller can fall back
    /// to a copy-and-delete deliberately rather than by accident.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=346EC9
    // Broiler-Falsified-If: a provider without the Rename capability moves the entry instead of failing with Unsupported
    // Broiler-Human:        PENDING
    ValueTask<StorageResult<StorageEntry>> RenameAsync(
        string fromRelativePath, string toRelativePath, CancellationToken cancellationToken = default);
}
