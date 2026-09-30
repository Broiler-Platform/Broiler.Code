// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    High
// Criteria:         5/4
// Resource impact:  3/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;

namespace Broiler.Code.Workspaces.Storage;

/// <summary>
/// Normalization and containment for workspace-relative paths.
///
/// Every path that crosses into storage goes through here. A workspace opens
/// files a user did not write — a project can name any path it likes — so
/// "../../../../etc/passwd" and "src/../../outside" are inputs to handle, not
/// hypotheticals.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=813AEB
// Broiler-Falsified-If: a relative path whose .. segments climb above its start, such as src/../../outside, is returned as a usable path instead of null
// Broiler-Human:        PENDING
public static class WorkspacePath
{
    /// <summary>
    /// Normalizes to forward slashes and resolves <c>.</c> and <c>..</c>
    /// segments. Returns null when the path escapes the root, rather than
    /// returning something that looks usable.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A4BCB6
    // Broiler-Falsified-If: a relative path whose .. segments climb above its start, such as src/../../outside, is returned as a usable path instead of null
    // Broiler-Human:        PENDING
    public static string? Normalize(string relativePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);

        // A rooted or drive-qualified path is not relative, whatever it claims.
        if (relativePath.Length >= 2 && relativePath[1] == ':')
            return null;
        if (relativePath.StartsWith('/') || relativePath.StartsWith('\\'))
            return null;

        var segments = new List<string>();
        foreach (string raw in relativePath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries))
        {
            if (raw == ".")
                continue;
            if (raw == "..")
            {
                if (segments.Count == 0)
                    return null;
                segments.RemoveAt(segments.Count - 1);
                continue;
            }

            // A trailing dot or space is stripped by Windows when the path is
            // opened, so "name." and "name" reach the same file. Treating them
            // as distinct lets one bypass a check made against the other.
            if (raw.TrimEnd('.', ' ').Length != raw.Length)
                return null;

            // Reserved device names resolve to a device rather than a file.
            if (IsReservedDeviceName(raw))
                return null;

            segments.Add(raw);
        }

        return segments.Count == 0 ? string.Empty : string.Join('/', segments);
    }

    /// <summary>
    /// True when <paramref name="candidate"/> is inside
    /// <paramref name="root"/>. The separator matters: without it, "src2"
    /// passes a prefix test against "src".
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6C5A15
    // Broiler-Falsified-If: a candidate that only shares a name prefix with the root, such as src2/a against root src, is reported as contained
    // Broiler-Human:        PENDING
    public static bool IsContained(string candidate, string root)
    {
        if (root.Length == 0)
            return true;
        if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            return false;
        return candidate.Length == root.Length || candidate[root.Length] == '/';
    }

    /// <summary>
    /// A comparer for detecting collisions on a case-insensitive provider. Two
    /// paths differing only in case are the same file there and different files
    /// elsewhere, so the workspace has to know which rule its provider follows
    /// before it creates the second one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D6F413
    // Broiler-Falsified-If: for a case-insensitive provider the returned comparer treats a.cs and A.cs as different paths
    // Broiler-Human:        PENDING
    public static StringComparer GetComparer(bool caseInsensitive) =>
        caseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6786A8
    // Broiler-Falsified-If: a segment of COM or LPT followed by a superscript digit U+00B9, U+00B2 or U+00B3, which Windows also reserves as a device name, is not recognized as reserved
    // Broiler-Human:        PENDING
    private static bool IsReservedDeviceName(string segment)
    {
        int dot = segment.IndexOf('.');
        string stem = dot < 0 ? segment : segment[..dot];
        return stem.ToUpperInvariant() switch
        {
            "CON" or "PRN" or "AUX" or "NUL" => true,
            ['C', 'O', 'M', >= '1' and <= '9'] => true,
            ['L', 'P', 'T', >= '1' and <= '9'] => true,
            _ => false,
        };
    }
}
