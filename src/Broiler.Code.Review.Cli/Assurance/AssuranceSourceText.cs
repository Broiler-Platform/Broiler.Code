// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           4
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         8/7
// Resource impact:  4/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.IO;
using System.Text;

namespace Broiler.Code.Review.Cli.Assurance;

/// <summary>
/// A source file's text together with what it takes to write it back unchanged.
///
/// Components differ: Broiler.JS has hundreds of files that open with a UTF-8
/// byte-order mark and most components have none. The owning component's
/// generator writes every changed file without one, which would put a one-byte
/// change on the first line of each of those files. So the mark is recorded
/// here and written back as it was, and only UTF-8 is accepted at all: a file
/// in any other encoding, or with bytes that are not valid UTF-8, is read for
/// listing but never rewritten, because decoding it and encoding it again would
/// not give back the same bytes.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=CA4609
// Broiler-Falsified-If: a file whose bytes are not valid UTF-8 is accepted, so a later write replaces those bytes with U+FFFD
// Broiler-Human:        PENDING
internal sealed class AssuranceSourceText
{
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=FFCB5B
    // Broiler-Falsified-If: a file that opens with EF BB BF is written back without those three bytes
    // Broiler-Human:        PENDING
    private static readonly byte[] Utf8Bom = [0xEF, 0xBB, 0xBF];

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AD46DD
    // Broiler-Falsified-If: a file that opens with FF FE is not refused as UTF-16 or UTF-32 before decoding
    // Broiler-Human:        PENDING
    private static readonly byte[] Utf16LittleEndianBom = [0xFF, 0xFE];

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=844152
    // Broiler-Falsified-If: a file that opens with FE FF is not refused as UTF-16 before decoding
    // Broiler-Human:        PENDING
    private static readonly byte[] Utf16BigEndianBom = [0xFE, 0xFF];

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=3E00C8
    // Broiler-Falsified-If: decoding a byte sequence that is not valid UTF-8, such as a lone 0x80, yields U+FFFD instead of throwing
    // Broiler-Human:        PENDING
    private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private AssuranceSourceText(string text, bool hasByteOrderMark, byte[] original)
    {
        Text = text;
        HasByteOrderMark = hasByteOrderMark;
        Original = original;
    }

    /// <summary>The decoded text, without the byte-order mark.</summary>
    public string Text { get; }

    /// <summary>Whether the file opened with a UTF-8 byte-order mark.</summary>
    public bool HasByteOrderMark { get; }

    /// <summary>The bytes as read, to detect a change on disk before writing.</summary>
    public byte[] Original { get; }

    /// <summary>
    /// Reads <paramref name="path"/> as UTF-8, or explains why it cannot be
    /// rewritten safely.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=BEA0D1
    // Broiler-Falsified-If: a file containing a byte such as 0xFF in its body is returned as source text instead of with a problem
    // Broiler-Human:        PENDING
    public static bool TryRead(string path, out AssuranceSourceText? source, out string? problem)
    {
        source = null;
        problem = null;

        byte[] bytes;
        try
        {
            bytes = File.ReadAllBytes(path);
        }
        catch (IOException exception)
        {
            problem = $"could not be read: {exception.Message}";
            return false;
        }
        catch (UnauthorizedAccessException exception)
        {
            problem = $"could not be read: {exception.Message}";
            return false;
        }

        bool bom = bytes.AsSpan().StartsWith(Utf8Bom);
        if (!bom && (bytes.AsSpan().StartsWith(Utf16LittleEndianBom) || bytes.AsSpan().StartsWith(Utf16BigEndianBom)))
        {
            problem = "is UTF-16 or UTF-32; only UTF-8 files are handled";
            return false;
        }

        string text;
        try
        {
            text = StrictUtf8.GetString(bytes, bom ? Utf8Bom.Length : 0, bytes.Length - (bom ? Utf8Bom.Length : 0));
        }
        catch (DecoderFallbackException)
        {
            problem = "is not valid UTF-8";
            return false;
        }

        // Valid UTF-8 always round-trips, so this cannot fail today. It is
        // checked rather than assumed because every untouched byte coming back
        // as it was is the promise a write makes.
        if (!Encode(text, bom).AsSpan().SequenceEqual(bytes))
        {
            problem = "does not round-trip through UTF-8";
            return false;
        }

        source = new AssuranceSourceText(text, bom, bytes);
        return true;
    }

    /// <summary>The bytes for <paramref name="text"/>, with the mark if the file had one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4C6711
    // Broiler-Falsified-If: Encode with the mark requested returns bytes that do not open with EF BB BF, or without it returns bytes that do
    // Broiler-Human:        PENDING
    public static byte[] Encode(string text, bool hasByteOrderMark)
    {
        ArgumentNullException.ThrowIfNull(text);

        byte[] body = StrictUtf8.GetBytes(text);
        if (!hasByteOrderMark)
            return body;

        byte[] withMark = new byte[Utf8Bom.Length + body.Length];
        Utf8Bom.CopyTo(withMark, 0);
        body.CopyTo(withMark, Utf8Bom.Length);
        return withMark;
    }

    /// <summary>
    /// Writes <paramref name="text"/> in place of this file, keeping its
    /// byte-order mark. Refuses when the file changed on disk since it was read,
    /// so an edit made meanwhile is not overwritten.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=5A02F9
    // Broiler-Falsified-If: a file whose bytes changed on disk after TryRead is overwritten instead of refused
    // Broiler-Human:        PENDING
    public bool TryWrite(string path, string text, out string? problem)
    {
        problem = null;

        try
        {
            if (!File.ReadAllBytes(path).AsSpan().SequenceEqual(Original))
            {
                problem = "changed on disk since it was read; nothing was written to it";
                return false;
            }

            File.WriteAllBytes(path, Encode(text, HasByteOrderMark));
            return true;
        }
        catch (IOException exception)
        {
            problem = $"could not be written: {exception.Message}";
            return false;
        }
        catch (UnauthorizedAccessException exception)
        {
            problem = $"could not be written: {exception.Message}";
            return false;
        }
    }
}
