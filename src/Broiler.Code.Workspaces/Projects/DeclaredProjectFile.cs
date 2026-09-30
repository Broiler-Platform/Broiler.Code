// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           9
// Human-reviewed:   0/19
// IP risk:          Low
// Security risk:    High
// Criteria:         17/2
// Resource impact:  7/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Broiler.Code.Workspaces.Projects;

/// <summary>
/// Which of the four mutation classes a construct falls into. Frozen by
/// docs/architecture/broiler-code-project-mutations.md.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2955E5
// Broiler-Human:        PENDING
public enum MutationClass
{
    /// <summary>Read and round-tripped byte-identically; not offered for edit.</summary>
    Lossless,

    /// <summary>Read and editable with a minimal diff.</summary>
    Editable,

    /// <summary>Declaration readable; its effect needs a trusted evaluation.</summary>
    EvaluatedOnly,

    /// <summary>Preserved verbatim and never modified.</summary>
    Unsupported,
}

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F76646
// Broiler-Human:        PENDING
public sealed record ProjectConstruct(
    string Description,
    MutationClass Class,
    string Reason);

/// <summary>
/// An SDK-style project file, held as text plus a parsed view.
///
/// The text is authoritative. Edits are applied to the original character
/// sequence, not by reserializing a document object model, because every XML
/// serializer normalizes something — attribute quoting, self-closing tags,
/// blank lines, indentation — and a project file is under version control. An
/// IDE that reformats it on open makes every future diff unreadable, which is
/// a worse outcome than not supporting the edit at all.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=B06DBD
// Broiler-Falsified-If: a project file whose DOCTYPE declares an external entity gets the contents of the file or URL it names substituted into the parsed values
// Broiler-Human:        PENDING
public sealed class DeclaredProjectFile
{
    private readonly string _text;
    private readonly XDocument _document;

    private DeclaredProjectFile(
        string relativePath,
        string text,
        XDocument document,
        IReadOnlyList<ProjectConstruct> constructs)
    {
        RelativePath = relativePath;
        _text = text;
        _document = document;
        Constructs = constructs;
    }

    public string RelativePath { get; }

    /// <summary>Every construct found, with its mutation class.</summary>
    public IReadOnlyList<ProjectConstruct> Constructs { get; }

    /// <summary>
    /// True when the project contains something the declared model can read but
    /// must not modify. Structural edits are refused wholesale rather than
    /// applied around the unknown part, because "edit the bits I understood"
    /// is exactly how a construct gets half-rewritten.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4CF2C0
    // Broiler-Falsified-If: a project containing a Choose, Target or UsingTask element reports false, so AddProjectReference edits it
    // Broiler-Human:        PENDING
    public bool HasUnsupportedConstructs =>
        Constructs.Any(construct => construct.Class == MutationClass.Unsupported);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=F5EDED
    // Broiler-Falsified-If: a project file whose internal DTD expands its entities past ten million characters is parsed instead of failing with an XmlException
    // Broiler-Human:        PENDING
    public static DeclaredProjectFile Parse(string relativePath, string text)
    {
        ArgumentNullException.ThrowIfNull(relativePath);
        ArgumentNullException.ThrowIfNull(text);

        // PreserveWhitespace keeps the original formatting in the tree so the
        // parsed view and the text agree about where things are.
        XDocument document = XDocument.Parse(text, LoadOptions.PreserveWhitespace);
        return new DeclaredProjectFile(relativePath, text, document, Classify(document));
    }

