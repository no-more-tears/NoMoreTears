# Starts only the observability stack.
# Expected location: infrastructure\scripts\run-docker-observability.ps1
# Compose files live in: infrastructure\docker\
$ErrorActionPreference = "Stop"

$DockerDir = Join-Path $PSScriptRoot "..\docker"

try {
    Set-Location -Path $DockerDir
} catch {
    Write-Host "Error: failed to change directory to $DockerDir"
    exit 1
}

Write-Host "Starting observability..."
docker compose -f docker-compose.observability.yaml up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "Observability started successfully"
} else {
    Write-Host "Error: failed to start observability"
    exit $LASTEXITCODE
}