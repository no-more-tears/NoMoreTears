#!/usr/bin/env bash
# Shared helpers for the infrastructure scripts. Source it, do not execute it.
# Expected location: infrastructure/scripts/lib/common.sh

SCRIPTS_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
REPO_ROOT="$(cd "$SCRIPTS_DIR/../.." && pwd)"
DOCKER_DIR="$REPO_ROOT/infrastructure/docker"
SRC_DIR="$REPO_ROOT/src"
DEFAULT_ENV_FILE="$DOCKER_DIR/.env"

info() {
    printf '%s\n' "$*"
}

fail() {
    printf 'Error: %s\n' "$*" >&2
    exit 1
}

require_command() {
    command -v "$1" >/dev/null 2>&1 || fail "'$1' is not installed or not on PATH"
}

# Exports every KEY=VALUE line of a dotenv file literally. The file is parsed rather than sourced,
# because connection strings and URLs contain ';' and '&', which the shell would execute.
# Carriage returns are stripped, so a file edited on Windows does not leave '\r' in the values.
load_env_file() {
    local file="$1"
    local line
    local key
    local value
    local number=0

    [[ -f "$file" ]] || fail "env file not found: $file"

    while IFS= read -r line || [[ -n "$line" ]]; do
        number=$((number + 1))
        line="${line%$'\r'}"
        [[ "$line" =~ ^[[:space:]]*(#|$) ]] && continue

        key="${line%%=*}"
        key="${key#export }"
        key="${key//[[:space:]]/}"
        [[ "$line" == *=* && "$key" =~ ^[A-Za-z_][A-Za-z0-9_]*$ ]] || fail "invalid line $number in $file (expected KEY=VALUE)"

        value="${line#*=}"
        # Optional matching quotes around the whole value are removed, nothing inside is interpreted.
        if [[ "$value" =~ ^\"(.*)\"$ || "$value" =~ ^\'(.*)\'$ ]]; then
            value="${BASH_REMATCH[1]}"
        fi

        export "$key=$value"
    done < "$file"
}

# Fails with the full list of missing variables instead of stopping at the first one.
require_vars() {
    local missing=()
    local name

    for name in "$@"; do
        [[ -n "${!name:-}" ]] || missing+=("$name")
    done

    if (( ${#missing[@]} > 0 )); then
        fail "required variables are not set: ${missing[*]}"
    fi
}

# compose_up <compose file> <label> [extra 'up' arguments, e.g. --wait]
compose_up() {
    local file="$1"
    local label="$2"
    shift 2

    require_command docker
    [[ -f "$DOCKER_DIR/$file" ]] || fail "compose file not found: $DOCKER_DIR/$file"

    info "Starting $label..."
    (cd "$DOCKER_DIR" && docker compose -f "$file" up -d "$@") || fail "failed to start $label"
    info "$label started successfully"
}
