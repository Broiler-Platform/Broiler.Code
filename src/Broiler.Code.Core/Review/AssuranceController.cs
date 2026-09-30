// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   24
// Annotated:        24/24
// Exempt:           16
// Human-reviewed:   0/24
// IP risk:          Low
// Security risk:    High
// Criteria:         21/11
// Resource impact:  6/10 max
// Unverified:       24
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.Code.Review.Assurance;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Text;

namespace Broiler.Code.Core.Review;

/// <summary>Why a unit-level review action could not be carried out.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=E818E1
// Broiler-Human:        PENDING
public enum AssuranceActionOutcome
{
    Applied = 0,

    /// <summary>No file is open, or the caret is not inside an annotated unit.</summary>
    NoTarget,

    /// <summary>The unit refused the decision — it is exempt, unannotated, or already says this.</summary>
    Refused,

    /// <summary>The buffer would not take the edit.</summary>
    EditRejected,
}

/// <summary>The result of a unit-level action, with a sentence for the status line.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CF8E0D
// Broiler-Falsified-If: a result whose outcome is Refused, NoTarget or EditRejected reports Succeeded as true
// Broiler-Human:        PENDING
public readonly record struct AssuranceActionResult(AssuranceActionOutcome Outcome, string Message)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4CC1D0
    // Broiler-Falsified-If: a result whose outcome is Refused, NoTarget or EditRejected reports Succeeded as true
    // Broiler-Human:        PENDING
    public bool Succeeded => Outcome == AssuranceActionOutcome.Applied;
}

/// <summary>
/// The assurance half of the review pane: which code unit the caret is in, what
/// the annotation above it records, and the one line a reviewer writes.
///
/// It sits beside <see cref="ReviewController"/> rather than inside it because
/// the two record different claims about different things, and folding them
/// together would blur the distinction the whole review workspace rests on. A
/// file review says <em>a person read this file's content, and here is the hash
/// of what they read</em>; it lives in <c>.broiler-review/</c> and belongs to
/// this repository. A unit's assurance line says <em>a person stands behind this
/// declaration</em>; it lives in the source file and belongs to the component
/// that owns the format. Neither is a substitute for the other, and a pane that
/// showed one number for both would be claiming something nobody recorded.
///
/// The write goes through the open buffer rather than through storage. That is
/// what makes it undoable, what puts it in front of the reviewer before it is
/// committed, and what leaves saving where it already is — this controller never
/// writes a file.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=844676
// Broiler-Falsified-If: a signature is written into the open buffer from a document model read at an earlier buffer version, so edits made since that version are reverted
// Broiler-Human:        PENDING
public sealed class AssuranceController : IDisposable
{
    private readonly CodeWorkspace _workspace;
    private readonly IAssuranceUnitScanner? _scanner;

    private WorkspaceItemId _current = WorkspaceItemId.None;
    private AssuranceDocument? _document;
    private SourceBuffer? _watched;
    private string _documentPath = string.Empty;
    private int _documentVersion = -1;
    private int _caretLine;
    private bool _disposed;

    /// <param name="scanner">
    /// The language service that finds units and fingerprints them, when the host
    /// composed one. Without it the pane reads the annotation blocks alone, which
    /// is enough to show what is recorded and to record a decision, and not
    /// enough to recount the file's generated header.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=EAF115
    // Broiler-Falsified-If: a null workspace is accepted and the first document switch throws NullReferenceException instead of the constructor throwing ArgumentNullException
    // Broiler-Human:        PENDING
    public AssuranceController(CodeWorkspace workspace, IAssuranceUnitScanner? scanner = null)
    {
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _scanner = scanner;
    }

    /// <summary>Raised when the unit under the caret, or what it records, changed.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=8C487F
    // Broiler-Human:        PENDING
    public event EventHandler? Changed;

    /// <summary>
    /// Who is recording reviews. The same name the file-level review uses, so a
    /// reviewer is one person to this tool rather than two.
    /// </summary>
    public string Reviewer { get; set; } = string.Empty;

    /// <summary>True when a language service supplied the units.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C31AED
    // Broiler-Human:        PENDING
    public bool HasUnitScanner => _scanner is not null;

    /// <summary>The current file's assurance model, or null when nothing is open.</summary>
    public AssuranceDocument? Document => _document;

