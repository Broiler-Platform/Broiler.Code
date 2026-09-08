using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Core.Diagnostics;
using Broiler.Code.Core.Review;
using Broiler.Code.Review;
using Broiler.Code.Review.Assurance;
using Broiler.Code.Core.Templates;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Storage;
using Broiler.Graphics;
using Broiler.UI;
using Broiler.UI.Button;
using Broiler.UI.CodeEditor;
using Broiler.UI.ComboBox;
using Broiler.UI.Edit;
using Broiler.UI.Label;
using Broiler.UI.Menu;
using Broiler.UI.Panel;
using Broiler.UI.Splitter;
using Broiler.UI.TabView;
using Broiler.UI.Toolbar;
using Broiler.UI.TreeView;

namespace Broiler.Code.Core.Shell;

/// <summary>
/// The controls a host supplies. They are abstractions, so this assembly never
/// references a Standard implementation and a head remains free to substitute
/// its own — which is also what keeps the shell testable without a platform.
/// </summary>
public sealed record CodeShellControls
{
    public required UiPanel Root { get; init; }

    public required UiPanel Body { get; init; }

    public required UiMenu Menu { get; init; }

    public required UiToolbar Toolbar { get; init; }

    public required UiTreeView Explorer { get; init; }

    public required UiSplitter ExplorerSplitter { get; init; }

    public required UiPanel DocumentArea { get; init; }

    public required UiTabView Tabs { get; init; }

    public required UiCodeEditor Editor { get; init; }

    public required UiTreeView Problems { get; init; }

    /// <summary>
    /// The Human Review pane, and the splitter that sizes it.
    ///
    /// Optional, unlike everything above it. A head that supplies neither gets
    /// the shell it had before the review workspace existed, which is what lets
    /// the Android and browser heads adopt it when they are ready rather than
    /// when this file changed. Supplying one and not the other composes the
    /// pane without a splitter, which is a poor experience but not a broken one.
    /// </summary>
    public UiTreeView? Review { get; init; }

    public UiSplitter? ReviewSplitter { get; init; }

    /// <summary>
    /// Holds the review tree and the note field. Supplied separately because a
    /// tree is not a container: without it the review tree is docked on its own
    /// and there is nowhere to type a note.
    /// </summary>
    public UiPanel? ReviewPane { get; init; }

    /// <summary>
    /// Where a new review note is typed.
    ///
    /// A single-line field, which is the honest shape of what Broiler.UI has
    /// today: <c>UiEdit</c> is single-line and <c>UiRichEdit</c> is a formatted
    /// document, not a plain-text box. A note is one or two sentences far more
    /// often than it is a paragraph, so this is a real constraint rather than a
    /// crippling one — and the record format already carries multi-line text, so
    /// a multi-line control later needs no migration.
    /// </summary>
    public UiEdit? ReviewNoteInput { get; init; }

    /// <summary>
    /// Chooses what kind of thing a new note is.
    ///
    /// Optional like the rest of the pane, and the reason it exists is a gap the
    /// review workspace shipped with: the record format carries four note kinds,
    /// reads all four back and renders all four, and the product could only ever
    /// write a Question because there was nowhere to choose. A count of open
    /// notes that cannot distinguish an observation from a concern is a count
    /// that means less than it says.
    ///
    /// A head that supplies none keeps the old behaviour exactly — every note a
    /// Question — rather than losing the note field.
    /// </summary>
    public UiComboBox? ReviewNoteKindInput { get; init; }

    /// <summary>
    /// Records the file-level review decision from inside the pane.
    ///
    /// Optional like the rest of it, and the reason it exists is that the pane
    /// could read a review and not write one: it said "Nothing recorded yet —
    /// mark it reviewed, or add a note" on a row that did nothing, and the four
    /// decisions it was pointing at were reachable only from the Review menu. A
    /// workspace whose whole purpose is recording what a person has read should
    /// not send them to a menu bar to record it.
    ///
    /// It picks a status rather than committing one: the write still goes
    /// through the same named commands the menu drives, so the rule that a
    /// decision needs a reviewer and a saved file is enforced in one place and
    /// the picker snaps back to what was actually recorded when it is refused.
    /// </summary>
    public UiComboBox? ReviewStatusInput { get; init; }

    public required UiLabel Status { get; init; }

    public required UiLabel Output { get; init; }

    /// <summary>
    /// Makes a toolbar button. A factory rather than a list of buttons because
    /// the shell decides which commands the toolbar carries and the host decides
    /// what a button is — the same split as every other control here.
    /// </summary>
    public required Func<UiButton> CreateButton { get; init; }
}

/// <summary>
/// The composed IDE shell: menu, toolbar, Solution Explorer, editor tabs,
/// Problems and Output panes, splitter, and status line.
///
/// This is the piece that turns the tested parts into something on screen. The
/// coordination it drives — document lifetime, view state, save prompts — lives
/// in <see cref="DocumentCoordinator"/>; the tree contents in
/// <see cref="SolutionExplorerSource"/>; the problem rows in
/// <see cref="ProblemsModel"/>. This file is layout, command wiring, and the
/// status text, and deliberately holds no logic those three already own.
/// </summary>
public sealed class CodeShell : IDisposable
{
    /// <summary>
    /// What the toolbar carries, in order. A deliberate subset of the menu: a
    /// toolbar that mirrors every command is a second menu that is harder to
    /// read.
    /// </summary>
    private static readonly string[] ToolbarCommands =
    [
        CodeCommandNames.New,
        CodeCommandNames.Open,

        // The one addition to a deliberately small toolbar. Opening a folder is
        // the first thing a reviewer does and the only way into the tree, and a
        // command reachable only three levels into a menu is one nobody finds.
        CodeCommandNames.OpenFolder,
        CodeCommandNames.Save,
        CodeCommandNames.SaveAll,
        CodeCommandNames.Build,
        CodeCommandNames.Cancel,
    ];

    /// <summary>
    /// What a note can be, in the order a reviewer reaches for them.
    ///
    /// The identifiers are the enum names, so the control's selection maps back
    /// without a second table to keep in step. Question is first because it is
    /// the default and the commonest, and Observation last because it is the one
    /// that asks for nothing — the order is roughly how much of somebody else's
    /// time each one claims.
    /// </summary>
    private static readonly UiComboBoxItem[] NoteKindItems =
    [
        new(nameof(ReviewNoteKind.Question), "Question"),
        new(nameof(ReviewNoteKind.Concern), "Concern"),
        new(nameof(ReviewNoteKind.Todo), "To do"),
        new(nameof(ReviewNoteKind.Observation), "Observation"),
    ];

    /// <summary>
    /// The file-level decisions the pane can record, in the order a review moves
    /// through them: nothing, started, and then one of the three ways it can end.
    ///
    /// The identifiers are the command names rather than the enum names, so the
    /// picker drives exactly what the Review menu drives and there is no second
    /// table deciding what a selection means. Clearing is one of them because
    /// undoing a decision recorded on the wrong file is part of recording
    /// decisions, and it is the entry a reviewer reaches for in a hurry.
    /// </summary>
    private static readonly UiComboBoxItem[] ReviewStatusItems =
    [
        new(CodeCommandNames.ClearReview, "Not reviewed"),
        new(CodeCommandNames.MarkInReview, "In review"),
        new(CodeCommandNames.MarkReviewed, "Reviewed"),
        new(CodeCommandNames.MarkQuestion, "Open question"),
        new(CodeCommandNames.MarkNeedsChange, "Needs change"),
    ];

    /// <summary>
    /// What a grip's reported movement is measured against.
    ///
    /// <see cref="UiSplitter"/> is normalized: it divides the pointer's travel by
    /// <see cref="UiSplitter.DragExtent"/> to produce a value, so multiplying a
    /// reported change back by the same number gives the layout units the grip
    /// moved, whatever the number is — the drag is one-to-one either way. What
    /// the number does decide is the range, because the value is clamped: a pane
    /// can be dragged 0.8 × this away from the width its head chose and no
    /// further, which is wider than any window this runs in.
    ///
    /// A constant rather than the body's own width because the body has no width
    /// until the first frame is arranged, and a grip has to behave on the frame
    /// after that one.
    /// </summary>
    private const double SplitterDragExtent = 2400;

    /// <summary>The narrowest a pane may be dragged. Below this it is a strip nobody can read.</summary>
    private const double MinimumPaneWidth = 120;

    /// <summary>How much editor a pane has to leave behind it, however far its grip is dragged.</summary>
    private const double MinimumDocumentWidth = 240;

    private readonly CodeShellControls _controls;
    private readonly Dictionary<string, UiButton> _toolbarButtons = [];
    private readonly ProblemsModel _problems = new();
    private readonly ProblemsTreeSource _problemsSource;
    private ReviewController? _review;
    private AssuranceController? _assurance;
    private ReviewPaneSource? _reviewSource;
    private CancellationTokenSource? _reviewLoad;
    private CodeWorkspace? _workspace;
    private SolutionExplorerSource? _explorerSource;
    private DocumentCoordinator? _coordinator;
    private CodeCommandSet _commands;
    private IFileDialogService? _fileDialogs;

