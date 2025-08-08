
# Integration Design: EdFi.Ods.AdminApi.V1 with EdFi.Ods.AdminApi (V2)

## Overview

This document outlines the design for integrating EdFi.Ods.AdminApi.V1 endpoints
into the existing EdFi.Ods.AdminApi (V2) solution. The integration will maintain
backward compatibility while leveraging the enhanced architecture of V2.

### Goals

* Maintain backward compatibility for existing V1 API clients
* Leverage V2's enhanced architecture and infrastructure
* Minimize code duplication between versions
* Provide clear migration path for V1 to V2
* Centralize common functionality

### Integration Strategy

* **Phase 1**: Clean up and modernize V1 codebase
* **Phase 2**: Merge projects and consolidate infrastructure
* **Phase 3**: Implement unified endpoint mapping
* **Phase 4**: Testing and Validation
* **Phase 5**: V1/V2 Multi-Tenancy Integration Strategy
* **Phase 6**: Docker setup

---

## Phase 1: EdFi.Ods.AdminApi.V1 Cleanup and Modernization

### 1.1 Remove Legacy Security Components

**Objective**: Simplify V1 codebase by removing Ed-Fi ODS 5.3 compatibility and standardizing on V6.

**Tasks**:

* Remove `EdFi.SecurityCompatiblity53.DataAccess` dependency
* Remove `OdsSecurityVersionResolver` and related version detection logic
* Remove conditional service implementations (V53Service vs V6Service)
* Update all code flows to use only V6 services
* Rename project assemblies from `EdFi.Ods.AdminApi` to `EdFi.Ods.AdminApi.V1`

**Example Transformation**:

```csharp
// BEFORE: Version-dependent service resolution
private readonly IOdsSecurityModelVersionResolver _resolver;
private readonly EditResourceOnClaimSetCommandV53Service _v53Service;
private readonly EditResourceOnClaimSetCommandV6Service _v6Service;

public EditResourceOnClaimSetCommand(IOdsSecurityModelVersionResolver resolver,
    EditResourceOnClaimSetCommandV53Service v53Service,
    EditResourceOnClaimSetCommandV6Service v6Service)
{
    _resolver = resolver;
    _v53Service = v53Service;
    _v6Service = v6Service;
}

public void Execute(IEditResourceOnClaimSetModel model)
{
    var securityModel = _resolver.DetermineSecurityModel();
    switch (securityModel)
    {
        case EdFiOdsSecurityModelCompatibility.ThreeThroughFive or EdFiOdsSecurityModelCompatibility.FiveThreeCqe:
            _v53Service.Execute(model);
            break;
        case EdFiOdsSecurityModelCompatibility.Six:
            _v6Service.Execute(model);
            break;
        default:
            throw new EdFiOdsSecurityModelCompatibilityException(securityModel);
    }
}

// AFTER: Simplified V6-only implementation
public class EditResourceOnClaimSetCommand(EditResourceOnClaimSetCommandV6Service v6Service)
{
    private readonly EditResourceOnClaimSetCommandV6Service _v6Service = v6Service;
    
    public void Execute(IEditResourceOnClaimSetModel model)
    {
        _v6Service.Execute(model);
    }
}
```

### 1.2 Project Structure Standardization

**Objective**: Align V1 project structure with V2 conventions and dependency management.

**Tasks**:

* Convert V1 project to use `Directory.Packages.props` for version management
* Remove explicit version numbers from V1 project file package references
* Ensure V1 project builds successfully with V6-only dependencies
* Validate all unit tests pass after cleanup
  
---

## Phase 2: Project Merge and Infrastructure Consolidation

### 2.1 Eliminate Duplicate Infrastructure Classes

**Objective**: Consolidate common infrastructure components to reduce maintenance overhead.

**Classes to Consolidate** (merge from V1 to V2, then remove from V1):

* `AdminApiDbContext.cs` - Database context configuration
* `AdminApiEndpointBuilder.cs` - Endpoint registration patterns
* `AdminApiVersions.cs` - API versioning constants
* `CloudOdsAdminApp.cs` - Cloud deployment configurations  
* `CommonQueryParams.cs` - Shared query parameter models
* `DatabaseEngineEnum.cs` - Database engine enumeration
* `EndpointRouteBuilderExtensions.cs` - Route building extensions
* `Enumerations.cs` - Common enumerations
* `IMarkerForEdFiOdsAdminAppManagement.cs` - Assembly markers
* `InstanceContext.cs` - Instance context management
* `OdsSecurityVersionResolver.cs` - Security version resolution (remove from V1)
* `OperationalContext.cs` - Operational context management
* `ValidatorExtensions.cs` - Validation helper extensions
* `WebApplicationBuilderExtensions.cs` - Application builder extensions
* `WebApplicationExtensions.cs` - Application configuration extensions
* **Security folder and all classes** - Use V2 security implementation
* **Connect\Register and Connect\Token endpoints** - Use V2 implementation
* **Artifacts folder** - Remove from V1, use V2 artifacts
* **Information feature** - Remove from V1, use V2 implementation

