#!/usr/bin/env bash
# Runs the entire local Development stack sequentially in labeled phases:
#   Phase 1: Ensure infrastructure/docker/.env exists
#   Phase 2: Core infrastructure (Docker, wait until healthy)
#   Phase 3: Observability stack (Docker, wait until healthy)
#   Phase 4: Database migrations
#   Phase 5: Build all .NET hosts
#   Phase 6: Start consumers, SignalR and WebApi in parallel
#
# Usage: infrastructure/scripts/run-all.sh [--docker-consumers]
set -euo pipefail

pause_on_exit() {
    if [[ -t 0 ]]; then
        echo
        read -n 1 -s -r -p "Press any key to exit..." || true
        echo
    fi
}
trap pause_on_exit EXIT

SCRIPT_DIR="$(dirname "${BASH_SOURCE[0]}")"

# shellcheck source=lib/dotnet-host.sh
source "$SCRIPT_DIR/lib/dotnet-host.sh"

DOCKER_CONSUMERS=false

case "${1:-}" in
    "") ;;
    --docker-consumers) DOCKER_CONSUMERS=true ;;
    -h | --help) info "Usage: $(basename "$0") [--docker-consumers]"; exit 0 ;;
    *) fail "unknown option '$1' (use --help)" ;;
esac

TOTAL_PHASES=6

phase() {
    local number="$1"
    local title="$2"
    echo
    info "Phase $number/$TOTAL_PHASES: $title"
}

phase 1 "Environment configuration (.env)"
ENV_FILE="$DOCKER_DIR/.env"
if [[ -f "$ENV_FILE" ]]; then
    info ".env already exists ($ENV_FILE), skipping generation."
else
    info ".env not found, running generate-env.sh..."
    # Redirect stdin from /dev/null so generate-env.sh does not pause mid-pipeline.
    "$SCRIPT_DIR/generate-env.sh" < /dev/null
fi
load_development_env

phase 2 "Core infrastructure (Docker)"
compose_up docker-compose.infra.yaml infra --wait

phase 3 "Observability stack (Docker)"
compose_up docker-compose.observability.yaml observability --wait

phase 4 "Database migrations"
run_development_migrations || fail "failed to apply migrations"

require_command dotnet

if [[ "$DOCKER_CONSUMERS" == true ]]; then
    phase 5 "Consumers stack (Docker)"
    compose_up docker-compose.consumers.yaml consumers --build

    phase 6 "WebApi host"
    export_development_webapi_settings
    run_development_host "$WEBAPI_PROJECT" ASPNETCORE_ENVIRONMENT https
    exit 0
fi

phase 5 "Building .NET hosts"
PROJECTS=(
    "$MASTERS_CONSUMERS_PROJECT"
    "$REQUESTS_CONSUMERS_PROJECT"
    "$REQUESTS_BACKGROUND_JOBS_PROJECT"
    "$NOTIFICATION_SIGNALR_PROJECT"
    "$WEBAPI_PROJECT"
)

for project in "${PROJECTS[@]}"; do
    info "Building $(basename "$project" .csproj)..."
    dotnet build "$SRC_DIR/$project" --nologo -v q || fail "failed to build $project"
done

phase 6 "Starting consumers, SignalR and WebApi"
info "Press Ctrl+C to stop all hosts."

pids=()

stop_hosts() {
    trap - INT TERM EXIT
    (( ${#pids[@]} > 0 )) && kill "${pids[@]}" 2>/dev/null || true
    wait 2>/dev/null || true
    pause_on_exit
}
trap stop_hosts EXIT
trap 'exit 130' INT
trap 'exit 143' TERM

(
    export_development_masters_consumers_settings
    run_host "$MASTERS_CONSUMERS_PROJECT" DOTNET_ENVIRONMENT Development --no-build --no-launch-profile
) &
pids+=($!)

(
    export_development_requests_consumers_settings
    run_host "$REQUESTS_CONSUMERS_PROJECT" DOTNET_ENVIRONMENT Development --no-build --no-launch-profile
) &
pids+=($!)

(
    export_development_requests_background_jobs_settings
    run_host "$REQUESTS_BACKGROUND_JOBS_PROJECT" DOTNET_ENVIRONMENT Development --no-build --no-launch-profile
) &
pids+=($!)

(
    export_development_notification_signalr_settings
    run_host "$NOTIFICATION_SIGNALR_PROJECT" ASPNETCORE_ENVIRONMENT Development --no-build \
        --launch-profile NoMoreTears.Modules.Notification.HostSide.SignalR
) &
pids+=($!)

(
    export_development_webapi_settings
    run_host "$WEBAPI_PROJECT" ASPNETCORE_ENVIRONMENT Development --no-build --launch-profile https
) &
pids+=($!)

# Stop everything as soon as one host exits.
wait -n
status=$?
info "A host exited (code $status), stopping the others..."
exit "$status"