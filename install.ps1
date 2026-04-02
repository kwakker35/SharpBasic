# install.ps1 — Build and install sharpbasic as a .NET global tool
# Run from the repo root: .\install.ps1

param(
    [switch]$Uninstall
)

$projectPath = Join-Path $PSScriptRoot "src\SharpBasic.Repl\SharpBasic.Repl.csproj"

if ($Uninstall) {
    Write-Host "Uninstalling sharpbasic..."
    dotnet tool uninstall -g SharpBasic
    exit $LASTEXITCODE
}

Write-Host "Building and installing sharpbasic as a .NET global tool..."

# Compute dev version: base version from csproj + short git hash suffix
$baseVersion = ([xml](Get-Content $projectPath)).Project.PropertyGroup.Version
$gitHash = git rev-parse --short HEAD 2>$null
$devVersion = if ($gitHash) { "$baseVersion-dev.$gitHash" } else { $baseVersion }
Write-Host "Version: $devVersion"

# Pack the tool with dev version
dotnet pack $projectPath --configuration Release --output "$PSScriptRoot\artifacts" -v quiet -p:Version=$devVersion
if ($LASTEXITCODE -ne 0) { Write-Error "Pack failed."; exit 1 }

# Install from local package — uninstall first so version ordering never blocks an update
$nupkg = Get-ChildItem "$PSScriptRoot\artifacts\SharpBasic.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $nupkg) { Write-Error "No .nupkg found in artifacts\."; exit 1 }

dotnet tool uninstall -g SharpBasic 2>$null   # silent — fine if not yet installed
dotnet tool install  -g SharpBasic --add-source "$PSScriptRoot\artifacts" --version $devVersion

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Done! Run 'sharpbasic' to start the REPL, or 'sharpbasic <file.sbx>' to run a program."
} else {
    Write-Error "Installation failed."
    exit 1
}
