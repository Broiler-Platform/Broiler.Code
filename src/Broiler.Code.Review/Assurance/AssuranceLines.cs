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
public sealed class AssuranceLines
{
    private readonly List<string> _lines = [];
    private readonly List<string> _separators = [];

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

    public string this[int index] => _lines[index];

    /// <summary>
    /// The ending an inserted line takes: the file's first real one, and LF for
    /// a file that has none because it holds a single unterminated line.
    /// </summary>
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
    public void Replace(int index, string line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines[index] = line;
    }

    /// <summary>Inserts lines, each terminated with <see cref="NewLine"/>.</summary>
    public void Insert(int index, IReadOnlyList<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        string newLine = NewLine;
        for (int offset = 0; offset < lines.Count; offset++)
        {
            _lines.Insert(index + offset, lines[offset]);
            _separators.Insert(index + offset, newLine);
        }
    }

    /// <summary>Removes lines together with their terminators.</summary>
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
    public static string IndentOf(string line)
    {
        ArgumentNullException.ThrowIfNull(line);

        int index = 0;
        while (index < line.Length && char.IsWhiteSpace(line[index]))
            index++;

        return line[..index];
    }

    /// <summary>Reassembles the file. Untouched lines come back byte-identical.</summary>
    public string Render()
    {
        var builder = new System.Text.StringBuilder();
        for (int index = 0; index < _lines.Count; index++)
            builder.Append(_lines[index]).Append(_separators[index]);

        return builder.ToString();
    }
}
