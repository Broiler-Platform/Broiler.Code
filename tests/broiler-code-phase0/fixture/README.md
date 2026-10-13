# SampleReports — shared workspace test fixture

This fixture is used by `DeclaredProjectTests`, `SemanticServiceTests` and
`EvaluationTests`. Its historical `tests/broiler-code-phase0/fixture` path is
still used by those tests even though the Phase 0 benchmark harnesses have been
retired. Keep this directory in the checkout: the tests read its project files
and sources, and the evaluation tests invoke the .NET SDK against its App project.

Restored from the parent of commit `39f0ca0`, which removed the fixture together
with the Phase 0 harnesses. It is ordinary committed test data, with no Broiler
component or submodule dependency.

The fixture deliberately remains outside
[`eng/solutions.json`](../../../eng/solutions.json): its default App build fails
with an intentional C# error used by the semantic tests. Android and Web projects
are parsed as test data; the unit tests do not build them or require their workloads.

## Layout

| Path | Why it is here |
| --- | --- |
| `SampleReports.slnx` | Standard solution format with solution folders and solution items |
| `global.json` | Pins the SDK feature band; the worker must honor it instead of substituting the IDE's SDK |
| `NuGet.config` | Restore configuration that changes what the build resolves, so it belongs in the granted snapshot and the input manifest |
| `Directory.Build.props` / `.targets` | Ancestor isolation — see below |
| `src/SampleReports.Core` | Multi-targeting library (`net10.0;netstandard2.0`), embedded resource, linked file, conditional item, wildcard item |
| `src/SampleReports.App` | Console application with a project reference and the intentional errors |
| `shared/BuildStamp.cs` | A source file outside every project cone, reached by a linked `Compile` item |
| `targets/SampleReports.Android` | `net10.0-android36.0` head with a project-local ABI property |
| `targets/SampleReports.Web` | Standalone `Microsoft.NET.Sdk.WebAssembly` head with explicit interpreted and AOT publish profiles |
| `expected-diagnostics.json` | The diagnostics the fixture must produce, by rule ID and span — never by message text |

## Ancestor isolation

The fixture carries its own `Directory.Build.props` and `Directory.Build.targets`
that do not chain up to the repository's. Without them the fixture would
evaluate one way inside the Broiler checkout and another way after the build
worker materialized it into a neutral per-job root, and every measurement taken
against it would be unreproducible. The same reasoning is why the worker
materializes outside the checkout in the first place.

## The intentional errors

`src/SampleReports.App/Reporting/ReportRunner.Broken.cs` calls
`ReportFormatter.Format(report)` while the method is declared in
`src/SampleReports.Core/Reporting/ReportFormatter.cs` as
`Format(Report, FormatOptions)`. Nothing in the calling file is wrong when read
on its own — a syntax-only classifier cannot find this, which is exactly the
capability boundary Phase 0 has to make visible.

`src/SampleReports.App/Reporting/LegacySummary.cs` calls an `[Obsolete]` member
from the same referenced project and is always compiled, so both fixture
variants emit one warning. "Build succeeded" therefore never means "Problems
pane empty".

Setting `FixtureCorrected=true` swaps `ReportRunner.Broken.cs` for
`ReportRunner.Corrected.cs` through a conditional `Compile Remove` item, so the
corrected variant needed by the Phase 4 and Phase 5 artifact gates is reachable
without editing tracked source:

```bash
dotnet build tests/broiler-code-phase0/fixture/src/SampleReports.App/SampleReports.App.csproj -c Release -p:FixtureCorrected=true
```

## Run the consuming tests

From the repository root, with the .NET 10 SDK installed:

```sh
dotnet test src/Broiler.Code.Workspaces.Tests -c Release
dotnet test src/Broiler.Code.Language.CSharp.Tests -c Release
```

The evaluation tests restore and resolve the App project's references themselves.
Do not prebuild the broken App project or add the fixture solution to the CI
build matrix. Preserve source layout and line numbers: semantic diagnostics are
asserted against the recorded call site, including its line position.
