# Starts only the core infrastructure.
# Expected location: infrastructure\scripts\run-docker-infra.ps1
# Compose files live in: infrastructure\docker\
$ErrorActionPreference = "Stop"

$DockerDir = Join-Path $PSScriptRoot "..\docker"

try {
    Set-Location -Path $DockerDir
} catch {
    Write-Host "Error: failed to change directory to $DockerDir"
    exit 1
}

Write-Host "Starting infra..."
docker compose -f docker-compose.infra.yaml up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "Infra started successfully"
} else {
    Write-Host "Error: failed to start infra"
    exit $LASTEXITCODE
}