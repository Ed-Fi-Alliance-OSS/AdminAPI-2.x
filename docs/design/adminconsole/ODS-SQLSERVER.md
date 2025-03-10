# ODS MSSQL Databases

Possible options can be found here
https://docs.ed-fi.org/reference/docker#2b-microsoft-sql-server

The 3rd option might be the best one. It uses by default Sql Server Express due to lack of license, but clients can set its own key and version setting the environment variable MSSQL_PID.

## Docker Compose

#### File
https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Implementation/blob/main/Docker/docker-compose-local-mssql.yml

#### Docker image
https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Implementation/blob/main/Docker/ods-api-db-ods-minimal/ubuntu/mssql/Dockerfile

## On-Premises
We should be able to connect the databases when are reacheable by the ODS/API, Admin API, and so on.
