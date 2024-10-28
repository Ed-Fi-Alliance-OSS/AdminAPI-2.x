# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

FROM edfialliance/ods-api-db-admin:7.2.5570-5.1.0@sha256:53a9c8bbed76075bc438e63ba9579a48f1539bf7d5a3ee1b5171739c7e496ab0 AS base
USER root
RUN apk --upgrade --no-cache add dos2unix=~7 unzip=~6

FROM base AS setup
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"

USER root
COPY Settings/DB-Admin/pgsql/run-adminapi-migrations.sh /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh
COPY Application/EdFi.Ods.AdminApi/Artifacts/PgSql/Structure/Admin/ /tmp/AdminApiScripts/PgSql
COPY Settings/dev/adminapi-test-seeddata.sql /tmp/AdminApiScripts/PgSql/adminapi-test-seeddata.sql
RUN dos2unix /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh && \
    dos2unix /tmp/AdminApiScripts/PgSql/* && \
    chmod -R 777 /tmp/AdminApiScripts/PgSql/*

USER postgres

EXPOSE 5432

CMD ["docker-entrypoint.sh", "postgres"]
