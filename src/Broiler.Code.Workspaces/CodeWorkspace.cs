// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   37
// Annotated:        37/37
// Exempt:           18
// Human-reviewed:   0/37
// IP risk:          Low
// Security risk:    High
// Criteria:         26/13
// Resource impact:  4/10 max
// Unverified:       37
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Storage;
using Broiler.Code.Workspaces.Text;

namespace Broiler.Code.Workspaces;

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=26878C
// Broiler-Human:        PENDING
public enum SaveOutcomeKind
{
    Saved,

    /// <summary>Nothing to write; the document was already clean.</summary>
    NotDirty,

    /// <summary>The file changed on disk since it was read.</summary>
    Conflict,

    /// <summary>
    /// The document has never been saved, so there is nowhere to write it. The
    /// caller has to ask the user for a location and call
    /// <see cref="CodeWorkspace.SaveDocumentAsAsync"/>. Distinguished from
    /// <see cref="Failed"/> because nothing went wrong.
    /// </summary>
    NeedsLocation,

    Failed,
}

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5A8FEF
// Broiler-Falsified-If: an outcome of kind NeedsLocation, Conflict or Failed reports Succeeded true, so a dirty tab is closed although its text was not written
// Broiler-Human:        PENDING
public sealed record SaveOutcome(
    WorkspaceItemId Id,
    string RelativePath,
    SaveOutcomeKind Kind,
    string? Message = null)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B4996A
    // Broiler-Falsified-If: an outcome of kind NeedsLocation, Conflict or Failed reports Succeeded true, so a dirty tab is closed although its text was not written
    // Broiler-Human:        PENDING
    public bool Succeeded => Kind is SaveOutcomeKind.Saved or SaveOutcomeKind.NotDirty;
}

/// <summary>
/// The result of saving several documents. Every document is attempted and
/// every outcome is reported: a Save All that stops at the first failure leaves
/// the user with some files written and no idea which.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=2; Fingerprint=09C72A
// Broiler-Falsified-If: a report holding a Conflict outcome reports AllSucceeded true or leaves that outcome out of Failures
// Broiler-Human:        PENDING
public sealed record SaveAllReport(IReadOnlyList<SaveOutcome> Outcomes)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=2; Fingerprint=3E55D8
    // Broiler-Falsified-If: a report holding a NeedsLocation or Conflict outcome reports AllSucceeded true
    // Broiler-Human:        PENDING
    public bool AllSucceeded => Outcomes.All(outcome => outcome.Succeeded);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=2; Fingerprint=006B3F
    // Broiler-Falsified-If: a NeedsLocation or Conflict outcome is missing from Failures
    // Broiler-Human:        PENDING
    public IEnumerable<SaveOutcome> Failures => Outcomes.Where(outcome => !outcome.Succeeded);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=2; Fingerprint=9608EA
    // Broiler-Falsified-If: a NotDirty outcome is counted as a saved document
    // Broiler-Human:        PENDING
    public int SavedCount => Outcomes.Count(outcome => outcome.Kind == SaveOutcomeKind.Saved);
}

/// <summary>
/// A workspace-level problem the user has to be told about: a construct the
/// declared model cannot edit, a path outside the grant, a collision.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DBBD52
// Broiler-Human:        PENDING
public sealed record WorkspaceDiagnostic(
    string Code,
    string Message,
    WorkspaceItemId Item = default,
    string? RelativePath = null);

/// <summary>
/// The open workspace: declared solutions and projects, the items they own, and
/// the documents currently open for editing.
///
/// Identity is a <see cref="WorkspaceItemId"/> throughout, never a path. A
/// rename changes an item's path and keeps its ID, so the open tab, the undo
/// history, and the per-user state stay attached to it.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=85D810
// Broiler-Falsified-If: an operation on a document opened through a file-dialog grant reaches the file at the same relative path under the workspace root instead of the granted file
// Broiler-Human:        PENDING
public sealed class CodeWorkspace
{
    private readonly IWorkspaceStorage _storage;
    private readonly WorkspaceIdFactory _ids;
    private readonly Dictionary<WorkspaceItemId, WorkspaceItem> _items = [];
    private readonly Dictionary<WorkspaceItemId, SourceDocument> _open = [];
    private readonly Dictionary<string, WorkspaceItemId> _byPath;

