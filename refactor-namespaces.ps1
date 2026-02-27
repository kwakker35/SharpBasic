# ============================================================
# SharpBASIC — Namespace Refactor Script
# Renames Lexer → Lexing, Parser → Parsing, Evaluator → Evaluation
# Run this from a plain PowerShell window with VS Code CLOSED
# ============================================================

$root = "C:\Users\GROVECA\source\repos\Personal\SharpBASIC"
Set-Location $root

# ---- Git setup ----
Write-Host "Setting up branch..." -ForegroundColor Cyan
git checkout main
git branch -D refactor/rename-namespaces 2>$null
git checkout -b refactor/rename-namespaces

# ---- Rename folders ----
Write-Host "Renaming folders..." -ForegroundColor Cyan
Rename-Item "$root\src\SharpBasic.Lexer"             "SharpBasic.Lexing"
Rename-Item "$root\src\SharpBasic.Parser"            "SharpBasic.Parsing"
Rename-Item "$root\src\SharpBasic.Evaluator"         "SharpBasic.Evaluation"
Rename-Item "$root\tests\SharpBasic.Lexer.Tests"     "SharpBasic.Lexing.Tests"
Rename-Item "$root\tests\SharpBasic.Parser.Tests"    "SharpBasic.Parsing.Tests"
Rename-Item "$root\tests\SharpBasic.Evaluator.Tests" "SharpBasic.Evaluation.Tests"

# ---- Rename .csproj files ----
Write-Host "Renaming .csproj files..." -ForegroundColor Cyan
Rename-Item "$root\src\SharpBasic.Lexing\SharpBasic.Lexer.csproj"                        "SharpBasic.Lexing.csproj"
Rename-Item "$root\src\SharpBasic.Parsing\SharpBasic.Parser.csproj"                      "SharpBasic.Parsing.csproj"
Rename-Item "$root\src\SharpBasic.Evaluation\SharpBasic.Evaluator.csproj"                "SharpBasic.Evaluation.csproj"
Rename-Item "$root\tests\SharpBasic.Lexing.Tests\SharpBasic.Lexer.Tests.csproj"          "SharpBasic.Lexing.Tests.csproj"
Rename-Item "$root\tests\SharpBasic.Parsing.Tests\SharpBasic.Parser.Tests.csproj"        "SharpBasic.Parsing.Tests.csproj"
Rename-Item "$root\tests\SharpBasic.Evaluation.Tests\SharpBasic.Evaluator.Tests.csproj"  "SharpBasic.Evaluation.Tests.csproj"

