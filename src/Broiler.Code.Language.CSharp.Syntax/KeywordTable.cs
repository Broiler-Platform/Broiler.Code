// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           0
// Human-reviewed:   0/7
// IP risk:          None
// Security risk:    Medium
// Criteria:         7/0
// Resource impact:  1/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.UI.CodeEditor;
namespace Broiler.Code.Language.CSharp.Syntax;

/// <summary>
/// The keyword set the portable classifier recognizes. Reserved words are
/// listed in full; contextual words are limited to the ones that are almost
/// never used as ordinary identifiers, so the fallback classifier does not
/// miscolor a local named <c>value</c>, <c>from</c>, or <c>where</c>.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E21412
// Broiler-Falsified-If: a reserved keyword such as stackalloc or as is classified as None
// Broiler-Human:        PENDING
internal static class KeywordTable
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=2D59FB
    // Broiler-Falsified-If: a control-flow keyword such as goto or yield is missing, so it is coloured as a plain keyword
    // Broiler-Human:        PENDING
    private static readonly string[] ControlKeywords =
    [
        "await", "break", "case", "catch", "continue", "default", "do", "else",
        "finally", "for", "foreach", "goto", "if", "return", "switch", "throw",
        "try", "while", "yield",
    ];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=D549FF
    // Broiler-Falsified-If: a reserved C# keyword such as volatile or sizeof is missing from the list
    // Broiler-Human:        PENDING
    private static readonly string[] Keywords =
    [
        "abstract", "as", "base", "bool", "byte", "char", "checked", "class",
        "const", "decimal", "delegate", "double", "dynamic", "enum", "event",
        "explicit", "extern", "false", "fixed", "float", "implicit", "in",
        "int", "interface", "internal", "is", "lock", "long", "namespace",
        "new", "nint", "nuint", "null", "object", "operator", "out", "override",
        "params", "partial", "private", "protected", "public", "readonly",
        "record", "ref", "sbyte", "sealed", "short", "sizeof", "stackalloc",
        "static", "string", "struct", "this", "true", "typeof", "uint", "ulong",
        "unchecked", "unsafe", "ushort", "using", "var", "virtual", "void",
        "volatile",
    ];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=751E50
    // Broiler-Falsified-If: the set compares case-insensitively, so an identifier spelled If is coloured as a keyword
    // Broiler-Human:        PENDING
    private static readonly HashSet<string> ControlSet =
        new(ControlKeywords, StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=7F7557
    // Broiler-Falsified-If: the set compares case-insensitively, so an identifier spelled Class is coloured as a keyword
    // Broiler-Human:        PENDING
    private static readonly HashSet<string> KeywordSet =
        new(Keywords, StringComparer.Ordinal);

    /// <summary>
    /// <c>async</c> is contextual but is a keyword wherever it can legally
    /// appear, so it is treated as one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=537ECC
    // Broiler-Falsified-If: the word async is classified as None
    // Broiler-Human:        PENDING
    static KeywordTable() => KeywordSet.Add("async");

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B1A3BA
    // Broiler-Falsified-If: a ten-character reserved keyword such as stackalloc is returned as None because the length guard rejects it
    // Broiler-Human:        PENDING
    public static CodeClassificationKind Lookup(ReadOnlySpan<char> word)
    {
        // Every keyword is ASCII lower-case and at most 10 characters, so a
        // length and first-character check rejects the overwhelming majority of
        // identifiers before any hashing happens.
        if (word.Length is < 2 or > 10 || !char.IsAsciiLetterLower(word[0]))
            return CodeClassificationKind.None;

        var lookup = ControlSet.GetAlternateLookup<ReadOnlySpan<char>>();
        if (lookup.Contains(word))
            return CodeClassificationKind.ControlKeyword;

        return KeywordSet.GetAlternateLookup<ReadOnlySpan<char>>().Contains(word)
            ? CodeClassificationKind.Keyword
            : CodeClassificationKind.None;
    }
}
