#!/usr/bin/env bash
# Starts only the observability stack.
# Compose files live in: infrastructure/docker/
set -euo pipefail

# shellcheck source=lib/common.sh
source "$(dirname "${BASH_SOURCE[0]}")/lib/common.sh"

compose_up docker-compose.observability.yaml observability
