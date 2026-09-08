using Broiler.Code.Core.Diagnostics;
using Broiler.Code.Core.Shell;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Storage;
using Broiler.Graphics;
using Broiler.Input;
using Broiler.Input.Mouse;
using Broiler.UI;
using Broiler.UI.Button.Standard;
using Broiler.UI.CodeEditor;
using Broiler.UI.CodeEditor.Standard;
using Broiler.UI.ComboBox;
using Broiler.UI.ComboBox.Standard;
using Broiler.UI.Edit.Standard;
using Broiler.UI.Label.Standard;
using Broiler.UI.Menu.Standard;
using Broiler.UI.Panel.Standard;
using Broiler.UI.Splitter;
using Broiler.UI.Splitter.Standard;
using Broiler.UI.Standard;
using Broiler.UI.TabView.Standard;
using Broiler.UI.Toolbar.Standard;
using Broiler.UI.TreeView.Standard;

namespace Broiler.Code.Core.Tests;

/// <summary>
/// The window's own chrome: whether a grip moves the pane beside it, and whether
/// a pane with nothing in it takes a band of the window anyway.
///
/// Both were reported from the running editor rather than found here, which is
/// the point of pinning them down: the splitters drew, took the pointer, and
/// moved a normalized value no one applied, and the Problems tree reserved its
/// height whether or not it had a row to put in it.
/// </summary>
public sealed class ShellPaneTests : IDisposable
{
    private readonly string _root = Path.Combine(
        Path.GetTempPath(), "broiler-panes", Guid.NewGuid().ToString("n"));

