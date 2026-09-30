// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           5
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    High
// Criteria:         10/10
// Resource impact:  3/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Core.Shell;
using Broiler.Code.Workspaces.Storage;

namespace Broiler.Code.Linux;

/// <summary>
/// File dialogs through the desktop's own helper — zenity on GTK desktops,
/// kdialog on KDE.
///
/// There is no toolkit-neutral file chooser on Linux, and this head deliberately
/// does not take a GTK or Qt dependency to get one. Running the helper the
/// desktop already ships is what a toolkit-less application does: the dialog the
/// user sees is their desktop's real chooser, with their bookmarks and their
/// recent files, not an imitation drawn by us.
///
/// When neither helper is installed there is no dialog, and
/// <see cref="IsAvailable"/> is false so the head reports Open, Open Folder and
/// Save As as unavailable rather than opening nothing.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D8426B
// Broiler-Falsified-If: a grant from an open, save or folder request roots storage at a directory other than the chosen folder or the directory that contains the chosen file
// Broiler-Human:        PENDING
internal sealed class LinuxFileDialogs : IFileDialogService
{
    private readonly string? _helper;

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=BEF9D7
    // Broiler-Falsified-If: IsAvailable is true while neither zenity nor kdialog is present on PATH
    // Broiler-Human:        PENDING
    public LinuxFileDialogs() => _helper = FindHelper();

    /// <summary>Which helper was found, for the support claim.</summary>
    public string? Helper => _helper;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=1B90A3
    // Broiler-Human:        PENDING
    public bool IsAvailable => _helper is not null;

