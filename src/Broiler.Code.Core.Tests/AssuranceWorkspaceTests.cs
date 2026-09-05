using Broiler.Code.Core.Review;
using Broiler.Code.Core.Shell;
using Broiler.Code.Review;
using Broiler.Code.Review.Assurance;
using Broiler.Code.Workspaces;
using Broiler.Code.Workspaces.Model;
using Broiler.Code.Workspaces.Storage;
using Broiler.Code.Workspaces.Text;
using Broiler.Graphics;
using Broiler.UI.Button.Standard;
using Broiler.UI.CodeEditor;
using Broiler.UI.CodeEditor.Standard;
using Broiler.UI.ComboBox;
using Broiler.UI.ComboBox.Standard;
using Broiler.UI.Edit.Standard;
using Broiler.UI.Label.Standard;
using Broiler.UI.Menu;
using Broiler.UI.Menu.Standard;
using Broiler.UI.Panel.Standard;
using Broiler.UI.Splitter.Standard;
using Broiler.UI.TabView.Standard;
using Broiler.UI.Toolbar.Standard;
using Broiler.UI.TreeView;
using Broiler.UI.TreeView.Standard;

namespace Broiler.Code.Core.Tests;

/// <summary>
/// Reviewing one declaration at a time, as the user meets it: put the caret in a
/// class or a method, read what is claimed about it in the right-hand pane, and
/// sign it — after which the source file says so.
///
/// The shell is exercised with no language service composed, which is the level
/// every host is guaranteed to have and the level at which the whole of what a
/// human writes still works. What the scanner adds on top — exempt units, real
/// fingerprints, and the recounted file header — is asserted where the scanner
/// lives.
/// </summary>
public sealed class AssuranceWorkspaceTests : IDisposable
{
    /// <summary>
    /// Two annotated declarations, laid out the way the component that owns the
    /// format lays them out: the block immediately above the declaration, the
    /// value column shared across every line of it.
    /// </summary>
    private const string AnnotatedSource =
        "namespace Sample;\n" +
        "\n" +
        "/// <summary>A thing.</summary>\n" +
        "// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AAAAAA\n" +
        "// Broiler-Human:        PENDING\n" +
        "public sealed class Thing\n" +
        "{\n" +
        "    /// <summary>Does the work.</summary>\n" +
        "    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=BBBBBB\n" +
        "    // Broiler-Falsified-If: the caller can observe a partially applied change\n" +
        "    // Broiler-Human:        PENDING\n" +
        "    public int Work(int value)\n" +
        "    {\n" +
        "        return value + 1;\n" +
        "    }\n" +
        "}\n";

    private const string PlainSource = "class Beta { }\n";

    /// <summary>
    /// A second annotated file whose declaration sits at a different line, so a
    /// caret carried over from another file would find a unit here.
    /// </summary>
    private const string OtherSource =
        "namespace Sample;\n" +
        "\n" +
        "public sealed class Other\n" +
        "{\n" +
        "    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CCCCCC\n" +
        "    // Broiler-Human:        PENDING\n" +
        "    public int Value => 1;\n" +
        "}\n";

    private readonly string _root = Path.Combine(
        Path.GetTempPath(), "broiler-assurance-shell", Guid.NewGuid().ToString("n"));

    public AssuranceWorkspaceTests()
    {
        Directory.CreateDirectory(Path.Combine(_root, "src"));
        File.WriteAllText(Path.Combine(_root, "src", "Thing.cs"), AnnotatedSource);
        File.WriteAllText(Path.Combine(_root, "src", "Beta.cs"), PlainSource);
        File.WriteAllText(Path.Combine(_root, "src", "Other.cs"), OtherSource);
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

    /// <summary>
    /// The gesture the feature is for: the caret moves into a declaration and the
    /// pane is about that declaration.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task The_Caret_Selects_The_Unit_The_Pane_Shows()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);

        // Line 5 is the class declaration; line 11 is the method's.
        PutCaretOn(controls, 5);
        Assert.Contains("class Thing", shell.Assurance!.CurrentUnit!.DisplayName, StringComparison.Ordinal);

        PutCaretOn(controls, 13);
        Assert.Contains("Work", shell.Assurance.CurrentUnit!.DisplayName, StringComparison.Ordinal);

        shell.Dispose();
    }

