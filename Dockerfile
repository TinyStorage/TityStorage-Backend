FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV NUGET_CERT_REVOCATION_MODE=offline
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false 

COPY src ./

RUN dotnet publish TinyStorage.Application -c Release -o /published/Web /p:PublishReadyToRun=true

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV TZ=Europe/Moscow

FROM base AS production
WORKDIR /app
COPY --from=build /published/Web ./

ENV ASPNETCORE_URLS="http://0.0.0.0:8080" \
    DOTNET_CLI_TELEMETRY_OPTOUT=0 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    LC_ALL=ru_RU.UTF-8 \
    LANG=ru_RU.UTF-8 \
    DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["./TinyStorage.Application"]

