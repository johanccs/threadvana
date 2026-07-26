# ThreadCraft Academy — Setup & Launch
# ======================================
# One script: check prerequisites, build, verify, launch.
#
# Usage:
#   .\setup.ps1                  # Build and launch (default)
#   .\setup.ps1 -Clean           # Rebuild from scratch, then launch
#   .\setup.ps1 -NoBrowser       # Launch without opening browser
#   .\setup.ps1 -Publish         # Publish for production (no launch)

param(
    [switch]$Clean,
    [switch]$NoBrowser,
    [switch]$Publish
)

$ErrorActionPreference = "Stop"
$RepoRoot = $PSScriptRoot
Set-Location $RepoRoot

$SlnPath = Join-Path $RepoRoot "ThreadCraft.slnx"
$WebProj = Join-Path $RepoRoot "src\ThreadCraft.Web\ThreadCraft.Web.csproj"
$ContentRoot = Join-Path $RepoRoot "content\lessons"
$AppSettings = Join-Path $RepoRoot "src\ThreadCraft.Web\appsettings.json"
$Port = 5080

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  ThreadCraft Academy — Setup & Launch" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan

# ---- 1. .NET SDK -------------------------------------------------------
Write-Host "`n[1/5] Checking .NET SDK..." -ForegroundColor Yellow
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Host "  .NET SDK not found. Install .NET 8+ from:" -ForegroundColor Red
    Write-Host "  https://dotnet.microsoft.com/en-us/download" -ForegroundColor Red
    exit 1
}
$sdkVersion = & dotnet --version 2>$null
if ([Version]$sdkVersion -lt [Version]"8.0.0") {
    Write-Host "  .NET SDK $sdkVersion is too old (need 8.0+)." -ForegroundColor Red
    exit 1
}
Write-Host "  .NET SDK $sdkVersion — OK" -ForegroundColor Green

# ---- 2. Content --------------------------------------------------------
Write-Host "`n[2/5] Checking content..." -ForegroundColor Yellow
if (-not (Test-Path $ContentRoot)) {
    Write-Host "  ERROR: content/lessons not found at $ContentRoot" -ForegroundColor Red
    exit 1
}
$lessonCount = (Get-ChildItem $ContentRoot -Recurse -Filter lesson.md).Count
$catCount = (Get-ChildItem $ContentRoot -Directory).Count
Write-Host "  Found $lessonCount lessons across $catCount categories — OK" -ForegroundColor Green

# ---- 3. Build ----------------------------------------------------------
if ($Clean) {
    Write-Host "`n[3/5] Cleaning..." -ForegroundColor Yellow
    & dotnet clean $SlnPath --nologo -v q 2>&1 | Out-Null
    Write-Host "  Clean complete." -ForegroundColor Green
}

Write-Host "`n[3/5] Building solution..." -ForegroundColor Yellow
$buildOutput = & dotnet build $SlnPath --configuration Release --nologo 2>&1
$errors = $buildOutput | Where-Object { $_ -match "^.*error CS\d+" }
if ($LASTEXITCODE -ne 0 -or $errors) {
    Write-Host "  Build FAILED:" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
    exit 1
}
Write-Host "  Build succeeded." -ForegroundColor Green

# ---- 4. API Key --------------------------------------------------------
Write-Host "`n[4/5] Checking AI coach key..." -ForegroundColor Yellow
$apiKey = $null
if (Test-Path $AppSettings) {
    $json = Get-Content $AppSettings -Raw | ConvertFrom-Json
    $apiKey = $json.Assistant.ApiKey
}
if (-not $apiKey) { $apiKey = $env:OPENROUTER_API_KEY }
if ($apiKey) {
    Write-Host "  API key found." -ForegroundColor Green
} else {
    Write-Host "  No OpenRouter API key found." -ForegroundColor DarkYellow
    Write-Host "  Set OPENROUTER_API_KEY env var or add to appsettings.json." -ForegroundColor DarkYellow
    Write-Host "  The AI coach panel will show setup instructions until configured." -ForegroundColor DarkYellow
}

# ---- 5. Launch / Publish -----------------------------------------------
if ($Publish) {
    Write-Host "`n[5/5] Publishing..." -ForegroundColor Yellow
    $publishDir = Join-Path $RepoRoot "publish"
    & dotnet publish $WebProj --configuration Release --output $publishDir --nologo 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Publish FAILED." -ForegroundColor Red
        exit 1
    }
    Write-Host "  Published to: $publishDir" -ForegroundColor Green
    Write-Host "  Run with: dotnet $publishDir\ThreadCraft.Web.dll" -ForegroundColor Cyan
    exit 0
}

Write-Host "`n[5/5] Starting server..." -ForegroundColor Yellow
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://localhost:$Port"

Write-Host "  Launching on http://localhost:$Port ..." -ForegroundColor Green
Write-Host "  Press Ctrl+C to stop.`n" -ForegroundColor DarkGray

Push-Location (Split-Path $WebProj)
try {
    if (-not $NoBrowser) { Start-Process "http://localhost:$Port" }
    & dotnet run --configuration Release --no-build --urls "http://localhost:$Port"
} finally {
    Pop-Location
}