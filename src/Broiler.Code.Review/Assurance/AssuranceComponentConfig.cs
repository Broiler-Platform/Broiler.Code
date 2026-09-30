using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Broiler.Code.Review.Assurance;

/// <summary>Who writes a component's assurance artefacts.</summary>
public enum AssuranceMode
{
    /// <summary>This tool writes them. The write commands are allowed.</summary>
    Owned = 0,

    /// <summary>
    /// Something else writes them (Broiler.VM's own architecture tests). This
    /// tool only reads and verifies; every write command refuses.
    /// </summary>
    External,
}

/// <summary>Which words mark a comment as a forged assurance summary.</summary>
public enum AssuranceForgeryVocabulary
{
    /// <summary>
    /// The banner, the GENERATED marker, and the header's row labels written as
    /// <c>// Label:</c> at the start of a comment. Few false positives in code
    /// that talks about reviews for a living.
    /// </summary>
    Narrow = 0,

    /// <summary>
    /// The owning component's full list, which also matches words such as
    /// <c>reviewer</c> and <c>approved</c> anywhere in a comment.
    /// </summary>
    Strict,
}

/// <summary>A file pattern the component leaves out of the covered set, and why.</summary>
/// <param name="Glob">The pattern.</param>
/// <param name="Reason">Why, in words a report can print beside each file it excludes.</param>
public sealed record AssuranceExclusion(AssuranceGlob Glob, string Reason);

/// <summary>
/// The SPDX lines a generated file header opens with.
/// </summary>
/// <param name="Copyright">
/// One or more <c>SPDX-FileCopyrightText</c> values, without the prefix:
/// <c>2026 Broiler Platform contributors</c>.
/// </param>
/// <param name="License">One SPDX licence expression: <c>Apache-2.0 AND BSD-3-Clause</c>.</param>
public sealed record AssuranceSpdx(IReadOnlyList<string> Copyright, string License);

/// <summary>SPDX lines for the files one pattern matches, in place of the component default.</summary>
public sealed record AssuranceSpdxOverride(AssuranceGlob Glob, AssuranceSpdx Spdx);

/// <summary>Where the component-level artefacts are written, relative to the component root.</summary>
public sealed record AssuranceArtefactPaths
{
    public string Report { get; init; } = "CODE-ASSURANCE.md";

    public string HumanReview { get; init; } = "HUMAN_REVIEW.md";

    public string Manifest { get; init; } = "assurance.manifest.json";
}

/// <summary>Why a configuration file was refused.</summary>
public sealed class AssuranceConfigException : Exception
{
    public AssuranceConfigException(string message)
        : base(message)
    {
    }

    public AssuranceConfigException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

/// <summary>
/// A component's <c>assurance.config.json</c>: what the tool covers there and
/// how it writes.
///
/// The file's presence is the opt-in. Without it the write commands refuse, so
/// that pointing the tool at a component cannot rewrite that component's
/// sources. Nothing in it is guessed. An unknown property is an error rather
/// than ignored, because a misspelled <c>exclude</c> would otherwise cover
/// exactly the files it was written to leave out.
/// </summary>
public sealed record AssuranceComponentConfig
{
    /// <summary>The configuration's file name, at the component root.</summary>
    public const string FileName = "assurance.config.json";

    /// <summary>The only schema version this build reads.</summary>
    public const int CurrentSchema = 1;

    /// <summary>The component's name, for generated prose.</summary>
    public string Component { get; init; } = string.Empty;

    /// <summary>Whether this tool writes the component's artefacts.</summary>
    public AssuranceMode Mode { get; init; } = AssuranceMode.Owned;

    /// <summary>
    /// The product projects, relative to the root with forward slashes. Every
    /// <c>*.cs</c> under each one's directory is covered, less build output,
    /// nested checkouts and <see cref="Exclude"/>. An explicit list, because
    /// components carry test projects, samples, tools and stale copies of other
    /// components that no rule could tell apart from a product.
    /// </summary>
    public IReadOnlyList<string> Projects { get; init; } = [];

    /// <summary>Files left out of the covered set, each with the reason a report prints.</summary>
    public IReadOnlyList<AssuranceExclusion> Exclude { get; init; } = [];

    /// <summary>The SPDX lines for every covered file no override matches. Null until configured.</summary>
    public AssuranceSpdx? Spdx { get; init; }

    /// <summary>SPDX lines for particular files. The first matching pattern wins.</summary>
    public IReadOnlyList<AssuranceSpdxOverride> SpdxOverrides { get; init; } = [];

    /// <summary>Where the component-level artefacts go.</summary>
    public AssuranceArtefactPaths Artefacts { get; init; } = new();

