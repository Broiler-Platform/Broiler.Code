using Broiler.Code.Review.Assurance;

namespace Broiler.Code.Language.CSharp.Tests;

/// <summary>
/// A real annotated source file, kept verbatim so that several tests can assert
/// against the same evidence.
///
/// It is <c>src/Broiler.VM.Abstractions/VmArtifactDescriptor.cs</c> as committed
/// in the component that owns the Broiler Code Assurance format. Its generated
/// header, its four annotations and the fingerprints inside them were all
/// written by that component's own generator, which is what makes them evidence
/// rather than expectations this repository invented.
/// </summary>
internal static class AssuranceFixture
{
    internal const string Descriptor = """
// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           9
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The caller-supplied description of an artifact.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=06FA02
// Broiler-Human:        PENDING
public readonly struct VmArtifactDescriptor : System.IEquatable<VmArtifactDescriptor>
{
    /// <summary>Creates an artifact descriptor.</summary>
    public VmArtifactDescriptor(
        VmProfileId profileId,
        uint formatVersion,
        VmFeatureManifestId featureManifestId,
        VmLimitVector requestedLimits,
        VmCallerIdentity callerIdentity)
    {
        ProfileId = profileId;
        FormatVersion = formatVersion;
        FeatureManifestId = featureManifestId;
        RequestedLimits = requestedLimits;
        CallerIdentity = callerIdentity;
    }

    /// <summary>The profile whose verifier owns these bytes.</summary>
    public VmProfileId ProfileId { get; }

    /// <summary>Exactly one profile-format version. Not a range.</summary>
    public uint FormatVersion { get; }

    /// <summary>Exactly one feature manifest. Not a set.</summary>
    public VmFeatureManifestId FeatureManifestId { get; }

    /// <summary>Limits the artifact requests.</summary>
    public VmLimitVector RequestedLimits { get; }

    /// <summary>The caller's own identity for these bytes.</summary>
    public VmCallerIdentity CallerIdentity { get; }

    /// <summary>Whether the descriptor is structurally usable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4A3BFD
    // Broiler-Human:        PENDING
    public bool IsWellFormed =>
        !ProfileId.IsEmpty &&
        FormatVersion >= 1 &&
        !FeatureManifestId.IsEmpty &&
        FeatureManifestId.StartsWithProfileNamespace(ProfileId);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=90F94D
    // Broiler-Human:        PENDING
    public bool Equals(VmArtifactDescriptor other) =>
        ProfileId.Equals(other.ProfileId) &&
        FormatVersion == other.FormatVersion &&
        FeatureManifestId.Equals(other.FeatureManifestId) &&
        RequestedLimits.Equals(other.RequestedLimits) &&
        CallerIdentity.Equals(other.CallerIdentity);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmArtifactDescriptor other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine(ProfileId, FormatVersion, FeatureManifestId, RequestedLimits, CallerIdentity);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmArtifactDescriptor left, VmArtifactDescriptor right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A85631
    // Broiler-Human:        PENDING
    public static bool operator !=(VmArtifactDescriptor left, VmArtifactDescriptor right) => !left.Equals(right);
}
""";
}
