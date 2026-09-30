// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           10
// Human-reviewed:   0/19
// IP risk:          Low
// Security risk:    Critical
// Criteria:         18/16
// Resource impact:  7/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Broiler.Graphics;
using Broiler.Graphics.Geometry;
using Broiler.UI;

namespace Broiler.Code.Windows;

/// <summary>
/// Native IME composition, over IMM32.
///
/// The editor cannot compose text correctly without this. An IME sends
/// composition state through window messages, and a host that only handles
/// WM_CHAR sees the committed result with no composition, no candidate
/// underline, and a candidate window parked at the top-left of the screen
/// instead of at the caret.
///
/// The caret rectangle matters more than it looks: <c>ImmSetCompositionWindow</c>
/// is what puts the candidate list under the text being typed. Without it a
/// Japanese or Chinese user reads their candidates in one corner while typing in
/// another.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=7; Fingerprint=AB5D86
// Broiler-Falsified-If: a composition string is read past the end of the unmanaged buffer allocated for it
// Broiler-Human:        PENDING
[SupportedOSPlatform("windows5.0")]
internal sealed partial class WindowsTextInputService(IntPtr window) : IDisposable
{
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=F569F9
    // Broiler-Falsified-If: the value differs from 0x010D, WM_IME_STARTCOMPOSITION, so the start of a composition is never reported and its characters also arrive as plain text input
    // Broiler-Human:        PENDING
    private const int WmImeStartComposition = 0x010D;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=3FF446
    // Broiler-Falsified-If: the value differs from 0x010F, WM_IME_COMPOSITION, so neither the composition nor the result string is ever read
    // Broiler-Human:        PENDING
    private const int WmImeComposition = 0x010F;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=271737
    // Broiler-Falsified-If: the value differs from 0x010E, WM_IME_ENDCOMPOSITION, so a cancelled composition leaves the window suppressing later typed characters
    // Broiler-Human:        PENDING
    private const int WmImeEndComposition = 0x010E;

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=896D93
    // Broiler-Falsified-If: the value differs from 0x0008, GCS_COMPSTR, so the in-progress composition is read from another composition attribute
    // Broiler-Human:        PENDING
    private const int GcsCompStr = 0x0008;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=500793
    // Broiler-Falsified-If: the value differs from 0x0800, GCS_RESULTSTR, so a committed string is not recognised and the commit is reported as a cancellation
    // Broiler-Human:        PENDING
    private const int GcsResultStr = 0x0800;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=387ADE
    // Broiler-Falsified-If: the value differs from 0x0002, CFS_POINT, so the IME ignores the current position and leaves the candidate window away from the caret
    // Broiler-Human:        PENDING
    private const int CfsPoint = 0x0002;

    private bool _disposed;

    /// <summary>Composition text changed. Null text means the composition ended.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=834B45
    // Broiler-Human:        PENDING
    public event Action<string?, bool>? CompositionChanged;

    /// <summary>
    /// Handles an IME window message. Returns true when the message was
    /// consumed, so the caller does not also let the default handler turn it
    /// into a duplicate WM_CHAR.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=77CB51
    // Broiler-Falsified-If: a WM_IME_COMPOSITION carrying both the result and the composition flags reports the in-progress string instead of the committed one
    // Broiler-Human:        PENDING
    public bool TryHandleMessage(uint message, IntPtr wParam, IntPtr lParam)
    {
        switch (message)
        {
            case WmImeStartComposition:
                CompositionChanged?.Invoke(string.Empty, false);
                return true;

            case WmImeComposition:
            {
                long flags = lParam.ToInt64();

                // The committed result arrives on the same message as the
                // in-progress composition, and must be handled first: the
                // composition string is empty by then, so reading it first
                // would report the commit as a cancellation.
                if ((flags & GcsResultStr) != 0 && TryRead(GcsResultStr, out string committed))
                {
                    CompositionChanged?.Invoke(committed, true);
                    return true;
                }

                if ((flags & GcsCompStr) != 0 && TryRead(GcsCompStr, out string composing))
                {
                    CompositionChanged?.Invoke(composing, false);
                    return true;
                }

                return false;
            }

            case WmImeEndComposition:
                CompositionChanged?.Invoke(null, false);
                return true;

            default:
                return false;
        }
    }

