# syntax=docker/dockerfile:1
# Build context: src/ (see infrastructure/docker/docker-compose.consumers.yaml).

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY . .
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish Modules/Requests/HostSide/Consumers/NoMoreTears.Modules.Requests.HostSide.Consumers.csproj \
        --configuration Release \
        --output /app \
        -p:UseAppHost=false \
        -p:DebugType=None

# A long-running generic host (no web server); the module providers need the shared ASP.NET Core framework.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# ICU for culture-aware validation messages.
RUN apk add --no-cache icu-libs icu-data-full

ENV DOTNET_ENVIRONMENT=Docker \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["dotnet", "NoMoreTears.Modules.Requests.HostSide.Consumers.dll"]