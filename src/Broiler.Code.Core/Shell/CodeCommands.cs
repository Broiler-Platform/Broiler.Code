// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   33
// Annotated:        33/33
// Exempt:           13
// Human-reviewed:   0/33
// IP risk:          Low
// Security risk:    Medium
// Criteria:         11/0
// Resource impact:  2/10 max
// Unverified:       33
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Linq;

namespace Broiler.Code.Core.Shell;

/// <summary>
/// The commands the shell exposes. Named rather than anonymous handlers so the
/// menu, the toolbar, and a keyboard shortcut all drive the same thing and
/// report the same enabled state.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=3D8A5D
// Broiler-Human:        PENDING
public static class CodeCommandNames
{
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=A2BC07
    // Broiler-Human:        PENDING
    public const string New = "code.new";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=C2C47E
    // Broiler-Human:        PENDING
    public const string NewProject = "code.newProject";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=184DDC
    // Broiler-Human:        PENDING
    public const string Open = "code.open";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=3F1DA9
    // Broiler-Human:        PENDING
    public const string OpenFolder = "code.openFolder";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=D78DD6
    // Broiler-Human:        PENDING
    public const string Save = "code.save";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=E5D33D
    // Broiler-Human:        PENDING
    public const string SaveAs = "code.saveAs";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=E3190A
    // Broiler-Human:        PENDING
    public const string SaveAll = "code.saveAll";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=46542F
    // Broiler-Human:        PENDING
    public const string Close = "code.close";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=97BFED
    // Broiler-Human:        PENDING
    public const string Build = "code.build";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=2319D7
    // Broiler-Human:        PENDING
    public const string Rebuild = "code.rebuild";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=A1433A
    // Broiler-Human:        PENDING
    public const string Cancel = "code.cancel";

    // Human Review. Named commands like every other action, so the menu, a
    // keyboard shortcut, and a later toolbar button all drive one path and
    // report one enabled state — which for these matters more than usual,
    // because "why is Mark Reviewed greyed out?" has a specific answer the
    // command set is what produces.
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=DF2035
    // Broiler-Human:        PENDING
    public const string MarkReviewed = "code.review.reviewed";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=8FA29B
    // Broiler-Human:        PENDING
    public const string MarkInReview = "code.review.inReview";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=7D0CE4
    // Broiler-Human:        PENDING
    public const string MarkQuestion = "code.review.question";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=39AE9B
    // Broiler-Human:        PENDING
    public const string MarkNeedsChange = "code.review.needsChange";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=DE9D90
    // Broiler-Human:        PENDING
    public const string ClearReview = "code.review.clear";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=22E995
    // Broiler-Human:        PENDING
    public const string AddNote = "code.review.addNote";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=666A88
    // Broiler-Human:        PENDING
    public const string ReviewCoverage = "code.review.coverage";

    // Per-unit assurance. Separate commands from the four file-level decisions
    // above, because they record a different claim in a different place: a file
    // review says a person read this content and lives in .broiler-review/, and
    // a unit signature says a person stands behind this declaration and lives in
    // the source file the component that owns the format reads. One menu entry
    // doing both would be one entry making two claims.
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=14198E
    // Broiler-Human:        PENDING
    public const string ApproveUnit = "code.review.unit.approve";
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=AD2D9D
    // Broiler-Human:        PENDING
    public const string WithdrawUnit = "code.review.unit.withdraw";
}

// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=43E2DF
// Broiler-Human:        PENDING
public enum CommandAvailability
{
    /// <summary>Runnable now.</summary>
    Enabled,

    /// <summary>Exists, but not applicable to the current state.</summary>
    Disabled,

    /// <summary>
    /// Not implemented on this host or in this phase. Distinguished from
    /// Disabled so the UI can say why rather than showing a dead control the
    /// user keeps trying.
    /// </summary>
    Unavailable,
}

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=D3BCE3
// Broiler-Falsified-If: a command built as Disabled or Unavailable reports IsEnabled true, so InvokeAsync runs it
// Broiler-Human:        PENDING
public sealed record CodeCommand(
    string Name,
    string Text,
    CommandAvailability Availability,
    string? Reason = null,
    char? AccessKey = null)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8DFD0A
    // Broiler-Falsified-If: IsEnabled is true for an Availability other than Enabled
    // Broiler-Human:        PENDING
    public bool IsEnabled => Availability == CommandAvailability.Enabled;
}

