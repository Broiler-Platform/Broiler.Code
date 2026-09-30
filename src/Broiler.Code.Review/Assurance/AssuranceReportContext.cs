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
    public const string DefaultGenerateCommand = "broiler-review assurance generate";

    /// <summary>The check command the tool names when it cannot derive one.</summary>
    public const string DefaultCheckCommand = "broiler-review assurance check";

    /// <summary>
    /// The commands for <paramref name="config"/>: its <c>regenerateCommand</c>,
    /// and a check command made from it by replacing the word <c>generate</c>
    /// when it occurs exactly once, so that a component invoking the tool
    /// through <c>dotnet run</c> is told to check the same way.
    /// </summary>
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
internal static class AssuranceFormat
{
    /// <summary>
    /// <c>part of whole (pct%)</c>, or the bare part when the whole is zero.
    /// The percentage is rounded the way the owning component rounds it, to even.
    /// </summary>
    public static string Portion(int part, int whole) => whole == 0
        ? Count(part)
        : string.Create(CultureInfo.InvariantCulture, $"{part} of {whole} ({(int)Math.Round(100.0 * part / whole)}%)");

    public static string Count(int value) => value.ToString(CultureInfo.InvariantCulture);

    /// <summary>One decimal, rounded half away from zero, as .NET's custom format does.</summary>
    public static string Mean(double value) => value.ToString("0.0", CultureInfo.InvariantCulture);

    /// <summary><c>`a`</c>, <c>`a` and `b`</c>, <c>`a`, `b` and `c`</c>.</summary>
    public static string CodeList(IReadOnlyList<string> items)
    {
        if (items.Count == 0)
            return string.Empty;

        if (items.Count == 1)
            return $"`{items[0]}`";

        return "`" + string.Join("`, `", items.Take(items.Count - 1)) + "` and `" + items[^1] + "`";
    }
}
