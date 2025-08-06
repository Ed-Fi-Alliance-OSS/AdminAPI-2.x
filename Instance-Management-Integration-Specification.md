# Ed-Fi Admin API Instance Management Integration Specification

**Project**: Integration of Instance Management Worker into Admin API  
**Date**: August 6, 2025  
**Repository**: AdminAPI-2.x  
**Branch**: main  

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Requirements Gathering](#requirements-gathering)
3. [High-Level Specification](#high-level-specification)
4. [Detailed Migration Blueprint](#detailed-migration-blueprint)
5. [Implementation Steps](#implementation-steps)

---

## Project Overview

### Current State
- **Instance Management Worker**: Standalone console application in `Ed-Fi-Admin-Console-Instance-Management-Worker-Process` folder
- **Admin API**: Separate solution in `AdminAPI-2.x` folder
- **Current Execution**: Console app runs on schedule (cron) to perform background database instance management tasks

### Integration Goal
Integrate the instance-management-worker console application into the Admin-Api solution, replacing the standalone scheduled process with an API endpoint that:
1. Validates payload (returns 400 if invalid)
2. Starts background task to perform work
3. Responds with 202 Accepted without waiting for completion

---

## Requirements Gathering

Through iterative questioning, the following requirements were established:

### Background Tasks Performed
The instance-management-worker currently performs:
- **Database instance lifecycle management** for Ed-Fi ODS/API instances
- **Creating new ODS database instances** (minimal or full from templates)
- **Deleting database instances** marked for deletion
- **Database operations** (copying, renaming, status checking)
- **Admin API integration** (retrieving job metadata, updating instance status)

### Key Decisions Made

| Question | Decision | Rationale |
|----------|----------|-----------|
| **Triggering Mechanism** | Trigger full worker cycle (process all pending operations across all tenants) | Maintains current functionality scope |
| **Concurrency Handling** | Reject concurrent requests with 409 Conflict | Prevents race conditions and data integrity issues |
| **Authentication** | Same as existing Admin API endpoints | Consistency with current security model |
| **Request Payload** | Empty/minimal payload | Simplicity - just trigger the full cycle |
| **Progress Reporting** | Fire-and-forget (202 Accepted only) | Simplicity - no progress tracking needed |
| **HTTP Method & Path** | `POST /adminconsole/instances/process` | Follows existing adminconsole API pattern |
| **Error Handling** | Different HTTP status codes based on error type | Better API semantics for troubleshooting |
| **Error Response Codes** | Standard set (400, 401, 409, 500) | Well-understood HTTP semantics |
| **Project Structure** | New project: `EdFi.Ods.AdminApi.InstanceManagement` | Separation of concerns while integrating |
| **Background Execution** | Quartz.NET job scheduler | Robust job management with concurrency control |
| **Job Scheduling** | On-demand only (API-triggered, no automatic scheduling) | Full control to API callers |
| **Console App Future** | Replace entirely (remove console app) | Eliminates duplication and simplifies architecture |
| **Configuration Strategy** | Extend existing Admin API appsettings.json | Integration with current configuration system |
| **Deployment Strategy** | Optional feature (enabled/disabled via configuration) | Flexible deployment options |

---

## High-Level Specification

### New API Endpoint

**Endpoint**: `POST /adminconsole/instances/process`
- **Authentication**: Same as existing Admin API endpoints
- **Payload**: Empty/minimal (no parameters required)
- **Execution**: Fire-and-forget background processing
- **Concurrency**: Single execution (reject concurrent requests)

### Response Codes

| Code | Scenario | Description |
|------|----------|-------------|
| `202 Accepted` | Success | Worker process started successfully |
| `400 Bad Request` | Invalid payload | Payload validation failed |
| `401 Unauthorized` | Authentication failure | User not authenticated |
| `409 Conflict` | Worker already running | Another worker cycle in progress |
| `500 Internal Server Error` | Server/initialization errors | System-level failures |

### Background Processing

- **Technology**: Quartz.NET job scheduler
- **Trigger**: On-demand only (no automatic scheduling)
- **Scope**: Full worker cycle processing all pending operations across all tenants
- **Operations**: Create instances, delete instances (same as current console app)
- **Error Handling**: Individual operation failures logged but don't affect API response

### Architecture Changes

1. **Project Structure**
   - Create new project: `EdFi.Ods.AdminApi.InstanceManagement`
   - Move all worker logic from standalone solution
   - Integrate with main Admin API solution

2. **Configuration**
   - Extend existing Admin API `appsettings.json`
   - Optional feature controlled by configuration flags
   - Runtime enablement based on configuration presence

3. **Console Application**
   - Remove entirely (no backward compatibility needed)
   - All functionality replaced by API endpoint

### Technical Implementation

1. **Quartz.NET Integration**
   - Configure in Admin API startup
   - Job class wrapping current `Application.Run()` logic
   - Job state tracking to prevent concurrent execution

2. **Controller Implementation**
   - New controller in `/adminconsole` namespace
   - Validate no job currently running
   - Trigger Quartz job execution
   - Return appropriate HTTP status codes

3. **Dependency Injection**
   - Register instance management services in Admin API DI container
   - Reuse existing provisioner interfaces and implementations
   - Maintain current Admin API client integration

---

## Detailed Migration Blueprint

### Phase 1: Foundation & Setup

#### Step 1.1: Solution Structure Setup
**Goal**: Establish project structure within Admin API solution

**Step 1.1a**: Create Basic Project Structure
- Create `EdFi.Ods.AdminApi.InstanceManagement` class library project
- Add to existing solution file
- Set up basic folder structure (Controllers, Services, Models, Configuration)

**Step 1.1b**: Add Core Dependencies
- Add NuGet package references (Quartz.NET, Microsoft.Extensions.*)
- Add project reference to main Admin API project
- Add reference to any shared Ed-Fi libraries

**Step 1.1c**: Copy Core Interfaces
- Copy `IInstanceProvisioner` interface
- Copy basic model classes (InstanceStatus, DbInstanceType, etc.)
- Update namespaces to match new project structure

#### Step 1.2: Configuration Integration
**Goal**: Merge configuration systems
- Analyze current worker `AppSettings` and `AdminApiSettings` classes
- Create new configuration section in Admin API `appsettings.json`
- Map existing worker settings to new structure
- Add feature toggle for instance management enablement

#### Step 1.3: Dependency Injection Setup
**Goal**: Register instance management services
- Create service registration extension method
- Register provisioner interfaces and implementations
- Set up Quartz.NET services in Admin API startup
- Configure conditional registration based on feature toggle

### Phase 2: Core Logic Migration

#### Step 2.1: Move Provisioner Logic
**Goal**: Transfer database provisioning capabilities

**Step 2.1a**: Copy Base Provisioner Classes
- Copy `IInstanceProvisioner` and any base implementations
- Copy shared enums and constants
- Ensure compilation without runtime dependencies

**Step 2.1b**: Move Database-Specific Provisioners
- Copy PostgreSQL provisioner implementation
- Copy SQL Server provisioner implementation
- Update connection string handling for new configuration

**Step 2.1c**: Update Provisioner Registration
- Create provisioner factory or registry
- Add conditional registration based on database type
- Test basic provisioner instantiation

#### Step 2.2: Move Admin API Client Logic
**Goal**: Transfer Admin API communication
- Copy `AdminApiCaller` and related classes
- Move tenant and instance data models
- Migrate HTTP client configurations
- Ensure compatibility with existing Admin API endpoints

#### Step 2.3: Core Application Logic
**Goal**: Transfer main worker business logic

**Step 2.3a**: Extract Business Logic Interface
- Create `IInstanceManagementService` interface
- Define method signatures for create/delete operations
- Create empty implementation class

**Step 2.3b**: Copy Create Instance Logic
- Copy only the `CreateInstances()` method logic
- Adapt for dependency injection
- Test compilation and basic execution

**Step 2.3c**: Copy Delete Instance Logic
- Copy only the `DeleteInstances()` method logic
- Ensure proper error handling
- Test integration with create logic

**Step 2.3d**: Remove Console Dependencies
- Remove any console-specific logging or hosting code
- Adapt for web application hosting
- Ensure proper async/await patterns

### Phase 3: Quartz.NET Integration

#### Step 3.1: Job Definition
**Goal**: Create Quartz.NET job wrapper
- Create `InstanceManagementJob` implementing `IJob`
- Wrap existing `Application.Run()` logic in job execution
- Implement proper cancellation token handling
- Add job state tracking for concurrency control

#### Step 3.2: Job Scheduler Configuration
**Goal**: Set up Quartz.NET infrastructure
- Configure Quartz.NET in Admin API startup
- Set up in-memory job store (no persistence needed)
- Create job factory with DI integration
- Configure job execution settings

#### Step 3.3: Concurrency Management
**Goal**: Prevent overlapping executions
- Implement job state tracking service
- Add distributed lock mechanism if needed
- Create job status checking functionality
- Handle cleanup for interrupted jobs

### Phase 4: API Endpoint Implementation

#### Step 4.1: Controller Creation
**Goal**: Create the trigger endpoint

**Step 4.1a**: Create Basic Controller Structure
- Create `InstanceProcessingController` with basic structure
- Add route configuration matching `/adminconsole/instances/process`
- Add basic POST method with empty implementation

**Step 4.1b**: Add Authentication Integration
- Apply existing Admin API authentication attributes
- Test authentication works with new endpoint
- Ensure authorization follows existing patterns

**Step 4.1c**: Add Basic Validation
- Implement request payload validation (even if minimal)
- Add model binding and validation attributes
- Return 400 for invalid requests

#### Step 4.2: Job Triggering Logic
**Goal**: Connect API to background processing
- Implement job status checking before triggering
- Add Quartz.NET job scheduling calls
- Return appropriate HTTP status codes
- Add proper error handling

#### Step 4.3: Error Response Mapping
**Goal**: Implement comprehensive error handling
- Map different error types to HTTP status codes
- Add detailed error logging
- Create error response models
- Ensure consistent error handling patterns

### Phase 5: Testing & Validation

#### Step 5.1: Unit Testing
**Goal**: Ensure component-level correctness
- Port existing worker unit tests
- Create tests for new controller
- Test Quartz.NET job execution
- Test error handling scenarios

#### Step 5.2: Integration Testing
**Goal**: Validate end-to-end functionality
- Test full API-to-database workflow
- Validate concurrency controls
- Test configuration scenarios
- Verify error response codes

#### Step 5.3: Performance & Load Testing
**Goal**: Ensure system stability
- Test under multiple concurrent requests
- Validate memory usage in web hosting
- Test with realistic database loads
- Verify timeout and cancellation behavior

### Phase 6: Documentation & Cleanup

#### Step 6.1: API Documentation
**Goal**: Document new endpoint
- Update OpenAPI/Swagger specifications
- Add endpoint documentation
- Document configuration options
- Create migration guide from console app

#### Step 6.2: Console App Removal
**Goal**: Clean up old solution
- Remove standalone worker solution
- Update deployment documentation
- Remove old Docker configurations
- Update CI/CD pipelines

#### Step 6.3: Final Validation
**Goal**: Ensure complete migration
- End-to-end testing in staging environment
- Performance validation
- Security review
- Documentation review

---

## Implementation Steps

### Micro-Steps for Critical Components

#### Step 1.1a Refined: Create Basic Project Structure

**Step 1.1a.1**: Create Empty Project
- Use `dotnet new classlib` for new project
- Add to solution file
- Verify compilation

**Step 1.1a.2**: Create Folder Structure
- Create `Controllers`, `Services`, `Models`, `Configuration` folders
- Add placeholder classes to ensure folder structure
- Commit structure changes

**Step 1.1a.3**: Basic Namespace Setup
- Define consistent namespace pattern
- Create base classes or interfaces as placeholders
- Ensure no compilation errors

### Step Quality Assessment

After multiple iterations, the steps are appropriately sized because:

#### ✅ **Small Enough for Safety**
- Each step has a clear, single responsibility
- Steps can be completed in 2-4 hours of focused work
- Each step can be tested independently
- Rollback is possible after each step

#### ✅ **Large Enough for Progress**
- Each step produces meaningful, demonstrable progress
- Steps build logically on previous work
- Completion of each step reduces overall project risk
- Each step can be code-reviewed as a discrete unit

#### ✅ **Well-Structured Dependencies**
- Clear prerequisite relationships between steps
- No circular dependencies
- Parallel work possible where appropriate
- Critical integration points are isolated and testable

#### ✅ **Risk-Appropriate Granularity**
- High-risk areas (core business logic, database operations) broken into smaller steps
- Lower-risk areas (documentation, cleanup) use larger steps
- Critical integration points are isolated and testable

---

## Conclusion

This specification provides a comprehensive, step-by-step approach to migrating the Ed-Fi Instance Management Worker from a standalone console application into the Admin API as a new endpoint. The approach prioritizes safety through small, testable steps while ensuring meaningful progress toward the integration goal.

The resulting system will provide the same functionality as the current worker process but with the added benefit of on-demand execution via API calls, better integration with the Admin API infrastructure, and simplified deployment and maintenance.

---

**Document Generated**: August 6, 2025  
**Version**: 1.0  
**Status**: Ready for Development Handoff
