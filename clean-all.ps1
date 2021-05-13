Param (
    [Parameter(Mandatory=$false)]
    [Switch]$DryRun
)    

$solutionDirectory = $PSScriptRoot
Write-Host "Clean solution build artifacts in $solutionDirectory ..."

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