    /// <summary>The unit the caret is in, or null.</summary>
    public AssuranceUnit? CurrentUnit { get; private set; }

    /// <summary>Every unit of the current file, for a pane listing them.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F9CFD5
    // Broiler-Falsified-If: with no document open the property returns null instead of an empty list
    // Broiler-Human:        PENDING
    public IReadOnlyList<AssuranceUnit> Units => _document?.Units ?? [];

    /// <summary>True when the file on screen carries assurance annotations at all.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E68ED9
    // Broiler-Falsified-If: a file with neither annotation blocks nor a generated header reports true
    // Broiler-Human:        PENDING
    public bool IsAnnotatedFile => _document is { IsAnnotated: true };


    /// <summary>Points the pane at a document and re-reads it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=85AAF2
    // Broiler-Falsified-If: after a switch to another document, the unit picked before the editor reports a caret comes from the previous document's caret line rather than line 0
    // Broiler-Human:        PENDING
    public void SetCurrentDocument(WorkspaceItemId id)
    {
        if (_current == id)
            return;

        _current = id;
        _documentVersion = -1;
        Watch();

        // The caret belongs to the document that has gone. Carrying its line into
        // the new file would put the pane on whatever declaration happens to
        // occupy that line there — and the next signature would land on it. The
        // editor tells us where the real caret is when it next moves; until then
        // the top of the file is the honest answer.
        _caretLine = 0;

        Refresh();
    }

    /// <summary>
    /// Moves the pane to the unit containing <paramref name="line"/>.
    ///
    /// Cheap on purpose: the caret moves on every keystroke and every click, and
    /// re-reading the file each time would put a parse of the whole document on a
    /// gesture that has to feel instant. The model is rebuilt only when the
    /// buffer's version has actually moved.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=A74D32
    // Broiler-Falsified-If: moving the caret to a line inside a different declaration, with the buffer version unchanged, leaves CurrentUnit on the previous declaration
    // Broiler-Human:        PENDING
    public void SetCaretLine(int line)
    {
        int clamped = Math.Max(0, line);
        if (_caretLine == clamped && _document is not null && !VersionMoved())
            return;

        _caretLine = clamped;
        Refresh();
    }

    /// <summary>Re-reads the current document and re-picks the unit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=C349F7
    // Broiler-Falsified-If: CurrentUnit after a refresh comes from a document model read at a buffer version older than the buffer's current one
    // Broiler-Human:        PENDING
    public void Refresh()
    {
        string before = Signature();

        Rebuild();
        CurrentUnit = _document?.UnitAt(_caretLine);

        if (!string.Equals(before, Signature(), StringComparison.Ordinal))
            Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Detaches from the buffer it is watching.
    ///
    /// The controller outlives no workspace, but the buffer of an open document
    /// does outlive the controller when a workspace is replaced — so the
    /// subscription has to come off, or every superseded controller goes on
    /// re-reading a file for a pane nobody is showing.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F365D4
    // Broiler-Falsified-If: an edit to the watched buffer after Dispose still re-reads the document and raises Changed
    // Broiler-Human:        PENDING
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        Unwatch();
        _document = null;
    }

    /// <summary>
    /// What the pane would show, reduced to the things a reader would notice
    /// changing.
    ///
    /// Compared instead of the unit itself because a rebuild produces new
    /// objects every time — the annotation is a fresh instance, so record
    /// equality reports a change on every keystroke, and every change rebuilds
    /// the menu. What actually matters is which declaration is selected and what
    /// its line now says.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D0B45C
    // Broiler-Falsified-If: a change to the current unit's human line text, with the same unit and state, leaves the returned string unchanged
    // Broiler-Human:        PENDING
    private string Signature() =>
        CurrentUnit is not { } unit
            ? string.Empty
            : string.Concat(
                unit.Name, "\0",
                AssuranceStateMachine.Name(unit.State), "\0",
                unit.Annotation?.HumanBody ?? string.Empty);

