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
// Security risk:    High
// Criteria:         11/7
// Resource impact:  5/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Recovery;
using Broiler.UI.CodeEditor;
using Broiler.UI.TabView;

namespace Broiler.Code.Core.Shell;

/// <summary>What a host decided when asked about a dirty document.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0F3991
// Broiler-Human:        PENDING
public enum DirtyCloseChoice
{
    Save,
    Discard,
    Cancel,
}

/// <summary>
/// Asks the user what to do about unsaved changes. The coordinator never
/// decides this: losing someone's work is not a default.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A2BD04
// Broiler-Falsified-If: an implementation answers Save or Discard for a dialog the user dismissed without choosing either
// Broiler-Human:        PENDING
public delegate ValueTask<DirtyCloseChoice> DirtyClosePrompt(
    IReadOnlyList<SourceDocument> dirtyDocuments, CancellationToken cancellationToken);

/// <summary>
/// Binds the workspace's documents to one editor and one tab strip.
///
/// The single editor control is reused across tabs rather than one editor per
/// document — that is what keeps a hundred open files from costing a hundred
/// controls. The consequence is that the control's view state is destroyed on
/// every switch, so the coordinator saves and restores caret, selection, and
/// scroll per document. A user who returns to a tab expects the line they left.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E40A07
// Broiler-Falsified-If: a dirty document is closed and its unsaved text dropped although the prompt did not answer Discard
// Broiler-Human:        PENDING
public sealed class DocumentCoordinator : IDisposable
{
    private readonly CodeWorkspace _workspace;
    private readonly UiCodeEditor _editor;
    private readonly UiTabView _tabs;
    private readonly RecoveryJournal? _journal;
    private readonly Dictionary<string, WorkspaceItemId> _tabToItem = [];
    private readonly Dictionary<WorkspaceItemId, SourceBufferDocument> _documents = [];
    private WorkspaceItemId _active = WorkspaceItemId.None;
    private bool _disposed;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=ED65BE
    // Broiler-Human:        PENDING
    public DocumentCoordinator(
        CodeWorkspace workspace,
        UiCodeEditor editor,
        UiTabView tabs,
        RecoveryJournal? journal = null)
    {
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _editor = editor ?? throw new ArgumentNullException(nameof(editor));
        _tabs = tabs ?? throw new ArgumentNullException(nameof(tabs));
        _journal = journal;

        _tabs.SelectionChanged += OnTabSelectionChanged;
        _tabs.CloseRequested += OnCloseRequested;
    }

    /// <summary>Asked before a dirty document is closed. Cancels if unset.</summary>
    public DirtyClosePrompt? DirtyClosePrompt { get; set; }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=324218
    // Broiler-Human:        PENDING
    public WorkspaceItemId ActiveDocument => _active;

    /// <summary>
    /// Opens a document and activates its tab. A document that is already open
    /// activates its existing tab and buffer rather than opening a second one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=9F57C0
    // Broiler-Falsified-If: opening an item that already has a tab creates a second tab or a second buffer adapter for the same document
    // Broiler-Human:        PENDING
    public async ValueTask<bool> OpenAsync(WorkspaceItemId id, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        string tabId = TabIdFor(id);
        if (_tabs.FindTab(tabId) is not null)
        {
            _tabs.SelectTab(tabId);
            return true;
        }

        Workspaces.Storage.StorageResult<SourceDocument> opened =
            await _workspace.OpenDocumentAsync(id, cancellationToken).ConfigureAwait(false);
        if (!opened.Succeeded)
            return false;

        SourceDocument document = opened.Value!;
        var adapter = new SourceBufferDocument(document.Buffer);
        _documents[id] = adapter;
        _tabToItem[tabId] = id;

        document.Buffer.Changed += (_, _) => OnBufferChanged(id);

        UiTabItem tab = _tabs.AddTab(tabId, document.Item.Name);
        tab.IsDirty = document.IsDirty;
        _tabs.SelectTab(tabId);

        // Activated explicitly rather than relying on the selection event.
        // Adding the first tab selects it without raising SelectionChanged —
        // there was no previous selection to change from — so a coordinator
        // that only listened would leave the editor bound to nothing for the
        // very first document opened.
        ActivateSelectedTab();
        return true;
    }

