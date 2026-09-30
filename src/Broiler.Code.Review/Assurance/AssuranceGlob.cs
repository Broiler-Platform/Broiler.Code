using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// A path pattern from <c>assurance.config.json</c>, matched against a
/// root-relative path with forward slashes.
///
/// The syntax is the small common subset of gitignore and MSBuild globs:
/// <c>*</c> matches within one path segment, <c>?</c> matches one character
/// other than <c>/</c>, and <c>**</c> matches any number of whole segments.
/// A pattern with no <c>/</c> in it matches the file name at any depth, so
/// <c>*.g.cs</c> means every generated file. Matching is case-sensitive, as it
/// is on the Linux runners where these configs are checked; a pattern that only
/// matched on Windows would exclude nothing in CI.
/// </summary>
public sealed class AssuranceGlob
{
    private readonly Regex _regex;
    private readonly bool _nameOnly;

    private AssuranceGlob(string pattern, Regex regex, bool nameOnly)
    {
        Pattern = pattern;
        _regex = regex;
        _nameOnly = nameOnly;
    }

    /// <summary>The pattern as written.</summary>
    public string Pattern { get; }

    /// <summary>
    /// Compiles <paramref name="pattern"/>, or explains why it cannot be one.
    /// A rooted pattern, a backslash or a <c>..</c> segment is refused: the
    /// pattern would never match a root-relative path, and a pattern that
    /// silently matches nothing is an exclusion nobody notices failing.
    /// </summary>
    public static bool TryParse(string? pattern, out AssuranceGlob? glob, out string? problem)
    {
        glob = null;
        problem = null;

        if (string.IsNullOrWhiteSpace(pattern))
        {
            problem = "a glob is empty";
            return false;
        }

        if (pattern.Contains('\\', StringComparison.Ordinal))
        {
            problem = $"the glob '{pattern}' uses '\\'; write paths with '/'";
            return false;
        }

        if (pattern.StartsWith('/') || (pattern.Length > 1 && pattern[1] == ':'))
        {
            problem = $"the glob '{pattern}' is rooted; globs are relative to the component root";
            return false;
        }

        foreach (string segment in pattern.Split('/'))
        {
            if (segment is ".." or ".")
            {
                problem = $"the glob '{pattern}' has a '{segment}' segment";
                return false;
            }
        }

        bool nameOnly = !pattern.Contains('/', StringComparison.Ordinal);
        glob = new AssuranceGlob(pattern, new Regex(ToRegex(pattern), RegexOptions.CultureInvariant), nameOnly);
        return true;
    }

    /// <summary>Whether <paramref name="relativePath"/> (forward slashes) matches.</summary>
    public bool IsMatch(string relativePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);

        string subject = _nameOnly ? relativePath[(relativePath.LastIndexOf('/') + 1)..] : relativePath;
        return _regex.IsMatch(subject);
    }

    public override string ToString() => Pattern;

    private static string ToRegex(string pattern)
    {
        var regex = new StringBuilder("^");
        for (int index = 0; index < pattern.Length; index++)
        {
            char character = pattern[index];
            if (character == '*' && index + 1 < pattern.Length && pattern[index + 1] == '*')
            {
                bool segmentStart = index == 0 || pattern[index - 1] == '/';
                bool slashAfter = index + 2 < pattern.Length && pattern[index + 2] == '/';
                if (segmentStart && slashAfter)
                {
                    // "**/" at a segment start: zero or more whole directories.
                    regex.Append("(?:[^/]*/)*");
                    index += 2;
                }
                else
                {
                    regex.Append(".*");
                    index++;
                }
            }
            else if (character == '*')
            {
                regex.Append("[^/]*");
            }
            else if (character == '?')
            {
                regex.Append("[^/]");
            }
            else
            {
                regex.Append(Regex.Escape(character.ToString()));
            }
        }

        return regex.Append('$').ToString();
    }
}
