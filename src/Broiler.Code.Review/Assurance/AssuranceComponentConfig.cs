// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   34
// Annotated:        34/34
// Exempt:           31
// Human-reviewed:   0/34
// IP risk:          Low
// Security risk:    High
// Criteria:         22/20
// Resource impact:  7/10 max
// Unverified:       34
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Broiler.Code.Review.Assurance;

/// <summary>Who writes a component's assurance artefacts.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E57DA4
// Broiler-Human:        PENDING
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
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2E0594
// Broiler-Human:        PENDING
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

/// <summary>Which exemption predicate decides that a unit needs no review.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=022A7C
// Broiler-Human:        PENDING
public enum AssuranceExemptionPredicate
{
    /// <summary>
    /// The owning component's eight cases, narrowed where they would exempt code
    /// that runs: a field or auto-property is exempt only when its initializer,
    /// if it has one, is inert (a literal, a name, <c>default</c>, <c>nameof</c>,
    /// <c>typeof</c>, a parameterless <c>new()</c>, or arithmetic over those); an
    /// expression body that throws is exempt only when the exception's arguments
    /// are inert; and a member of a type named <c>AssemblyMarker</c> gets no
    /// exemption for where it lives. An initializer that calls something or
    /// holds a lambda is code nobody else reviews, so its unit is relevant.
    /// </summary>
    Strict = 0,

    /// <summary>
    /// The owning component's predicate exactly, for a component whose record
    /// that component's own generator writes (Broiler.VM).
    /// </summary>
    OwningComponent,
}

/// <summary>Whether a named value is a unit someone assesses or one the manifest only watches.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=46771E
// Broiler-Human:        PENDING
public enum AssuranceNamedValues
{
    /// <summary>
    /// A named value is relevant like any other declaration and carries a
    /// block, as the owning component treats it.
    /// </summary>
    Reviewed = 0,

    /// <summary>
    /// A named value (a const, an enum, a static readonly Guid or handle
    /// stated by literals) is exempt as <c>NamedValue</c>: it carries no
    /// block, and its fingerprint stays in the manifest, so a change to it
    /// still moves a value the check compares. For a component of values
    /// transcribed from elsewhere, such as a platform SDK's defines, which
    /// carry no decision to assess.
    /// </summary>
    Watched,
}

/// <summary>Whether the check accepts a falsification criterion on a block assessed below High.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=77EDCA
// Broiler-Human:        PENDING
public enum AssuranceCriteriaBelowHigh
{
    /// <summary>
    /// A criterion on a block assessed <c>None</c>, <c>Low</c> or <c>Medium</c>
    /// is accepted, as the owning component accepts it and as blocks written
    /// before the rubric stopped writing one there carry it.
    /// </summary>
    Permitted = 0,

    /// <summary>
    /// A criterion is written only for <c>High</c> and <c>Critical</c>, and the
    /// check reports one below them. For a component whose blocks
    /// <c>prune</c> has brought to that rule, so that none comes back.
    /// </summary>
    Refused,
}

/// <summary>A file pattern the component leaves out of the covered set, and why.</summary>
/// <param name="Glob">The pattern.</param>
/// <param name="Reason">Why, in words a report can print beside each file it excludes.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=58D106
// Broiler-Human:        PENDING
public sealed record AssuranceExclusion(AssuranceGlob Glob, string Reason);

/// <summary>
/// The SPDX lines a generated file header opens with.
/// </summary>
/// <param name="Copyright">
/// One or more <c>SPDX-FileCopyrightText</c> values, without the prefix:
/// <c>2026 Broiler Platform contributors</c>.
/// </param>
/// <param name="License">One SPDX licence expression: <c>Apache-2.0 AND BSD-3-Clause</c>.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4A048D
// Broiler-Human:        PENDING
public sealed record AssuranceSpdx(IReadOnlyList<string> Copyright, string License);

/// <summary>SPDX lines for the files one pattern matches, in place of the component default.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C40E62
// Broiler-Human:        PENDING
public sealed record AssuranceSpdxOverride(AssuranceGlob Glob, AssuranceSpdx Spdx);

/// <summary>Where the component-level artefacts are written, relative to the component root.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=35C1E6
// Broiler-Falsified-If: a default artefact path is rooted or has a '..' or '.git' segment, which reaches the writer unchecked because only configured values pass CheckRelative
// Broiler-Human:        PENDING
public sealed record AssuranceArtefactPaths
{
    public string Report { get; init; } = "CODE-ASSURANCE.md";

