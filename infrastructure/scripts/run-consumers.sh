#!/usr/bin/env bash
# Runs every asynchronous host at once: the Masters and Requests consumers, the Requests background jobs
# and the Notification SignalR host.
# Usage: infrastructure/scripts/run-consumers.sh [--docker]
#
#   (no option)  Development: the hosts run with 'dotnet run' in parallel; Ctrl+C stops all of them,
#                and if one host stops, the others are stopped too. Credentials come from infrastructure/docker/.env.
#   --docker     Builds and starts docker-compose.consumers.yaml in the background.
#
# Requires docker-compose.infra.yaml to be running and the migrations to be applied (run-migrations.sh).
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
    -h | --help) info "Usage: $(basename "$0") [--docker]"; exit 0 ;;
    --docker) compose_up docker-compose.consumers.yaml consumers --build; exit 0 ;;
    "") ;;
    *) fail "unknown option '$1' (use --help)" ;;
esac

load_development_env
require_command dotnet

# Both hosts share projects, so they are built once up front instead of by two concurrent 'dotnet run'.
for project in "$MASTERS_CONSUMERS_PROJECT" "$REQUESTS_CONSUMERS_PROJECT" "$REQUESTS_BACKGROUND_JOBS_PROJECT" "$NOTIFICATION_SIGNALR_PROJECT"; do
    info "Building $(basename "$project" .csproj)..."
    dotnet build "$SRC_DIR/$project" --nologo -v q || fail "failed to build $project"
done

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

# Stop everything as soon as one host exits.
wait -n
status=$?
info "A consumer host exited (code $status), stopping the others..."
exit "$status"