    // Documents the user reached through a file dialog live outside the
    // workspace root. The dialog is the grant, so each carries the storage it
    // was granted through; everything else uses the workspace's own.
    private readonly Dictionary<WorkspaceItemId, IWorkspaceStorage> _grants = [];
    private readonly List<WorkspaceDiagnostic> _diagnostics = [];
    private readonly List<CodeProject> _projects = [];
    private readonly List<CodeSolution> _solutions = [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AFFEC6
    // Broiler-Falsified-If: on a storage provider that reports CaseInsensitivePaths, 'Src/A.cs' and 'src/a.cs' are registered as two items over one file
    // Broiler-Human:        PENDING
    public CodeWorkspace(IWorkspaceStorage storage, WorkspaceIdFactory? ids = null)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _ids = ids ?? new WorkspaceIdFactory();
        _byPath = new Dictionary<string, WorkspaceItemId>(
            WorkspacePath.GetComparer(
                storage.Capabilities.HasFlag(StorageCapabilities.CaseInsensitivePaths)));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E5167B
    // Broiler-Human:        PENDING
    public event Action<SourceDocument>? DocumentOpened;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A11D6F
    // Broiler-Human:        PENDING
    public event Action<WorkspaceItemId>? DocumentClosed;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=54B25B
    // Broiler-Human:        PENDING
    public event Action<WorkspaceItem>? ItemChanged;

    public IWorkspaceStorage Storage => _storage;

    /// <summary>
    /// The storage a document's bytes actually go through: its own grant when it
    /// came from a file dialog, the workspace's otherwise.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A07458
    // Broiler-Falsified-If: a document opened through a file-dialog grant resolves to the workspace's own storage, so its reads and saves use the same relative path under the workspace root
    // Broiler-Human:        PENDING
    public IWorkspaceStorage StorageFor(WorkspaceItemId id) =>
        _grants.TryGetValue(id, out IWorkspaceStorage? granted) ? granted : _storage;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3749A5
    // Broiler-Human:        PENDING
    public IReadOnlyCollection<WorkspaceItem> Items => _items.Values;

    public IReadOnlyList<CodeProject> Projects => _projects;

    public IReadOnlyList<CodeSolution> Solutions => _solutions;

    public IReadOnlyList<WorkspaceDiagnostic> Diagnostics => _diagnostics;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D9B9FC
    // Broiler-Human:        PENDING
    public IReadOnlyCollection<SourceDocument> OpenDocuments => _open.Values;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=2; Fingerprint=FCC1F3
    // Broiler-Falsified-If: a workspace whose only dirty document is untitled or granted reports no unsaved changes
    // Broiler-Human:        PENDING
    public bool HasUnsavedChanges => _open.Values.Any(document => document.IsDirty);

    /// <summary>
    /// Registers an item. A path already registered returns the existing ID, so
    /// the same file discovered through two projects is one item with one
    /// identity rather than two that disagree about whether it is dirty.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=6CF57B
    // Broiler-Falsified-If: a Compile Include that climbs above the workspace root with '..' is registered with a usable ID instead of being refused with BRW0001
    // Broiler-Human:        PENDING
    public WorkspaceItem AddItem(
        string relativePath,
        WorkspaceItemKind kind,
        WorkspaceItemId owningProject = default,
        bool isLinked = false,
        string? linkedAs = null)
    {
        string? normalized = WorkspacePath.Normalize(relativePath);
        if (normalized is null)
        {
            _diagnostics.Add(new WorkspaceDiagnostic(
                "BRW0001",
                $"'{relativePath}' resolves outside the granted workspace roots and was not added.",
                RelativePath: relativePath));
            return new WorkspaceItem
            {
                Id = WorkspaceItemId.None,
                Kind = kind,
                RelativePath = relativePath,
                IsReadOnlyDeclaration = true,
            };
        }

        if (_byPath.TryGetValue(normalized, out WorkspaceItemId existingId))
        {
            WorkspaceItem existing = _items[existingId];

            // A case-only difference on a case-insensitive provider is the same
            // file. Recording it is what stops a later rename from silently
            // clobbering one with the other.
            if (!string.Equals(existing.RelativePath, normalized, StringComparison.Ordinal))
            {
                _diagnostics.Add(new WorkspaceDiagnostic(
                    "BRW0002",
                    $"'{normalized}' and '{existing.RelativePath}' differ only in case and are the " +
                    "same file on this storage provider.",
                    existing.Id,
                    normalized));
            }

            return existing;
        }

        var item = new WorkspaceItem
        {
            Id = _ids.Next(),
            Kind = kind,
            RelativePath = normalized,
            OwningProject = owningProject,
            IsLinked = isLinked,
            LinkedAs = linkedAs,
        };

        _items[item.Id] = item;
        _byPath[normalized] = item.Id;
        return item;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BBB0F9
    // Broiler-Falsified-If: a project whose HasUnsupportedConstructs is set is added without a BRW0100 diagnostic
    // Broiler-Human:        PENDING
    public void AddProject(CodeProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        _projects.Add(project);
        if (project.HasUnsupportedConstructs)
        {
            _diagnostics.Add(new WorkspaceDiagnostic(
                "BRW0100",
                $"'{project.RelativePath}' contains constructs the declared model can read but not " +
                "edit. It stays read-only until a trusted evaluation supplies a target-specific graph.",
                project.Id,
                project.RelativePath));
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=8E8454
    // Broiler-Human:        PENDING
    public void AddSolution(CodeSolution solution)
    {
        ArgumentNullException.ThrowIfNull(solution);
        _solutions.Add(solution);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3FCB7F
    // Broiler-Human:        PENDING
    public WorkspaceItem? FindItem(WorkspaceItemId id) =>
        _items.TryGetValue(id, out WorkspaceItem? item) ? item : null;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=382E6B
    // Broiler-Falsified-If: a path that normalizes to a registered item's path, such as 'src/./A.cs' for 'src/A.cs', returns null instead of that item
    // Broiler-Human:        PENDING
    public WorkspaceItem? FindItem(string relativePath)
    {
        string? normalized = WorkspacePath.Normalize(relativePath);
        return normalized is not null && _byPath.TryGetValue(normalized, out WorkspaceItemId id)
            ? _items[id]
            : null;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2C0ED0
    // Broiler-Human:        PENDING
    public SourceDocument? FindOpenDocument(WorkspaceItemId id) =>
        _open.TryGetValue(id, out SourceDocument? document) ? document : null;

    /// <summary>
    /// Opens a document for editing. Opening one that is already open returns
    /// the existing instance — a second buffer over the same file would give
    /// the user two views that disagree and two undo histories that fight.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=A611DA
    // Broiler-Falsified-If: opening an item that is already open returns a new SourceDocument with a second buffer instead of the open instance
    // Broiler-Human:        PENDING
    public async ValueTask<StorageResult<SourceDocument>> OpenDocumentAsync(
        WorkspaceItemId id, CancellationToken cancellationToken = default)
    {
        if (_open.TryGetValue(id, out SourceDocument? already))
            return StorageResult<SourceDocument>.Ok(already);
        if (!_items.TryGetValue(id, out WorkspaceItem? item))
            return StorageResult<SourceDocument>.Fail(StorageFailureKind.NotFound, id.ToString());

        // An untitled document has no bytes anywhere to read; it exists only in
        // memory until a Save As gives it somewhere to live.
        if (item.IsUntitled)
        {
            return StorageResult<SourceDocument>.Fail(
                StorageFailureKind.NotFound,
                $"'{item.RelativePath}' has never been saved.");
        }

        // RelativePath is where the bytes are, for a linked item as much as for
        // any other. The link is how a project displays it, not where it lives.
        StorageResult<StorageTextContent> read = await StorageFor(id)
            .ReadTextAsync(item.RelativePath, cancellationToken).ConfigureAwait(false);
        if (!read.Succeeded)
            return StorageResult<SourceDocument>.Fail(read.Failure!.Kind, read.Failure.Message);

        StorageTextContent content = read.Value!;
        var buffer = new SourceBuffer(content.Text);
        WorkspaceItem updated = item with
        {
            Encoding = content.Encoding,
            ExternalRevision = content.ExternalRevision,
            DurableFileId = content.DurableFileId,
        };
        _items[id] = updated;

        var document = new SourceDocument(updated, buffer);
        _open[id] = document;
        DocumentOpened?.Invoke(document);
        return StorageResult<SourceDocument>.Ok(document);
    }

    /// <summary>
    /// Creates a document that exists only in memory, open and editable, with
    /// no file behind it. This is what makes an empty editor usable: a caret in
    /// a real buffer rather than a read-only surface waiting for a workspace.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9E1B2F
    // Broiler-Falsified-If: an untitled document named like a workspace file, such as 'Program.cs', is returned by FindItem('Program.cs') in place of the workspace's own item
    // Broiler-Human:        PENDING
    public SourceDocument CreateUntitledDocument(string displayName, string text = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(text);

        var item = new WorkspaceItem
        {
            Id = _ids.Next(),
            Kind = WorkspaceItemKind.SourceDocument,
            RelativePath = displayName,
            IsUntitled = true,
        };

        _items[item.Id] = item;
        _byPath[UntitledKey(displayName)] = item.Id;

        var document = new SourceDocument(item, new SourceBuffer(text));
        _open[item.Id] = document;
        DocumentOpened?.Invoke(document);
        return document;
    }

    /// <summary>
    /// A name no untitled document is currently using. Counting open documents
    /// would reuse a name as soon as one is closed, and two tabs called
    /// "Untitled1.cs" are two things the user cannot tell apart.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=1F05B6
    // Broiler-Falsified-If: a name held by an untitled document that is still open is returned again
    // Broiler-Human:        PENDING
    public string NextUntitledName(string extension = ".cs")
    {
        for (int index = 1; ; index++)
        {
            string candidate = $"Untitled{index.ToString(CultureInfo.InvariantCulture)}{extension}";
            if (!_byPath.ContainsKey(UntitledKey(candidate)))
                return candidate;
        }
    }

    /// <summary>
    /// Opens a document through a grant the user gave by choosing it in a file
    /// dialog, which is how a file outside the workspace root is reached. The
    /// grant travels with the document, so a later save writes back through the
    /// same storage rather than resolving against a root that never contained
    /// it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=50FFC1
    // Broiler-Falsified-If: a path that climbs out of the granted directory with '..' is read through the grant instead of failing with OutsideGrant
    // Broiler-Human:        PENDING
    public async ValueTask<StorageResult<SourceDocument>> OpenGrantedDocumentAsync(
        IWorkspaceStorage storage,
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storage);

        string? normalized = WorkspacePath.Normalize(relativePath);
        if (normalized is null)
        {
            return StorageResult<SourceDocument>.Fail(
                StorageFailureKind.OutsideGrant, relativePath);
        }

        // Already open under this grant: the same file reached twice is one
        // document, exactly as it is inside the workspace root.
        string key = GrantKey(storage, normalized);
        if (_byPath.TryGetValue(key, out WorkspaceItemId existing) &&
            _open.TryGetValue(existing, out SourceDocument? already))
        {
            return StorageResult<SourceDocument>.Ok(already);
        }

        StorageResult<StorageTextContent> read = await storage
            .ReadTextAsync(normalized, cancellationToken).ConfigureAwait(false);
        if (!read.Succeeded)
            return StorageResult<SourceDocument>.Fail(read.Failure!.Kind, read.Failure.Message);

        StorageTextContent content = read.Value!;
        var item = new WorkspaceItem
        {
            Id = existing.IsNone ? _ids.Next() : existing,
            Kind = WorkspaceItemKind.SourceDocument,
            RelativePath = normalized,
            Encoding = content.Encoding,
            ExternalRevision = content.ExternalRevision,
            DurableFileId = content.DurableFileId,
        };

        _items[item.Id] = item;
        _byPath[key] = item.Id;
        _grants[item.Id] = storage;

        var document = new SourceDocument(item, new SourceBuffer(content.Text));
        _open[item.Id] = document;
        DocumentOpened?.Invoke(document);
        return StorageResult<SourceDocument>.Ok(document);
    }

    /// <summary>
    /// Writes an open document to a location the user chose, and binds it
    /// there. The item's ID does not change, so the tab, the buffer, and the
    /// undo history follow it — a Save As is a new location for the same
    /// document, not a new document.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=E518E5
    // Broiler-Falsified-If: a Save As onto a path whose key another open document already holds re-points that key to this document without refusing, leaving two open documents over one file
    // Broiler-Human:        PENDING
    public async ValueTask<SaveOutcome> SaveDocumentAsAsync(
        WorkspaceItemId id,
        IWorkspaceStorage storage,
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storage);

        if (!_open.TryGetValue(id, out SourceDocument? document))
            return new SaveOutcome(id, string.Empty, SaveOutcomeKind.Failed, "The document is not open.");

        string? normalized = WorkspacePath.Normalize(relativePath);
        if (normalized is null)
        {
            return new SaveOutcome(
                id, relativePath, SaveOutcomeKind.Failed,
                $"'{relativePath}' resolves outside the granted root.");
        }

        WorkspaceItem item = document.Item;

        // No expected revision: the user picked this path knowing what is
        // there, and the dialog already asked about overwriting.
        StorageResult<string> write = await storage.WriteTextAsync(
            normalized,
            document.Buffer.Current.ToString(),
            item.Encoding,
            expectedRevision: null,
            cancellationToken).ConfigureAwait(false);

        if (!write.Succeeded)
            return new SaveOutcome(id, normalized, SaveOutcomeKind.Failed, write.Failure!.Message);

        // Re-keyed under its new location, and its old key dropped, so the
        // document is findable where it now lives and not where it used to be.
        _byPath.Remove(item.IsUntitled ? UntitledKey(item.RelativePath) : KeyFor(id, item.RelativePath));

        WorkspaceItem updated = item with
        {
            RelativePath = normalized,
            ExternalRevision = write.Value,
            IsUntitled = false,
        };

        if (ReferenceEquals(storage, _storage))
            _grants.Remove(id);
        else
            _grants[id] = storage;

        _items[id] = updated;
        _byPath[KeyFor(id, normalized)] = id;
        document.Item = updated;
        document.Buffer.MarkSaved();
        ItemChanged?.Invoke(updated);
        return new SaveOutcome(id, normalized, SaveOutcomeKind.Saved);
    }

    /// <summary>
    /// Closes a document. A dirty document is refused unless
    /// <paramref name="discardChanges"/> says the user chose to lose them; the
    /// workspace never decides that on its own.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=427A23
    // Broiler-Falsified-If: a dirty document is removed from the open set when discardChanges is false
    // Broiler-Human:        PENDING
    public bool CloseDocument(WorkspaceItemId id, bool discardChanges = false)
    {
        if (!_open.TryGetValue(id, out SourceDocument? document))
            return false;
        if (document.IsDirty && !discardChanges)
            return false;

        _open.Remove(id);
        DocumentClosed?.Invoke(id);
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=FA6A8B
    // Broiler-Falsified-If: an edit accepted while the storage write is still pending is marked saved, so the document reports clean although the file lacks that edit
    // Broiler-Human:        PENDING
    public async ValueTask<SaveOutcome> SaveDocumentAsync(
        WorkspaceItemId id, bool overwriteConflict = false, CancellationToken cancellationToken = default)
    {
        if (!_open.TryGetValue(id, out SourceDocument? document))
            return new SaveOutcome(id, string.Empty, SaveOutcomeKind.Failed, "The document is not open.");

        // An untitled document has nowhere to be written. Saying so is the
        // caller's cue to ask for a location; writing it under its display name
        // would put a file called "Untitled1.cs" somewhere nobody chose.
        if (document.Item.IsUntitled)
        {
            return new SaveOutcome(
                id, document.Item.RelativePath, SaveOutcomeKind.NeedsLocation,
                "The document has never been saved.");
        }

        if (!document.IsDirty)
            return new SaveOutcome(id, document.Item.RelativePath, SaveOutcomeKind.NotDirty);

        WorkspaceItem item = document.Item;

        StorageResult<string> write = await StorageFor(id).WriteTextAsync(
            item.RelativePath,
            document.Buffer.Current.ToString(),
            item.Encoding,
            overwriteConflict ? null : item.ExternalRevision,
            cancellationToken).ConfigureAwait(false);

        if (!write.Succeeded)
        {
            SaveOutcomeKind kind = write.Failure!.Kind == StorageFailureKind.Conflict
                ? SaveOutcomeKind.Conflict
                : SaveOutcomeKind.Failed;
            return new SaveOutcome(id, item.RelativePath, kind, write.Failure.Message);
        }

        WorkspaceItem updated = item with { ExternalRevision = write.Value };
        _items[id] = updated;
        document.Item = updated;
        document.Buffer.MarkSaved();
        ItemChanged?.Invoke(updated);
        return new SaveOutcome(id, updated.RelativePath, SaveOutcomeKind.Saved);
    }

    /// <summary>
    /// Saves every dirty document, attempting all of them. Text is never
    /// dropped: a document whose save failed stays dirty and stays open.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=59626B
    // Broiler-Falsified-If: a document whose save failed or conflicted stops Save All, so dirty documents after it are neither attempted nor reported
    // Broiler-Human:        PENDING
    public async ValueTask<SaveAllReport> SaveAllAsync(CancellationToken cancellationToken = default)
    {
        var outcomes = new List<SaveOutcome>();
        foreach (WorkspaceItemId id in _open.Keys.ToArray())
        {
            cancellationToken.ThrowIfCancellationRequested();
            outcomes.Add(await SaveDocumentAsync(id, cancellationToken: cancellationToken)
                .ConfigureAwait(false));
        }

        return new SaveAllReport(outcomes);
    }

    /// <summary>
    /// Renames an item, keeping its identity. The ID does not change, so an
    /// open tab, its undo history, and its per-user state follow the file.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=806AE0
    // Broiler-Falsified-If: renaming a document opened through a file-dialog grant renames the file at the same relative path under the workspace root instead of the granted file
    // Broiler-Human:        PENDING
    public async ValueTask<StorageResult<WorkspaceItem>> RenameItemAsync(
        WorkspaceItemId id, string newRelativePath, CancellationToken cancellationToken = default)
    {
        if (!_items.TryGetValue(id, out WorkspaceItem? item))
            return StorageResult<WorkspaceItem>.Fail(StorageFailureKind.NotFound, id.ToString());

        string? normalized = WorkspacePath.Normalize(newRelativePath);
        if (normalized is null)
        {
            return StorageResult<WorkspaceItem>.Fail(
                StorageFailureKind.OutsideGrant,
                $"'{newRelativePath}' resolves outside the granted workspace roots.");
        }

        bool caseOnly = _byPath.Comparer.Equals(normalized, item.RelativePath);
        if (!caseOnly && _byPath.ContainsKey(normalized))
        {
            return StorageResult<WorkspaceItem>.Fail(
                StorageFailureKind.Conflict, $"'{normalized}' already exists in the workspace.");
        }

        if (!_storage.Capabilities.HasFlag(StorageCapabilities.Rename))
        {
            return StorageResult<WorkspaceItem>.Fail(
                StorageFailureKind.Unsupported, "This storage provider cannot rename.");
        }

        StorageResult<StorageEntry> renamed = await _storage
            .RenameAsync(item.RelativePath, normalized, cancellationToken).ConfigureAwait(false);
        if (!renamed.Succeeded)
            return StorageResult<WorkspaceItem>.Fail(renamed.Failure!.Kind, renamed.Failure.Message);

        _byPath.Remove(item.RelativePath);
        WorkspaceItem updated = item with
        {
            RelativePath = normalized,
            ExternalRevision = renamed.Value!.ExternalRevision,
        };
        _items[id] = updated;
        _byPath[normalized] = id;
        if (_open.TryGetValue(id, out SourceDocument? open))
            open.Item = updated;

        ItemChanged?.Invoke(updated);
        return StorageResult<WorkspaceItem>.Ok(updated);
    }

    /// <summary>
    /// Re-reads an item's revision and reports whether it changed underneath
    /// the open document. Checked before a save and when the window regains
    /// focus; a provider with change notification calls it from the event.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=076D96
    // Broiler-Falsified-If: for a document opened through a file-dialog grant the revision is read from the same relative path under the workspace root instead of from the granted file
    // Broiler-Human:        PENDING
    public async ValueTask<bool> HasExternalChangeAsync(
        WorkspaceItemId id, CancellationToken cancellationToken = default)
    {
        if (!_items.TryGetValue(id, out WorkspaceItem? item) || item.ExternalRevision is null)
            return false;

        StorageResult<StorageEntry> stat = await _storage
            .StatAsync(item.RelativePath, cancellationToken).ConfigureAwait(false);
        if (!stat.Succeeded)
            return false;

        return !string.Equals(stat.Value!.ExternalRevision, item.ExternalRevision, StringComparison.Ordinal);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A6B429
    // Broiler-Human:        PENDING
    public void AddDiagnostic(WorkspaceDiagnostic diagnostic)
    {
        ArgumentNullException.ThrowIfNull(diagnostic);
        _diagnostics.Add(diagnostic);
    }

    /// <summary>
    /// The lookup key for an item's path. Items under the workspace root key on
    /// the relative path alone, which is what makes that path their identity.
    /// A granted item has to include its root as well: two files called
    /// "Program.cs" in two granted directories are two documents, and keying
    /// them the same would silently make them one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=EA8EF3
    // Broiler-Falsified-If: an item opened through a grant is keyed on its bare relative path, so it collides with the workspace item at the same relative path
    // Broiler-Human:        PENDING
    private string KeyFor(WorkspaceItemId id, string normalizedPath) =>
        _grants.TryGetValue(id, out IWorkspaceStorage? granted)
            ? GrantKey(granted, normalizedPath)
            : normalizedPath;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=322DBF
    // Broiler-Falsified-If: two grants whose storages list different first roots produce the same key for one relative path, so opening the second returns the first's open document
    // Broiler-Human:        PENDING
    private static string GrantKey(IWorkspaceStorage storage, string normalizedPath) =>
        storage.GrantedRoots.Count == 0
            ? normalizedPath
            : $" grant {storage.GrantedRoots[0]} {normalizedPath}";

    // A separate namespace from any real path, so an untitled document called
    // "Program.cs" never collides with the workspace's own Program.cs.
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F83AF9
    // Broiler-Falsified-If: the key of an untitled document named 'Program.cs' equals the key of the workspace file 'Program.cs' or of a granted file, so one is found in place of the other
    // Broiler-Human:        PENDING
    private static string UntitledKey(string displayName) => $" untitled {displayName}";
}
