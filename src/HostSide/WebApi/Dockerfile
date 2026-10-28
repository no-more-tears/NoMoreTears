# syntax=docker/dockerfile:1
# Build context: src/ (see infrastructure/docker/docker-compose.webapi.yaml).

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY . .
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish HostSide/WebApi/NoMoreTears.HostSide.WebApi.csproj \
        --configuration Release \
        --output /app \
        -p:UseAppHost=false \
        -p:DebugType=None

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Docker \
    ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app .

USER $APP_UID
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD wget -q --spider http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "NoMoreTears.HostSide.WebApi.dll"]
