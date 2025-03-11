# ODS MSSQL Databases

## Install ODS Databases
Possible options can be found here
https://docs.ed-fi.org/reference/docker#2b-microsoft-sql-server

The 3rd option might be the best one. It uses by default Sql Server Express due to lack of license, but clients can set its own key and version using the environment variable MSSQL_PID.

### Using Docker

- Compose file
https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Implementation/blob/main/Docker/docker-compose-local-mssql.yml

-  DBs Docker image
https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Implementation/blob/main/Docker/ods-api-db-ods-minimal/ubuntu/mssql/Dockerfile

## Instance Management Worker

> [!TIP]
> See the following link for more details [Instance Management Worker](./INSTANCE-MANAGEMENT.md).

We will use most of the same Postgres implementation but may vary in some particular actions where T-SQL is
different or needs extra steps.

### On-Premises and Docker
We should be able to connect the databases when are reacheable by the ODS/API, Admin API, and so on. for this particular case we will use the code from [Sandbox library](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS/tree/main/Application/EdFi.Ods.Sandbox)

### Cloud
Possible solutions here could be
1. Check for the Admin App implementation <br>
    - https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-AdminApp/blob/v2.0.0/Application/EdFi.Ods.AdminApp.Management.Azure/AzureDatabaseLifecycleManagementService.cs
    - https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-AdminApp/blob/v2.0.0/Application/EdFi.Ods.AdminApp.Management.Azure/AzureDatabaseManagementService.cs
2. This may need using the Powershell command line or CLI provided by Azure or AWS
    - Azure CLI:
    <br />
    https://learn.microsoft.com/en-us/azure/azure-sql/database/scripts/import-from-bacpac-cli?view=azuresql

