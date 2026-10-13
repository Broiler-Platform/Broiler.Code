# Broiler.SDK

Broiler.SDK packages the existing Broiler development tools as a .NET tool. It
provides the `broiler-review` command, including human-review coverage and
staleness checks plus the C# assurance scanner, annotation tools and reports.
The package bundles the supporting Broiler assemblies and Roslyn dependencies;
using it requires no Broiler.Code checkout or desktop editor.

## Build the package

With the .NET 10 SDK installed, run from a Broiler.Code checkout:

```sh
dotnet pack src/Broiler.Code.Review.Cli -c Release
```

This creates `artifacts/packages/Broiler.SDK.<version>.nupkg`. Its version follows
`BroilerCodeVersion` in `Directory.Build.props`. To set a release version:

```sh
dotnet pack src/Broiler.Code.Review.Cli -c Release -p:BroilerCodeVersion=0.1.0-preview.2
```

## Install and use

Install a built or downloaded package from its containing directory. The example
uses the repository's default preview version; substitute the version you built
or downloaded. A .NET 10 runtime is required to run the tool, and the SDK supplies
the `dotnet tool` commands.

```sh
dotnet tool install --global Broiler.SDK --version 0.1.0-preview.1 --source ./artifacts/packages
broiler-review --help
broiler-review coverage --root .
broiler-review check --root . --fail-on-stale
broiler-review assurance --help
broiler-review assurance list --root . --json units.json
```

For a repository-local tool, create a manifest once and install into it:

```sh
dotnet new tool-manifest
dotnet tool install --local Broiler.SDK --version 0.1.0-preview.1 --source ./artifacts/packages
dotnet tool run broiler-review coverage --root .
```

Commit `.config/dotnet-tools.json`; other contributors can then run
`dotnet tool restore` with a NuGet configuration that includes the package source
and maps `Broiler.SDK` to it if package source mapping is enabled. Use
`dotnet tool update` with the same scope and package source to install a newer
version. The install examples use `--source` to override the checkout's
nuget.org-only source mapping for this command.

Human-review commands read `.broiler-review/` records. Assurance commands include
`list`, `insert`, `generate`, `check`, `status` and `prune`; annotation writes
require an `assurance.config.json` that opts the target repository in. See the
[assurance command reference](https://github.com/Broiler-Platform/Broiler.Code/blob/main/docs/architecture/broiler-code-assurance-cli.md).

## Release and validation

CI packs and installs the tool on Windows and Linux, then exercises review and
assurance commands against a small standalone C# project. To run the same check:

```powershell
./scripts/test-sdk-package.ps1 -PackagePath ./artifacts/packages/Broiler.SDK.0.1.0-preview.1.nupkg
```

The dispatch-only Publish workflow includes the versioned `.nupkg` in its run
artifacts and draft GitHub release. It does not push to a NuGet feed. The package
can be distributed through a local source or pushed to a feed by its maintainer.
