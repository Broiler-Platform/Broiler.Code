[CmdletBinding()]
param(
    [switch] $Verify
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$manifestPath = Join-Path $repositoryRoot 'eng/solutions.json'
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json

# Every component repository carries nested checkouts of the components it depends on so that
# it still builds standalone. Composed here, those nested copies duplicate a top-level
# checkout, so a solution generated straight from the reference graph would list the same
# assembly under several paths. Each entry folds one <parent>/<nested> spelling onto the
# checkout it duplicates.
#
# The table is applied REPEATEDLY rather than once, so a path nested more than one level deep
# (Broiler.UI/Broiler.Graphics/Broiler.Media/... is three) collapses through the same
# single-hop entries instead of needing an entry per deep path. Every value is shorter than
# its key, so the rewrite strictly shortens the path and the loop always terminates.
#
# A prefix match is used rather than "the last segment that names a component" because
# component repositories contain directories that repeat their own name and are NOT separate
# checkouts - Broiler.Graphics/src/Broiler.Graphics/ among them.
$duplicateCheckoutMappings = [ordered]@{
    'Broiler.Graphics/Broiler.Input/' = 'Broiler.Input/'
    'Broiler.Graphics/Broiler.Media/' = 'Broiler.Media/'
    'Broiler.Media/Broiler.Graphics/' = 'Broiler.Graphics/'
    'Broiler.UI/Broiler.Graphics/' = 'Broiler.Graphics/'
    'Broiler.UI/Broiler.Input/' = 'Broiler.Input/'
}

# Deliberately absent: Broiler.UI/Broiler.Documents/. Broiler.Code has no top-level
# Broiler.Documents checkout to fold onto, so a mapping for it would rewrite a real path
# into one that does not exist. No project in any closure below reaches Broiler.Documents;
# if one ever does, Convert-ToRepositoryPath fails loudly and the answer is to add the
# component as a submodule here, not to add a mapping.

$referenceCache = @{}

function Convert-ToRepositoryPath {
    param(
        [Parameter(Mandatory)]
        [string] $FullPath
    )

    $normalizedFullPath = [IO.Path]::GetFullPath($FullPath)
    $rootPrefix = $repositoryRoot.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    if (-not $normalizedFullPath.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Project path escapes the repository: $normalizedFullPath"
    }
    $relativePath = $normalizedFullPath.Substring($rootPrefix.Length).Replace('\', '/')

    # Fold to a fixed point: one pass only strips the outermost nesting level.
    for ($pass = 0; $pass -lt 16; $pass++) {
        $foldedThisPass = $false
        foreach ($mapping in $duplicateCheckoutMappings.GetEnumerator()) {
            if ($relativePath.StartsWith($mapping.Key, [StringComparison]::OrdinalIgnoreCase)) {
                $relativePath = $mapping.Value + $relativePath.Substring($mapping.Key.Length)
                $foldedThisPass = $true
                break
            }
        }
        if (-not $foldedThisPass) {
            break
        }
    }

    $canonicalFullPath = Join-Path $repositoryRoot $relativePath
    if (-not (Test-Path -LiteralPath $canonicalFullPath -PathType Leaf)) {
        throw "Project does not exist: $relativePath"
    }

    return $relativePath
}

# The path a ProjectReference names, before the duplicate-checkout fold. This is where
# MSBuild itself would send the reference, which is exactly what makes it worth knowing:
# where it differs from the folded path, the build graph and the generated solution
# disagree, and eng/fold-duplicate-checkouts.targets exists to close that gap.
function Expand-ProjectReferenceInclude {
    param(
        [Parameter(Mandatory)]
        [string] $ProjectPath,

        [Parameter(Mandatory)]
        [string] $Include
    )

    $projectFullPath = Join-Path $repositoryRoot $ProjectPath
    $projectDirectory = Split-Path -Parent $projectFullPath

    $resolvedInclude = $Include
    $resolvedInclude = $resolvedInclude.Replace(
        '$(MSBuildThisFileDirectory)',
        $projectDirectory + [IO.Path]::DirectorySeparatorChar)
    $resolvedInclude = $resolvedInclude.Replace(
        '$(BroilerGraphicsPath)',
        (Join-Path $repositoryRoot 'Broiler.Graphics/src/Broiler.Graphics/Broiler.Graphics.csproj'))

    # The component roots this repository sets in Directory.Build.props. A component that
    # reads its siblings from one of these needs no entry in the generated fold: the
    # property already points at the top-level checkout. Resolving them the same way here
    # is what keeps the closure honest — get this wrong and the solutions would name a
    # checkout the build never touches, which is the failure the fold exists to prevent.
    foreach ($componentRoot in @('Graphics', 'Input', 'Media', 'UI')) {
        $resolvedInclude = $resolvedInclude.Replace(
            "`$(Broiler${componentRoot}Root)",
            (Join-Path $repositoryRoot "Broiler.$componentRoot"))
    }

    if ($resolvedInclude.Contains('$(')) {
        throw "Unsupported property in ProjectReference '$Include' from '$ProjectPath'."
    }

    if (-not [IO.Path]::IsPathRooted($resolvedInclude)) {
        $resolvedInclude = Join-Path $projectDirectory $resolvedInclude
    }

    return [IO.Path]::GetFullPath($resolvedInclude)
}

function Resolve-ProjectReference {
    param(
        [Parameter(Mandatory)]
        [string] $ProjectPath,

        [Parameter(Mandatory)]
        [string] $Include
    )

    return Convert-ToRepositoryPath -FullPath (
        Expand-ProjectReferenceInclude -ProjectPath $ProjectPath -Include $Include)
}

# The ProjectReferences of one project that name a nested duplicate checkout, paired with
# the canonical checkout they fold onto. The Include is returned exactly as written, because
# the generated targets file removes the item by that spelling before adding the folded one.
function Get-ProjectReferenceRewrites {
    param(
        [Parameter(Mandatory)]
        [string] $ProjectPath
    )

    $projectFullPath = Join-Path $repositoryRoot $ProjectPath
    [xml] $project = Get-Content -Raw -LiteralPath $projectFullPath

    $rewrites = [Collections.Generic.List[pscustomobject]]::new()
    foreach ($node in $project.SelectNodes('//ProjectReference[@Include]')) {
        foreach ($include in ([string] $node.Include).Split(';', [StringSplitOptions]::RemoveEmptyEntries)) {
            $namedPath = Expand-ProjectReferenceInclude -ProjectPath $ProjectPath -Include $include
            $canonicalPath = Resolve-ProjectReference -ProjectPath $ProjectPath -Include $include
            if ($namedPath -eq [IO.Path]::GetFullPath((Join-Path $repositoryRoot $canonicalPath))) {
                continue
            }

            $rewrites.Add([pscustomobject]@{
                Include   = $include
                Canonical = $canonicalPath
            })
        }
    }

    return @($rewrites)
}

function Get-ProjectReferences {
    param(
        [Parameter(Mandatory)]
        [string] $ProjectPath
    )

    if ($referenceCache.ContainsKey($ProjectPath)) {
        return @($referenceCache[$ProjectPath])
    }

    $projectFullPath = Join-Path $repositoryRoot $ProjectPath
    [xml] $project = Get-Content -Raw -LiteralPath $projectFullPath
    $references = @(
        $project.SelectNodes('//ProjectReference[@Include]') |
            ForEach-Object {
                foreach ($include in ([string] $_.Include).Split(';', [StringSplitOptions]::RemoveEmptyEntries)) {
                    Resolve-ProjectReference -ProjectPath $ProjectPath -Include $include
                }
            } |
            Sort-Object -Unique
    )

    $referenceCache[$ProjectPath] = $references
    return @($references)
}

function Get-ProjectClosure {
    param(
        [Parameter(Mandatory)]
        [string[]] $Roots
    )

    $pending = [Collections.Generic.Queue[string]]::new()
    $visited = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)

    foreach ($root in $Roots) {
        $pending.Enqueue((Convert-ToRepositoryPath -FullPath (Join-Path $repositoryRoot $root)))
    }

    while ($pending.Count -gt 0) {
        $projectPath = $pending.Dequeue()
        if (-not $visited.Add($projectPath)) {
            continue
        }

        foreach ($reference in Get-ProjectReferences -ProjectPath $projectPath) {
            $pending.Enqueue($reference)
        }
    }

    return @($visited | Sort-Object)
}

function Convert-ToXmlAttribute {
    param(
        [Parameter(Mandatory)]
        [string] $Value
    )

    return [Security.SecurityElement]::Escape($Value)
}

function New-SolutionText {
    param(
        [Parameter(Mandatory)]
        [pscustomobject] $Definition,

        [Parameter(Mandatory)]
        [string[]] $Projects
    )

    $rootSet = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach ($root in $Definition.roots) {
        [void] $rootSet.Add((Convert-ToRepositoryPath -FullPath (Join-Path $repositoryRoot $root)))
    }

    # Solution-level deploy flags. Visual Studio's F5 and `msbuild /t:Deploy` on
    # the solution use these to install an application head; `dotnet build` does
    # not, which is why nothing in CI depends on them. They live in the manifest
    # rather than in the .slnx because a hand-edit to a generated file is silently
    # reverted by the next generator run.
    $deployByProject = @{}
    foreach ($entry in @($Definition.deploy)) {
        if ($null -eq $entry) {
            continue
        }

        $deployProject = Convert-ToRepositoryPath -FullPath (Join-Path $repositoryRoot $entry.project)
        if ($deployProject -notin $Projects) {
            throw (
                "$($Definition.path) declares a deploy entry for '$($entry.project)', " +
                'which is not in its project closure.')
        }

        $solutionExpression = [string] $entry.solution
        if ([string]::IsNullOrWhiteSpace($solutionExpression)) {
            throw "$($Definition.path) declares a deploy entry for '$($entry.project)' with no solution expression."
        }

        $deployByProject[$deployProject] = $solutionExpression
    }

    $groups = [ordered]@{}
    $groups['Entry points'] = @($Projects | Where-Object { $rootSet.Contains($_) })
    foreach ($project in $Projects | Where-Object { -not $rootSet.Contains($_) }) {
        $topLevelDirectory = $project.Split('/')[0]
        $groupName = "Dependencies/$topLevelDirectory"
        if (-not $groups.Contains($groupName)) {
            $groups[$groupName] = @()
        }
        $groups[$groupName] += $project
    }

    $lines = [Collections.Generic.List[string]]::new()
    $lines.Add('<Solution>')
    $lines.Add('  <!-- Generated from eng/solutions.json by scripts/update-solutions.ps1. -->')
    $lines.Add('  <Configurations>')
    $lines.Add('    <BuildType Name="Debug" />')
    $lines.Add('    <BuildType Name="Release" />')
    $lines.Add('  </Configurations>')

    foreach ($group in $groups.GetEnumerator()) {
        if ($group.Value.Count -eq 0) {
            continue
        }

        $folderName = Convert-ToXmlAttribute -Value "/$($group.Key)/"
        $lines.Add("  <Folder Name=`"$folderName`">")
        foreach ($project in $group.Value | Sort-Object) {
            $projectPath = Convert-ToXmlAttribute -Value $project
            if ($deployByProject.ContainsKey($project)) {
                $deploySolution = Convert-ToXmlAttribute -Value $deployByProject[$project]
                $lines.Add("    <Project Path=`"$projectPath`">")
                $lines.Add("      <Deploy Solution=`"$deploySolution`" />")
                $lines.Add('    </Project>')
            }
            else {
                $lines.Add("    <Project Path=`"$projectPath`" />")
            }
        }
        $lines.Add('  </Folder>')
    }

    $lines.Add('</Solution>')
    return ($lines -join "`n") + "`n"
}

function New-FoldTargetsText {
    param(
        # Deliberately untyped: PowerShell's binder enumerates an ordered dictionary into
        # DictionaryEntry[] when the parameter names a collection type, and the conversion
        # back fails. Keyed project -> rewrite list, in the order the caller built it.
        [Parameter(Mandatory)]
        $Rewrites
    )

    $lines = [Collections.Generic.List[string]]::new()
    $lines.Add('<Project>')
    $lines.Add('')
    $lines.Add('  <!-- Generated from eng/solutions.json by scripts/update-solutions.ps1. -->')
    $lines.Add('')
    $lines.Add('  <!--')
    $lines.Add('    The solutions above fold every nested duplicate checkout onto the top-level one, so')
    $lines.Add('    they name a single Broiler.Input.Mouse. The ProjectReferences do not: Broiler.UI')
    $lines.Add('    reaches its OWN nested Broiler.Input, Broiler.Graphics reaches its own, and the')
    $lines.Add('    Broiler.Code heads reach the top-level checkout. Each spelling is correct for the')
    $lines.Add('    repository that owns it and builds standalone from it. Composed here they are three')
    $lines.Add('    project files claiming one package identity, Broiler.Input.Mouse/0.1.0-preview.1.')
    $lines.Add('')
    $lines.Add('    NuGet collapses that identity onto ONE checkout per restore. When the winner is a')
    $lines.Add('    checkout the solution does not list, the referencing project and the restore graph')
    $lines.Add('    disagree, the assembly drops out of the head''s deps.json, and the app dies at')
    $lines.Add('    startup with FileNotFoundException on an assembly sitting right next to the exe.')
    $lines.Add('    Nothing warns: the build reports zero errors and quietly compiles both copies.')
    $lines.Add('')
    $lines.Add('    This file removes each nested spelling and adds the folded one back, so the build')
    $lines.Add('    graph names exactly the projects the solutions do. It is a static ItemGroup rather')
    $lines.Add('    than a target ON PURPOSE. Visual Studio nominates a project to NuGet from its')
    $lines.Add('    EVALUATION, so a target-based rewrite is honoured by `dotnet build` and skipped by')
    $lines.Add('    the IDE - which is the case that actually broke. Anything that runs here must')
    $lines.Add('    survive being read without executing a single target.')
    $lines.Add('')
    $lines.Add('    Generated rather than hand-written so it cannot drift from the solutions: both come')
    $lines.Add('    from one walk of one reference graph, and CI runs update-solutions.ps1 -Verify.')
    $lines.Add('  -->')
    $lines.Add('')
    $lines.Add('  <!--')
    $lines.Add('    This project, relative to the repository root and always forward-slashed, so the')
    $lines.Add('    conditions below read the same on both hosts. Every project that imports this file')
    $lines.Add('    lives under BroilerRepositoryRoot - MSBuild picks the NEAREST Directory.Build.targets')
    $lines.Add('    above a project - so the Substring is always in range.')
    $lines.Add('  -->')
    $lines.Add('  <PropertyGroup>')
    $lines.Add(
        '    <_BroilerFoldProjectPath>' +
        '$(MSBuildProjectFullPath.Substring($(BroilerRepositoryRoot.Length)))' +
        '</_BroilerFoldProjectPath>')
    $lines.Add(
        '    <_BroilerFoldProjectPath>' +
        "`$(_BroilerFoldProjectPath.Replace('\', '/'))" +
        '</_BroilerFoldProjectPath>')
    $lines.Add('  </PropertyGroup>')

    foreach ($entry in $Rewrites.GetEnumerator()) {
        $project = [string] $entry.Key
        $rewrites = @($entry.Value)

        $projectAttribute = Convert-ToXmlAttribute -Value $project
        $condition = "'`$(_BroilerFoldProjectPath)' == '$projectAttribute'"

        $lines.Add('')
        $lines.Add("  <ItemGroup Condition=`"$condition`">")
        foreach ($rewrite in $rewrites) {
            $includeAttribute = Convert-ToXmlAttribute -Value $rewrite.Include
            $canonicalAttribute = Convert-ToXmlAttribute -Value $rewrite.Canonical
            $lines.Add("    <ProjectReference Remove=`"$includeAttribute`" />")
            $lines.Add(
                '    <ProjectReference Include="$(BroilerRepositoryRoot)' + $canonicalAttribute + '" />')
        }
        $lines.Add('  </ItemGroup>')
    }

    $lines.Add('')
    $lines.Add('</Project>')
    return ($lines -join "`n") + "`n"
}

$manifestSolutionPaths = @($manifest.solutions | ForEach-Object { [string] $_.path })
$duplicateManifestPaths = @(
    $manifestSolutionPaths |
        Group-Object |
        Where-Object Count -gt 1 |
        ForEach-Object Name
)
if ($duplicateManifestPaths.Count -gt 0) {
    throw "Duplicate solution paths in eng/solutions.json:`n  $($duplicateManifestPaths -join "`n  ")"
}

$testRootOwners = @{}
foreach ($definition in $manifest.solutions | Where-Object path -Like '*.Tests.slnx') {
    foreach ($root in $definition.roots) {
        $canonicalRoot = Convert-ToRepositoryPath -FullPath (Join-Path $repositoryRoot $root)
        if ($testRootOwners.ContainsKey($canonicalRoot)) {
            throw (
                "Test root '$canonicalRoot' belongs to both " +
                "'$($testRootOwners[$canonicalRoot])' and '$($definition.path)'.")
        }
        $testRootOwners[$canonicalRoot] = $definition.path
    }
}

$topLevelSolutions = @(
    Get-ChildItem -LiteralPath $repositoryRoot -Filter '*.slnx' -File |
        ForEach-Object { $_.Name } |
        Sort-Object
)
$unexpectedSolutions = @(
    $topLevelSolutions |
        Where-Object { $_ -notin $manifestSolutionPaths }
)
if ($unexpectedSolutions.Count -gt 0) {
    throw "Top-level solution files are not declared in eng/solutions.json:`n  $($unexpectedSolutions -join "`n  ")"
}

$errors = [Collections.Generic.List[string]]::new()
$utf8WithoutBom = [Text.UTF8Encoding]::new($false)
$foldTargetsPath = 'eng/fold-duplicate-checkouts.targets'
$foldProjects = [Collections.Generic.SortedSet[string]]::new([StringComparer]::OrdinalIgnoreCase)

foreach ($definition in $manifest.solutions) {
    $solutionPath = Join-Path $repositoryRoot $definition.path
    $projects = Get-ProjectClosure -Roots @($definition.roots)
    foreach ($project in $projects) {
        [void] $foldProjects.Add($project)
    }

    if ($definition.path -notlike '*.Tests.slnx' -and
        $definition.path -ne 'Broiler.Benchmarks.slnx') {
        $qualityProjects = @(
            $projects |
                Where-Object { $_ -match '(?i)\.(Tests|Benchmarks)\.csproj$' }
        )
        if ($qualityProjects.Count -gt 0) {
            $errors.Add(
                "$($definition.path) includes test or benchmark projects: " +
                ($qualityProjects -join ', '))
        }
    }

    foreach ($pattern in @(
        $definition.forbiddenProjectPatterns |
            Where-Object { -not [string]::IsNullOrWhiteSpace([string] $_) }
    )) {
        $violations = @($projects | Where-Object { $_ -match $pattern })
        if ($violations.Count -gt 0) {
            $errors.Add(
                "$($definition.path) crosses a declared platform boundary for '$pattern': " +
                ($violations -join ', '))
        }
    }

    $expectedText = New-SolutionText -Definition $definition -Projects $projects
    if ($Verify) {
        if (-not (Test-Path -LiteralPath $solutionPath -PathType Leaf)) {
            $errors.Add("$($definition.path) is missing. Run scripts/update-solutions.ps1.")
            continue
        }

        $actualText = [IO.File]::ReadAllText($solutionPath).Replace("`r`n", "`n")
        if ($actualText -ne $expectedText) {
            $errors.Add("$($definition.path) is stale. Run scripts/update-solutions.ps1.")
        }
    }
    else {
        [IO.File]::WriteAllText($solutionPath, $expectedText, $utf8WithoutBom)
        Write-Host ("Updated {0} ({1} roots, {2} projects)." -f
            $definition.path,
            @($definition.roots).Count,
            $projects.Count)
    }
}

# The fold covers the union of every solution closure: a project outside all of them is
# never built here, so rewriting its references would be inventing work.
$foldTargetsFullPath = Join-Path $repositoryRoot $foldTargetsPath
$foldRewrites = [ordered]@{}
foreach ($project in $foldProjects) {
    $projectRewrites = @(Get-ProjectReferenceRewrites -ProjectPath $project)
    if ($projectRewrites.Count -gt 0) {
        $foldRewrites[$project] = $projectRewrites
    }
}

$expectedFoldText = New-FoldTargetsText -Rewrites $foldRewrites
if ($Verify) {
    if (-not (Test-Path -LiteralPath $foldTargetsFullPath -PathType Leaf)) {
        $errors.Add("$foldTargetsPath is missing. Run scripts/update-solutions.ps1.")
    }
    else {
        $actualFoldText = [IO.File]::ReadAllText($foldTargetsFullPath).Replace("`r`n", "`n")
        if ($actualFoldText -ne $expectedFoldText) {
            $errors.Add("$foldTargetsPath is stale. Run scripts/update-solutions.ps1.")
        }
    }
}
else {
    [IO.File]::WriteAllText($foldTargetsFullPath, $expectedFoldText, $utf8WithoutBom)
    $foldedReferenceCount = @($foldRewrites.Values | ForEach-Object { $_ }).Count
    Write-Host ("Updated {0} ({1} projects, {2} references folded)." -f
        $foldTargetsPath,
        $foldRewrites.Count,
        $foldedReferenceCount)
}

if ($errors.Count -gt 0) {
    throw "Solution verification failed:`n  $($errors -join "`n  ")"
}

if ($Verify) {
    Write-Host (
        "Verified $($manifest.solutions.Count) focused solutions and $foldTargetsPath " +
        'against eng/solutions.json.')
}
