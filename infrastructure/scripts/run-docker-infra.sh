#!/usr/bin/env bash
# Starts only the core infrastructure (postgres, pgadmin, redis, papercut).
# Compose files live in: infrastructure/docker/
set -euo pipefail

# shellcheck source=lib/common.sh
source "$(dirname "${BASH_SOURCE[0]}")/lib/common.sh"

compose_up docker-compose.infra.yaml infra