    public ShellPaneTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);
        }
        catch (IOException)
        {
        }
    }

    /// <summary>
    /// The whole path, driven the way a user drives it: press the grip, move,
    /// release, and the explorer is wider by what the pointer travelled.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Dragging_The_Explorer_Grip_Widens_The_Explorer()
    {
        using Harness harness = await Harness.CreateAsync(_root);
        double before = harness.Controls.Explorer.PreferredSize.Width;
        BRect grip = harness.Controls.ExplorerSplitter.Bounds;

        Assert.True(grip.Width > 0, "the grip has to be arranged before it can be dragged");

        harness.Route.Dispatch(MouseDown(grip.Left + (grip.Width / 2), 300));
        harness.Route.Dispatch(MouseMove(grip.Left + (grip.Width / 2) + 90, 300));
        harness.Route.Dispatch(MouseUp(grip.Left + (grip.Width / 2) + 90, 300));

        Assert.Equal(before + 90, harness.Controls.Explorer.PreferredSize.Width, 3);

        // And the pane the window draws follows, not only the number behind it.
        harness.Session.RenderFrame();
        Assert.Equal(before + 90, harness.Controls.Explorer.Bounds.Width, 3);
    }

    /// <summary>
    /// The review pane is docked on the far side, so the movement that widens
    /// the explorer has to narrow it — a grip that ran the wrong way would be a
    /// different bug wearing the same fix.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Dragging_The_Review_Grip_The_Other_Way_Widens_The_Review_Pane()
    {
        using Harness harness = await Harness.CreateAsync(_root);
        UiSplitter grip = harness.Controls.ReviewSplitter!;
        double before = harness.Controls.Review!.PreferredSize.Width;

        grip.Value += 60 / 2400d;
        Assert.Equal(before - 60, harness.Controls.Review.PreferredSize.Width, 3);

        grip.Value -= 160 / 2400d;
        Assert.Equal(before + 100, harness.Controls.Review.PreferredSize.Width, 3);
    }

    /// <summary>
    /// The pane is a dock panel and a dock panel is as wide as its widest child,
    /// so narrowing the tree alone would leave the pane standing at the note
    /// field's width and the grip would look inert for the second half of its
    /// travel.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task The_Whole_Review_Pane_Follows_Its_Grip()
    {
        using Harness harness = await Harness.CreateAsync(_root);

        harness.Controls.ReviewSplitter!.Value += 100 / 2400d;
        harness.Session.RenderFrame();

        double width = harness.Controls.Review!.PreferredSize.Width;
        Assert.Equal(width, harness.Controls.ReviewNoteInput!.PreferredSize.Width, 3);
        Assert.Equal(width, harness.Controls.ReviewNoteKindInput!.PreferredSize.Width, 3);
        Assert.Equal(width, harness.Controls.ReviewStatusInput!.PreferredSize.Width, 3);
        Assert.Equal(width, harness.Controls.ReviewPane!.Bounds.Width, 3);
    }

    /// <summary>
    /// A grip dragged to the end of the window must leave a pane somebody can
    /// read and an editor somebody can type in, rather than collapsing either.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task A_Pane_Cannot_Be_Dragged_Away_Or_Over_The_Editor()
    {
        using Harness harness = await Harness.CreateAsync(_root);

        harness.Controls.ExplorerSplitter.Value = harness.Controls.ExplorerSplitter.Minimum;
        Assert.Equal(120, harness.Controls.Explorer.PreferredSize.Width, 3);

        harness.Controls.ExplorerSplitter.Value = harness.Controls.ExplorerSplitter.Maximum;
        harness.Session.RenderFrame();

        // 1000 wide, less the review pane, the two grips and the strip of editor
        // that has to survive.
        Assert.True(
            harness.Controls.Editor.Bounds.Width >= 240,
            $"the editor kept {harness.Controls.Editor.Bounds.Width} units");
    }

    /// <summary>
    /// The reported defect: a band under the editor that named nothing, could
    /// not be dismissed, and belonged to a Problems pane with no problems in it.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task The_Bottom_Panes_Take_No_Room_While_They_Are_Empty()
    {
        using Harness harness = await Harness.CreateAsync(_root);

        Assert.Equal(UiVisibility.Collapsed, harness.Controls.Problems.Visibility);
        Assert.Equal(UiVisibility.Collapsed, harness.Controls.Output.Visibility);

        harness.Session.RenderFrame();
        Assert.Equal(BRect.Empty, harness.Controls.Problems.Bounds);

        // The editor reaches the status line, which is the whole of the fix.
        Assert.True(
            harness.Controls.Editor.Bounds.Bottom >= harness.Controls.Status.Bounds.Top - 1,
            "the editor should reach the status line when nothing is docked between them");
    }

    [Fact(Timeout = 600000)]
    public async Task A_Diagnostic_Brings_The_Problems_Pane_Back()
    {
        using Harness harness = await Harness.CreateAsync(_root);

        harness.Shell.SetDocumentProblems(
            "src/Alpha.cs",
            [Diagnostic("BR0001", "something is wrong")],
            harness.Controls.Editor.Snapshot);

        Assert.Equal(UiVisibility.Visible, harness.Controls.Problems.Visibility);
        harness.Session.RenderFrame();
        Assert.True(harness.Controls.Problems.Bounds.Height > 0);

        harness.Shell.SetDocumentProblems(
            "src/Alpha.cs", [], harness.Controls.Editor.Snapshot);

        Assert.Equal(UiVisibility.Collapsed, harness.Controls.Problems.Visibility);
    }

    /// <summary>
    /// Review Coverage writes to the Output line, so the line has to appear when
    /// it does — a report into a collapsed strip is a command that silently does
    /// nothing.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Writing_Output_Shows_The_Output_Line()
    {
        using Harness harness = await Harness.CreateAsync(_root);

        Assert.True(await harness.Shell.InvokeAsync(CodeCommandNames.ReviewCoverage));

        Assert.Equal(UiVisibility.Visible, harness.Controls.Output.Visibility);
        Assert.Contains("Human review:", harness.Controls.Output.Text, StringComparison.Ordinal);

        harness.Shell.SetOutput(string.Empty);
        Assert.Equal(UiVisibility.Collapsed, harness.Controls.Output.Visibility);
    }

    /// <summary>
    /// A tree does not focus itself, so without the shell naming it a click in
    /// the review pane left the caret in the editor and every arrow key with it.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task The_Review_Pane_And_The_Grips_Take_Focus()
    {
        using Harness harness = await Harness.CreateAsync(_root);

        Assert.Same(harness.Controls.Review, harness.Shell.ResolveFocusTarget(harness.Controls.Review));
        Assert.Same(
            harness.Controls.ReviewStatusInput,
            harness.Shell.ResolveFocusTarget(harness.Controls.ReviewStatusInput));
        Assert.Same(
            harness.Controls.ExplorerSplitter,
            harness.Shell.ResolveFocusTarget(harness.Controls.ExplorerSplitter));

        // Unchanged: pressing a toolbar button acts on the document you are
        // editing, so it must not take the caret out of it.
        Assert.Null(harness.Shell.ResolveFocusTarget(harness.Controls.Toolbar));
    }

    private static MergedDiagnostic Diagnostic(string code, string message) =>
        new(
            DiagnosticOrigin.Live,
            "src/Alpha.cs",
            new CodeDiagnosticAdornment(0, 1, CodeDiagnosticSeverity.Error, message, code),
            1);

    private static InputEventHeader Header() =>
        new(
            InputDeviceId.FromOpaqueValue("panes"),
            new InputTimestamp(1, TimeSpan.TicksPerSecond, "panes"),
            1);

    private static MouseButtonEvent MouseDown(double x, double y) =>
        new(Header(), InputPoint.ClientDeviceIndependentPixels(x, y), MouseButtons.Left,
            MouseButton.Left, MouseButtonTransition.Down, InputEventSource.Synthetic);

    private static MouseMoveEvent MouseMove(double x, double y) =>
        new(Header(), InputPoint.ClientDeviceIndependentPixels(x, y), MouseButtons.Left,
            InputEventSource.Synthetic);

    private static MouseButtonEvent MouseUp(double x, double y) =>
        new(Header(), InputPoint.ClientDeviceIndependentPixels(x, y), MouseButtons.None,
            MouseButton.Left, MouseButtonTransition.Up, InputEventSource.Synthetic);

    /// <summary>
    /// A shell in a real session, arranged once, so the panes have the
    /// rectangles a drag and a hit test are answered from.
    /// </summary>
    private sealed class Harness(
        CodeShell shell, CodeShellControls controls, UiSession session, StandardInputRoute route)
        : IDisposable
    {
        public CodeShell Shell { get; } = shell;

        public CodeShellControls Controls { get; } = controls;

        public UiSession Session { get; } = session;

        public StandardInputRoute Route { get; } = route;

        public static async ValueTask<Harness> CreateAsync(string root)
        {
            var controls = new CodeShellControls
            {
                Root = new StandardPanel(),
                Body = new StandardPanel(),
                DocumentArea = new StandardPanel(),
                Menu = new StandardMenu(),
                Toolbar = new StandardToolbar(),
                Explorer = new StandardTreeView { PreferredSize = new BSize(240, 600) },
                ExplorerSplitter = new StandardSplitter(),
                Tabs = new StandardTabView { PreferredSize = new BSize(1000, 28) },
                Editor = new StandardCodeEditor { PreferredSize = new BSize(1000, 600) },
                Problems = new StandardTreeView { PreferredSize = new BSize(1000, 160) },
                ReviewPane = new StandardPanel(),
                Review = new StandardTreeView { PreferredSize = new BSize(280, 600) },
                ReviewSplitter = new StandardSplitter(),
                ReviewNoteInput = new StandardEdit { PreferredSize = new BSize(280, 26) },
                ReviewNoteKindInput = new StandardComboBox { PreferredSize = new BSize(280, 26) },
                ReviewStatusInput = new StandardComboBox { PreferredSize = new BSize(280, 26) },
                Status = new StandardLabel { Text = "Ready" },
                Output = new StandardLabel { Text = string.Empty },
                CreateButton = () => new StandardButton(),
            };

            var shell = new CodeShell(controls) { Reviewer = "Enrico" };
            await WorkspaceBootstrap.OpenAsync(shell, new FileSystemWorkspaceStorage(root));

            UiSession session = new StandardUiSessionBuilder()
                .WithDispatcher(new ImmediateUiDispatcher())
                .Build(new TestHost(new BSize(1000, 700)));
            session.AddRoot(controls.Root);
            session.RenderFrame();

            return new Harness(shell, controls, session, new StandardInputRoute(session));
        }

        public void Dispose()
        {
            Shell.Dispose();
            Session.Dispose();
        }
    }

    private sealed class TestHost(BSize viewportSize) : IUiHost
    {
        public BSize ViewportSize { get; } = viewportSize;

        public double Scale => 1;

        public BRenderList CreateRenderList(int capacity = 0) => new(capacity);

        public void Invalidate(UiInvalidation invalidation)
        {
        }

        public void Present(BRenderList renderList)
        {
        }
    }
}