    /// <summary>
    /// True while the status picker is being set to match the record, so the
    /// selection it raises is not read back as the user asking for a decision.
    /// <see cref="UiComboBox.SelectIndex"/> raises the same event either way,
    /// and without this a tab switch would record the previous file's status
    /// onto the new one.
    /// </summary>
    private bool _syncingReviewStatus;
    private bool _disposed;

    public CodeShell(CodeShellControls controls)
    {
        _controls = controls ?? throw new ArgumentNullException(nameof(controls));
        _problemsSource = new ProblemsTreeSource(_problems);
        _commands = new CodeCommandSet(() => _workspace, () => _coordinator?.ActiveDocument ?? WorkspaceItemId.None);

        Compose();
        _controls.Menu.ItemInvoked += OnMenuItemInvoked;
        _controls.Explorer.NodeActivated += OnExplorerNodeActivated;
        _controls.Problems.NodeActivated += OnProblemActivated;

        // The review pane follows whatever the editor is showing. Subscribed
        // here rather than through DocumentCoordinator because the coordinator
        // raises no active-document event, and the tab strip's selection is the
        // same fact seen one level lower.
        _controls.Tabs.SelectionChanged += OnActiveDocumentChanged;

        // The assurance section follows the caret, which is the gesture a
        // reviewer already makes: they read a declaration, and the pane is about
        // the declaration they are reading. Nothing else in the shell listened to
        // this event before, and it is the whole of what makes selecting a class
        // or a method fill the pane.
        _controls.Editor.SelectionChanged += OnEditorSelectionChanged;
        RefreshCommands();
        SetStatus("No workspace open. File ▸ Open Folder…");
    }

    /// <summary>Raised when a command runs, so a host can act on Open/New.</summary>
    public event EventHandler<CodeCommandEventArgs>? CommandInvoked;

    public UiElement RootElement => _controls.Root;

    public ProblemsModel Problems => _problems;

    public CodeCommandSet Commands => _commands;

    public CodeWorkspace? Workspace => _workspace;

    public DocumentCoordinator? Coordinator => _coordinator;

    /// <summary>
    /// The Human Review state for the open workspace, or null when the head
    /// composed no review pane.
    /// </summary>
    public ReviewController? Review => _review;

    /// <summary>
    /// The per-unit assurance state for the file on screen, or null when the head
    /// composed no review pane.
    /// </summary>
    public AssuranceController? Assurance => _assurance;

    /// <summary>
    /// Finds the code units of a source file and fingerprints them, when the head
    /// composed a language service that can.
    ///
    /// Set before a workspace attaches, like <see cref="RevisionProvider"/>. It is
    /// optional on purpose: doing this exactly needs a real C# parser, and keeping
    /// one out of Core's closure is the decision the Phase 0 payload measurements
    /// produced. Without it the pane reads the annotation blocks alone — enough to
    /// show what is recorded and to record a decision, and not enough to recount a
    /// file's generated header, which it therefore does not.
    /// </summary>
    public IAssuranceUnitScanner? AssuranceScanner { get; set; }

    /// <summary>
    /// Binds a workspace. The explorer, the tabs, and the commands all follow
    /// from this; before it, the shell is present and inert rather than absent.
    /// </summary>
    public void AttachWorkspace(CodeWorkspace workspace, Workspaces.Recovery.RecoveryJournal? journal = null)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(workspace);

        DetachWorkspace();
        _workspace = workspace;
        _explorerSource = new SolutionExplorerSource(workspace);
        _controls.Explorer.DataSource = _explorerSource;

        _coordinator = new DocumentCoordinator(workspace, _controls.Editor, _controls.Tabs, journal);
        _coordinator.DirtyClosePrompt = DirtyClosePrompt;

        // Moved behind the coordinator's own handler, which was subscribed in the
        // line above. Handlers run in subscription order, and this shell
        // subscribed in its constructor — so on a tab click it read
        // Coordinator.ActiveDocument before ActivateSelectedTab had moved it, and
        // pointed the review panes at the file the user had just left. The panes
        // then stayed there, because nothing else re-syncs them.
        //
        // Re-subscribing here rather than adding an event to the coordinator: the
        // ordering is the whole defect, and one place that states it is easier to
        // keep right than a second notification path.
        _controls.Tabs.SelectionChanged -= OnActiveDocumentChanged;
        _controls.Tabs.SelectionChanged += OnActiveDocumentChanged;
        _commands = new CodeCommandSet(() => _workspace, () => _coordinator?.ActiveDocument ?? WorkspaceItemId.None)
        {
            HasFileDialogs = _fileDialogs is not null,
            HasFolderPicker = _fileDialogs is { CanRequestFolder: true },
            HasBuildService = _commands.HasBuildService,
            HasReview = _controls.Review is not null,
            HasReviewer = !string.IsNullOrWhiteSpace(Reviewer),
        };

        // Expand to the projects so the tree opens showing something, rather
        // than one collapsed row the user has to discover.
        foreach (TreeRow row in _controls.Explorer.Rows.ToArray())
            _controls.Explorer.Expand(row.Id);

