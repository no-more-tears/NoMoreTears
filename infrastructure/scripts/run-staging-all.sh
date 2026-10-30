#!/usr/bin/env bash
# Runs the Staging environment in one go (Release build): database migrations, then the WebApi in the foreground.
# Usage: infrastructure/scripts/run-staging-all.sh [--env-file <path>]
#
# Nothing is taken from infrastructure/docker/.env: every setting has to come from the process
# environment or from --env-file.
set -euo pipefail

# shellcheck source=lib/dotnet-host.sh
source "$(dirname "${BASH_SOURCE[0]}")/lib/dotnet-host.sh"

usage() {
    cat <<EOF
Usage: $(basename "$0") [--env-file <path>]

Required variables:
  ConnectionStrings__Postgres
  RedisCache__ConnectionString, RedisCache__Password
  Identity__Jwt__SigningKey
  Identity__Links__EmailConfirmationUrl, Identity__Links__PasswordResetUrl
  Notification__Smtp__Host, Notification__Smtp__SenderAddress
Optional:
  ASPNETCORE_URLS (default http://+:8080)
EOF
}

while (( $# > 0 )); do
    case "$1" in
        --env-file)
            [[ -n "${2:-}" ]] || fail "--env-file requires a path"
            load_env_file "$2"
            shift 2
            ;;
        -h | --help) usage; exit 0 ;;
        *) fail "unknown option '$1' (use --help)" ;;
    esac
done

# Checked before building, so a missing setting does not cost a full build.
require_vars \
    ConnectionStrings__Postgres \
    RedisCache__ConnectionString \
    RedisCache__Password \
    Identity__Jwt__SigningKey \
    Identity__Links__EmailConfirmationUrl \
    Identity__Links__PasswordResetUrl \
    Notification__Smtp__Host \
    Notification__Smtp__SenderAddress

# Same port as the container image.
export ASPNETCORE_URLS="${ASPNETCORE_URLS:-http://+:8080}"

run_staging_host "$MIGRATIONS_PROJECT" DOTNET_ENVIRONMENT || fail "failed to apply migrations"
run_staging_host "$WEBAPI_PROJECT" ASPNETCORE_ENVIRONMENT
