// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           0
// Human-reviewed:   0/2
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  7/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Runtime.Versioning;
using Broiler.Code.Core.Hosting;

namespace Broiler.Code.Windows;

// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=A32934
// Broiler-Falsified-If: an argument beginning with a dash is opened as the workspace directory instead of being treated as an option
// Broiler-Human:        PENDING
[SupportedOSPlatform("windows7.0")]
internal static class Program
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=D0FC29
    // Broiler-Falsified-If: an argument beginning with a dash is opened as the workspace directory instead of being treated as an option
    // Broiler-Human:        PENDING
    [STAThread]
    private static int Main(string[] args)
    {
        // --services prints what the head provides and exits. It is how the
        // support claim is checked from a script or a test without opening a
        // window, and it prints substitutes and gaps as loudly as it prints
        // what works.
        if (Array.IndexOf(args, "--services") >= 0)
        {
            HostServiceReport report = CodeHost.DescribeServices();
            Console.WriteLine(report.Describe());
            return report.HasNoSubstitutes ? 0 : 1;
        }

        string? workspacePath = Array.Find(args, argument => !argument.StartsWith('-'));

        using var window = new CodeWindow();
        return CodeHost.Run(window, workspacePath);
    }
}
