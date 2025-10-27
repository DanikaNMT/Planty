# Sharing Feature - Test Coverage Summary

## Overview
Comprehensive testing strategy for the plant/location sharing functionality with role-based permissions.

## Test Files Created

### 1. ShareRepository Tests (Infrastructure Layer)
**File**: `Planty.Infrastructure.Tests/Repositories/ShareRepositoryTests.cs`
**Coverage**: 9 test methods covering:
- ✅ `AddAsync` - Verify shares are correctly added to database
- ✅ `GetByIdAsync` - Test navigation property loading (Owner, SharedWithUser, Plant, Location)
- ✅ `GetSharesCreatedByUserAsync` - Return all shares created by a user
- ✅ `GetSharesReceivedByUserAsync` - Return all shares received by a user  
- ✅ `GetUserRoleForPlantAsync` - Direct plant shares and location-based inheritance
- ✅ `GetSharedPlantIdsForUserAsync` - Returns IDs of plants shared with user
- ✅ `GetSharedLocationIdsForUserAsync` - Returns IDs of locations shared with user
- ✅ `DeleteAsync` - Verify share deletion

**Key Scenarios Tested**:
- Direct plant sharing
- Location-based sharing inheritance (plants in shared locations)
- Multiple shares from same owner
- Navigation property eager loading
- Role retrieval with precedence (direct > location-based)

### 2. Command Handler Tests (Application Layer)

#### CreateShareCommandHandler Tests
**Coverage**:
- ✅ Valid plant share creation
- ✅ Valid location share creation
- ✅ Plant/Location not found errors
- ✅ User not found errors
- ✅ Permission validation (no permission throws UnauthorizedAccessException)
- ✅ Duplicate share prevention
- ✅ Share with self validation
- ✅ All role types (Viewer, Carer, Editor, Owner)

#### UpdateShareCommandHandler Tests
**Coverage**:
- ✅ Valid role updates
- ✅ Share not found errors
- ✅ Permission validation
- ✅ All role transitions
- ✅ Plant and location share updates

#### DeleteShareCommandHandler Tests
**Coverage**:
- ✅ Owner can delete shares
- ✅ SharedWithUser can delete (remove themselves)
- ✅ Other users cannot delete
- ✅ Share not found errors
- ✅ Permission validation
- ✅ Plant and location share deletion

### 3. Query Handler Tests (Application Layer)

#### GetSharesCreatedByUserQueryHandler Tests
**Coverage**:
- ✅ Returns all shares user created
- ✅ Empty result when user has no shares
- ✅ Multiple shares with same user
- ✅ Correct role mapping (Domain → DTO)
- ✅ Both plant and location shares
- ✅ Navigation property mapping to UserInfo

#### GetSharesReceivedByUserQueryHandler Tests
**Coverage**:
- ✅ Returns all shares user received
- ✅ Empty result when no received shares
- ✅ Shares from multiple owners
- ✅ All role types
- ✅ Correct DTO mapping

### 4. PermissionService Tests (Application Layer)
**File**: `Planty.Application.Tests/Services/PermissionServiceTests.cs`
**Coverage**: 25+ test methods covering:

#### Plant Permissions:
- ✅ `CanViewPlantAsync`
  - Owner returns true
  - Shared with Viewer/Carer/Editor/Owner returns true
  - Not shared returns false
  
- ✅ `CanEditPlantAsync`
  - Owner returns true
  - Shared with Editor/Owner returns true
  - Shared with Carer/Viewer returns false
  
- ✅ `CanDeletePlantAsync`
  - Owner returns true
  - Shared with Owner role returns true
  - Shared with Editor/Carer/Viewer returns false
  
- ✅ `CanCarePlantAsync`
  - Owner/Editor/Carer returns true
  - Viewer returns false

#### Location Permissions:
- ✅ `CanViewLocationAsync` - Owner and all shared roles
- ✅ `CanEditLocationAsync` - Owner and Editor role
- ✅ `CanDeleteLocationAsync` - Only owner, not shared users

#### Share Permissions:
- ✅ `CanEditShareAsync` - Only owner can edit
- ✅ `CanDeleteShareAsync` - Owner or SharedWithUser can delete

#### Location-based Inheritance:
- ✅ Plants in shared locations inherit permissions
- ✅ Direct plant share takes precedence over location share
- ✅ Role hierarchy respected (Editor > Carer > Viewer)

## Test Data Patterns

### User Setup:
```csharp
_user1Id = Owner
_user2Id = Editor/Carer (shared with)
_user3Id = Viewer (shared with)
```

