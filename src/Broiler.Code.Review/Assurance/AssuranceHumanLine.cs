using System;
using System.Collections.Generic;
using System.Linq;

namespace Broiler.Code.Review.Assurance;

/// <summary>
/// The generator refused to write something, because writing it would create
/// or erase a claim only a human may make.
///
/// Thrown rather than returned, so that no caller can ignore it and write the
/// file anyway. The plan catches it per file and reports it; nothing is written
/// while any refusal stands.
/// </summary>
public sealed class AssuranceRefusalException : Exception
{
    public AssuranceRefusalException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// The human line's write budget: what the generator may turn a
/// <c>// Broiler-Human:</c> body into, and the guard that refuses anything else.
///
/// Ported from the owning component's generator, with its messages word for
/// word. Four inputs have four answers and there is no fifth: <c>PENDING</c>
/// stays <c>PENDING</c>; a reviewer who left the fingerprint to the machine gets
/// it filled; a decision the code has outrun becomes
/// <c>STALE; Previous=reviewer@fingerprint</c>; and a <c>STALE</c> line stays as
/// it is. Nothing here can produce a name the source did not already carry, and
/// <see cref="RefuseInventedApproval"/> checks that on every line it writes.
///
/// Two things are narrower than the owning component's generator, and both
/// close a way to seal an approval nobody gave. A reviewer is an alias
/// (<see cref="AssuranceVocabulary.IsAlias"/>), not any text without an
/// <c>=</c>. And a bare alias is bound only to the version the machine line
/// records, which is the version the generator last wrote down and the one a
/// reviewer can have been reading; if the code has moved since, the decision
/// is recorded as outrun rather than moved onto code nobody saw.
/// </summary>
public static class AssuranceHumanLine
{
    private const string PreviousMarker = "Previous=";

    private static readonly string FingerprintMarker = AssuranceVocabulary.FingerprintField + "=";

    /// <summary>
    /// The body the generator writes for <paramref name="annotation"/>, whose
    /// unit now fingerprints to <paramref name="currentFingerprint"/>.
    /// </summary>
    /// <param name="annotation">The block as the source states it.</param>
    /// <param name="currentFingerprint">The unit's fingerprint now.</param>
    /// <param name="where">The unit, as <c>path(line): name</c>, for a refusal.</param>
    /// <exception cref="AssuranceRefusalException">
    /// A bare alias on a unit whose machine line records no fingerprint yet, so
    /// nothing says which version was approved.
    /// </exception>
    public static string Refreshed(AssuranceAnnotation annotation, string currentFingerprint, string where)
    {
        ArgumentNullException.ThrowIfNull(annotation);
        ArgumentNullException.ThrowIfNull(currentFingerprint);
        ArgumentNullException.ThrowIfNull(where);

        // PENDING stays PENDING, and a STALE line stays exactly as a human
        // will find it: only a human clears one.
        if (annotation.HumanIsPending || annotation.Reviewer is null)
            return annotation.HumanIsStale ? annotation.HumanBody : AssuranceVocabulary.Pending;

        // A line outside the defined shapes is not rewritten here at all; the
        // guard every rewrite passes through refuses it with its own reason.
        if (!IsDefined(annotation.HumanBody))
            return annotation.HumanBody;

        string reviewer = annotation.Reviewer;
        string? approved = annotation.HumanFingerprint;

        // What the reviewer wrote beside their name is theirs, and is carried
        // through; the fingerprint moves to the end and appears once.
        string assessment = string.Concat(
            annotation.HumanBody
                .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Where(static part => !part.StartsWith(FingerprintMarker, StringComparison.Ordinal))
                .Select(static part => "; " + part));

        // A reviewer left the machine field to the machine. This is the only
        // transition into VERIFIED the generator makes, and the name was
        // already on the line. It is made only when the code is the version the
        // machine line records: that is the version the last generation wrote
        // down, and the only one the reviewer can be shown to have had in front
        // of them. The owning component fills whatever the code is now, which
        // seals a rewrite made after the reviewer read it.
        if (approved is null || string.Equals(approved, AssuranceVocabulary.ToBeFilled, StringComparison.Ordinal))
        {
            string? recorded = annotation.RecordedFingerprint;
            if (string.Equals(recorded, currentFingerprint, StringComparison.Ordinal))
                return $"{reviewer}{assessment}; {FingerprintMarker}{currentFingerprint}";

            if (AssuranceVocabulary.IsWellFormedFingerprint(recorded))
                return $"{AssuranceVocabulary.Stale}; {PreviousMarker}{reviewer}@{recorded}";

            throw new AssuranceRefusalException(
                $"The assurance generator will not bind the approval on {where} to a version: its human line " +
                $"reads '{annotation.HumanBody}' and its machine line records " +
                $"{(recorded is null ? "no Fingerprint" : $"Fingerprint={recorded}")}, so nothing says which " +
                $"version {reviewer} approved. Run the generator before recording a decision, or state the " +
                $"version approved as '{reviewer}; {FingerprintMarker}<six hex>'.");
        }

        if (string.Equals(approved, currentFingerprint, StringComparison.Ordinal))
            return $"{reviewer}{assessment}; {FingerprintMarker}{approved}";

        // The code moved after the decision. Who decided and on which version
        // are kept; the reviewer's own assessment of the old version is not,
        // because it describes code that is no longer here. That is the owning
        // component's rule, reproduced rather than improved.
        return $"{AssuranceVocabulary.Stale}; {PreviousMarker}{reviewer}@{approved}";
    }