    /// <summary>
    /// The original text, unchanged. A save that has applied no edit writes
    /// exactly what was read, down to the byte.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B7515B
    // Broiler-Falsified-If: the returned text differs from the text given to Parse although no edit was applied
    // Broiler-Human:        PENDING
    public override string ToString() => _text;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DF85CB
    // Broiler-Falsified-If: a project with two unconditional PropertyGroups that each set TargetFramework reports the first value, although the build uses the later one
    // Broiler-Human:        PENDING
    public IReadOnlyList<string> GetTargetFrameworks()
    {
        // Both spellings, in declared order. An unconditional group only: a
        // conditioned TargetFrameworks is evaluated-only, and reporting it as
        // though it always applied would be a guess.
        foreach (XElement group in UnconditionalPropertyGroups())
        {
            XElement? plural = group.Elements().LastOrDefault(e => e.Name.LocalName == "TargetFrameworks");
            if (plural is not null)
            {
                return [.. plural.Value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
            }

            XElement? single = group.Elements().LastOrDefault(e => e.Name.LocalName == "TargetFramework");
            if (single is not null)
                return [single.Value.Trim()];
        }

        return [];
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=789909
    // Broiler-Falsified-If: a ProjectReference whose Include uses backslashes is returned with a backslash, so the duplicate check in AddProjectReference misses it
    // Broiler-Human:        PENDING
    public IReadOnlyList<string> GetProjectReferences() =>
    [
        .. _document.Descendants()
            .Where(e => e.Name.LocalName == "ProjectReference")
            .Select(e => (string?)e.Attribute("Include"))
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => include!.Replace('\\', '/')),
    ];

    /// <summary>
    /// Compile items declared explicitly, with their link when they have one.
    /// Implicit SDK globs are deliberately absent: the item set they produce is
    /// a property of the filesystem, not of this file, and only an evaluation
    /// can say what it is.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A5F5AA
    // Broiler-Falsified-If: a Compile Include holding a wildcard or a semicolon-separated list, such as '..\Shared\**\*.cs', is returned as one literal path
    // Broiler-Human:        PENDING
    public IReadOnlyList<(string Include, string? Link)> GetExplicitCompileItems() =>
    [
        .. _document.Descendants()
            .Where(e => e.Name.LocalName == "Compile" && e.Attribute("Include") is not null)
            .Select(e => (
                Include: ((string)e.Attribute("Include")!).Replace('\\', '/'),
                Link: ((string?)e.Attribute("Link"))?.Replace('\\', '/'))),
    ];

    /// <summary>
    /// Adds a project reference, returning the new file text.
    ///
    /// The edit is a text insertion, so everything else in the file is
    /// untouched by construction. It is refused outright when the project holds
    /// an unsupported construct.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F92C92
    // Broiler-Falsified-If: a project whose last ProjectReference sits in an ItemGroup with a Condition receives the new reference inside that conditional group
    // Broiler-Human:        PENDING
    public ProjectEditResult AddProjectReference(string includePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(includePath);
        if (HasUnsupportedConstructs)
        {
            return ProjectEditResult.Refused(
                "BRW0101",
                $"'{RelativePath}' contains a construct the declared model cannot safely edit.");
        }

        string normalized = includePath.Replace('/', '\\');
        if (GetProjectReferences().Any(existing =>
                string.Equals(existing, includePath.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase)))
        {
            return ProjectEditResult.Refused("BRW0102", $"'{includePath}' is already referenced.");
        }

        string indent = DetectIndent();
        string newline = DetectNewLine();
        string element = $"{indent}{indent}<ProjectReference Include=\"{Escape(normalized)}\" />";

        // Prefer an existing ItemGroup that already holds ProjectReferences, so
        // the addition lands where a human would have put it.
        int insertion = FindProjectReferenceInsertionPoint();
        if (insertion >= 0)
            return ProjectEditResult.Applied(_text.Insert(insertion, element + newline));

        // Otherwise a new group immediately before the closing tag.
        int closing = _text.LastIndexOf("</Project>", StringComparison.Ordinal);
        if (closing < 0)
            return ProjectEditResult.Refused("BRW0103", "The project has no closing element.");

        string group =
            $"{newline}{indent}<ItemGroup>{newline}{element}{newline}{indent}</ItemGroup>{newline}";
        return ProjectEditResult.Applied(_text.Insert(closing, group));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0BA5D2
    // Broiler-Falsified-If: when the last ProjectReference element spans several lines, the returned offset falls inside that element, before its closing tag
    // Broiler-Human:        PENDING
    private int FindProjectReferenceInsertionPoint()
    {
        // The line after the last ProjectReference element, found in the text
        // rather than through the tree so the offset is exact.
        int last = _text.LastIndexOf("<ProjectReference", StringComparison.Ordinal);
        if (last < 0)
            return -1;

        int lineEnd = _text.IndexOf('\n', last);
        return lineEnd < 0 ? -1 : lineEnd + 1;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=3; Fingerprint=D21237
    // Broiler-Falsified-If: a file with CRLF line endings gets the inserted reference terminated by a bare LF
    // Broiler-Human:        PENDING
    private string DetectNewLine() => _text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=1A5BF5
    // Broiler-Falsified-If: a file indented with tabs gets the new element indented with spaces
    // Broiler-Human:        PENDING
    private string DetectIndent()
    {
        // Whatever the file already uses. Imposing two spaces on a file indented
        // with tabs produces a diff on every line the editor touches.
        foreach (string line in _text.Split('\n'))
        {
            string trimmed = line.TrimStart(' ', '\t');
            if (trimmed.Length == 0 || trimmed.Length == line.Length)
                continue;
            if (!trimmed.StartsWith('<') || trimmed.StartsWith("<?", StringComparison.Ordinal))
                continue;
            return line[..(line.Length - trimmed.Length)];
        }

        return "  ";
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=03B477
    // Broiler-Falsified-If: a PropertyGroup nested in a Choose or Target, or one carrying a Condition, is returned as unconditional
    // Broiler-Human:        PENDING
    private IEnumerable<XElement> UnconditionalPropertyGroups() =>
        _document.Root?.Elements()
            .Where(e => e.Name.LocalName == "PropertyGroup" && e.Attribute("Condition") is null)
        ?? [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0A9D0F
    // Broiler-Falsified-If: a double quote, an ampersand or an angle bracket in the include path survives unescaped in the inserted attribute
    // Broiler-Human:        PENDING
    private static string Escape(string value) =>
        new StringBuilder(value)
            .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;")
            .ToString();

    /// <summary>
    /// Walks the file and assigns every construct its mutation class. This is
    /// the matrix in executable form; a construct not listed here is
    /// Unsupported by the escalation rule, not by omission.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5469EB
    // Broiler-Falsified-If: a project whose only construct outside the matrix is an unknown top-level element, such as ItemDefinitionGroup or ProjectExtensions, gets no Unsupported construct
    // Broiler-Human:        PENDING
    private static List<ProjectConstruct> Classify(XDocument document)
    {
        var constructs = new List<ProjectConstruct>();
        XElement? root = document.Root;
        if (root is null)
            return constructs;

        if (root.Attribute("Sdk") is not null)
        {
            constructs.Add(new ProjectConstruct(
                "Sdk attribute", MutationClass.Editable, "A single well-known attribute."));
        }

        foreach (XElement element in root.Descendants())
        {
            switch (element.Name.LocalName)
            {
                case "Choose" or "When" or "Otherwise":
                    constructs.Add(new ProjectConstruct(
                        $"<{element.Name.LocalName}> block",
                        MutationClass.Unsupported,
                        "Modelling the branch correctly requires evaluating its condition."));
                    break;

                case "Target" or "UsingTask":
                    constructs.Add(new ProjectConstruct(
                        $"<{element.Name.LocalName}> element",
                        MutationClass.Unsupported,
                        "Custom build logic; its effect is only knowable by running it."));
                    break;

                case "Import":
                    constructs.Add(new ProjectConstruct(
                        "Import",
                        element.Attribute("Sdk") is not null
                            ? MutationClass.EvaluatedOnly
                            : MutationClass.Lossless,
                        "What an import contributes is knowable only after evaluation."));
                    break;

                case "PropertyGroup" or "ItemGroup" when element.Attribute("Condition") is not null:
                    constructs.Add(new ProjectConstruct(
                        $"Conditional <{element.Name.LocalName}>",
                        MutationClass.EvaluatedOnly,
                        "Whether it applies depends on the target framework, configuration, and RID."));
                    break;

                case "TargetFrameworks" or "TargetFramework":
                    constructs.Add(new ProjectConstruct(
                        element.Name.LocalName,
                        element.Parent?.Attribute("Condition") is null
                            ? MutationClass.Editable
                            : MutationClass.EvaluatedOnly,
                        "The one property the IDE must be able to change to be useful."));
                    break;

                case "ProjectReference":
                    constructs.Add(new ProjectConstruct(
                        "ProjectReference",
                        MutationClass.Editable,
                        "The graph edge the IDE most needs to manage."));
                    break;

                case "PackageReference":
                    constructs.Add(new ProjectConstruct(
                        "PackageReference",
                        element.Attribute("Version") is null
                            ? MutationClass.EvaluatedOnly
                            : MutationClass.Editable,
                        "A version from Directory.Packages.props lives in another file."));
                    break;

                case "Compile" or "None" or "Content" or "EmbeddedResource":
                    string? include = (string?)element.Attribute("Include");
                    bool wildcard = include is not null &&
                        (include.Contains('*', StringComparison.Ordinal) ||
                         include.Contains('?', StringComparison.Ordinal));
                    constructs.Add(new ProjectConstruct(
                        $"{element.Name.LocalName} item",
                        wildcard ? MutationClass.EvaluatedOnly : MutationClass.Editable,
                        wildcard
                            ? "The item set a wildcard produces is a property of the filesystem."
                            : "One path, one item, no expansion."));
                    break;
            }
        }

        return constructs;
    }
}

/// <summary>
/// The outcome of a project edit. A refusal carries a code and a reason,
/// because "nothing happened" is the least useful thing an IDE can tell
/// someone who just clicked Add Reference.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=62DF41
// Broiler-Falsified-If: a refused result reports Accepted true or carries new text
// Broiler-Human:        PENDING
public readonly record struct ProjectEditResult(
    bool Accepted,
    string? NewText,
    string? Code,
    string? Message)
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A9F88A
    // Broiler-Falsified-If: a result made by Applied reports Accepted false or drops the new text
    // Broiler-Human:        PENDING
    public static ProjectEditResult Applied(string newText) => new(true, newText, null, null);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F7BFFD
    // Broiler-Falsified-If: a result made by Refused reports Accepted true or carries new text
    // Broiler-Human:        PENDING
    public static ProjectEditResult Refused(string code, string message) =>
        new(false, null, code, message);
}
