#!/usr/bin/env bash
# Runs the .NET host projects (WebApi, migrations). Source it, do not execute it.
# Expected location: infrastructure/scripts/lib/dotnet-host.sh
#
# Development: 'dotnet run' from the source tree, local credentials come from infrastructure/docker/.env.
# Staging: 'dotnet run -c Release' from the source tree; every setting comes from the process
# environment or from --env-file.

# shellcheck source=common.sh
source "$(dirname "${BASH_SOURCE[0]}")/common.sh"

WEBAPI_PROJECT="HostSide/WebApi/NoMoreTears.HostSide.WebApi.csproj"
MIGRATIONS_PROJECT="Modules/Identity/HostSide/Migrations/NoMoreTears.Modules.Identity.HostSide.Migrations.csproj"

# Loads infrastructure/docker/.env unless another file is given.
load_development_env() {
    local file="${1:-$DEFAULT_ENV_FILE}"

    [[ -f "$file" ]] || fail "$file not found, run infrastructure/scripts/generate-env.sh first"
    load_env_file "$file"
}

# Connection string to the Postgres container of docker-compose.infra.yaml, published on localhost.
development_postgres_connection_string() {
    local application_name="$1"

    require_vars POSTGRES_DB POSTGRES_USER POSTGRES_PASSWORD
    printf 'Host=localhost;Port=%s;Database=%s;Username=%s;Password=%s;ApplicationName=%s' \
        "${POSTGRES_PORT:-5434}" "$POSTGRES_DB" "$POSTGRES_USER" "$POSTGRES_PASSWORD" "$application_name"
}

# Values that are already exported win, so a single setting can be overridden from the shell.
export_development_webapi_settings() {
    require_vars REDIS_PASSWORD JWT_SIGNING_KEY

    export ConnectionStrings__Postgres="${ConnectionStrings__Postgres:-$(development_postgres_connection_string NoMoreTears)}"
    export RedisCache__Password="${RedisCache__Password:-$REDIS_PASSWORD}"
    export Identity__Jwt__SigningKey="${Identity__Jwt__SigningKey:-$JWT_SIGNING_KEY}"
}

# run_development_migrations [app args]
# The connection string is passed only to the migrations process, so a WebApi value exported
# in the same script is not reused and the database sees the migrations application name.
run_development_migrations() {
    local connection_string

    connection_string="$(development_postgres_connection_string NoMoreTears.Migrations)"
    ConnectionStrings__Postgres="$connection_string" \
        run_development_host "$MIGRATIONS_PROJECT" DOTNET_ENVIRONMENT "" "$@"
}

# run_development_host <project path relative to src> <environment variable> <launch profile or ""> [app args]
run_development_host() {
    local project="$1"
    local environment_variable="$2"
    local launch_profile="$3"
    shift 3

    local run_args=(--no-launch-profile)
    [[ -n "$launch_profile" ]] && run_args=(--launch-profile "$launch_profile")

    run_host "$project" "$environment_variable" Development "${run_args[@]}" -- "$@"
}

# run_staging_host <project path relative to src> <environment variable>
# Release build without a launch profile, so launchSettings.json cannot switch the environment back.
run_staging_host() {
    run_host "$1" "$2" Staging -c Release --no-launch-profile
}

# run_host <project path relative to src> <environment variable> <environment> [dotnet run arguments]
run_host() {
    local project_path="$SRC_DIR/$1"
    local environment_variable="$2"
    local environment="$3"
    shift 3

    require_command dotnet
    [[ -f "$project_path" ]] || fail "project not found: $project_path"

    info "Running $(basename "$project_path" .csproj) ($environment)..."
    # The content root is the working directory, so appsettings.*.json are resolved from the project.
    (
        cd "$(dirname "$project_path")" \
            && export "$environment_variable=$environment" \
            && dotnet run --project "$project_path" "$@"
    )
}
