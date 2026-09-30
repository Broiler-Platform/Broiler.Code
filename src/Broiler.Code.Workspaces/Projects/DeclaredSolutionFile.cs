// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           9
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    High
// Criteria:         11/5
// Resource impact:  7/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Broiler.Code.Workspaces.Projects;

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=E94056
// Broiler-Human:        PENDING
public sealed record SolutionProjectEntry(
    string RelativePath,
    string Name,
    string? Folder,
    string? Guid);

/// <summary>
/// A solution file, in either supported format.
///
/// Both formats are read; both round-trip byte-identically when unchanged. The
/// classic <c>.sln</c> keeps its project GUIDs exactly: regenerating one
/// silently detaches per-user state and source-control history from the
/// project, and nothing about the file tells you that happened.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=2D6A28
// Broiler-Falsified-If: a classic .sln made of many lines that begin with Project("{ and never close the brace takes time that grows with the square of its size to parse
// Broiler-Human:        PENDING
public sealed class DeclaredSolutionFile
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=BD6E9B
    // Broiler-Falsified-If: a match attempt that starts on one line scans on through every later line, so a file of many unterminated Project lines costs time quadratic in its size
    // Broiler-Human:        PENDING
    private static readonly Regex SlnProjectPattern = new(
        """^Project\("\{(?<type>[^}]+)\}"\)\s*=\s*"(?<name>[^"]*)",\s*"(?<path>[^"]*)",\s*"\{(?<guid>[^}]+)\}"\s*$""",
        RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

    /// <summary>The GUID classic .sln uses for a solution folder rather than a project.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=FCFF7C
    // Broiler-Falsified-If: a classic .sln folder entry whose type GUID is 2150E333-8FDC-42A3-9474-1A3956D46DE1 is listed among the projects instead of the folders
    // Broiler-Human:        PENDING
    private const string SolutionFolderTypeGuid = "2150E333-8FDC-42A3-9474-1A3956D46DE1";

    private readonly string _text;

    private DeclaredSolutionFile(
        string relativePath,
        string text,
        SolutionFormat format,
        IReadOnlyList<SolutionProjectEntry> projects,
        IReadOnlyList<string> folders,
        IReadOnlyList<string> solutionItems)
    {
        RelativePath = relativePath;
        _text = text;
        Format = format;
        Projects = projects;
        Folders = folders;
        SolutionItems = solutionItems;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=7EBC93
    // Broiler-Human:        PENDING
    public enum SolutionFormat
    {
        Slnx,
        Sln,
    }

    public string RelativePath { get; }

    public SolutionFormat Format { get; }

    public IReadOnlyList<SolutionProjectEntry> Projects { get; }

    public IReadOnlyList<string> Folders { get; }

    public IReadOnlyList<string> SolutionItems { get; }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B7515B
    // Broiler-Falsified-If: the returned text differs from the text given to Parse, so an unchanged solution is not written back byte-identically
    // Broiler-Human:        PENDING
    public override string ToString() => _text;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=ADAC76
    // Broiler-Falsified-If: a file whose name ends in .slnx in any letter case is handed to the classic .sln matcher, so its projects are silently dropped
    // Broiler-Human:        PENDING
    public static DeclaredSolutionFile Parse(string relativePath, string text)
    {
        ArgumentNullException.ThrowIfNull(relativePath);
        ArgumentNullException.ThrowIfNull(text);

        return relativePath.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase)
            ? ParseSlnx(relativePath, text)
            : ParseSln(relativePath, text);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=24B745
    // Broiler-Falsified-If: a .slnx whose internal DTD nests entity references expands past ten million characters instead of failing with an XmlException
    // Broiler-Human:        PENDING
    private static DeclaredSolutionFile ParseSlnx(string relativePath, string text)
    {
        XDocument document = XDocument.Parse(text, LoadOptions.PreserveWhitespace);
        var projects = new List<SolutionProjectEntry>();
        var folders = new List<string>();
        var items = new List<string>();

        foreach (XElement element in document.Descendants())
        {
            switch (element.Name.LocalName)
            {
                case "Folder":
                    if ((string?)element.Attribute("Name") is { Length: > 0 } name)
                        folders.Add(name);
                    break;

                case "Project":
                    if ((string?)element.Attribute("Path") is { Length: > 0 } path)
                    {
                        string normalized = path.Replace('\\', '/');
                        projects.Add(new SolutionProjectEntry(
                            normalized,
                            NameFromPath(normalized),
                            FolderOf(element),
                            Guid: null));
                    }

                    break;

                case "File":
                    if ((string?)element.Attribute("Path") is { Length: > 0 } file)
                        items.Add(file.Replace('\\', '/'));
                    break;
            }
        }

        return new DeclaredSolutionFile(
            relativePath, text, SolutionFormat.Slnx, projects, folders, items);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=BD371B
    // Broiler-Falsified-If: a classic .sln made of many lines that begin with Project("{ and never close the brace takes time that grows with the square of its size to parse
    // Broiler-Human:        PENDING
    private static DeclaredSolutionFile ParseSln(string relativePath, string text)
    {
        var projects = new List<SolutionProjectEntry>();
        var folders = new List<string>();

        foreach (Match match in SlnProjectPattern.Matches(text))
        {
            string type = match.Groups["type"].Value;
            string name = match.Groups["name"].Value;
            string path = match.Groups["path"].Value.Replace('\\', '/');
            string guid = match.Groups["guid"].Value;

            // A solution folder is a Project entry with a reserved type GUID and
            // a path equal to its name. Treating it as a project would put a
            // folder in the build graph.
            if (string.Equals(type, SolutionFolderTypeGuid, StringComparison.OrdinalIgnoreCase))
            {
                folders.Add(name);
                continue;
            }

            projects.Add(new SolutionProjectEntry(path, name, Folder: null, guid));
        }

        return new DeclaredSolutionFile(
            relativePath, text, SolutionFormat.Sln, projects, folders, []);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E3D182
    // Broiler-Falsified-If: a Project nested in a Folder element reports a folder name other than that element's Name attribute
    // Broiler-Human:        PENDING
    private static string? FolderOf(XElement project)
    {
        XElement? parent = project.Parent;
        return parent?.Name.LocalName == "Folder" ? (string?)parent.Attribute("Name") : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=93A79B
    // Broiler-Falsified-If: a path whose directory contains a dot but whose file name has none, such as tools.v2/build, returns a name cut at the directory's dot
    // Broiler-Human:        PENDING
    private static string NameFromPath(string path)
    {
        int slash = path.LastIndexOf('/');
        string file = slash < 0 ? path : path[(slash + 1)..];
        int dot = file.LastIndexOf('.');
        return dot < 0 ? file : file[..dot];
    }

    /// <summary>
    /// A stable display order: by folder then by name, using an ordinal
    /// comparison so the tree looks the same on every platform.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B07BE9
    // Broiler-Falsified-If: the returned list omits or repeats one of Projects, so a declared project is loaded twice or not at all
    // Broiler-Human:        PENDING
    public IReadOnlyList<SolutionProjectEntry> InDisplayOrder() =>
    [
        .. Projects
            .OrderBy(entry => entry.Folder ?? string.Empty, StringComparer.Ordinal)
            .ThenBy(entry => entry.Name, StringComparer.Ordinal),
    ];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=27AE37
    // Broiler-Falsified-If: the text reports a project or folder count other than Projects.Count or Folders.Count
    // Broiler-Human:        PENDING
    public string Describe() => string.Create(
        CultureInfo.InvariantCulture,
        $"{Format} solution with {Projects.Count} projects and {Folders.Count} folders");
}
