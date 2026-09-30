// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           1
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    High
// Criteria:         11/8
// Resource impact:  4/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Workspaces.Model;

namespace Broiler.Code.Workspaces.Recovery;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3B1A78
// Broiler-Falsified-If: an entry journaled at one revision reports no conflict when the file on disk is now at a different revision, so restoring replaces the newer file
// Broiler-Human:        PENDING
public sealed record RecoveredDocument(
    WorkspaceItemId Id,
    string RelativePath,
    string Text,
    string? ExternalRevisionWhenJournaled)
{
    /// <summary>
    /// True when the file on disk has moved on since the journal was written,
    /// so restoring is a merge decision rather than a restore.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FB0559
    // Broiler-Falsified-If: an entry journaled at one revision reports no conflict when the file on disk is now at a different revision, so restoring replaces the newer file
    // Broiler-Human:        PENDING
    public bool ConflictsWith(string? currentRevision) =>
        ExternalRevisionWhenJournaled is not null &&
        !string.Equals(ExternalRevisionWhenJournaled, currentRevision, StringComparison.Ordinal);
}

/// <summary>
/// An app-private record of unsaved text, so a crash or a killed process does
/// not lose work.
///
/// It lives outside the workspace on purpose. Writing recovery data into the
/// user's source tree would put it under version control, inside the build's
/// input set, and inside the granted roots a build worker materializes — three
/// places it has no business being. On Android the same reasoning makes it the
/// app-private mirror rather than the SAF tree.
///
/// Entries are written whole and replaced atomically. A half-written journal is
/// worse than none: it looks recoverable and restores corrupted text.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=AD192A
// Broiler-Falsified-If: two overlapping RecordAsync calls for one document share the same .tmp file, so the entry left on disk holds the older text or interleaved bytes that no longer parse
// Broiler-Human:        PENDING
public sealed class RecoveryJournal
{
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=3066F2
    // Broiler-Falsified-If: an entry written with these options does not read back with the same id, path, text and revision
    // Broiler-Human:        PENDING
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    private readonly string _directory;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B584E1
    // Broiler-Falsified-If: a whitespace-only directory is accepted, so entries are written relative to the process's working directory
    // Broiler-Human:        PENDING
    public RecoveryJournal(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _directory = directory;
    }

    /// <summary>
    /// The journal for one workspace, keyed by a hash of its root so two open
    /// workspaces cannot overwrite each other's recovery data.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=29B579
    // Broiler-Falsified-If: two workspace roots that differ only in letter case, which are different directories on a case-sensitive file system, get the same journal directory and overwrite each other's entries
    // Broiler-Human:        PENDING
    public static RecoveryJournal ForWorkspace(string appPrivateRoot, string workspaceRoot)
    {
        string key = Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes(workspaceRoot.ToUpperInvariant())))[..16];
        return new RecoveryJournal(Path.Combine(appPrivateRoot, "recovery", key));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=34781E
    // Broiler-Falsified-If: two overlapping RecordAsync calls for one document share the same .tmp file, so the entry left on disk holds the older text or interleaved bytes that no longer parse
    // Broiler-Human:        PENDING
    public async ValueTask RecordAsync(
        WorkspaceItemId id,
        WorkspaceItem item,
        string text,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(text);
        Directory.CreateDirectory(_directory);

        var entry = new JournalEntry(
            id.Value, item.RelativePath, text, item.ExternalRevision);

        string path = EntryPath(id);
        string temporary = path + ".tmp";
        await File.WriteAllTextAsync(
            temporary, JsonSerializer.Serialize(entry, Options), cancellationToken)
            .ConfigureAwait(false);

        // Replace rather than write in place, so a crash mid-write leaves the
        // previous entry rather than a truncated one.
        File.Move(temporary, path, overwrite: true);
    }

    /// <summary>Drops an entry once its document is saved or discarded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3BE34A
    // Broiler-Falsified-If: Forget removes the entry file of a different document, or leaves the entry of its own document on disk
    // Broiler-Human:        PENDING
    public void Forget(WorkspaceItemId id)
    {
        string path = EntryPath(id);
        if (File.Exists(path))
            File.Delete(path);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=A248D3
    // Broiler-Falsified-If: Clear deletes files outside the journal directory, for example by following a link placed inside it
    // Broiler-Human:        PENDING
    public void Clear()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }

    /// <summary>
    /// Everything the journal holds. A malformed entry is skipped rather than
    /// failing the whole recovery: one unreadable file must not cost the user
    /// the other nineteen.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=63FA80
    // Broiler-Falsified-If: one entry file that cannot be opened for lack of permission makes ReadAllAsync throw instead of skipping it and returning the other entries
    // Broiler-Human:        PENDING
    public async ValueTask<IReadOnlyList<RecoveredDocument>> ReadAllAsync(
        CancellationToken cancellationToken = default)
    {
        var recovered = new List<RecoveredDocument>();
        if (!Directory.Exists(_directory))
            return recovered;

        foreach (string path in Directory.EnumerateFiles(_directory, "*.json"))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                string json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
                JournalEntry? entry = JsonSerializer.Deserialize<JournalEntry>(json, Options);
                if (entry is null)
                    continue;

                recovered.Add(new RecoveredDocument(
                    new WorkspaceItemId(entry.Id),
                    entry.RelativePath,
                    entry.Text,
                    entry.ExternalRevision));
            }
            catch (Exception exception) when (exception is JsonException or IOException)
            {
                // Skipped deliberately; see the summary above.
            }
        }

        recovered.Sort(static (left, right) =>
            string.CompareOrdinal(left.RelativePath, right.RelativePath));
        return recovered;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=66DC4A
    // Broiler-Falsified-If: two different ids map to the same entry file, or an id maps to a file outside the journal directory
    // Broiler-Human:        PENDING
    private string EntryPath(WorkspaceItemId id) => Path.Combine(
        _directory,
        string.Create(CultureInfo.InvariantCulture, $"{id.Value}.json"));

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=6A2E9E
    // Broiler-Human:        PENDING
    private sealed record JournalEntry(
        long Id, string RelativePath, string Text, string? ExternalRevision);
}
