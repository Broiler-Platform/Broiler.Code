// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// HAIRS Code Assurance
// --------------------
// Relevant units:   35
// Annotated:        35/35
// Exempt:           20
// Human-reviewed:   0/35
// IP risk:          Low
// Security risk:    High
// Criteria:         28/19
// Resource impact:  7/10 max
// Unverified:       35
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Hairs;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Storage;
using Broiler.UI;

namespace Broiler.Code.Core.Review;

/// <summary>Why a review action could not be carried out.</summary>
// Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=FD1BF3
// Hairs-Human:        PENDING
public enum ReviewActionOutcome
{
    Applied = 0,

    /// <summary>
    /// The action had nothing valid to act on: no file selected, no reviewer
    /// name recorded, or empty text where a note or an answer was required.
    /// </summary>
    NoTarget,

    /// <summary>
    /// The document has unsaved changes. Refused rather than recorded — see
    /// <see cref="ReviewController.RecordDecisionAsync"/>.
    /// </summary>
    DocumentIsDirty,

    /// <summary>The record could not be written.</summary>
    StorageFailed,
}

/// <summary>The result of a review action, with a sentence the status line can show.</summary>
// Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E2CAF9
// Hairs-Falsified-If: a result whose outcome is DocumentIsDirty, StorageFailed or NoTarget reports Succeeded as true
// Hairs-Human:        PENDING
public readonly record struct ReviewActionResult(ReviewActionOutcome Outcome, string Message)
{
    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F63AD5
    // Hairs-Falsified-If: a result whose outcome is DocumentIsDirty, StorageFailed or NoTarget reports Succeeded as true
    // Hairs-Human:        PENDING
    public bool Succeeded => Outcome == ReviewActionOutcome.Applied;
}

/// <summary>
/// The review half of the shell: which file is being reviewed, what is recorded
/// about it, and what is recorded about every other file in the workspace.
///
/// It sits in Core for the same reason the rest of the shell coordination does —
/// Core is the one assembly that knows both about a workspace and about what is
/// on screen. <see cref="Hairs"/> below it knows neither, which is
/// what lets the CI tool compute the same numbers with no display attached.
///
/// The whole-workspace map is kept in memory and refreshed as a unit. The
/// explorer asks for a decoration once per visible row per repaint, and a
/// per-row storage read would put a file read on that path; the map makes it a
/// dictionary lookup.
/// </summary>
// Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=655D40
// Hairs-Falsified-If: a decision other than clearing, taken on a document with unsaved changes, is written to the store with a hash of the unsaved buffer text
// Hairs-Human:        PENDING
public sealed class ReviewController : IDisposable
{
    private readonly CodeWorkspace _workspace;
    private readonly ReviewStore _store;
    private readonly IRevisionProvider _revisions;
    private readonly IUiDispatcher? _dispatcher;
    private readonly Dictionary<string, FileReview> _records = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ReviewState> _states = new(StringComparer.Ordinal);

    /// <summary>
    /// When each path was last committed, against <see cref="_writeSequence"/>.
    ///
    /// <see cref="LoadAsync"/> reads a snapshot of every record and then publishes
    /// it whole. A decision recorded while that read is in flight is newer than the
    /// snapshot, so republishing the snapshot over it rolls it back: the record
    /// stays right on disk while the explorer badge and the coverage number quietly
    /// forget it, until something else happens to rebuild the map. Attaching a
    /// workspace and immediately reviewing the first file is exactly that order,
    /// and the load it races reads no records at all, so the state does not come
    /// back on its own.
    ///
    /// A load remembers the sequence it started at, and <see cref="Publish"/> keeps
    /// the live entry for any path stamped after it. One entry per path ever
    /// committed, which is the same order as <see cref="_records"/> itself.
    /// </summary>
    private readonly Dictionary<string, long> _committedAt = new(StringComparer.Ordinal);
    private long _writeSequence;
    private WorkspaceItemId _current = WorkspaceItemId.None;
    private bool _disposed;