    public string HumanReview { get; init; } = "HUMAN_REVIEW.md";

    public string Manifest { get; init; } = "assurance.manifest.json";
}

/// <summary>Why a configuration file was refused.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A55C0E
// Broiler-Human:        PENDING
public sealed class AssuranceConfigException : Exception
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=50D11C
    // Broiler-Human:        PENDING
    public AssuranceConfigException(string message)
        : base(message)
    {
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=BF0C01
    // Broiler-Human:        PENDING
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
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=E258A8
// Broiler-Falsified-If: an artefacts path whose segment Windows resolves to '.git', such as '.git./info/x', is accepted by Parse, so generate on Windows writes inside the git directory
// Broiler-Human:        PENDING
public sealed record AssuranceComponentConfig
{
    /// <summary>The configuration's file name, at the component root.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=AF4861
    // Broiler-Falsified-If: the opt-in file name matches a file components carry for another reason, so insert and generate write to a component that never opted in
    // Broiler-Human:        PENDING
    public const string FileName = "assurance.config.json";

    /// <summary>The only schema version this build reads.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=0498C8
    // Broiler-Falsified-If: Parse accepts a schema number other than the one whose properties it defines
    // Broiler-Human:        PENDING
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

    /// <summary>
    /// Whether a <c>bin</c> or <c>obj</c> directory below a project's root is
    /// left out as well as the project's own. On by default, because some
    /// components track stale build output; the files it leaves out are listed
    /// as not covered. The owning component, like the SDK's default compile
    /// items, leaves out only the project's own <c>bin</c> and <c>obj</c>, and
    /// covers a file in a deeper one: set this to false for its rule.
    /// </summary>
    public bool ExcludeBuildOutputAtAnyDepth { get; init; } = true;

    /// <summary>Which exemption predicate the scanner applies.</summary>
    public AssuranceExemptionPredicate ExemptionPredicate { get; init; } = AssuranceExemptionPredicate.Strict;

    /// <summary>
    /// Whether named values are assessed or only watched. Assessed unless the
    /// configuration says otherwise, so a component that does not set it sees
    /// exactly the units it saw before the option existed.
    /// </summary>
    public AssuranceNamedValues NamedValues { get; init; } = AssuranceNamedValues.Reviewed;

    /// <summary>
    /// Whether the check accepts a criterion below High. Accepted unless the
    /// configuration says otherwise, so a component that does not set it is
    /// checked exactly as it was before the option existed.
    /// </summary>
    public AssuranceCriteriaBelowHigh CriteriaBelowHigh { get; init; } = AssuranceCriteriaBelowHigh.Permitted;

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=8EDAAD
    // Broiler-Falsified-If: a file matched by two spdxOverrides globs receives the second override's lines instead of the first's
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=899C44
    // Broiler-Falsified-If: an exclude glob of eight '*a' steps makes matching one 40-character file name take seconds, because the glob's regex runs with no match timeout
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=8BA3DA
    // Broiler-Falsified-If: a top-level property this schema does not define, such as a misspelled exclude, is accepted instead of raising AssuranceConfigException
    // Broiler-Human:        PENDING
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
                        config = config with { Component = SingleLine(value, path) };
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

                    case "excludeBuildOutputAtAnyDepth":
                        config = config with { ExcludeBuildOutputAtAnyDepth = RequiredBool(value, path) };
                        break;

                    case "exemptionPredicate":
                        config = config with
                        {
                            ExemptionPredicate = RequiredString(value, path) switch
                            {
                                "strict" => AssuranceExemptionPredicate.Strict,
                                "owning-component" => AssuranceExemptionPredicate.OwningComponent,
                                _ => throw Error(path, "must be \"strict\" or \"owning-component\""),
                            },
                        };
                        break;

                    case "namedValues":
                        config = config with
                        {
                            NamedValues = RequiredString(value, path) switch
                            {
                                "reviewed" => AssuranceNamedValues.Reviewed,
                                "watched" => AssuranceNamedValues.Watched,
                                _ => throw Error(path, "must be \"reviewed\" or \"watched\""),
                            },
                        };
                        break;

                    case "criteriaBelowHigh":
                        config = config with
                        {
                            CriteriaBelowHigh = RequiredString(value, path) switch
                            {
                                "permitted" => AssuranceCriteriaBelowHigh.Permitted,
                                "refused" => AssuranceCriteriaBelowHigh.Refused,
                                _ => throw Error(path, "must be \"permitted\" or \"refused\""),
                            },
                        };
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=54B79C
    // Broiler-Falsified-If: a project path with a '..' segment, a leading slash or a drive letter is returned instead of refused
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=4CD578
    // Broiler-Falsified-If: an exclude object with no glob, or with a property other than glob and reason, is accepted instead of refused
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=035C64
    // Broiler-Falsified-If: an spdxOverrides entry with no glob is accepted instead of raising AssuranceConfigException
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=FD6E84
    // Broiler-Falsified-If: a copyright value that already carries its comment prefix, or spans two lines, is accepted into the generated header
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F7B64B
    // Broiler-Falsified-If: an artefacts entry with a '..' segment or a leading slash is accepted, so generate writes outside the component root
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A15C57
    // Broiler-Falsified-If: a manifestComment whose first line lacks the generator's recognition prefix is accepted, so the next generate refuses to replace the manifest it wrote
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=CFB2D5
    // Broiler-Falsified-If: a preprocessor symbol holding a space or punctuation other than underscore is returned instead of refused
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=1E3147
    // Broiler-Falsified-If: a glob that AssuranceGlob.TryParse refuses, such as one with a backslash or a '..' segment, is returned instead of raising AssuranceConfigException
    // Broiler-Human:        PENDING
    private static AssuranceGlob Glob(string pattern, string path) =>
        AssuranceGlob.TryParse(pattern, out AssuranceGlob? glob, out string? problem)
            ? glob!
            : throw Error(path, problem!);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=0F58E7
    // Broiler-Falsified-If: an array element that is not a string, or is blank, is added to the list instead of refused
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=8513F6
    // Broiler-Falsified-If: an adrDirectory or artefacts value with a '..' segment is returned without passing CheckRelative
    // Broiler-Human:        PENDING
    private static string RelativePath(JsonElement value, string path) =>
        CheckRelative(RequiredString(value, path), path);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=56B0EB
    // Broiler-Falsified-If: a segment that Windows resolves to '.git', such as '.git.' in '.git./hooks/x', is accepted, so an artefact path lands inside the git directory
    // Broiler-Human:        PENDING
    private static string CheckRelative(string value, string path)
    {
        if (value.Contains('\\', StringComparison.Ordinal))
            throw Error(path, $"'{value}' uses '\\'; write paths with '/'");

        if (value.StartsWith('/') || (value.Length > 1 && value[1] == ':'))
            throw Error(path, $"'{value}' is rooted; paths are relative to the component root");

        if (value.Split('/').Any(static segment => segment is ".." or "." or ""))
            throw Error(path, $"'{value}' has an empty, '.' or '..' segment");

        // Git's own directory, in any case: a file written there is not part
        // of the tree, and some of them git executes.
        if (value.Split('/').Any(static segment => string.Equals(segment, ".git", StringComparison.OrdinalIgnoreCase)))
            throw Error(path, $"'{value}' names a path inside a '.git' directory");

        return value;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BE0454
    // Broiler-Falsified-If: a component, reason, license or regenerateCommand value holding a line feed is returned instead of refused
    // Broiler-Human:        PENDING
    private static string SingleLine(JsonElement value, string path) =>
        CheckSingleLine(RequiredString(value, path), path);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5B6EAE
    // Broiler-Falsified-If: a value containing a carriage return, U+0085, U+2028 or U+2029 is returned instead of raising AssuranceConfigException
    // Broiler-Human:        PENDING
    private static string CheckSingleLine(string value, string path)
    {
        if (value.AsSpan().IndexOfAny("\r\n\u0085\u2028\u2029") >= 0)
            throw Error(path, "must be one line");

        return value;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=EBCC94
    // Broiler-Falsified-If: a JSON number, or a string of only spaces, is returned as a value instead of refused
    // Broiler-Human:        PENDING
    private static string RequiredString(JsonElement value, string path)
    {
        if (value.ValueKind != JsonValueKind.String)
            throw Error(path, "must be a string");

        string text = value.GetString()!.Trim();
        if (text.Length == 0)
            throw Error(path, "must not be empty");

        return text;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=B489D0
    // Broiler-Falsified-If: the JSON string "false" or a number is read as a boolean instead of raising AssuranceConfigException
    // Broiler-Human:        PENDING
    private static bool RequiredBool(JsonElement value, string path) => value.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        _ => throw Error(path, "must be true or false"),
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BD9ECA
    // Broiler-Human:        PENDING
    private static AssuranceConfigException Error(string path, string message) =>
        new($"{FileName}: {path} {message}");
}
