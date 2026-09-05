using System;
using System.Collections.Generic;
using System.Globalization;
using Broiler.Code.Review;
using Broiler.Code.Review.Assurance;
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
public sealed class ReviewPaneSource : IObservableTreeDataSource, IDisposable
{
    private const string StatusGroup = "group:status";
    private const string NotesGroup = "group:notes";
    private const string UnitGroup = "group:unit";
    private const string UnitsGroup = "group:units";

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
    public ReviewPaneSource(ReviewController controller, AssuranceController? assurance = null)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _controller.Changed += OnControllerChanged;

        _assurance = assurance;
        if (_assurance is not null)
            _assurance.Changed += OnControllerChanged;
    }

    public event EventHandler<TreeDataChangedEventArgs>? DataChanged;

    public TreeNodeId Root => new("review");

    /// <summary>The note a row stands for, or null for a heading or a status row.</summary>
    public AnchoredNote? NoteFor(TreeNodeId node)
    {
        EnsureBuilt();
        return _rows.TryGetValue(node.Value, out Row? row) ? row.Note : null;
    }

    /// <summary>
    /// The code unit a row stands for, or null. Activating one of these rows is
    /// how a reviewer walks a file declaration by declaration.
    /// </summary>
    public AssuranceUnit? UnitFor(TreeNodeId node)
    {
        EnsureBuilt();
        return _rows.TryGetValue(node.Value, out Row? row) ? row.Unit : null;
    }

    public void Refresh()
    {
        _valid = false;
        DataChanged?.Invoke(this, new TreeDataChangedEventArgs(TreeNodeId.None));
    }

    public int GetChildCount(TreeNodeId node)
    {
        EnsureBuilt();
        if (node.Value == Root.Value)
            return _groups.Count;
        return _rows.TryGetValue(node.Value, out Row? row) ? row.Children.Count : 0;
    }

    public TreeNodeId GetChild(TreeNodeId node, int index)
    {
        EnsureBuilt();
        return new TreeNodeId(node.Value == Root.Value ? _groups[index] : _rows[node.Value].Children[index]);
    }

    public bool CanExpand(TreeNodeId node) => GetChildCount(node) > 0;

    public TreeNodePresentation GetPresentation(TreeNodeId node)
    {
        EnsureBuilt();
        return _rows.TryGetValue(node.Value, out Row? row)
            ? new TreeNodePresentation(node, row.Label, row.Secondary, row.IconKey, row.Decoration)
            : new TreeNodePresentation(node, node.Value);
    }

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
    public static TreeNodeDecoration DecorationFor(ReviewState state) => state switch
    {
        { Status: ReviewStatus.NeedsChange } => TreeNodeDecoration.Error,
        { Freshness: ReviewFreshness.Stale } => TreeNodeDecoration.Warning,
        { Status: ReviewStatus.Question } => TreeNodeDecoration.Warning,
        { Status: ReviewStatus.Reviewed } => TreeNodeDecoration.Information,
        _ => TreeNodeDecoration.None,
    };

    private void OnControllerChanged(object? sender, EventArgs e) => Refresh();

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
            Add(group, "unit:exempt", "Exempt", unit.Exemption);

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
    private void BuildUnits()
    {
        if (_assurance is not { IsAnnotatedFile: true } assurance || assurance.Units.Count == 0)
            return;

        int relevant = 0;
        int reviewed = 0;
        foreach (AssuranceUnit unit in assurance.Units)
        {
            if (unit.IsExempt)
                continue;

            relevant++;
            if (unit.State == AssuranceUnitState.Verified)
                reviewed++;
        }

        var group = new Row(
            UnitsGroup,
            "Units",
            string.Create(CultureInfo.InvariantCulture, $"{reviewed} of {relevant} reviewed"),
            "notes",
            TreeNodeDecoration.None);

        _rows[UnitsGroup] = group;
        _groups.Add(UnitsGroup);

        int ordinal = 0;
        foreach (AssuranceUnit unit in assurance.Units)
        {
            string key = string.Create(CultureInfo.InvariantCulture, $"unit:{ordinal++}");
            _rows[key] = new Row(
                key,
                unit.DisplayName,
                AssuranceStateMachine.ToDisplayString(unit.State),
                unit.IsExempt ? "detail" : "review",
                DecorationFor(unit.State))
            {
                Unit = unit,
            };

            group.Children.Add(key);
        }
    }

    /// <summary>
    /// A field key as a row label. The format's keys are terse because they are
    /// written inside a comment; a pane has room for the words.
    /// </summary>
    private static string Humanize(string key) => key switch
    {
        "IP" => "IP risk",
        "Security" => "Security risk",
        "Resources" => "Resource impact",
        "Spec" => "Specification",
        "EXEMPT" => "Exempt because",
        _ => key,
    };

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

    private static string KindIcon(ReviewNoteKind kind) => kind switch
    {
        ReviewNoteKind.Concern => "concern",
        ReviewNoteKind.Todo => "todo",
        ReviewNoteKind.Observation => "observation",
        _ => "question",
    };

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