/// <summary>
/// Command state for the shell, recomputed from the workspace rather than
/// tracked by hand — a flag that has to be updated in five places is a flag
/// that will be wrong in one of them.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=93934F
// Broiler-Falsified-If: Mark Reviewed is Enabled while the active document has unsaved edits or no name is set for the person recording it
// Broiler-Human:        PENDING
public sealed class CodeCommandSet
{
    private readonly Func<Workspaces.CodeWorkspace?> _workspace;
    private readonly Func<Workspaces.Model.WorkspaceItemId> _activeDocument;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C01844
    // Broiler-Falsified-If: a null delegate is accepted and GetCommands later throws NullReferenceException
    // Broiler-Human:        PENDING
    public CodeCommandSet(
        Func<Workspaces.CodeWorkspace?> workspace,
        Func<Workspaces.Model.WorkspaceItemId> activeDocument)
    {
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _activeDocument = activeDocument ?? throw new ArgumentNullException(nameof(activeDocument));
    }

    /// <summary>
    /// Set once a build service exists. Until Phase 4 lands one, the build
    /// commands report Unavailable with a reason rather than pretending.
    /// </summary>
    public bool HasBuildService { get; set; }

    public bool IsBuildRunning { get; set; }

    /// <summary>
    /// Set when the host can ask the user for a file. Without it Open and Save
    /// As are Unavailable with a reason — a menu entry that opens no dialog and
    /// reports nothing is worse than one that says the host cannot ask.
    /// </summary>
    public bool HasFileDialogs { get; set; }

    /// <summary>
    /// Set when the host can ask the user for a directory, which is a narrower
    /// claim than <see cref="HasFileDialogs"/>: picking a folder is a different
    /// platform call from picking a file, and a host may carry one and not the
    /// other.
    /// </summary>
    public bool HasFolderPicker { get; set; }

    /// <summary>
    /// Set when the head composed a review pane. Without it the review commands
    /// report Unavailable with a reason, in keeping with the rest of this set:
    /// a menu entry that silently does nothing is worse than one that says the
    /// host does not carry the feature.
    /// </summary>
    public bool HasReview { get; set; }

    /// <summary>Set once the host knows who is reviewing. An approval with no name is not evidence.</summary>
    public bool HasReviewer { get; set; }

    /// <summary>Set when the caret is inside a declaration carrying an assurance annotation.</summary>
    public bool HasAnnotatedUnit { get; set; }

