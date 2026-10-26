<#
.SYNOPSIS
    Build, test and push Broiler.SDK CI container images.
.DESCRIPTION
    Builds the base, browser, and office CI containers for GitHub Container Registry.
.PARAMETER Target
    The target image to build: 'all', 'base', 'browser', or 'office'. Default is 'all'.
.PARAMETER Registry
    Container registry host. Default is 'ghcr.io'.
.PARAMETER Owner
    Organization or repository owner. Default is 'broiler-platform'.
.PARAMETER Tag
    Image tag. Default is 'latest'.
.PARAMETER Push
    Switch to push images to registry after build.
.PARAMETER Test
    Switch to run verification commands inside the images.
.EXAMPLE
    .\build.ps1 -Target all -Test
    .\build.ps1 -Target base -Push
#>
[CmdletBinding()]
param(
    [ValidateSet('all', 'base', 'browser', 'office')]
    [string]$Target = 'all',

    [string]$Registry = $(if ($env:REGISTRY) { $env:REGISTRY } else { 'ghcr.io' }),
    [string]$Owner = $(if ($env:OWNER) { $env:OWNER } else { 'broiler-platform' }),
    [string]$Tag = $(if ($env:TAG) { $env:TAG } else { 'latest' }),

    [switch]$Push,
    [switch]$Test
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$BaseImage = "$Registry/$Owner/broiler-sdk-base:$Tag"
$BrowserImage = "$Registry/$Owner/broiler-sdk-browser:$Tag"
$OfficeImage = "$Registry/$Owner/broiler-sdk-office:$Tag"

function Build-Base {
    Write-Host "==> Building base image: $BaseImage" -ForegroundColor Cyan
    docker build `
        -t $BaseImage `
        -f (Join-Path $ScriptDir "base/Dockerfile") `
        (Join-Path $ScriptDir "base")

    if ($Test) {
        Write-Host "==> Testing base image..." -ForegroundColor Yellow
        docker run --rm $BaseImage dotnet --info
    }

    if ($Push) {
        Write-Host "==> Pushing base image: $BaseImage" -ForegroundColor Green
        docker push $BaseImage
    }
}

function Build-Browser {
    Write-Host "==> Building browser image: $BrowserImage" -ForegroundColor Cyan
    docker build `
        --build-arg BASE_IMAGE=$BaseImage `
        -t $BrowserImage `
        -f (Join-Path $ScriptDir "browser/Dockerfile") `
        (Join-Path $ScriptDir "browser")

    if ($Test) {
        Write-Host "==> Testing browser image..." -ForegroundColor Yellow
        docker run --rm $BrowserImage bash -c "node --version && npx playwright --version && test -d /opt/test262/test && test -d /opt/wpt/css"
    }

    if ($Push) {
        Write-Host "==> Pushing browser image: $BrowserImage" -ForegroundColor Green
        docker push $BrowserImage
    }
}

function Build-Office {
    Write-Host "==> Building office image: $OfficeImage" -ForegroundColor Cyan
    docker build `
        --build-arg BASE_IMAGE=$BaseImage `
        -t $OfficeImage `
        -f (Join-Path $ScriptDir "office/Dockerfile") `
        (Join-Path $ScriptDir "office")

    if ($Test) {
        Write-Host "==> Testing office image..." -ForegroundColor Yellow
        docker run --rm $OfficeImage bash -c "soffice --version && pdftoppm -v 2>&1 | head -n 1 && test -d /opt/libreoffice-extras"
    }

    if ($Push) {
        Write-Host "==> Pushing office image: $OfficeImage" -ForegroundColor Green
        docker push $OfficeImage
    }
}

switch ($Target) {
    'base'    { Build-Base }
    'browser' { Build-Browser }
    'office'  { Build-Office }
    'all'     {
        Build-Base
        Build-Browser
        Build-Office
    }
}

Write-Host "==> Build completed successfully." -ForegroundColor Green
