// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// HAIRS Code Assurance
// --------------------
// Relevant units:   32
// Annotated:        32/32
// Exempt:           10
// Human-reviewed:   0/32
// IP risk:          Low
// Security risk:    Medium
// Criteria:         25/0
// Resource impact:  4/10 max
// Unverified:       32
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using Hairs;
using Hairs.Assurance;
using Broiler.UI.TreeView;

namespace Broiler.Code.Core.Review;

/// <summary>
/// The right-hand pane: what is recorded about the file on screen, and what is
/// still open on it.
///
/// A tree over <see cref="UiTreeView"/> rather than a bespoke panel, for the
/// same reason the Problems pane is one — it inherits virtualization, keyboard
/// navigation, and the semantics that announce a row's level and position, none
/// of which a hand-drawn panel would have. A file with two hundred notes scrolls
/// here without anyone having written scrolling.
///
/// This data source is read-only: it renders state and writes nothing. The pane
/// the user sees is not — <c>CodeShell.ComposeReview</c> docks a note field
/// beneath this tree — but that field submits the <c>Add Note</c> command like
/// every other review action, so there is still one path that writes a record
/// and one place that decides whether a write is allowed.
/// </summary>
// Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F1FB49
// Hairs-Falsified-If: a Stale, New or Unknown unit is counted in the pane's reviewed total or given the Information decoration of a unit whose signature still matches its fingerprint
// Hairs-Human:        PENDING
public sealed class ReviewPaneSource : IObservableTreeDataSource, IDisposable
{
    // Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=DC24BC
    // Hairs-Human:        PENDING
    private const string StatusGroup = "group:status";
    // Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=B8E44B
    // Hairs-Human:        PENDING
    private const string NotesGroup = "group:notes";
    // Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=2E7AF5
    // Hairs-Human:        PENDING
    private const string UnitGroup = "group:unit";
    // Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=1B7DFD
    // Hairs-Human:        PENDING
    private const string UnitsGroup = "group:units";
    // Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=FCE5F9
    // Hairs-Human:        PENDING
    private const string ExemptGroup = "group:units.exempt";

    private readonly ReviewController _controller;
    private readonly AssuranceController? _assurance;
    private readonly Dictionary<string, Row> _rows = [];
    private readonly List<string> _groups = [];
    private bool _valid;
    private bool _disposed;

    /// <param name="assurance">
    /// The per-unit assurance half, when the host composed one. Optional for the
    /// same reason the whole review pane is optional to a head: a workspace of
    /// files that carry no assurance annotations is the normal case, and the two
    /// sections it adds simply do not appear.
    /// </param>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9837B1
    // Hairs-Falsified-If: a null controller is accepted and the first rebuild throws NullReferenceException instead of the constructor throwing ArgumentNullException
    // Hairs-Human:        PENDING
    public ReviewPaneSource(ReviewController controller, AssuranceController? assurance = null)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _controller.Changed += OnControllerChanged;

        _assurance = assurance;
        if (_assurance is not null)
            _assurance.Changed += OnControllerChanged;
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CFCEED
    // Hairs-Human:        PENDING
    public event EventHandler<TreeDataChangedEventArgs>? DataChanged;

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8F478C
    // Hairs-Falsified-If: a row key equal to the root id makes GetChildCount answer that row with the group count
    // Hairs-Human:        PENDING
    public TreeNodeId Root => new("review");

    /// <summary>The note a row stands for, or null for a heading or a status row.</summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=134AE9
    // Hairs-Falsified-If: a heading, status or unit row returns a note, so activating it moves the caret to code the row does not describe
    // Hairs-Human:        PENDING
    public AnchoredNote? NoteFor(TreeNodeId node)
    {
        EnsureBuilt();
        return _rows.TryGetValue(node.Value, out Row? row) ? row.Note : null;
    }

