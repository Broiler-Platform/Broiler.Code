# Broiler.Code

[![CI](https://github.com/Broiler-Platform/Broiler.Code/actions/workflows/ci.yml/badge.svg)](https://github.com/Broiler-Platform/Broiler.Code/actions/workflows/ci.yml)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](LICENSE)

Broiler.Code is the code editor of the [Broiler](https://github.com/Broiler-Platform/Broiler)
managed-code application stack for .NET. It holds the two desktop heads — Windows and Linux —
the shared shell they have in common, the workspace and text layer beneath it, the C#
language services, and the human review workspace that turns "somebody read this" into a
checkable record.

Everything below the application — graphics, media, input and the UI toolkit — lives in its
own repository and is consumed here as a NuGet package.

> **Preview.** Public APIs, repository layout and persisted behaviour are not frozen. A large
> share of the implementation was AI-assisted; the review record under `.broiler-review/` is
> the only claim this repository makes about what a person has actually read.

## Getting started

Clone the repository normally:

```bash
git clone https://github.com/Broiler-Platform/Broiler.Code.git
```

Build and restore resolve the component packages from nuget.org using
`NuGet.config`, with versions pinned in `Directory.Packages.props`.

Build and run the Windows head:

```bash
dotnet build Broiler.Windows.Code.slnx -c Release
```

```bash
dotnet run --project src/Broiler.Code.Windows/Broiler.Code.Windows.csproj -c Release
```

Run the tests:

```bash
dotnet test Broiler.Code.Tests.slnx -c Release
```

### Prerequisites

- **.NET SDK 10.0** or later.
- **Windows head** — builds on Windows; targets `net10.0-windows` and renders through the
  Direct2D graphics backend.
- **Linux head** — targets plain `net10.0`, with X11 clipboard and evdev input routing. The
  `Debug-Linux` and `Release-Linux` configurations pin `linux-x64`; the plain
  `Debug`/`Release` configurations build framework-dependent.

## Architecture

The dependency direction is the point of the layering, and it is asserted by tests rather
than only documented:

| Assembly | Depends on | Deliberately does **not** depend on |
|---|---|---|
| `Broiler.Code.Workspaces` | nothing | Broiler.UI, Roslyn, any platform |
| `Broiler.Code.Review` | `Broiler.Code.Workspaces` | Broiler.UI, Roslyn, git |
| `Broiler.Code.Language.CSharp.Syntax` | `Broiler.UI.CodeEditor` | Roslyn |
| `Broiler.Code.Language.CSharp.Roslyn` | `Broiler.UI.CodeEditor`, workspaces, the review model, Roslyn | — |
| `Broiler.Code.Core` | workspaces, review, Broiler.UI **abstractions**, Broiler.Input, Broiler.Graphics | Roslyn, any Broiler.UI implementation |
| `Broiler.Code.Windows` / `.Linux` | `Broiler.Code.Core`, the C# semantic assembly, the standard controls and one graphics/input backend | shell or workspace logic of its own |

Keeping Roslyn out of `Broiler.Code.Core`'s closure is what lets a browser or Android host
compose the portable classifier alone — the decision the Phase 0 payload probes measured.
`Broiler.Code.Review` has no UI reference for the same kind of reason: the coverage number CI
publishes and the badge the editor draws are computed by the same code, so they cannot
disagree.

The full record is [`docs/architecture/broiler-code.md`](docs/architecture/broiler-code.md);
the phased plan is [`docs/broiler-code-roadmap.md`](docs/broiler-code-roadmap.md).

## Development tools: Broiler.SDK

Create the `Broiler.SDK` NuGet tool package with the existing review CLI and C#
assurance tools:

```bash
dotnet pack src/Broiler.Code.Review.Cli -c Release
dotnet tool install --global Broiler.SDK --version 0.1.0-preview.1 --source ./artifacts/packages
broiler-review --help
```

Use the version of the package you built (the example is the current default).
See [Broiler.SDK](docs/broiler-sdk.md) for local tool installation, versioning and
package validation. CI validates the package on Windows and Linux; Publish adds
it to the draft release alongside the editor downloads and publishes it to
nuget.org using the `NUGET_TOKEN` Actions secret.

## Human review

`Broiler.Code.Review` evaluates a repository's `.broiler-review/` records: which files a
person has approved, and which approvals a later change has invalidated. Staleness is decided
by content, not by commit, so a rebase does not invalidate a review and a revert restores one.

```bash
dotnet run --project src/Broiler.Code.Review.Cli -- coverage --root .
```

In the editor, **File ▸ Open Folder…** grants a directory and fills the Solution
Explorer with the sources under it, so a component can be read and marked file by
file. It is the folder tree rather than the solution's own list that carries
them: an SDK-style project declares no compile items, and the declared model does
not evaluate the implicit globs that would invent them.

The design, including why the CI job reports rather than gates, is in
[`docs/architecture/broiler-code-review.md`](docs/architecture/broiler-code-review.md).

## Solutions

Each entry point has a focused solution containing exactly its transitive closure, so opening
one does not drag in another platform's backends.

| Solution | Entry point | Projects |
|---|---|---|
| `Broiler.Windows.Code.slnx` | `src/Broiler.Code.Windows` | 6 |
| `Broiler.Linux.Code.slnx` | `src/Broiler.Code.Linux` | 6 |
| `Broiler.Code.Tests.slnx` | the five test projects | 12 |

The solutions are **generated, not hand-edited**. `eng/solutions.json` declares each entry
point and the platform boundaries it must not cross; `scripts/update-solutions.ps1` walks the
real project-reference graph and writes the `.slnx` files from it:

```bash
pwsh scripts/update-solutions.ps1
```

`-Verify` fails instead of writing, which is the form CI runs:

```bash
pwsh scripts/update-solutions.ps1 -Verify
```

A hand-edit to a `.slnx` is silently reverted by the next generator run. Add or remove
projects by changing the reference graph, then regenerate.

## Continuous integration

[`ci.yml`](.github/workflows/ci.yml) runs on every push to `main` and every pull request:

- **Solution manifest** — `scripts/update-solutions.ps1 -Verify`, which fails if a checked-in
  `.slnx` no longer matches the reference graph. This is what catches a new
  `ProjectReference` that was never folded into a solution.
- **Build** — the Windows head on `windows-latest`, the Linux head on `ubuntu-latest`.
- **Tests** — the suite on both hosts, because the shell does clipboard, file-dialog and
  input-routing work that is easy to make accidentally platform-specific.
- **Broiler.SDK** — packs and installs the development tools on both hosts, then
  exercises review and assurance commands.
- **Human review** — coverage over this repository's sources, reported to the run summary.

[`.github/actions/setup-broiler`](.github/actions/setup-broiler/action.yml) installs
.NET after a normal checkout. CI and Publish restore component dependencies as
NuGet packages; they do not initialize component checkouts. The retired Phase 0
harnesses and payload probes are no longer part of this checkout or its workflows.
The shared [SampleReports fixture](tests/broiler-code-phase0/fixture/README.md)
remains required test data for workspace parsing, evaluation and semantic diagnostics;
its intentionally broken App project is excluded from the build solutions.

## Build configuration

[`Directory.Build.props`](Directory.Build.props) decomposes the four-configuration scheme the
desktop heads declare. `Debug`/`Release` build framework-dependent; `Debug-Windows`,
`Release-Windows`, `Debug-Linux` and `Release-Linux` pin a runtime identifier. MSBuild
understands only `Debug` and `Release` on its own, so without this file a `-c Release-Linux`
build is **unoptimized and has neither `RELEASE` nor `LINUX` defined**.

[`Directory.Build.targets`](Directory.Build.targets) keeps parent checkout targets
from rewriting this repository's local project references. Component dependencies
remain NuGet packages. `Directory.Build.props` carries no `NoWarn` list, so build
warnings remain visible.

The platform hosting glue — the Win32 clipboard and window sizing, and the X11 clipboard and evdev
input coordination — comes from the `Broiler.Hosting.Windows` and `Broiler.Hosting.Linux`
packages, the same ones the other Broiler applications use.

## Repository layout

| Path | Contents |
|---|---|
| `src/Broiler.Code.Workspaces` | Text layer and workspace model. No references at all, deliberately. |
| `src/Broiler.Code.Review` | Human review record, coverage and staleness — no UI, no Roslyn, no git |
| `src/Broiler.Code.Review.Cli` | The half of the review workspace that runs where there is no display |
| `src/Broiler.Code.Core` | The seam: workspace snapshots adapted to the control-facing editor interfaces, plus the shell |
| `src/Broiler.Code.Language.CSharp.Syntax` | Portable C# classifier — the fallback a host without Roslyn composes |
| `src/Broiler.Code.Language.CSharp.Roslyn` | Semantic language service on one pinned Roslyn toolset |
| `src/Broiler.Code.Windows` | Windows head — `WinExe`, Direct2D, Win32 clipboard and window sizing |
| `src/Broiler.Code.Linux` | Linux head — X11 clipboard and evdev input coordination |
| `src/Broiler.Code.*.Tests` | xUnit suites — workspaces, shell and adapters, language services, review model |
| `tests/broiler-code-phase0/fixture` | Shared SampleReports test data, retained independently of the retired harnesses |
| `docs/` | Architecture records, budgets, the SDK-mutation matrix and the roadmap |
| `eng/`, `scripts/` | Solution manifest and generator |
| `.github/` | CI workflow and the `setup-broiler` composite action |

## Dependencies

Component dependencies, including graphics, input, UI controls and platform
hosting, are NuGet packages from nuget.org. Direct package versions are centralized
in [`Directory.Packages.props`](Directory.Packages.props); transitive dependencies
such as media are resolved by NuGet. Local `ProjectReference` items connect only
projects owned by Broiler.Code. No component submodule checkout is required.

## License

Apache License 2.0 — see [LICENSE](LICENSE).