    /// <summary>
    /// The symbols the scanner parses under, or null for the scanner's default
    /// (the owning component's net10.0 set).
    /// </summary>
    public IReadOnlyList<string>? PreprocessorSymbols { get; init; }

    /// <summary>
    /// Whether a preprocessor directive in a covered file is a violation. Off by
    /// default: most components use <c>#nullable</c>, <c>#pragma</c> or
    /// <c>#if</c>, and the owning component's rule would fail them all.
    /// </summary>
    public bool ForbidDirectives { get; init; }

    /// <summary>Which vocabulary marks a comment below the header as a forged summary.</summary>
    public AssuranceForgeryVocabulary ForgeryVocabulary { get; init; } = AssuranceForgeryVocabulary.Narrow;

    /// <summary>Assemblies in which <c>EXEMPT=</c> may not be written: a unit there is assessed.</summary>
    public IReadOnlyList<string> ClosedToEscapeHatch { get; init; } = [];

    /// <summary>Where <c>Spec=ADR-nnnn</c> citations are resolved, relative to the root.</summary>
    public string AdrDirectory { get; init; } = "docs/adr";

    /// <summary>The command generated prose tells a reader to run. Null for the tool's own.</summary>
    public string? RegenerateCommand { get; init; }

    /// <summary>
    /// The manifest's <c>$comment</c> lines, verbatim, in place of the tool's
    /// own. Null for the tool's. For a component whose manifest prose is fixed
    /// by its own tests, so that the manifest this tool writes is the one that
    /// component's gate accepts byte for byte. The check still holds these lines
    /// to the review-claim rule.
    /// </summary>
    public IReadOnlyList<string>? ManifestComment { get; init; }

    /// <summary>
    /// The SPDX lines for <paramref name="relativePath"/>: the first override
    /// whose pattern matches, else the default. Null when neither is configured.
    /// </summary>
    public AssuranceSpdx? SpdxFor(string relativePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);

        foreach (AssuranceSpdxOverride entry in SpdxOverrides)
        {
            if (entry.Glob.IsMatch(relativePath))
                return entry.Spdx;
        }

        return Spdx;
    }

    /// <summary>
    /// The exclusion that leaves <paramref name="relativePath"/> out, or null.
    /// The first matching pattern gives the reason.
    /// </summary>
    public AssuranceExclusion? ExclusionFor(string relativePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);

        foreach (AssuranceExclusion exclusion in Exclude)
        {
            if (exclusion.Glob.IsMatch(relativePath))
                return exclusion;
        }

