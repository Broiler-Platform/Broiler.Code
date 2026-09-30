// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           5
// Human-reviewed:   0/3
// IP risk:          None
// Security risk:    Medium
// Criteria:         2/0
// Resource impact:  0/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.Code.Language.CSharp.Syntax;

/// <summary>
/// The lexer state at the start of a line.
///
/// Two lines with equal start state classify identically for identical text,
/// which is what lets incremental classification stop as soon as the state
/// reconverges after an edit. Without it, opening a block comment near the top
/// of a file would have no way to know where its effect ends.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=866494
// Broiler-Falsified-If: two raw-string states that differ only in QuoteCount compare equal, so incremental classification stops early and keeps stale colours
// Broiler-Human:        PENDING
internal readonly record struct LineState(LineStateKind Kind, byte QuoteCount, byte DollarCount)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=E8329F
    // Broiler-Falsified-If: Default has a kind other than LineStateKind.Default or a nonzero count, so a document's first line starts inside a comment or string
    // Broiler-Human:        PENDING
    public static readonly LineState Default = new(LineStateKind.Default, 0, 0);
}

// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=EDA14E
// Broiler-Human:        PENDING
internal enum LineStateKind : byte
{
    Default = 0,
    BlockComment,
    DocumentationBlockComment,
    VerbatimString,

    /// <summary>
    /// A raw string, which closes only on a run of at least
    /// <see cref="LineState.QuoteCount"/> quotes — so the count is part of the
    /// state, not a fixed delimiter.
    /// </summary>
    RawString,
}