    /// <summary>
    /// The code unit a row stands for, or null. Activating one of these rows is
    /// how a reviewer walks a file declaration by declaration.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=42A63A
    // Hairs-Falsified-If: a row for one declaration returns the unit of another, so activating it moves the caret to the wrong declaration
    // Hairs-Human:        PENDING
    public AssuranceUnit? UnitFor(TreeNodeId node)
    {
        EnsureBuilt();
        return _rows.TryGetValue(node.Value, out Row? row) ? row.Unit : null;
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1C7BFE
    // Hairs-Falsified-If: after a controller change the next query returns rows built for the previous file because the cache was not invalidated
    // Hairs-Human:        PENDING
    public void Refresh()
    {
        _valid = false;
        DataChanged?.Invoke(this, new TreeDataChangedEventArgs(TreeNodeId.None));
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EC9767
    // Hairs-Falsified-If: a node id with no row reports a nonzero child count
    // Hairs-Human:        PENDING
    public int GetChildCount(TreeNodeId node)
    {
        EnsureBuilt();
        if (node.Value == Root.Value)
            return _groups.Count;
        return _rows.TryGetValue(node.Value, out Row? row) ? row.Children.Count : 0;
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=062381
    // Hairs-Falsified-If: an index below GetChildCount for the same node throws or yields an id that has no row
    // Hairs-Human:        PENDING
    public TreeNodeId GetChild(TreeNodeId node, int index)
    {
        EnsureBuilt();
        return new TreeNodeId(node.Value == Root.Value ? _groups[index] : _rows[node.Value].Children[index]);
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C07485
    // Hairs-Falsified-If: a group with child rows reports that it cannot expand
    // Hairs-Human:        PENDING
    public bool CanExpand(TreeNodeId node) => GetChildCount(node) > 0;

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0FA67C
    // Hairs-Falsified-If: an id with no row throws instead of being presented under its own key
    // Hairs-Human:        PENDING
    public TreeNodePresentation GetPresentation(TreeNodeId node)
    {
        EnsureBuilt();
        return _rows.TryGetValue(node.Value, out Row? row)
            ? new TreeNodePresentation(node, row.Label, row.Secondary, row.IconKey, row.Decoration)
            : new TreeNodePresentation(node, node.Value);
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D2D5E4
    // Hairs-Falsified-If: after Dispose a Changed event from either controller still raises DataChanged
    // Hairs-Human:        PENDING
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _controller.Changed -= OnControllerChanged;

        if (_assurance is not null)
            _assurance.Changed -= OnControllerChanged;
    }

    /// <summary>
    /// Maps a unit's assurance state onto the decorations the tree already has.
    ///
    /// Pending is deliberately undecorated. Every relevant unit in the component
    /// this format was built for is pending today, and marking all of them would
    /// paint a file entirely yellow to say nothing — the same reason the Solution
    /// Explorer leaves an unreviewed file unbadged. What is decorated is what a
    /// reviewer has to act on: an approval the code has moved out from under, a
    /// relevant declaration carrying no assessment at all, and a state this build
    /// cannot establish.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5CD55F
    // Hairs-Falsified-If: a Stale or Unknown unit state maps to Information or None, the weight given to a signature whose fingerprint still matches
    // Hairs-Human:        PENDING
    public static TreeNodeDecoration DecorationFor(AssuranceUnitState state) => state switch
    {
        AssuranceUnitState.New => TreeNodeDecoration.Error,
        AssuranceUnitState.Stale => TreeNodeDecoration.Warning,
        AssuranceUnitState.Unknown => TreeNodeDecoration.Warning,
        AssuranceUnitState.Verified => TreeNodeDecoration.Information,
        AssuranceUnitState.HumanApprovedPendingFingerprint => TreeNodeDecoration.Information,
        _ => TreeNodeDecoration.None,
    };

    /// <summary>
    /// Maps a review state onto the decorations the tree already has.
    ///
    /// The vocabulary is reused rather than extended. <see cref="UiTreeView"/>
    /// lives in Broiler.UI, and adding a review-specific decoration there would
    /// put a product concept into a general control — and, in this repository,
    /// turn a self-contained feature into a submodule change. Warning for a
    /// stale approval and Error for a demanded change carry the right weight
    /// without inventing anything.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4C572B
    // Hairs-Falsified-If: a Reviewed record whose freshness is Unknown maps to Information, the same decoration as one whose content hash still matches
    // Hairs-Human:        PENDING
    public static TreeNodeDecoration DecorationFor(ReviewState state) => state switch
    {
        { Status: ReviewStatus.NeedsChange } => TreeNodeDecoration.Error,
        { Freshness: ReviewFreshness.Stale } => TreeNodeDecoration.Warning,
        { Status: ReviewStatus.Question } => TreeNodeDecoration.Warning,
        { Status: ReviewStatus.Reviewed } => TreeNodeDecoration.Information,
        _ => TreeNodeDecoration.None,
    };

    private void OnControllerChanged(object? sender, EventArgs e) => Refresh();

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=162AF5
    // Hairs-Falsified-If: rows or groups from the previous build survive a rebuild because the tables are not cleared first
    // Hairs-Human:        PENDING
    private void EnsureBuilt()
    {
        if (_valid)
            return;

        _rows.Clear();
        _groups.Clear();

        BuildUnit();
        BuildStatus();
        BuildNotes();
        BuildUnits();

        _valid = true;
    }

    /// <summary>
    /// What the assurance annotation on the declaration under the caret records.
    ///
    /// First in the pane, above the file's own review, because it is the section
    /// that answers the question a reviewer has while reading: <em>what is
    /// claimed about this, and by whom?</em> The file-level record below it
    /// answers a different one, and the two are kept visibly apart.
    ///
    /// Nothing at all is shown for a file that carries no annotations, which is
    /// most files. A section that appeared everywhere saying "not applicable"
    /// would be noise on every other file in the workspace.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BAAA62
    // Hairs-Falsified-If: a machine line that repeats a key shows the later value on two rows while the annotation's Field lookup reads the first
    // Hairs-Human:        PENDING
    private void BuildUnit()
    {
        if (_assurance is not { IsAnnotatedFile: true } assurance)
            return;

        if (assurance.CurrentUnit is not { } unit)
        {
            var empty = new Row(UnitGroup, "Assurance", "no declaration selected", "review", TreeNodeDecoration.None);
            _rows[UnitGroup] = empty;
            _groups.Add(UnitGroup);
            Add(empty, "unit:none", "Put the caret in an annotated declaration", "to review it");
            return;
        }

        var group = new Row(
            UnitGroup,
            "Assurance",
            unit.DisplayName,
            "review",
            DecorationFor(unit.State));

        _rows[UnitGroup] = group;
        _groups.Add(UnitGroup);

        Add(group, "unit:state", "State", AssuranceStateMachine.ToDisplayString(unit.State), DecorationFor(unit.State));

        if (unit.IsExempt)
            Add(group, "unit:exempt", "Exempt", ExemptionReason(unit) ?? unit.Exemption);

        if (unit.Annotation is { } annotation)
        {
            foreach (AssuranceField field in annotation.Fields)
            {
                // The machine's fields are shown in the order the source wrote
                // them rather than in an order this pane prefers, so a reader can
                // hold the row against the line it came from.
                Add(group, "unit:field:" + field.Key, Humanize(field.Key), field.Value);
            }

            if (annotation.HasCriterion)
            {
                // The sentence the unit is meant to be falsifiable by. It is the
                // thing a reviewer of a high-security declaration is being asked
                // to argue with, so it gets a row of its own rather than a
                // tooltip.
                Add(group, "unit:criterion", "Falsified if", annotation.Criterion);
            }

            Add(group, "unit:human", "Human line", annotation.HumanBody);
        }

        // Only when a language service is composed. Without one the fingerprint
        // is unknown, and an empty row would read as "there isn't one".
        if (unit.Fingerprint is { } fingerprint)
            Add(group, "unit:current", "Fingerprint now", fingerprint);
    }

    /// <summary>
    /// Every unit in the file, so the pane is a way through it and not only a
    /// readout of wherever the caret happens to be.
    ///
    /// Activating a row moves the caret to the declaration, which is the same
    /// gesture that already moves it to a note's code. Working down a file
    /// declaration by declaration is the task this whole workspace exists for,
    /// and a list is what makes it a list of things to do rather than a hunt.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8D6C63
    // Hairs-Falsified-If: a Stale, New, Unknown or signed-without-fingerprint unit is counted in the 'of M reviewed' total, or an exempt unit is counted in M
    // Hairs-Human:        PENDING
    private void BuildUnits()
    {
        if (_assurance is not { IsAnnotatedFile: true } assurance || assurance.Units.Count == 0)
            return;

        int relevant = 0;
        int reviewed = 0;
        int signed = 0;
        int exempt = 0;
        foreach (AssuranceUnit unit in assurance.Units)
        {
            if (unit.IsExempt)
            {
                exempt++;
                continue;
            }

            relevant++;
            if (unit.State == AssuranceUnitState.Verified)
                reviewed++;
            else if (unit.State == AssuranceUnitState.HumanApprovedPendingFingerprint)
                signed++;
        }

        // Signed units are counted apart from verified ones. Signing is not
        // verifying — the fingerprint that verifies is the owning component's
        // generator's to write — so a reviewer who has just put their name on
        // every declaration in the file would read "0 of 4 reviewed" and
        // conclude nothing happened.
        string counted = string.Create(CultureInfo.InvariantCulture, $"{reviewed} of {relevant} reviewed");
        if (signed > 0)
            counted += string.Create(CultureInfo.InvariantCulture, $", {signed} signed");

        var toReview = new Row(UnitsGroup, "Units — to review", counted, "notes", TreeNodeDecoration.None);
        _rows[UnitsGroup] = toReview;
        _groups.Add(UnitsGroup);

        // A second group rather than a run of rows inside the first. On the file
        // this format was built for, nine of thirteen declarations are exempt,
        // and mixed together the four a reviewer has to act on are four rows to
        // find among nine that are already answered. Split, the first group is
        // the work and the second is the evidence that the rest was considered —
        // and it collapses, which the work should not.
        Row? exemptGroup = null;
        if (exempt > 0)
        {
            exemptGroup = new Row(
                ExemptGroup,
                "Units — exempt",
                string.Create(CultureInfo.InvariantCulture, $"{exempt} the format expects no review on"),
                "detail",
                TreeNodeDecoration.None);

            _rows[ExemptGroup] = exemptGroup;
            _groups.Add(ExemptGroup);
        }

        int ordinal = 0;
        foreach (AssuranceUnit unit in assurance.Units)
        {
            string key = string.Create(CultureInfo.InvariantCulture, $"unit:{ordinal++}");
            _rows[key] = new Row(
                key,
                unit.DisplayName,
                UnitSummary(unit),
                unit.IsExempt ? "detail" : "review",
                DecorationFor(unit.State))
            {
                Unit = unit,
            };

            // Document order is kept within each group, so a reviewer working
            // down the first one is working down the file.
            (unit.IsExempt ? exemptGroup! : toReview).Children.Add(key);
        }
    }

    /// <summary>
    /// What a unit's row says beside its name: its state, and for an exempt one
    /// the reason it is exempt.
    ///
    /// The reason rather than the bare word, because "exempt" on nine of a
    /// file's thirteen rows tells a reviewer only that somebody decided
    /// something. Whether a declaration is exempt because it is an auto-property
    /// or because it sits inside an assembly marker is the difference between
    /// agreeing with the exemption and going to look at it — and a reviewer who
    /// cannot tell has to open the other tool's report to find out.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BD3E89
    // Hairs-Falsified-If: a non-exempt unit is summarised as exempt, or an exempt one shows its state text instead of its exemption
    // Hairs-Human:        PENDING
    private static string UnitSummary(AssuranceUnit unit)
    {
        if (!unit.IsExempt)
            return AssuranceStateMachine.ToDisplayString(unit.State);

        return ExemptionReason(unit) is { } reason ? $"exempt: {reason}" : "exempt";
    }

    /// <summary>
    /// Why a declaration is exempt, in words, or null when nothing says.
    ///
    /// A source that states its own reason wins: that is a sentence somebody
    /// wrote about this declaration, and no rendering here improves on it.
    /// Otherwise the scanner's case is spelled out. The identifiers are the
    /// owning component's, ported case for case by
    /// <c>CSharpAssuranceScanner</c>, and are rendered rather than shown raw for
    /// the same reason the machine line's keys are — the format writes them
    /// inside a comment, and a pane has room for the words.
    ///
    /// An identifier this build does not know is passed through unchanged. That
    /// component may add a ninth case before this one hears about it, and a row
    /// reading <c>exempt: SomeNewCase</c> is true and searchable, where "exempt"
    /// alone would have quietly dropped the answer.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=756211
    // Hairs-Falsified-If: a unit whose source states its own exemption reason is shown with the scanner's case text instead
    // Hairs-Human:        PENDING
    private static string? ExemptionReason(AssuranceUnit unit)
    {
        if (unit.Annotation?.ExemptReason is { Length: > 0 } stated)
            return stated;

        return unit.Exemption switch
        {
            "TrivialPropertyOrAccessor" => "trivial property or accessor",
            "ParameterAssigningConstructor" => "constructor assigns its parameters only",
            "TrivialExpressionBodiedMember" => "trivial expression body",
            "CompilerSuppliedRecordOrEnumMember" => "compiler-supplied record or enum member",
            "DelegatingOverrideOrOperator" => "override or operator that only delegates",
            "InsideAssemblyMarker" => "inside the assembly marker",
            "FieldDeclaringStorage" => "field declaring storage",
            "EnumMemberOfADeclaredVocabulary" => "enum member of a declared vocabulary",
            "NamedValue" => "named value the component watches",
            "DeclaredInSource" => "declared exempt in the source",
            null or "" or "None" => null,
            var other => other,
        };
    }

    /// <summary>
    /// A field key as a row label. The format's keys are terse because they are
    /// written inside a comment; a pane has room for the words.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D239E4
    // Hairs-Falsified-If: a key the table does not list is shown as something other than the key itself
    // Hairs-Human:        PENDING
    private static string Humanize(string key) => key switch
    {
        "IP" => "IP risk",
        "Security" => "Security risk",
        "Resources" => "Resource impact",
        "CS" => "Conceptual Stability",
        "CL" => "Change Likelihood",
        "Spec" => "Specification",
        "EXEMPT" => "Exempt because",
        _ => key,
    };

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8FB5BB
    // Hairs-Falsified-If: a record whose freshness is Stale is shown without the 'Changed since' warning row
    // Hairs-Human:        PENDING
    private void BuildStatus()
    {
        FileReview review = _controller.CurrentReview;
        ReviewState state = _controller.CurrentState;

        var group = new Row(StatusGroup, "Human Review", null, "review", DecorationFor(state));
        _rows[StatusGroup] = group;
        _groups.Add(StatusGroup);

        Add(group, "status", "Status", state.ToDisplayString(), DecorationFor(state));

        if (review.Reviewer.Length > 0)
            Add(group, "reviewer", "Reviewer", review.Reviewer);

        if (review.ReviewedAt is { } at)
        {
            Add(group, "reviewedAt", "Reviewed at",
                at.ToUniversalTime().ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture));
        }

        if (review.ReviewedRevision is { Length: > 0 } revision)
        {
            // Abbreviated the way git does, because the full forty characters
            // tell a reader nothing the first seven do not.
            Add(group, "revision", "Reviewed at revision",
                revision.Length > 7 ? revision[..7] : revision);
        }

        if (state.Freshness == ReviewFreshness.Stale)
        {
            Add(group, "stale", "Changed since",
                "the reviewed content — re-review needed", TreeNodeDecoration.Warning);
        }

        if (state.Status == ReviewStatus.Unreviewed && review.Notes.Count == 0)
            Add(group, "empty", "Nothing recorded yet", "mark it reviewed, or add a note");
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=BA11E2
    // Hairs-Falsified-If: an orphaned or ambiguous note is listed without its warning row, so its line number reads as trustworthy
    // Hairs-Human:        PENDING
    private void BuildNotes()
    {
        IReadOnlyList<AnchoredNote> notes = _controller.CurrentNotes;
        if (notes.Count == 0)
            return;

        int open = 0;
        foreach (AnchoredNote note in notes)
        {
            if (note.Note.IsOpen)
                open++;
        }

        var group = new Row(
            NotesGroup,
            "Notes",
            open == 0
                ? string.Create(CultureInfo.InvariantCulture, $"{notes.Count} resolved")
                : string.Create(CultureInfo.InvariantCulture, $"{open} open of {notes.Count}"),
            "notes",
            open > 0 ? TreeNodeDecoration.Warning : TreeNodeDecoration.None);

        _rows[NotesGroup] = group;
        _groups.Add(NotesGroup);

        int ordinal = 0;
        foreach (AnchoredNote note in notes)
        {
            string key = string.Create(CultureInfo.InvariantCulture, $"note:{ordinal++}");
            var row = new Row(key, note.Note.Text, PositionOf(note), KindIcon(note.Note.Kind), NoteDecoration(note))
            {
                Note = note,
            };

            _rows[key] = row;
            group.Children.Add(key);

            if (note.Note.Resolution is { } resolution)
                Add(row, key + ":resolution", "Resolved", resolution.Text);

            // An anchor that could not be placed is called out on its own row
            // rather than folded into the secondary label. A note whose code has
            // gone is the case where the reviewer most needs to be told why the
            // line number beside it is not to be trusted.
            if (note.Status == ReviewAnchorStatus.Orphaned)
                Add(row, key + ":anchor", "Anchored code no longer present", null, TreeNodeDecoration.Warning);
            else if (note.Status == ReviewAnchorStatus.Ambiguous)
                Add(row, key + ":anchor", "Anchored code appears more than once", null, TreeNodeDecoration.Warning);
        }
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1F29C6
    // Hairs-Falsified-If: an orphaned or ambiguous note is labelled with a line number or symbol instead of its anchor state
    // Hairs-Human:        PENDING
    private static string PositionOf(AnchoredNote note) => note.Status switch
    {
        ReviewAnchorStatus.FileLevel => "whole file",
        ReviewAnchorStatus.Orphaned => "anchor lost",
        ReviewAnchorStatus.Ambiguous => "ambiguous anchor",
        _ when note.Note.Anchor.Symbol is { Length: > 0 } symbol => symbol,
        _ when note.StartLine == note.EndLine =>
            string.Create(CultureInfo.InvariantCulture, $"line {note.StartLine + 1}"),
        _ => string.Create(CultureInfo.InvariantCulture, $"lines {note.StartLine + 1}–{note.EndLine + 1}"),
    };

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=486417
    // Hairs-Falsified-If: an unresolved Concern note gets Warning, Information or None instead of Error
    // Hairs-Human:        PENDING
    private static TreeNodeDecoration NoteDecoration(AnchoredNote note)
    {
        if (note.Note.Resolution is not null)
            return TreeNodeDecoration.None;

        return note.Note.Kind switch
        {
            ReviewNoteKind.Concern => TreeNodeDecoration.Error,
            ReviewNoteKind.Question or ReviewNoteKind.Todo => TreeNodeDecoration.Warning,
            _ => TreeNodeDecoration.Information,
        };
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=191384
    // Hairs-Falsified-If: a Concern, Todo or Observation note is given the question icon
    // Hairs-Human:        PENDING
    private static string KindIcon(ReviewNoteKind kind) => kind switch
    {
        ReviewNoteKind.Concern => "concern",
        ReviewNoteKind.Todo => "todo",
        ReviewNoteKind.Observation => "observation",
        _ => "question",
    };

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=46C1C3
    // Hairs-Falsified-If: a key that already has a row is replaced in the table and listed a second time under its parent
    // Hairs-Human:        PENDING
    private void Add(
        Row parent,
        string key,
        string label,
        string? secondary = null,
        TreeNodeDecoration decoration = TreeNodeDecoration.None)
    {
        _rows[key] = new Row(key, label, secondary, "detail", decoration);
        parent.Children.Add(key);
    }

    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=944ECD
    // Hairs-Human:        PENDING
    private sealed record Row(
        string Key,
        string Label,
        string? Secondary,
        string IconKey,
        TreeNodeDecoration Decoration)
    {
        public List<string> Children { get; } = [];

        public AnchoredNote? Note { get; init; }

        public AssuranceUnit? Unit { get; init; }
    }
}