    /// <summary>
    /// What the machine claimed reaches the pane, field by field, including the
    /// criterion — which is the sentence a reviewer of a high-security
    /// declaration is being asked to argue with.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task The_Pane_Shows_What_The_Annotation_Claims()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        IReadOnlyList<(string Label, string? Value)> rows = PaneRows(controls, "group:unit");

        Assert.Contains(("State", "needs human review"), rows);
        Assert.Contains(("Origin", "AI"), rows);
        Assert.Contains(("Security risk", "High"), rows);
        Assert.Contains(("Resource impact", "2"), rows);
        Assert.Contains(("Fingerprint", "BBBBBB"), rows);
        Assert.Contains(("Falsified if", "the caller can observe a partially applied change"), rows);
        Assert.Contains(("Human line", "PENDING"), rows);

        shell.Dispose();
    }

    /// <summary>
    /// The whole point, driven end to end: sign the declaration under the caret
    /// and the source file records it, at the column the format puts it in.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Signing_A_Unit_Writes_The_Reviewer_Into_The_Source()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        SourceDocument document = await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        Assert.True(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));

        string text = document.Buffer.Current.ToString();
        Assert.Contains("    // Broiler-Human:        Enrico\n", text, StringComparison.Ordinal);

        // The other declaration is untouched: one signature, one line.
        Assert.Contains("// Broiler-Human:        PENDING\n", text, StringComparison.Ordinal);

        shell.Dispose();
    }

    /// <summary>
    /// No fingerprint is ever written, and this is the safety property of the
    /// feature rather than an omission.
    ///
    /// The fingerprint on a human line is what binds an approval to a version of
    /// the code, and the owning component's generator is what writes it. An
    /// editor that wrote it too could turn one keystroke into a completed
    /// approval of code nobody read; this one structurally cannot, because a bare
    /// name is the only thing it knows how to write.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Signing_Never_Writes_A_Fingerprint()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        SourceDocument document = await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        Assert.True(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));

        string human = document.Buffer.Current.ToString()
            .Split('\n')
            .Single(line => line.Contains("Broiler-Human:        Enrico", StringComparison.Ordinal));

        Assert.DoesNotContain("Fingerprint", human, StringComparison.Ordinal);
        Assert.DoesNotContain("VERIFIED", human, StringComparison.Ordinal);

        shell.Dispose();
    }

    /// <summary>
    /// The edit goes through the open buffer, so it is undoable and the reviewer
    /// sees it before it is committed. A review that wrote straight to disk would
    /// be a change nobody could take back and nobody had to look at.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task A_Signature_Is_An_Ordinary_Undoable_Edit()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        SourceDocument document = await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        Assert.True(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));
        Assert.True(document.IsDirty);
        Assert.True(document.Buffer.CanUndo);

        Assert.True(document.Buffer.Undo());
        Assert.Equal(AnnotatedSource, document.Buffer.Current.ToString());

        // And nothing was written to storage on the way past.
        Assert.Equal(
            AnnotatedSource,
            await File.ReadAllTextAsync(Path.Combine(_root, "src", "Thing.cs")));

        shell.Dispose();
    }

    /// <summary>
    /// Withdrawing is the way out of a signature on the wrong declaration.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Withdrawing_Puts_The_Unit_Back_To_Pending()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        SourceDocument document = await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        Assert.True(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));
        Assert.True(await shell.InvokeAsync(CodeCommandNames.WithdrawUnit));

        Assert.Equal(AnnotatedSource, document.Buffer.Current.ToString());
        shell.Dispose();
    }

    /// <summary>
    /// A caret that is nowhere in particular disables the commands, with the
    /// reason on them rather than a bare greyed-out entry.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Without_A_Unit_The_Commands_Say_Why()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);

        // The namespace line: inside the file, above every annotation.
        PutCaretOn(controls, 0);

        CodeCommand sign = shell.Commands.Find(CodeCommandNames.ApproveUnit)!;
        Assert.Equal(CommandAvailability.Disabled, sign.Availability);
        Assert.Equal("Put the caret inside an annotated declaration.", sign.Reason);

        Assert.False(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));
        shell.Dispose();
    }

    /// <summary>
    /// A file that carries no annotations says so, which is a different problem
    /// from the caret being in the wrong place and has a different answer.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task A_File_Without_Annotations_Says_So()
    {
        (CodeShell shell, _) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);

        await OpenAsync(shell, workspace, "src/Beta.cs");

        CodeCommand sign = shell.Commands.Find(CodeCommandNames.ApproveUnit)!;
        Assert.Equal(CommandAvailability.Disabled, sign.Availability);
        Assert.Equal("This file carries no Broiler Code Assurance annotations.", sign.Reason);

        shell.Dispose();
    }

    /// <summary>
    /// A signature has to say whose it is, enforced the same way the file-level
    /// decisions enforce it.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Signing_Without_A_Reviewer_Is_Refused()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell(reviewer: string.Empty);
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        CodeCommand sign = shell.Commands.Find(CodeCommandNames.ApproveUnit)!;
        Assert.Equal(CommandAvailability.Disabled, sign.Availability);
        Assert.Contains("reviewer name", sign.Reason!, StringComparison.Ordinal);

        shell.Dispose();
    }

    /// <summary>
    /// Unlike the four file-level decisions, this one is allowed on a document
    /// with unsaved changes — and the difference is deliberate.
    ///
    /// A file review records a hash of the content a person read, so unsaved text
    /// would make it unverifiable by anybody. A signature records a name and no
    /// claim about content at all. Requiring a save between every declaration
    /// would be friction bought with nothing.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Signing_Is_Allowed_While_The_Document_Is_Dirty()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        SourceDocument document = await OpenThingAsync(shell, workspace);

        Append(document, "// an unsaved edit\n");
        PutCaretOn(controls, 13);

        // The file-level command refuses, naming the save...
        Assert.Equal(
            CommandAvailability.Disabled,
            shell.Commands.Find(CodeCommandNames.MarkReviewed)!.Availability);

        // ...and the unit command does not.
        Assert.True(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));
        Assert.Contains(
            "// Broiler-Human:        Enrico",
            document.Buffer.Current.ToString(),
            StringComparison.Ordinal);

        shell.Dispose();
    }

    /// <summary>
    /// Every unit in the file is listed, and activating one moves the caret to
    /// its declaration — which is what makes the pane a way through a file rather
    /// than a readout of wherever the caret already was.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Activating_A_Unit_Row_Moves_The_Caret_To_It()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 0);

        var units = new TreeNodeId("group:units");
        Assert.Equal(2, controls.Review!.DataSource!.GetChildCount(units));

        controls.Review.ActivateNode(controls.Review.DataSource.GetChild(units, 1));

        ICodeTextSnapshot snapshot = controls.Editor.Snapshot;
        Assert.Equal(11, snapshot.GetLineFromPosition(controls.Editor.Selection.Focus));

        shell.Dispose();
    }

    /// <summary>
    /// A note written while the caret is in a declaration records that
    /// declaration's name.
    ///
    /// The review record has carried a symbol field since it was designed and
    /// nothing had ever filled it in, because nothing in the shell knew what a
    /// declaration was. It stays what it always was — display and search, never
    /// the thing that decides where a note goes.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task A_Note_Records_The_Declaration_It_Was_Written_In()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        controls.ReviewNoteInput!.Text = "Is the increment checked?";
        Assert.True(await shell.InvokeAsync(CodeCommandNames.AddNote));

        ReviewNote note = shell.Review!.CurrentReview.Notes.Single();
        Assert.Contains("Work", note.Anchor.Symbol!, StringComparison.Ordinal);

        shell.Dispose();
    }

    /// <summary>
    /// The note-kind picker writes the kind it is showing.
    ///
    /// The record format has carried four kinds from the start and the product
    /// could only ever write a Question, because the pane had nowhere to choose.
    /// A count of open notes that cannot tell an observation from a concern is a
    /// count that means less than it says.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task The_Picker_Chooses_What_Kind_Of_Note_Is_Written()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);

        Assert.True(SelectKind(controls, ReviewNoteKind.Concern));
        controls.ReviewNoteInput!.Text = "The overflow case is not handled.";
        Assert.True(await shell.InvokeAsync(CodeCommandNames.AddNote));

        ReviewNote note = shell.Review!.CurrentReview.Notes.Single();
        Assert.Equal(ReviewNoteKind.Concern, note.Kind);

        // And an Observation, which never counts as open, so a reviewer can leave
        // context on a file they are approving.
        Assert.True(SelectKind(controls, ReviewNoteKind.Observation));
        controls.ReviewNoteInput.Text = "Mirrors the reader in the sibling type.";
        Assert.True(await shell.InvokeAsync(CodeCommandNames.AddNote));

        Assert.Equal(1, shell.Review.CurrentReview.OpenNoteCount);
        shell.Dispose();
    }

    /// <summary>
    /// A head that composes no picker keeps exactly the behaviour it had: every
    /// note a Question, and the note field still there.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Without_A_Picker_Every_Note_Is_A_Question()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell(withPicker: false);
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);

        controls.ReviewNoteInput!.Text = "Why is this ordered this way?";
        Assert.True(await shell.InvokeAsync(CodeCommandNames.AddNote));

        Assert.Equal(ReviewNoteKind.Question, shell.Review!.CurrentReview.Notes.Single().Kind);
        shell.Dispose();
    }

    /// <summary>
    /// The Review menu carries the two unit commands, so they are reachable
    /// rather than only invocable by name.
    /// </summary>
    [Fact(Timeout = 600000)]
    public void The_Review_Menu_Carries_The_Unit_Commands()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();

        UiMenuItem review = controls.Menu.Items.Single(item => item.Id == "review");
        Assert.Contains(review.Children, item => item.CommandName == CodeCommandNames.ApproveUnit);
        Assert.Contains(review.Children, item => item.CommandName == CodeCommandNames.WithdrawUnit);

        shell.Dispose();
    }

    /// <summary>
    /// Without a language service the file header is not rewritten, and the
    /// signature is still recorded.
    ///
    /// The licence to rewrite a generated header is the ability to reproduce it,
    /// and a build that cannot compute a fingerprint cannot count reviewed units.
    /// It fails closed, and what it fails closed on is the part the reviewer does
    /// not own.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Without_A_Scanner_The_Signature_Lands_And_The_Header_Does_Not_Move()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        SourceDocument document = await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        Assert.False(shell.Assurance!.HasUnitScanner);
        Assert.Null(shell.Assurance.Document!.Summary);

        Assert.True(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));
        Assert.Contains(
            "// Broiler-Human:        Enrico",
            document.Buffer.Current.ToString(),
            StringComparison.Ordinal);

        shell.Dispose();
    }

    /// <summary>
    /// Picks a note kind in the pane's control the way a pointer would, by the
    /// identifier the shell writes into it rather than by a position a reordering
    /// would silently change.
    /// </summary>
    private static bool SelectKind(CodeShellControls controls, ReviewNoteKind kind)
    {
        UiComboBox picker = controls.ReviewNoteKindInput!;
        for (int index = 0; index < picker.Items.Count; index++)
        {
            if (string.Equals(picker.Items[index].Id, kind.ToString(), StringComparison.Ordinal))
                return picker.SelectIndex(index);
        }

        return false;
    }

    /// <summary>
    /// Opening another file does not carry the caret's line with it.
    ///
    /// The failure this guards against is the worst one the feature could have:
    /// the pane would report whatever declaration happened to occupy that line in
    /// the new file, and the next signature would land on it — in a file the
    /// reviewer had not read, with their name on it.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Opening_Another_File_Does_Not_Carry_The_Caret_Line()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        await OpenThingAsync(shell, workspace);

        PutCaretOn(controls, 13);
        Assert.NotNull(shell.Assurance!.CurrentUnit);

        await OpenAsync(shell, workspace, "src/Other.cs");

        // Other.cs annotates the declaration on a different line, so a carried
        // caret would have found a unit here.
        Assert.Null(shell.Assurance.CurrentUnit);
        Assert.False(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));

        shell.Dispose();
    }

    /// <summary>
    /// Clicking a tab moves both review panes to the file that tab is for.
    ///
    /// The shell subscribes to the tab strip in its constructor and the document
    /// coordinator subscribes in its own, which happens later — so handlers run
    /// shell-first, and the shell used to read the active document before the
    /// coordinator had moved it. Both panes then sat on the file the user had
    /// just left, and nothing re-synced them. Asserted through the control the
    /// user actually clicks rather than through the coordinator, because the
    /// ordering is the whole defect.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Clicking_A_Tab_Moves_The_Panes_To_That_File()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);

        await OpenThingAsync(shell, workspace);
        await OpenAsync(shell, workspace, "src/Other.cs");

        WorkspaceItem thing = workspace.FindItem("src/Thing.cs")!;
        string thingTab = controls.Tabs.Tabs
            .Single(tab => tab.Header.Contains("Thing", StringComparison.Ordinal)).Id;

        Assert.True(controls.Tabs.SelectTab(thingTab));

        Assert.Equal(thing.Id, shell.Coordinator!.ActiveDocument);
        Assert.Equal(thing.Id, shell.Review!.CurrentDocument);

        // And the assurance pane is reading Thing.cs, not the file it was on.
        PutCaretOn(controls, 13);
        Assert.Contains("Work", shell.Assurance!.CurrentUnit!.DisplayName, StringComparison.Ordinal);

        shell.Dispose();
    }

    /// <summary>
    /// Undoing a signature puts the pane back too.
    ///
    /// The caret does not move when a buffer is undone, so a pane that listened
    /// only to the caret would go on showing an approval the file no longer
    /// carries — and would report a reviewer who has just taken their name off.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task Undoing_A_Signature_Puts_The_Pane_Back()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        SourceDocument document = await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        Assert.True(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));
        Assert.Contains(("Human line", "Enrico"), PaneRows(controls, "group:unit"));

        Assert.True(document.Buffer.Undo());

        Assert.Contains(("Human line", "PENDING"), PaneRows(controls, "group:unit"));
        shell.Dispose();
    }

    /// <summary>
    /// A buffer that refuses the edit leaves the model holding nothing.
    ///
    /// The rewrite is applied to the document model before the buffer is asked,
    /// so a refusal that was not undone there would leave a ghost edit behind —
    /// and the next signature would write both, silently recording a decision the
    /// reviewer had been told was refused.
    /// </summary>
    [Fact(Timeout = 600000)]
    public async Task A_Refused_Edit_Leaves_Nothing_Behind()
    {
        (CodeShell shell, CodeShellControls controls) = CreateShell();
        CodeWorkspace workspace = CreateWorkspace();
        shell.AttachWorkspace(workspace);
        SourceDocument document = await OpenThingAsync(shell, workspace);
        PutCaretOn(controls, 13);

        document.Buffer.IsReadOnly = true;
        Assert.False(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));
        Assert.Equal(AnnotatedSource, document.Buffer.Current.ToString());

        document.Buffer.IsReadOnly = false;
        Assert.True(await shell.InvokeAsync(CodeCommandNames.ApproveUnit));

        // One signature, not two: the refused attempt left no trace to replay.
        Assert.Equal(
            1,
            document.Buffer.Current.ToString()
                .Split('\n')
                .Count(line => line.Contains("Broiler-Human:        Enrico", StringComparison.Ordinal)));

        shell.Dispose();
    }

    private static void PutCaretOn(CodeShellControls controls, int line)
    {
        ICodeTextSnapshot snapshot = controls.Editor.Snapshot;
        controls.Editor.Selection = CodeSelection.Caret(snapshot.GetLineStart(line));
    }

    private static IReadOnlyList<(string Label, string? Value)> PaneRows(
        CodeShellControls controls, string group)
    {
        ITreeDataSource source = controls.Review!.DataSource!;
        var node = new TreeNodeId(group);
        var rows = new List<(string, string?)>();

        for (int index = 0; index < source.GetChildCount(node); index++)
        {
            TreeNodePresentation row = source.GetPresentation(source.GetChild(node, index));
            rows.Add((row.Label, row.SecondaryLabel));
        }

        return rows;
    }

    private static void Append(SourceDocument document, string text)
    {
        TextSnapshot current = document.Buffer.Current;
        document.Buffer.Apply(new EditTransaction(
            current.Version, TextChange.Insert(current.Length, text), "test edit"));
    }

    /// <summary>
    /// Opens a file the way the existing review tests do.
    ///
    /// The panes are pointed at the document explicitly because this drives the
    /// coordinator directly rather than through the shell's own open command:
    /// adding the first tab selects it without raising a selection change — there
    /// was no previous selection to change from — so the event the shell listens
    /// to does not fire for the very first document a test opens.
    /// </summary>
    private static async Task<SourceDocument> OpenAsync(
        CodeShell shell, CodeWorkspace workspace, string path)
    {
        WorkspaceItem item = workspace.FindItem(path)!;
        SourceDocument document = (await workspace.OpenDocumentAsync(item.Id)).Value!;
        await shell.Coordinator!.OpenAsync(item.Id);
        shell.Review?.SetCurrentDocument(item.Id);
        shell.Assurance?.SetCurrentDocument(item.Id);
        return document;
    }

    private static Task<SourceDocument> OpenThingAsync(CodeShell shell, CodeWorkspace workspace) =>
        OpenAsync(shell, workspace, "src/Thing.cs");

    private CodeWorkspace CreateWorkspace()
    {
        var workspace = new CodeWorkspace(new FileSystemWorkspaceStorage(_root));
        var project = new CodeProject
        {
            Id = new WorkspaceItemId(1000),
            Name = "Sample",
            RelativePath = "src/Sample.csproj",
            TargetFrameworks = ["net10.0"],
            Items =
            [
                workspace.AddItem("src/Thing.cs", WorkspaceItemKind.SourceDocument).Id,
                workspace.AddItem("src/Beta.cs", WorkspaceItemKind.SourceDocument).Id,
                workspace.AddItem("src/Other.cs", WorkspaceItemKind.SourceDocument).Id,
            ],
        };

        workspace.AddProject(project);
        workspace.AddSolution(new CodeSolution
        {
            Id = new WorkspaceItemId(1001),
            Name = "Sample",
            RelativePath = "Sample.slnx",
            Projects = [project.Id],
        });

        return workspace;
    }

    private static (CodeShell Shell, CodeShellControls Controls) CreateShell(
        string reviewer = "Enrico", bool withPicker = true)
    {
        var controls = new CodeShellControls
        {
            Root = new StandardPanel(),
            Body = new StandardPanel(),
            DocumentArea = new StandardPanel(),
            Menu = new StandardMenu(),
            Toolbar = new StandardToolbar(),
            Explorer = new StandardTreeView(),
            ExplorerSplitter = new StandardSplitter(),
            Tabs = new StandardTabView(),
            Editor = new StandardCodeEditor { PreferredSize = new BSize(900, 600) },
            Problems = new StandardTreeView(),
            ReviewPane = new StandardPanel(),
            Review = new StandardTreeView(),
            ReviewSplitter = new StandardSplitter(),
            ReviewNoteInput = new StandardEdit(),
            ReviewNoteKindInput = withPicker ? new StandardComboBox() : null,
            Status = new StandardLabel(),
            Output = new StandardLabel(),
            CreateButton = () => new StandardButton(),
        };

        return (new CodeShell(controls) { Reviewer = reviewer }, controls);
    }
}
