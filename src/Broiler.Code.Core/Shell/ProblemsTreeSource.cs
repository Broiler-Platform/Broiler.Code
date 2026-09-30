// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           5
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    Medium
// Criteria:         7/0
// Resource impact:  4/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using Broiler.Code.Core.Diagnostics;
using Broiler.UI.CodeEditor;
using Broiler.UI.TreeView;

namespace Broiler.Code.Core.Shell;

/// <summary>
/// Presents the Problems model as a tree grouped by document.
///
/// A tree rather than a flat list because that is what the data is: diagnostics
/// belong to files. Reusing <see cref="UiTreeView"/> also means the Problems
/// pane inherits its virtualization, keyboard navigation, and the semantics
/// that announce a row's level and position — none of which would exist in a
/// bespoke list.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6788B5
// Broiler-Falsified-If: activating a diagnostic row resolves through EntryFor to another row's entry, so it navigates to a different diagnostic's location
// Broiler-Human:        PENDING
public sealed class ProblemsTreeSource(ProblemsModel model) : IObservableTreeDataSource
{
    private readonly ProblemsModel _model = model ?? throw new ArgumentNullException(nameof(model));
    private readonly Dictionary<string, ProblemEntry> _entriesByNode = [];
    private readonly List<string> _documents = [];
    private readonly Dictionary<string, List<string>> _children = [];
    private bool _valid;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CFCEED
    // Broiler-Human:        PENDING
    public event EventHandler<TreeDataChangedEventArgs>? DataChanged;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DCF68B
    // Broiler-Human:        PENDING
    public TreeNodeId Root => new("problems");

    /// <summary>The entry a node stands for, or null for a document group.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=13AFAB
    // Broiler-Falsified-If: a document group node yields a diagnostic entry instead of null
    // Broiler-Human:        PENDING
    public ProblemEntry? EntryFor(TreeNodeId node)
    {
        EnsureBuilt();
        return _entriesByNode.GetValueOrDefault(node.Value);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=1C7BFE
    // Broiler-Human:        PENDING
    public void Refresh()
    {
        _valid = false;
        DataChanged?.Invoke(this, new TreeDataChangedEventArgs(TreeNodeId.None));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=678B09
    // Broiler-Falsified-If: a document group's child count differs from the number of diagnostics grouped under that document
    // Broiler-Human:        PENDING
    public int GetChildCount(TreeNodeId node)
    {
        EnsureBuilt();
        if (node.Value == Root.Value)
            return _documents.Count;
        return _children.TryGetValue(node.Value, out List<string>? children) ? children.Count : 0;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=317DE1
    // Broiler-Falsified-If: the child at an index of a document group is an entry grouped under a different document
    // Broiler-Human:        PENDING
    public TreeNodeId GetChild(TreeNodeId node, int index)
    {
        EnsureBuilt();
        return new TreeNodeId(node.Value == Root.Value
            ? $"doc:{_documents[index]}"
            : _children[node.Value][index]);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=C07485
    // Broiler-Falsified-If: a diagnostic row reports itself expandable
    // Broiler-Human:        PENDING
    public bool CanExpand(TreeNodeId node) => GetChildCount(node) > 0;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F93F8F
    // Broiler-Falsified-If: a document-scoped diagnostic on the first line of its file is shown as line 0
    // Broiler-Human:        PENDING
    public TreeNodePresentation GetPresentation(TreeNodeId node)
    {
        EnsureBuilt();

        if (_entriesByNode.TryGetValue(node.Value, out ProblemEntry? entry))
        {
            return new TreeNodePresentation(
                node,
                entry.Message,
                entry.IsProjectLevel
                    ? entry.Code
                    : string.Create(
                        CultureInfo.InvariantCulture,
                        $"{entry.Code}  line {entry.Line + 1}"),
                "diagnostic",
                entry.Severity switch
                {
                    CodeDiagnosticSeverity.Error => TreeNodeDecoration.Error,
                    CodeDiagnosticSeverity.Warning => TreeNodeDecoration.Warning,
                    CodeDiagnosticSeverity.Information => TreeNodeDecoration.Information,
                    _ => TreeNodeDecoration.None,
                });
        }

        string document = node.Value.StartsWith("doc:", StringComparison.Ordinal)
            ? node.Value[4..]
            : node.Value;
        int count = _children.TryGetValue(node.Value, out List<string>? children) ? children.Count : 0;
        return new TreeNodePresentation(node, ShortName(document), $"{count}", "file");
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5E45A0
    // Broiler-Falsified-If: two identical diagnostics on the same line produce one row instead of two
    // Broiler-Human:        PENDING
    private void EnsureBuilt()
    {
        if (_valid)
            return;

        _entriesByNode.Clear();
        _documents.Clear();
        _children.Clear();

        int ordinal = 0;
        foreach ((string document, IReadOnlyList<ProblemEntry> entries) in _model.GetGroupedByDocument())
        {
            string documentKey = $"doc:{document}";
            _documents.Add(document);
            var childKeys = new List<string>(entries.Count);

            foreach (ProblemEntry entry in entries)
            {
                // Keyed by ordinal rather than by content: two identical
                // diagnostics on one line are two rows, and a key collision
                // would silently drop one.
                string key = $"entry:{ordinal++}";
                _entriesByNode[key] = entry;
                childKeys.Add(key);
            }

            _children[documentKey] = childKeys;
        }

        _valid = true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D20199
    // Broiler-Human:        PENDING
    private static string ShortName(string path)
    {
        int slash = path.LastIndexOfAny(['/', '\\']);
        return slash < 0 ? path : path[(slash + 1)..];
    }
}