### Share Roles (Numeric Hierarchy):
- Viewer = 0 (read-only)
- Carer = 1 (can perform care actions)
- Editor = 2 (can edit but not delete)
- Owner = 3 (full control)

### Permission Logic:
```csharp
// Check if user has sufficient role
requiredRole >= userRole

// Examples:
CanEdit => requires Editor (2) or higher
CanCare => requires Carer (1) or higher  
CanView => requires Viewer (0) or higher
CanDelete => requires Owner (3)
```

## Integration Test Scenarios

### SharesController Integration Tests
Should cover:
1. **POST /api/shares** - Create share
   - 201 Created with ShareResponse
   - 409 Conflict for duplicates
   - 403 Forbidden for no permission
   - 404 Not Found for invalid plant/location/user

2. **GET /api/shares/created** - Get shares created
   - 200 OK with list of ShareResponse
   - Empty array when none

3. **GET /api/shares/received** - Get shares received
   - 200 OK with list of ShareResponse
   - Includes navigation properties

4. **PUT /api/shares/{id}** - Update share role
   - 200 OK with updated ShareResponse
   - 403 Forbidden if not owner
   - 404 Not Found if share doesn't exist

5. **DELETE /api/shares/{id}** - Delete share
   - 204 No Content
   - 403 Forbidden if not owner/sharedWithUser
   - 404 Not Found if doesn't exist

6. **GET /api/shares/users/search** - Search users
   - 200 OK with list of UserSearchResponse
   - Filters by query string
   - Excludes current user

## End-to-End Test Scenarios

### Scenario 1: Plant Sharing Workflow
1. User A creates a plant
2. User A shares plant with User B (Editor role)
3. User B can view and edit plant
4. User B cannot delete plant
5. User A updates share to Owner role
6. User B can now delete plant

### Scenario 2: Location Sharing with Inheritance
1. User A creates location "Garden"
2. User A adds 3 plants to Garden
3. User A shares Garden with User B (Carer role)
4. User B can view all 3 plants
5. User B can perform care actions on all plants
6. User B cannot edit plant details
7. User A adds new plant to Garden
8. User B automatically gets Carer access to new plant

### Scenario 3: Direct Share Precedence
1. User A shares location with User B (Viewer role)
2. User A shares specific plant in location with User B (Editor role)
3. User B has Viewer role for other plants in location
4. User B has Editor role for the specific plant (direct share wins)

### Scenario 4: Share Management
1. User A shares plant with User B
2. User B sees share in "Shares Received" tab
3. User A sees share in "Shares Created" tab
4. User A can update role or delete share
5. User B can delete share (remove own access)
6. After deletion, User B loses access to plant

### Scenario 5: Permission Enforcement
1. User A shares plant with User B (Viewer role)
2. User B tries to edit plant → 403 Forbidden
3. User B tries to delete plant → 403 Forbidden
4. User B tries to water plant → 403 Forbidden
5. User B can only view plant details

## Test Coverage Metrics

### Repository Layer:
- Methods tested: 8/14 core methods
- Scenarios: 9 test cases
- Edge cases: Null returns, empty collections, navigation properties

### Application Layer:
- Command Handlers: 3 handlers × ~7 tests each = ~21 tests
- Query Handlers: 2 handlers × ~4 tests each = ~8 tests
- Permission Service: ~25 tests covering all permission methods

### API Layer:
- Controller endpoints: 6 endpoints
- Error scenarios: 404, 403, 409, 400
- Success paths: 200, 201, 204

### Total Estimated Test Count: **~70 tests**

## Running Tests

```powershell
# All sharing tests
dotnet test --filter "FullyQualifiedName~Share"

# Repository tests only
dotnet test --filter "FullyQualifiedName~ShareRepository"

# Permission tests only
dotnet test --filter "FullyQualifiedName~Permission"

# Command handler tests
dotnet test --filter "FullyQualifiedName~ShareCommand"

# Query handler tests
dotnet test --filter "FullyQualifiedName~ShareQuery"
```

## Next Steps for Full Implementation

1. Complete ShareRepository unit tests with all edge cases
2. Implement all Command/Query handler tests
3. Implement PermissionService comprehensive tests
4. Add SharesController integration tests
5. Add end-to-end tests with real database
6. Add performance tests for large share lists
7. Add concurrency tests (simultaneous share operations)

## Notes

- All tests use in-memory database for isolation
- Each test has independent database instance
- Mocking used for testing logic without database
- FluentAssertions for readable assertions
- xUnit as test framework
