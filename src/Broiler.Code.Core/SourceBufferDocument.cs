// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           13
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Medium
// Criteria:         6/0
// Resource impact:  3/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.Code.Workspaces.Text;
using Broiler.UI.CodeEditor;

namespace Broiler.Code.Core;

/// <summary>
/// Presents a <see cref="SourceBuffer"/> to the editor control.
///
/// This adapter is the whole reason the control has its own document interface:
/// the control never learns that <see cref="SourceBuffer"/> exists, and the
/// buffer never references a UI package. Both sides can change independently as
/// long as this file keeps translating between them.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7C50A7
// Broiler-Falsified-If: after a buffer change, Snapshot still returns an adapter over a TextSnapshot older than the buffer's Current
// Broiler-Human:        PENDING
public sealed class SourceBufferDocument : ICodeDocument, IDisposable
{
    private readonly SourceBuffer _buffer;
    private SnapshotAdapter _snapshot;
    private bool _disposed;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=991026
    // Broiler-Falsified-If: a document built over a buffer keeps its first snapshot when that buffer later accepts an edit
    // Broiler-Human:        PENDING
    public SourceBufferDocument(SourceBuffer buffer)
    {
        _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        _snapshot = new SnapshotAdapter(_buffer.Current);
        _buffer.Changed += OnBufferChanged;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0A1E8E
    // Broiler-Human:        PENDING
    public event Action<ICodeTextSnapshot>? SnapshotChanged;

    /// <summary>
    /// Raised with the change that produced the new snapshot, so a classifier
    /// can reuse work instead of reclassifying the document.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7D1B2C
    // Broiler-Human:        PENDING
    public event Action<ICodeTextSnapshot, CodeTextChange>? SnapshotChangedWithDelta;

    public ICodeTextSnapshot Snapshot => _snapshot;

    public bool IsReadOnly => _buffer.IsReadOnly;

    public string LineEnding => _buffer.LineEnding;

    public bool CanUndo => _buffer.CanUndo;

    public bool CanRedo => _buffer.CanRedo;

    /// <summary>The underlying buffer, for hosts that own saving and recovery.</summary>
    public SourceBuffer Buffer => _buffer;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=977416
    // Broiler-Falsified-If: an intent whose Start plus Length overflows int escapes Submit as an exception instead of returning an OutOfRange rejection
    // Broiler-Human:        PENDING
    public CodeEditOutcome Submit(CodeEditIntent intent)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(intent.Text);

        var transaction = new EditTransaction(
            intent.BaseVersion,
            new TextChange(intent.Start, intent.Length, intent.Text),
            intent.Name);
        EditResult result = _buffer.Apply(transaction);

        return new CodeEditOutcome(
            result.Accepted,
            _snapshot,
            result.Reason switch
            {
                EditRejectionReason.StaleBaseVersion => CodeEditRejection.StaleVersion,
                EditRejectionReason.OutOfRange => CodeEditRejection.OutOfRange,
                EditRejectionReason.ReadOnly => CodeEditRejection.ReadOnly,
                _ => CodeEditRejection.None,
            });
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C88CB3
    // Broiler-Human:        PENDING
    public bool Undo() => _buffer.Undo();

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FAD26B
    // Broiler-Human:        PENDING
    public bool Redo() => _buffer.Redo();

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=479336
    // Broiler-Human:        PENDING
    public void BreakUndoGroup() => _buffer.BreakUndoGroup();

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=68D3E0
    // Broiler-Falsified-If: a buffer change after Dispose still replaces Snapshot or raises SnapshotChanged
    // Broiler-Human:        PENDING
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _buffer.Changed -= OnBufferChanged;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2FECFC
    // Broiler-Falsified-If: SnapshotChangedWithDelta carries an old or new length other than those of the buffer change it reports
    // Broiler-Human:        PENDING
    private void OnBufferChanged(TextSnapshot snapshot, TextChange change)
    {
        _snapshot = new SnapshotAdapter(snapshot);
        SnapshotChanged?.Invoke(_snapshot);
        SnapshotChangedWithDelta?.Invoke(
            _snapshot, new CodeTextChange(change.Start, change.OldLength, change.NewLength));
    }

    /// <summary>
    /// A thin, allocation-light wrapper. One instance per snapshot, so the
    /// control's reference comparison against the current snapshot means what
    /// it looks like it means.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F23A03
    // Broiler-Falsified-If: an adapter returns characters of a later snapshot after the buffer has accepted another edit
    // Broiler-Human:        PENDING
    private sealed class SnapshotAdapter(TextSnapshot snapshot) : ICodeTextSnapshot
    {
        public TextSnapshot Inner { get; } = snapshot;

        public int Version => Inner.Version;

        public int Length => Inner.Length;

        public int LineCount => Inner.LineCount;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=14A5E0
        // Broiler-Human:        PENDING
        public int GetLineStart(int line) => Inner.GetLineStart(line);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E1CF98
        // Broiler-Human:        PENDING
        public int GetLineLength(int line) => Inner.GetLineLength(line);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1E5400
        // Broiler-Human:        PENDING
        public int GetLineFromPosition(int position) => Inner.GetLineFromPosition(position);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=01135D
        // Broiler-Human:        PENDING
        public string GetText(int start, int length) => Inner.GetText(start, length);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5C2778
        // Broiler-Human:        PENDING
        public void CopyTo(int start, int length, Span<char> destination) =>
            Inner.CopyTo(start, length, destination);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A00A93
        // Broiler-Human:        PENDING
        public int GetNextCaretPosition(int position) => Inner.GetNextCaretPosition(position);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=831A3D
        // Broiler-Human:        PENDING
        public int GetPreviousCaretPosition(int position) => Inner.GetPreviousCaretPosition(position);
    }
}