        AttachReview(workspace);
        RefreshCommands();
        SetStatus($"{workspace.Projects.Count} projects, {workspace.Items.Count} items");
        ShowWorkspaceDiagnostics();
    }

    /// <summary>
    /// Binds the review workspace to the open workspace.
    ///
    /// The records are read once, in the background, rather than on the way in.
    /// A repository the size of this one has thousands of them, and blocking the
    /// window on that read would make opening a workspace feel like the tool had
    /// hung. Until it completes the explorer simply shows no badges, which is the
    /// same thing it shows for a file nobody has reviewed.
    /// </summary>
    private void AttachReview(CodeWorkspace workspace)
    {
        if (_controls.Review is null)
            return;

        _review = new ReviewController(workspace, RevisionProvider, Reviewer, Dispatcher);
        _assurance = new AssuranceController(workspace, AssuranceScanner) { Reviewer = Reviewer };
        _reviewSource = new ReviewPaneSource(_review, _assurance);
        _controls.Review.DataSource = _reviewSource;

        if (_explorerSource is { } explorer)
            explorer.ReviewStateOf = _review.StateFor;

        _review.Changed += OnReviewChanged;
        _assurance.Changed += OnAssuranceChanged;

        // Cancelled by DetachWorkspace, so closing a workspace stops a load that
        // is still reading rather than leaving it to finish filling a controller
        // nobody is showing.
        _reviewLoad = new CancellationTokenSource();
        _ = LoadReviewAsync(_review, _reviewLoad.Token);
    }

    private async Task LoadReviewAsync(ReviewController controller, CancellationToken cancellationToken)
    {
        try
        {
            if (Dispatcher is null)
            {
                // Nothing to marshal the result back with, and ReviewController's
                // contract without a dispatcher is that one thread drives it. Moving
                // the read to the pool anyway would leave it publishing into the
                // maps from there while this thread carries on using them — and
                // "attach a workspace, then review the first file" is one breath,
                // which is exactly the window that would land in.
                //
                // Synchronous underneath, per the note below, so this finishes
                // before AttachWorkspace returns rather than freezing anything a
                // host without a dispatcher was going to draw.
                await controller.LoadAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            // Task.Run, despite LoadAsync being async, because the desktop
            // storage provider is synchronous underneath its async signatures —
            // FileSystemWorkspaceStorage reads with File.ReadAllBytes and returns
            // ValueTask.FromResult, so nothing ever yields. Awaiting it directly
            // would read every record in the repository, and every file those
            // records describe, on the UI thread — exactly the freeze the
            // background load exists to avoid.
            //
            // The result is not applied here. LoadAsync posts it through the
            // dispatcher, so the maps are mutated on the UI thread even though
            // the reads finished on a pool thread.
            await Task.Run(
                () => controller.LoadAsync(cancellationToken).AsTask(), cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // The workspace closed while the records were being read. Nothing to
            // report: the controller it was filling has already been discarded.
        }
        catch (ObjectDisposedException)
        {
            // The same race seen from the other side — the cancellation source
            // was disposed between the check and the throw.
        }
    }

    /// <summary>
    /// Asked before a dirty document closes. A host that sets nothing gets
    /// Cancel, because losing someone's work is not a default.
    /// </summary>
    public DirtyClosePrompt? DirtyClosePrompt { get; set; }

    /// <summary>
    /// Who is recording reviews. Until a head sets it the review commands stay
    /// disabled with that as the reason: a record saying a file was approved,
    /// with no name on it, is not evidence of a human review.
    /// </summary>
    public string Reviewer
    {
        get;
        set
        {
            field = value ?? string.Empty;
            if (_review is not null)
                _review.Reviewer = field;
            RefreshCommands();
        }
    } = string.Empty;

    /// <summary>
    /// Supplies the revision a review is recorded at, when the head has one.
    /// Provenance only — see <see cref="IRevisionProvider"/>.
    /// </summary>
    public IRevisionProvider? RevisionProvider { get; set; }

    /// <summary>
    /// Makes a revision provider for a root the user grants at runtime, so
    /// <see cref="RevisionProvider"/> follows the folder the shell is actually
    /// showing rather than the one the head opened on.
    ///
    /// A factory rather than the shell constructing one, for the reason
    /// <see cref="Review.GitRevisionProvider"/> gives: asking git means running
    /// a process, and a host whose platform has none simply supplies nothing
    /// here. Left unset, provenance degrades to absent — which is an ordinary
    /// answer — rather than to wrong.
    /// </summary>
    public Func<IWorkspaceStorage, IRevisionProvider?>? RevisionProviderFactory { get; set; }

    /// <summary>
    /// Marshals background work onto the UI thread.
    ///
    /// Set by a head before the workspace attaches. Without it the review load
    /// applies its result on whichever thread finished the last read, which
    /// races every repaint — the reason
    /// <see cref="Hosting.UiThreadDispatcher"/> exists rather than the Standard
    /// immediate dispatcher.
    /// </summary>
    public UI.IUiDispatcher? Dispatcher { get; set; }

    /// <summary>Runs a named command. The single path the menu, toolbar, and keys share.</summary>
    public async ValueTask<bool> InvokeAsync(string name, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        CodeCommand? command = _commands.Find(name);
        if (command is null)
            return false;

        if (!command.IsEnabled)
        {
            SetStatus(command.Reason ?? $"{command.Text} is not available right now.");
            return false;
        }

        bool handled = name switch
        {
            CodeCommandNames.New => await NewDocumentAsync(cancellationToken).ConfigureAwait(false),
            CodeCommandNames.NewProject => await NewProjectAsync(cancellationToken).ConfigureAwait(false),
            CodeCommandNames.Open => await OpenAsync(cancellationToken).ConfigureAwait(false),
            CodeCommandNames.OpenFolder =>
                await OpenFolderAsync(cancellationToken).ConfigureAwait(false),
            CodeCommandNames.Save => await SaveActiveAsync(cancellationToken).ConfigureAwait(false),
            CodeCommandNames.SaveAs => await SaveActiveAsAsync(cancellationToken).ConfigureAwait(false),
            CodeCommandNames.SaveAll => await SaveAllAsync(cancellationToken).ConfigureAwait(false),
            CodeCommandNames.Close => _coordinator is not null &&
                await _coordinator.CloseAsync(_coordinator.ActiveDocument, cancellationToken).ConfigureAwait(false),
            CodeCommandNames.MarkReviewed =>
                await RecordReviewAsync(ReviewStatus.Reviewed, cancellationToken).ConfigureAwait(false),
            CodeCommandNames.MarkInReview =>
                await RecordReviewAsync(ReviewStatus.InReview, cancellationToken).ConfigureAwait(false),
            CodeCommandNames.MarkQuestion =>
                await RecordReviewAsync(ReviewStatus.Question, cancellationToken).ConfigureAwait(false),
            CodeCommandNames.MarkNeedsChange =>
                await RecordReviewAsync(ReviewStatus.NeedsChange, cancellationToken).ConfigureAwait(false),
            CodeCommandNames.ClearReview =>
                await RecordReviewAsync(ReviewStatus.Unreviewed, cancellationToken).ConfigureAwait(false),
            CodeCommandNames.AddNote => await AddNoteFromInputAsync(cancellationToken).ConfigureAwait(false),
            CodeCommandNames.ApproveUnit => SignUnit(sign: true),
            CodeCommandNames.WithdrawUnit => SignUnit(sign: false),
            CodeCommandNames.ReviewCoverage => ShowReviewCoverage(),
            _ => false,
        };

        // Raised for every command, including the ones handled here, so a host
        // can react — retitle its window, drive a build — without the shell
        // knowing what it wants.
        CommandInvoked?.Invoke(this, new CodeCommandEventArgs(name, handled));
        RefreshCommands();
        return handled;
    }

    /// <summary>
    /// Creates an untitled document and shows it. It is editable at once and
    /// asks for a location only when it is saved, so a user can start typing
    /// without first deciding where the file will live.
    /// </summary>
    public async ValueTask<bool> NewDocumentAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_workspace is null || _coordinator is null)
        {
            SetStatus("There is no workspace to add a document to.");
            return false;
        }

        SourceDocument document = _workspace.CreateUntitledDocument(_workspace.NextUntitledName());
        if (!await _coordinator.OpenAsync(document.Id, cancellationToken).ConfigureAwait(false))
        {
            SetStatus("The new document could not be opened.");
            return false;
        }

        // Refreshed here rather than relying on InvokeAsync to do it after:
        // EnsureDocumentAsync calls this directly on startup, and without it
        // the first document opens with Save and Close still showing disabled.
        RefreshCommands();
        SetStatus($"{document.Item.RelativePath} — not saved yet");
        return true;
    }

    /// <summary>
    /// Creates a solution with one console project and opens it.
    ///
    /// The save dialog supplies both halves of the answer at once: the directory
    /// to create it in and, from the file name, what to call it. That avoids a
    /// text-input dialog this shell does not have, and it is the same gesture
    /// the user already knows.
    ///
    /// What lands on disk is a plain <c>.slnx</c>, <c>.csproj</c>, and
    /// <c>Program.cs</c>. Nothing Broiler-specific is written, so the result
    /// builds with <c>dotnet build</c> and opens in any other tool.
    /// </summary>
    public async ValueTask<bool> NewProjectAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (FileDialogs is null)
        {
            SetStatus("This host has no way to ask where to create a project.");
            return false;
        }

        FileGrant? grant = await FileDialogs.RequestSaveAsync(
            new FileDialogRequest
            {
                Title = "New Project",
                SuggestedName = "MyApp.slnx",
                Filters = [FileDialogFilter.Solutions],
            },
            cancellationToken).ConfigureAwait(false);

        if (grant is null)
            return false;

        string name = StemOf(grant.RelativePath);
        TemplateResult solution = CodeTemplateService.PlanSolution(
            name, [$"src/{name}/{name}.csproj"]);
        if (!solution.Succeeded)
        {
            SetStatus(solution.Message!);
            return false;
        }

        TemplateResult project = CodeTemplateService.PlanProject(
            name, ProjectTemplateKind.ConsoleApplication);
        if (!project.Succeeded)
        {
            SetStatus(project.Message!);
            return false;
        }

        // Written as one plan, so the existing-file check covers every file
        // before any of them is created. A half-written project is worse than
        // none: it looks like something the user can open.
        var templates = new CodeTemplateService(grant.Storage);
        TemplateResult written = await templates
            .WriteAsync(new TemplateResult(true, [.. solution.Files, .. project.Files]), cancellationToken)
            .ConfigureAwait(false);

        if (!written.Succeeded)
        {
            SetStatus(written.Message!);
            return false;
        }

        // The new solution replaces the open one, so unsaved work is asked
        // about first — and if the user declines, the project still exists.
        if (_coordinator is not null &&
            !await _coordinator.CloseAllAsync(cancellationToken).ConfigureAwait(false))
        {
            SetStatus($"Created {name}, but the open workspace was kept: a document has unsaved changes.");
            return false;
        }

        await WorkspaceBootstrap.OpenAsync(this, grant.Storage, cancellationToken).ConfigureAwait(false);
        SetStatus($"Created {name} at {grant.DisplayPath}");
        return true;
    }

    /// <summary>
    /// Asks for a file and opens it. A solution replaces the workspace; a source
    /// file is opened through the grant the dialog created, which is how a file
    /// outside the current root is reached without widening that root.
    /// </summary>
    public async ValueTask<bool> OpenAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (FileDialogs is null)
        {
            SetStatus("This host has no way to ask for a file.");
            return false;
        }

        FileGrant? grant = await FileDialogs.RequestOpenAsync(
            new FileDialogRequest
            {
                Title = "Open",
                Filters = [FileDialogFilter.Sources, FileDialogFilter.Solutions, FileDialogFilter.All],
            },
            cancellationToken).ConfigureAwait(false);

        if (grant is null)
            return false;

        if (IsSolution(grant.RelativePath))
            return await OpenSolutionAsync(grant, cancellationToken).ConfigureAwait(false);

        if (_workspace is null)
        {
            SetStatus("There is no workspace to open a document into.");
            return false;
        }

        StorageResult<SourceDocument> opened = await _workspace
            .OpenGrantedDocumentAsync(grant.Storage, grant.RelativePath, cancellationToken)
            .ConfigureAwait(false);

        if (!opened.Succeeded)
        {
            SetStatus($"{grant.DisplayPath} could not be opened: {opened.Failure!.Message}");
            return false;
        }

        if (_coordinator is null ||
            !await _coordinator.OpenAsync(opened.Value!.Id, cancellationToken).ConfigureAwait(false))
        {
            SetStatus($"{grant.DisplayPath} could not be shown.");
            return false;
        }

        _controls.Explorer.Refresh();
        SetStatus(grant.DisplayPath);
        return true;
    }

    /// <summary>
    /// Asks for a directory and opens it as the workspace. This is how a
    /// reviewer points the tree at a component and walks it file by file.
    ///
    /// Unlike Open, which needs a workspace to open a document into, this makes
    /// one: everything under the granted directory is registered, so the
    /// explorer shows what is actually there rather than only what a solution
    /// declares — which for an ordinary SDK-style project is nothing. The
    /// workspace it replaces is closed first, so unsaved work is asked about
    /// before it goes and a declined prompt leaves everything as it was.
    /// </summary>
    public async ValueTask<bool> OpenFolderAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (FileDialogs is not { CanRequestFolder: true } dialogs)
        {
            SetStatus("This host has no way to ask for a folder.");
            return false;
        }

        FileGrant? grant = await dialogs
            .RequestFolderAsync(new FileDialogRequest { Title = "Open Folder" }, cancellationToken)
            .ConfigureAwait(false);

        if (grant is null)
            return false;

        if (_coordinator is not null &&
            !await _coordinator.CloseAllAsync(cancellationToken).ConfigureAwait(false))
        {
            SetStatus("The open workspace was kept: a document has unsaved changes.");
            return false;
        }

        ApplyRevisionProviderFor(grant);

        CodeWorkspace workspace = await WorkspaceBootstrap
            .OpenAsync(this, grant.Storage, cancellationToken).ConfigureAwait(false);

        int sources = workspace.Items.Count(item =>
            item.Kind == WorkspaceItemKind.SourceDocument && !item.IsUntitled);
        SetStatus($"{grant.DisplayPath} — {sources} source files");
        return true;
    }

    /// <summary>
    /// Points the revision provider at a root the user has just granted, when
    /// the head supplied a way to make one.
    ///
    /// It runs before the workspace attaches, because AttachWorkspace is what
    /// builds the review controller from it. A provider left pointing at the
    /// previous root would stamp that repository's commit onto every review
    /// recorded in this one — and provenance is the field nobody would think to
    /// check, so it has to be right without being noticed.
    /// </summary>
    private void ApplyRevisionProviderFor(FileGrant grant)
    {
        // Assigned unconditionally, including when there is no factory. Skipping
        // the assignment instead would leave the provider the head built for the
        // root it started on, which is the one outcome this method exists to
        // prevent: a review recorded in the newly granted repository carrying
        // the previous repository's commit. Absent provenance is an ordinary
        // answer and says so; wrong provenance looks exactly like right
        // provenance.
        RevisionProvider = RevisionProviderFactory?.Invoke(grant.Storage);
    }

    /// <summary>
    /// Asks where to write the active document, writes it, and rebinds it
    /// there. The document keeps its ID, so its tab, buffer, and undo history
    /// come with it.
    /// </summary>
    public async ValueTask<bool> SaveActiveAsAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_workspace is null || _coordinator is null || _coordinator.ActiveDocument.IsNone)
        {
            SetStatus("There is no document to save.");
            return false;
        }

        if (FileDialogs is null)
        {
            SetStatus("This host has no way to ask where to save.");
            return false;
        }

        WorkspaceItemId id = _coordinator.ActiveDocument;
        WorkspaceItem? item = _workspace.FindItem(id);

        FileGrant? grant = await FileDialogs.RequestSaveAsync(
            new FileDialogRequest
            {
                Title = "Save As",
                SuggestedName = item?.Name,
                Filters = [FileDialogFilter.Sources, FileDialogFilter.All],
            },
            cancellationToken).ConfigureAwait(false);

        if (grant is null)
            return false;

        SaveOutcome outcome = await _workspace
            .SaveDocumentAsAsync(id, grant.Storage, grant.RelativePath, cancellationToken)
            .ConfigureAwait(false);

        if (!outcome.Succeeded)
        {
            SetStatus(outcome.Message ?? $"{grant.DisplayPath} could not be written.");
            return false;
        }

        _coordinator.RenameTab(id, outcome.RelativePath);
        _controls.Explorer.Refresh();
        RefreshCommands();
        SetStatus($"Saved {grant.DisplayPath}");
        return true;
    }

    /// <summary>
    /// Asks the user for a file. Null on a host that cannot ask, which is what
    /// makes Open and Save As report Unavailable rather than doing nothing.
    /// </summary>
    public IFileDialogService? FileDialogs
    {
        get => _fileDialogs;
        set
        {
            _fileDialogs = value;
            _commands.HasFileDialogs = value is not null;
            _commands.HasFolderPicker = value is { CanRequestFolder: true };
            RefreshCommands();
        }
    }

    /// <summary>
    /// Guarantees something editable is on screen. Called after a workspace is
    /// attached: a window whose editor has no document refuses every keystroke,
    /// and an empty untitled buffer is a better first impression than a surface
    /// that silently ignores typing.
    /// </summary>
    public async ValueTask<bool> EnsureDocumentAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_coordinator is null)
            return false;
        if (!_coordinator.ActiveDocument.IsNone)
            return true;

        return await NewDocumentAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Which control should take keyboard focus for a pointer hit, so a host can
    /// route focus without knowing the shell's layout. Null leaves focus where
    /// it is — clicking the toolbar should not steal the caret from the editor.
    /// </summary>
    public UiElement? ResolveFocusTarget(UiElement? hit)
    {
        ThrowIfDisposed();
        for (UiElement? element = hit; element is not null; element = element.Parent)
        {
            if (ReferenceEquals(element, _controls.Editor))
                return _controls.Editor;
            if (ReferenceEquals(element, _controls.Explorer))
                return _controls.Explorer;
            if (ReferenceEquals(element, _controls.Problems))
                return _controls.Problems;

            // The review pane and the grips. A tree does not focus itself, so
            // without the first of these the review tree took a click and then
            // ignored every arrow key; the rest do focus themselves, and are
            // named here so the head is told the caret has left the editor —
            // otherwise it goes on drawing one in a document the keystrokes are
            // no longer reaching.
            if (ReferenceEquals(element, _controls.Review) ||
                ReferenceEquals(element, _controls.ReviewStatusInput) ||
                ReferenceEquals(element, _controls.ReviewNoteKindInput) ||
                ReferenceEquals(element, _controls.ReviewNoteInput) ||
                ReferenceEquals(element, _controls.ExplorerSplitter) ||
                ReferenceEquals(element, _controls.ReviewSplitter))
            {
                return element;
            }
        }

        return null;
    }

    /// <summary>The editor, so a host can give it focus when the window opens.</summary>
    public UiCodeEditor Editor => _controls.Editor;

    private async ValueTask<bool> OpenSolutionAsync(FileGrant grant, CancellationToken cancellationToken)
    {
        StorageResult<CodeWorkspace> loaded = await WorkspaceLoader
            .LoadSolutionAsync(grant.Storage, grant.RelativePath, cancellationToken)
            .ConfigureAwait(false);

        if (!loaded.Succeeded)
        {
            SetStatus($"{grant.DisplayPath} could not be loaded: {loaded.Failure!.Message}");
            return false;
        }

        // Every open document belongs to the workspace being replaced, so the
        // user is asked about unsaved work before it goes.
        if (_coordinator is not null &&
            !await _coordinator.CloseAllAsync(cancellationToken).ConfigureAwait(false))
        {
            SetStatus("The open solution was kept: a document with unsaved changes was not closed.");
            return false;
        }

        ApplyRevisionProviderFor(grant);
        AttachWorkspace(loaded.Value!);
        return true;
    }

    /// <summary>The file name without its extension, and without its folders.</summary>
    private static string StemOf(string relativePath)
    {
        int slash = relativePath.LastIndexOf('/');
        string name = slash < 0 ? relativePath : relativePath[(slash + 1)..];
        int dot = name.LastIndexOf('.');
        return dot <= 0 ? name : name[..dot];
    }

    private static bool IsSolution(string relativePath) =>
        relativePath.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase) ||
        relativePath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase);

    /// <summary>Opens a document and shows it, from the explorer or a host.</summary>
    public async ValueTask<bool> OpenDocumentAsync(
        WorkspaceItemId id, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_coordinator is null)
            return false;

        bool opened = await _coordinator.OpenAsync(id, cancellationToken).ConfigureAwait(false);
        if (!opened)
        {
            SetStatus("That item could not be opened.");
            return false;
        }

        WorkspaceItem? item = _workspace?.FindItem(id);
        SetStatus(item is null ? "Opened." : $"{item.RelativePath}");
        RefreshCommands();
        return true;
    }

    /// <summary>Publishes diagnostics for a document into the Problems pane.</summary>
    public void SetDocumentProblems(
        string documentPath, IEnumerable<MergedDiagnostic> diagnostics, ICodeTextSnapshot snapshot)
    {
        ThrowIfDisposed();
        _problems.SetDocumentEntries(documentPath, ProblemsModel.ToEntries(diagnostics, snapshot));
        _problemsSource.Refresh();
        _controls.Problems.Refresh();
        RefreshStatusCounts();
    }

    public void SetAnalysisMode(CodeAnalysisMode mode)
    {
        ThrowIfDisposed();
        _problems.Mode = mode;
        _controls.Editor.AnalysisMode = mode;
        RefreshStatusCounts();
    }

    public void SetOutput(string text)
    {
        ThrowIfDisposed();
        _controls.Output.Text = text ?? string.Empty;
        RefreshBottomPanes();
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _controls.Menu.ItemInvoked -= OnMenuItemInvoked;
        _controls.Explorer.NodeActivated -= OnExplorerNodeActivated;
        _controls.Problems.NodeActivated -= OnProblemActivated;
        if (_controls.Review is { } review)
            review.NodeActivated -= OnReviewNodeActivated;
        if (_controls.ReviewNoteInput is { } noteInput)
            noteInput.Submitted -= OnReviewNoteSubmitted;
        if (_controls.ReviewStatusInput is { } reviewStatus)
            reviewStatus.SelectionChanged -= OnReviewStatusSelected;
        _controls.ExplorerSplitter.ValueChanged -= OnExplorerSplitterMoved;
        if (_controls.ReviewSplitter is { } reviewSplitter)
            reviewSplitter.ValueChanged -= OnReviewSplitterMoved;
        _controls.Editor.SelectionChanged -= OnEditorSelectionChanged;
        _controls.Tabs.SelectionChanged -= OnActiveDocumentChanged;
        foreach (UiButton button in _toolbarButtons.Values)
            button.Clicked -= OnToolbarButtonClicked;
        _toolbarButtons.Clear();
        DetachWorkspace();
    }

    /// <summary>
    /// The layout. Docked rather than absolutely positioned so the panes keep
    /// their relationship as the window resizes.
    /// </summary>
    private void Compose()
    {
        UiPanel root = _controls.Root;
        root.LayoutMode = UiPanelLayoutMode.Dock;
        root.AddChild(_controls.Menu);
        root.SetDock(_controls.Menu, UiDock.Top);
        root.AddChild(_controls.Toolbar);
        root.SetDock(_controls.Toolbar, UiDock.Top);

        // Status first among the bottom-docked children, so it stays the
        // outermost strip and the Output pane sits above it.
        root.AddChild(_controls.Status);
        root.SetDock(_controls.Status, UiDock.Bottom);
        root.AddChild(_controls.Output);
        root.SetDock(_controls.Output, UiDock.Bottom);
        root.AddChild(_controls.Problems);
        root.SetDock(_controls.Problems, UiDock.Bottom);

        UiPanel body = _controls.Body;
        body.LayoutMode = UiPanelLayoutMode.Dock;
        body.AddChild(_controls.Explorer);
        body.SetDock(_controls.Explorer, UiDock.Left);
        body.AddChild(_controls.ExplorerSplitter);
        body.SetDock(_controls.ExplorerSplitter, UiDock.Left);

        UiPanel documents = _controls.DocumentArea;
        documents.LayoutMode = UiPanelLayoutMode.Dock;
        documents.AddChild(_controls.Tabs);
        documents.SetDock(_controls.Tabs, UiDock.Top);
        documents.AddChild(_controls.Editor);
        documents.SetDock(_controls.Editor, UiDock.Fill);

        ComposeReview(body);

        // Added last so it takes what the docked panes left, which is what Fill
        // means here. Adding it before the review pane would give the editor the
        // whole remainder and leave the review pane nothing to dock into.
        body.AddChild(documents);
        body.SetDock(documents, UiDock.Fill);
        root.AddChild(body);
        root.SetDock(body, UiDock.Fill);

        _controls.Problems.DataSource = _problemsSource;
        ComposeSplitter(_controls.ExplorerSplitter, OnExplorerSplitterMoved);
        ComposeToolbar();
        RefreshBottomPanes();
    }

    /// <summary>
    /// Makes a grip that actually moves the pane beside it.
    ///
    /// <see cref="UiSplitter"/> is a normalized value with a drag gesture on it
    /// and no opinion about layout — its own documentation says hosts apply the
    /// value — and this shell composed both grips without applying either. They
    /// drew, took the pointer, and moved a number nobody read, which is a grip
    /// that looks broken rather than one that is missing.
    ///
    /// The keys are stepped down from the control's defaults because those are
    /// fractions of the drag extent, and the extent here is a whole window's
    /// worth: an arrow key moves a pane 24 units and a page key 120, rather than
    /// half a pane at a time.
    /// </summary>
    private static void ComposeSplitter(
        UiSplitter splitter, EventHandler<UiSplitterValueChangedEventArgs> moved)
    {
        splitter.Orientation = UiSplitterOrientation.Vertical;
        splitter.DragExtent = SplitterDragExtent;
        splitter.SmallChange = 0.01;
        splitter.LargeChange = 0.05;
        splitter.ValueChanged += moved;
    }

    /// <summary>
    /// Docks the Human Review pane to the right of the editor, giving the shell
    /// the three columns the review workspace is built around: what to review on
    /// the left, the code in the middle, and what is known about it on the right.
    ///
    /// Does nothing when the head supplied no review tree, so a host that has
    /// not adopted the review workspace composes exactly the shell it did before.
    /// </summary>
    private void ComposeReview(UiPanel body)
    {
        if (_controls.Review is not { } review)
            return;

        // The pane goes to the far right and the splitter to its left, mirroring
        // the explorer's arrangement on the other side. Both grips are applied
        // by ComposeSplitter, which is where the width a drag produces is turned
        // back into a PreferredSize.
        if (_controls.ReviewPane is { } pane)
        {
            pane.LayoutMode = UiPanelLayoutMode.Dock;

            // Docked first among the Top children so it is the pane's own
            // heading: the decision is about the whole file, and the sections
            // below it — the declaration under the caret, the record, the notes
            // — are what that decision is made from.
            if (_controls.ReviewStatusInput is { } status)
            {
                status.SetItems(ReviewStatusItems);
                SyncReviewStatusInput();
                status.SelectionChanged += OnReviewStatusSelected;
                pane.AddChild(status);
                pane.SetDock(status, UiDock.Top);
            }

            if (_controls.ReviewNoteInput is { } input)
            {
                input.PlaceholderText = "Add a review note…";
                input.Submitted += OnReviewNoteSubmitted;
                pane.AddChild(input);
                pane.SetDock(input, UiDock.Bottom);
            }

            // Docked after the field so it lands above it: a dock panel gives
            // each Bottom child the edge the previous one left, so the second is
            // the higher of the two. Reading downwards it is "a Question about…"
            // followed by the text, which is the order the sentence is in.
            if (_controls.ReviewNoteKindInput is { } kinds)
            {
                kinds.SetItems(NoteKindItems);
                kinds.SelectIndex(0);
                pane.AddChild(kinds);
                pane.SetDock(kinds, UiDock.Bottom);
            }

            pane.AddChild(review);
            pane.SetDock(review, UiDock.Fill);
            body.AddChild(pane);
            body.SetDock(pane, UiDock.Right);
        }
        else
        {
            body.AddChild(review);
            body.SetDock(review, UiDock.Right);
        }

        if (_controls.ReviewSplitter is { } splitter)
        {
            ComposeSplitter(splitter, OnReviewSplitterMoved);
            body.AddChild(splitter);
            body.SetDock(splitter, UiDock.Right);
        }

        review.NodeActivated += OnReviewNodeActivated;
    }

    private void OnExplorerSplitterMoved(object? sender, UiSplitterValueChangedEventArgs e) =>
        ResizeExplorer(GripTravel(e));

    /// <summary>
    /// The review pane is docked on the far side, so the movement that widens
    /// the explorer narrows it. One sign, and the two grips both feel like the
    /// edge of the pane they are drawn against.
    /// </summary>
    private void OnReviewSplitterMoved(object? sender, UiSplitterValueChangedEventArgs e) =>
        ResizeReview(-GripTravel(e));

    /// <summary>How far the grip moved, in layout units. See <see cref="SplitterDragExtent"/>.</summary>
    private static double GripTravel(UiSplitterValueChangedEventArgs e) =>
        (e.NewValue - e.OldValue) * SplitterDragExtent;

    private void ResizeExplorer(double travel)
    {
        UiTreeView explorer = _controls.Explorer;
        if (explorer.IsDisposed || travel == 0)
            return;

        BSize size = explorer.PreferredSize;
        explorer.PreferredSize = new BSize(PaneWidth(size.Width + travel, explorer), size.Height);
    }

    /// <summary>
    /// Resizes the Human Review pane.
    ///
    /// Every control in it is set, not the tree alone: the pane is a dock panel
    /// and a dock panel is as wide as its widest child, so narrowing the tree by
    /// itself would leave the pane standing at the note field's width and the
    /// grip would appear to do nothing.
    /// </summary>
    private void ResizeReview(double travel)
    {
        if (_controls.Review is not { IsDisposed: false } review || travel == 0)
            return;

        double width = PaneWidth(
            review.PreferredSize.Width + travel, (UiElement?)_controls.ReviewPane ?? review);

        review.PreferredSize = new BSize(width, review.PreferredSize.Height);

        if (_controls.ReviewStatusInput is { IsDisposed: false } status)
            status.PreferredSize = new BSize(width, status.PreferredSize.Height);
        if (_controls.ReviewNoteKindInput is { IsDisposed: false } kinds)
            kinds.PreferredSize = new BSize(width, kinds.PreferredSize.Height);
        if (_controls.ReviewNoteInput is { IsDisposed: false } input)
            input.PreferredSize = new BSize(width, input.PreferredSize.Height);
    }

    /// <summary>
    /// The width a pane should take once its grip has moved, kept between a
    /// strip too narrow to read and one that would leave no editor.
    ///
    /// The ceiling is measured from the arranged bounds of the other docked
    /// panes rather than from their preferred sizes, so it is the room they
    /// actually took. Before the first frame there are none and only the floor
    /// applies — which costs nothing, because a grip cannot have been dragged
    /// before it was drawn.
    /// </summary>
    private double PaneWidth(double width, UiElement pane)
    {
        double body = _controls.Body.Bounds.Width;
        if (!double.IsFinite(body) || body <= 0)
            return Math.Max(MinimumPaneWidth, width);

        double taken = 0;
        foreach (UiElement child in _controls.Body.Children)
        {
            if (!ReferenceEquals(child, pane) &&
                _controls.Body.GetDock(child) is UiDock.Left or UiDock.Right)
            {
                taken += child.Bounds.Width;
            }
        }

        return Math.Clamp(
            width, MinimumPaneWidth, Math.Max(MinimumPaneWidth, body - taken - MinimumDocumentWidth));
    }

    /// <summary>
    /// Gives the bottom of the window to the editor when there is nothing to put
    /// there.
    ///
    /// The Problems tree and the Output line were docked unconditionally at the
    /// height their head asked for, so a session with no diagnostics and no
    /// coverage report drew a blank band under the editor that named nothing and
    /// could not be dismissed — reported, reasonably, as "an empty area at the
    /// bottom, maybe an output pane?". Collapsed rather than hidden, so the dock
    /// gives the space back instead of leaving a gap where the pane was.
    /// </summary>
    private void RefreshBottomPanes()
    {
        if (!_controls.Problems.IsDisposed)
        {
            _controls.Problems.Visibility = _problems.GetVisible().Count > 0
                ? UiVisibility.Visible
                : UiVisibility.Collapsed;
        }

        if (!_controls.Output.IsDisposed)
        {
            _controls.Output.Visibility = _controls.Output.Text.Length > 0
                ? UiVisibility.Visible
                : UiVisibility.Collapsed;
        }
    }

    private void RefreshCommands()
    {
        SyncReviewState();
        var items = new List<UiMenuItem>();

        // Mnemonics go in AccessKey, never in the text. UiMenu renders Text
        // verbatim, so "&File" is drawn with the ampersand in it — this shell
        // did exactly that, and the menu bar read "&File" and "&Build".
        var file = new UiMenuItem("file", "File") { AccessKey = 'F' };
        AddItem(file, CodeCommandNames.New);
        AddItem(file, CodeCommandNames.NewProject);
        AddItem(file, CodeCommandNames.Open);
        AddItem(file, CodeCommandNames.OpenFolder);
        file.Children.Add(new UiMenuItem("file.sep", string.Empty) { IsSeparator = true });
        AddItem(file, CodeCommandNames.Save);
        AddItem(file, CodeCommandNames.SaveAs);
        AddItem(file, CodeCommandNames.SaveAll);
        AddItem(file, CodeCommandNames.Close);
        items.Add(file);

        var build = new UiMenuItem("build", "Build") { AccessKey = 'B' };
        AddItem(build, CodeCommandNames.Build);
        AddItem(build, CodeCommandNames.Rebuild);
        AddItem(build, CodeCommandNames.Cancel);
        items.Add(build);

        // A menu of its own rather than entries under File. Human review is not
        // a file operation: it is the thing this shell exists to make routine,
        // and burying it three items deep would say the opposite.
        var review = new UiMenuItem("review", "Review") { AccessKey = 'R' };
        AddItem(review, CodeCommandNames.MarkReviewed);
        AddItem(review, CodeCommandNames.MarkInReview);
        AddItem(review, CodeCommandNames.MarkQuestion);
        AddItem(review, CodeCommandNames.MarkNeedsChange);
        review.Children.Add(new UiMenuItem("review.sep", string.Empty) { IsSeparator = true });
        AddItem(review, CodeCommandNames.AddNote);
        AddItem(review, CodeCommandNames.ClearReview);
        review.Children.Add(new UiMenuItem("review.sep2", string.Empty) { IsSeparator = true });
        AddItem(review, CodeCommandNames.ApproveUnit);
        AddItem(review, CodeCommandNames.WithdrawUnit);
        review.Children.Add(new UiMenuItem("review.sep3", string.Empty) { IsSeparator = true });
        AddItem(review, CodeCommandNames.ReviewCoverage);
        items.Add(review);

        _controls.Menu.SetItems(items);
        RefreshToolbar();

        void AddItem(UiMenuItem parent, string name)
        {
            CodeCommand? command = _commands.Find(name);
            if (command is null)
                return;

            parent.Children.Add(new UiMenuItem(name, MenuText(command))
            {
                CommandName = name,
                IsEnabled = command.IsEnabled,
                AccessKey = command.AccessKey,
            });
        }
    }

    /// <summary>
    /// Points the review pane at the active document and refreshes the state the
    /// review commands are enabled from.
    ///
    /// Driven from the command refresh because every path that can change either
    /// — a tab switch, a save, an edit, a recorded decision — already ends
    /// there. A second notification path would be one more thing to forget.
    /// </summary>
    private void SyncReviewState()
    {
        WorkspaceItemId active = _coordinator?.ActiveDocument ?? WorkspaceItemId.None;

        if (_review is not null && _review.CurrentDocument != active)
            _review.SetCurrentDocument(active);

        if (_assurance is not null)
        {
            _assurance.Reviewer = Reviewer;
            _assurance.SetCurrentDocument(active);
        }

        _commands.HasReview = _controls.Review is not null;
        _commands.HasReviewer = !string.IsNullOrWhiteSpace(Reviewer);
        _commands.HasAnnotatedUnit = _assurance is { CurrentUnit: not null };
        _commands.AssuranceUnitReason = AssuranceReason();
        SyncReviewStatusInput();
    }

    /// <summary>
    /// Points the status picker at what is actually recorded for the file on
    /// screen.
    ///
    /// Driven from the command refresh like the rest of this, so it follows a
    /// tab switch, a completed background load, and a decision the menu recorded
    /// — and so a decision the shell refused, because the file has unsaved
    /// changes or nobody has named the reviewer, puts the picker back where it
    /// was instead of leaving it showing a status no record carries.
    ///
    /// Only the status is read, never the freshness: a stale approval is still
    /// an approval its reviewer recorded, and offering to re-record it is a
    /// different gesture from being told it has gone stale, which the row below
    /// already says.
    /// </summary>
    private void SyncReviewStatusInput()
    {
        if (_controls.ReviewStatusInput is not { IsDisposed: false } status)
            return;

        string command = (_review?.CurrentState.Status ?? ReviewStatus.Unreviewed) switch
        {
            ReviewStatus.InReview => CodeCommandNames.MarkInReview,
            ReviewStatus.Reviewed => CodeCommandNames.MarkReviewed,
            ReviewStatus.Question => CodeCommandNames.MarkQuestion,
            ReviewStatus.NeedsChange => CodeCommandNames.MarkNeedsChange,
            _ => CodeCommandNames.ClearReview,
        };

        int index = Array.FindIndex(ReviewStatusItems, item => item.Id == command);
        if (index < 0 || index == status.SelectedIndex)
            return;

        _syncingReviewStatus = true;
        try
        {
            status.SelectIndex(index);
        }
        finally
        {
            _syncingReviewStatus = false;
        }
    }

    /// <summary>
    /// Records the decision the reviewer picked, through the command the menu
    /// entry drives.
    ///
    /// The command rather than the controller, so that one place still decides
    /// whether a decision may be written — and so the reason it may not is said
    /// out loud on the status line, which is what
    /// <see cref="InvokeAsync(string, CancellationToken)"/> already does for a
    /// command that is disabled.
    /// </summary>
    private void OnReviewStatusSelected(object? sender, UiComboBoxSelectionChangedEventArgs e)
    {
        if (_syncingReviewStatus)
            return;

        if (_controls.ReviewStatusInput?.SelectedItem is { Id: { Length: > 0 } command })
            _ = RecordPickedStatusAsync(command);
    }

    /// <summary>
    /// Runs the picked decision and then puts the picker back on whatever ended
    /// up recorded.
    ///
    /// The second half is not redundant: a command that ran refreshes the picker
    /// on its way out, and a command that was refused for being disabled returns
    /// before it gets there — which is the case that matters, because that is
    /// exactly when the picker is showing something no record carries.
    /// </summary>
    private async ValueTask RecordPickedStatusAsync(string command)
    {
        await InvokeAsync(command).ConfigureAwait(true);
        if (!_disposed)
            SyncReviewStatusInput();
    }

    /// <summary>
    /// An unavailable command stays visible and disabled with the reason in its
    /// text. Hiding it would leave the user wondering whether the feature exists
    /// at all.
    /// </summary>
    private static string MenuText(CodeCommand command) =>
        command.Availability == CommandAvailability.Unavailable
            ? $"{command.Text} (unavailable)"
            : command.Text;

    /// <summary>
    /// The toolbar's buttons are made once and then only updated. Rebuilding
    /// them on every command refresh would replace the element under the
    /// pointer mid-click and discard focus on every keystroke.
    /// </summary>
    private void ComposeToolbar()
    {
        _controls.Toolbar.Title = "Broiler Code";

        foreach (string name in ToolbarCommands)
        {
            UiButton button = _controls.CreateButton();
            button.CommandName = name;
            button.Clicked += OnToolbarButtonClicked;
            _controls.Toolbar.AddChild(button);
            _toolbarButtons[name] = button;
        }

        // The file group and the build group are different kinds of action.
        if (_toolbarButtons.TryGetValue(CodeCommandNames.Build, out UiButton? buildButton))
            _controls.Toolbar.SetSeparatorBefore(buildButton, true);
    }

    private void RefreshToolbar()
    {
        foreach ((string name, UiButton button) in _toolbarButtons)
        {
            if (button.IsDisposed)
                continue;

            CodeCommand? command = _commands.Find(name);
            if (command is null)
                continue;

            button.Text = command.Text;
            button.IsEnabled = command.IsEnabled;

            // Sized from the label rather than left at a default: a button
            // narrower than its text renders as an unreadable stub.
            button.PreferredSize = new BSize((command.Text.Length * 7.5) + 20, 28);
        }
    }

    private void OnToolbarButtonClicked(object? sender, UiButtonClickEventArgs e)
    {
        if (sender is UiButton { CommandName: { Length: > 0 } name })
            _ = InvokeAsync(name);
    }

    private void OnMenuItemInvoked(object? sender, UiMenuItemInvokedEventArgs e)
    {
        if (e.Item?.CommandName is { Length: > 0 } name)
            _ = InvokeAsync(name);
    }

    private void OnExplorerNodeActivated(object? sender, TreeNodeEventArgs e)
    {
        if (_explorerSource is null)
            return;

        WorkspaceItemId id = _explorerSource.ItemFor(e.Node);
        if (!id.IsNone && _workspace?.FindItem(id) is { Kind: WorkspaceItemKind.SourceDocument })
            _ = OpenDocumentAsync(id);
    }

    private void OnProblemActivated(object? sender, TreeNodeEventArgs e)
    {
        if (_problemsSource.EntryFor(e.Node) is not { } entry || entry.IsProjectLevel)
            return;

        // Navigation: open the document, then put the caret on the diagnostic.
        WorkspaceItem? item = _workspace?.FindItem(entry.DocumentPath);
        if (item is null)
            return;

        _ = NavigateAsync(item.Id, entry);
    }

    private void OnActiveDocumentChanged(object? sender, UiTabSelectionChangedEventArgs e) =>
        RefreshCommands();

    private void OnReviewChanged(object? sender, EventArgs e)
    {
        // An unreadable record makes its file look unreviewed, so it is said out
        // loud rather than left for the reviewer to notice a badge that quietly
        // went missing.
        if (_review is { Unreadable.Count: > 0 } review)
        {
            SetStatus(review.Unreadable.Count == 1
                ? review.Unreadable[0].Message
                : $"{review.Unreadable.Count} review records could not be read; those files show as unreviewed.");
        }

        // The explorer badges come from the same controller, so a recorded
        // decision has to redraw the tree as well as the pane. Refresh() rather
        // than a targeted change: a single decision can move one row, and a
        // completed background load moves thousands.
        _explorerSource?.Refresh();
        RefreshCommands();
    }

    /// <summary>
    /// The caret chose a different declaration, so the pane and the commands
    /// follow it.
    ///
    /// Routed through the controller's own change event rather than refreshing
    /// from here, because the caret moves on every keystroke and the unit under it
    /// does not. Rebuilding the menu that often would be work on the typing path
    /// for a result that is almost always the same.
    /// </summary>
    private void OnEditorSelectionChanged(object? sender, CodeSelectionChangedEventArgs e)
    {
        if (_assurance is null)
            return;

        ICodeTextSnapshot snapshot = _controls.Editor.Snapshot;
        _assurance.SetCaretLine(
            snapshot.GetLineFromPosition(Math.Clamp(_controls.Editor.Selection.Focus, 0, snapshot.Length)));
    }

    private void OnAssuranceChanged(object? sender, EventArgs e) => RefreshCommands();

    /// <summary>
    /// Why the unit commands are unavailable, or null when they are not.
    ///
    /// Each refusal is a different problem with a different fix — open a file,
    /// put the caret in a declaration, open one that carries annotations — and a
    /// single greyed-out entry saying none of that is how a feature meant to be
    /// used daily stops being used.
    /// </summary>
    private string? AssuranceReason()
    {
        if (_assurance is not { } assurance)
            return "This host does not compose the Human Review pane.";

        if (assurance.Document is null)
            return "Open a file to review it.";

        if (!assurance.IsAnnotatedFile)
            return "This file carries no Broiler Code Assurance annotations.";

        if (assurance.CurrentUnit is null)
            return "Put the caret inside an annotated declaration.";

        return null;
    }

    /// <summary>
    /// Activating a note in the review pane puts the caret on the code it is
    /// about. The note's placed line is used, not its recorded one, so this lands
    /// correctly on a file that has been edited since the note was written.
    ///
    /// A row standing for a code unit does the same thing for its declaration,
    /// which is what turns the Units section into a way through the file rather
    /// than a readout of it.
    /// </summary>
    private void OnReviewNodeActivated(object? sender, TreeNodeEventArgs e)
    {
        if (_reviewSource?.UnitFor(e.Node) is { } unit)
        {
            GoToLine(unit.DeclarationLine);
            return;
        }

        if (_reviewSource?.NoteFor(e.Node) is not { } note ||
            note.Status is ReviewAnchorStatus.FileLevel or ReviewAnchorStatus.Orphaned)
        {
            return;
        }

        GoToLine(note.StartLine);
    }

    /// <summary>Puts the caret at the start of a line and scrolls it into view.</summary>
    private void GoToLine(int line)
    {
        ICodeTextSnapshot snapshot = _controls.Editor.Snapshot;
        if (line < 0 || line >= snapshot.LineCount)
            return;

        _controls.Editor.Selection = CodeSelection.Caret(snapshot.GetLineStart(line));
        _controls.Editor.EnsureCaretVisible();
    }

    /// <summary>
    /// Adds a note from the pane's input field, anchored to the line the caret
    /// is on.
    ///
    /// The caret rather than a separate "pick a line" gesture: the reviewer is
    /// already reading the line they are asking about, and a note-taking step
    /// that first asks where is a note-taking step people skip.
    /// </summary>
    private void OnReviewNoteSubmitted(object? sender, UiEditSubmittedEventArgs e) =>
        _ = InvokeAsync(CodeCommandNames.AddNote);

    /// <summary>
    /// Writes the note in the pane's input field, anchored to the line the caret
    /// is on.
    ///
    /// The caret rather than a separate "pick a line" gesture: the reviewer is
    /// already reading the line they are asking about, and a note-taking step
    /// that first asks where is a note-taking step people skip.
    ///
    /// The kind comes from the pane's picker when the head composed one, and is
    /// a Question when it did not — which is what the product could write at
    /// all before the picker existed.
    ///
    /// When the caret is inside an annotated declaration the note also records
    /// that declaration's qualified name as its symbol. The review format has
    /// carried that field from the start and nothing has ever filled it in,
    /// because nothing in the shell knew what a declaration was; the assurance
    /// scanner does. It stays what it always was — display and search, never the
    /// thing that decides where a note goes, which is still the anchored text.
    /// </summary>
    private async ValueTask<bool> AddNoteFromInputAsync(CancellationToken cancellationToken)
    {
        if (_review is null || _controls.ReviewNoteInput is not { } input)
        {
            SetStatus("This host has no way to write a review note.");
            return false;
        }

        string text = input.Text;
        if (text.Trim().Length == 0)
        {
            SetStatus("Type the note first.");
            return false;
        }

        ICodeTextSnapshot snapshot = _controls.Editor.Snapshot;
        int line = snapshot.GetLineFromPosition(
            Math.Clamp(_controls.Editor.Selection.Focus, 0, snapshot.Length));

        ReviewActionResult result = await _review
            .AddNoteAsync(SelectedNoteKind(), text, line, line, SymbolAtCaret(), cancellationToken)
            .ConfigureAwait(true);

        if (result.Succeeded && !input.IsDisposed)
            input.Text = string.Empty;

        SetStatus(result.Message);
        return result.Succeeded;
    }

    /// <summary>
    /// Writes the reviewer's name onto the declaration under the caret, or takes
    /// it back.
    ///
    /// Synchronous, unlike the file-level decisions, because it writes nothing to
    /// storage: the rewrite goes into the open buffer, which is what makes it
    /// undoable and puts it in front of the reviewer before it is saved. Saving
    /// stays where it was.
    /// </summary>
    private bool SignUnit(bool sign)
    {
        if (_assurance is not { } assurance)
        {
            SetStatus("This host does not compose the Human Review pane.");
            return false;
        }

        assurance.Reviewer = Reviewer;
        AssuranceActionResult result = sign ? assurance.Approve() : assurance.Withdraw();

        SetStatus(result.Message);
        RefreshCommands();
        return result.Succeeded;
    }

    /// <summary>
    /// The kind the pane's picker is showing, or a Question when the head
    /// composed no picker.
    ///
    /// Read from the control at the moment the note is written rather than
    /// tracked as the selection changes: one place decides, and there is no
    /// second copy of the answer to go stale.
    /// </summary>
    private ReviewNoteKind SelectedNoteKind()
    {
        if (_controls.ReviewNoteKindInput?.SelectedItem is not { } item)
            return ReviewNoteKind.Question;

        return Enum.TryParse(item.Id, out ReviewNoteKind kind) ? kind : ReviewNoteKind.Question;
    }

    /// <summary>
    /// The qualified name of the declaration the caret is in, or null when it is
    /// not in one — which is most of the time, and is why the field is optional.
    /// </summary>
    private string? SymbolAtCaret() => _assurance?.CurrentUnit?.Name;

    private async ValueTask<bool> RecordReviewAsync(ReviewStatus status, CancellationToken cancellationToken)
    {
        if (_review is null)
        {
            SetStatus("This host does not compose the Human Review pane.");
            return false;
        }

        ReviewActionResult result = await _review
            .RecordDecisionAsync(status, cancellationToken).ConfigureAwait(true);

        SetStatus(result.Message);
        return result.Succeeded;
    }

    /// <summary>
    /// Writes the coverage summary to the Output line.
    ///
    /// The counterpart to the Test262 and WPT numbers, computed over the open
    /// workspace rather than the whole platform: this is the shell's answer, and
    /// the CI tool's is the one that covers every component.
    /// </summary>
    private bool ShowReviewCoverage()
    {
        if (_review is null)
        {
            SetStatus("This host does not compose the Human Review pane.");
            return false;
        }

        ReviewCoverageTotals totals = ReviewCoverage.Overall(_review.Snapshot());

        // Through SetOutput rather than onto the label, so the pane it lands in
        // is shown. The Output line is collapsed while it is empty, and writing
        // past the one method that knows that would report coverage into a strip
        // the window is not drawing.
        SetOutput(
            $"Human review: {totals.Verified}/{totals.Total} files verified " +
            $"({totals.FormatPercent(totals.VerifiedPercent)}), " +
            $"{totals.StaleApprovals} modified since review, " +
            $"{totals.Unreviewed} never reviewed, {totals.OpenNotes} open notes.");

        SetStatus("Review coverage written to the Output pane.");
        return true;
    }

    private async ValueTask NavigateAsync(WorkspaceItemId id, ProblemEntry entry)
    {
        if (!await OpenDocumentAsync(id).ConfigureAwait(false))
            return;

        ICodeTextSnapshot snapshot = _controls.Editor.Snapshot;
        if (entry.Line < snapshot.LineCount)
        {
            int position = snapshot.GetLineStart(entry.Line) +
                Math.Min(entry.Column, snapshot.GetLineLength(entry.Line));
            _controls.Editor.Selection = CodeSelection.Caret(position);
            _controls.Editor.EnsureCaretVisible();
        }
    }

    private async ValueTask<bool> SaveActiveAsync(CancellationToken cancellationToken)
    {
        if (_workspace is null || _coordinator is null)
            return false;

        SaveOutcome outcome = await _workspace
            .SaveDocumentAsync(_coordinator.ActiveDocument, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // A document that has never been saved has nowhere to go, so Save
        // becomes Save As. Reporting "the save failed" for a brand-new file
        // would be both wrong and unactionable.
        if (outcome.Kind == SaveOutcomeKind.NeedsLocation)
            return await SaveActiveAsAsync(cancellationToken).ConfigureAwait(false);

        SetStatus(outcome.Kind switch
        {
            SaveOutcomeKind.Saved => $"Saved {outcome.RelativePath}",
            SaveOutcomeKind.NotDirty => "No changes to save.",
            SaveOutcomeKind.Conflict => $"{outcome.RelativePath} changed on disk since it was opened.",
            _ => outcome.Message ?? "The save failed.",
        });
        return outcome.Succeeded;
    }

    private async ValueTask<bool> SaveAllAsync(CancellationToken cancellationToken)
    {
        if (_coordinator is null)
            return false;

        SaveAllReport report = await _coordinator.SaveAllAsync(cancellationToken).ConfigureAwait(false);

        // A never-saved document is not a failure, it is a question. Each is
        // brought forward and asked about in turn, so Save All does not quietly
        // skip the documents most likely to be lost.
        var unsaved = new List<string>();
        int saved = report.SavedCount;

        foreach (SaveOutcome outcome in report.Outcomes)
        {
            if (outcome.Succeeded)
                continue;

            if (outcome.Kind != SaveOutcomeKind.NeedsLocation)
            {
                unsaved.Add(outcome.RelativePath);
                continue;
            }

            bool located =
                await _coordinator.OpenAsync(outcome.Id, cancellationToken).ConfigureAwait(false) &&
                await SaveActiveAsAsync(cancellationToken).ConfigureAwait(false);

            if (located)
                saved++;
            else
                unsaved.Add(outcome.RelativePath);
        }

        // Partial failure is named, not summarised away: the user needs to know
        // which files are still unsaved.
        SetStatus(unsaved.Count == 0
            ? $"Saved {saved} documents."
            : $"Saved {saved}; not saved: {string.Join(", ", unsaved)}");
        return unsaved.Count == 0;
    }

    private void ShowWorkspaceDiagnostics()
    {
        if (_workspace is null)
            return;

        _problems.SetProjectEntries(_workspace.Diagnostics.Select(diagnostic =>
            ProblemsModel.ProjectEntry(
                diagnostic.RelativePath ?? "workspace", diagnostic.Code, diagnostic.Message)));
        _problemsSource.Refresh();
        _controls.Problems.Refresh();
        RefreshStatusCounts();
    }

    private void RefreshStatusCounts()
    {
        SetStatus(_problems.Counts.Describe(_problems.Mode));

        // Every path that changes the problem rows ends here, so this is the one
        // place that has to remember the pane only earns its band of the window
        // while it has rows.
        RefreshBottomPanes();
    }

    private void SetStatus(string text)
    {
        if (!_controls.Status.IsDisposed)
            _controls.Status.Text = text;
    }

    private void DetachWorkspace()
    {
        _coordinator?.Dispose();
        _coordinator = null;
        _explorerSource?.Dispose();
        _explorerSource = null;

        // Cancelled before the controller is disposed, so an in-flight load stops
        // at its next read instead of running to completion against a workspace
        // that has gone.
        _reviewLoad?.Cancel();
        _reviewLoad?.Dispose();
        _reviewLoad = null;

        if (_review is not null)
            _review.Changed -= OnReviewChanged;
        if (_assurance is not null)
            _assurance.Changed -= OnAssuranceChanged;
        _reviewSource?.Dispose();
        _reviewSource = null;
        _review?.Dispose();
        _review = null;
        _assurance?.Dispose();
        _assurance = null;

        if (_controls.Review is { IsDisposed: false } review)
            review.DataSource = null;

        // A host may dispose its session — and with it the whole element tree —
        // before the shell. Detaching then has nothing left to detach from, and
        // throwing out of Dispose over it would turn an ordering detail into a
        // crash on the way out.
        if (!_controls.Explorer.IsDisposed)
            _controls.Explorer.DataSource = null;

        _workspace = null;
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);
}

public sealed class CodeCommandEventArgs(string name, bool handled) : EventArgs
{
    public string Name { get; } = name;

    /// <summary>
    /// False when the shell raised the intent but did not act — New and Open
    /// need a picker, which belongs to the host.
    /// </summary>
    public bool Handled { get; } = handled;
}