    /// <summary>
    /// True for the four human-line shapes the format defines and nothing else:
    /// <c>PENDING</c>; a reviewer alias; an alias followed by any of
    /// <c>Fingerprint=</c>, <c>IP=</c>, <c>Security=</c> and <c>Resources=</c>;
    /// or <c>STALE; Previous=reviewer@fingerprint</c>. The head must be an
    /// alias (<see cref="AssuranceVocabulary.IsAlias"/>), where the owning
    /// component takes anything without an <c>=</c>.
    /// </summary>
    public static bool IsDefined(string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        if (string.Equals(body, AssuranceVocabulary.Pending, StringComparison.Ordinal))
            return true;

        string[] parts = body.Split(';', StringSplitOptions.TrimEntries);
        if (parts.Length == 0 || parts[0].Length == 0)
            return false;

        if (string.Equals(parts[0], AssuranceVocabulary.Stale, StringComparison.Ordinal))
        {
            return parts.Length == 2 &&
                parts[1].StartsWith(PreviousMarker, StringComparison.Ordinal) &&
                parts[1].IndexOf('@', StringComparison.Ordinal) > PreviousMarker.Length;
        }

        if (!AssuranceVocabulary.IsAlias(parts[0]))
            return false;

        for (int index = 1; index < parts.Length; index++)
        {
            string part = parts[index];
            if (!AssuranceVocabulary.HumanFieldMarkers.Any(marker => part.StartsWith(marker, StringComparison.Ordinal)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Every name a human line carries: the reviewer, a <c>Previous=</c>
    /// reviewer, and (harmlessly, because they are carried through) the
    /// reviewer's own assessment parts. Only the exact body <c>PENDING</c>
    /// carries none.
    /// </summary>
    public static IReadOnlySet<string> ReviewerNames(string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        var names = new HashSet<string>(StringComparer.Ordinal);
        if (string.Equals(body, AssuranceVocabulary.Pending, StringComparison.Ordinal))
            return names;

        string[] parts = body.Split(';', StringSplitOptions.TrimEntries);
        for (int index = 0; index < parts.Length; index++)
        {
            string part = parts[index];
            if (part.Length == 0 || part.StartsWith(FingerprintMarker, StringComparison.Ordinal))
                continue;

            // A reserved word only where the format puts it: at the head.
            if (index == 0 && string.Equals(part, AssuranceVocabulary.Stale, StringComparison.Ordinal))
                continue;

            string value = part.StartsWith(PreviousMarker, StringComparison.Ordinal)
                ? part[PreviousMarker.Length..]
                : part;

            int at = value.LastIndexOf('@');
            names.Add(at < 0 ? value : value[..at]);
        }

        return names;
    }

    /// <summary>
    /// Throws when <paramref name="before"/> is not a defined shape, or when
    /// <paramref name="after"/> names anyone <paramref name="before"/> did not.
    /// </summary>
    /// <param name="where">The unit, as <c>path(line): name</c>.</param>
    /// <param name="before">The human body as the source states it.</param>
    /// <param name="after">The body the generator is about to write.</param>
    public static void RefuseInventedApproval(string where, string before, string after)
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);

        if (!IsDefined(before))
        {
            throw new AssuranceRefusalException(
                $"The assurance generator will not rewrite the human line on {where}, which " +
                $"reads '{before}'. A human line is one of '{AssuranceVocabulary.Pending}', a " +
                "reviewer, a reviewer with a Fingerprint, or " +
                $"'{AssuranceVocabulary.Stale}; {PreviousMarker}<reviewer>@<fingerprint>'. " +
                "Only a human may create an approval." +
                $" A reviewer is an alias: it opens with a letter, holds letters, digits, '.', '_', '-', ''' and " +
                $"single spaces, is at most {AssuranceVocabulary.MaxAliasLength} characters long, and is not a " +
                "placeholder such as TODO, NONE or NOT REVIEWED.");
        }

        IReadOnlySet<string> permitted = ReviewerNames(before);
        foreach (string name in ReviewerNames(after))
        {
            if (!permitted.Contains(name))
            {
                throw new AssuranceRefusalException(
                    $"The assurance generator tried to write reviewer '{name}' onto {where}, " +
                    $"whose human line reads '{before}'. Only a human may create an approval.");
            }
        }
    }

    /// <summary>
    /// The human line as a reader is shown it: the body verbatim, or a phrase
    /// saying there is none. Never the state machine's name for it, which would
    /// put a bare review word into generated prose with no count beside it.
    /// </summary>
    public static string Display(AssuranceAnnotation? annotation) => annotation switch
    {
        null => "no annotation",
        { HumanBody.Length: 0 } => "an empty line",
        _ => annotation.HumanBody,
    };

    /// <summary>
    /// Every human body in a text, at any indentation: the lines whose trimmed
    /// text opens with the human marker.
    /// </summary>
    public static IEnumerable<string> BodiesIn(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var lines = new AssuranceLines(text);
        for (int index = 0; index < lines.Count; index++)
        {
            string line = lines[index].Trim();
            if (line.StartsWith(AssuranceVocabulary.HumanMarker, StringComparison.Ordinal))
                yield return line[AssuranceVocabulary.HumanMarker.Length..].Trim();
        }
    }
}