### 2.2 DataAccess Layer Strategy

**Objective**: Maintain V1 compatibility by preserving Ed-Fi ODS 6.x DataAccess implementations.

**Tasks**:

* Copy Ed-Fi ODS 6.x Admin.DataAccess and Security.DataAccess code implementation files directly to AdminAPI V1 project
* Avoid handling divergence between 6.x and 7.x Security and Admin DataAccess usages while maintaining V1 API compatibility
  
* **Isolation Benefits**:
  * V1 maintains stable DataAccess layer independent of V2 upgrades
  * Eliminates version compatibility complexity in shared DataAccess components
  * Reduces risk of breaking V1 functionality when V2 adopts newer Ed-Fi ODS versions
  
### 2.4 Database Setup Strategy

**Objective**: Maintain separate database infrastructure to support V1 with
Ed-Fi ODS 6.x and V2 with Ed-Fi ODS 7.x DataAccess compatibility.

**Tasks**:

* **Separate Database Instances**: Setup dedicated EdFi_Admin and EdFi_Security databases for each version:
  * `EdFi_Admin_V1` and `EdFi_Security_V1` (6.x schema)
  * `EdFi_Admin` and `EdFi_Security` (7.x schema)

* **Version-Specific Connection Strings**: Define separate connection string configurations for V1 and V2 since each uses different DbContexts:

  ```json
  {
        "ConnectionStrings": 
        {
            "v1": {
                // V1 connections (6.x schema)
                "EdFi_Admin": "Server=.;Database=EdFi_Admin_V1;Integrated Security=true",
                "EdFi_Security": "Server=.;Database=EdFi_Security_V1;Integrated Security=true",
            },
            "v2": {
                // V2 connections (7.x schema) 
                "EdFi_Admin": "Server=.;Database=EdFi_Admin;Integrated Security=true",
                "EdFi_Security": "Server=.;Database=EdFi_Security;Integrated Security=true"
            }
        }
   }
  ```

* **DbContext Registration**: Configure separate DbContext instances in DI container:

  ```csharp
  // V1 DbContext (6.x DataAccess)
  services.AddDbContext<AdminApiV1DbContext>(options =>
      options.UseSqlServer(connectionString.GetConnectionString("EdFi_Admin_V1")));
  
  // V2 DbContext (7.x DataAccess) 
  services.AddDbContext<AdminApiDbContext>(options =>
      options.UseSqlServer(connectionString.GetConnectionString("EdFi_Admin")));
  ```

* **Schema Management**:
  * V1 databases maintain Ed-Fi ODS 6.x schema structure
  * V2 databases use Ed-Fi ODS 7.x schema structure
  * No shared tables or cross-version dependencies

### 2.3 Response Format Standardization

**Objective**: Align V1 response formats with V2 patterns while maintaining
backward compatibility.

**Tasks**:

* Replace explicit `AdminApiResponse` wrapping with `Microsoft.AspNetCore.Http.Results`
* Update V1 endpoints to use modern ASP.NET Core response patterns

### 2.4 Project Type Conversion

**Objective**: Convert V1 from standalone application to class library.

**Tasks**:

* Convert `EdFi.Ods.AdminApi.V1` project to class library type
* Remove `appsettings.json` files from V1 project
* Move V1-specific configuration to V2 project `appsettings.json`
* Move V1 E2E tests to V2 E2E tests folder structure

---

## Phase 3: Endpoint Mapping and API Versioning

### 3.1 Update AdminApiVersions Configuration

**Objective**: Extend V2's versioning infrastructure to include V1 endpoints.

**Implementation**:

Update `EdFi.Ods.AdminApi.Common.Infrastructure.AdminApiVersions` to include V1:

```csharp
public static class AdminApiVersions
{
    public const string V1 = "1.0";
    public const string V2 = "2.0";
    
    public static readonly ApiVersion[] SupportedVersions = 
    {
        new(1, 0), // V1 support
        new(2, 0)  // V2 current
    };
}
```

### 3.2 Implement V1 Endpoint Mapping

