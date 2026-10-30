#!/usr/bin/env bash
# Starts the whole local Development environment in one go:
# infra and observability containers, database migrations, then the WebApi in the foreground.
# Usage: infrastructure/scripts/run-development-all.sh
# Credentials come from infrastructure/docker/.env (run generate-env.sh once beforehand).
set -euo pipefail

# shellcheck source=lib/dotnet-host.sh
source "$(dirname "${BASH_SOURCE[0]}")/lib/dotnet-host.sh"

case "${1:-}" in
    -h | --help) info "Usage: $(basename "$0")"; exit 0 ;;
    "") ;;
    *) fail "unknown option '$1' (use --help)" ;;
esac

load_development_env

# --wait blocks until the health checks pass, so the migrations do not race a starting Postgres.
compose_up docker-compose.infra.yaml infra --wait
compose_up docker-compose.observability.yaml observability --wait

run_development_migrations || fail "failed to apply migrations"

export_development_webapi_settings
run_development_host "$WEBAPI_PROJECT" ASPNETCORE_ENVIRONMENT https
