# syntax=docker/dockerfile:1
# Build context: src/ (see infrastructure/docker/docker-compose.migrations.yaml).

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY . .
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish Modules/Identity/HostSide/Migrations/NoMoreTears.Modules.Identity.HostSide.Migrations.csproj \
        --configuration Release \
        --output /app \
        -p:UseAppHost=false \
        -p:DebugType=None

# A one-shot console job only needs the base runtime, not ASP.NET Core.
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS runtime
WORKDIR /app

ENV DOTNET_ENVIRONMENT=Docker

COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["dotnet", "NoMoreTears.Modules.Identity.HostSide.Migrations.dll"]