**Objective**: Create unified endpoint registration similar to V2's pattern.

**Implementation** (add to `WebApplicationExtensions.cs`):

```csharp
// Existing V2 method
public static void MapAdminConsoleFeatureEndpoints(this WebApplication application)
{
    application.UseEndpoints(endpoints =>
    {
        foreach (var routeBuilder in AdminConsoleFeatureHelper.GetFeatures())
        {
            routeBuilder.MapEndpoints(endpoints);
        }
    });
}

// New V1 method
public static void MapAdminApiV1FeatureEndpoints(this WebApplication application)
{
    application.UseEndpoints(endpoints =>
    {
        foreach (var routeBuilder in AdminApiV1FeatureHelper.GetFeatures())
        {
            routeBuilder.MapEndpoints(endpoints);
        }
    });
}
```

### 3.3 API Versioning Strategy

**URL Structure**:

* V1 endpoints: `/v1/applications`, `/v1/claimsets`, etc.
* V2 endpoints: `/v2/applications`, `/v2/claimsets`, etc.  
* Default (unversioned): Connect endpoints (Register and Token), discovery endpoint, and health check endpoints remain unversioned.

---

## Phase 4: Testing and Validation

**Objective**: Enhance test coverage for V1 project and ensure seamless integration with V2 test infrastructure.

**Tasks**:

* Add unit tests for uncovered areas using NUnit and Shouldly patterns consistent with V2

### 4.1 Integration Test Consolidation

**Objective**: Merge V1 integration tests with V2 test infrastructure while maintaining test isolation and version-specific database compatibility.

**Tasks**:

* **Consolidate Test Projects**: Merge V1 `*.DBTests` projects into V2 database testing infrastructure:
  * Move V1 integration tests to `EdFi.Ods.AdminApi.DBTests` project
  * Organize tests in version-specific namespaces: `EdFi.Ods.AdminApi.DBTests.V1` and `EdFi.Ods.AdminApi.DBTests.V2`
  * Maintain separate test base classes for V1 and V2 to handle different DbContexts
  * Ensure V1 and V2 tests use completely separate test databases

### 4.2 End-to-End Test Migration

**Objective**: Consolidate V1 E2E tests into V2 test structure while maintaining version-specific validation.

**Tasks**:

* **E2E Test Organization**: Move V1 E2E tests to V2 `E2E Tests` folder with version-specific subdirectories:

  ```md

  E2E Tests/
  ├── V1/
  │   ├── Applications/
  │   ├── ClaimSets/
  │   
  ├── V2/
  
  ```

  * Update V1 Postman collections to use `/v1/` URL prefix
  * Create combined collection supporting both V1 and V2 endpoints
  * Add version-specific environment variables
  * Test version routing (v1 vs v2 vs unversioned URLs)
  * Add tests that validate V1 and V2 can operate simultaneously without conflicts
  
## Phase 5: V1/V2 Multi-Tenancy Integration Strategy

**Objective**: Strategy for maintaining multi-tenancy support in V2 while ensuring V1 endpoints continue to work without multi-tenancy requirements during the integration of AdminAPI V1 and V2.

**Tasks**:

* **Define Version-Aware Multi-Tenancy Middleware**: Enhance
  `TenantResolverMiddleware` to detect V1 vs V2 endpoints and apply
  multi-tenancy rules accordingly.

```csharp
private static bool IsV1Endpoint(HttpContext context)
{
    var path = context.Request.Path.Value;    

    if (path.StartsWith("/v1/", StringComparison.InvariantCultureIgnoreCase))
        return true;   
    return false;
}

public async Task InvokeAsync(HttpContext context, RequestDelegate next)
{
 
        // Check if this is a V1 endpoint
        if (IsV1Endpoint(context))
        {
            // For V1 endpoints, skip multi-tenancy validation entirely
            await next.Invoke(context);
            return;
        }

     if (multiTenancyEnabled)
     {
     }
}

```

## Phase 6: Docker Setup

**Objective**: Update Docker files to include Admin API V1 specific changes and
support separate V1/V2 database infrastructure.

**Tasks**:

* Update Build Stage for V1 Project Integration
**Files to modify**: dev.mssql.Dockerfile and dev.pgsql.Dockerfile
  
```docker
# Add after existing project copies

COPY --from=assets ./Application/NuGet.Config EdFi.Ods.AdminApi.V1/
COPY --from=assets ./Application/EdFi.Ods.AdminApi.V1 EdFi.Ods.AdminApi.V1/
```

