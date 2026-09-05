# Broiler.Code

[![CI](https://github.com/Broiler-Platform/Broiler.Code/actions/workflows/ci.yml/badge.svg)](https://github.com/Broiler-Platform/Broiler.Code/actions/workflows/ci.yml)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](LICENSE)

Broiler.Code is the code editor of the [Broiler](https://github.com/Broiler-Platform/Broiler)
managed-code application stack for .NET. It holds the two desktop heads — Windows and Linux —
the shared shell they have in common, the workspace and text layer beneath it, the C#
language services, and the human review workspace that turns "somebody read this" into a
checkable record.

Everything below the application — graphics, media, input and the UI toolkit — lives in its
own repository and is consumed here as a submodule.

> **Preview.** Public APIs, repository layout and persisted behaviour are not frozen. A large
> share of the implementation was AI-assisted; the review record under `.broiler-review/` is
> the only claim this repository makes about what a person has actually read.

## Getting started

The dependency components are submodules, so the checkout must be recursive:

```bash
git clone --recurse-submodules https://github.com/Broiler-Platform/Broiler.Code.git
```

If you already cloned without them:

```bash
git submodule update --init --recursive
```

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
- **Phase 0 payload probes** — need the `wasm-tools` workload
  (`dotnet workload install wasm-tools`).

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
| `Broiler.Windows.Code.slnx` | `src/Broiler.Code.Windows` | 38 |
| `Broiler.Linux.Code.slnx` | `src/Broiler.Code.Linux` | 40 |
| `Broiler.Code.Tests.slnx` | the four test projects | 41 |
| `Broiler.Code.Phase0.slnx` | the three Phase 0 harnesses | 3 |
| `Broiler.WebAssembly.Code.slnx` | the two payload probes | 2 |

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

The `SampleReports` fixture under `tests/broiler-code-phase0/fixture` is deliberately in no
solution: it fails to compile by design — that is what the build worker harness measures — and
including it would break build verification.

## Continuous integration

[`ci.yml`](.github/workflows/ci.yml) runs on every push to `main` and every pull request:

- **Solution manifest** — `scripts/update-solutions.ps1 -Verify`, which fails if a checked-in
  `.slnx` no longer matches the reference graph. This is what catches a new
  `ProjectReference` that was never folded into a solution.
- **Build** — the Windows head on `windows-latest`, the Linux head on `ubuntu-latest`.
- **Tests** — the suite on both hosts, because the shell does clipboard, file-dialog and
  input-routing work that is easy to make accidentally platform-specific.
- **Phase 0 harnesses** and **payload probes** — built, not run: the prototype, the
  Roslyn measurement and the two browser payloads are benchmarks whose numbers belong
  to a recorded baseline taken on a known machine, not to a shared runner. What CI
  protects is that they still compile.
- **Human review** — coverage over this repository, reported to the run summary.

The nested-submodule set is defined once, in
[`.github/actions/setup-broiler`](.github/actions/setup-broiler/action.yml).

## Build configuration

[`Directory.Build.props`](Directory.Build.props) decomposes the four-configuration scheme the
desktop heads declare. `Debug`/`Release` build framework-dependent; `Debug-Windows`,
`Release-Windows`, `Debug-Linux` and `Release-Linux` pin a runtime identifier. MSBuild
understands only `Debug` and `Release` on its own, so without this file a `-c Release-Linux`
build is **unoptimized and has neither `RELEASE` nor `LINUX` defined**.

[`Directory.Build.targets`](Directory.Build.targets) exists only to be *found*: it stops
the Broiler monorepo's root targets — and the reference redirects they import — from
reaching these projects when this repository is checked out as a submodule there. The file
itself is empty; the comment inside explains why redirecting them would double-build
`Broiler.UI`, `Broiler.Graphics`, `Broiler.Input` and `Broiler.Media`.

`Directory.Build.props` carries **no** `NoWarn` list, deliberately. A clean rebuild of all five solutions emits
one `CS8602` in `Broiler.Graphics.Linux.OpenGL` and three `xUnit1031` in
`Broiler.Code.Core.Tests`, and nothing else — every `Broiler.Code.*` project sets
`TreatWarningsAsErrors` and compiles clean. Both are left visible: the first is a real
upstream defect, the second is fixable here. Do not import a suppression list from a
consumer repository; those were measured against a much larger graph and would hide the
next real warning.

## Repository layout

| Path | Contents |
|---|---|
| `src/Broiler.Code.Workspaces` | Text layer and workspace model. No references at all, deliberately. |
| `src/Broiler.Code.Review` | Human review record, coverage and staleness — no UI, no Roslyn, no git |
| `src/Broiler.Code.Review.Cli` | The half of the review workspace that runs where there is no display |
| `src/Broiler.Code.Core` | The seam: workspace snapshots adapted to the control-facing editor interfaces, plus the shell |
| `src/Broiler.Code.Language.CSharp.Syntax` | Portable C# classifier — the fallback a host without Roslyn composes |
| `src/Broiler.Code.Language.CSharp.Roslyn` | Semantic language service on one pinned Roslyn toolset |
| `src/Broiler.Code.Windows` | Windows head — `WinExe`, Direct2D, Win32 clipboard |
| `src/Broiler.Code.Linux` | Linux head — X11 clipboard, evdev input |
| `src/Broiler.App` | Source-only directory shared by the desktop heads — the per-platform clipboards. It has no project of its own; each head links the file it needs. |
| `src/Broiler.Code.*.Tests` | xUnit suites — workspaces, shell and adapters, language services, review model |
| `tests/broiler-code-phase0` | Phase 0 harnesses, fixtures, traces, payload probes and recorded baselines |
| `docs/` | Architecture records, budgets, the SDK-mutation matrix and the roadmap |
| `eng/`, `scripts/` | Solution manifest and generator |
| `.github/` | CI workflow and the `setup-broiler` composite action |

## Dependencies

Four components are submodules, pinned to `main`:

| Component | Purpose |
|---|---|
| `Broiler.Graphics` | Managed bitmap/codec/raster core plus platform backends |
| `Broiler.Media` | Image, audio and video abstractions and managed codecs |
| `Broiler.Input` | Keyboard, mouse, pen, touch and text input abstractions |
| `Broiler.UI` | Platform-neutral retained-mode UI toolkit, including the CodeEditor control family |

Each of those repositories carries nested checkouts of the components *it* depends on, so
that it still builds standalone. `git submodule update --init --recursive` restores the whole
set.

### Known issue: some components compile more than once

Because each component repository references its own nested checkouts by literal relative
path, composing them here means `Broiler.Graphics`, `Broiler.Media` and `Broiler.Input` are
each compiled from more than one source tree. Every nested gitlink points at the same commit
as the top-level one, so the duplicates are assembly-identical and the build reports no
reference conflicts — but it is wasted work. The fix is a `$(BroilerGraphicsPath)`-style
property hook upstream in `Broiler.UI`, `Broiler.Graphics` and `Broiler.Media`. The solution
generator folds the nested paths onto the top-level ones, so the `.slnx` files list each
assembly once.

## License

Apache License 2.0 — see [LICENSE](LICENSE).
