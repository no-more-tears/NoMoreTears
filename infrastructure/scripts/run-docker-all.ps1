# Starts every local-dev compose file.
# Each file is run as its own separate `docker compose` invocation.
# Expected location: infrastructure\scripts\run-docker-all.ps1
# Compose files live in: infrastructure\docker\
$ErrorActionPreference = "Stop"

$DockerDir = Join-Path $PSScriptRoot "..\docker"

try {
    Set-Location -Path $DockerDir
} catch {
    Write-Host "Error: failed to change directory to $DockerDir"
    exit 1
}

Write-Host "Starting all services..."

docker compose -f docker-compose.infra.yaml up -d
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: failed to start infra"
    exit $LASTEXITCODE
}

docker compose -f docker-compose.observability.yaml up -d
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: failed to start observability"
    exit $LASTEXITCODE
}

Write-Host "All services started successfully"