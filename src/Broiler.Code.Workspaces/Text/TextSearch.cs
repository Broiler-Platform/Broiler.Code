// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           0
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Medium
// Criteria:         4/0
// Resource impact:  5/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.Code.Workspaces.Text;

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D429FF
// Broiler-Human:        PENDING
public readonly record struct TextMatch(int Start, int Length)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=8476FD
    // Broiler-Human:        PENDING
    public int End => Start + Length;
}

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A1CD18
// Broiler-Human:        PENDING
public readonly record struct SearchOptions(
    bool MatchCase = false,
    bool WholeWord = false)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DB8E27
    // Broiler-Human:        PENDING
    public static SearchOptions Default => new();
}

/// <summary>
/// Find over a snapshot, scanning in bounded windows.
///
/// The naive implementation calls <c>ToString()</c> and uses
/// <c>string.IndexOf</c>. On the 10 MiB fixture that allocates the document per
/// keystroke in an incremental-search box, which is the cost this whole layer
/// exists to avoid.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=12F3C0
// Broiler-Falsified-If: a search pattern longer than the 8192-character scan window makes FindNext or FindPrevious run without returning or throw instead of returning a match or null
// Broiler-Human:        PENDING
public static class TextSearch
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=866461
    // Broiler-Human:        PENDING
    private const int WindowLength = 8192;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B57EE5
    // Broiler-Falsified-If: a pattern of 8193 characters searched in a longer document never advances the window start and the call does not return
    // Broiler-Human:        PENDING
    public static TextMatch? FindNext(
        TextSnapshot snapshot, string pattern, int from, SearchOptions options = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(pattern);
        if (pattern.Length == 0 || pattern.Length > snapshot.Length)
            return null;

        from = Math.Clamp(from, 0, snapshot.Length);
        StringComparison comparison = options.MatchCase
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        // Windows overlap by pattern.Length - 1 so a match straddling a window
        // boundary is still found.
        int overlap = pattern.Length - 1;
        int start = from;
        while (start < snapshot.Length)
        {
            int length = Math.Min(WindowLength, snapshot.Length - start);
            string window = snapshot.GetText(start, length);
            int index = 0;
            while (index <= window.Length - pattern.Length)
            {
                int found = window.IndexOf(pattern, index, comparison);
                if (found < 0)
                    break;

                int absolute = start + found;
                if (!options.WholeWord || IsWholeWord(snapshot, absolute, pattern.Length))
                    return new TextMatch(absolute, pattern.Length);
                index = found + 1;
            }

            if (start + length >= snapshot.Length)
                break;
            start += length - overlap;
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=E6ED6A
    // Broiler-Falsified-If: a pattern of 8193 characters searched before a position past the first window never moves the window end and the call does not return
    // Broiler-Human:        PENDING
    public static TextMatch? FindPrevious(
        TextSnapshot snapshot, string pattern, int before, SearchOptions options = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(pattern);
        if (pattern.Length == 0 || pattern.Length > snapshot.Length)
            return null;

        before = Math.Clamp(before, 0, snapshot.Length);
        StringComparison comparison = options.MatchCase
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        int overlap = pattern.Length - 1;
        int end = before;
        while (end > 0)
        {
            int start = Math.Max(0, end - WindowLength);
            string window = snapshot.GetText(start, end - start);
            int limit = window.Length - pattern.Length;
            for (int i = limit; i >= 0; i--)
            {
                if (!window.AsSpan(i, pattern.Length).Equals(pattern, comparison))
                    continue;
                int absolute = start + i;
                if (absolute + pattern.Length > before)
                    continue;
                if (!options.WholeWord || IsWholeWord(snapshot, absolute, pattern.Length))
                    return new TextMatch(absolute, pattern.Length);
            }

            if (start == 0)
                break;
            end = start + overlap;
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DEF042
    // Broiler-Falsified-If: a match preceded by an underscore or followed by a digit is accepted as a whole word
    // Broiler-Human:        PENDING
    private static bool IsWholeWord(TextSnapshot snapshot, int start, int length)
    {
        if (start > 0 && IsWordCharacter(snapshot[start - 1]))
            return false;
        int end = start + length;
        return end >= snapshot.Length || !IsWordCharacter(snapshot[end]);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=FD5376
    // Broiler-Human:        PENDING
    private static bool IsWordCharacter(char c) => c == '_' || char.IsLetterOrDigit(c);
}
