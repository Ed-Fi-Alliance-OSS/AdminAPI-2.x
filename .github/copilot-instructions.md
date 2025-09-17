# Ed-Fi Admin API 2.x

Ed-Fi Admin API 2.x is a .NET 8.0 ASP.NET Core web application that provides programmatic administration capabilities for ODS/API platform instances. The application supports ODS/API version 7.0 and greater.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Setup
- Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
- Install PowerShell Core (`pwsh`) for cross-platform PowerShell script execution.
- **CRITICAL**: Access to Ed-Fi Alliance private NuGet feed is required for all build operations.
- Install coverage analysis tools: `dotnet tool install -g dotnet-reportgenerator-globaltool`.

### Build Commands 
- **NEVER CANCEL builds or tests** - they may take 15+ minutes to complete.
- Use PowerShell (`pwsh`) to execute all build.ps1 commands, not bash.
- All build commands: `pwsh ./build.ps1 -Command <CommandName>`.

**Core Commands:**
- `pwsh ./build.ps1 -Command Build` -- compiles the solution. Takes 3-5 minutes. NEVER CANCEL.
- `pwsh ./build.ps1 -Command UnitTest` -- runs unit tests. Takes 8-12 minutes. NEVER CANCEL. Set timeout to 20+ minutes.
- `pwsh ./build.ps1 -Command IntegrationTest` -- runs integration tests with database. Takes 15-25 minutes. NEVER CANCEL. Set timeout to 40+ minutes.
- `pwsh ./build.ps1 -Command BuildAndTest` -- runs Build + UnitTest + IntegrationTest. Takes 25-35 minutes total. NEVER CANCEL. Set timeout to 60+ minutes.

**Additional Commands:**
- `pwsh ./build.ps1 -Command Clean` -- cleans build artifacts.
- `pwsh ./build.ps1 -Command Run -LaunchProfile "EdFi.Ods.AdminApi (Dev)"` -- runs the application locally.

### Code Coverage
- Add `-RunCoverageAnalysis` flag to test commands for coverage reports.
- Coverage reports generated in `coveragereport/` directory.
- Example: `pwsh ./build.ps1 -Command UnitTest -RunCoverageAnalysis`.

### Running the Application
**Local Development Options:**
1. **Command Line**: `pwsh ./build.ps1 -Command Run -LaunchProfile "EdFi.Ods.AdminApi (Dev)"`.
2. **Docker**: Use `Docker/V2/Compose/pgsql/SingleTenant/compose-build-dev.yml`.
3. **Visual Studio**: Open `Application/Ed-Fi-ODS-AdminApi.sln` and use launch profiles.

**Launch Profiles Available:**
- `EdFi.Ods.AdminApi (Dev)` -- Development mode with Swagger UI at https://localhost:7214/swagger.
- `EdFi.Ods.AdminApi (Prod)` -- Production mode for E2E testing.
- `EdFi.Ods.AdminApi (Docker)` -- Docker containerized version.

### Validation and Testing
**ALWAYS run these validation steps after making changes:**
1. Build successfully with `pwsh ./build.ps1 -Command Build`.
2. Run unit tests with `pwsh ./build.ps1 -Command UnitTest`.
3. **Manual Validation Scenario**: Start the application and verify:
   - Application starts without errors.
   - Swagger UI loads at https://localhost:7214/swagger.
   - Health endpoint returns 200 OK: `curl -k https://localhost:7214/health`.
   - Can register a client: `curl -k -X POST https://localhost:7214/connect/register -H "Content-Type: application/x-www-form-urlencoded" -d "ClientId=TestClient&ClientSecret=TestSecret&DisplayName=Test"`.

**Integration Tests:**
- Require SQL Server or PostgreSQL database connection.
- Run with `pwsh ./build.ps1 -Command IntegrationTest -UseIntegratedSecurity:$false -DbUsername "sa" -DbPassword "YourPassword"`.

**E2E Tests:**
- Located in `Application/EdFi.Ods.AdminApi/E2E Tests/V2/`.
- Use Postman collections or Newman CLI.
- Run with: `newman run "Admin API E2E.postman_collection.json" -e "Admin API.postman_environment.json" -k`.

## Coding Standards

### General
- Make only high confidence suggestions when reviewing code changes.
- **NEVER change NuGet.config files** unless explicitly asked to.

### Formatting
- Apply code-formatting style defined in `.editorconfig`.
- Prefer file-scoped namespace declarations and single-line using directives.
- Insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
- Ensure that the final return statement of a method is on its own line.
- Use pattern matching and switch expressions wherever possible.
- Use `nameof` instead of string literals when referring to member names.

### Nullable Reference Types
- Declare variables non-nullable, and check for `null` at entry points.
- Always use `is null` or `is not null` instead of `== null` or `!= null`.
- Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

### Testing Standards
- We use NUnit tests.
- We use Shouldly for assertions.
- Use FakeItEasy for mocking in tests.
- Copy existing style in nearby files for test method names and capitalization.

## Architecture and Key Locations

### Project Structure
- **Application/EdFi.Ods.AdminApi** -- Main web application.
- **Application/EdFi.Ods.AdminApi.V1** -- Version 1 API endpoints and features.
- **Application/EdFi.Ods.AdminApi.Common** -- Shared libraries and utilities.
- **Application/EdFi.Ods.AdminApi.AdminConsole** -- Admin console interface.
- **Application/EdFi.Ods.AdminApi.UnitTests** -- Unit test projects.
- **Application/EdFi.Ods.AdminApi.DBTests** -- Integration test projects.

### Key Configuration Files
- **Application/EdFi.Ods.AdminApi/Properties/launchSettings.json** -- Launch profiles for development.
- **Application/NuGet.Config** -- Package source configuration (contains Ed-Fi private feed).
- **Application/Directory.Packages.props** -- Centralized package version management.

### Important Dependencies
**Ed-Fi Packages (require private NuGet feed access):**
- `EdFi.Suite3.Admin.DataAccess` (7.3.67)
- `EdFi.Suite3.Security.DataAccess` (7.3.431)
- `EdFi.Suite3.Common` (6.2.392)

## Troubleshooting

### Common Build Issues
- **NuGet restore failures**: Verify access to Ed-Fi Alliance private NuGet feed.
- **Build timeouts**: Always set timeouts to 60+ minutes for builds and 40+ minutes for tests.
- **Integration test failures**: Ensure database connection parameters are correct.

### Docker Development
- Use `Docker/V2/Compose/pgsql/SingleTenant/compose-build-dev.yml` for local development.
- Generate SSL certificates: `cd Docker/Settings/ssl && bash ./generate-certificate.sh`.
- Configure environment: Copy `Docker/V2/Compose/pgsql/.env.example` to `.env` and customize.

### Manual Testing Checklist
After any code changes, ALWAYS verify:
1. Application builds without errors.
2. Unit tests pass.
3. Application starts and Swagger UI loads.
4. Health check endpoint responds.
5. Can successfully register and authenticate a client.
6. Core API endpoints return expected responses.

## CI/CD Pipeline
- **Build workflow**: `.github/workflows/on-pullrequest.yml`.
- **E2E tests**: Multiple workflows for different database configurations.
- **Security**: CodeQL analysis and dependency review on all PRs.
- All tests must pass before merging to main branch.
