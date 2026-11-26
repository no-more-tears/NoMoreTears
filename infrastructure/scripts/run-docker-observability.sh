#!/usr/bin/env bash
# Starts only the observability stack.
# Compose files live in: infrastructure/docker/
set -euo pipefail

pause_on_exit() {
    if [[ -t 0 ]]; then
        echo
        read -n 1 -s -r -p "Press any key to exit..." || true
        echo
    fi
}
trap pause_on_exit EXIT

# shellcheck source=lib/common.sh
source "$(dirname "${BASH_SOURCE[0]}")/lib/common.sh"

compose_up docker-compose.observability.yaml observability