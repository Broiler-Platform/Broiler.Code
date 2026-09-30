// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           1
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Low
// Criteria:         7/0
// Resource impact:  3/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// What the component-level artefacts say about the component that is not in
/// its units: its name, its commands, where the other artefacts are, and which
/// files and assemblies the record covers.
/// </summary>
/// <param name="Component">The component's name, for titles and prose.</param>
/// <param name="Paths">Where the three artefacts are, relative to the root.</param>
/// <param name="GenerateCommand">The command that rewrites the artefacts.</param>
/// <param name="CheckCommand">The command that compares them with the tree.</param>
/// <param name="CoveredFiles">Every covered file, ordered by path (ordinal).</param>
/// <param name="Excluded">Files the configuration leaves out, with the reason.</param>
/// <param name="Assemblies">The covered assemblies.</param>
/// <param name="ClosedToEscapeHatch">Assemblies in which <c>EXEMPT=</c> may not be written.</param>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=1968B8
// Broiler-Falsified-If: a regenerate command naming generate twice is given a check command with one of them replaced instead of the default
// Broiler-Human:        PENDING
public sealed record AssuranceReportContext(
    string Component,
    AssuranceArtefactPaths Paths,
    string GenerateCommand,
    string CheckCommand,
    IReadOnlyList<string> CoveredFiles,
    IReadOnlyList<AssuranceExcludedSource> Excluded,
    IReadOnlyList<string> Assemblies,
    IReadOnlyList<string> ClosedToEscapeHatch)
{
    /// <summary>Review records a person keeps beside the generated ones. See <see cref="AssuranceCorpus.SeparateRecords"/>.</summary>
    public IReadOnlyList<string> SeparateRecords { get; init; } = [];

    /// <summary>The generate command the tool names when the configuration names none.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=6951B9
    // Broiler-Human:        PENDING
    public const string DefaultGenerateCommand = "broiler-review assurance generate";

    /// <summary>The check command the tool names when it cannot derive one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=0FBCE6
    // Broiler-Human:        PENDING
    public const string DefaultCheckCommand = "broiler-review assurance check";

    /// <summary>
    /// The commands for <paramref name="config"/>: its <c>regenerateCommand</c>,
    /// and a check command made from it by replacing the word <c>generate</c>
    /// when it occurs exactly once, so that a component invoking the tool
    /// through <c>dotnet run</c> is told to check the same way.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=A61C64
    // Broiler-Falsified-If: a configuration whose regenerate command names generate exactly once is given a check command that still says generate
    // Broiler-Human:        PENDING
    public static (string Generate, string Check) CommandsFor(AssuranceComponentConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (config.RegenerateCommand is not { Length: > 0 } generate)
            return (DefaultGenerateCommand, DefaultCheckCommand);

        return (generate, Sibling(generate, "check"));
    }

    /// <summary>
    /// Another assurance command, written the way <paramref name="generateCommand"/>
    /// is: the same text with the word <c>generate</c> replaced by
    /// <paramref name="verb"/> when it occurs exactly once, and
    /// <c>broiler-review assurance &lt;verb&gt;</c> otherwise.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=9240D8
    // Broiler-Falsified-If: a command naming generate twice has one of them replaced instead of falling back to the default command
    // Broiler-Human:        PENDING
    public static string Sibling(string generateCommand, string verb)
    {
        ArgumentNullException.ThrowIfNull(generateCommand);
        ArgumentNullException.ThrowIfNull(verb);

        string fallback = "broiler-review assurance " + verb;
        string[] words = generateCommand.Split(' ');
        int found = -1;
        for (int index = 0; index < words.Length; index++)
        {
            if (!string.Equals(words[index], "generate", StringComparison.Ordinal))
                continue;

            if (found >= 0)
                return fallback;

            found = index;
        }

        if (found < 0)
            return fallback;

        words[found] = verb;
        return string.Join(' ', words);
    }
}

/// <summary>Number formatting shared by the component-level artefacts.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=B050D9
// Broiler-Human:        PENDING
internal static class AssuranceFormat
{
    /// <summary>
    /// <c>part of whole (pct%)</c>, or the bare part when the whole is zero.
    /// The percentage is rounded the way the owning component rounds it, to even.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7C5D25
    // Broiler-Falsified-If: a portion of 1 of 8 is written with 13% rather than the 12% that rounding half to even gives
    // Broiler-Human:        PENDING
    public static string Portion(int part, int whole) => whole == 0
        ? Count(part)
        : string.Create(CultureInfo.InvariantCulture, $"{part} of {whole} ({(int)Math.Round(100.0 * part / whole)}%)");

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=1EE28B
    // Broiler-Falsified-If: a count of 1234 is written with a group separator or with digits other than ASCII
    // Broiler-Human:        PENDING
    public static string Count(int value) => value.ToString(CultureInfo.InvariantCulture);

    /// <summary>One decimal, rounded half away from zero, as .NET's custom format does.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=55FBEE
    // Broiler-Falsified-If: on a machine whose current culture uses a decimal comma, a mean is written with a comma
    // Broiler-Human:        PENDING
    public static string Mean(double value) => value.ToString("0.0", CultureInfo.InvariantCulture);

    /// <summary><c>`a`</c>, <c>`a` and `b`</c>, <c>`a`, `b` and `c`</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=E9D9DC
    // Broiler-Falsified-If: three items are joined without the word and before the last one
    // Broiler-Human:        PENDING
    public static string CodeList(IReadOnlyList<string> items)
    {
        if (items.Count == 0)
            return string.Empty;

        if (items.Count == 1)
            return $"`{items[0]}`";

        return "`" + string.Join("`, `", items.Take(items.Count - 1)) + "` and `" + items[^1] + "`";
    }
}
