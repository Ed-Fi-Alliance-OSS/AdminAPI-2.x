
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
* **Phase 4**: Logging Modernization
* **Phase 5**: Testing and Validation

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

**Implementation Strategy**:

* Copy Ed-Fi ODS 6.x Admin.DataAccess and Security.DataAccess code implementation files directly to AdminAPI V1 project
* Avoid handling divergence between 6.x and 7.x Security and Admin DataAccess usages while maintaining V1 API compatibility
* **Isolation Benefits**:
  * V1 maintains stable DataAccess layer independent of V2 upgrades
  * Eliminates version compatibility complexity in shared DataAccess components
  * Reduces risk of breaking V1 functionality when V2 adopts newer Ed-Fi ODS versions

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
* Default (unversioned): Route to V2 or configurable default

---

## Phase 4: Logging Modernization

### 4.1 Migrate to Serilog

**Objective**: Standardize both V1 and V2 on modern structured logging.

**Tasks**:

* Replace log4net with Serilog in both V1 and V2
* Implement consistent logging patterns across versions and configure structured
  logging

## Phase 5: Testing and Validation

**Objective**: Enhance test coverage for V1 project and ensure seamless integration with V2 test infrastructure.

**Tasks**:

* Add unit tests for uncovered areas using NUnit and Shouldly patterns consistent with V2

### 5.2 Integration Test Consolidation

**Objective**: Merge V1 integration tests with V2 test infrastructure while maintaining test isolation.

**Tasks**:

* Consolidate V1 `*.DBTests` with V2 database testing approach
* Ensure V1 tests use same test database setup as V2
* Update connection string management for integrated solution

### 5.3 End-to-End Test Migration

**Objective**: Consolidate V1 E2E tests into V2 test structure while maintaining version-specific validation.

**Tasks**:

* **E2E Test Organization**: Move V1 E2E tests to V2 `E2E Tests` folder with version-specific subdirectories:

  ```  
  E2E Tests/
  ├── V1/
  │   ├── Applications/
  │   ├── ClaimSets/
  │   └── Common/
  ├── V2/
  └── Shared/
  ```

  * Update V1 Postman collections to use `/v1/` URL prefix
  * Create combined collection supporting both V1 and V2 endpoints
  * Add version-specific environment variables
  * Test version routing (v1 vs v2 vs unversioned URLs)
  * Add tests that validate V1 and V2 can operate simultaneously without conflicts
