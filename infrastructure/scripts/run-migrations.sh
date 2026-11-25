#!/usr/bin/env bash
# Applies the database migrations in Development and exits.
# Usage: infrastructure/scripts/run-migrations.sh [-- app args]
#
# Requires docker-compose.infra.yaml to be running (Postgres on localhost).
# Credentials come from infrastructure/docker/.env.
set -euo pipefail

pause_on_exit() {
    if [[ -t 0 ]]; then
        echo
        read -n 1 -s -r -p "Press any key to exit..." || true
        echo
    fi
}
trap pause_on_exit EXIT

# shellcheck source=lib/dotnet-host.sh
source "$(dirname "${BASH_SOURCE[0]}")/lib/dotnet-host.sh"

case "${1:-}" in
    -h | --help) info "Usage: $(basename "$0") [-- app args]"; exit 0 ;;
    --) shift ;;
    "") ;;
    *) fail "unknown option '$1' (use --help)" ;;
esac

load_development_env
run_development_migrations "$@"