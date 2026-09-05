using Broiler.Code.Core.Shell;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Storage;
using Broiler.Graphics;
using Broiler.Input;
using Broiler.Input.Mouse;
using Broiler.UI;
using Broiler.UI.Button.Standard;
using Broiler.UI.CodeEditor.Standard;
using Broiler.UI.Label.Standard;
using Broiler.UI.Menu.Standard;
using Broiler.UI.Panel.Standard;
using Broiler.UI.Splitter.Standard;
using Broiler.UI.Standard;
using Broiler.UI.TabView.Standard;
using Broiler.UI.Toolbar.Standard;
using Broiler.UI.TreeView;
using Broiler.UI.TreeView.Standard;

namespace Broiler.Code.Core.Tests;

/// <summary>
/// Opening a file by pointing at it.
///
/// The shell opens a document when the explorer raises NodeActivated, and the
/// tree raised that from the Enter key and nowhere else — so a user who clicked
/// a file in the Solution Explorer, and then double-clicked it, saw the row
/// highlight and nothing more. Every existing shell test opened documents by
/// calling <c>OpenDocumentAsync</c> directly, which is why none of them noticed.
/// This drives the mouse.
/// </summary>
public sealed class ExplorerActivationTests : IDisposable
{
    private readonly string _root = Path.Combine(
        Path.GetTempPath(), "broiler-activate", Guid.NewGuid().ToString("n"));

    public ExplorerActivationTests()
    {
        Directory.CreateDirectory(Path.Combine(_root, "src"));
        File.WriteAllText(Path.Combine(_root, "src", "Alpha.cs"), "class Alpha { }\n");
        File.WriteAllText(Path.Combine(_root, "src", "Beta.cs"), "class Beta { }\n");
    }

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

    [Fact(Timeout = 600000)]
    public async Task Double_Clicking_A_File_In_The_Explorer_Opens_It()
    {
        using Harness harness = await Harness.CreateAsync(_root);

        // Beta, because opening the workspace already showed Alpha: a test
        // whose target is the document that is on screen anyway proves nothing.
        WorkspaceItem beta = harness.Item("src/Beta.cs");
        Assert.NotEqual(beta.Id, harness.Shell.Coordinator!.ActiveDocument);
        int row = harness.RevealRow(beta.Id);

        harness.ClickRow(row);
        harness.Clock.Advance(TimeSpan.FromMilliseconds(120));
        harness.ClickRow(row);

        Assert.Equal(beta.Id, await harness.ActiveDocumentAsync(beta.Id));
    }

    [Fact(Timeout = 600000)]
    public async Task One_Click_Selects_The_Row_Without_Opening_It()
    {
        using Harness harness = await Harness.CreateAsync(_root);
        WorkspaceItem beta = harness.Item("src/Beta.cs");
        WorkspaceItemId before = harness.Shell.Coordinator!.ActiveDocument;
        int row = harness.RevealRow(beta.Id);

        harness.ClickRow(row);

        // Selection is not opening: a single click moves the highlight, and a
        // reviewer walking the tree with the mouse must not have a document
        // loaded under them at every step.
        Assert.Equal(
            harness.Controls.Explorer.Rows[row].Id,
            Assert.Single(harness.Controls.Explorer.Selection));
        Assert.Equal(before, await harness.ActiveDocumentAsync(beta.Id));
    }

    private sealed class Harness(
        CodeShell shell,
        CodeShellControls controls,
        UiSession session,
        ManualClock clock,
        CodeWorkspace workspace) : IDisposable
    {
        public CodeShell Shell { get; } = shell;

        public CodeShellControls Controls { get; } = controls;

        public UiSession Session { get; } = session;

        public ManualClock Clock { get; } = clock;

        public CodeWorkspace Workspace { get; } = workspace;

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
                Editor = new StandardCodeEditor { PreferredSize = new BSize(900, 600) },
                Problems = new StandardTreeView { PreferredSize = new BSize(1000, 120) },
                Status = new StandardLabel(),
                Output = new StandardLabel(),
                CreateButton = () => new StandardButton(),
            };