* **Docker Compose Considerations**: Add V1 specific environment variables if any.
  
 ```yml

  adminapi:  
    environment:
      # Existing V2 configuration
      ConnectionStrings__v2__EdFi_Admin: "host=pb-admin;port=${PGBOUNCER_LISTEN_PORT:-6432};username=${POSTGRES_USER};password=${POSTGRES_PASSWORD};database=EdFi_Admin;pooling=false"
      ConnectionStrings__v2__EdFi_Security: "host=pb-admin;port=${PGBOUNCER_LISTEN_PORT:-6432};username=${POSTGRES_USER};password=${POSTGRES_PASSWORD};database=EdFi_Security;pooling=false"
      
      # New V1-specific database connections
      ConnectionStrings__v1__EdFi_Admin: "host=pb-admin-v1;port=${PGBOUNCER_LISTEN_PORT:-6432};username=${POSTGRES_USER};password=${POSTGRES_PASSWORD};database=EdFi_Admin;pooling=false"
      ConnectionStrings__v1__EdFi_Security: "host=pb-admin-v1;port=${PGBOUNCER_LISTEN_PORT:-6432};username=${POSTGRES_USER};password=${POSTGRES_PASSWORD};database=EdFi_Security;pooling=false"


      # Update on compose file to use the unified database container:

    services:
      # Unified database container for both V1 and V2
    db-admin:
        build:
        context: ../../../
        dockerfile: Docker/db.pgsql.admin.unified.Dockerfile
        environment:
        POSTGRES_USER: ${POSTGRES_USER}
        POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
        POSTGRES_DB: postgres
        volumes:
        - vol-db-admin:/var/lib/postgresql/data
        restart: always
        container_name: ed-fi-db-admin-unified
        healthcheck:
        test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER}"]
        start_period: "60s"
        retries: 3

 ```

* `db.pgsql.admin.unified.Dockerfile` update:
  `edfialliance/ods-api-db-admin:7.3` will be used as base image which contains
  EdFi_Admin and EdFi_Security databases with 7.x schema. V1-specific EdFi_Admin
  and EdFi_Security database schema files (6.x) will be downloaded as Azure
  artifacts and executed to create separate `EdFi_Admin_V1` and
  `EdFi_Security_V1` databases on the same db_admin container. This approach
  provides:

  * **V2 Databases** (pre-existing): `EdFi_Admin` and `EdFi_Security` with 7.x schema
  * **V1 Databases** (created): `EdFi_Admin_V1` and `EdFi_Security_V1` with 6.x schema
  * **Single Container**: All four databases on one PostgreSQL instance
  * **Schema Isolation**: Each version maintains its appropriate Ed-Fi ODS schema version

  **Implementation Details**:
  
  ```dockerfile

    FROM edfialliance/ods-api-db-admin:7.3 AS base
    
    # Download Ed-Fi ODS 6.x schema artifacts for V1 databases
    RUN wget -O /tmp/edfi-6x-artifacts.zip \
        https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_apis/packaging/feeds/EdFi/nuget/packages/EdFi.Suite3.Ods.Artifacts.PgSql/versions/6.2.0/content \
        && unzip /tmp/edfi-6x-artifacts.zip -d /tmp/V1Schema/ \
        && rm /tmp/edfi-6x-artifacts.zip    

    ...

    # Enhanced migration script
    COPY Settings/DB-Admin/pgsql/run-unified-adminapi-migrations.sh /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh
    
  ```

  * **Migration Script Strategy**:
  
  ```bash

    #!/bin/bash
    # Create V1 databases with 6.x schema
    echo "Creating V1 databases with Ed-Fi ODS 6.x schema..."
    psql --command "CREATE DATABASE \"EdFi_Admin_V1\";"
    psql --command "CREATE DATABASE \"EdFi_Security_V1\";"
    
    # Apply 6.x schema to V1 databases
    psql --dbname "EdFi_Admin_V1" --file /tmp/V1Schema/Admin/EdFi_Admin_6x.sql
    psql --dbname "EdFi_Security_V1" --file /tmp/V1Schema/Security/EdFi_Security_6x.sql
    
    # Apply AdminAPI customizations to respective databases
    # AdminAPI customizations to V1 databases
    for FILE in /tmp/AdminApiScripts/Admin/PgSql/*.sql; do
        psql --dbname "EdFi_Admin_V1" --file "$FILE"
    done
    
    # AdminAPI customizations to V2 databases (existing EdFi_Admin, EdFi_Security)
    for FILE in /tmp/AdminApiScripts/Admin/PgSql/*.sql; do
        psql --dbname "EdFi_Admin" --file "$FILE"
    done

  ```
