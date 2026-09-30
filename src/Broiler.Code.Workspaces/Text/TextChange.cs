// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           4
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Low
// Criteria:         4/0
// Resource impact:  0/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.Code.Workspaces.Text;

/// <summary>
/// One replacement inside a snapshot: replace <paramref name="OldLength"/>
/// characters at <paramref name="Start"/> with <paramref name="NewText"/>.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=401DE7
// Broiler-Human:        PENDING
public readonly record struct TextChange(int Start, int OldLength, string NewText)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=8ED4E6
    // Broiler-Human:        PENDING
    public int NewLength => NewText.Length;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=EAB88D
    // Broiler-Human:        PENDING
    public int Delta => NewText.Length - OldLength;

    /// <summary>End of the changed region in the successor snapshot.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=231D39
    // Broiler-Human:        PENDING
    public int NewEnd => Start + NewText.Length;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E4154A
    // Broiler-Human:        PENDING
    public static TextChange Insert(int position, string text) => new(position, 0, text);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0AEBC8
    // Broiler-Human:        PENDING
    public static TextChange Delete(int position, int length) => new(position, length, string.Empty);
}

/// <summary>
/// An edit transaction names the version it was composed against. The buffer
/// either produces exactly one successor snapshot or rejects the transaction as
/// stale; it never rebases silently, because a silent rebase turns a race into
/// a wrong edit at a plausible-looking position.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=445C0B
// Broiler-Human:        PENDING
public readonly record struct EditTransaction(int BaseVersion, TextChange Change, string Name)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=406D69
    // Broiler-Falsified-If: the transaction carries a base version other than the Version of the snapshot it was composed against
    // Broiler-Human:        PENDING
    public static EditTransaction Insert(TextSnapshot snapshot, int position, string text, string name) =>
        new(snapshot.Version, TextChange.Insert(position, text), name);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0DEA34
    // Broiler-Falsified-If: the transaction carries a base version other than the Version of the snapshot it was composed against
    // Broiler-Human:        PENDING
    public static EditTransaction Delete(TextSnapshot snapshot, int position, int length, string name) =>
        new(snapshot.Version, TextChange.Delete(position, length), name);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DC3170
    // Broiler-Falsified-If: the transaction carries a base version other than the Version of the snapshot it was composed against
    // Broiler-Human:        PENDING
    public static EditTransaction Replace(
        TextSnapshot snapshot, int position, int length, string text, string name) =>
        new(snapshot.Version, new TextChange(position, length, text), name);
}

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0B1A4C
// Broiler-Human:        PENDING
public enum EditRejectionReason
{
    None = 0,

    /// <summary>The transaction named a version the buffer has already replaced.</summary>
    StaleBaseVersion,

    /// <summary>The range lies outside the snapshot.</summary>
    OutOfRange,

    /// <summary>The buffer is read-only.</summary>
    ReadOnly,
}

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=423DF8
// Broiler-Human:        PENDING
public readonly record struct EditResult(
    bool Accepted,
    TextSnapshot Snapshot,
    EditRejectionReason Reason)
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=77D0DB
    // Broiler-Falsified-If: a rejection built with EditRejectionReason.None is returned instead of throwing
    // Broiler-Human:        PENDING
    public static EditResult Rejected(TextSnapshot current, EditRejectionReason reason)
    {
        if (reason == EditRejectionReason.None)
            throw new ArgumentOutOfRangeException(nameof(reason), "A rejection needs a reason.");
        return new EditResult(false, current, reason);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=61B998
    // Broiler-Human:        PENDING
    public static EditResult Applied(TextSnapshot snapshot) =>
        new(true, snapshot, EditRejectionReason.None);
}
