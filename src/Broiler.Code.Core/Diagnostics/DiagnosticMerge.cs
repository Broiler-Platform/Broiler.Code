// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// HAIRS Code Assurance
// --------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           8
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Medium
// Criteria:         8/0
// Resource impact:  3/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Linq;
using Broiler.UI.CodeEditor;

namespace Broiler.Code.Core.Diagnostics;

/// <summary>Where a diagnostic came from. Merging depends on it.</summary>
// Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=F3F31E
// Hairs-Human:        PENDING
public enum DiagnosticOrigin
{
    /// <summary>The live language service, for the snapshot being edited.</summary>
    Live,

    /// <summary>An authoritative build, for the snapshot it compiled.</summary>
    Build,

    /// <summary>The workspace or the toolchain; usually has no source span.</summary>
    Workspace,
}

/// <summary>
/// One diagnostic with everything the merge needs to decide precedence.
/// </summary>
// Hairs-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=CD170A
// Hairs-Falsified-If: a live and a build diagnostic with the same code, document and span but different message text are both kept by the merge
// Hairs-Human:        PENDING
public sealed record MergedDiagnostic(
    DiagnosticOrigin Origin,
    string DocumentPath,
    CodeDiagnosticAdornment Adornment,
    int SnapshotVersion,
    string? BuildId = null,
    string? TargetFramework = null)
{
    /// <summary>
    /// Identity for deduplication: code, document, and span. Not the message —
    /// it is localized, and the live service and the build worker may be
    /// running different compiler builds that word it differently.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8DBAFC
    // Hairs-Falsified-If: two diagnostics with the same code, document, start and length but different message text produce different keys
    // Hairs-Human:        PENDING
    public (string?, string, int, int) Key =>
        (Adornment.Code, DocumentPath, Adornment.Start, Adornment.Length);
}

/// <summary>
/// Merges live and build diagnostics for one document.
///
/// The rule that matters: an older build may not replace newer live results.
/// Builds are slow, so a build that started before the user's last three edits
/// finishes after them, and applying its diagnostics would resurrect errors the
/// user has already fixed. Live results win whenever they describe a snapshot
/// at least as new as the build's.
///
/// Build diagnostics are never rebased onto a newer snapshot — their spans
/// describe text that may have moved. They are kept only while their snapshot
/// is still current, and otherwise shown as belonging to an earlier build.
/// </summary>
// Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9D1A6B
// Hairs-Falsified-If: a build diagnostic is shown in place of the live diagnostic with the same code, document and span
// Hairs-Human:        PENDING
public sealed class DocumentDiagnosticMerge
{
    private readonly List<MergedDiagnostic> _live = [];
    private readonly List<MergedDiagnostic> _build = [];

    /// <summary>Snapshot version of the newest live result accepted.</summary>
    public int LiveSnapshotVersion { get; private set; } = -1;

    /// <summary>Snapshot version the newest accepted build compiled.</summary>
    public int BuildSnapshotVersion { get; private set; } = -1;

    public string? BuildId { get; private set; }

    /// <summary>
    /// True when the build's diagnostics describe an older snapshot than the
    /// live ones. The UI says so, because otherwise a user sees build errors
    /// for code they have changed and cannot tell why.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4CD7F2
    // Hairs-Falsified-If: a build whose snapshot version equals the live snapshot version is reported stale
    // Hairs-Human:        PENDING
    public bool BuildIsStale => BuildSnapshotVersion >= 0 && BuildSnapshotVersion < LiveSnapshotVersion;

    /// <summary>
    /// Accepts live results. Refused when they describe an older snapshot than
    /// what is already held — analysis runs concurrently and completes out of
    /// order.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=388580
    // Hairs-Falsified-If: live results for an older snapshot version replace the results already accepted for a newer one
    // Hairs-Human:        PENDING
    public bool TryApplyLive(int snapshotVersion, IEnumerable<MergedDiagnostic> diagnostics)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);
        if (snapshotVersion < LiveSnapshotVersion)
            return false;

        _live.Clear();
        _live.AddRange(diagnostics);
        LiveSnapshotVersion = snapshotVersion;
        return true;
    }

    /// <summary>
    /// Accepts build results. Refused when a newer build has already been
    /// applied — two builds can be in flight, and the older one finishing last
    /// must not win.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4BBAA1
    // Hairs-Falsified-If: a build for an older snapshot applied after a newer build replaces the newer build's diagnostics
    // Hairs-Human:        PENDING
    public bool TryApplyBuild(
        string buildId, int snapshotVersion, IEnumerable<MergedDiagnostic> diagnostics)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(buildId);
        ArgumentNullException.ThrowIfNull(diagnostics);

        if (snapshotVersion < BuildSnapshotVersion)
            return false;

        _build.Clear();
        _build.AddRange(diagnostics);
        BuildSnapshotVersion = snapshotVersion;
        BuildId = buildId;
        return true;
    }

    /// <summary>
    /// The diagnostics to show. Live wins on identity; a build diagnostic
    /// survives only when live has not covered it and the build is not stale.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=12D26E
    // Hairs-Falsified-If: a zero-width build diagnostic at a non-zero offset is still returned after live results have moved past the build's snapshot
    // Hairs-Human:        PENDING
    public IReadOnlyList<MergedDiagnostic> GetMerged()
    {
        var merged = new List<MergedDiagnostic>(_live);
        if (_build.Count == 0)
            return merged;

        // A stale build contributes nothing with a span. Its spans point into a
        // snapshot that has moved, and rebasing them is deferred until a tested
        // change-tracking algorithm exists.
        if (BuildIsStale)
        {
            merged.AddRange(_build.Where(diagnostic => diagnostic.Adornment.Length == 0));
            return merged;
        }

        var seen = new HashSet<(string?, string, int, int)>(_live.Select(d => d.Key));
        foreach (MergedDiagnostic diagnostic in _build)
        {
            if (seen.Add(diagnostic.Key))
                merged.Add(diagnostic);
        }

        return merged;
    }

    // Hairs-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=11AF64
    // Hairs-Falsified-If: after ClearBuild, GetMerged still returns a diagnostic from the cleared build or BuildIsStale is true
    // Hairs-Human:        PENDING
    public void ClearBuild()
    {
        _build.Clear();
        BuildSnapshotVersion = -1;
        BuildId = null;
    }
}
