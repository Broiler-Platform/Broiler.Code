using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Broiler.Code.Review.Assurance;

/// <summary>Why a source rewrite was refused, or that it was applied.</summary>
public enum AssuranceEditOutcome
{
    Applied = 0,

    /// <summary>The declaration carries no annotation block, so there is no line to write.</summary>
    NotAnnotated,

    /// <summary>The unit is exempt. The owning component expects no human line to move on it.</summary>
    Exempt,

    /// <summary>The reviewer's name is missing, or would not survive the format.</summary>
    NoReviewer,

    /// <summary>The line already says this. Rewriting it would be a diff with no change in it.</summary>
    NothingToDo,
}

/// <summary>
/// The result of rewriting one annotation: the new file text, and a sentence
/// saying what happened to it.
/// </summary>
/// <param name="Outcome">Whether the rewrite was applied.</param>
/// <param name="Text">The file as it should now be. The original text when nothing was applied.</param>
/// <param name="Message">A sentence for the status line.</param>
/// <param name="HeaderUpdated">
/// Whether the generated file header was recounted as part of the same edit.
/// False is normal and not a failure — see <see cref="AssuranceDocument.BannerIsReproducible"/>.
/// </param>
public readonly record struct AssuranceEditResult(
    AssuranceEditOutcome Outcome,
    string Text,
    string Message,
    bool HeaderUpdated)
{
    public bool Succeeded => Outcome == AssuranceEditOutcome.Applied;
}

/// <summary>
/// One source file, read as the assurance system reads it: a generated header,
/// and a sequence of code units each carrying what a machine assessed and what a
/// human has said.
///
/// This is the model the review pane shows and the thing that produces the
/// rewrite. It exists in the review assembly, beside the per-file review record,
/// because it is the same kind of object: evidence about source, computed from
/// source, with no display and no language service of its own.
///
/// It reads at two levels of confidence and says which one it is at.
/// With an <see cref="IAssuranceUnitScanner"/> composed it knows every unit in
/// the file, which of them are exempt, and what each one's fingerprint is now;
/// without one it knows only the units somebody has already annotated, because
/// an annotation block is plain text and finding one needs no parser. The second
/// level is enough to show a reviewer what is recorded and to record their
/// decision — which is the whole of what a human writes — and it is not enough
/// to recount the generated header, so it does not.
/// </summary>
public sealed class AssuranceDocument
{
    private readonly AssuranceLines _lines;
    private readonly int _bannerLength;
    private readonly IAssuranceUnitScanner? _scanner;
    private readonly string _path;

    private AssuranceDocument(
        AssuranceLines lines,
        IReadOnlyList<AssuranceUnit> units,
        int bannerLength,
        IAssuranceUnitScanner? scanner,
        string path)
    {
        _lines = lines;
        _bannerLength = bannerLength;
        _scanner = scanner;
        _path = path;
        Units = units;
        HasUnitScanner = scanner is not null;
    }

    /// <summary>The file's units, in document order.</summary>
    public IReadOnlyList<AssuranceUnit> Units { get; }

    /// <summary>
    /// True when a language service supplied the units, so exemptions,
    /// fingerprints and therefore the header's counts are all known.
    /// </summary>
    public bool HasUnitScanner { get; }

    /// <summary>True when the file carries a generated assurance header.</summary>
    public bool HasBanner => _bannerLength > 0;

    /// <summary>True when the file carries anything this pane can talk about.</summary>
    public bool IsAnnotated => Units.Count > 0 || HasBanner;

    /// <summary>
    /// The counts the generated header reports, or null when this build cannot
    /// establish them.
    ///
    /// Null without a scanner, and it has to be: the header counts exempt units
    /// and units nobody has annotated, and neither is visible in the annotation
    /// text alone. Returning a number computed over the annotated units only
    /// would produce a plausible header that quietly understated the file.
    /// </summary>
    public AssuranceSummary? Summary { get; private set; }

    /// <summary>
    /// True when this build reproduces the header the file already carries,
    /// exactly.
    ///
    /// This is the licence to rewrite it. Recording a review moves two of the
    /// header's numbers, so leaving it alone makes the file disagree with its own
    /// annotations; but writing a header from counts that differ from the owning
    /// component's would be worse, because the result looks generated and is
    /// wrong. Reproducing what is there first is the only evidence available that
    /// this build counts the same way that component does — over this file, which
    /// is the file being changed.
    ///
    /// It fails closed. No scanner means no summary means no rewrite.
    /// </summary>
    public bool BannerIsReproducible { get; private set; }

