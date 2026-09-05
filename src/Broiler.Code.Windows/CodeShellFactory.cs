using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Broiler.Code.Core.Review;
using Broiler.Code.Core.Shell;
using Broiler.Code.Language.CSharp.Roslyn;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Storage;
using Broiler.Graphics;
using Broiler.UI.Button.Standard;
using Broiler.UI.CodeEditor.Standard;
using Broiler.UI.ComboBox.Standard;
using Broiler.UI.Edit.Standard;
using Broiler.UI.Label.Standard;
using Broiler.UI.Menu.Standard;
using Broiler.UI.Panel.Standard;
using Broiler.UI.Splitter.Standard;
using Broiler.UI.TabView.Standard;
using Broiler.UI.Toolbar.Standard;
using Broiler.UI.TreeView.Standard;

namespace Broiler.Code.Windows;

/// <summary>
/// Chooses the Standard implementations for the shell.
///
/// This is the composition root's actual job. <see cref="CodeShell"/> is written
/// against abstractions so it can be tested with no platform and hosted
/// anywhere; naming concrete types is what a head is for.
/// </summary>
[SupportedOSPlatform("windows7.0")]
internal static class CodeShellFactory
{
    public static (CodeShell Shell, StandardCodeEditor Editor) Create(BSize size)
    {
        var editor = new StandardCodeEditor { PreferredSize = size };

        var controls = new CodeShellControls
        {
            Root = new StandardPanel(),
            Body = new StandardPanel(),
            DocumentArea = new StandardPanel(),
            Menu = new StandardMenu(),
            Toolbar = new StandardToolbar(),
            Explorer = new StandardTreeView { PreferredSize = new BSize(280, size.Height) },
            ExplorerSplitter = new StandardSplitter(),
            Tabs = new StandardTabView { PreferredSize = new BSize(size.Width, 28) },
            Editor = editor,
            Problems = new StandardTreeView { PreferredSize = new BSize(size.Width, 160) },

            // The Human Review pane. Composed here rather than in the shell
            // because a head is what decides which controls exist — and because
            // a host without one still gets a working editor, which is what
            // lets the Android and browser heads adopt this later.
            ReviewPane = new StandardPanel(),
            Review = new StandardTreeView { PreferredSize = new BSize(320, size.Height) },
            ReviewSplitter = new StandardSplitter(),
            ReviewNoteInput = new StandardEdit { PreferredSize = new BSize(320, 26) },

            // What kind of thing the note is. Composed here for the same reason
            // as the rest of the pane, and it closes a gap the review workspace
            // shipped with: the record format has always carried four note kinds
            // and the product could only write one of them, because there was
            // nowhere to choose.
            ReviewNoteKindInput = new StandardComboBox { PreferredSize = new BSize(320, 26) },
            Status = new StandardLabel { Text = "Ready" },
            Output = new StandardLabel { Text = string.Empty },
            CreateButton = () => new StandardButton(),
        };

        return (new CodeShell(controls), editor);
    }

    /// <summary>
    /// Opens a workspace so the editor has a document to edit.
    ///
    /// The bootstrap itself is <see cref="WorkspaceBootstrap"/>'s, in Core,
    /// where it is testable; this only decides which directory is granted. A
    /// path given on the command line is used as-is. Without one, a scratch
    /// directory is granted and left empty — Broiler Code opens on an untitled
    /// buffer the user can type into, and asks where to put it only when they
    /// save.
    /// </summary>
    public static async ValueTask<CodeWorkspace> OpenWorkspaceAsync(CodeShell shell, string? path)
    {
        string root = path is { Length: > 0 } && Directory.Exists(path)
            ? Path.GetFullPath(path)
            : ScratchRoot();

        // Both are set before the workspace attaches, because AttachWorkspace is
        // what builds the review controller from them. Neither asking git lives
        // here: a head decides which directory is granted, and Core owns what to
        // ask about it.
        // Set with the revision provider, and for the same reason: AttachWorkspace
        // is what builds the review controllers from them, so both have to be in
        // place before a workspace opens. Without a scanner the assurance pane
        // reads the annotation blocks alone and declines to recount a file's
        // generated header; with one it knows the file's units, which of them are
        // exempt, and what each one's fingerprint is now.
        shell.AssuranceScanner = new CSharpAssuranceScanner();

        shell.RevisionProvider = new GitRevisionProvider(root);
        shell.Reviewer = await GitIdentity.ResolveReviewerAsync(root).ConfigureAwait(false);

        // And for a root the user grants later through Open Folder. Without it
        // the shell would go on recording this directory's commit against
        // reviews of files in another one.
        shell.RevisionProviderFactory = static granted => granted.GrantedRoots.Count == 0
            ? null
            : new GitRevisionProvider(granted.GrantedRoots[0]);

        return await WorkspaceBootstrap
            .OpenAsync(shell, new FileSystemWorkspaceStorage(root))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Where an unsaved session lives until it is saved somewhere real. Under
    /// LocalApplicationData rather than the temp directory: a recovery journal
    /// that a cleaner can delete is not a recovery journal.
    /// </summary>
    private static string ScratchRoot()
    {
        string root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Broiler",
            "Code",
            "scratch");

        Directory.CreateDirectory(root);
        return root;
    }
}