        return null;
    }

    /// <summary>
    /// Reads a configuration. Comments and trailing commas are allowed, because
    /// the file is written by hand. Throws <see cref="AssuranceConfigException"/>
    /// naming the property at fault.
    /// </summary>
    public static AssuranceComponentConfig Parse(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            });
        }
        catch (JsonException exception)
        {
            throw new AssuranceConfigException($"{FileName} is not valid JSON: {exception.Message}", exception);
        }

        using (document)
        {
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                throw Error("$", "must be an object");

            var config = new AssuranceComponentConfig();
            bool sawProjects = false;

            foreach (JsonProperty property in root.EnumerateObject())
            {
                string path = "$." + property.Name;
                JsonElement value = property.Value;

                switch (property.Name)
                {
                    case "$comment":
                        break;

                    case "schema":
                        if (value.ValueKind != JsonValueKind.Number ||
                            !value.TryGetInt32(out int schema) ||
                            schema != CurrentSchema)
                        {
                            throw Error(path, $"must be {CurrentSchema}; this build reads no other schema");
                        }

                        break;

                    case "component":
                        config = config with { Component = RequiredString(value, path) };
                        break;

                    case "mode":
                        config = config with
                        {
                            Mode = RequiredString(value, path) switch
                            {
                                "owned" => AssuranceMode.Owned,
                                "external" => AssuranceMode.External,
                                _ => throw Error(path, "must be \"owned\" or \"external\""),
                            },
                        };
                        break;

                    case "projects":
                        sawProjects = true;
                        config = config with { Projects = ProjectList(value, path) };
                        break;

                    case "exclude":
                        config = config with { Exclude = Exclusions(value, path) };
                        break;

                    case "spdx":
                        config = config with { Spdx = SpdxOf(value, path, allowGlob: false).Spdx };
                        break;

                    case "spdxOverrides":
                        config = config with { SpdxOverrides = Overrides(value, path) };
                        break;

                    case "artefacts":
                        config = config with { Artefacts = ArtefactPaths(value, path) };
                        break;

                    case "preprocessorSymbols":
                        config = config with { PreprocessorSymbols = Symbols(value, path) };
                        break;

                    case "forbidDirectives":
                        config = config with { ForbidDirectives = RequiredBool(value, path) };
                        break;

                    case "forgeryVocabulary":
                        config = config with
                        {
                            ForgeryVocabulary = RequiredString(value, path) switch
                            {
                                "narrow" => AssuranceForgeryVocabulary.Narrow,
                                "strict" => AssuranceForgeryVocabulary.Strict,
                                _ => throw Error(path, "must be \"narrow\" or \"strict\""),
                            },
                        };
                        break;

                    case "closedToEscapeHatch":
                        config = config with { ClosedToEscapeHatch = StringList(value, path) };
                        break;

                    case "adrDirectory":
                        config = config with { AdrDirectory = RelativePath(value, path) };
                        break;

                    case "regenerateCommand":
                        config = config with { RegenerateCommand = SingleLine(value, path) };
                        break;

                    case "manifestComment":
                        config = config with { ManifestComment = CommentLines(value, path) };
                        break;

                    default:
                        throw Error(path, "is not a property this schema defines");
                }
            }

            if (!sawProjects || config.Projects.Count == 0)
                throw Error("$.projects", "must list at least one product project");

            return config;
        }
    }

    private static IReadOnlyList<string> ProjectList(JsonElement value, string path)
    {
        IReadOnlyList<string> projects = StringList(value, path);
        for (int index = 0; index < projects.Count; index++)
        {
            string project = CheckRelative(projects[index], $"{path}[{index}]");
            if (!project.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                throw Error($"{path}[{index}]", $"'{project}' is not a .csproj file");
        }

        if (projects.Distinct(StringComparer.OrdinalIgnoreCase).Count() != projects.Count)
            throw Error(path, "names a project twice");

        return projects;
    }

    private static IReadOnlyList<AssuranceExclusion> Exclusions(JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.Array)
            throw Error(path, "must be an array");

        var exclusions = new List<AssuranceExclusion>();
        int index = 0;
        foreach (JsonElement entry in value.EnumerateArray())
        {
            string at = $"{path}[{index++}]";
            string pattern;
            string reason = "excluded by assurance.config.json";

            if (entry.ValueKind == JsonValueKind.String)
            {
                pattern = entry.GetString()!;
            }
            else if (entry.ValueKind == JsonValueKind.Object)
            {
                string? glob = null;
                foreach (JsonProperty property in entry.EnumerateObject())
                {
                    switch (property.Name)
                    {
                        case "glob":
                            glob = RequiredString(property.Value, $"{at}.glob");
                            break;
                        case "reason":
                            reason = SingleLine(property.Value, $"{at}.reason");
                            break;
                        default:
                            throw Error($"{at}.{property.Name}", "is not a property this schema defines");
                    }
                }

                pattern = glob ?? throw Error(at, "has no \"glob\"");
            }
            else
            {
                throw Error(at, "must be a glob string or {\"glob\", \"reason\"}");
            }

            exclusions.Add(new AssuranceExclusion(Glob(pattern, at), reason));
        }

        return exclusions;
    }

    private static IReadOnlyList<AssuranceSpdxOverride> Overrides(JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.Array)
            throw Error(path, "must be an array");

        var overrides = new List<AssuranceSpdxOverride>();
        int index = 0;
        foreach (JsonElement entry in value.EnumerateArray())
        {
            string at = $"{path}[{index++}]";
            (AssuranceSpdx spdx, AssuranceGlob? glob) = SpdxOf(entry, at, allowGlob: true);
            overrides.Add(new AssuranceSpdxOverride(glob ?? throw Error(at, "has no \"glob\""), spdx));
        }

        return overrides;
    }

    private static (AssuranceSpdx Spdx, AssuranceGlob? Glob) SpdxOf(JsonElement value, string path, bool allowGlob)
    {
        if (value.ValueKind != JsonValueKind.Object)
            throw Error(path, "must be an object");

        IReadOnlyList<string>? copyright = null;
        string? license = null;
        AssuranceGlob? glob = null;

        foreach (JsonProperty property in value.EnumerateObject())
        {
            string at = $"{path}.{property.Name}";
            switch (property.Name)
            {
                case "copyright":
                    copyright = StringList(property.Value, at);
                    for (int index = 0; index < copyright.Count; index++)
                    {
                        string line = CheckSingleLine(copyright[index], $"{at}[{index}]");
                        if (line.StartsWith("//", StringComparison.Ordinal) ||
                            line.StartsWith("SPDX-", StringComparison.OrdinalIgnoreCase))
                        {
                            throw Error($"{at}[{index}]", "is the value only, without '// SPDX-FileCopyrightText:'");
                        }
                    }

                    break;

                case "license":
                    license = SingleLine(property.Value, at);
                    break;

                case "glob" when allowGlob:
                    glob = Glob(RequiredString(property.Value, at), at);
                    break;

                default:
                    throw Error(at, "is not a property this schema defines");
            }
        }

        if (copyright is null || copyright.Count == 0)
            throw Error($"{path}.copyright", "must list at least one copyright line");

        if (license is null)
            throw Error($"{path}.license", "is required");

        return (new AssuranceSpdx(copyright, license), glob);
    }

    private static AssuranceArtefactPaths ArtefactPaths(JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.Object)
            throw Error(path, "must be an object");

        var artefacts = new AssuranceArtefactPaths();
        foreach (JsonProperty property in value.EnumerateObject())
        {
            string at = $"{path}.{property.Name}";
            artefacts = property.Name switch
            {
                "report" => artefacts with { Report = RelativePath(property.Value, at) },
                "humanReview" => artefacts with { HumanReview = RelativePath(property.Value, at) },
                "manifest" => artefacts with { Manifest = RelativePath(property.Value, at) },
                _ => throw Error(at, "is not a property this schema defines"),
            };
        }

        string[] all = [artefacts.Report, artefacts.HumanReview, artefacts.Manifest];
        if (all.Distinct(StringComparer.OrdinalIgnoreCase).Count() != all.Length)
            throw Error(path, "names one file for two artefacts");

        return artefacts;
    }

    /// <summary>Lines of prose, where an empty string is a blank line.</summary>
    private static IReadOnlyList<string> CommentLines(JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.Array)
            throw Error(path, "must be an array of strings");

        var lines = new List<string>();
        int index = 0;
        foreach (JsonElement entry in value.EnumerateArray())
        {
            string at = $"{path}[{index++}]";
            if (entry.ValueKind != JsonValueKind.String)
                throw Error(at, "must be a string");

            lines.Add(CheckSingleLine(entry.GetString()!, at));
        }

        // The generator recognizes its own manifest by this opening, and would
        // otherwise refuse to replace a manifest it wrote itself.
        if (lines.Count == 0 || !lines[0].StartsWith("GENERATED - DO NOT EDIT MANUALLY", StringComparison.Ordinal))
            throw Error(path, "must open with a line starting 'GENERATED - DO NOT EDIT MANUALLY'");

        return lines;
    }

    private static IReadOnlyList<string> Symbols(JsonElement value, string path)
    {
        IReadOnlyList<string> symbols = StringList(value, path);
        for (int index = 0; index < symbols.Count; index++)
        {
            string symbol = symbols[index];
            if (symbol.Length == 0 || !symbol.All(static c => char.IsLetterOrDigit(c) || c == '_'))
                throw Error($"{path}[{index}]", $"'{symbol}' is not a preprocessor symbol");
        }

        return symbols;
    }

    private static AssuranceGlob Glob(string pattern, string path) =>
        AssuranceGlob.TryParse(pattern, out AssuranceGlob? glob, out string? problem)
            ? glob!
            : throw Error(path, problem!);

    private static IReadOnlyList<string> StringList(JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.Array)
            throw Error(path, "must be an array of strings");

        var list = new List<string>();
        int index = 0;
        foreach (JsonElement entry in value.EnumerateArray())
            list.Add(RequiredString(entry, $"{path}[{index++}]"));

        return list;
    }

    private static string RelativePath(JsonElement value, string path) =>
        CheckRelative(RequiredString(value, path), path);

    private static string CheckRelative(string value, string path)
    {
        if (value.Contains('\\', StringComparison.Ordinal))
            throw Error(path, $"'{value}' uses '\\'; write paths with '/'");

        if (value.StartsWith('/') || (value.Length > 1 && value[1] == ':'))
            throw Error(path, $"'{value}' is rooted; paths are relative to the component root");

        if (value.Split('/').Any(static segment => segment is ".." or "." or ""))
            throw Error(path, $"'{value}' has an empty, '.' or '..' segment");

        return value;
    }

    private static string SingleLine(JsonElement value, string path) =>
        CheckSingleLine(RequiredString(value, path), path);

    private static string CheckSingleLine(string value, string path)
    {
        if (value.AsSpan().IndexOfAny("\r\n\u0085\u2028\u2029") >= 0)
            throw Error(path, "must be one line");

        return value;
    }

    private static string RequiredString(JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.String)
            throw Error(path, "must be a string");

        string text = value.GetString()!.Trim();
        if (text.Length == 0)
            throw Error(path, "must not be empty");

        return text;
    }

    private static bool RequiredBool(JsonElement value, string path) => value.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        _ => throw Error(path, "must be true or false"),
    };

    private static AssuranceConfigException Error(string path, string message) =>
        new($"{FileName}: {path} {message}");
}
