# Education Organizations Endpoints

## Overview

Provides a consolidated view of education organizations across all Ed-Fi ODS
instances through REST API endpoints. The data is refreshed on a scheduled
basis.

## Features

* **REST API Endpoints:**
  * `GET /v2/educationorganizations` or `GET /v1/educationorganizations` -
    Returns all education organizations from all instances
  * `GET /v2/educationorganizations/{instanceId}` or
    `GET /v1/educationorganizations/{instanceId}` - Returns education organizations
    for a specific instance

* **Automatic Data Refresh:**
  * Quartz.NET scheduled job runs every 6 hours by default
  * No manual refresh endpoint - data is refreshed automatically via background
    job

* **Cross-Database Support:**
  * Works with both SQL Server and PostgreSQL
  * Uses C# service layer for dynamic database connections and efficient data
    refresh

## Database Schema

### Tables

* `adminapi.EducationOrganizations` - Stores consolidated education organization
  data

  ```sql
  CREATE TABLE [adminapi].[EducationOrganizations] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [InstanceId] INT NOT NULL,
        [InstanceName] NVARCHAR(100) NOT NULL,
        [EducationOrganizationId] INT NOT NULL,
        [NameOfInstitution] NVARCHAR(75) NOT NULL,
        [ShortNameOfInstitution] NVARCHAR(75) NULL,
        [Discriminator] NVARCHAR(128) NOT NULL,
        [ParentId] INT NULL,
        [OdsDatabaseName] NVARCHAR(255) NULL,
        [LastRefreshed] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastModifiedDate] DATETIME2 NULL,
        CONSTRAINT [PK_EducationOrganizations] PRIMARY KEY ([Id])
    );
  ```

## API Usage Examples

### Get All Education Organizations

```http
GET /v2/educationorganizations
Authorization: Bearer <token>
```

### Get Education Organizations for Specific Instance

```http
GET /v2/educationorganizations/123
Authorization: Bearer <token>
```

## Configuration

### Quartz.NET Job Scheduling

Add to `appsettings.json`:

```json
{
   "AppSettings": {
   "EducationOrgsRefreshIntervalInHours": 6
   }
}
```

### Database Connection

The system uses the existing AdminAPI database connection and dynamically
connects to ODS instance databases based on the `OdsInstances` table
configuration.

## Architecture

### Service Layer

The service files can be maintained in a common project and shared between the
V1 and V2 projects to avoid code duplication.

* `IGetEducationOrganizationQuery` - Main query handling interface

* `GetEducationOrganizationQuery` - Implementation of database context query logic
  for reading the EducationOrganizations

The `RefreshEducationOrganizationCommand` service layer implements a
comprehensive C# solution to aggregate education organization data across
multiple ODS instances and persist it into the
[adminapi].[EducationOrganizations] table.

**Key Components:**

* **Dynamic Database Connectivity**: Reads ODS instance connection strings from
    the `OdsInstances` table and establishes connections to each ODS database
    dynamically
* **Multi-Database Support**: Handles both SQL Server and PostgreSQL with
    database-specific query implementations
* **Parent Hierarchy Logic**: Implements the same parent relationship logic from
    the AWS Lambda reference using COALESCE joins to establish education
    organization hierarchies
    [LambdaFunction](https://github.com/edanalytics/startingblocks_oss/blob/efc423212930e01f0166033d97be392d3a675999/lambdas/TenantResourceTreeLambdaFunction/index.mjs#L100)
* **Robust Error Handling**: Continues processing other instances even if one
    fails, with detailed logging for troubleshooting

**Core Methods:**

* `RefreshDataAsync(int? instanceId)` - Main orchestration method that clears
  existing data and processes ODS instances
* `GetOdsInstancesToProcessAsync(int? instanceIdFilter)` - Queries the
  OdsInstances table to get connection information
* `ProcessOdsInstanceAsync(OdsInstanceInfo odsInstance)` - Processes a single
  ODS instance with validation and error handling
* `QueryEducationOrganizationsFromOdsAsync(OdsInstanceInfo odsInstance)` -
  Routes to database-specific query methods
* `QueryEducationOrganizationsSqlServerAsync()` /
  `QueryEducationOrganizationsPostgreSqlAsync()` - Database-specific
  implementations that execute the parent hierarchy queries

**Data Flow:**

1. Service reads ODS instance configurations from the AdminAPI database
2. For each instance, establishes a direct connection using the stored
   connection string
3. Executes complex SQL queries with COALESCE logic to determine parent
   relationships
4. Maps raw data to Entity Framework entities with proper validation
5. Performs batch inserts using Entity Framework for optimal performance
6. Continues processing remaining instances even if individual instances fail

### Background Jobs

* `EducationOrganizationRefreshJob` - Quartz.NET job for scheduled refresh

* Runs with `DisallowConcurrentExecution` to prevent overlapping executions

### Controllers

* `EducationOrganizationsController` - REST API endpoints for read-only access
* Includes proper authorization, error handling, and logging
