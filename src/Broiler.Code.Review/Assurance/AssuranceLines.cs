// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           3
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    High
// Criteria:         10/3
// Resource impact:  4/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// A source file split into lines that remember how each of them ended.
///
/// Rewriting one line of a source file must not rewrite the rest of it. This
/// repository holds CRLF files, LF files and files mixed within themselves, and
/// a splitter that normalizes as it reads gives back a file whose every line has
/// changed — which turns "a reviewer named themselves on one declaration" into a
/// whole-file diff, and invalidates every other reviewer's content hash on the
/// way past.
///
/// So each line keeps its own terminator and <see cref="Render"/> puts it back.
/// Lines this class inserts take <see cref="NewLine"/>, the file's own first
/// ending, so an inserted line matches the file it lands in rather than the
/// platform the editor happens to be running on.
///
/// The split is the one the owning component uses, including its last rule: the
/// text after the final terminator is always a line, so a file ending in a
/// newline has an empty final line and round-trips unchanged.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=995134
// Broiler-Falsified-If: a text mixing CRLF, LF and lone CR endings does not render back byte for byte after being split and left unedited
// Broiler-Human:        PENDING
public sealed class AssuranceLines
{
    private readonly List<string> _lines = [];
    private readonly List<string> _separators = [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=8C9593
    // Broiler-Falsified-If: a lone CR, an LF or a CRLF pair ends a line at a position other than where the C# parser starts its next line, so line N here is not the parser's line N
    // Broiler-Human:        PENDING
    public AssuranceLines(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        int start = 0;
        for (int index = 0; index < text.Length; index++)
        {
            string? separator = text[index] switch
            {
                '\r' when index + 1 < text.Length && text[index + 1] == '\n' => "\r\n",
                '\r' => "\r",
                '\n' => "\n",
                _ => null,
            };

            if (separator is null)
                continue;

            _lines.Add(text[start..index]);
            _separators.Add(separator);
            index += separator.Length - 1;
            start = index + 1;
        }

        _lines.Add(text[start..]);
        _separators.Add(string.Empty);
    }

    public int Count => _lines.Count;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4CAD47
    // Broiler-Human:        PENDING
    public string this[int index] => _lines[index];

    /// <summary>
    /// The ending an inserted line takes: the file's first real one, and LF for
    /// a file that has none because it holds a single unterminated line.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=ACC04D
    // Broiler-Falsified-If: a file whose first terminator is CRLF reports LF, or a file with no terminator reports anything but LF
    // Broiler-Human:        PENDING
    public string NewLine
    {
        get
        {
            foreach (string separator in _separators)
            {
                if (separator.Length > 0)
                    return separator;
            }

            return "\n";
        }
    }

    /// <summary>Replaces one line's text, leaving its terminator alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=804D55
    // Broiler-Falsified-If: after Replace, the replaced line's terminator or any other line's text differs from before
    // Broiler-Human:        PENDING
    public void Replace(int index, string line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines[index] = line;
    }

    /// <summary>
    /// The terminator line <paramref name="index"/> ends with: CRLF, LF, a lone
    /// CR, or empty for the last line.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=22A6BE
    // Broiler-Falsified-If: the last line reports a non-empty terminator, or a line ending in CRLF reports LF
    // Broiler-Human:        PENDING
    public string SeparatorOf(int index) => _separators[index];

    /// <summary>Inserts lines, each terminated with <see cref="NewLine"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A1E29C
    // Broiler-Falsified-If: lines inserted into a CRLF file through the two-argument overload are terminated with LF
    // Broiler-Human:        PENDING
    public void Insert(int index, IReadOnlyList<string> lines) => Insert(index, lines, NewLine);

    /// <summary>
    /// Inserts lines, each terminated with <paramref name="separator"/>. For a
    /// caller that has chosen the ending itself, such as the ending of the
    /// neighbouring lines in a file whose endings are mixed.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=97C492
    // Broiler-Falsified-If: a separator other than CRLF, LF or CR, such as the empty string, is accepted and joins an inserted line to the line below it
    // Broiler-Human:        PENDING
    public void Insert(int index, IReadOnlyList<string> lines, string separator)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(separator);

        if (separator is not ("\r\n" or "\n" or "\r"))
            throw new ArgumentException("A separator is CRLF, LF or CR.", nameof(separator));

        for (int offset = 0; offset < lines.Count; offset++)
        {
            _lines.Insert(index + offset, lines[offset]);
            _separators.Insert(index + offset, separator);
        }
    }

    /// <summary>Removes lines together with their terminators.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4966FE
    // Broiler-Falsified-If: removing a range leaves a removed line's terminator on a surviving line, so Render loses or repeats a line ending
    // Broiler-Human:        PENDING
    public void RemoveRange(int index, int count)
    {
        _lines.RemoveRange(index, count);
        _separators.RemoveRange(index, count);
    }

    /// <summary>
    /// The leading whitespace of a line, which is the indent a rewritten
    /// annotation is re-emitted at. Taken from the line rather than guessed from
    /// the declaration, so a block indented by hand stays where it was put.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=DA4DF1
    // Broiler-Falsified-If: a line indented with a tab followed by spaces returns an indent without the tab
    // Broiler-Human:        PENDING
    public static string IndentOf(string line)
    {
        ArgumentNullException.ThrowIfNull(line);

        int index = 0;
        while (index < line.Length && char.IsWhiteSpace(line[index]))
            index++;

        return line[..index];
    }

    /// <summary>Reassembles the file. Untouched lines come back byte-identical.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=96E435
    // Broiler-Falsified-If: a text split and left unedited renders to a string that differs from the original in any character
    // Broiler-Human:        PENDING
    public string Render()
    {
        var builder = new System.Text.StringBuilder();
        for (int index = 0; index < _lines.Count; index++)
            builder.Append(_lines[index]).Append(_separators[index]);

        return builder.ToString();
    }
}