    /// <param name="dispatcher">
    /// Marshals a completed background load onto the UI thread. A caller that
    /// passes none must drive this controller from one thread only — which the
    /// tests do, and a real host does not.
    /// </param>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=43A04B
    // Hairs-Falsified-If: a controller built without a revision provider fails when a decision is recorded instead of recording no revision
    // Hairs-Human:        PENDING
    public ReviewController(
        CodeWorkspace workspace,
        IRevisionProvider? revisions = null,
        string reviewer = "",
        IUiDispatcher? dispatcher = null)
    {
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _store = new ReviewStore(new WorkspaceHairsStorage(workspace.Storage));
        _revisions = revisions ?? NoRevisionProvider.Instance;
        _dispatcher = dispatcher;
        Reviewer = reviewer;
    }

    /// <summary>Raised when a record changed, so the panes and the explorer redraw.</summary>
    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=8C487F
    // Hairs-Human:        PENDING
    public event EventHandler? Changed;

    /// <summary>
    /// Who is recording reviews. Empty until a host supplies one, and a decision
    /// cannot be recorded without it: an approval with no name attached is not
    /// evidence of anything.
    /// </summary>
    public string Reviewer { get; set; }

    /// <summary>The file the review pane is showing, or None.</summary>
    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C46AA6
    // Hairs-Human:        PENDING
    public WorkspaceItemId CurrentDocument => _current;

    /// <summary>The record for the current file. Never null once a file is selected.</summary>
    public FileReview CurrentReview { get; private set; } = FileReview.Empty(string.Empty);

    /// <summary>The current file's state, evaluated against what is in its buffer.</summary>
    public ReviewState CurrentState { get; private set; } = ReviewState.None;

    /// <summary>
    /// The current file's notes, placed against its current content. Recomputed
    /// on every refresh rather than stored, because a note's line is a function
    /// of the text and storing it would make it wrong after the first edit.
    /// </summary>
    public IReadOnlyList<AnchoredNote> CurrentNotes { get; private set; } = [];

    /// <summary>
    /// Records that exist on disk and could not be read, usually because a merge
    /// left conflict markers in one.
    ///
    /// Surfaced rather than skipped: an unreadable record makes its file look
    /// unreviewed, which silently turns somebody's approval into "needs review"
    /// and lowers the coverage number for a reason nobody can see. The shell
    /// reports these on the status line after a load.
    /// </summary>
    public IReadOnlyList<StorageFailure> Unreadable { get; private set; } = [];

    /// <summary>
    /// Reads every record in the workspace and evaluates it. Call once after
    /// attaching a workspace.
    ///
    /// Every reviewed file is read here, including the closed ones. That is the
    /// expensive part and it is why this is asynchronous and why the shell runs
    /// it off the UI thread: without it a workspace that has just opened has
    /// nothing open in it, every record would evaluate against no content, and
    /// the explorer would badge a fully reviewed repository as "review state
    /// unknown" while the command-line tool reported it as reviewed. The two
    /// must agree — they share <see cref="ReviewStateEvaluator"/> precisely so
    /// that they cannot disagree about the rule, and feeding it different
    /// content would defeat that as thoroughly as a second implementation.
    ///
    /// The result is applied through <see cref="IUiDispatcher"/> when the host
    /// supplied one, because everything that reads the maps — the explorer, the
    /// pane, the commands — runs on the UI thread.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=709745
    // Hairs-Falsified-If: a decision committed while LoadAsync is still reading records is missing from StateFor after the load publishes
    // Hairs-Human:        PENDING
    public async ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        // Read before the records are, so anything committed from here on counts as
        // newer than this snapshot even if it lands while the reads are still going.
        long startedAt = Interlocked.Read(ref _writeSequence);

        ReviewStore.ReviewRecordSet all = await _store
            .ReadAllAsync(cancellationToken).ConfigureAwait(false);

        // Built into locals rather than into the live maps, so a load in flight
        // never leaves the explorer reading a half-filled dictionary.
        int count = all.Reviews.Count;
        var records = new Dictionary<string, FileReview>(count, StringComparer.Ordinal);
        var states = new Dictionary<string, ReviewState>(count, StringComparer.Ordinal);

