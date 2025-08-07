# Integrating EdFi.AdminConsole.HealthCheckService into EdFi.Ods.AdminApi with Quartz.NET

## Overview

This document describes the design and process for integrating the `Ed-Fi-Admin-Console-Instance-Management-Worker-Process`
into the `EdFi.Ods.AdminApi` application, leveraging Quartz.NET for scheduled and on-demand execution of Instance-Management-Worker.

## Instance Management Worker Solution

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
The compontent that performes these tasks will be integrated as a new **Feature** (Features layer) in `EdFi.Ods.AdminApi`

### Architecture

#### Components

* **InstanceManagementService**: Service that performs instance management across tenants and instances.
Now part of the Admin API Features, not its own project.

* **InstanceManagementJob**: Quartz.NET job that invokes `InstanceManagementService.RunAsync()`.

This file defines the InstanceManagementJob class, which is the scheduled background job.
It uses Quartz.NET for scheduling and ensures that only one instance runs at a time (via the [DisallowConcurrentExecution] attribute).

* **Quartz.NET Scheduler**: Manages scheduled and ad-hoc job execution.

* **InstanceManagementTrigger Endpoint**: API endpoint to trigger instance management on demand.
POST `/adminconsole/instancemanagement/trigger`

### Process Flow

#### 1. Service Registration

* Register `InstanceManagementJob` with Quartz.NET using `AddQuartz` and `AddQuartzHostedService`.

#### 2. Scheduling with Quartz.NET

* Configure Quartz.NET to schedule `InstanceManagementJob` at a configurable interval (e.g., every 10 minutes,
using `InstanceManagementFrequencyInMinutes` from configuration).
* Use the `[DisallowConcurrentExecution]` attribute on `InstanceManagementJob` to prevent overlapping executions.

#### 3. On-Demand Triggering

* Implement an API endpoint (e.g., `/adminconsole/instancemanagement/trigger`) in `EdFi.Ods.AdminApi`. Note: Grouped with `adminconsole` endpoints for consistency.
* The endpoint uses `ISchedulerFactory` to schedule an immediate, one-time execution of `InstanceManagementJob`.

#### 4. Concurrency Control

* `[DisallowConcurrentExecution]` ensures only one instance of `InstanceManagementJob` runs at a time, regardless of trigger source (scheduled or on-demand).

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