# ---- Helper: replace text in all .cs and .csproj files ----
function ReplaceInFiles($pattern, $replacement, $fileFilter) {
    Get-ChildItem $root -Recurse -Filter $fileFilter |
        Where-Object { $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\bin\\" } |
        ForEach-Object {
            $content = Get-Content $_.FullName -Raw
            if ($content -match [regex]::Escape($pattern)) {
                $content = $content.Replace($pattern, $replacement)
                Set-Content $_.FullName $content -NoNewline
                Write-Host "  Updated: $($_.FullName)" -ForegroundColor Gray
            }
        }
}

# ---- Update namespace declarations in .cs files ----
Write-Host "Updating namespaces in .cs files..." -ForegroundColor Cyan
ReplaceInFiles "namespace SharpBasic.Lexer"            "namespace SharpBasic.Lexing"           "*.cs"
ReplaceInFiles "namespace SharpBasic.Parser"           "namespace SharpBasic.Parsing"          "*.cs"
ReplaceInFiles "namespace SharpBasic.Evaluator"        "namespace SharpBasic.Evaluation"       "*.cs"

# ---- Update using statements in .cs files ----
Write-Host "Updating using statements in .cs files..." -ForegroundColor Cyan
ReplaceInFiles "using SharpBasic.Lexer;"               "using SharpBasic.Lexing;"              "*.cs"
ReplaceInFiles "using SharpBasic.Parser;"              "using SharpBasic.Parsing;"             "*.cs"
ReplaceInFiles "using SharpBasic.Evaluator;"           "using SharpBasic.Evaluation;"          "*.cs"

# ---- Update ProjectReference paths in .csproj files ----
Write-Host "Updating ProjectReferences in .csproj files..." -ForegroundColor Cyan
ReplaceInFiles "SharpBasic.Lexer\SharpBasic.Lexer.csproj"                "SharpBasic.Lexing\SharpBasic.Lexing.csproj"               "*.csproj"
ReplaceInFiles "SharpBasic.Parser\SharpBasic.Parser.csproj"              "SharpBasic.Parsing\SharpBasic.Parsing.csproj"             "*.csproj"
ReplaceInFiles "SharpBasic.Evaluator\SharpBasic.Evaluator.csproj"        "SharpBasic.Evaluation\SharpBasic.Evaluation.csproj"       "*.csproj"

# ---- Update .sln file ----
Write-Host "Updating .sln file..." -ForegroundColor Cyan
$slnPath = "$root\SharpBasic.sln"
$sln = Get-Content $slnPath -Raw
$sln = $sln.Replace('"SharpBasic.Lexer"',              '"SharpBasic.Lexing"')
$sln = $sln.Replace('"SharpBasic.Parser"',             '"SharpBasic.Parsing"')
$sln = $sln.Replace('"SharpBasic.Evaluator"',          '"SharpBasic.Evaluation"')
$sln = $sln.Replace('"SharpBasic.Lexer.Tests"',        '"SharpBasic.Lexing.Tests"')
$sln = $sln.Replace('"SharpBasic.Parser.Tests"',       '"SharpBasic.Parsing.Tests"')
$sln = $sln.Replace('"SharpBasic.Evaluator.Tests"',    '"SharpBasic.Evaluation.Tests"')
$sln = $sln.Replace('src\SharpBasic.Lexer\SharpBasic.Lexer.csproj',                       'src\SharpBasic.Lexing\SharpBasic.Lexing.csproj')
$sln = $sln.Replace('src\SharpBasic.Parser\SharpBasic.Parser.csproj',                     'src\SharpBasic.Parsing\SharpBasic.Parsing.csproj')
$sln = $sln.Replace('src\SharpBasic.Evaluator\SharpBasic.Evaluator.csproj',               'src\SharpBasic.Evaluation\SharpBasic.Evaluation.csproj')
$sln = $sln.Replace('tests\SharpBasic.Lexer.Tests\SharpBasic.Lexer.Tests.csproj',         'tests\SharpBasic.Lexing.Tests\SharpBasic.Lexing.Tests.csproj')
$sln = $sln.Replace('tests\SharpBasic.Parser.Tests\SharpBasic.Parser.Tests.csproj',       'tests\SharpBasic.Parsing.Tests\SharpBasic.Parsing.Tests.csproj')
$sln = $sln.Replace('tests\SharpBasic.Evaluator.Tests\SharpBasic.Evaluator.Tests.csproj', 'tests\SharpBasic.Evaluation.Tests\SharpBasic.Evaluation.Tests.csproj')
Set-Content $slnPath $sln -NoNewline

# ---- Fix EvalError.cs — missing namespace declaration ----
Write-Host "Fixing EvalError.cs namespace..." -ForegroundColor Cyan
$evalErrorPath = "$root\src\SharpBasic.Evaluation\EvalError.cs"
$content = Get-Content $evalErrorPath -Raw
if ($content -notmatch "namespace SharpBasic.Evaluation") {
    $content = $content.Replace(
        "public record EvalError",
        "namespace SharpBasic.Evaluation;`n`npublic record EvalError"
    )
    # Remove the stray using that was there
    $content = $content.Replace("using SharpBasic.Evaluation;`n", "")
    Set-Content $evalErrorPath $content -NoNewline
    Write-Host "  Fixed EvalError.cs" -ForegroundColor Gray
}

# ---- Build and test ----
Write-Host "Building and testing..." -ForegroundColor Cyan
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "BUILD FAILED — fix errors before proceeding" -ForegroundColor Red
    exit 1
}
dotnet test
if ($LASTEXITCODE -ne 0) {
    Write-Host "TESTS FAILED — fix errors before proceeding" -ForegroundColor Red
    exit 1
}

# ---- Commit refactor to branch ----
Write-Host "Committing refactor..." -ForegroundColor Cyan
git add -A
git commit -m "refactor: rename Lexer→Lexing, Parser→Parsing, Evaluator→Evaluation namespaces"
git push origin refactor/rename-namespaces

# ---- Merge to main ----
Write-Host "Merging to main..." -ForegroundColor Cyan
git checkout main
git merge refactor/rename-namespaces
git push origin main

# ---- Update test/run-helper branch ----
Write-Host "Updating test/run-helper with renames..." -ForegroundColor Cyan
git checkout test/run-helper
git merge main
Write-Host ""
Write-Host "Done. Now fix RunHelper.cs usings (SharpBasic.Lexing / SharpBasic.Parsing) then run dotnet test." -ForegroundColor Green