            var shell = new CodeShell(controls);
            var storage = new FileSystemWorkspaceStorage(root);
            await WorkspaceBootstrap.OpenAsync(shell, storage);

            var clock = new ManualClock();
            UiSession session = new StandardUiSessionBuilder()
                .WithClock(clock)
                .Build(new TestHost(new BSize(1000, 700)));
            session.AddRoot(controls.Root);

            // Arranged once, so the rows the pointer aims at have real
            // rectangles rather than zero-sized ones.
            session.RenderFrame();
            return new Harness(shell, controls, session, clock, shell.Workspace!);
        }

        public WorkspaceItem Item(string relativePath) =>
            Workspace.Items.First(item => item.RelativePath == relativePath);

        /// <summary>Expands until the item's row is on screen, and returns its index.</summary>
        public int RevealRow(WorkspaceItemId id)
        {
            UiTreeView explorer = Controls.Explorer;

            // Expanded outwards rather than through RevealNode, because
            // RevealNode also selects — and these tests are about what the
            // pointer does to an unselected tree.
            for (int pass = 0; pass < 8; pass++)
            {
                foreach (TreeRow row in explorer.Rows.ToArray())
                    explorer.Expand(row.Id);
            }

            Session.RenderFrame();

            var source = (SolutionExplorerSource)explorer.DataSource!;
            TreeNodeId node = source.NodeFor(id);
            int index = explorer.Rows.ToList().FindIndex(row => row.Id == node);
            Assert.InRange(index, 0, explorer.VisibleRowCapacity - 1);
            return index;
        }

        /// <summary>A press and release on a row's label, as a mouse delivers one.</summary>
        public void ClickRow(int index)
        {
            var tree = (StandardTreeView)Controls.Explorer;
            double y = tree.Bounds.Top +
                ((index - tree.FirstVisibleRow + 0.5) * tree.RowHeight);

            // Well right of the deepest expander, so the press is on the row
            // and not on its triangle.
            double x = tree.Bounds.Left + 120;

            Session.DispatchInput(UiInputEvent.FromMouseButton(Mouse(x, y, MouseButtonTransition.Down)));
            Session.DispatchInput(UiInputEvent.FromMouseButton(Mouse(x, y, MouseButtonTransition.Up)));
        }

        /// <summary>
        /// The active document, once an open of <paramref name="expected"/> has
        /// had time to run. The shell opens without awaiting — it is answering
        /// an event — so the test waits for the file read rather than assuming
        /// it finished, and waits out the whole window before concluding that
        /// nothing was opened.
        /// </summary>
        public async ValueTask<WorkspaceItemId> ActiveDocumentAsync(WorkspaceItemId expected)
        {
            for (int attempt = 0; attempt < 200; attempt++)
            {
                if (Shell.Coordinator!.ActiveDocument == expected)
                    break;
                await Task.Delay(10);
            }

            return Shell.Coordinator!.ActiveDocument;
        }

        public void Dispose()
        {
            // Shell first: disposing the session disposes the element tree, and
            // the shell still wants to detach from its controls.
            Shell.Dispose();
            Session.Dispose();
        }

        private static MouseButtonEvent Mouse(double x, double y, MouseButtonTransition transition) =>
            new(
                new InputEventHeader(
                    InputDeviceId.FromOpaqueValue("mouse"),
                    new InputTimestamp(1, TimeSpan.TicksPerSecond, "explorer"),
                    1),
                InputPoint.ClientDeviceIndependentPixels(x, y),
                transition == MouseButtonTransition.Down ? MouseButtons.Left : MouseButtons.None,
                MouseButton.Left,
                transition,
                InputEventSource.Synthetic);
    }

    private sealed class ManualClock : IUiClock
    {
        public UiTimestamp Now { get; private set; }

        public void Advance(TimeSpan delta) => Now = new UiTimestamp(Now.Elapsed + delta);
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
