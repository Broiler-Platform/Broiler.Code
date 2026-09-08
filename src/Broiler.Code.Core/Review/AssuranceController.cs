using System;
using System.Collections.Generic;
using Broiler.Code.Review.Assurance;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Text;

namespace Broiler.Code.Core.Review;

/// <summary>Why a unit-level review action could not be carried out.</summary>
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
public readonly record struct AssuranceActionResult(AssuranceActionOutcome Outcome, string Message)
{
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
    public AssuranceController(CodeWorkspace workspace, IAssuranceUnitScanner? scanner = null)
    {
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _scanner = scanner;
    }

    /// <summary>Raised when the unit under the caret, or what it records, changed.</summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Who is recording reviews. The same name the file-level review uses, so a
    /// reviewer is one person to this tool rather than two.
    /// </summary>
    public string Reviewer { get; set; } = string.Empty;

    /// <summary>True when a language service supplied the units.</summary>
    public bool HasUnitScanner => _scanner is not null;

    /// <summary>The current file's assurance model, or null when nothing is open.</summary>
    public AssuranceDocument? Document => _document;

    /// <summary>The unit the caret is in, or null.</summary>
    public AssuranceUnit? CurrentUnit { get; private set; }

    /// <summary>Every unit of the current file, for a pane listing them.</summary>
    public IReadOnlyList<AssuranceUnit> Units => _document?.Units ?? [];

    /// <summary>True when the file on screen carries assurance annotations at all.</summary>
    public bool IsAnnotatedFile => _document is { IsAnnotated: true };


    /// <summary>Points the pane at a document and re-reads it.</summary>
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
    public void SetCaretLine(int line)
    {
        int clamped = Math.Max(0, line);
        if (_caretLine == clamped && _document is not null && !VersionMoved())
            return;

        _caretLine = clamped;
        Refresh();
    }

    /// <summary>Re-reads the current document and re-picks the unit.</summary>
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
    private void Watch()
    {
        Unwatch();

        if (_workspace.FindOpenDocument(_current) is { } open)
        {
            _watched = open.Buffer;
            _watched.Changed += OnBufferChanged;
        }
    }

    private void Unwatch()
    {
        if (_watched is not null)
            _watched.Changed -= OnBufferChanged;

        _watched = null;
    }

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
    public AssuranceActionResult Approve() =>
        Act(static (document, unit, reviewer) => document.Approve(unit, reviewer));

    /// <summary>Puts the unit the caret is in back to pending.</summary>
    public AssuranceActionResult Withdraw() =>
        Act(static (document, unit, _) => document.Withdraw(unit));

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

    private bool VersionMoved() =>
        _workspace.FindOpenDocument(_current) is { } open && open.Buffer.Current.Version != _documentVersion;

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
