# syntax=docker/dockerfile:1
# Build context: src/ (see infrastructure/docker/docker-compose.webapi.yaml).

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY . .
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish Modules/Masters/HostSide/Consumers/NoMoreTears.Modules.Masters.HostSide.Consumers.csproj \
        --configuration Release \
        --output /app \
        -p:UseAppHost=false \
        -p:DebugType=None

# A long-running generic host (no web server); the module providers need the shared ASP.NET Core framework.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# ICU for culture-aware validation messages, tzdata for the IANA time zones of work schedules.
RUN apk add --no-cache icu-libs icu-data-full tzdata

ENV DOTNET_ENVIRONMENT=Docker \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["dotnet", "NoMoreTears.Modules.Masters.HostSide.Consumers.dll"]
