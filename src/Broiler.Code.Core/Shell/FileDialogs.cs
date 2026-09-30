// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           7
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  2/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Workspaces.Storage;

namespace Broiler.Code.Core.Shell;

/// <summary>
/// One entry in a dialog's type list. Extensions are bare, without a dot or a
/// wildcard, so a host can render them in whatever form its platform wants.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C1B768
// Broiler-Human:        PENDING
public sealed record FileDialogFilter(string Label, IReadOnlyList<string> Extensions)
{
    public static FileDialogFilter Sources { get; } = new("C# source", ["cs"]);

    public static FileDialogFilter Solutions { get; } = new("Solutions", ["slnx", "sln"]);

    public static FileDialogFilter All { get; } = new("All files", ["*"]);
}

// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4FB4F7
// Broiler-Human:        PENDING
public sealed record FileDialogRequest
{
    public string Title { get; init; } = string.Empty;

    /// <summary>The name offered in a save dialog, ignored by an open dialog.</summary>
    public string? SuggestedName { get; init; }

    public IReadOnlyList<FileDialogFilter> Filters { get; init; } = [];
}

/// <summary>
/// A location the user chose, and the storage grant that choosing it created.
///
/// The dialog <em>is</em> the grant: a host hands back storage already scoped to
/// what the user picked, so this assembly never names a filesystem type and the
/// same shell works over a sandboxed provider that can only see what was
/// granted. It is also what lets a document live outside the workspace root
/// without the workspace having to widen its own.
/// </summary>
/// <param name="RelativePath">
/// What was chosen inside the grant, or empty when the grant itself is the
/// choice — which is what <see cref="IFileDialogService.RequestFolderAsync"/>
/// returns.
/// </param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D8055C
// Broiler-Human:        PENDING
public sealed record FileGrant(IWorkspaceStorage Storage, string RelativePath, string DisplayPath);

/// <summary>
/// Asks the user for a file. A host that has no way to ask supplies nothing and
/// the affected commands report themselves unavailable, rather than appearing
/// to work and doing nothing.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=BE9D67
// Broiler-Falsified-If: an implementation returns a grant whose Storage reaches above the chosen file's directory or above the folder the user picked
// Broiler-Human:        PENDING
public interface IFileDialogService
{
    /// <summary>
    /// Whether this host can ask for a directory as well as for a file.
    ///
    /// Separate from having dialogs at all, because on every platform it is a
    /// different call: comdlg32 picks files and shell32 picks folders, zenity
    /// needs an extra flag, and a sandboxed host may be granted one and not the
    /// other. It defaults to false so a host written before folders existed
    /// reports Open Folder unavailable rather than opening nothing.
    /// </summary>
    bool CanRequestFolder => false;

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=944E8F
    // Broiler-Falsified-If: an implementation returns a grant whose Storage reaches above the directory holding the file the user picked
    // Broiler-Human:        PENDING
    ValueTask<FileGrant?> RequestOpenAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=DCA869
    // Broiler-Falsified-If: an implementation returns a grant whose Storage reaches above the directory of the name the user chose
    // Broiler-Human:        PENDING
    ValueTask<FileGrant?> RequestSaveAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asks for a directory to open as a workspace.
    ///
    /// The directory the user picked <em>is</em> the grant, so the returned
    /// <see cref="FileGrant.RelativePath"/> is empty: the choice is the root
    /// itself rather than something inside it, and the storage handed back
    /// reaches that directory and nothing wider.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=EF6151
    // Broiler-Falsified-If: an implementation returns a grant whose Storage reaches above the directory the user picked
    // Broiler-Human:        PENDING
    ValueTask<FileGrant?> RequestFolderAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult<FileGrant?>(null);
}