    /// <summary>
    /// Reads <paramref name="text"/>.
    ///
    /// <paramref name="scanner"/> is optional and its absence is a normal state,
    /// not a degraded one to be warned about: most hosts this shell is meant to
    /// run on cannot carry a C# parser at all.
    /// </summary>
    public static AssuranceDocument Read(
        string text, IAssuranceUnitScanner? scanner = null, string path = "")
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(path);

        var lines = new AssuranceLines(text);
        int banner = AssuranceBanner.Length(lines);

        IReadOnlyList<AssuranceUnit> units = scanner is null
            ? FromAnnotations(lines)
            : FromScanner(lines, scanner, text, path);

        var document = new AssuranceDocument(lines, units, banner, scanner, path);
        document.Summarize();
        return document;
    }

    /// <summary>
    /// The unit <paramref name="line"/> falls in, or null.
    ///
    /// The innermost one, because a type's extent covers its members and a caret
    /// inside a method is a question about the method. A caret between two units
    /// — on a blank line, or in a doc comment — belongs to neither and is
    /// answered with null rather than with the nearest, which would put the pane
    /// on a declaration the reviewer is not looking at.
    /// </summary>
    public AssuranceUnit? UnitAt(int line)
    {
        AssuranceUnit? found = null;
        foreach (AssuranceUnit unit in Units)
        {
            if (!unit.Contains(line))
                continue;

            if (found is null || unit.StartLine >= found.StartLine)
                found = unit;
        }

        return found;
    }

    /// <summary>The unit with this qualified name, or null.</summary>
    public AssuranceUnit? UnitNamed(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        foreach (AssuranceUnit unit in Units)
        {
            if (string.Equals(unit.Name, name, StringComparison.Ordinal))
                return unit;
        }

        return null;
    }

    /// <summary>
    /// The body to write when <paramref name="reviewer"/> approves
    /// <paramref name="unit"/>.
    ///
    /// The reviewer's name and nothing else. The fingerprint is deliberately not
    /// written, and that is the safety property of this whole feature rather than
    /// a gap in it: the owning component's generator fills the fingerprint in,
    /// and a bare name is exactly the shape it expects from a person. An editor
    /// that wrote the fingerprint itself could turn a keystroke into a completed
    /// approval of code, which is the one thing the policy says no automatic step
    /// may do. This one structurally cannot.
    ///
    /// A different reviewer's line is replaced outright: their assessment was
    /// their reading, and their approval was of a version this person has not
    /// spoken about.
    ///
    /// A reviewer signing their own line again is treated by what that line
    /// already says. If the code has moved out from under it, the lapsed
    /// fingerprint is dropped so the generator seals the version that is here
    /// now — that is what re-reviewing means. Otherwise the line is returned
    /// unchanged, which the caller reports as nothing to do. Rebuilding it from
    /// the name alone would delete the very field that records which version was
    /// approved, turning a second signature into an act that withdraws the first.
    /// </summary>
    public static string ApprovalBody(AssuranceUnit unit, string reviewer)
    {
        ArgumentNullException.ThrowIfNull(unit);
        ArgumentNullException.ThrowIfNull(reviewer);

        AssuranceAnnotation annotation = unit.Annotation
            ?? throw new ArgumentException("The unit carries no annotation.", nameof(unit));

        string name = reviewer.Trim();
        if (!string.Equals(annotation.Reviewer, name, StringComparison.Ordinal))
            return name;

        if (unit.State != AssuranceUnitState.Stale)
            return annotation.HumanBody;

        var builder = new StringBuilder(name);
        foreach (string part in annotation.HumanAssessment)
            builder.Append("; ").Append(part);

        return builder.ToString();
    }

    /// <summary>
    /// Records <paramref name="reviewer"/> as having reviewed
    /// <paramref name="unit"/>, and recounts the header when it may.
    /// </summary>
    public AssuranceEditResult Approve(AssuranceUnit unit, string reviewer)
    {
        ArgumentNullException.ThrowIfNull(unit);

        // Exemption is answered before the missing annotation, because it is the
        // more useful of the two true answers: an exempt unit carries no
        // annotation precisely because it is exempt, and telling a reviewer the
        // block is missing invites them to add one the format does not want.
        if (unit.IsExempt)
        {
            return Refused(
                AssuranceEditOutcome.Exempt,
                $"{unit.DisplayName} is exempt ({unit.Exemption}); the assurance system expects no review on it.");
        }

        if (unit.Annotation is not { } annotation)
        {
            return Refused(
                AssuranceEditOutcome.NotAnnotated,
                $"{unit.DisplayName} carries no assurance annotation, so there is no line to sign.");
        }

        if (!AssuranceVocabulary.IsWritableReviewer(reviewer))
        {
            return Refused(
                AssuranceEditOutcome.NoReviewer,
                reviewer is null || reviewer.Trim().Length == 0
                    ? "Set a reviewer name before signing a unit — an approval with no name is not evidence."
                    : $"'{reviewer.Trim()}' cannot be written as a reviewer: a name may not contain " +
                      "';', '=' or '@', and has to have something visible in it.");
        }

        return Rewrite(unit, annotation, ApprovalBody(unit, reviewer), Approved(unit, reviewer));
    }

    /// <summary>
    /// Puts <paramref name="unit"/> back to pending.
    ///
    /// The way out of an approval recorded on the wrong declaration, and the only
    /// decision here that removes a claim rather than making one. It writes the
    /// reserved word and nothing else, so it cannot leave a half-formed line
    /// behind.
    ///
    /// Allowed on an exempt unit, which signing is not, and the asymmetry is the
    /// point: it is the same exemption the file-level Clear Review has. Signing
    /// makes a claim the format does not want on an exempt declaration;
    /// withdrawing takes one back, and a review recorded before the declaration
    /// became exempt has to have a way out.
    /// </summary>
    public AssuranceEditResult Withdraw(AssuranceUnit unit)
    {
        ArgumentNullException.ThrowIfNull(unit);

        if (unit.Annotation is not { } annotation)
        {
            return Refused(
                AssuranceEditOutcome.NotAnnotated,
                $"{unit.DisplayName} carries no assurance annotation.");
        }

        return Rewrite(
            unit,
            annotation,
            AssuranceVocabulary.Pending,
            $"{unit.DisplayName} set back to {AssuranceVocabulary.Pending}.");
    }

    private static string Approved(AssuranceUnit unit, string reviewer) =>
        $"{unit.DisplayName} signed by {reviewer.Trim()}; the assurance generator fills the fingerprint in.";

    private AssuranceEditResult Refused(AssuranceEditOutcome outcome, string message) =>
        new(outcome, _lines.Render(), message, false);

    private AssuranceEditResult Rewrite(
        AssuranceUnit unit, AssuranceAnnotation annotation, string body, string message)
    {
        string line = annotation.RenderHumanLine(body);
        if (string.Equals(_lines[annotation.HumanLine], line, StringComparison.Ordinal))
        {
            return Refused(
                AssuranceEditOutcome.NothingToDo,
                $"{unit.DisplayName} already records exactly that.");
        }

        _lines.Replace(annotation.HumanLine, line);
        bool header = RecountBanner();

        return new AssuranceEditResult(
            AssuranceEditOutcome.Applied,
            _lines.Render(),
            header ? message + " The file header was recounted." : message,
            header);
    }

    /// <summary>
    /// Rewrites the generated header from the file as it now is, when this build
    /// had already proved it counts the same way.
    ///
    /// The re-read is over the edited text rather than over the units in hand,
    /// for the same reason the owning component's generator re-scans before it
    /// writes: the header describes the file as it will be, and deriving it from
    /// the file as it was is how a generator stops being a fixed point.
    /// </summary>
    private bool RecountBanner()
    {
        if (!BannerIsReproducible || _bannerLength == 0)
            return false;

        // Only ever called with a scanner composed, because that is the only way
        // BannerIsReproducible becomes true.
        AssuranceDocument edited = Read(_lines.Render(), _scanner, _path);
        if (edited.Summary is not { } summary)
            return false;

        IReadOnlyList<string> banner = AssuranceBanner.Render(summary, _lines[0], _lines[1]);

        // Whether any of it actually moved. Recording a review often changes no
        // number at all — a name with no fingerprint yet is not a verified unit,
        // so the counts stand — and reporting "the header was recounted" when the
        // bytes are identical trains a reader to ignore the sentence.
        bool changed = false;
        for (int line = 0; line < banner.Count && line < _bannerLength; line++)
        {
            if (!string.Equals(banner[line], _lines[line], StringComparison.Ordinal))
                changed = true;
        }

        if (!changed)
            return false;

        // Replaced line by line rather than removed and re-inserted. The two are
        // the same length — reproducing the header is what earned the right to
        // write it — and replacing keeps each line's own terminator, where
        // inserting would give all sixteen the file's first one. A file whose
        // header is CRLF and whose body is LF would otherwise come back with
        // every header line rewritten, which is a whole-block diff to record one
        // reviewer's name.
        for (int line = 0; line < banner.Count; line++)
            _lines.Replace(line, banner[line]);

        return true;
    }

    private void Summarize()
    {
        if (!HasUnitScanner)
        {
            Summary = null;
            BannerIsReproducible = false;
            return;
        }

        int relevant = 0;
        int annotated = 0;
        int exempt = 0;
        int verified = 0;
        int unverified = 0;
        int criteria = 0;
        int criteriaRequired = 0;
        int? maxResources = null;
        var assessed = new List<AssuranceAnnotation>();

        foreach (AssuranceUnit unit in Units)
        {
            if (unit.Annotation is { HasCriterion: true })
                criteria++;

            if (unit.Annotation is { ExemptReason: null } stated &&
                stated.Field("Security") is "High" or "Critical")
            {
                criteriaRequired++;
            }

            if (unit.IsExempt)
            {
                exempt++;
                continue;
            }

            relevant++;

            if (AssuranceStateMachine.BlocksRelease(unit.State))
                unverified++;

            if (unit.State == AssuranceUnitState.Verified)
                verified++;

            if (unit.Annotation is not { ExemptReason: null } annotation)
                continue;

            annotated++;
            assessed.Add(annotation);

            if (annotation.Field("Resources") is { } score &&
                int.TryParse(score, NumberStyles.None, CultureInfo.InvariantCulture, out int resources))
            {
                maxResources = maxResources is { } current ? Math.Max(current, resources) : resources;
            }
        }

        var summary = new AssuranceSummary(
            relevant,
            annotated,
            exempt,
            verified,
            unverified,
            AssuranceBanner.Worst(assessed, "IP", AssuranceVocabulary.IpRiskValues),
            AssuranceBanner.Worst(assessed, "Security", AssuranceVocabulary.SecurityRiskValues),
            criteria,
            criteriaRequired,
            maxResources);

        Summary = summary;
        BannerIsReproducible = ReproducesBanner(summary);
    }

    private bool ReproducesBanner(AssuranceSummary summary)
    {
        if (_bannerLength < 2)
            return false;

        IReadOnlyList<string> rendered = AssuranceBanner.Render(summary, _lines[0], _lines[1]);
        if (rendered.Count != _bannerLength)
            return false;

        for (int line = 0; line < rendered.Count; line++)
        {
            if (!string.Equals(rendered[line], _lines[line], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Units as a language service found them, with the annotation above each
    /// one attached.
    /// </summary>
    private static IReadOnlyList<AssuranceUnit> FromScanner(
        AssuranceLines lines, IAssuranceUnitScanner scanner, string text, string path)
    {
        IReadOnlyList<AssuranceScannedUnit> scanned = scanner.Scan(text, path);
        var units = new List<AssuranceUnit>(scanned.Count);

        foreach (AssuranceScannedUnit unit in scanned)
        {
            AssuranceAnnotation? annotation = AnnotationAbove(lines, unit.DeclarationLine);
            units.Add(new AssuranceUnit
            {
                Name = unit.Name,
                DisplayName = unit.DisplayName,

                // The extent starts at the annotation, not at the declaration, so
                // that a caret parked on the reviewer's own line is inside the
                // unit it belongs to rather than in the one above it.
                StartLine = annotation?.FirstLine ?? unit.DeclarationLine,
                EndLine = Math.Max(unit.EndLine, unit.DeclarationLine),
                DeclarationLine = unit.DeclarationLine,
                IsExempt = unit.IsExempt || annotation?.ExemptReason is not null,
                Exemption = annotation?.ExemptReason is not null ? "DeclaredInSource" : unit.Exemption,
                Fingerprint = unit.Fingerprint,
                Annotation = annotation,
                State = AssuranceStateMachine.Resolve(annotation, unit.IsExempt, unit.Fingerprint),
            });
        }

        return units;
    }

    /// <summary>
    /// Units read from the annotation blocks alone.
    ///
    /// Only annotated declarations are found this way, which is the honest limit
    /// of reading comments without a parser: an unannotated declaration is
    /// indistinguishable from any other line of code. Every unit found is one a
    /// reviewer can act on, so the pane loses nothing it could have offered.
    ///
    /// A unit's extent runs to the line before the next annotation block, so a
    /// caret anywhere in a member's body reports that member. Where the previous
    /// member ends and the next begins is not knowable here, and stopping short
    /// would leave a reviewer clicking into their own code and being told they
    /// are nowhere.
    /// </summary>
    private static IReadOnlyList<AssuranceUnit> FromAnnotations(AssuranceLines lines)
    {
        var found = new List<AssuranceAnnotation>();
        for (int line = 0; line < lines.Count; line++)
        {
            if (AssuranceAnnotation.TryParse(lines, line, out AssuranceAnnotation? annotation) &&
                annotation is not null)
            {
                found.Add(annotation);
                line = annotation.LastLine;
            }
        }

        var units = new List<AssuranceUnit>(found.Count);
        for (int index = 0; index < found.Count; index++)
        {
            AssuranceAnnotation annotation = found[index];
            int declaration = Math.Min(annotation.LastLine + 1, lines.Count - 1);
            int end = index + 1 < found.Count ? found[index + 1].FirstLine - 1 : lines.Count - 1;
            string name = DeclarationName(lines, declaration);

            units.Add(new AssuranceUnit
            {
                Name = name,
                DisplayName = name,
                StartLine = annotation.FirstLine,
                EndLine = Math.Max(end, declaration),
                DeclarationLine = declaration,
                IsExempt = annotation.ExemptReason is not null,
                Exemption = annotation.ExemptReason is not null ? "DeclaredInSource" : "None",
                Fingerprint = null,
                Annotation = annotation,
                State = AssuranceStateMachine.Resolve(annotation, isExemptByPredicate: false, currentFingerprint: null),
            });
        }

        return units;
    }

    private static AssuranceAnnotation? AnnotationAbove(AssuranceLines lines, int declarationLine)
    {
        // Upwards from the declaration, over the doc comment, the block comments
        // the attributes and the preprocessor lines that can sit between the two.
        // The first line that is none of those ends the search: a block further
        // up belongs to whatever is above that line, not to this declaration.
        //
        // A directive line is skipped rather than treated as an end because it is
        // trivia to the parser this stands in for — a <c>#region</c> between an
        // annotation and its declaration would otherwise report an approved unit
        // as never assessed.
        //
        // The owning component reads the block out of the declaration's leading
        // trivia, which is the same set of lines by another route. Scanning is
        // what a build with no parser can do, and the two agree as long as this
        // skips everything trivia holds — which is why block comments are here.
        AssuranceAnnotation? found = null;

        for (int line = declarationLine - 1; line >= 0; line--)
        {
            string trimmed = lines[line].TrimStart();

            if (trimmed.StartsWith(AssuranceVocabulary.AiMarker, StringComparison.Ordinal))
            {
                // Kept rather than returned, and the scan continues. Two blocks
                // above one declaration is malformed either way, and that
                // component's own lookup takes the first in source order — so
                // this takes the topmost too, rather than reporting a different
                // one than the tool that will read the file next.
                if (AssuranceAnnotation.TryParse(lines, line, out AssuranceAnnotation? annotation))
                    found = annotation;

                continue;
            }

            if (trimmed.Length == 0 ||
                trimmed.StartsWith("//", StringComparison.Ordinal) ||
                trimmed.StartsWith("/*", StringComparison.Ordinal) ||
                trimmed.StartsWith('*') ||
                trimmed.StartsWith('#') ||
                trimmed.StartsWith('['))
            {
                continue;
            }

            break;
        }

        return found;
    }

    /// <summary>
    /// A name for the declaration under an annotation, worked out from the text.
    ///
    /// A stand-in for the qualified name a language service supplies, and it says
    /// so by being what the source line says. Guessing an identifier out of a
    /// declaration is where a reader's trust is cheapest to lose — a wrong name
    /// on a review pane is a review of something else — so the fallback is the
    /// declaration itself, shortened, which cannot be wrong about which line it
    /// came from.
    /// </summary>
    private static string DeclarationName(AssuranceLines lines, int declarationLine)
    {
        for (int line = declarationLine; line < lines.Count && line <= declarationLine + 3; line++)
        {
            string trimmed = lines[line].Trim();

            // Attributes and doc comments sit between the block and the
            // declaration; they name nothing.
            if (trimmed.Length == 0 || trimmed.StartsWith('[') || trimmed.StartsWith("//", StringComparison.Ordinal))
                continue;

            return Shorten(trimmed);
        }

        return string.Create(CultureInfo.InvariantCulture, $"line {declarationLine + 1}");
    }

    private static string Shorten(string declaration)
    {
        string text = declaration.TrimEnd('{', ' ', '\t');
        int arrow = text.IndexOf("=>", StringComparison.Ordinal);
        if (arrow > 0)
            text = text[..arrow];

        text = text.TrimEnd();
        return text.Length <= 72 ? text : text[..71] + "…";
    }
}
