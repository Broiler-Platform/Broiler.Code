// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// HAIRS Code Assurance
// --------------------
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
// Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6788B5
// Hairs-Falsified-If: activating a diagnostic row resolves through EntryFor to another row's entry, so it navigates to a different diagnostic's location
// Hairs-Human:        PENDING
public sealed class ProblemsTreeSource(ProblemsModel model) : IObservableTreeDataSource
{
    private readonly ProblemsModel _model = model ?? throw new ArgumentNullException(nameof(model));
    private readonly Dictionary<string, ProblemEntry> _entriesByNode = [];
    private readonly List<string> _documents = [];
    private readonly Dictionary<string, List<string>> _children = [];
    private bool _valid;

    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CFCEED
    // Hairs-Human:        PENDING
    public event EventHandler<TreeDataChangedEventArgs>? DataChanged;

    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DCF68B
    // Hairs-Human:        PENDING
    public TreeNodeId Root => new("problems");

    /// <summary>The entry a node stands for, or null for a document group.</summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=13AFAB
    // Hairs-Falsified-If: a document group node yields a diagnostic entry instead of null
    // Hairs-Human:        PENDING
    public ProblemEntry? EntryFor(TreeNodeId node)
    {
        EnsureBuilt();
        return _entriesByNode.GetValueOrDefault(node.Value);
    }

    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=1C7BFE
    // Hairs-Human:        PENDING
    public void Refresh()
    {
        _valid = false;
        DataChanged?.Invoke(this, new TreeDataChangedEventArgs(TreeNodeId.None));
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=678B09
    // Hairs-Falsified-If: a document group's child count differs from the number of diagnostics grouped under that document
    // Hairs-Human:        PENDING
    public int GetChildCount(TreeNodeId node)
    {
        EnsureBuilt();
        if (node.Value == Root.Value)
            return _documents.Count;
        return _children.TryGetValue(node.Value, out List<string>? children) ? children.Count : 0;
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=317DE1
    // Hairs-Falsified-If: the child at an index of a document group is an entry grouped under a different document
    // Hairs-Human:        PENDING
    public TreeNodeId GetChild(TreeNodeId node, int index)
    {
        EnsureBuilt();
        return new TreeNodeId(node.Value == Root.Value
            ? $"doc:{_documents[index]}"
            : _children[node.Value][index]);
    }

    // Hairs-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=C07485
    // Hairs-Falsified-If: a diagnostic row reports itself expandable
    // Hairs-Human:        PENDING
    public bool CanExpand(TreeNodeId node) => GetChildCount(node) > 0;

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F93F8F
    // Hairs-Falsified-If: a document-scoped diagnostic on the first line of its file is shown as line 0
    // Hairs-Human:        PENDING
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

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5E45A0
    // Hairs-Falsified-If: two identical diagnostics on the same line produce one row instead of two
    // Hairs-Human:        PENDING
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

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D20199
    // Hairs-Human:        PENDING
    private static string ShortName(string path)
    {
        int slash = path.LastIndexOfAny(['/', '\\']);
        return slash < 0 ? path : path[(slash + 1)..];
    }
}
