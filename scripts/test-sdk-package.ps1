# Installs the actual nupkg into an isolated tool path and exercises its dependency closure.
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $PackagePath
)

$ErrorActionPreference = 'Stop'
$package = (Resolve-Path -LiteralPath $PackagePath).Path
$archive = [System.IO.Compression.ZipFile]::OpenRead($package)
try {
    $entry = @($archive.Entries | Where-Object FullName -Like '*.nuspec')
    if ($entry.Count -ne 1) { throw 'Expected one package manifest.' }
    $reader = [System.IO.StreamReader]::new($entry[0].Open())
    try { [xml] $manifest = $reader.ReadToEnd() } finally { $reader.Dispose() }
    $id = $manifest.package.metadata.id
    $version = $manifest.package.metadata.version
    if ($id -ne 'Broiler.SDK') { throw "Expected Broiler.SDK, got $id." }
} finally {
    $archive.Dispose()
}

$tempRoot = [System.IO.Path]::GetFullPath([System.IO.Path]::GetTempPath())
$scratch = Join-Path $tempRoot ('broiler-sdk-test-' + [guid]::NewGuid().ToString('N'))
$oldPackages = $env:NUGET_PACKAGES
New-Item -ItemType Directory -Path $scratch | Out-Null
Push-Location $scratch
try {
    # Neither an already-cached package nor a remote feed may satisfy this install.
    $env:NUGET_PACKAGES = Join-Path $scratch 'cache'
    $feed = Join-Path $scratch 'feed'
    New-Item -ItemType Directory -Path $feed | Out-Null
    Copy-Item -LiteralPath $package -Destination $feed
    '<configuration><packageSources><clear/><add key="test" value="./feed"/></packageSources><packageSourceMapping><clear/></packageSourceMapping></configuration>' |
        Set-Content NuGet.config
    dotnet tool install Broiler.SDK --version $version --tool-path ./tools --configfile ./NuGet.config
    if ($LASTEXITCODE -ne 0) { throw 'Package installation failed.' }

    $tool = Join-Path $scratch $(if ($IsWindows) { 'tools/broiler-review.exe' } else { 'tools/broiler-review' })
    & $tool --help
    if ($LASTEXITCODE -ne 0) { throw 'Tool help failed.' }

    New-Item -ItemType Directory -Path fixture | Out-Null
    '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' |
        Set-Content fixture/Sample.csproj
    'public static class Sample { public static int Twice(int value) => value * 2; }' |
        Set-Content fixture/Sample.cs
    '{"schema":1,"component":"SDK smoke test","projects":["Sample.csproj"]}' |
        Set-Content fixture/assurance.config.json

    & $tool coverage --root ./fixture --json coverage.json
    if ($LASTEXITCODE -ne 0) { throw 'Coverage failed.' }
    $coverage = Get-Content coverage.json -Raw | ConvertFrom-Json
    if ($coverage.overall.total -ne 1 -or $coverage.overall.unreviewed -ne 1) {
        throw 'Coverage did not find the unreviewed fixture.'
    }
    $expectedFailure = & $tool coverage --root ./fixture --minimum 100 2>&1 | Out-String
    if ($LASTEXITCODE -ne 1) { throw 'Coverage did not preserve its failing exit code.' }

    & $tool assurance list --root ./fixture --all-units --json units.json
    if ($LASTEXITCODE -ne 0) { throw 'Assurance scan failed.' }
    $assurance = Get-Content units.json -Raw | ConvertFrom-Json
    if ($assurance.totals.files -ne 1 -or $assurance.totals.unreadable -ne 0 -or
        -not ($assurance.units | Where-Object unit -Like '*Twice*')) {
        throw 'Assurance did not scan the fixture method.'
    }
    Write-Host "Broiler.SDK $version installed and passed review and assurance smoke tests."
} finally {
    $env:NUGET_PACKAGES = $oldPackages
    Pop-Location
    $resolvedScratch = (Resolve-Path -LiteralPath $scratch).Path
    if ([System.IO.Path]::GetDirectoryName($resolvedScratch) -ne $tempRoot.TrimEnd('\', '/') -or
        [System.IO.Path]::GetFileName($resolvedScratch) -notlike 'broiler-sdk-test-*') {
        throw "Refusing to remove unexpected scratch path: $resolvedScratch"
    }
    Remove-Item -LiteralPath $resolvedScratch -Recurse -Force
}