    /// <summary>
    /// Both helpers choose a directory as readily as a file — one flag on
    /// zenity, one verb on kdialog — so this is exactly the claim that a helper
    /// was found at all.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4F5825
    // Broiler-Human:        PENDING
    public bool CanRequestFolder => IsAvailable;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=33ED51
    // Broiler-Falsified-If: a chooser the user cancelled (helper exits non-zero) yields a grant instead of null
    // Broiler-Human:        PENDING
    public async ValueTask<FileGrant?> RequestOpenAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Grant(await RunAsync(request, DialogMode.Open, cancellationToken).ConfigureAwait(false));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=105FBA
    // Broiler-Falsified-If: the grant for a chosen save path lets storage write outside the directory that contains that path
    // Broiler-Human:        PENDING
    public async ValueTask<FileGrant?> RequestSaveAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Grant(await RunAsync(request, DialogMode.Save, cancellationToken).ConfigureAwait(false));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F9F393
    // Broiler-Falsified-If: the grant for a chosen folder is rooted at its parent directory rather than at the folder itself
    // Broiler-Human:        PENDING
    public async ValueTask<FileGrant?> RequestFolderAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return FolderGrant(
            await RunAsync(request, DialogMode.Folder, cancellationToken).ConfigureAwait(false));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=3C076F
    // Broiler-Falsified-If: a helper that writes more than a pipe buffer of warnings to standard error never exits, because standard error is redirected and never read, and the dialog never returns
    // Broiler-Human:        PENDING
    private async ValueTask<string?> RunAsync(
        FileDialogRequest request, DialogMode mode, CancellationToken cancellationToken)
    {
        if (_helper is null)
            return null;

        var start = new ProcessStartInfo(_helper)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        foreach (string argument in Arguments(_helper, request, mode))
            start.ArgumentList.Add(argument);

        using Process? process = Process.Start(start);
        if (process is null)
            return null;

        string output = await process.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

        // A non-zero exit is how both helpers report that the user cancelled,
        // which is a normal outcome and not an error to surface.
        if (process.ExitCode != 0)
            return null;

        string path = output.Trim();
        return path.Length == 0 ? null : path;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=EF8CCD
    // Broiler-Falsified-If: a suggested name beginning with a dash is passed to kdialog as a bare argument, where it is parsed as an option instead of a start path
    // Broiler-Human:        PENDING
    private static IEnumerable<string> Arguments(
        string helper, FileDialogRequest request, DialogMode mode)
    {
        bool isKde = helper.EndsWith("kdialog", StringComparison.Ordinal);
        if (isKde)
        {
            // kdialog takes a start path and one space-separated filter string.
            // The directory verb takes the start path and no filter at all: a
            // file filter applied to a folder chooser hides every folder.
            yield return mode switch
            {
                DialogMode.Save => "--getsavefilename",
                DialogMode.Folder => "--getexistingdirectory",
                _ => "--getopenfilename",
            };

            yield return request.SuggestedName is { Length: > 0 } suggested ? suggested : ".";
            if (mode != DialogMode.Folder)
                yield return KdialogFilter(request.Filters);

            if (request.Title is { Length: > 0 })
            {
                yield return "--title";
                yield return request.Title;
            }

            yield break;
        }

        yield return "--file-selection";
        if (mode == DialogMode.Folder)
            yield return "--directory";

        if (mode == DialogMode.Save)
        {
            yield return "--save";

            // The overwrite prompt is the dialog's job. Without this a save over
            // an existing file happens with no warning at all.
            yield return "--confirm-overwrite";
        }

        if (request.Title is { Length: > 0 })
            yield return "--title=" + request.Title;
        if (request.SuggestedName is { Length: > 0 } name)
            yield return "--filename=" + name;

        if (mode == DialogMode.Folder)
            yield break;

        foreach (FileDialogFilter filter in request.Filters)
            yield return "--file-filter=" + ZenityFilter(filter);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=059831
    // Broiler-Human:        PENDING
    private static string ZenityFilter(FileDialogFilter filter)
    {
        var patterns = new List<string>(filter.Extensions.Count);
        foreach (string extension in filter.Extensions)
            patterns.Add(extension == "*" ? "*" : "*." + extension);

        return filter.Label + " | " + string.Join(' ', patterns);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=705305
    // Broiler-Human:        PENDING
    private static string KdialogFilter(IReadOnlyList<FileDialogFilter> filters)
    {
        var groups = new List<string>(filters.Count);
        foreach (FileDialogFilter filter in filters)
        {
            var patterns = new List<string>(filter.Extensions.Count);
            foreach (string extension in filter.Extensions)
                patterns.Add(extension == "*" ? "*" : "*." + extension);

            groups.Add(string.Join(' ', patterns) + "|" + filter.Label);
        }

        return groups.Count == 0 ? "*|All files" : string.Join('\n', groups);
    }

    /// <summary>
    /// A directory the user picked. The directory itself is the grant, so
    /// storage is rooted at it and nothing was chosen inside it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1C003D
    // Broiler-Falsified-If: the storage for a chosen folder is rooted anywhere other than that folder
    // Broiler-Human:        PENDING
    private static FileGrant? FolderGrant(string? absolutePath)
    {
        if (absolutePath is null)
            return null;

        string full = Path.GetFullPath(absolutePath);
        return new FileGrant(new FileSystemWorkspaceStorage(full), string.Empty, full);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F14BCF
    // Broiler-Falsified-If: the storage for a chosen file is rooted anywhere other than that file's own directory, or the returned name carries a directory part
    // Broiler-Human:        PENDING
    private static FileGrant? Grant(string? absolutePath)
    {
        if (absolutePath is null)
            return null;

        string full = Path.GetFullPath(absolutePath);
        string? directory = Path.GetDirectoryName(full);
        if (directory is null)
            return null;

        // The chooser is the grant: storage scoped to the directory the user
        // picked and nothing wider.
        return new FileGrant(
            new FileSystemWorkspaceStorage(directory), Path.GetFileName(full), full);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=8CB137
    // Broiler-Falsified-If: a zenity file without execute permission earlier on PATH is chosen over an executable helper later on PATH, and the dialog then fails to start
    // Broiler-Human:        PENDING
    private static string? FindHelper()
    {
        if (!OperatingSystem.IsLinux())
            return null;

        // Looked up on PATH rather than assumed at a fixed location: a helper
        // that is not installed must read as absent, not as a broken dialog.
        string? path = Environment.GetEnvironmentVariable("PATH");
        if (path is null)
            return null;

        foreach (string name in new[] { "zenity", "kdialog" })
        {
            foreach (string directory in path.Split(Path.PathSeparator))
            {
                if (directory.Length == 0)
                    continue;

                string candidate = Path.Combine(directory, name);
                if (File.Exists(candidate))
                    return candidate;
            }
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CD7675
    // Broiler-Human:        PENDING
    private enum DialogMode
    {
        Open,
        Save,
        Folder,
    }
}
