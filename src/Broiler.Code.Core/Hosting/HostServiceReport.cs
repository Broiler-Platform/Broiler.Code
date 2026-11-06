// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// HAIRS Code Assurance
// --------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           3
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Low
// Criteria:         6/0
// Resource impact:  2/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Linq;

namespace Broiler.Code.Core.Hosting;

/// <summary>How a host provides one of the services the Code claim depends on.</summary>
// Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=06308F
// Hairs-Human:        PENDING
public enum HostServiceQuality
{
    /// <summary>Not provided. The feature is unavailable and says so.</summary>
    Unavailable = 0,

    /// <summary>
    /// Provided by a stand-in that behaves plausibly without doing the real
    /// thing — an in-memory clipboard, an inline dispatcher. Acceptable in a
    /// test, never in a support claim.
    /// </summary>
    Substitute,

    /// <summary>The real platform service.</summary>
    Native,
}

// Hairs-AI:           Origin=AI; IP=None; Security=None; Resources=1; Fingerprint=E09897
// Hairs-Human:        PENDING
public sealed record HostService(string Name, HostServiceQuality Quality, string Detail);

/// <summary>
/// What a desktop head actually provides, as a value rather than a comment.
///
/// The roadmap forbids carrying Writer's immediate dispatcher, in-memory
/// clipboard fallback, or legacy input adapter into the Code support claim. A
/// prose rule is not enforceable, so each head declares what it has and a test
/// asserts the claim. A substitute is allowed to exist — it just cannot be
/// reported as support.
/// </summary>
// Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=132A2A
// Hairs-Falsified-If: a report containing a Substitute service yields that service from Supported
// Hairs-Human:        PENDING
public sealed record HostServiceReport(string HostName, IReadOnlyList<HostService> Services)
{
    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=82721A
    // Hairs-Human:        PENDING
    public const string Dispatcher = "ui-thread dispatcher";
    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=14EBA8
    // Hairs-Human:        PENDING
    public const string InputRouting = "input routing";
    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0C0632
    // Hairs-Human:        PENDING
    public const string Clipboard = "clipboard";
    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C7C4FD
    // Hairs-Human:        PENDING
    public const string TextInput = "text input and IME";

    /// <summary>
    /// Asking the user for a file. A head without it cannot run Open or Save
    /// As, so it is part of the claim rather than an implementation detail.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A1E25B
    // Hairs-Human:        PENDING
    public const string FileDialogs = "file dialogs";

    /// <summary>
    /// The services that are the real platform ones. Only these may appear in a
    /// support statement.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=E18881
    // Hairs-Falsified-If: a Substitute service is yielded by Supported
    // Hairs-Human:        PENDING
    public IEnumerable<HostService> Supported =>
        Services.Where(service => service.Quality == HostServiceQuality.Native);

    /// <summary>
    /// Anything not native. A head with entries here is not making a full
    /// support claim, and the reasons are the text the user is shown.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=94AE9D
    // Hairs-Falsified-If: a Native service is yielded by NotSupported
    // Hairs-Human:        PENDING
    public IEnumerable<HostService> NotSupported =>
        Services.Where(service => service.Quality != HostServiceQuality.Native);

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=EE0E6D
    // Hairs-Falsified-If: a name absent from Services returns a quality other than Unavailable
    // Hairs-Human:        PENDING
    public HostServiceQuality QualityOf(string name) =>
        Services.FirstOrDefault(service =>
            string.Equals(service.Name, name, StringComparison.Ordinal))?.Quality
        ?? HostServiceQuality.Unavailable;

    /// <summary>
    /// True when nothing is a substitute. Unavailable is honest; a substitute
    /// reported as support is not, which is why they are distinguished.
    /// </summary>
    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=D2DFA3
    // Hairs-Falsified-If: a report with one Substitute service among Native ones returns true
    // Hairs-Human:        PENDING
    public bool HasNoSubstitutes =>
        Services.All(service => service.Quality != HostServiceQuality.Substitute);

    // Hairs-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=DC9188
    // Hairs-Falsified-If: a Substitute service is described without the word SUBSTITUTE
    // Hairs-Human:        PENDING
    public string Describe()
    {
        IEnumerable<string> lines = Services.Select(service => service.Quality switch
        {
            HostServiceQuality.Native => $"  {service.Name}: native — {service.Detail}",
            HostServiceQuality.Substitute => $"  {service.Name}: SUBSTITUTE — {service.Detail}",
            _ => $"  {service.Name}: unavailable — {service.Detail}",
        });

        return $"{HostName}\n{string.Join('\n', lines)}";
    }
}
