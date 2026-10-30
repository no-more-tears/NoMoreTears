#!/usr/bin/env bash
# Runs the WebApi host in Development with the 'https' launch profile.
# Usage: infrastructure/scripts/run-webapi.sh [-- app args]
#
# Requires docker-compose.infra.yaml and docker-compose.observability.yaml to be running
# and the migrations to be applied (run-migrations.sh). Credentials come from infrastructure/docker/.env.
set -euo pipefail

# shellcheck source=lib/dotnet-host.sh
source "$(dirname "${BASH_SOURCE[0]}")/lib/dotnet-host.sh"

case "${1:-}" in
    -h | --help) info "Usage: $(basename "$0") [-- app args]"; exit 0 ;;
    --) shift ;;
    "") ;;
    *) fail "unknown option '$1' (use --help)" ;;
esac

load_development_env
export_development_webapi_settings
run_development_host "$WEBAPI_PROJECT" ASPNETCORE_ENVIRONMENT https "$@"
