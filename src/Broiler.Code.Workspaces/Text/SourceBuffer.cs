// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           8
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    Medium
// Criteria:         10/0
// Resource impact:  5/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;

namespace Broiler.Code.Workspaces.Text;

/// <summary>
/// The sole authority for a document's text: edit transactions, versions, and
/// undo/redo. The editor control never mutates an independent copy — it submits
/// versioned intents here and renders the accepted snapshot.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=38ED39
// Broiler-Falsified-If: undoing every entry after a sequence of typing, deletions and replacements leaves text that differs from the text the buffer was constructed with
// Broiler-Human:        PENDING
public sealed class SourceBuffer
{
    private readonly List<UndoEntry> _undo = [];
    private readonly List<UndoEntry> _redo = [];
    private int _version;
    private bool _groupOpen;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0E9201
    // Broiler-Falsified-If: a buffer built over a non-empty text reports IsDirty before any edit is applied
    // Broiler-Human:        PENDING
    public SourceBuffer(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Current = TextSnapshot.Create(text);
        SavedVersion = Current.Version;
        LineEnding = DetectLineEnding(Current);
    }

    /// <summary>Raised after a transaction is accepted, with the change that produced it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4E52F0
    // Broiler-Human:        PENDING
    public event Action<TextSnapshot, TextChange>? Changed;

    public TextSnapshot Current { get; private set; }

    /// <summary>Version last written to storage; drives the dirty overlay.</summary>
    public int SavedVersion { get; private set; }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0F712F
    // Broiler-Falsified-If: an accepted edit after MarkSaved leaves IsDirty reading false
    // Broiler-Human:        PENDING
    public bool IsDirty => Current.Version != SavedVersion;

    public bool IsReadOnly { get; set; }

    /// <summary>
    /// The terminator inserted for a new line, detected from the document so an
    /// edit does not introduce a second convention into a file that already has
    /// one.
    /// </summary>
    public string LineEnding { get; set; }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D82573
    // Broiler-Human:        PENDING
    public bool CanUndo => _undo.Count > 0;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=87BA60
    // Broiler-Human:        PENDING
    public bool CanRedo => _redo.Count > 0;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=11A34D
    // Broiler-Falsified-If: a change with Start 1 and OldLength int.MaxValue passes the range check because the sum wraps negative, and Apply throws instead of returning an OutOfRange rejection
    // Broiler-Human:        PENDING
    public EditResult Apply(EditTransaction transaction)
    {
        if (IsReadOnly)
            return EditResult.Rejected(Current, EditRejectionReason.ReadOnly);
        if (transaction.BaseVersion != Current.Version)
            return EditResult.Rejected(Current, EditRejectionReason.StaleBaseVersion);

        TextChange change = transaction.Change;
        if (change.Start < 0 || change.OldLength < 0 ||
            change.Start + change.OldLength > Current.Length)
        {
            return EditResult.Rejected(Current, EditRejectionReason.OutOfRange);
        }

        string replaced = Current.GetText(change.Start, change.OldLength);
        Push(transaction.Name, change, replaced);
        return EditResult.Applied(Commit(change));
    }

    /// <summary>
    /// Closes the current undo group. The control calls this when the caret
    /// moves, focus changes, or the document is saved — the buffer cannot see
    /// those events, and without them a whole editing session collapses into
    /// one undo step.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7F7AA7
    // Broiler-Human:        PENDING
    public void BreakUndoGroup() => _groupOpen = false;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D1481B
    // Broiler-Falsified-If: undoing a coalesced typing group leaves text that differs from the text before the group's first insertion
    // Broiler-Human:        PENDING
    public bool Undo()
    {
        if (IsReadOnly || _undo.Count == 0)
            return false;

        UndoEntry entry = _undo[^1];
        _undo.RemoveAt(_undo.Count - 1);
        _groupOpen = false;

        // Inverses run in reverse order: each was recorded against the document
        // as it stood after its predecessor.
        for (int i = entry.Steps.Count - 1; i >= 0; i--)
        {
            UndoStep step = entry.Steps[i];
            Commit(new TextChange(step.Change.Start, step.Change.NewLength, step.ReplacedText));
        }

        _redo.Add(entry);
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=41BB4D
    // Broiler-Falsified-If: redoing an undone group produces text that differs from the text the group originally produced
    // Broiler-Human:        PENDING
    public bool Redo()
    {
        if (IsReadOnly || _redo.Count == 0)
            return false;

        UndoEntry entry = _redo[^1];
        _redo.RemoveAt(_redo.Count - 1);
        _groupOpen = false;

        foreach (UndoStep step in entry.Steps)
            Commit(step.Change);

        _undo.Add(entry);
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=76CBA4
    // Broiler-Falsified-If: typing immediately after MarkSaved is merged into the undo group that was open before the save
    // Broiler-Human:        PENDING
    public void MarkSaved()
    {
        SavedVersion = Current.Version;
        BreakUndoGroup();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=346F3D
    // Broiler-Falsified-If: an insertion that does not start where the previous insertion ended is merged into the open undo group
    // Broiler-Human:        PENDING
    private void Push(string name, TextChange change, string replaced)
    {
        _redo.Clear();

        // Consecutive typing coalesces into one undo step so that undo removes a
        // word rather than a character. Only a pure insertion continuing exactly
        // where the previous one ended qualifies; anything else starts a group.
        if (_groupOpen && _undo.Count > 0 && change.OldLength == 0 && change.NewLength > 0)
        {
            UndoEntry open = _undo[^1];
            UndoStep last = open.Steps[^1];
            if (last.Change.OldLength == 0 && last.Change.NewEnd == change.Start)
            {
                open.Steps.Add(new UndoStep(change, replaced));
                return;
            }
        }

        _undo.Add(new UndoEntry(name, [new UndoStep(change, replaced)]));
        _groupOpen = change.OldLength == 0 && change.NewLength > 0;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D3EEFD
    // Broiler-Falsified-If: Changed is raised while Current still holds the previous snapshot, so a handler reading Current sees the old version
    // Broiler-Human:        PENDING
    private TextSnapshot Commit(TextChange change)
    {
        TextSnapshot next = Current.WithChange(change, ++_version);
        Current = next;
        Changed?.Invoke(next, change);
        return next;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F33A30
    // Broiler-Falsified-If: a document whose first line ends in CRLF is given a bare LF or CR as its LineEnding
    // Broiler-Human:        PENDING
    private static string DetectLineEnding(TextSnapshot snapshot)
    {
        // The first terminator wins. Scanning the whole document to take a
        // majority vote would defeat the point of not materializing it, and a
        // mixed file has no right answer anyway.
        int limit = Math.Min(snapshot.Length, 8192);
        for (int i = 0; i < limit; i++)
        {
            char c = snapshot[i];
            if (c == '\n')
                return "\n";
            if (c == '\r')
                return i + 1 < snapshot.Length && snapshot[i + 1] == '\n' ? "\r\n" : "\r";
        }

        return Environment.NewLine;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=53E0A7
    // Broiler-Human:        PENDING
    private sealed record UndoEntry(string Name, List<UndoStep> Steps);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4B07D0
    // Broiler-Human:        PENDING
    private readonly record struct UndoStep(TextChange Change, string ReplacedText);
}
