# Integrating EdFi.AdminConsole.InstanceManagement into EdFi.Ods.AdminApi with Quartz.NET

This document describes the design and process for integrating the `Ed-Fi-Admin-Console-Instance-Management-Worker-Process`
into the `EdFi.Ods.AdminApi` application, leveraging Quartz.NET for on-demand execution of Instance-Management-Worker.

## Overview of the InstanceManagement solution

These are the 4 projects that create the Instance Management Worker solution. `EdFi.AdminConsole.InstanceMgrWorker.Configuration` is
the only project that would be copied over to Admin API.

1. EdFi.AdminConsole.InstanceManagementWorker (Console Application, where process starts)
2. EdFi.AdminConsole.InstanceMgrWorker.Configuration (Manage ods database creation and deletion)
3. EdFi.AdminConsole.InstanceMgrWorker.Core (main core functinality, mainly call Admin API features)
4. EdFi.Ods.AdminConsole.InstanceMgrWorker.Core.UnitTests (17 Unit tests)

### Restoring and deleting the instance database

The `EdFi.AdminConsole.InstanceMgrWorker.Configuration` project is the responsable to do these tasks.
It should not change given that we still need these tasks.

#### Restoring and deleting a mssql

The process to create the mssql database is the following:

1. Reads the logical file names from the backup using `RESTORE FILELISTONLY`.
2. Executes a `RESTORE DATABASE` command to create the new database from the backup, moving the data and log files to the correct locations.

To delete an ods database instance it simply exeutes `DROP DATABASE`

#### Restoring and deleting a pgsql

To create the ods instance database on pgsql it simply executes the `CREATE DATABASE "new-database" TEMPLATE "template-database"` command.

To delete it, the command is `DROP DATABASE IF EXISTS "database"`.

### HTTP call to Admin API and ODS API

The `EdFi.AdminConsole.InstanceMgrWorker.Core` is the responsible to do these tasks.
This project can be removed, given that `Instance-Management-Worker` lives now with Admin API.
To get tenants and instances, and other other transactions we do in this project, we can use the
`Database/Commands` classes

### Project execution

The `EdFi.AdminConsole.InstanceManagementWorker` is the responsible to do these tasks.
This project can be removed as well. Its main tasks is to loop throuh tenants and intances
to process instances to be created and instances to be deleted.
The compontent that performes these tasks will be integrated as news **Features** (Features layer) in `EdFi.Ods.AdminApi`

## New Architecture

### Components

* **InstanceManagementCompleteService Feature**: Service that performs instance management creation for given instance.
`InstanceManagementCompleteService` is triggered on every call to `POST /adminconsole/instances`

* **InstanceManagementDeleteService Feature**: Service that performs instance management deletion for given instance.
`InstanceManagementDeleteService` is triggered on every call to `DELETE /adminconsole/instances`

* **InstanceManagementRenameService Feature**: Service that performs instance management renaming for given instance.
`InstanceManagementRenameService` is triggered on every call to `PUT /adminconsole/instances`

* **InstanceManagementCompleteJob**: Quartz.NET job that invokes `InstanceManagementCompleteService.RunAsync()`.

* **InstanceManagementDeleteJob**: Quartz.NET job that invokes `InstanceManagementDeleteService.RunAsync()`.

* **InstanceManagementRenameJob**: Quartz.NET job that invokes `InstanceManagementRenameService.RunAsync()`.

### Configuration

There are a number of application settings that need to be added to Admin API

| Name   | Description |
| ---    | ---         |
| AppSettings:OverrideExistingDatabase | When a creation of a new instance is requested, but the database already exists. |
| AppSettings:SqlServerBakFile | Backup Sql Server file to use as a template when creating a new ods instance database. |
| AppSettings:MaxRetryAttempts |  When calling Ods API and Admin API, how many times to retry when connection is not successfull |
| DatabaseProvider | Database engine |

There are a number of application settings that we **DO NOT** need anymore in Admin API

| Name   | Description |
| ---    | ---         |
| AdminApiSettings:AdminConsoleTenantsURL | Call to get Tenants from Admin API |
| AdminApiSettings:AdminConsoleInstancesURL | Call to get Instances from Admin API |
| AdminApiSettings:AdminConsoleCompleteInstancesURL | Call to complete Instances from Admin API |
| AdminApiSettings:AdminConsoleInstanceDeletedURL | Call to delete Instances from Admin API |
| AdminApiSettings:AdminConsoleInstanceDeleteFailedURL | Call when a deletion of a instance database has failed |
| AdminApiSettings:AccessTokenUrl | Call to get access token |
| AdminApiSettings:ClientId | Client Id to get authenticated on Admin API |
| AdminApiSettings:ClientSecret | Client Secret Id to get authenticated on Admin API |
| AdminApiSettings:GrantType | Grant Type |
| AdminApiSettings:Scope | Scope |

### Connection Strings

Two new connection strings need to be added to Admin API

| Name   | Description |
| ---    | ---         |
| EdFi_Master | To get authenticated on Database Engine (mssql or pgsql) |
| EdFi_Ods | Where the new Instance database is created or deleted |

### Cleanup

Given the new arquitecture, it should be safe to remove these 3 endpoints

1. /adminconsole/instances/<instanceId>/deletefailed
2. /adminconsole/instances/<instanceId>/renameFailed
3. /adminconsole/instances/<instanceId>/completed
