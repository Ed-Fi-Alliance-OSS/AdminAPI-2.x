# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# First two layers use a dotnet/sdk image to build the Admin API from source
# code. The next two layers use the dotnet/aspnet image to run the built code.
# The extra layers in the middle support caching of base layers.

# Define assets stage using Alpine 3.20 to match the version used in other stages
FROM alpine:3.20@sha256:187cce89a2fdd4eaf457a0af45f5ce27672f35ce0f6df49b5b0ee835afe0561b AS assets

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine@sha256:3f0fb43bb36dbce018486be9ba00a88121b838ac8ea47f8e3ddc9579a7391dda AS build
RUN apk add --no-cache musl=1.2.5-r1 && \
    rm -rf /var/cache/apk/*

ARG ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}

WORKDIR /source
COPY --from=assets ./Application/NuGet.Config ./
COPY --from=assets ./Application/Directory.Packages.props ./
COPY --from=assets ./Application/NuGet.Config EdFi.Ods.AdminApi/
COPY --from=assets ./Application/EdFi.Ods.AdminApi EdFi.Ods.AdminApi/
RUN rm -f EdFi.Ods.AdminApi/appsettings.Development.json

COPY --from=assets ./Application/NuGet.Config EdFi.Ods.AdminApi.Common/
COPY --from=assets ./Application/EdFi.Ods.AdminApi.Common EdFi.Ods.AdminApi.Common/

COPY --from=assets ./Application/EdFi.Ods.AdminApi.V1 EdFi.Ods.AdminApi.V1/

WORKDIR /source/EdFi.Ods.AdminApi
RUN export ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
RUN dotnet restore && dotnet build -c Release
RUN dotnet publish -c Release /p:EnvironmentName=$ASPNETCORE_ENVIRONMENT --no-build -o /app/EdFi.Ods.AdminApi

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine@sha256:cb69be896f82e0d73f513c128ece501c7c1f1809c49415a69dc096e013d5314a AS runtimebase
RUN apk add --no-cache \
        bash=5.2.26-r0 \
        dos2unix=7.5.2-r0 \
        gettext=0.22.5-r0 \
        icu=74.2-r1 \
        musl=1.2.5-r1 \
        openssl=3.3.5-r0 \
        postgresql14-client=14.17-r0 && \
    rm -rf /var/cache/apk/* && \
    addgroup -S edfi && adduser -S edfi -G edfi

FROM runtimebase AS setup
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"

# Alpine image does not contain Globalization Cultures library so we need to install ICU library to get for LINQ expression to work
# Disable the globaliztion invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_HTTP_PORTS=80
ENV DB_FOLDER=pgsql

WORKDIR /app
COPY --from=build /app/EdFi.Ods.AdminApi .

COPY --chmod=500 --from=assets Docker/Settings/dev/${DB_FOLDER}/run.sh /app/run.sh
COPY --from=assets Docker/Settings/dev/log4net.config /app/log4net.txt

RUN cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
    chmod 500 /app/*.sh -- ** && \
    chown -R edfi /app && \
    apk del dos2unix

EXPOSE ${ASPNETCORE_HTTP_PORTS}
USER edfi

ENTRYPOINT ["/app/run.sh"]
