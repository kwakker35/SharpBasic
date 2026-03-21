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

# Pack the tool
dotnet pack $projectPath --configuration Release --output "$PSScriptRoot\artifacts" -v quiet
if ($LASTEXITCODE -ne 0) { Write-Error "Pack failed."; exit 1 }

# Install (or reinstall) from local package
$nupkg = Get-ChildItem "$PSScriptRoot\artifacts\SharpBasic.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $nupkg) { Write-Error "No .nupkg found in artifacts\."; exit 1 }

dotnet tool install -g SharpBasic --add-source "$PSScriptRoot\artifacts" --version $nupkg.BaseName.Replace("SharpBasic.", "")
if ($LASTEXITCODE -ne 0) {
    # Already installed — update instead
    dotnet tool update -g SharpBasic --add-source "$PSScriptRoot\artifacts"
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Done! Run 'sharpbasic' to start the REPL, or 'sharpbasic <file.bas>' to run a program."
} else {
    Write-Error "Installation failed."
    exit 1
}
