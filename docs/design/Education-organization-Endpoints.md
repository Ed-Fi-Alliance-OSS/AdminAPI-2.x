# Education Organizations Endpoints

## Overview

Provides a consolidated view of education organizations across all Ed-Fi ODS
instances through REST API endpoints. The data is refreshed on a scheduled
basis.

## Features

* **REST API Endpoints:**
  * `GET /v2/educationorganizations` - Returns all education organizations from
    all instances
  * `GET /v2/educationorganizations/{instanceId}` - Returns education
    organizations for a specific instance

* **Automatic Data Refresh:**
  * Quartz.NET scheduled job runs every 6 hours by default
  * No manual refresh endpoint - data is refreshed automatically via background
    job

* **Cross-Database Support:**
  * Works with both SQL Server and PostgreSQL
  * Uses stored procedure/function for efficient data refresh

## Database Schema

### Tables

* `adminapi.EducationOrganizations` - Stores consolidated education organization
  data
* Includes id, instance_id, instance_name, education_organization_id,
    name_of_institution short_name_of_institution, discriminator, parentid,
    ods_database_name, last_refreshed, last_modified_date

### Stored Procedures/Functions

* SQL Server: `RefreshEducationOrganizations(@InstanceIdFilter)`
* PostgreSQL: `refresh_education_organizations(p_instance_id_filter)`
  
The implementation logic for the stored procedure/function can be referenced from
[LambdaFunction](https://github.com/edanalytics/startingblocks_oss/blob/efc423212930e01f0166033d97be392d3a675999/lambdas/TenantResourceTreeLambdaFunction/index.mjs#L100)

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

* `IGetEducationOrganizationQuery` - Main query handling interface

* `GetEducationOrganizationQuery` - Implementation with cross-database context
  query logic

### Background Jobs

* `EducationOrganizationRefreshJob` - Quartz.NET job for scheduled refresh

* Runs with `DisallowConcurrentExecution` to prevent overlapping executions

### Controllers

* `EducationOrganizationsController` - REST API endpoints for read-only access
* Includes proper authorization, error handling, and logging
