# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#tag 8.0-alpine
FROM mcr.microsoft.com/dotnet/aspnet:8.0.10-alpine3.20-amd64@sha256:1659f678b93c82db5b42fb1fb12d98035ce482b85747c2c54e514756fa241095 AS base
RUN apk upgrade --no-cache && \
    apk add --no-cache bash=~5 dos2unix=~7 gettext=~0 icu=~74 jq=~1 musl=~1.2.5-r1 openssl=3.3.4-r0 postgresql14-client=~14 unzip=~6 && \
    rm -rf /var/cache/apk/* && \
    addgroup -S edfi && adduser -S edfi -G edfi

FROM base AS build
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"

# Alpine image does not contain Globalization Cultures library so we need to install ICU library to get for LINQ expression to work
# Disable the globaliztion invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ARG ADMIN_API_VERSION
ENV ADMIN_API_VERSION="${ADMIN_API_VERSION:-2.2.0}"
ENV ASPNETCORE_HTTP_PORTS=80

WORKDIR /app

COPY --chmod=500 Settings/pgsql/run.sh /app/run.sh
COPY Settings/pgsql/log4net.config /app/log4net.txt

RUN umask 0077 && \
    wget -nv -O /app/AdminApi.zip "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_apis/packaging/feeds/EdFi/nuget/packages/EdFi.Suite3.ODS.AdminApi/versions/${ADMIN_API_VERSION}/content" && \
    unzip /app/AdminApi.zip AdminApi/* -d /app/ && \
    cp -r /app/AdminApi/. /app/ && \
    rm -f /app/AdminApi.zip && \
    rm -r /app/AdminApi && \
    cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
    chmod 700 /app/*.sh -- ** && \
    rm -f /app/*.exe && \
    apk del unzip dos2unix && \
    chown -R edfi /app

EXPOSE ${ASPNETCORE_HTTP_PORTS}
USER edfi

ENTRYPOINT [ "/app/run.sh" ]