    /// <summary>Places the candidate window at the caret, in client pixels.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4E0935
    // Broiler-Falsified-If: the input context taken by ImmGetContext is not released when ImmSetCompositionWindow fails or throws
    // Broiler-Human:        PENDING
    public void SetCaretRectangle(BRect caret)
    {
        IntPtr context = ImmGetContext(window);
        if (context == IntPtr.Zero)
            return;

        try
        {
            var form = new CompositionForm
            {
                Style = CfsPoint,
                CurrentPosition = new Point((int)caret.Left, (int)caret.Top),
                Area = new Rect((int)caret.Left, (int)caret.Top, (int)caret.Right, (int)caret.Bottom),
            };
            ImmSetCompositionWindow(context, ref form);
        }
        finally
        {
            ImmReleaseContext(window, context);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3A8E0A
    // Broiler-Falsified-If: a composition message handled after Dispose still reads the composition string from IMM32
    // Broiler-Human:        PENDING
    public void Dispose() => _disposed = true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=7; Fingerprint=7BAD58
    // Broiler-Falsified-If: when the second ImmGetCompositionStringW call returns fewer bytes than the first, the text returned includes the uninitialised tail of the buffer
    // Broiler-Human:        PENDING
    private bool TryRead(int index, out string text)
    {
        text = string.Empty;
        if (_disposed)
            return false;

        IntPtr context = ImmGetContext(window);
        if (context == IntPtr.Zero)
            return false;

        try
        {
            // Called twice by design: once for the byte length, once for the
            // data. The length is in bytes and the buffer is UTF-16.
            int bytes = ImmGetCompositionStringW(context, index, IntPtr.Zero, 0);
            if (bytes <= 0)
                return false;

            IntPtr buffer = Marshal.AllocHGlobal(bytes);
            try
            {
                if (ImmGetCompositionStringW(context, index, buffer, bytes) <= 0)
                    return false;
                text = Marshal.PtrToStringUni(buffer, bytes / sizeof(char));
                return true;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        finally
        {
            ImmReleaseContext(window, context);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=825C58
    // Broiler-Falsified-If: a field is wider than 32 bits, so the composition form no longer matches its native 28-byte layout and the IME reads the caret position from the wrong offset
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential)]
    private struct Point(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=96B7E9
    // Broiler-Falsified-If: the fields are not four 32-bit values in left, top, right, bottom order, so the IME reads the caret area from the wrong offsets
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential)]
    private struct Rect(int left, int top, int right, int bottom)
    {
        public int Left = left;
        public int Top = top;
        public int Right = right;
        public int Bottom = bottom;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=7817E6
    // Broiler-Falsified-If: the struct size differs from the native 28-byte COMPOSITIONFORM or its fields are out of style, position, area order, so the IME reads past it or from the wrong offsets
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential)]
    private struct CompositionForm
    {
        public int Style;
        public Point CurrentPosition;
        public Rect Area;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=BF4351
    // Broiler-Falsified-If: the window parameter or the returned input-context handle is declared narrower than IntPtr, so a 64-bit handle is truncated
    // Broiler-Human:        PENDING
    [LibraryImport("imm32.dll")]
    private static partial IntPtr ImmGetContext(IntPtr window);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=D89C7D
    // Broiler-Falsified-If: the import binds to an entry other than ImmReleaseContext, so every composition read leaks the input context it took
    // Broiler-Human:        PENDING
    [LibraryImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ImmReleaseContext(IntPtr window, IntPtr context);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AF6C17
    // Broiler-Falsified-If: the import binds to the ANSI ImmGetCompositionStringA, so the byte count and the text are not UTF-16 and the composition is decoded wrongly
    // Broiler-Human:        PENDING
    [LibraryImport("imm32.dll", EntryPoint = "ImmGetCompositionStringW")]
    private static partial int ImmGetCompositionStringW(
        IntPtr context, int index, IntPtr buffer, int bufferLength);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=3A7217
    // Broiler-Falsified-If: the composition form is passed by value instead of by pointer, so the IME reads the form from an arbitrary address
    // Broiler-Human:        PENDING
    [LibraryImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ImmSetCompositionWindow(IntPtr context, ref CompositionForm form);
}