        foreach ((string path, FileReview review) in all.Reviews)
        {
            cancellationToken.ThrowIfCancellationRequested();
            records[path] = review;
            states[path] = ReviewStateEvaluator.Evaluate(
                review, await ContentOfAsync(path, cancellationToken).ConfigureAwait(false));
        }

        Publish(records, states, [.. all.Unreadable.Select(static failure => new StorageFailure((StorageFailureKind)(int)failure.Kind, failure.Message))], startedAt);
    }

    /// <summary>
    /// Swaps in a completed load and tells the panes, on the UI thread.
    ///
    /// Re-posted rather than assumed: <see cref="LoadAsync"/> finishes on
    /// whichever thread its last read completed on, and mutating the maps there
    /// would race every repaint. This is the same arrangement
    /// <see cref="CodeAnalysisController"/> uses for a completed classification,
    /// and for the same reason.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=0748C6
    // Hairs-Falsified-If: a review cleared after the load began has its old state back in StateFor once the load's snapshot is swapped in
    // Hairs-Human:        PENDING
    private void Publish(
        Dictionary<string, FileReview> records,
        Dictionary<string, ReviewState> states,
        IReadOnlyList<StorageFailure> unreadable,
        long loadStartedAt)
    {
        if (_dispatcher is { } dispatcher && !dispatcher.CheckAccess())
        {
            dispatcher.Post(() => Publish(records, states, unreadable, loadStartedAt));
            return;
        }

        // The workspace may have closed while the records were being read. The
        // controller is then already detached from the panes, and applying a
        // result to it would only resurrect state nobody is showing.
        if (_disposed)
            return;

        // What the snapshot cannot speak for: paths committed since the read began.
        // Captured before the swap, because the swap is what would lose them.
        //
        // Absence is captured too. Clearing a review removes the path from both
        // maps, and a snapshot taken before that still holds the old record — so a
        // path with no live entry must come out of the swap with no entry either,
        // rather than having its withdrawn approval put back.
        var committedSince = new List<string>();
        foreach ((string path, long at) in _committedAt)
        {
            if (at > loadStartedAt)
                committedSince.Add(path);
        }

        var liveRecords = new Dictionary<string, FileReview>(committedSince.Count, StringComparer.Ordinal);
        var liveStates = new Dictionary<string, ReviewState>(committedSince.Count, StringComparer.Ordinal);
        foreach (string path in committedSince)
        {
            if (_records.TryGetValue(path, out FileReview? record))
                liveRecords[path] = record;
            if (_states.TryGetValue(path, out ReviewState state))
                liveStates[path] = state;
        }

        _records.Clear();
        foreach ((string path, FileReview review) in records)
            _records[path] = review;

        _states.Clear();
        foreach ((string path, ReviewState state) in states)
            _states[path] = state;

        foreach (string path in committedSince)
        {
            _records.Remove(path);
            _states.Remove(path);

            if (liveRecords.TryGetValue(path, out FileReview? record))
                _records[path] = record;
            if (liveStates.TryGetValue(path, out ReviewState state))
                _states[path] = state;
        }

        Unreadable = unreadable;

        // After the restore above, so the current document is re-evaluated against
        // the records that survive rather than the ones the snapshot would impose.
        RefreshCurrent();
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// The content to evaluate a record against: the open buffer when there is
    /// one, so a file being edited is judged by what the reviewer can see, and
    /// otherwise the bytes on storage.
    ///
    /// Reading the buffer from a background thread is safe because
    /// <c>SourceBuffer.Current</c> is a reference to an immutable snapshot — the
    /// read either sees the old snapshot or the new one, never a torn one, and
    /// either answer is a state this file legitimately had.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=B107F6
    // Hairs-Falsified-If: a document opened or closed on the UI thread while a background load evaluates records makes this lookup throw or miss the open buffer
    // Hairs-Human:        PENDING
    private async ValueTask<string?> ContentOfAsync(
        string relativePath, CancellationToken cancellationToken)
    {
        if (TextOf(relativePath) is { } open)
            return open;

        StorageResult<StorageTextContent> read = await _workspace.Storage
            .ReadTextAsync(relativePath, cancellationToken).ConfigureAwait(false);

        // A record whose file is gone stays Unknown rather than Stale. The file
        // may simply have been deleted, and blaming the reviewer for that would
        // be the wrong report.
        return read.Succeeded ? read.Value!.Text : null;
    }

    /// <summary>Points the review pane at a document.</summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=AE7E48
    // Hairs-Falsified-If: after a document is selected, the next decision is written to the record of the previously selected file
    // Hairs-Human:        PENDING
    public void SetCurrentDocument(WorkspaceItemId id)
    {
        _current = id;
        RefreshCurrent();
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// The state to badge a file with in the explorer. Files with no record
    /// report <see cref="ReviewState.None"/>, which is the honest answer for the
    /// overwhelming majority of a codebase before this tool is used on it.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=06A3F4
    // Hairs-Falsified-If: a path that differs only in case from a path with a recorded state is reported with that state
    // Hairs-Human:        PENDING
    public ReviewState StateFor(string relativePath) =>
        _states.TryGetValue(relativePath, out ReviewState state) ? state : ReviewState.None;

    /// <summary>The record for a path, or an empty one.</summary>
    // Hairs-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=26B92D
    // Hairs-Falsified-If: a path with no record of its own returns the record of another path instead of an empty record for itself
    // Hairs-Human:        PENDING
    public FileReview ReviewFor(string relativePath) =>
        _records.TryGetValue(relativePath, out FileReview? review) ? review : FileReview.Empty(relativePath);

    /// <summary>
    /// Every reviewable item in the workspace, with its state, for a coverage
    /// report.
    ///
    /// The denominator is the workspace's items, not the review directory's
    /// records — a percentage over the files somebody already reviewed would be
    /// 100% by construction and would say nothing at all.
    ///
    /// Project files and configuration count. A .csproj decides what is compiled
    /// and a global.json decides which compiler does it, so both are worth a
    /// human's eyes; excluding them would leave the two files most able to
    /// change what a build produces outside the number. Folders carry no bytes,
    /// and an untitled buffer has never been anywhere a second person could read
    /// it, so neither is counted.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C538FF
    // Hairs-Falsified-If: an untitled buffer or a folder appears in the returned list and so in the coverage denominator
    // Hairs-Human:        PENDING
    public IReadOnlyList<ReviewedFile> Snapshot()
    {
        var files = new List<ReviewedFile>();
        foreach (WorkspaceItem item in _workspace.Items)
        {
            if (item.Kind == WorkspaceItemKind.Folder || item.IsUntitled)
                continue;

            FileReview review = ReviewFor(item.RelativePath);
            files.Add(new ReviewedFile(
                item.RelativePath,
                ReviewCoverage.ComponentOf(item.RelativePath),
                StateFor(item.RelativePath),
                review.Reviewer,
                review.ReviewedAt));
        }

        return files;
    }

    /// <summary>
    /// Records a decision about the current file.
    ///
    /// A dirty document is refused. The record's whole worth is that it names the
    /// exact content a human read, and unsaved text is content nobody else can
    /// fetch — a hash of it would be unverifiable by CI, by a second reviewer, or
    /// by the same reviewer tomorrow. Saving first costs a keystroke; an approval
    /// nobody can check costs the claim.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=5B26D5
    // Hairs-Falsified-If: a decision other than clearing, taken on a document with unsaved changes, is written to the store
    // Hairs-Human:        PENDING
    public async ValueTask<ReviewActionResult> RecordDecisionAsync(
        ReviewStatus status, CancellationToken cancellationToken = default)
    {
        if (Target() is not { } target)
            return new ReviewActionResult(ReviewActionOutcome.NoTarget, "Select a file to review first.");

        // Clearing is exempt, and it is the only decision that is. It records
        // nothing about content, so there is no content for it to be wrong
        // about — and a reviewer who marked the wrong file should not have to
        // save that file to take it back. CodeCommandSet enables and disables
        // the commands on exactly this rule; the two must agree, or the menu
        // offers something this refuses.
        if (status != ReviewStatus.Unreviewed && target.Document.IsDirty)
        {
            return new ReviewActionResult(
                ReviewActionOutcome.DocumentIsDirty,
                $"Save {target.Item.RelativePath} first — a review records the content on disk.");
        }

        if (string.IsNullOrWhiteSpace(Reviewer) && status != ReviewStatus.Unreviewed)
        {
            return new ReviewActionResult(
                ReviewActionOutcome.NoTarget,
                "Set a reviewer name before recording a review.");
        }

        string? revision = await _revisions.GetCurrentRevisionAsync(cancellationToken).ConfigureAwait(false);
        FileReview updated = ReviewFor(target.Item.RelativePath)
            .WithDecision(status, Reviewer, target.Text, DateTimeOffset.UtcNow, revision);

        return await CommitAsync(updated, Describe(status, target.Item.RelativePath), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Adds a note to the current file, anchored to a line range.
    ///
    /// Unlike a decision this is allowed while the document is dirty. A note is a
    /// question, not an attestation — nothing downstream treats it as proof — and
    /// a reviewer who has to save before writing down what confuses them will
    /// stop writing it down.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=5AFDA7
    // Hairs-Falsified-If: a note is written with an empty or whitespace author when no name is configured
    // Hairs-Human:        PENDING
    public async ValueTask<ReviewActionResult> AddNoteAsync(
        ReviewNoteKind kind,
        string text,
        int startLine,
        int endLine,
        string? symbol = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (Target() is not { } target)
            return new ReviewActionResult(ReviewActionOutcome.NoTarget, "Select a file to add a note to.");

        if (text.Trim().Length == 0)
            return new ReviewActionResult(ReviewActionOutcome.NoTarget, "A note needs some text.");

        // A note is a question rather than an attestation, so it is allowed on a
        // dirty document — but it still has to say who is asking. An anonymous
        // question cannot be answered by whoever wrote it.
        if (string.IsNullOrWhiteSpace(Reviewer))
        {
            return new ReviewActionResult(
                ReviewActionOutcome.NoTarget, "Set a reviewer name before writing a note.");
        }

        FileReview review = ReviewFor(target.Item.RelativePath);
        FileReview updated = review.AddNote(new ReviewNote
        {
            Id = review.NextNoteId(),
            Kind = kind,
            Text = text.Trim(),
            Author = Reviewer,
            CreatedAt = DateTimeOffset.UtcNow,
            Anchor = startLine < 0
                ? ReviewAnchor.File
                : NoteAnchoring.CreateAnchor(target.Text, startLine, endLine, symbol),
        });

        return await CommitAsync(updated, $"Note added to {target.Item.RelativePath}.", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Closes a note with an answer.
    ///
    /// The answer is required. A resolution with no text records that somebody
    /// clicked a button, which is precisely the "pretty tick-box system" this
    /// tool exists instead of.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=975B40
    // Hairs-Falsified-If: a note id that is not in the record is reported as resolved and the record is rewritten
    // Hairs-Human:        PENDING
    public async ValueTask<ReviewActionResult> ResolveNoteAsync(
        string noteId, string answer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(noteId);
        ArgumentNullException.ThrowIfNull(answer);

        if (Target() is not { } target)
            return new ReviewActionResult(ReviewActionOutcome.NoTarget, "Select a file first.");

        if (answer.Trim().Length == 0)
            return new ReviewActionResult(ReviewActionOutcome.NoTarget, "Say what the answer was before resolving.");

        if (string.IsNullOrWhiteSpace(Reviewer))
        {
            return new ReviewActionResult(
                ReviewActionOutcome.NoTarget, "Set a reviewer name before resolving a note.");
        }

        FileReview updated = ReviewFor(target.Item.RelativePath).ReplaceNote(
            noteId,
            note => note with
            {
                Resolution = new ReviewResolution(
                    DateTimeOffset.UtcNow, Reviewer, answer.Trim()),
            });

        return await CommitAsync(updated, $"Note {noteId} resolved.", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Removes a note outright, for one written by mistake.</summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=80DA0A
    // Hairs-Falsified-If: a note id that is not in the record is reported as removed and the record is rewritten
    // Hairs-Human:        PENDING
    public async ValueTask<ReviewActionResult> RemoveNoteAsync(
        string noteId, CancellationToken cancellationToken = default)
    {
        if (Target() is not { } target)
            return new ReviewActionResult(ReviewActionOutcome.NoTarget, "Select a file first.");

        FileReview updated = ReviewFor(target.Item.RelativePath).RemoveNote(noteId);
        return await CommitAsync(updated, $"Note {noteId} removed.", cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Detaches the controller. A load still in flight will find
    /// <see cref="Publish"/> refusing to apply its result, which is why
    /// <c>_disposed</c> is checked there rather than only set here.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=556019
    // Hairs-Falsified-If: a background load that completes after Dispose repopulates StateFor
    // Hairs-Human:        PENDING
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _records.Clear();
        _states.Clear();
        _committedAt.Clear();
        Unreadable = [];
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=656A69
    // Hairs-Falsified-If: the review maps are written and Changed is raised on a thread-pool thread when a decision is recorded with a revision lookup that completes asynchronously
    // Hairs-Human:        PENDING
    private async ValueTask<ReviewActionResult> CommitAsync(
        FileReview updated, string message, CancellationToken cancellationToken)
    {
        Hairs.Storage.StorageResult<bool> written = await _store.WriteAsync(updated, cancellationToken).ConfigureAwait(false);
        if (!written.Succeeded)
        {
            return new ReviewActionResult(
                ReviewActionOutcome.StorageFailed,
                written.Failure?.Message ?? "The review record could not be written.");
        }

        // The in-memory maps follow the write rather than anticipating it, so a
        // refused write leaves the UI showing what is actually on disk.
        //
        // One record changed, so one record is re-evaluated. Rebuilding the
        // whole map here would re-read every reviewed file in the repository
        // for each click of Mark Reviewed.
        if (updated.IsEmpty)
        {
            _records.Remove(updated.Path);
            _states.Remove(updated.Path);
        }
        else
        {
            _records[updated.Path] = updated;
            _states[updated.Path] = ReviewStateEvaluator.Evaluate(
                updated,
                await ContentOfAsync(updated.Path, cancellationToken).ConfigureAwait(true));
        }

        // Stamped after the maps are written, so a load that publishes between the
        // two does not see a stamp promising state that is not there yet.
        _committedAt[updated.Path] = Interlocked.Increment(ref _writeSequence);

        RefreshCurrent();
        Changed?.Invoke(this, EventArgs.Empty);
        return new ReviewActionResult(ReviewActionOutcome.Applied, message);
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=923801
    // Hairs-Falsified-If: a file edited after its record was written keeps its earlier state in StateFor after RefreshCurrent evaluates it as stale
    // Hairs-Human:        PENDING
    private void RefreshCurrent()
    {
        if (_current.IsNone || _workspace.FindItem(_current) is not { } item)
        {
            CurrentReview = FileReview.Empty(string.Empty);
            CurrentState = ReviewState.None;
            CurrentNotes = [];
            return;
        }

        string? text = TextOf(item.RelativePath);
        CurrentReview = ReviewFor(item.RelativePath);
        CurrentState = ReviewStateEvaluator.Evaluate(CurrentReview, text);
        CurrentNotes = text is null ? [] : NoteAnchoring.Place(CurrentReview, text);

        // Push the freshly evaluated state back into the map the explorer reads.
        // The map is otherwise rebuilt only on load and on a recorded decision,
        // so a file edited after being approved would keep a stale "reviewed"
        // badge until something else happened to rebuild it — the pane and the
        // tree would then disagree about the same file, on screen, at once.
        //
        // Only the current document is refreshed this way. It is the only one
        // whose text can have changed since the last rebuild, and re-hashing
        // every open document on every tab switch would put the cost of a large
        // file on a gesture that should be instant.
        if (!CurrentReview.IsEmpty)
            _states[item.RelativePath] = CurrentState;
    }

    /// <summary>
    /// The text of a file as the user currently sees it, or null when it is not
    /// open. A closed file's content is on storage and reading it per repaint is
    /// exactly the cost the state map exists to avoid; its record is shown with
    /// <see cref="ReviewFreshness.Unknown"/> until it is opened.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D9C0BE
    // Hairs-Falsified-If: the text of an open document is returned for a path that names a different file
    // Hairs-Human:        PENDING
    private string? TextOf(string relativePath) =>
        _workspace.FindItem(relativePath) is { } item &&
        _workspace.FindOpenDocument(item.Id) is { } document
            ? document.Buffer.Current.ToString()
            : null;

    // Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=77FAB9
    // Hairs-Falsified-If: a target is returned for a selected item that has no open document
    // Hairs-Human:        PENDING
    private ReviewTarget? Target()
    {
        if (_current.IsNone ||
            _workspace.FindItem(_current) is not { } item ||
            _workspace.FindOpenDocument(_current) is not { } document)
        {
            return null;
        }

        return new ReviewTarget(item, document, document.Buffer.Current.ToString());
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=991041
    // Hairs-Falsified-If: a Question decision is described with the message for a cleared review
    // Hairs-Human:        PENDING
    private static string Describe(ReviewStatus status, string path) => status switch
    {
        ReviewStatus.Reviewed => $"{path} marked reviewed.",
        ReviewStatus.InReview => $"{path} marked in review.",
        ReviewStatus.Question => $"{path} marked as having an open question.",
        ReviewStatus.NeedsChange => $"{path} marked as needing a change.",
        _ => $"Review of {path} cleared.",
    };

    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DF1751
    // Hairs-Human:        PENDING
    private readonly record struct ReviewTarget(WorkspaceItem Item, SourceDocument Document, string Text);

    // Hairs-AI:           Origin=AI; IP=None; Security=High; Resources=5; Fingerprint=AF689C
    // Hairs-Falsified-If: a storage operation forwarded through this adapter drops a failure kind or message returned by the underlying workspace storage
    // Hairs-Human:        PENDING
    private sealed class WorkspaceHairsStorage(IWorkspaceStorage inner) : Hairs.Storage.IHairsStorage
    {
        // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=39D3CD
        // Hairs-Human:        PENDING
        public Hairs.Storage.StorageCapabilities Capabilities =>
            (Hairs.Storage.StorageCapabilities)(int)inner.Capabilities;

        public IReadOnlyList<string> GrantedRoots => inner.GrantedRoots;

        // Hairs-AI:           Origin=AI; IP=None; Security=High; Resources=5; Fingerprint=E23C97
        // Hairs-Falsified-If: a failed read from the workspace storage is returned as Succeeded or with a different failure kind
        // Hairs-Human:        PENDING
        public async ValueTask<Hairs.Storage.StorageResult<Hairs.Storage.StorageTextContent>> ReadTextAsync(
            string relativePath, CancellationToken cancellationToken = default)
        {
            StorageResult<StorageTextContent> result = await inner
                .ReadTextAsync(relativePath, cancellationToken).ConfigureAwait(false);
            if (!result.Succeeded)
                return Fail<Hairs.Storage.StorageTextContent>(result.Failure!);

            StorageTextContent value = result.Value!;
            return Hairs.Storage.StorageResult<Hairs.Storage.StorageTextContent>.Ok(
                new Hairs.Storage.StorageTextContent(
                    value.Text,
                    new Hairs.Storage.TextEncodingInfo(value.Encoding.WebName, value.Encoding.HasByteOrderMark),
                    value.ExternalRevision,
                    value.DurableFileId));
        }

        // Hairs-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=572E2F
        // Hairs-Falsified-If: a write passes an encoding or expected revision different from the caller's, or reports a failed workspace write as Succeeded
        // Hairs-Human:        PENDING
        public async ValueTask<Hairs.Storage.StorageResult<string>> WriteTextAsync(
            string relativePath,
            string text,
            Hairs.Storage.TextEncodingInfo encoding,
            string? expectedRevision,
            CancellationToken cancellationToken = default)
        {
            StorageResult<string> result = await inner
                .WriteTextAsync(
                    relativePath,
                    text,
                    new TextEncodingInfo(encoding.WebName, encoding.HasByteOrderMark),
                    expectedRevision,
                    cancellationToken)
                .ConfigureAwait(false);
            return result.Succeeded
                ? Hairs.Storage.StorageResult<string>.Ok(result.Value!)
                : Fail<string>(result.Failure!);
        }

        // Hairs-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=6EF31E
        // Hairs-Falsified-If: an entry returned by the workspace storage is omitted or has its IsDirectory flag inverted
        // Hairs-Human:        PENDING
        public async ValueTask<Hairs.Storage.StorageResult<IReadOnlyList<Hairs.Storage.StorageEntry>>> ListAsync(
            string relativeDirectory, CancellationToken cancellationToken = default)
        {
            StorageResult<IReadOnlyList<StorageEntry>> result = await inner
                .ListAsync(relativeDirectory, cancellationToken).ConfigureAwait(false);
            if (!result.Succeeded)
                return Fail<IReadOnlyList<Hairs.Storage.StorageEntry>>(result.Failure!);

            var entries = new Hairs.Storage.StorageEntry[result.Value!.Count];
            for (int index = 0; index < entries.Length; index++)
                entries[index] = Entry(result.Value[index]);
            return Hairs.Storage.StorageResult<IReadOnlyList<Hairs.Storage.StorageEntry>>.Ok(entries);
        }

        // Hairs-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=22E3A2
        // Hairs-Falsified-If: a failed stat from the workspace storage is returned as Succeeded
        // Hairs-Human:        PENDING
        public async ValueTask<Hairs.Storage.StorageResult<Hairs.Storage.StorageEntry>> StatAsync(
            string relativePath, CancellationToken cancellationToken = default)
        {
            StorageResult<StorageEntry> result = await inner
                .StatAsync(relativePath, cancellationToken).ConfigureAwait(false);
            return result.Succeeded
                ? Hairs.Storage.StorageResult<Hairs.Storage.StorageEntry>.Ok(Entry(result.Value!))
                : Fail<Hairs.Storage.StorageEntry>(result.Failure!);
        }

        // Hairs-AI:           Origin=AI; IP=None; Security=High; Resources=5; Fingerprint=2E064A
        // Hairs-Falsified-If: a failed delete from the workspace storage is returned as Succeeded
        // Hairs-Human:        PENDING
        public async ValueTask<Hairs.Storage.StorageResult<bool>> DeleteAsync(
            string relativePath, CancellationToken cancellationToken = default)
        {
            StorageResult<bool> result = await inner
                .DeleteAsync(relativePath, cancellationToken).ConfigureAwait(false);
            return result.Succeeded
                ? Hairs.Storage.StorageResult<bool>.Ok(result.Value)
                : Fail<bool>(result.Failure!);
        }

        // Hairs-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E52CB6
        // Hairs-Falsified-If: a failed rename from the workspace storage is returned as Succeeded
        // Hairs-Human:        PENDING
        public async ValueTask<Hairs.Storage.StorageResult<Hairs.Storage.StorageEntry>> RenameAsync(
            string fromRelativePath, string toRelativePath, CancellationToken cancellationToken = default)
        {
            StorageResult<StorageEntry> result = await inner
                .RenameAsync(fromRelativePath, toRelativePath, cancellationToken).ConfigureAwait(false);
            return result.Succeeded
                ? Hairs.Storage.StorageResult<Hairs.Storage.StorageEntry>.Ok(Entry(result.Value!))
                : Fail<Hairs.Storage.StorageEntry>(result.Failure!);
        }

        // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4E8A1D
        // Hairs-Human:        PENDING
        private static Hairs.Storage.StorageEntry Entry(StorageEntry entry) =>
            new(entry.RelativePath, entry.IsDirectory, entry.SizeBytes, entry.ExternalRevision, entry.DurableFileId);

        // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2DD8F4
        // Hairs-Human:        PENDING
        private static Hairs.Storage.StorageResult<T> Fail<T>(StorageFailure failure) =>
            Hairs.Storage.StorageResult<T>.Fail(
                (Hairs.Storage.StorageFailureKind)(int)failure.Kind, failure.Message);
    }
}
