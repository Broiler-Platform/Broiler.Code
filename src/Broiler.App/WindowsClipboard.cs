// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           0
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    Critical
// Criteria:         15/15
// Resource impact:  7/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Broiler.UI;

namespace Broiler.App;

/// <summary>
/// The Win32 clipboard, shared by the Browser, Writer and Code heads.
///
/// There is deliberately no in-memory fallback. A private string standing in
/// for the clipboard makes copy and paste appear to work while silently not
/// interoperating with anything else on the machine — a user copies from the
/// editor, pastes into a browser, and gets their previous clipboard contents.
/// The Browser and Writer hosts did exactly that until they were wired to this;
/// it reports failure instead, and the caller shows the command as unavailable.
///
/// The bindings are <c>DllImport</c> rather than <c>LibraryImport</c> on
/// purpose: this file is compiled into the Browser, Writer and Code heads
/// alike, and the generated marshalling stubs would require
/// <c>AllowUnsafeBlocks</c> in every one of them. The two application heads use
/// <c>DllImport</c> for their own interop for the same reason.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=7; Fingerprint=F39BF4
// Broiler-Falsified-If: a CF_UNICODETEXT block with no terminating NUL inside its allocation is read past the end of the block
// Broiler-Human:        PENDING
[SupportedOSPlatform("windows5.0")]
internal sealed class WindowsClipboard(IntPtr ownerWindow) : IUiClipboardHost
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=0; Fingerprint=82BB56
    // Broiler-Falsified-If: the value differs from 13, CF_UNICODETEXT, so a block in another clipboard format is read as NUL-terminated UTF-16
    // Broiler-Human:        PENDING
    private const uint CfUnicodeText = 13;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=6C1EE9
    // Broiler-Falsified-If: the value differs from 0x0002, GMEM_MOVEABLE, so the block handed to SetClipboardData is fixed memory the clipboard does not accept
    // Broiler-Human:        PENDING
    private const uint GmemMoveable = 0x0002;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=7; Fingerprint=23A8D0
    // Broiler-Falsified-If: a CF_UNICODETEXT block with no terminating NUL inside its GlobalSize is read past the end of the block
    // Broiler-Human:        PENDING
    public bool TryGetText(out string text)
    {
        text = string.Empty;
        if (!IsClipboardFormatAvailable(CfUnicodeText))
            return false;

        // The clipboard is a shared, single-owner resource: another process can
        // hold it, so opening is allowed to fail and the caller is told rather
        // than being handed something stale.
        if (!OpenClipboard(ownerWindow))
            return false;

        try
        {
            IntPtr handle = GetClipboardData(CfUnicodeText);
            if (handle == IntPtr.Zero)
                return false;

            IntPtr pointer = GlobalLock(handle);
            if (pointer == IntPtr.Zero)
                return false;

            try
            {
                text = Marshal.PtrToStringUni(pointer) ?? string.Empty;
                return text.Length > 0;
            }
            finally
            {
                GlobalUnlock(handle);
            }
        }
        finally
        {
            CloseClipboard();
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=33578F
    // Broiler-Falsified-If: the terminating NUL is written outside the (text.Length + 1) * 2 bytes allocated for the block
    // Broiler-Human:        PENDING
    public void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (!OpenClipboard(ownerWindow))
            return;

        try
        {
            EmptyClipboard();

            // The clipboard takes ownership of the moveable block on success,
            // so it must not be freed here — and must be freed if
            // SetClipboardData fails, or the process leaks it on every copy.
            int bytes = (text.Length + 1) * sizeof(char);
            IntPtr block = GlobalAlloc(GmemMoveable, (UIntPtr)bytes);
            if (block == IntPtr.Zero)
                return;

            IntPtr pointer = GlobalLock(block);
            if (pointer == IntPtr.Zero)
            {
                GlobalFree(block);
                return;
            }

            try
            {
                Marshal.Copy(text.ToCharArray(), 0, pointer, text.Length);
                Marshal.WriteInt16(pointer, text.Length * sizeof(char), 0);
            }
            finally
            {
                GlobalUnlock(block);
            }

            if (SetClipboardData(CfUnicodeText, block) == IntPtr.Zero)
                GlobalFree(block);
        }
        finally
        {
            CloseClipboard();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=0B1567
    // Broiler-Falsified-If: the BOOL result is marshalled as a one-byte bool, so a failed open reads as success and the clipboard is used without being held
    // Broiler-Human:        PENDING
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenClipboard(IntPtr owner);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=819D6E
    // Broiler-Falsified-If: the import binds to an entry other than CloseClipboard in user32, so the clipboard stays open and blocks every other application's copy
    // Broiler-Human:        PENDING
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseClipboard();

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=82E0CE
    // Broiler-Falsified-If: the import binds to an entry other than EmptyClipboard in user32, so SetClipboardData runs without this window having taken clipboard ownership
    // Broiler-Human:        PENDING
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EmptyClipboard();

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AAAC79
    // Broiler-Falsified-If: the format parameter is declared as a type other than the 32-bit UINT the function takes, so availability is asked for a different format
    // Broiler-Human:        PENDING
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsClipboardFormatAvailable(uint format);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=317039
    // Broiler-Falsified-If: the HANDLE result is declared narrower than IntPtr, so a 64-bit handle is truncated before GlobalLock
    // Broiler-Human:        PENDING
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetClipboardData(uint format);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=BDB3DF
    // Broiler-Falsified-If: the HANDLE result is declared narrower than IntPtr, so a successful call can read as zero and the caller frees a block the clipboard now owns
    // Broiler-Human:        PENDING
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetClipboardData(uint format, IntPtr data);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=B864A6
    // Broiler-Falsified-If: the byte count is declared narrower than SIZE_T, so a large request allocates a smaller block than the caller then writes
    // Broiler-Human:        PENDING
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalAlloc(uint flags, UIntPtr bytes);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=80E1CC
    // Broiler-Falsified-If: the HGLOBAL parameter is declared narrower than IntPtr, so a different block is released
    // Broiler-Human:        PENDING
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalFree(IntPtr handle);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=D52183
    // Broiler-Falsified-If: the returned pointer is declared narrower than IntPtr, so a 64-bit block address is truncated before text is read or written through it
    // Broiler-Human:        PENDING
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalLock(IntPtr handle);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=BDEB39
    // Broiler-Falsified-If: the import binds to an entry other than GlobalUnlock in kernel32, so every paste and copy leaves the block's lock count raised
    // Broiler-Human:        PENDING
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalUnlock(IntPtr handle);
}
