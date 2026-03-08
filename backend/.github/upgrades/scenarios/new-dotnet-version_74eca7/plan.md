# .NET 10 Upgrade Plan

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
  - [backend.csproj](#backendcsproj)
  - [backend.Tests.csproj](#backendtestscsproj)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

This plan outlines the migration of a 2-project .NET solution from **.NET 9** to **.NET 10 (LTS)**. The solution exhibits a **simple, linear dependency structure** with minimal complexity:

### Solution Profile
- **Total Projects**: 2 (1 application + 1 test project)
- **Dependency Depth**: 1 level
- **Current Framework**: net9.0
- **Target Framework**: net10.0 (Long Term Support through November 2028)

### Issue Breakdown
- **Total Issues**: 17 across both projects
  - **Mandatory**: 2 (Target framework updates required for both projects)
  - **Potential**: 12 (10 behavioral changes in test project + 2 package updates)
  - **Optional**: 3 (Deprecated packages in main project)

### Risk Assessment
**Overall Risk: LOW**

This is a straightforward upgrade with:
- ✅ No breaking API changes requiring code modifications
- ✅ No security vulnerabilities
- ✅ Simple dependency chain (test → app)
- ⚠️ Minor attention needed: 3 deprecated packages requiring replacement
- ⚠️ 10 behavioral changes to verify in tests (assertion message formatting)

### Effort Estimate
- **Developer Time**: 2-4 hours
- **Testing Time**: 1-2 hours
- **Total**: ~6 hours for complete migration and validation

### Recommended Strategy
**All-at-Once Migration** — Update both projects simultaneously in a single coordinated effort. This approach is ideal given the solution's simplicity and minimal risk profile.

---

## Migration Strategy

### Selected Approach: All-at-Once Migration

Given the solution's characteristics, we'll execute a **coordinated, simultaneous update** of all projects:

#### Why All-at-Once?
1. **Simple Structure**: Only 2 projects with 1 dependency level
2. **Low Risk**: No breaking changes, minimal behavioral changes
3. **Efficiency**: Faster than incremental (single build/test cycle)
4. **Consistency**: All projects stay aligned on same .NET version

#### Migration Sequence

The upgrade will follow dependency order (foundation → consumers):

**Phase 1: Foundation Project**
- `backend.csproj` (Level 0 - no dependencies)
  - Update target framework to net10.0
  - Remove deprecated MediatR.Extensions.Microsoft.DependencyInjection
  - Migrate to built-in MediatR DI registration
  - Address deprecated API versioning packages
  - Build and verify

**Phase 2: Test Project**
- `backend.Tests.csproj` (Level 1 - depends on backend)
  - Update target framework to net10.0
  - Upgrade Microsoft.AspNetCore.Mvc.Testing: 9.0.8 → 10.0.3
  - Upgrade Microsoft.EntityFrameworkCore.InMemory: 9.0.8 → 10.0.3
  - Review behavioral changes in test assertions
  - Run full test suite

**Phase 3: Validation**
- Complete solution build
- Full test execution
- Runtime verification

#### Rollback Strategy

All changes are committed to branch `upgrade-to-NET10`. If issues arise:
1. **Immediate rollback**: `git checkout main`
2. **Selective fixes**: Cherry-pick working changes
3. **No risk to production**: Main branch remains on .NET 9

---

## Detailed Dependency Analysis

### Project Hierarchy

The solution follows a clean, single-level dependency structure:

```
Level 0 (Foundation)
└── backend.csproj
    └── Used by: backend.Tests

Level 1 (Application/Tests)
└── backend.Tests.csproj
    └── Depends on: backend
```

### Dependency Graph Details

| Project | Level | Type | Dependencies | Used By | Issues |
|---------|-------|------|--------------|---------|--------|
| **backend.csproj** | 0 | AspNetCore | None | backend.Tests | 4 (1 mandatory) |
| **backend.Tests.csproj** | 1 | DotNetCoreApp | backend | None | 13 (1 mandatory) |

### Migration Order Rationale

**Order: backend.csproj → backend.Tests.csproj**

1. **backend.csproj** must be upgraded first because:
   - It has no dependencies (foundation project)
   - backend.Tests.csproj depends on it
   - Changes to deprecated packages may affect test setup

2. **backend.Tests.csproj** upgraded second because:
   - Depends on backend.csproj
   - Requires upgraded Microsoft packages aligned with .NET 10
   - Test behavioral changes can only be verified after main project is stable

### Critical Dependencies

**Cross-Project References:**
- `backend.Tests.csproj` → `backend.csproj` (project reference)

**Shared Package Ecosystem:**
- Both projects share common .NET runtime
- Test project uses ASP.NET Core testing packages that must align with main project's framework

---

## Project-by-Project Plans

This section provides detailed, step-by-step upgrade instructions for each project in dependency order.

---

### backend.csproj

**Project Type:** AspNetCore (Web API)  
**Current Framework:** net9.0  
**Target Framework:** net10.0  
**Dependency Level:** 0 (Foundation - no dependencies)  
**Complexity:** Low  

#### Overview
The backend project is a standard ASP.NET Core web API with 40 files. It requires updating the target framework and addressing 3 deprecated NuGet packages. No code changes are required, only project file modifications.

#### Issues Summary
- **Total Issues:** 4
  - 1 Mandatory: Target framework update
  - 3 Optional: Deprecated packages requiring migration

#### Step-by-Step Migration Plan

**Step 1: Update Target Framework**
- **Action:** Modify `backend.csproj`
- **Change:** `<TargetFramework>net9.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`
- **Severity:** Mandatory
- **Validation:** Project should restore successfully

**Step 2: Remove Deprecated MediatR.Extensions Package**
- **Current Package:** `MediatR.Extensions.Microsoft.DependencyInjection` v11.1.0
- **Status:** Deprecated - functionality merged into main MediatR package
- **Action:**
  1. Remove `<PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="11.1.0" />`
  2. Verify `MediatR` v14.1.0 is already present (it is)
  3. Update DI registration in code from `services.AddMediatR(...)` to `services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))`
- **Files to Update:**
  - `backend.csproj` (remove package reference)
  - Code file with DI registration (likely `Program.cs` or `Startup.cs`)
- **Severity:** Optional (but recommended)
- **Validation:** Build succeeds, application starts correctly

**Step 3: Evaluate API Versioning Package Migration**
- **Current Packages:**
  - `Microsoft.AspNetCore.Mvc.Versioning` v5.1.0
  - `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` v5.1.0
- **Status:** Deprecated as of August 2022
- **Recommended:** Migrate to `Asp.Versioning.Mvc` and `Asp.Versioning.Mvc.ApiExplorer`
- **Action Options:**
  - **Option A (Recommended):** Migrate to new packages following [migration guide](https://github.com/dotnet/aspnet-api-versioning/wiki/Migration)
  - **Option B:** Keep existing packages (still receive critical bug fixes)
- **Decision Point:** Evaluate project timeline and risk tolerance
- **Severity:** Optional
- **Validation:** If migrated - all API versioning features work, Swagger documentation generates correctly

**Step 4: Build Verification**
- **Action:** Build the backend project in isolation
- **Command:** `dotnet build backend\backend.csproj`
- **Expected Result:** Clean build with zero errors
- **Troubleshooting:** Address any build errors before proceeding

#### Dependencies Affected
**Outgoing:** None  
**Incoming:** `backend.Tests.csproj` (this project will need to be rebuilt after backend changes)

#### Testing Strategy
1. **Build Test:** Successful compilation
2. **Startup Test:** Application starts without errors
3. **Health Check:** Verify key endpoints respond
4. **Dependency Injection Test:** Verify MediatR registration works if modified

#### Rollback Plan
If issues arise:
1. Revert `backend.csproj` to net9.0
2. Restore deprecated package references
3. Revert code changes to DI registration

---

### backend.Tests.csproj

**Project Type:** DotNetCoreApp (Test Project)  
**Current Framework:** net9.0  
**Target Framework:** net10.0  
**Dependency Level:** 1 (Depends on: backend)  
**Complexity:** Low  

#### Overview
The test project contains 21 files with integration and unit tests. It requires framework update, 2 package upgrades, and verification of 10 behavioral changes related to test assertions.

#### Prerequisites
⚠️ **IMPORTANT:** `backend.csproj` must be successfully upgraded to net10.0 before starting this project.

#### Issues Summary
- **Total Issues:** 13
  - 1 Mandatory: Target framework update
  - 2 Potential: Package upgrades recommended
  - 10 Potential: Behavioral changes in assertions

#### Step-by-Step Migration Plan

**Step 1: Update Target Framework**
- **Action:** Modify `backend.Tests.csproj`
- **Change:** `<TargetFramework>net9.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`
- **Severity:** Mandatory
- **Validation:** Project restores successfully

**Step 2: Upgrade Microsoft.AspNetCore.Mvc.Testing**
- **Current Version:** 9.0.8
- **Target Version:** 10.0.3
- **Action:** Update package reference
  ```xml
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.3" />
  ```
- **Reason:** Align with .NET 10 runtime for ASP.NET Core testing features
- **Severity:** Potential (strongly recommended)
- **Validation:** Integration tests compile

**Step 3: Upgrade Microsoft.EntityFrameworkCore.InMemory**
- **Current Version:** 9.0.8
- **Target Version:** 10.0.3
- **Action:** Update package reference
  ```xml
  <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.3" />
  ```
- **Reason:** Align with .NET 10 runtime for in-memory database testing
- **Severity:** Potential (strongly recommended)
- **Validation:** Database setup in tests works correctly

**Step 4: Review HttpContent Behavioral Changes**
- **Affected Type:** `System.Net.Http.HttpContent`
- **Issue Count:** 10 occurrences in `DestinationsControllerIntegrationTests.cs`
- **Affected Lines:** 60, 76, 109, 155, 220, 235, 256, 273, 289, 305
- **Behavioral Change:** `ReadFromJsonAsync` error message formatting may differ in .NET 10
- **Action:**
  1. Review each usage of `response.Content.ReadFromJsonAsync<T>()`
  2. If tests verify error messages, update expected strings
  3. If tests only verify deserialization, no changes needed
- **Severity:** Potential
- **Validation:** All integration tests pass

**Step 5: Build Verification**
- **Action:** Build the test project
- **Command:** `dotnet build backend.Tests\backend.Tests.csproj`
- **Expected Result:** Clean build with zero errors

**Step 6: Execute Full Test Suite**
- **Action:** Run all tests
- **Command:** `dotnet test backend.Tests\backend.Tests.csproj`
- **Expected Result:** All tests pass (same pass rate as .NET 9)
- **Troubleshooting:**
  - If assertion failures occur, review error message differences
  - Update expected values in assertions if behavioral changes are benign

#### Dependencies Affected
**Outgoing:** `backend.csproj` (must be upgraded first)  
**Incoming:** None (this is a top-level test project)

#### Testing Strategy
1. **Build Test:** Successful compilation
2. **Unit Tests:** Run all unit tests, verify pass rate
3. **Integration Tests:** Run all integration tests, specifically verify:
   - DestinationsController endpoints work correctly
   - JSON deserialization works as expected
   - In-memory database operations succeed
4. **Code Coverage:** Ensure coverage metrics remain stable

#### Files Requiring Review
- `backend.Tests\Integration\DestinationsControllerIntegrationTests.cs` (10 behavioral change locations)
- Any test files that verify error message text

#### Rollback Plan
If tests fail:
1. Revert `backend.Tests.csproj` to net9.0
2. Downgrade packages to 9.0.8 versions
3. Analyze test failures to determine if they're legitimate issues or test assertion problems

---

## Package Update Reference

This section provides a comprehensive reference for all NuGet packages in the solution, their current state, and required actions.

### Summary Statistics
- **Total Packages:** 21
- **✅ Compatible (no action needed):** 16
- **⬆️ Upgrade Recommended:** 2
- **⚠️ Deprecated (migration recommended):** 3

---

### Packages Requiring Action

#### 🔴 High Priority Updates

| Package | Current | Target | Action | Project(s) | Reason |
|---------|---------|--------|--------|-----------|---------|
| **Microsoft.AspNetCore.Mvc.Testing** | 9.0.8 | 10.0.3 | Upgrade | backend.Tests | Align with .NET 10 runtime |
| **Microsoft.EntityFrameworkCore.InMemory** | 9.0.8 | 10.0.3 | Upgrade | backend.Tests | Align with .NET 10 runtime |

**Update Commands:**
```bash
# From backend.Tests directory
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 10.0.3
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 10.0.3
```

#### 🟡 Deprecated Packages (Migration Recommended)

| Package | Version | Status | Replacement | Project(s) | Migration Priority |
|---------|---------|--------|-------------|-----------|-------------------|
| **MediatR.Extensions.Microsoft.DependencyInjection** | 11.1.0 | Deprecated | Built into MediatR 14.1.0+ | backend | Medium |
| **Microsoft.AspNetCore.Mvc.Versioning** | 5.1.0 | Deprecated | Asp.Versioning.Mvc | backend | Low |
| **Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer** | 5.1.0 | Deprecated | Asp.Versioning.Mvc.ApiExplorer | backend | Low |

**Deprecation Details:**

**MediatR.Extensions.Microsoft.DependencyInjection**
- **Reason:** Functionality merged into main MediatR package as of v12.0
- **Migration Steps:**
  1. Remove package reference
  2. Update service registration:
     ```csharp
     // Old
     services.AddMediatR(typeof(Program).Assembly);

     // New
     services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
     ```
- **Documentation:** Built-in to MediatR 14.1.0 (already in your project)

**Microsoft.AspNetCore.Mvc.Versioning (Both packages)**
- **Reason:** New package namespace as of August 2022
- **Migration Guide:** https://github.com/dotnet/aspnet-api-versioning/wiki/Migration
- **Migration Steps:**
  1. Remove old packages
  2. Install new packages:
     ```bash
     dotnet add package Asp.Versioning.Mvc
     dotnet add package Asp.Versioning.Mvc.ApiExplorer
     ```
  3. Update namespace imports: `Microsoft.AspNetCore.Mvc.Versioning` → `Asp.Versioning`
  4. Update service registration (minor API changes)
- **Note:** Old packages still receive critical bug fixes but won't get new features

---

### Compatible Packages (No Action Required)

These packages are already compatible with .NET 10 and require no changes:

#### Test Packages
- ✅ **coverlet.collector** v6.0.0
- ✅ **FluentAssertions** v6.12.0
- ✅ **Microsoft.NET.Test.Sdk** v17.8.0
- ✅ **Moq** v4.20.69
- ✅ **xunit** v2.6.1
- ✅ **xunit.runner.visualstudio** v2.5.3

#### Application Packages
- ✅ **MediatR** v14.1.0
- ✅ **Microsoft.AspNetCore.OpenApi** v10.0.3 (already .NET 10)
- ✅ **Swashbuckle.AspNetCore** v10.1.4

#### Logging Packages (Serilog)
- ✅ **Serilog.AspNetCore** v10.0.0
- ✅ **Serilog.Enrichers.Environment** v3.0.1
- ✅ **Serilog.Enrichers.Process** v3.0.0
- ✅ **Serilog.Enrichers.Thread** v4.0.0
- ✅ **Serilog.Settings.Configuration** v10.0.0
- ✅ **Serilog.Sinks.Console** v6.1.1
- ✅ **Serilog.Sinks.File** v7.0.0

---

### Package Update Strategy

1. **Required Updates** (Do first):
   - Microsoft.AspNetCore.Mvc.Testing → 10.0.3
   - Microsoft.EntityFrameworkCore.InMemory → 10.0.3

2. **Recommended Deprecation Fixes** (Do during upgrade):
   - Remove MediatR.Extensions.Microsoft.DependencyInjection
   - Update MediatR service registration

3. **Optional Migrations** (Can defer):
   - API Versioning packages (evaluate based on project timeline)

---

## Breaking Changes Catalog

This section documents all breaking changes and behavioral modifications identified in the .NET 9 → .NET 10 upgrade.

### Summary
- **Breaking Code Changes:** 0 (no code will fail to compile)
- **Behavioral Changes:** 10 (runtime behavior differences)
- **API Deprecations:** 3 (packages marked obsolete)

---

### 1. HttpContent.ReadFromJsonAsync Behavioral Changes

**Issue ID:** Api.0003  
**Severity:** Potential  
**Impact:** Behavioral change in error message formatting

#### Description
`System.Net.Http.HttpContent.ReadFromJsonAsync<T>()` may produce different error messages in .NET 10 when deserialization fails.

#### Affected Code
**File:** `backend.Tests\Integration\DestinationsControllerIntegrationTests.cs`  
**Occurrences:** 10

**Affected Lines:**
1. Line 60: `var destinations = await response.Content.ReadFromJsonAsync<PagedResultDto<DestinationDto>>();`
2. Line 76: `var result = await response.Content.ReadFromJsonAsync<DestinationDto>();`
3. Line 109: `var result = await response.Content.ReadFromJsonAsync<DestinationDto>();`
4. Line 155: `var result = await response.Content.ReadFromJsonAsync<DestinationDto>();`
5. Line 220: `var countries = await response.Content.ReadFromJsonAsync<List<string>>();`
6. Line 235: `var types = await response.Content.ReadFromJsonAsync<List<string>>();`
7. Line 256: `var result = await response.Content.ReadFromJsonAsync<PagedResultDto<DestinationDto>>();`
8. Line 273: `var result = await response.Content.ReadFromJsonAsync<PagedResultDto<DestinationDto>>();`
9. Line 289: `var result = await response.Content.ReadFromJsonAsync<PagedResultDto<DestinationDto>>();`
10. Line 305: `var result = await response.Content.ReadFromJsonAsync<PagedResultDto<DestinationDto>>();`

#### Migration Actions
**No code changes required** unless tests verify specific error message text.

**If tests check error messages:**
1. Run tests to identify failures
2. Update expected error message strings to match .NET 10 format
3. Consider using more flexible assertions (e.g., `Should().Contain()` instead of exact match)

**If tests only check successful deserialization:**
- No action needed; behavior remains functionally identical

#### Risk Level
**Low** - This is a formatting change, not a functional change. The deserialization logic works identically.

---

### 2. MediatR.Extensions.Microsoft.DependencyInjection Deprecation

**Issue ID:** NuGet.0005  
**Severity:** Optional  
**Impact:** Package removed from active development

#### Description
The separate DI extensions package is deprecated as of MediatR v12.0. Functionality is now built into the main MediatR package.

#### Affected Code
**File:** `backend\backend.csproj`  
**Package:** MediatR.Extensions.Microsoft.DependencyInjection v11.1.0

#### Migration Actions
1. **Remove package reference** from backend.csproj
2. **Update service registration** (likely in Program.cs or Startup.cs):

**Old Registration:**
```csharp
services.AddMediatR(typeof(Program).Assembly);
// or
services.AddMediatR(typeof(Program));
```

**New Registration:**
```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

#### Files to Modify
- `backend\backend.csproj` (remove PackageReference)
- Service registration file (search for `AddMediatR` call)

#### Risk Level
**Very Low** - Simple API change with direct equivalent

---

### 3. API Versioning Packages Deprecation

**Issue ID:** NuGet.0005  
**Severity:** Optional  
**Impact:** No new features, critical bugs still fixed

#### Description
Microsoft.AspNetCore.Mvc.Versioning packages deprecated in August 2022. New namespace available: Asp.Versioning.

#### Affected Code
**File:** `backend\backend.csproj`  
**Packages:**
- Microsoft.AspNetCore.Mvc.Versioning v5.1.0
- Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer v5.1.0

#### Migration Actions
**Option 1: Migrate to New Packages (Recommended)**
1. Remove old package references
2. Add new packages:
   ```bash
   dotnet add package Asp.Versioning.Mvc
   dotnet add package Asp.Versioning.Mvc.ApiExplorer
   ```
3. Update namespaces:
   ```csharp
   // Old
   using Microsoft.AspNetCore.Mvc.Versioning;

   // New
   using Asp.Versioning;
   ```
4. Review service registration for minor API changes
5. Follow migration guide: https://github.com/dotnet/aspnet-api-versioning/wiki/Migration

**Option 2: Keep Existing Packages**
- No action required
- Continue receiving critical bug fixes
- Defer migration to future sprint

#### Files to Modify (if migrating)
- `backend\backend.csproj`
- Any files with `using Microsoft.AspNetCore.Mvc.Versioning;`
- Service configuration file

#### Risk Level
**Medium** - If migrated: requires code changes across multiple files  
**Low** - If deferred: existing functionality continues to work

---

### 4. Target Framework Change

**Issue ID:** Project.0002  
**Severity:** Mandatory  
**Impact:** Fundamental framework update

#### Description
Both projects must update their target framework from net9.0 to net10.0.

#### Affected Files
- `backend\backend.csproj`
- `backend.Tests\backend.Tests.csproj`

#### Migration Actions
Update `<TargetFramework>` element in both project files:

```xml
<!-- Old -->
<TargetFramework>net9.0</TargetFramework>

<!-- New -->
<TargetFramework>net10.0</TargetFramework>
```

#### Risk Level
**Very Low** - Mechanical change, no behavioral impact beyond runtime version

---

## Risk Management

### Overall Risk Profile: **LOW**

This upgrade presents minimal risk due to the solution's simplicity and lack of breaking changes.

---

### Risk Assessment Matrix

| Risk Category | Likelihood | Impact | Severity | Mitigation |
|---------------|-----------|--------|----------|------------|
| Build Failures | Very Low | Medium | **LOW** | Simple framework update, tested packages |
| Test Failures | Low | Medium | **LOW** | Behavioral changes limited to error messages |
| Runtime Errors | Very Low | High | **LOW** | No breaking API changes identified |
| Dependency Conflicts | Very Low | Medium | **LOW** | All packages .NET 10 compatible |
| Deprecated Package Issues | Low | Low | **LOW** | Clear migration paths documented |
| Performance Regression | Very Low | Medium | **LOW** | .NET 10 generally improves performance |

---

### Identified Risks

#### 1. Test Assertion Failures (Behavioral Changes)
**Probability:** Low (30%)  
**Impact:** Low  
**Risk Level:** LOW

**Description:** 10 test locations use `ReadFromJsonAsync` which may have different error message formatting in .NET 10.

**Mitigation:**
- Review test assertions to determine if they check error messages
- Most tests likely only verify successful deserialization (no impact)
- If failures occur, update expected error strings

**Contingency:**
- Tests can be fixed quickly (simple string updates)
- Does not block deployment

---

#### 2. MediatR DI Registration Issues
**Probability:** Low (20%)  
**Impact:** Medium  
**Risk Level:** LOW

**Description:** Updating MediatR service registration syntax may be missed or incorrectly implemented.

**Mitigation:**
- Clear code examples provided in plan
- Verify application startup after change
- Test MediatR handler resolution

**Contingency:**
- Easy to revert deprecated package if issues arise
- Compiler will catch missing registrations

---

#### 3. API Versioning Migration Complexity
**Probability:** Medium (if migrated)  
**Impact:** Medium  
**Risk Level:** LOW (can be deferred)

**Description:** Migrating API versioning packages requires code changes across multiple files.

**Mitigation:**
- **Defer migration** - This is optional; old packages still work
- If migrating: follow official migration guide step-by-step
- Test all versioned endpoints after migration

**Contingency:**
- Keep existing packages (Option B from plan)
- Schedule migration for future sprint

---

#### 4. Package Version Conflicts
**Probability:** Very Low (5%)  
**Impact:** Medium  
**Risk Level:** VERY LOW

**Description:** Upgraded packages might have transitive dependency conflicts.

**Mitigation:**
- All specified package versions are tested and compatible
- NuGet resolver handles transitive dependencies automatically

**Contingency:**
- Downgrade to 9.0.x versions of Microsoft packages
- Review dependency tree with `dotnet list package --include-transitive`

---

### Risk Mitigation Strategies

#### Pre-Migration
1. ✅ **Branch Isolation**: All work on `upgrade-to-NET10` branch
2. ✅ **Assessment Complete**: Full compatibility analysis performed
3. ✅ **Backup Plan**: Main branch remains on .NET 9

#### During Migration
1. **Incremental Validation**: Build after each project update
2. **Test Early**: Run test suite immediately after test project upgrade
3. **Commit Frequently**: Commit after each successful phase
4. **Monitor Build Output**: Watch for warnings that could indicate issues

#### Post-Migration
1. **Full Test Suite**: Execute all unit and integration tests
2. **Manual Testing**: Verify key application features work
3. **Performance Baseline**: Compare startup time and response times
4. **Staged Rollout**: Deploy to dev environment first

---

### Rollback Procedures

#### Quick Rollback (Any Point)
```bash
git checkout main
```
**Result:** Immediate return to .NET 9

#### Selective Rollback (After Commit)
```bash
# Return to .NET 9 but keep other improvements
git checkout main
git cherry-pick <commit-hash>  # Pick specific commits to keep
```

#### Partial Rollback (Single Project)
If backend.Tests fails but backend works:
1. Revert backend.Tests.csproj to net9.0
2. Downgrade test packages to 9.0.8
3. Keep backend on net10.0 temporarily

---

## Testing & Validation Strategy

### Testing Phases

#### Phase 1: Build Validation
**Objective:** Ensure all projects compile successfully

**Steps:**
1. Clean solution: `dotnet clean`
2. Restore packages: `dotnet restore`
3. Build backend: `dotnet build backend\backend.csproj`
4. Build tests: `dotnet build backend.Tests\backend.Tests.csproj`
5. Build solution: `dotnet build backend.sln`

**Success Criteria:**
- ✅ Zero compilation errors
- ✅ Zero blocking warnings
- ✅ All package references resolved

---

#### Phase 2: Automated Testing
**Objective:** Verify functional correctness

**Test Execution:**
```bash
# Run all tests
dotnet test backend.Tests\backend.Tests.csproj

# Run with detailed output
dotnet test backend.Tests\backend.Tests.csproj --verbosity normal

# Generate code coverage (if configured)
dotnet test backend.Tests\backend.Tests.csproj --collect:"XPlat Code Coverage"
```

**Success Criteria:**
- ✅ **Test Pass Rate:** 100% (same as .NET 9 baseline)
- ✅ **Code Coverage:** No decrease from baseline
- ✅ **Test Execution Time:** Within 10% of baseline

**Expected Issues:**
- Possible assertion failures in `DestinationsControllerIntegrationTests.cs` due to error message formatting
- Action: Update expected strings if failures occur

---

#### Phase 3: Runtime Validation
**Objective:** Verify application runs correctly in .NET 10 runtime

**Steps:**
1. **Application Startup:**
   ```bash
   cd backend
   dotnet run
   ```
   - Verify application starts without errors
   - Check console for startup warnings
   - Confirm listening on expected ports

2. **Health Checks:**
   - Navigate to health endpoint (if configured)
   - Verify Swagger UI loads: `https://localhost:<port>/swagger`
   - Check application logs for errors

3. **Functional Testing:**
   - Test key API endpoints manually
   - Verify CRUD operations work
   - Check API versioning endpoints
   - Test MediatR command/query handlers

**Success Criteria:**
- ✅ Application starts in < 10 seconds
- ✅ No runtime exceptions in logs
- ✅ Swagger documentation generates correctly
- ✅ Sample API calls return expected results

---

#### Phase 4: Integration Testing
**Objective:** Verify system components work together

**Test Areas:**
1. **Database Operations:**
   - Verify in-memory database in tests works
   - Check Entity Framework queries execute correctly

2. **MediatR Integration:**
   - Verify handlers are discovered and registered
   - Test command execution
   - Test query execution
   - Confirm validation pipeline works

3. **API Versioning:**
   - Test version headers
   - Verify version endpoint routing
   - Check API Explorer functionality

4. **Logging:**
   - Verify Serilog configuration loads
   - Check logs are written to expected sinks
   - Confirm log enrichers work (Environment, Process, Thread)

**Success Criteria:**
- ✅ All integration tests pass
- ✅ All system components initialized correctly
- ✅ No regression in functionality

---

### Test Failure Response Plan

#### If Build Fails
1. Review error messages
2. Check for missing package references
3. Verify target framework syntax
4. Consult project-specific plan sections

#### If Tests Fail
1. **Identify failure category:**
   - Assertion failures → likely error message formatting (low risk)
   - Null reference errors → investigate dependency injection
   - Timeout errors → investigate performance regression

2. **For assertion failures:**
   - Review test code at failing line
   - Update expected values if behavioral change is benign
   - Re-run tests

3. **For dependency injection errors:**
   - Verify MediatR registration updated correctly
   - Check service registrations in Program.cs
   - Review DI container logs

#### If Runtime Errors Occur
1. Check application logs for stack traces
2. Verify deprecated packages migrated correctly
3. Test specific functionality showing errors
4. Consult breaking changes catalog

---

### Performance Validation

**Baseline Metrics (Capture Before Upgrade):**
- Application startup time
- Average API response time (key endpoints)
- Memory usage at idle
- Test suite execution time

**Post-Upgrade Validation:**
- Compare metrics to baseline
- Expected: Minor improvements (. NET 10 has performance enhancements)
- Alert if: >10% degradation in any metric

**Tools:**
- `dotnet-counters` for runtime metrics
- Application Insights (if configured)
- Test execution time from test runner

---

### Validation Checklist

Before marking upgrade complete, verify:

- [ ] All projects build without errors
- [ ] All projects build without warnings (or warnings reviewed and accepted)
- [ ] Full test suite passes (100% of previously passing tests)
- [ ] Application starts successfully
- [ ] Swagger UI loads and displays API documentation
- [ ] Sample API calls to key endpoints work
- [ ] MediatR handlers execute successfully
- [ ] Logging configuration works
- [ ] In-memory database in tests functions correctly
- [ ] No new runtime exceptions in logs
- [ ] Performance metrics within acceptable range
- [ ] All deprecated packages either migrated or documented for future migration
- [ ] Code committed to upgrade branch
- [ ] README or documentation updated with .NET 10 requirement

---

## Complexity & Effort Assessment

### Solution Complexity: **SIMPLE**

**Characteristics:**
- 2 projects (well below 5-project threshold)
- 1-level dependency depth
- No multi-targeting
- No platform-specific code
- Modern SDK-style projects
- No legacy dependencies

---

### Effort Breakdown

#### Developer Time Estimates

| Phase | Activity | Estimated Time | Confidence |
|-------|----------|----------------|------------|
| **Phase 1: Backend Project** | | | |
| | Update target framework | 5 min | High |
| | Remove deprecated MediatR package | 15 min | High |
| | Update MediatR DI registration | 20 min | Medium |
| | Build and verify | 10 min | High |
| | **Phase 1 Subtotal** | **50 min** | |
| **Phase 2: Test Project** | | | |
| | Update target framework | 5 min | High |
| | Upgrade Microsoft packages | 10 min | High |
| | Review behavioral changes | 30 min | Medium |
| | Build and verify | 10 min | High |
| | **Phase 2 Subtotal** | **55 min** | |
| **Phase 3: Validation** | | | |
| | Run test suite | 15 min | High |
| | Manual testing | 30 min | Medium |
| | Runtime verification | 20 min | Medium |
| | **Phase 3 Subtotal** | **65 min** | |
| **Phase 4: Optional API Versioning** | | | |
| | Package migration (if done) | 60 min | Low |
| | Testing versioned endpoints | 30 min | Medium |
| | **Phase 4 Subtotal** | **90 min** | |
| **Contingency & Documentation** | | | |
| | Issue resolution buffer | 45 min | Medium |
| | Documentation updates | 15 min | High |
| | **Other Subtotal** | **60 min** | |

#### Total Time Estimates

**Minimum Path** (defer API versioning migration):
- Development: ~2 hours
- Testing: ~1.5 hours
- **Total: 3.5 hours**

**Complete Path** (include API versioning migration):
- Development: ~3.5 hours
- Testing: ~2 hours
- **Total: 5.5 hours**

**Recommended Approach:**
- **Sprint 1:** Core upgrade without API versioning (3.5 hours)
- **Future Sprint:** API versioning migration if needed (1.5 hours)

---

### Complexity Factors

#### Low Complexity Indicators ✅
- Modern .NET (9 → 10, not legacy .NET Framework)
- SDK-style projects (no manual XML editing)
- Simple dependency structure
- Well-maintained packages (Serilog, MediatR, xUnit)
- No breaking API changes
- All deprecated packages have clear migration paths

#### Minimal Complexity Indicators ⚠️
- 3 deprecated packages (but optional to migrate)
- 10 behavioral changes (but only error message formatting)
- Test project has 13 issues (but 10 are related to same behavioral change)

#### No High Complexity Indicators ✅
- No security vulnerabilities requiring immediate patching
- No incompatible packages requiring rewrites
- No platform-specific code requiring TFM changes
- No third-party dependencies without .NET 10 support

---

### Skill Level Required

**Minimum Skills:**
- Basic understanding of .NET project files
- Ability to run command-line build tools
- Understanding of NuGet package management

**Recommended Skills:**
- Experience with dependency injection in .NET
- Familiarity with MediatR pattern
- Understanding of ASP.NET Core testing practices

**Not Required:**
- Deep .NET runtime internals knowledge
- Migration from .NET Framework experience
- Advanced MSBuild knowledge

---

### Timeline Recommendation

**Single Developer:**
- **Day 1 Morning:** Backend project upgrade + MediatR migration (1.5 hours)
- **Day 1 Afternoon:** Test project upgrade + initial testing (2 hours)
- **Day 2 Morning:** Full validation + documentation (1.5 hours)
- **Total:** 2 business days (half-day buffer)

**Team Approach:**
- **Developer A:** Backend project
- **Developer B:** Test project
- **Both:** Joint validation
- **Total:** 1 business day

---

## Source Control Strategy

### Branch Structure

**Source Branch:** `main` (production-ready .NET 9 code)  
**Upgrade Branch:** `upgrade-to-NET10` (already created)  
**Target Branch:** `main` (after successful validation)

---

### Commit Strategy

Organize commits by phase for clear history and easy rollback:

#### Recommended Commit Sequence

```
upgrade-to-NET10
├── Commit 1: "chore: update backend target framework to net10.0"
│   └── Changes: backend.csproj (TargetFramework only)
│
├── Commit 2: "refactor: remove deprecated MediatR.Extensions package"
│   ├── backend.csproj (remove package)
│   └── Program.cs (update DI registration)
│
├── Commit 3: "build: update backend.Tests target framework to net10.0"
│   └── backend.Tests.csproj (TargetFramework only)
│
├── Commit 4: "build: upgrade Microsoft test packages to 10.0.3"
│   └── backend.Tests.csproj (package versions)
│
├── Commit 5: "test: update assertions for .NET 10 behavioral changes"
│   └── DestinationsControllerIntegrationTests.cs (if needed)
│
└── Commit 6: "docs: update README for .NET 10 requirement"
    └── README.md
```

**Benefits:**
- Each commit is independently revertable
- Clear change history
- Easy to cherry-pick specific changes
- Facilitates code review

---

### Merge Strategy

#### Option 1: Pull Request (Recommended)
```bash
# After all changes committed and validated
git push origin upgrade-to-NET10

# Create PR: upgrade-to-NET10 → main
# Review, approve, merge
```

**Benefits:**
- Code review opportunity
- CI/CD validation before merge
- Team visibility
- Documented decision trail

#### Option 2: Direct Merge
```bash
# If single developer or low-ceremony process
git checkout main
git merge upgrade-to-NET10
git push origin main
```

---

### Rollback Scenarios

#### Scenario 1: Issues Found During Development
```bash
# Discard all changes and start over
git checkout main
git branch -D upgrade-to-NET10
git checkout -b upgrade-to-NET10
```

#### Scenario 2: Issues Found After Merge
```bash
# Revert the merge commit
git revert -m 1 <merge-commit-hash>
git push origin main
```

#### Scenario 3: Issues in Production
```bash
# Fast rollback: deploy previous version
# Or: revert merge and deploy

# Investigation branch
git checkout -b hotfix/net10-issues upgrade-to-NET10
# Fix issues, test, merge
```

---

### Branch Retention

- **Keep `upgrade-to-NET10` branch** for 30 days post-merge
- Allows easy reference if issues arise
- Can be deleted after confidence period

---

## Success Criteria

The upgrade will be considered successful when ALL criteria below are met:

### 🎯 Mandatory Criteria

#### Build & Compilation
- [ ] ✅ Solution builds without errors: `dotnet build backend.sln`
- [ ] ✅ Zero blocking warnings introduced by upgrade
- [ ] ✅ All NuGet packages restore successfully
- [ ] ✅ Both projects target `net10.0` framework

#### Testing
- [ ] ✅ All unit tests pass: `dotnet test`
- [ ] ✅ All integration tests pass
- [ ] ✅ Test pass rate equals or exceeds .NET 9 baseline (100%)
- [ ] ✅ No new test failures introduced

#### Runtime
- [ ] ✅ Application starts without errors
- [ ] ✅ No runtime exceptions during startup
- [ ] ✅ Key API endpoints respond correctly
- [ ] ✅ Swagger documentation generates successfully

#### Dependencies
- [ ] ✅ `Microsoft.AspNetCore.Mvc.Testing` upgraded to 10.0.3
- [ ] ✅ `Microsoft.EntityFrameworkCore.InMemory` upgraded to 10.0.3
- [ ] ✅ Deprecated `MediatR.Extensions.Microsoft.DependencyInjection` removed
- [ ] ✅ MediatR DI registration updated to new syntax

---

### 🎯 Validation Criteria

#### Functional Testing
- [ ] ✅ CRUD operations on Destinations API work correctly
- [ ] ✅ MediatR commands and queries execute successfully
- [ ] ✅ API versioning endpoints respond correctly
- [ ] ✅ Logging pipeline functions (Serilog writes to configured sinks)

#### Integration Testing
- [ ] ✅ In-memory database operations in tests work
- [ ] ✅ WebApplicationFactory test infrastructure works
- [ ] ✅ Dependency injection container resolves all services

#### Performance
- [ ] ✅ Application startup time ≤ .NET 9 baseline (or improved)
- [ ] ✅ Test execution time within 110% of baseline
- [ ] ✅ No obvious performance regressions in API responses

---

### 🎯 Documentation Criteria

#### Code Documentation
- [ ] ✅ README updated with .NET 10 SDK requirement
- [ ] ✅ Any setup instructions updated (if SDK version mentioned)
- [ ] ✅ CI/CD configuration updated for .NET 10 (if applicable)

#### Change Documentation
- [ ] ✅ Commit history clearly documents upgrade steps
- [ ] ✅ PR description (if using PR workflow) explains changes
- [ ] ✅ Any deprecated package migrations documented in code comments

---

### 🎯 Optional Success Criteria

These are desirable but not required for initial upgrade completion:

#### API Versioning Migration
- [ ] ⭐ `Microsoft.AspNetCore.Mvc.Versioning` migrated to `Asp.Versioning.Mvc`
- [ ] ⭐ `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` migrated to `Asp.Versioning.Mvc.ApiExplorer`
- [ ] ⭐ All versioned endpoints tested and working

**Note:** API versioning migration can be deferred to future sprint

---

### 📊 Acceptance Checklist

Before considering upgrade complete, verify:

**Pre-Deployment:**
- [ ] All mandatory criteria met
- [ ] All validation criteria met
- [ ] Code reviewed (if team process requires)
- [ ] Changes merged to main branch
- [ ] Git tags applied (e.g., `v1.0-net10`)

**Post-Deployment (Dev Environment):**
- [ ] Application deployed to dev environment successfully
- [ ] Smoke tests pass in dev environment
- [ ] Logs reviewed for any warnings/errors
- [ ] Performance metrics captured and reviewed

**Production Readiness:**
- [ ] Deployment plan documented
- [ ] Rollback plan tested
- [ ] Team notified of upgrade
- [ ] Monitoring alerts configured for new runtime

---

### 🚫 Failure Criteria

Upgrade should be rolled back if:

- ❌ Build fails and cannot be resolved within 2 hours
- ❌ More than 10% of tests fail
- ❌ Critical application functionality broken
- ❌ Performance degradation >20% in key metrics
- ❌ Unresolvable runtime errors in core features

---

### ✅ Sign-Off

**Upgrade Complete When:**
1. All mandatory criteria checked ✅
2. All validation criteria checked ✅
3. Documentation criteria checked ✅
4. Application deployed to dev environment successfully
5. Team lead/stakeholder approval obtained (if required)

**Responsible Party:** Lorelay Pricop Florescu
**Review Date:** 08/03/2026 
**Status:** [ ] Complete [X] Incomplete [ ] Rolled Back
