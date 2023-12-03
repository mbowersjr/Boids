Param (
    [Parameter(Mandatory=$false)]
    [Switch]$DryRun
)

$solutionDirectory = Join-Path -Path $PSScriptRoot -ChildPath "src"
Write-Host "Cleaning build output directories under $solutionDirectory..."

$binDirectories = Get-ChildItem -Path $solutionDirectory -Recurse -Directory -Filter "bin"
Write-Host "$($binDirectories.Count) ""bin"" directories found ..."

foreach ($dir in $binDirectories) {
    Write-Host " - $($dir.FullName)"

    if ($DryRun) {
        Remove-Item -Path $dir.FullName -Recurse -Force -WhatIf
    }
    else {
        Remove-Item -Path $dir.FullName -Recurse -Force
    }
}

$objDirectories = Get-ChildItem -Path $solutionDirectory -Recurse -Directory -Filter "obj"
Write-Host "$($objDirectories.Count) ""obj"" directories found ..."

foreach ($dir in $objDirectories) {
    Write-Host " - $($dir.FullName)"

    if ($DryRun) {
        Remove-Item -Path $dir.FullName -Recurse -Force -WhatIf
    }
    else {
        Remove-Item -Path $dir.FullName -Recurse -Force
    }
}

$buildDirectory = Join-Path -Path $PSScriptRoot -ChildPath "build"
if (Test-Path -Path $buildDirectory) {
    Write-Host "Cleaning build directory $buildDirectory..."

    if ($DryRun) {
        Remove-Item -Path $buildDirectory -Recurse -Force -WhatIf
    }
    else {
        Remove-Item -Path $buildDirectory -Recurse -Force
    }
}

$publishDirectory = Join-Path -Path $PSScriptRoot -ChildPath "publish"
if (Test-Path -Path $publishDirectory) {
    Write-Host "Cleaning publish directory $publishDirectory..."

    if ($DryRun) {
        Remove-Item -Path $publishDirectory -Recurse -Force -WhatIf
    }
    else {
        Remove-Item -Path $publishDirectory -Recurse -Force
    }
}