    /// <summary>
    /// Follows the open document's buffer, so an edit that changes an annotation
    /// reaches the pane without waiting for the caret to move.
    ///
    /// Undo is the case that makes this necessary: taking back a signature
    /// restores the text and leaves the caret where it was, and a pane that only
    /// listened to the caret would go on showing an approval the file no longer
    /// carries.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BFABF1
    // Broiler-Falsified-If: after a switch of documents, an undo in the newly current buffer does not update the pane until the caret moves
    // Broiler-Human:        PENDING
    private void Watch()
    {
        Unwatch();

        if (_workspace.FindOpenDocument(_current) is { } open)
        {
            _watched = open.Buffer;
            _watched.Changed += OnBufferChanged;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BA021F
    // Broiler-Falsified-If: after Unwatch, an edit to the previously watched buffer still reaches OnBufferChanged
    // Broiler-Human:        PENDING
    private void Unwatch()
    {
        if (_watched is not null)
            _watched.Changed -= OnBufferChanged;

        _watched = null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=B616D0
    // Broiler-Falsified-If: an edit that rewrites a human line in the watched buffer leaves CurrentUnit reporting the line as it was before the edit
    // Broiler-Human:        PENDING
    private void OnBufferChanged(TextSnapshot snapshot, TextChange change)
    {
        if (_disposed)
            return;

        _documentVersion = -1;
        Refresh();
    }

    /// <summary>
    /// Records the reviewer against the unit the caret is in, and recounts the
    /// file's generated header when this build has shown it may.
    ///
    /// Allowed on a document with unsaved changes, unlike
    /// <see cref="ReviewController.RecordDecisionAsync"/>, and the difference is
    /// the point rather than an inconsistency. That decision records a hash of the
    /// content a person read, so unsaved text would make it unverifiable by
    /// anyone. This one records a name and no claim about content at all: the
    /// fingerprint that binds the approval to a version is written later, by the
    /// owning component's generator, over the tree that was actually committed.
    /// There is nothing here for unsaved text to make unverifiable — and a
    /// reviewer working down a file would otherwise have to save between every
    /// declaration.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=6; Fingerprint=1CA5CD
    // Broiler-Falsified-If: signing while the configured name is empty or whitespace changes the open buffer
    // Broiler-Human:        PENDING
    public AssuranceActionResult Approve() =>
        Act(static (document, unit, reviewer) => document.Approve(unit, reviewer));

    /// <summary>Puts the unit the caret is in back to pending.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=6; Fingerprint=4CEFEA
    // Broiler-Falsified-If: withdrawing changes a line of the buffer other than the human line of the unit that contains the caret
    // Broiler-Human:        PENDING
    public AssuranceActionResult Withdraw() =>
        Act(static (document, unit, _) => document.Withdraw(unit));

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=826F83
    // Broiler-Falsified-If: after the buffer refuses one decision, the next accepted decision writes the refused line change into the buffer as well as its own
    // Broiler-Human:        PENDING
    private AssuranceActionResult Act(
        Func<AssuranceDocument, AssuranceUnit, string, AssuranceEditResult> decide)
    {
        Rebuild();
        CurrentUnit = _document?.UnitAt(_caretLine);

        if (_document is not { } document)
            return new AssuranceActionResult(AssuranceActionOutcome.NoTarget, "Open a file to review it.");

        if (CurrentUnit is not { } unit)
        {
            return new AssuranceActionResult(
                AssuranceActionOutcome.NoTarget,
                document.IsAnnotated
                    ? "Put the caret inside an annotated declaration."
                    : "This file carries no Broiler Code Assurance annotations.");
        }

        AssuranceEditResult edit = decide(document, unit, Reviewer);
        if (!edit.Succeeded)
            return new AssuranceActionResult(AssuranceActionOutcome.Refused, edit.Message);

        string? rejection = Apply(edit.Text);

        // Invalidated either way. The document model applied the rewrite to its
        // own copy before the buffer was asked, so a refusal leaves it holding an
        // edit the file does not have — and the next successful action would
        // write both. Re-reading costs one parse on a path a person took
        // deliberately.
        _documentVersion = -1;

        if (rejection is not null)
        {
            Refresh();
            return new AssuranceActionResult(AssuranceActionOutcome.EditRejected, rejection);
        }

        // Rebuilt from the buffer rather than from the text just produced, so the
        // pane reports what the document actually holds.
        Refresh();
        Changed?.Invoke(this, EventArgs.Empty);

        return new AssuranceActionResult(AssuranceActionOutcome.Applied, edit.Message);
    }

    /// <summary>
    /// Puts the rewritten text into the open buffer as one edit, and returns a
    /// sentence when the buffer refused it.
    ///
    /// The edit is narrowed to the span that actually changed rather than
    /// submitted as a whole-file replacement. A whole-file edit would be one undo
    /// step that swallows the file, and would move every caret and selection in
    /// it; what a reviewer did was change one line, and the undo history should
    /// say so.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=89F717
    // Broiler-Falsified-If: a buffer that moved to a new version between reading the snapshot and submitting the edit accepts the replacement instead of rejecting it as stale
    // Broiler-Human:        PENDING
    private string? Apply(string text)
    {
        if (_workspace.FindOpenDocument(_current) is not { } open)
            return "The file is no longer open.";

        TextSnapshot snapshot = open.Buffer.Current;
        string current = snapshot.ToString();
        if (string.Equals(current, text, StringComparison.Ordinal))
            return null;

        (int start, int oldLength, string replacement) = Narrow(current, text);

        EditResult result = open.Buffer.Apply(new EditTransaction(
            snapshot.Version,
            new TextChange(start, oldLength, replacement),
            "Assurance review"));

        return result.Accepted ? null : result.Reason switch
        {
            EditRejectionReason.ReadOnly => "The file is read-only.",
            EditRejectionReason.StaleBaseVersion => "The file changed while the review was being recorded.",
            _ => "The edit could not be applied.",
        };
    }

    /// <summary>
    /// The one span in which two texts differ: the common prefix and the common
    /// suffix are trimmed away, and what is left is the replacement.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=722C27
    // Broiler-Falsified-If: applying the returned span to the current text yields a string other than the updated text, for example when the change sits inside a run of repeated characters
    // Broiler-Human:        PENDING
    private static (int Start, int OldLength, string Replacement) Narrow(string current, string updated)
    {
        int prefix = 0;
        int shortest = Math.Min(current.Length, updated.Length);
        while (prefix < shortest && current[prefix] == updated[prefix])
            prefix++;

        int suffix = 0;
        while (suffix < shortest - prefix &&
            current[^(suffix + 1)] == updated[^(suffix + 1)])
        {
            suffix++;
        }

        int oldLength = current.Length - prefix - suffix;
        return (prefix, oldLength, updated[prefix..(updated.Length - suffix)]);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8E344F
    // Broiler-Falsified-If: an edit to the open buffer that leaves the caret on the same line is reported as no version change
    // Broiler-Human:        PENDING
    private bool VersionMoved() =>
        _workspace.FindOpenDocument(_current) is { } open && open.Buffer.Current.Version != _documentVersion;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=8C5011
    // Broiler-Falsified-If: the document model is kept after the open buffer's version or the item's path changed, so the next decision is computed from text the buffer no longer holds
    // Broiler-Human:        PENDING
    private void Rebuild()
    {
        if (_current.IsNone ||
            _workspace.FindItem(_current) is not { } item ||
            _workspace.FindOpenDocument(_current) is not { } open)
        {
            _document = null;
            _documentPath = string.Empty;
            _documentVersion = -1;
            return;
        }

        TextSnapshot snapshot = open.Buffer.Current;
        if (_document is not null &&
            snapshot.Version == _documentVersion &&
            string.Equals(item.RelativePath, _documentPath, StringComparison.Ordinal))
        {
            return;
        }

        string text = snapshot.ToString();

        // Only C# carries this format today, and parsing anything else with a C#
        // scanner would report units that are not there. A file the scanner
        // cannot speak for still gets the annotation-text reading, which is
        // language-neutral: a comment is a comment.
        //
        // A file carrying no assurance marker at all is not parsed either, and
        // that is a product decision before it is a cost one. Running the scanner
        // over every C# file in an ordinary repository would fill this pane with
        // declarations nobody has ever assessed, on files that do not take part
        // in the system — and the search that decides it is a substring scan,
        // which is what keeps the parse off the typing path for the files that
        // are not being reviewed.
        IAssuranceUnitScanner? scanner =
            item.RelativePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) &&
            text.Contains(AssuranceVocabulary.AiMarker, StringComparison.Ordinal)
                ? _scanner
                : null;

        _document = AssuranceDocument.Read(text, scanner, item.RelativePath);
        _documentPath = item.RelativePath;
        _documentVersion = snapshot.Version;
    }
}