    /// <summary>
    /// Closes a document, asking about unsaved changes first. Returns false
    /// when the user cancelled or a save failed — the tab stays open either
    /// way, because closing it would discard the very text they declined to
    /// lose.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=0BED20
    // Broiler-Falsified-If: a dirty document is closed and its unsaved text dropped when the prompt answered anything other than Discard, including an unset prompt or a Save that failed
    // Broiler-Human:        PENDING
    public async ValueTask<bool> CloseAsync(WorkspaceItemId id, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        SourceDocument? document = _workspace.FindOpenDocument(id);
        if (document is null)
            return false;

        if (document.IsDirty)
        {
            DirtyCloseChoice choice = DirtyClosePrompt is null
                ? DirtyCloseChoice.Cancel
                : await DirtyClosePrompt([document], cancellationToken).ConfigureAwait(false);

            switch (choice)
            {
                case DirtyCloseChoice.Cancel:
                    return false;
                case DirtyCloseChoice.Save:
                    SaveOutcome outcome = await _workspace
                        .SaveDocumentAsync(id, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (!outcome.Succeeded)
                        return false;
                    break;
            }
        }

        if (!_workspace.CloseDocument(id, discardChanges: true))
            return false;

        string tabId = TabIdFor(id);
        _tabs.RemoveTab(tabId);
        _tabToItem.Remove(tabId);
        if (_documents.Remove(id, out SourceBufferDocument? adapter))
            adapter.Dispose();
        _journal?.Forget(id);

        if (_active == id)
            _active = WorkspaceItemId.None;
        ActivateSelectedTab();
        return true;
    }

    /// <summary>
    /// Closes every document. Returns false as soon as one is cancelled, and
    /// leaves everything still open still open — a half-closed workspace is a
    /// state the user did not ask for.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=B97F3F
    // Broiler-Falsified-If: documents after one whose close was cancelled or whose save failed are still closed
    // Broiler-Human:        PENDING
    public async ValueTask<bool> CloseAllAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        foreach (WorkspaceItemId id in _tabToItem.Values.ToArray())
        {
            if (!await CloseAsync(id, cancellationToken).ConfigureAwait(false))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Retitles a document's tab after its path changed. The tab's ID is the
    /// item ID, not the path, so a rename or a Save As moves the label without
    /// disturbing the tab, its buffer, or its undo history.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9C22B1
    // Broiler-Falsified-If: after a Save As into another directory the tab header shows a directory segment or the old name instead of the new file name
    // Broiler-Human:        PENDING
    public bool RenameTab(WorkspaceItemId id, string newRelativePath)
    {
        ThrowIfDisposed();
        if (_tabs.FindTab(TabIdFor(id)) is not { } tab)
            return false;

        int slash = newRelativePath.LastIndexOf('/');
        tab.Header = slash < 0 ? newRelativePath : newRelativePath[(slash + 1)..];
        tab.IsDirty = _workspace.FindOpenDocument(id)?.IsDirty ?? false;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=15A4AD
    // Broiler-Falsified-If: a document whose save failed has its recovery journal entry deleted or its tab shown as clean
    // Broiler-Human:        PENDING
    public async ValueTask<SaveAllReport> SaveAllAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        SaveAllReport report = await _workspace.SaveAllAsync(cancellationToken).ConfigureAwait(false);
        foreach (SaveOutcome outcome in report.Outcomes.Where(o => o.Kind == SaveOutcomeKind.Saved))
        {
            _journal?.Forget(outcome.Id);
            if (_tabs.FindTab(TabIdFor(outcome.Id)) is { } tab)
                tab.IsDirty = false;
        }

        return report;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=096E33
    // Broiler-Human:        PENDING
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _tabs.SelectionChanged -= OnTabSelectionChanged;
        _tabs.CloseRequested -= OnCloseRequested;
        foreach (SourceBufferDocument adapter in _documents.Values)
            adapter.Dispose();
        _documents.Clear();
    }

    private void OnTabSelectionChanged(object? sender, UiTabSelectionChangedEventArgs e) =>
        ActivateSelectedTab();

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=DC7F01
    // Broiler-Falsified-If: a close requested from the tab strip on a dirty document removes the tab without going through the dirty-close prompt
    // Broiler-Human:        PENDING
    private void OnCloseRequested(object? sender, UiTabCloseRequestedEventArgs e)
    {
        if (_tabToItem.TryGetValue(e.Id, out WorkspaceItemId id))
        {
            // Fire and forget: the prompt is asynchronous and the control is
            // not waiting on an answer. The tab stays until CloseAsync removes
            // it, which is exactly the request-shaped behaviour ADR 0024 wants.
            _ = CloseAsync(id);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7EDCB4
    // Broiler-Falsified-If: after a tab switch the editor is bound to a document other than the selected tab's, so keystrokes edit a file the user is not looking at
    // Broiler-Human:        PENDING
    private void ActivateSelectedTab()
    {
        if (_disposed)
            return;

        // The outgoing document's view state is captured before the editor is
        // pointed anywhere else, or it is gone.
        CaptureViewState();

        UiTabItem? selected = _tabs.SelectedTab;
        if (selected is null || !_tabToItem.TryGetValue(selected.Id, out WorkspaceItemId id))
        {
            _active = WorkspaceItemId.None;
            _editor.Document = null;
            return;
        }

        _active = id;
        _editor.Document = _documents.TryGetValue(id, out SourceBufferDocument? adapter) ? adapter : null;
        RestoreViewState(id);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=537B64
    // Broiler-Falsified-If: the outgoing document's caret and scroll position are recorded on the incoming document
    // Broiler-Human:        PENDING
    private void CaptureViewState()
    {
        if (_active.IsNone || _workspace.FindOpenDocument(_active) is not { } document)
            return;

        document.ViewState = new DocumentViewState(
            _editor.Selection.Anchor,
            _editor.Selection.Focus,
            _editor.Viewport.FirstVisibleLine,
            _editor.Viewport.HorizontalOffset);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F339C1
    // Broiler-Human:        PENDING
    private void RestoreViewState(WorkspaceItemId id)
    {
        if (_workspace.FindOpenDocument(id) is not { } document)
            return;

        DocumentViewState state = document.ViewState;
        _editor.Selection = new CodeSelection(state.SelectionAnchor, state.SelectionFocus);
        _editor.Viewport = _editor.Viewport with
        {
            FirstVisibleLine = state.FirstVisibleLine,
            HorizontalOffset = state.HorizontalOffset,
        };
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=6EDB4E
    // Broiler-Falsified-If: an edit made while the previous journal write for the same document is still in flight leaves the journal without that edit's text
    // Broiler-Human:        PENDING
    private void OnBufferChanged(WorkspaceItemId id)
    {
        if (_disposed)
            return;

        SourceDocument? document = _workspace.FindOpenDocument(id);
        if (document is null)
            return;

        if (_tabs.FindTab(TabIdFor(id)) is { } tab)
            tab.IsDirty = document.IsDirty;

        // Journalled on every change rather than on a timer: a crash between
        // ticks is exactly when recovery matters, and the write is bounded by
        // the document rather than by the workspace.
        if (_journal is not null && document.IsDirty)
        {
            _ = _journal.RecordAsync(id, document.Item, document.Buffer.Current.ToString())
                .AsTask();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=239B87
    // Broiler-Human:        PENDING
    private static string TabIdFor(WorkspaceItemId id) => $"doc:{id.Value}";

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0A45A6
    // Broiler-Human:        PENDING
    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);
}