    /// <summary>
    /// Why there is no unit to act on, when there is not. Supplied by the shell
    /// because the answer depends on things the command set cannot see — whether
    /// the open file carries annotations at all, and where the caret is.
    /// </summary>
    public string? AssuranceUnitReason { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B29067
    // Broiler-Falsified-If: a keystroke that dirties the active document leaves the four decision commands Enabled until something else refreshes the set
    // Broiler-Human:        PENDING
    public IReadOnlyList<CodeCommand> GetCommands()
    {
        Workspaces.CodeWorkspace? workspace = _workspace();
        bool hasWorkspace = workspace is not null;
        Workspaces.Model.WorkspaceItemId active = _activeDocument();
        bool hasDocument = !active.IsNone;
        bool dirty = workspace?.HasUnsavedChanges ?? false;

        // Asked of the buffer here rather than cached in a property the shell
        // refreshes. A keystroke makes a document dirty without going anywhere
        // near the command set, so a flag would say "clean" until something else
        // happened to refresh it — and the four commands that must never run on
        // a dirty document would be enabled exactly when it matters.
        bool activeIsDirty =
            hasDocument && workspace?.FindOpenDocument(active) is { IsDirty: true };

        return
        [
            // New needs somewhere to put the document, which is the workspace,
            // not a file dialog: an untitled buffer is created in memory and
            // only asks for a location when it is saved.
            new CodeCommand(
                CodeCommandNames.New, "New File",
                hasWorkspace ? CommandAvailability.Enabled : CommandAvailability.Disabled,
                AccessKey: 'N'),

            // A project, unlike a file, has to exist on storage the moment it is
            // created — a .csproj nobody can find is not a project — so this one
            // does need somewhere to put it before it can do anything.
            Picker(CodeCommandNames.NewProject, "New Project…", 'P'),
            Picker(CodeCommandNames.Open, "Open…", 'O'),

            // A folder needs no workspace to open into — it becomes the
            // workspace — so unlike Open it is enabled the moment the host can
            // ask, which is what lets it be the first thing a reviewer does.
            HasFolderPicker
                ? new CodeCommand(
                    CodeCommandNames.OpenFolder, "Open Folder…", CommandAvailability.Enabled,
                    AccessKey: 'F')
                : new CodeCommand(
                    CodeCommandNames.OpenFolder, "Open Folder…", CommandAvailability.Unavailable,
                    "This host has no way to ask for a folder.", 'F'),
            new CodeCommand(
                CodeCommandNames.Save, "Save",
                hasDocument ? CommandAvailability.Enabled : CommandAvailability.Disabled,
                AccessKey: 'S'),
            HasFileDialogs
                ? new CodeCommand(
                    CodeCommandNames.SaveAs, "Save As…",
                    hasDocument ? CommandAvailability.Enabled : CommandAvailability.Disabled,
                    AccessKey: 'A')
                : Picker(CodeCommandNames.SaveAs, "Save As…", 'A'),
            new CodeCommand(
                CodeCommandNames.SaveAll, "Save All",
                dirty ? CommandAvailability.Enabled : CommandAvailability.Disabled,
                AccessKey: 'L'),
            new CodeCommand(
                CodeCommandNames.Close, "Close Document",
                hasDocument ? CommandAvailability.Enabled : CommandAvailability.Disabled,
                AccessKey: 'C'),
            Build(CodeCommandNames.Build, "Build", hasWorkspace, 'B'),
            Build(CodeCommandNames.Rebuild, "Rebuild", hasWorkspace, 'R'),
            new CodeCommand(
                CodeCommandNames.Cancel, "Cancel Build",
                IsBuildRunning ? CommandAvailability.Enabled : CommandAvailability.Disabled,
                AccessKey: 'X'),

            Review(CodeCommandNames.MarkReviewed, "Mark Reviewed", hasDocument, activeIsDirty, 'R'),
            Review(CodeCommandNames.MarkInReview, "Mark In Review", hasDocument, activeIsDirty, 'I'),
            Review(CodeCommandNames.MarkQuestion, "Mark Open Question", hasDocument, activeIsDirty, 'Q'),
            Review(CodeCommandNames.MarkNeedsChange, "Mark Needs Change", hasDocument, activeIsDirty, 'N'),

            // Clearing is allowed on a dirty document, and is the only review
            // command that is. It records nothing about content, so there is no
            // content for it to be wrong about — and a reviewer who marked the
            // wrong file should not have to save it to undo that.
            HasReview
                ? new CodeCommand(
                    CodeCommandNames.ClearReview, "Clear Review",
                    hasDocument ? CommandAvailability.Enabled : CommandAvailability.Disabled,
                    AccessKey: 'C')
                : NoReview(CodeCommandNames.ClearReview, "Clear Review", 'C'),

            // A note is a question, not an attestation, so unlike the four
            // decisions it is allowed on a document with unsaved changes: a
            // reviewer who has to save before writing down what confuses them
            // stops writing it down. It still needs a name and a file.
            HasReview
                ? new CodeCommand(
                    CodeCommandNames.AddNote, "Add Note",
                    hasDocument && HasReviewer
                        ? CommandAvailability.Enabled
                        : CommandAvailability.Disabled,
                    hasDocument
                        ? HasReviewer ? null : "Set a reviewer name first — a note has to say who is asking."
                        : "Open a file to write a note about it.",
                    'T')
                : NoReview(CodeCommandNames.AddNote, "Add Note", 'T'),

            // Signing a unit is allowed on a document with unsaved changes, and
            // the four commands above are not. The difference is what each one
            // records: a file review names the exact content a human read, so
            // unsaved text would make it unverifiable by anyone; this writes a
            // name and no claim about content, because the fingerprint that binds
            // an approval to a version is written later by the owning component's
            // generator over the tree that was committed. A reviewer working down
            // a file would otherwise have to save between every declaration.
            Unit(CodeCommandNames.ApproveUnit, "Sign Unit as Reviewed", 'U'),
            Unit(CodeCommandNames.WithdrawUnit, "Withdraw Unit Signature", 'W'),

            HasReview
                ? new CodeCommand(
                    CodeCommandNames.ReviewCoverage, "Review Coverage",
                    hasWorkspace ? CommandAvailability.Enabled : CommandAvailability.Disabled,
                    AccessKey: 'V')
                : NoReview(CodeCommandNames.ReviewCoverage, "Review Coverage", 'V'),
        ];
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3E9EBA
    // Broiler-Falsified-If: a name that matches no command exactly returns a command instead of null
    // Broiler-Human:        PENDING
    public CodeCommand? Find(string name) =>
        GetCommands().FirstOrDefault(command => command.Name == name);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=76EF24
    // Broiler-Falsified-If: a host without file dialogs gets an Enabled Open or New Project entry
    // Broiler-Human:        PENDING
    private CodeCommand Picker(string name, string text, char accessKey) => HasFileDialogs
        ? new CodeCommand(name, text, CommandAvailability.Enabled, AccessKey: accessKey)
        : new CodeCommand(
            name, text, CommandAvailability.Unavailable,
            "This host has no way to ask for a file.", accessKey);

    /// <summary>
    /// A command that records a review decision.
    ///
    /// Every refusal carries the reason, because each one is a different problem
    /// with a different fix: compose a review pane, name a reviewer, open a file,
    /// or save it. A single greyed-out entry saying none of that is how a feature
    /// meant to be used daily stops being used.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A2E32B
    // Broiler-Falsified-If: a decision command is Enabled when no name is set for the person recording or the active document is dirty
    // Broiler-Human:        PENDING
    private CodeCommand Review(
        string name, string text, bool hasDocument, bool activeIsDirty, char accessKey)
    {
        if (!HasReview)
            return NoReview(name, text, accessKey);

        if (!HasReviewer)
        {
            return new CodeCommand(
                name, text, CommandAvailability.Disabled,
                "Set a reviewer name first — a review record has to say who made it.", accessKey);
        }

        if (!hasDocument)
            return new CodeCommand(name, text, CommandAvailability.Disabled, "Open a file to review it.", accessKey);

        if (activeIsDirty)
        {
            return new CodeCommand(
                name, text, CommandAvailability.Disabled,
                "Save the file first — a review records the content on disk.", accessKey);
        }

        return new CodeCommand(name, text, CommandAvailability.Enabled, AccessKey: accessKey);
    }

    /// <summary>
    /// A command that writes a reviewer's name onto one declaration.
    ///
    /// Withdrawing needs no reviewer — it removes a claim rather than making one,
    /// and a reviewer who signed the wrong declaration should not need a name
    /// recorded to take it back — but it does need a unit, so both share this.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BBAD01
    // Broiler-Falsified-If: Sign Unit as Reviewed is Enabled when no name is set for the person signing
    // Broiler-Human:        PENDING
    private CodeCommand Unit(string name, string text, char accessKey)
    {
        if (!HasReview)
            return NoReview(name, text, accessKey);

        bool signing = string.Equals(name, CodeCommandNames.ApproveUnit, StringComparison.Ordinal);
        if (signing && !HasReviewer)
        {
            return new CodeCommand(
                name, text, CommandAvailability.Disabled,
                "Set a reviewer name first — a signature has to say whose it is.", accessKey);
        }

        if (!HasAnnotatedUnit)
        {
            return new CodeCommand(
                name, text, CommandAvailability.Disabled,
                AssuranceUnitReason ?? "Put the caret inside an annotated declaration.", accessKey);
        }

        return new CodeCommand(name, text, CommandAvailability.Enabled, AccessKey: accessKey);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AA43AB
    // Broiler-Falsified-If: a host without a composed review pane gets a review command whose availability is anything but Unavailable
    // Broiler-Human:        PENDING
    private static CodeCommand NoReview(string name, string text, char accessKey) =>
        new(name, text, CommandAvailability.Unavailable,
            "This host does not compose the Human Review pane.", accessKey);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7BE37A
    // Broiler-Falsified-If: Build is Enabled while a build is already running
    // Broiler-Human:        PENDING
    private CodeCommand Build(string name, string text, bool hasWorkspace, char accessKey)
    {
        if (!HasBuildService)
        {
            return new CodeCommand(
                name, text, CommandAvailability.Unavailable,
                "No build service is attached to this host yet.", accessKey);
        }

        if (IsBuildRunning)
        {
            return new CodeCommand(
                name, text, CommandAvailability.Disabled, "A build is already running.", accessKey);
        }

        return new CodeCommand(
            name, text,
            hasWorkspace ? CommandAvailability.Enabled : CommandAvailability.Disabled,
            AccessKey: accessKey);
    }
}
