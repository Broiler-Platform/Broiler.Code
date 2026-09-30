// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           32
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    Critical
// Criteria:         17/17
// Resource impact:  3/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Core.Shell;
using Broiler.Code.Workspaces.Storage;

namespace Broiler.Code.Windows;

/// <summary>
/// The common file dialogs, over comdlg32.
///
/// The dialog is where the grant comes from: whatever the user picks, this
/// returns storage rooted at that file's own directory and nothing wider. A
/// document opened or saved this way therefore reaches exactly one directory,
/// and the workspace never has to widen its root to accommodate it.
///
/// comdlg32 rather than IFileDialog: it needs no COM apartment setup beyond the
/// STA the head already declares, and this head asks for one file at a time.
/// comdlg32 has no folder chooser, so that one call goes to shell32's
/// SHBrowseForFolder — still a plain export rather than a COM interface, which
/// is the same trade for the same reason.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=10DF41
// Broiler-Falsified-If: comdlg32 or shell32 is told a buffer holds more characters than it does, so the chosen path is written past the end of the buffer
// Broiler-Human:        PENDING
[SupportedOSPlatform("windows7.0")]
internal sealed partial class WindowsFileDialogs(IntPtr owner) : IFileDialogService
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=0; Fingerprint=0D563F
    // Broiler-Falsified-If: the file and path buffers sized from this value hold fewer characters than the length reported with them to comdlg32 and shell32
    // Broiler-Human:        PENDING
    private const int MaxPath = 32768;

    public bool CanRequestFolder => true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=CCDE94
    // Broiler-Falsified-If: a name typed into the open dialog for a file that does not exist is returned as a grant
    // Broiler-Human:        PENDING
    public ValueTask<FileGrant?> RequestOpenAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        // FILE_MUST_EXIST | PATH_MUST_EXIST | HIDEREADONLY | EXPLORER
        return ValueTask.FromResult(Show(request, save: false, 0x00001000 | 0x00000800 | 0x00000004 | 0x00080000));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=25872F
    // Broiler-Falsified-If: choosing an existing file in the save dialog returns a grant without the dialog asking whether to overwrite it
    // Broiler-Human:        PENDING
    public ValueTask<FileGrant?> RequestSaveAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        // OVERWRITEPROMPT | PATH_MUST_EXIST | HIDEREADONLY | EXPLORER. The
        // overwrite prompt is the dialog's, so the save path below does not
        // need an expected-revision check the user has already answered.
        return ValueTask.FromResult(Show(request, save: true, 0x00000002 | 0x00000800 | 0x00000004 | 0x00080000));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=1E281B
    // Broiler-Falsified-If: the grant for a chosen folder is rooted at its parent directory rather than at the folder itself
    // Broiler-Human:        PENDING
    public ValueTask<FileGrant?> RequestFolderAsync(
        FileDialogRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(Browse(request.Title));
    }

    /// <summary>
    /// The folder chooser.
    ///
    /// The modern style needs OLE on the calling thread, and [STAThread] gives
    /// the CLR's CoInitializeEx and not OleInitialize, so this asks for OLE
    /// itself and falls back to the classic dialog if the apartment refuses.
    /// Balanced, because an unmatched OleInitialize leaves OLE up on a thread
    /// that is going to keep running the window.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=1C93F2
    // Broiler-Falsified-If: a buffer handed to shell32 holds fewer characters than shell32 is told or assumes it holds, so the display name or the chosen path is written past its end
    // Broiler-Human:        PENDING
    private FileGrant? Browse(string title)
    {
        const int SOk = 0;
        const int SFalse = 1;

        int initialized = OleInitialize(IntPtr.Zero);
        bool hasOle = initialized is SOk or SFalse;

        try
        {
            // BIF_RETURNONLYFSDIRS | BIF_EDITBOX, plus BIF_NEWDIALOGSTYLE when
            // OLE is up. Only filesystem directories: a virtual folder has no
            // path to grant storage over.
            uint flags = 0x0001 | 0x0010;
            if (hasOle)
                flags |= 0x0040;

            IntPtr list;
            char[] display = new char[260];
            unsafe
            {
                fixed (char* name = display)
                fixed (char* caption = title + '\0')
                {
                    var browse = new BrowseInfo
                    {
                        Owner = owner,
                        DisplayName = (IntPtr)name,
                        Title = (IntPtr)caption,
                        Flags = flags,
                    };

                    list = SHBrowseForFolder(ref browse);
                }
            }

            if (list == IntPtr.Zero)
                return null;

            try
            {
                char[] buffer = new char[MaxPath];
                unsafe
                {
                    fixed (char* path = buffer)
                    {
                        // The Ex form takes a length. The original assumes a
                        // MAX_PATH buffer and would truncate a deeper path into
                        // one that names a different directory.
                        if (!SHGetPathFromIDListEx(list, path, MaxPath, 0))
                            return null;
                    }
                }

                int terminator = Array.IndexOf(buffer, '\0');
                string full = new(buffer, 0, terminator < 0 ? buffer.Length : terminator);
                if (full.Length == 0)
                    return null;

                // The directory itself is the grant, so there is nothing chosen
                // inside it and the relative path is empty.
                full = Path.GetFullPath(full);
                return new FileGrant(new FileSystemWorkspaceStorage(full), string.Empty, full);
            }
            finally
            {
                CoTaskMemFree(list);
            }
        }
        finally
        {
            if (hasOle)
                OleUninitialize();
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=11CBC4
    // Broiler-Falsified-If: a suggested name of 32768 characters or more leaves the file buffer without a terminating NUL when comdlg32 reads it
    // Broiler-Human:        PENDING
    private FileGrant? Show(FileDialogRequest request, bool save, int flags)
    {
        char[] buffer = new char[MaxPath];
        if (request.SuggestedName is { Length: > 0 } suggested)
        {
            int length = Math.Min(suggested.Length, MaxPath - 1);
            suggested.AsSpan(0, length).CopyTo(buffer);
        }

        unsafe
        {
            fixed (char* file = buffer)
            fixed (char* filter = BuildFilter(request.Filters))
            fixed (char* title = request.Title + '\0')
            {
                var name = new OpenFileName
                {
                    StructureSize = sizeof(OpenFileName),
                    Owner = owner,
                    Filter = (IntPtr)filter,
                    FilterIndex = 1,
                    File = (IntPtr)file,
                    MaxFile = MaxPath,
                    Title = (IntPtr)title,
                    Flags = flags,
                };

                bool chosen = save ? GetSaveFileName(ref name) : GetOpenFileName(ref name);
                if (!chosen)
                    return null;
            }
        }

        int terminator = Array.IndexOf(buffer, '\0');
        string full = new(buffer, 0, terminator < 0 ? buffer.Length : terminator);
        if (full.Length == 0)
            return null;

        full = Path.GetFullPath(full);
        string? directory = Path.GetDirectoryName(full);
        if (directory is null)
            return null;

        return new FileGrant(
            new FileSystemWorkspaceStorage(directory), Path.GetFileName(full), full);
    }

    /// <summary>
    /// comdlg32's filter format: label, NUL, pattern, NUL, … and a final NUL.
    /// An empty list still needs one entry, or the dialog shows no files at all.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=2BE11D
    // Broiler-Falsified-If: the returned filter does not end in two NUL characters, so comdlg32 reads past the end of the array
    // Broiler-Human:        PENDING
    private static char[] BuildFilter(IReadOnlyList<FileDialogFilter> filters)
    {
        var text = new StringBuilder();
        if (filters.Count == 0)
            filters = [FileDialogFilter.All];

        foreach (FileDialogFilter filter in filters)
        {
            var patterns = new List<string>(filter.Extensions.Count);
            foreach (string extension in filter.Extensions)
                patterns.Add(extension == "*" ? "*.*" : $"*.{extension}");

            string joined = string.Join(";", patterns);
            text.Append(filter.Label).Append(" (").Append(joined).Append(')').Append('\0');
            text.Append(joined).Append('\0');
        }

        text.Append('\0');
        return [.. text.ToString()];
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=025C85
    // Broiler-Falsified-If: the struct size differs from the native OPENFILENAMEW size on the running architecture (152 bytes on x64, 88 on x86), so comdlg32 reads the file buffer and its length from the wrong offsets
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct OpenFileName
    {
        public int StructureSize;
        public IntPtr Owner;
        public IntPtr Instance;
        public IntPtr Filter;
        public IntPtr CustomFilter;
        public int MaxCustomFilter;
        public int FilterIndex;
        public IntPtr File;
        public int MaxFile;
        public IntPtr FileTitle;
        public int MaxFileTitle;
        public IntPtr InitialDirectory;
        public IntPtr Title;
        public int Flags;
        public short FileOffset;
        public short FileExtension;
        public IntPtr DefaultExtension;
        public IntPtr CustomData;
        public IntPtr Hook;
        public IntPtr TemplateName;
        public IntPtr ReservedPointer;
        public int ReservedInt;
        public int FlagsEx;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=86B63E
    // Broiler-Falsified-If: a field is out of BROWSEINFOW order or width, so shell32 reads the display-name buffer pointer or the flags from the wrong offset
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct BrowseInfo
    {
        public IntPtr Owner;
        public IntPtr Root;
        public IntPtr DisplayName;
        public IntPtr Title;
        public uint Flags;
        public IntPtr Callback;
        public IntPtr Parameter;
        public int Image;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=C657D2
    // Broiler-Falsified-If: the import binds to the ANSI SHBrowseForFolderA, so the UTF-16 title and display-name buffers are read and written as single-byte text
    // Broiler-Human:        PENDING
    [LibraryImport("shell32.dll", EntryPoint = "SHBrowseForFolderW")]
    private static partial IntPtr SHBrowseForFolder(ref BrowseInfo browse);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=7C322F
    // Broiler-Falsified-If: the length argument reaches shell32 as a value other than the buffer's character count, so the path is written past the end of the buffer
    // Broiler-Human:        PENDING
    [LibraryImport("shell32.dll", EntryPoint = "SHGetPathFromIDListEx")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static unsafe partial bool SHGetPathFromIDListEx(
        IntPtr list, char* path, int length, int options);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=9CEDB0
    // Broiler-Falsified-If: the import binds to an entry other than ole32 CoTaskMemFree, so the item list is released by an allocator other than the one that created it
    // Broiler-Human:        PENDING
    [LibraryImport("ole32.dll")]
    private static partial void CoTaskMemFree(IntPtr memory);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=F71911
    // Broiler-Falsified-If: the HRESULT is declared as a type other than the 32-bit int ole32 returns, so S_FALSE from an already-initialised thread reads as a failure
    // Broiler-Human:        PENDING
    [LibraryImport("ole32.dll")]
    private static partial int OleInitialize(IntPtr reserved);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=BAD4CD
    // Broiler-Falsified-If: the import binds to CoUninitialize instead of OleUninitialize, leaving OLE initialised on the window thread after every folder dialog
    // Broiler-Human:        PENDING
    [LibraryImport("ole32.dll")]
    private static partial void OleUninitialize();

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=19E8B8
    // Broiler-Falsified-If: the BOOL result is marshalled as a one-byte bool, so a cancelled open dialog reads as a chosen file
    // Broiler-Human:        PENDING
    [LibraryImport("comdlg32.dll", EntryPoint = "GetOpenFileNameW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetOpenFileName(ref OpenFileName name);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=4B973B
    // Broiler-Falsified-If: the import binds to GetOpenFileNameW instead of GetSaveFileNameW, so a save offers only existing files and never prompts to overwrite
    // Broiler-Human:        PENDING
    [LibraryImport("comdlg32.dll", EntryPoint = "GetSaveFileNameW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetSaveFileName(ref OpenFileName name);
}
