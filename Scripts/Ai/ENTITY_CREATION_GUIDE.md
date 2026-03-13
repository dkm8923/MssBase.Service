# Security Service Entity Creation Guide

This guide provides step-by-step instructions for adding a new entity to the Security service. Use this as a reference whenever you need to add another entity (following the pattern established by `Application` and `ApplicationUser`).

## Overview

Each entity requires implementations across 5 projects with the following structure:
- **Contract.Security** - Interface contracts
- **Data.Security** - Database models (auto-generated via EF Core)
- **Dto.Security** - Data transfer objects and request objects
- **Logic.Security** - Business logic and validators
- **Service.Security** - Service layer with caching
- **MssBase.Service** - API controllers

## Step-by-Step Implementation

### Step 1: Database Model (Auto-Generated)
**Location**: `Services/Security/Data.Security/Models/{EntityName}.cs`

The entity model is typically auto-generated via Entity Framework Core migrations. It contains:
- Primary key property (e.g., `{EntityName}Id`)
- Audit fields (`CreatedOn`, `CreatedBy`, `UpdatedOn`, `UpdatedBy`)
- `Active` flag
- Entity-specific properties
- Navigation properties for relationships

**Example Pattern** (from Application.cs):
```csharp
public int ApplicationId { get; set; }
public DateTime CreatedOn { get; set; }
public string CreatedBy { get; set; } = null!;
public DateTime? UpdatedOn { get; set; }
public string? UpdatedBy { get; set; }
public bool Active { get; set; }
public string Name { get; set; } = null!;
public string? Description { get; set; }
public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
```

---

### Step 2: Data Transfer Objects (DTOs)
**Location**: `Services/Security/Dto.Security/{EntityName}/`

#### 2a. Main DTO - `{EntityName}Dto.cs`
Contains read-only data representation. Include all properties from the model except navigation collections.

**Pattern**:
```csharp
public record ApplicationDto
{
    public int ApplicationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
```

#### 2b. Insert/Update Request - `InsertUpdate{EntityName}Request.cs`
Request object for POST/PUT operations. Implements `ICurrentUser` interface.

**Pattern**:
```csharp
public record InsertUpdateApplicationRequest : ICurrentUser
{
    public bool Active { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrentUser { get; set; } = null!;  // From ICurrentUser
}
```

#### 2c. Filter Logic Request - `Logic/Filter{EntityName}LogicRequest.cs`
Extends `BaseLogicGet` for filtering operations in the Logic layer.

**Pattern**:
```csharp
public record FilterApplicationLogicRequest : BaseLogicGet
{
    public string? CreatedBy { get; set; }
    public DateOnly? CreatedOnDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateOnly? UpdatedOnDate { get; set; }
    public List<int>? ApplicationIds { get; set; }
    public string? Name { get; set; }
}
```

#### 2d. Filter Service Request - `Service/Filter{EntityName}ServiceRequest.cs`
Extends Filter Logic Request and implements `IDeleteCache`.

**Pattern**:
```csharp
public record FilterApplicationServiceRequest : FilterApplicationLogicRequest, IDeleteCache
{
    public bool DeleteCache { get; set; } = false;
}
```

---

### Step 3: Validators
**Location**: `Services/Security/Logic.Security/Validators/{EntityName}/`

#### 3a. Insert/Update Validator - `InsertUpdate{EntityName}RequestValidator.cs`
Validates the Insert/Update request object using FluentValidation.

**IMPORTANT - Field Length Validation**:
Always verify the correct max-length constraints from the database model/DTO before writing validators. Each field has a maximum character length defined in the database schema. Use the exact values or validation will fail. Common lengths:
- Email fields: typically 128 characters
- Name/FirstName/LastName fields: typically 64 characters
- Description fields: typically 256 characters
- Password fields: typically 256 characters
- CurrentUser/audit fields: typically 64 characters

**Pattern** (Application example):
```csharp
public class InsertUpdateApplicationRequestValidator : AbstractValidator<InsertUpdateApplicationRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

    private static class EntityFieldNames
    {
        public const string Name = "Name";
        public const string Description = "Description";
        public const string CurrentUser = "CurrentUser";
    }

    public InsertUpdateApplicationRequestValidator(IValidatorUtilities validatorUtilities)
    {
        _validatorUtilities = validatorUtilities;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Name))
            .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Name, 64));

        RuleFor(v => v.Description)
            .Length(0, 256).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Description, 256));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
```

**Example** (ApplicationUser - reference for multi-field entity):
```csharp
public class InsertUpdateApplicationUserRequestValidator : AbstractValidator<InsertUpdateApplicationUserRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

    private static class EntityFieldNames
    {
        public const string Email = "Email";                    // 128 max
        public const string FirstName = "FirstName";            // 64 max
        public const string LastName = "LastName";              // 64 max
        public const string DateOfBirth = "DateOfBirth";        // optional
        public const string Password = "Password";              // 256 max
        public const string ApplicationId = "ApplicationId";    // required > 0
        public const string CurrentUser = "CurrentUser";        // 64 max
    }

    public InsertUpdateApplicationUserRequestValidator(IValidatorUtilities validatorUtilities)
    {
        _validatorUtilities = validatorUtilities;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.Email))
            .EmailAddress().WithMessage("Email must be in a valid format!")
            .Length(1, 128).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Email, 128));

        RuleFor(v => v.FirstName)
            .Length(0, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.FirstName, 64));

        RuleFor(v => v.LastName)
            .Length(0, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.LastName, 64));

        RuleFor(v => v.Password)
            .Length(0, 256).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.Password, 256));

        RuleFor(v => v.ApplicationId)
            .GreaterThan(0).WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.ApplicationId));

        RuleFor(v => v.CurrentUser)
            .NotEmpty().WithMessage(_validatorUtilities.CreateRequiredFieldErrorMessage(EntityFieldNames.CurrentUser))
            .Length(1, 64).WithMessage(_validatorUtilities.CreateMaxLengthErrorMessage(EntityFieldNames.CurrentUser, 64));
    }
}
```

#### 3b. Filter Validator - `Filter{EntityName}LogicRequestValidator.cs`
Validates the Filter Logic Request. Often minimal or empty with custom logic as needed.

**Pattern**:
```csharp
public class FilterApplicationLogicRequestValidator : AbstractValidator<FilterApplicationLogicRequest>
{
    private readonly IValidatorUtilities _validatorUtilities;

    public FilterApplicationLogicRequestValidator(IValidatorUtilities validatorUtilities)
    {
        _validatorUtilities = validatorUtilities;
        // Add custom validation rules as needed
    }
}
```

---

### Step 4: Contract Interfaces
**Location**: `Services/Security/Contract.Security/{EntityName}/`

#### 4a. Logic Interface - `I{EntityName}Logic.cs`
Defines the contract for business logic operations.

**Pattern**:
```csharp
public interface IApplicationLogic
{
    Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseLogicGet req);
    Task<ErrorValidationResult<ApplicationDto>> GetById(int applicationId, BaseLogicGet req);
    Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(FilterApplicationLogicRequest req);
    Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req);
    Task<ErrorValidationResult<ApplicationDto>> Update(int applicationId, InsertUpdateApplicationRequest req);
    Task<ErrorValidationResult> Delete(int applicationId);
}
```

#### 4b. Service Interface - `I{EntityName}Service.cs`
Defines the contract for service layer operations (wrapper around logic with caching).

**Pattern**:
```csharp
public interface IApplicationService
{
    Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseServiceGet req, bool includeRelated = false);
    Task<ErrorValidationResult<ApplicationDto>> GetById(int applicationId, BaseServiceGet req, bool includeRelated = false);
    Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> Filter(FilterApplicationServiceRequest req);
    Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req);
    Task<ErrorValidationResult<ApplicationDto>> Update(int applicationId, InsertUpdateApplicationRequest req);
    Task<ErrorValidationResult> Delete(int applicationId);
}
```

---

### Step 5: Logic Implementation
**Location**: `Services/Security/Logic.Security/Logic/{EntityName}Logic.cs`

Implements the `I{EntityName}Logic` interface with database operations.

**Key Components**:
- Constructor with dependency injection (`IDatabaseConnectionStrings`, validators)
- `GetAll()` - delegates to Filter
- `GetById()` - delegates to Filter with specific ID
- `Filter()` - validates request and queries database
- `Insert()` - validates and inserts new record
- `Update()` - validates and updates existing record
- `Delete()` - deletes a record
- Private validation methods (`_validateEntityOnInsertUpdate`, `_validateEntityFilter`)

**Key Pattern Details**:
- Uses `SecurityDBContextFactory` for database context
- Converts models to DTOs using extension methods (`.ToDto()`, `.ToDtos()`)
- Converts DTOs to models using extension methods (`.ToEntityOnInsert()`, `.ToEntityOnUpdate()`)
- Returns `ErrorValidationResult<T>` with validation errors
- Uses `AsNoTracking()` for read-only contexts

---

### Step 6: Service Implementation
**Location**: `Services/Security/Service.Security/Service/{EntityName}Service.cs`

Implements the `I{EntityName}Service` interface with caching layer.

**Key Components**:
- Constructor with dependency injection (Logic interface, `ICacheService`)
- Cache key section name constant
- All CRUD methods delegate to Logic layer
- Wraps Get operations with cache retrieval
- Clears cache on Insert/Update/Delete operations

**Pattern**:
```csharp
public class ApplicationService : IApplicationService
{
    private readonly string cacheKeySectionName = ICacheService.ApplicationService;
    private readonly IApplicationLogic _applicationLogic;
    private readonly ICacheService _cacheService;

    public ApplicationService(IApplicationLogic applicationLogic, ICacheService cacheService)
    {
        _applicationLogic = applicationLogic;
        _cacheService = cacheService;
    }

    public async Task<ErrorValidationResult<IEnumerable<ApplicationDto>>> GetAll(BaseServiceGet req, bool includeRelated = false)
    {
        var cacheKeyName = CacheUtilities.CreateGetAllCacheKey(cacheKeySectionName, req.IncludeInactive, includeRelated);
        return await _cacheService.GetByKeyAsync(req.DeleteCache, cacheKeyName, () => _applicationLogic.GetAll(req));
    }

    public async Task<ErrorValidationResult<ApplicationDto>> Insert(InsertUpdateApplicationRequest req)
    {
        await _cacheService.RemoveKeysByPatternAsync(cacheKeySectionName);
        return await _applicationLogic.Insert(req);
    }
    // ... other methods follow similar pattern
}
```

---

### Step 7: API Controller
**Location**: `MssBase.Service/Controllers/Security/{EntityName}Controller.cs`

Implements REST endpoints for CRUD operations.

**Key Components**:
- Route: `[Route("api/security/[controller]")]`
- Tags for Swagger documentation
- Dependency injection of `I{EntityName}Service`
- HTTP endpoints with Swagger documentation:
  - `GET /` - Get all records
  - `GET /{id}` - Get single record by ID
  - `POST /filter` - Filter records
  - `POST /` - Insert new record
  - `PUT /{id}` - Update record
  - `DELETE /{id}` - Delete record

**Common Swagger Attributes**:
- `[SwaggerOperation()]` - Summary and description
- `[SwaggerResponse()]` - Response codes and types
- `[AutoValidationAttribute]` - Auto-validate Floor request

**Typical Pattern for Each Endpoint**:
```csharp
[HttpGet()]
public async Task<IActionResult> GetApplications([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, [FromQuery] bool includeRelated = false)
{
    try
    {
        var records = await _applicationSvc.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive }, includeRelated);
        return Ok(records);
    }
    catch (Exception ex)
    {
        return HandleControllerException(HttpContext, ex);
    }
}
```

---

### Step 8: Service Registration
**Location**: `MssBase.Service/ServiceExtensions.cs`

Register the service and logic implementations in the dependency injection container.

**Pattern**:
```csharp
services.AddScoped<I{EntityName}Logic, {EntityName}Logic>();
services.AddScoped<I{EntityName}Service, {EntityName}Service>();
```

Also register validators:
```csharp
services.AddScoped<IValidator<InsertUpdate{EntityName}Request>, InsertUpdate{EntityName}RequestValidator>();
services.AddScoped<IValidator<Filter{EntityName}LogicRequest>, Filter{EntityName}LogicRequestValidator>();
```

---

### Step 9: Converter Extensions
**Location**: `Services/Security/Data.Security/Converters/` (if not already present)

Some converters may already be created. You may need to extend existing converters with:
- `.ToDto()` - Convert model to DTO
- `.ToDtos()` - Convert IEnumerable model to IEnumerable DTO
- `.ToEntityOnInsert()` - Convert request to new model
- `.ToEntityOnUpdate()` - Convert request to existing model

---

## Step 10: Unit Testing - Utilities Class
**Location**: `Tests/ServiceTests/Security/IntegrationTests.Security/Shared/Utilities/{EntityName}Utilities.cs`

Create a utilities helper class for testing purposes. This class provides common test data creation and validation methods.

### 10a. Utilities Class - `{EntityName}Utilities.cs`
Implements `I{EntityName}Utilities` interface and provides helper methods for test data creation and validation.

**Key Methods**:
- `CreateInsertUpdateRequestWithRandomValues()` - Creates a request with random test data
- `CreateInsertUpdateRequestWithMaxLengthErrors()` - Creates a request with field length violations
- `CreateSingleApplicationTestRecord()` - Creates and inserts a single test record
- `CreateSingleApplicationTestRecordWithSpecificValues()` - Creates a test record with specific values
- `CreateTestRecords()` - Creates multiple test records
- `DeleteAllRecords()` - Deletes all records including inactive ones
- `ConvertApplicationDtoToInsertUpdateRequest()` - Converts DTO to request
- `VerifyTestRecordValuesMatch()` - Verifies two records have matching values
- `GetExpectedRecordDoesNotExistErrors()` - Returns expected error messages for missing records
- `GetExpectedUniqueFieldErrors()` - Returns expected error messages for unique violations
- `GetExpectedRequiredFieldErrors()` - Returns expected error messages for required fields
- `GetExpectedMaxLengthFieldErrors()` - Returns expected error messages for length violations

**Pattern**:
```csharp
public class ApplicationUtilities : IApplicationUtilities
{
    protected readonly IApplicationLogic _applicationLogic;

    public ApplicationUtilities(IApplicationLogic applicationLogic)
    {
        _applicationLogic = applicationLogic;
    }

    public InsertUpdateApplicationRequest CreateInsertUpdateRequestWithRandomValues(bool active = true)
    {
        return new InsertUpdateApplicationRequest
        {
            Name = LogicTestUtilities.GenerateRandomString(64),
            Description = LogicTestUtilities.GenerateRandomString(256),
            Active = active,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public async Task<ApplicationDto> CreateSingleApplicationTestRecord(bool active = true)
    {
        var insertReq = CreateInsertUpdateRequestWithRandomValues(active);
        var ret = await _applicationLogic.Insert(insertReq);
        ret.Errors.Should().BeNullOrEmpty("Insert of application test record failed when it should have succeeded.");
        return ret.Response;
    }

    public async Task DeleteAllRecords()
    {
        var recordsToDelete = await _applicationLogic.GetAll(new BaseLogicGet { IncludeInactive = true });
        foreach (var record in recordsToDelete.Response)
        {
            await _applicationLogic.Delete(record.ApplicationId);
        }
    }

    public void VerifyTestRecordValuesMatch(ApplicationDto recordA, ApplicationDto recordB)
    {
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.Name.Should().Be(recordB.Name);
        recordA.Description.Should().Be(recordB.Description);
        recordA.Active.Should().Be(recordB.Active);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }

    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Name", new List<string> { "Name cannot exceed 64 characters!" } },
            { "Description", new List<string> { "Description cannot exceed 256 characters!" } },
            { "CurrentUser", new List<string> { "CurrentUser cannot exceed 64 characters!" } }
        };
    }
    // ... other methods
}
```

### 10b. Utilities Interface - `Contracts/I{EntityName}Utilities.cs`
Defines the contract for the utilities class.

**Pattern**:
```csharp
public interface IApplicationUtilities
{
    public Task DeleteAllRecords();
    public Task<List<ApplicationDto>> CreateTestRecords(short numberOfRecordsToCreate = 5, bool active = true);
    public Task<ApplicationDto> CreateSingleApplicationTestRecord(bool active = true);
    public Task<ApplicationDto> CreateSingleApplicationTestRecordWithSpecificValues(InsertUpdateApplicationRequest req = null);
    public InsertUpdateApplicationRequest CreateInsertUpdateRequestWithMaxLengthErrors();
    public InsertUpdateApplicationRequest CreateInsertUpdateRequestWithRandomValues(bool active = true);
    public InsertUpdateApplicationRequest ConvertApplicationDtoToInsertUpdateRequest(ApplicationDto req);
    public void VerifyTestRecordValuesMatch(ApplicationDto recordA, ApplicationDto recordB);
    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors();
    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors();
    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors();
    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors();
}
```

---

## Step 11: Controller Integration Tests
**Location**: `Tests/ServiceTests/Security/IntegrationTests.Security/Controller/{EntityName}ControllerTests.cs`

Integration tests for REST API endpoints.

### Test Structure
The controller tests inherit from `SecurityTestBase` and use `WebApplicationFactory<Program>` for integration testing.

**Key Test Sections**:
- **GetAll Tests**: Active records, inactive records (with flag), zero records
- **GetById Tests**: Active record, inactive record (with flag), not found, invalid ID
- **Filter Tests**: Active data, inactive data (with flag), filtered results, zero records, null/blank requests
- **Insert Tests**: Create record, null request, blank JSON request, validation errors (required fields, length, uniqueness)
- **Update Tests**: Update record, null request, blank JSON request, validation errors
- **Delete Tests**: Delete record, attempt to delete non-existent record, invalid ID

**Pattern**:
```csharp
[Collection("SecurityIntegrationTests")]
public class ApplicationControllerTests : SecurityTestBase, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApplicationControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region GetAll

    [Fact]
    public async Task Application_GetAll_Should_Return_Active_Data()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords(1, false); // inactive record

        // Act
        var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(
            _client, ApiEndPoints.Security.Application.Base);

        // Assert
        result.Errors.Should().HaveCount(0);
        result.Response.Should().HaveCount(5); // Only active records
    }

    [Fact]
    public async Task Application_GetAll_Should_Return_Inactive_Data()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords(1, false);

        // Act
        var result = await ControllerTestUtilities.GetAllRecordsWithValidationResult<List<ApplicationDto>>(
            _client, ApiEndPoints.Security.Application.Base + "?" + 
            ControllerTestUtilities.CreateIncludeInactiveQueryStringParm(true));

        // Assert
        result.Errors.Should().HaveCount(0);
        result.Response.Should().HaveCount(6); // 5 active + 1 inactive
    }

    #endregion

    #region GetById

    [Fact]
    public async Task Application_GetById_Should_Return_Active_Record()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

        // Act
        var result = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(
            _client, ApiEndPoints.Security.Application.Base, testRecord.ApplicationId);

        // Assert
        result.Errors.Should().HaveCount(0);
        _SecurityTestUtilities.Application.VerifyTestRecordValuesMatch(result.Response, testRecord);
    }

    #endregion

    #region Filter

    [Fact]
    public async Task Application_Filter_Should_Return_Active_Data()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();

        var postReq = new FilterApplicationServiceRequest { };

        // Act
        var result = await ControllerTestUtilities.GetFilteredRecordsWithValidationResult<List<ApplicationDto>>(
            _client, ApiEndPoints.Security.Application.Base, postReq);

        // Assert
        result.Errors.Should().HaveCount(0);
        result.Response.Should().HaveCountGreaterThan(0);
        result.Response.ForEach(r => r.Active.Should().BeTrue());
    }

    #endregion

    #region Insert

    [Fact]
    public async Task Application_Insert_Should_Create_Record()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();

        // Act
        var insertedRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();
        var insertCheck = await ControllerTestUtilities.GetRecordByIdWithValidationResult<ApplicationDto>(
            _client, ApiEndPoints.Security.Application.Base, insertedRecord.ApplicationId);

        // Assert
        _SecurityTestUtilities.Application.VerifyTestRecordValuesMatch(insertedRecord, insertCheck.Response);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Application_Update_Should_Update_Record()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var insertedRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

        var updateReq = new InsertUpdateApplicationRequest
        {
            Name = "Updated Application Name",
            Description = "Updated Application Description",
            Active = false,
            CurrentUser = TestConstants.CurrentUser
        };

        // Act
        var updateResult = await ControllerTestUtilities.UpdateRecordWithValidationResult<ApplicationDto>(
            _client, ApiEndPoints.Security.Application.Base, updateReq, insertedRecord.ApplicationId);

        // Assert
        updateResult.Response.ApplicationId.Should().Be(insertedRecord.ApplicationId);
        updateResult.Response.Name.Should().Be(updateReq.Name);
        updateResult.Response.Description.Should().Be(updateReq.Description);
        updateResult.Response.Active.Should().Be(updateReq.Active);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Application_Delete_Should_Delete_Record()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

        // Act
        var response = await ControllerTestUtilities.DeleteRecord(_client, ApiEndPoints.Security.Application.Base, testRecord.ApplicationId);
        var getResponse = await _client.GetAsync(ApiEndPoints.Security.Application.Base + "/" + testRecord.ApplicationId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
```

---

## Step 12: Logic Layer Tests
**Location**: `Tests/ServiceTests/Security/IntegrationTests.Security/Logic/{EntityName}/{EntityName}LogicTests.cs`

Unit and integration tests for business logic layer.

### Test Structure
The logic tests inherit from `SecurityTestBase` and test the `I{EntityName}Logic` interface directly.

**Key Test Sections**:
- **GetAll Tests**: Active records, inactive records, zero records
- **GetById Tests**: Active record, inactive record, not found
- **Filter Tests**: Active data, inactive data, specific filter criteria, zero records
- **Insert Tests**: Valid insert, validation errors (required fields, length, uniqueness)
- **Update Tests**: Valid update, validation errors
- **Delete Tests**: Valid delete, error on non-existent record

**Pattern**:
```csharp
[Collection("SecurityIntegrationTests")]
public class ApplicationLogicTests : SecurityTestBase
{
    #region GetAll

    [Fact]
    public async Task Application_GetAll_Should_Return_Active_Data()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();
        await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

        // Act
        var result = await _applicationLogic.GetAll(new BaseLogicGet());

        // Assert
        result.Response.Should().HaveCount(5);
    }

    [Fact]
    public async Task Application_GetAll_Should_Return_Inactive_Data()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();
        await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord(false);

        // Act
        var result = await _applicationLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

        // Assert
        result.Response.Should().HaveCount(6);
    }

    #endregion

    #region GetById

    [Fact]
    public async Task Application_GetById_Should_Return_Active_Record()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

        // Act
        var result = await _applicationLogic.GetById(testRecord.ApplicationId, new BaseLogicGet());

        // Assert
        result.Response.Should().NotBeNull();
    }

    #endregion

    #region Filter

    [Fact]
    public async Task Application_Filter_Should_Return_Active_Data()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();

        var postReq = new FilterApplicationLogicRequest { };

        // Act
        var result = await _applicationLogic.Filter(postReq);

        // Assert
        result.Errors.Should().HaveCount(0);
        result.Response.Should().HaveCountGreaterThan(0);
        foreach (var r in result.Response)
        {
            r.Active.Should().BeTrue();
        }
    }

    #endregion

    #region Insert

    [Fact]
    public async Task Application_Insert_Should_Create_Record()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var insertReq = _SecurityTestUtilities.Application.CreateInsertUpdateRequestWithRandomValues();

        // Act
        var result = await _applicationLogic.Insert(insertReq);

        // Assert
        result.Errors.Should().BeNullOrEmpty();
        result.Response.Should().NotBeNull();
        result.Response.Name.Should().Be(insertReq.Name);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Application_Update_Should_Update_Record()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

        var updateReq = new InsertUpdateApplicationRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Active = false,
            CurrentUser = TestConstants.CurrentUser
        };

        // Act
        var result = await _applicationLogic.Update(testRecord.ApplicationId, updateReq);

        // Assert
        result.Errors.Should().BeNullOrEmpty();
        result.Response.Name.Should().Be(updateReq.Name);
        result.Response.Active.Should().Be(updateReq.Active);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Application_Delete_Should_Delete_Record()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();

        // Act
        var result = await _applicationLogic.Delete(testRecord.ApplicationId);
        var getResult = await _applicationLogic.GetById(testRecord.ApplicationId, new BaseLogicGet { IncludeInactive = true });

        // Assert
        result.Errors.Should().BeNullOrEmpty();
        getResult.Response.Should().BeNull();
    }

    #endregion
}
```

---

## Step 13: Service Layer Tests (with Caching)
**Location**: `Tests/ServiceTests/Security/IntegrationTests.Security/Service/{EntityName}ServiceTests.cs`

Tests for the service layer with cache validation.

### Test Structure
Service tests verify CRUD operations and ensure cache behavior is correct.

**Key Test Sections**:
- **GetAll Tests**: Verify caching with active/inactive flags, cache key generation
- **GetById Tests**: Verify caching and cache key patterns
- **Filter Tests**: Verify caching for filtered queries, cache invalidation
- **Insert Tests**: Verify cache is cleared on insert
- **Update Tests**: Verify cache is cleared on update
- **Delete Tests**: Verify cache is cleared on delete

**Pattern**:
```csharp
[Collection("SecurityIntegrationTests")]
public class ApplicationServiceTests : SecurityTestBase
{
    private readonly IApplicationService _applicationService;
    private readonly ICacheTestUtilities _cacheTestUtilities;

    public ApplicationServiceTests()
    {
        _cacheTestUtilities = _serviceProvider.GetService<ICacheTestUtilities>();
        _applicationService = _serviceProvider.GetService<IApplicationService>();
    }

    #region GetAll

    [Fact]
    public async Task Application_GetAll_Active_Should_Cache()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();
        await _cacheTestUtilities.DeleteAllKeyData();

        var expectedCacheKey = $"ApplicationService_GetAll_0_0";

        // Act
        var result = await _applicationService.GetAll(new BaseServiceGet());
        var availableCacheKeys = _cacheTestUtilities.GetKeys();

        // Assert
        availableCacheKeys.Should().Contain(expectedCacheKey);
        result.Response.Should().HaveCount(5);
    }

    [Fact]
    public async Task Application_GetAll_IncludeInactive_Should_Cache()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();
        await _cacheTestUtilities.DeleteAllKeyData();

        var expectedCacheKey = "ApplicationService_GetAll_1_0";

        // Act
        var result = await _applicationService.GetAll(new BaseServiceGet { IncludeInactive = true });
        var availableCacheKeys = _cacheTestUtilities.GetKeys();

        // Assert
        availableCacheKeys.Should().Contain(expectedCacheKey);
        result.Response.Should().HaveCount(5);
    }

    #endregion

    #region GetById

    [Fact]
    public async Task Application_GetById_Should_Cache()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _cacheTestUtilities.DeleteAllKeyData();

        var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();
        var expectedCacheKey = $"ApplicationService_GetById_{testRecord.ApplicationId}_0_0";

        // Act
        var result = await _applicationService.GetById(testRecord.ApplicationId, new BaseServiceGet());
        var availableCacheKeys = _cacheTestUtilities.GetKeys();

        // Assert
        availableCacheKeys.Should().Contain(expectedCacheKey);
        result.Response.Should().NotBeNull();
    }

    #endregion

    #region Filter

    [Fact]
    public async Task Application_Filter_Should_Cache()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();
        await _cacheTestUtilities.DeleteAllKeyData();

        var postReq = new FilterApplicationServiceRequest { CreatedBy = TestConstants.CurrentUser };
        var expectedCacheKey = $"ApplicationService_Filter_IntegrationTest_0_0_0_0_0_0";

        // Act
        var result = await _applicationService.Filter(postReq);
        var availableCacheKeys = _cacheTestUtilities.GetKeys();

        // Assert
        availableCacheKeys.Should().Contain(expectedCacheKey);
        result.Response.Should().HaveCountGreaterThan(0);
    }

    #endregion

    #region Insert

    [Fact]
    public async Task Application_Insert_Should_Clear_Cache()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        await _SecurityTestUtilities.Application.CreateTestRecords();
        var applicationCacheKeys = _cacheTestUtilities.GetKeys();
        applicationCacheKeys.Should().HaveCountGreaterThan(0);

        var insertReq = _SecurityTestUtilities.Application.CreateInsertUpdateRequestWithRandomValues();

        // Act
        var result = await _applicationService.Insert(insertReq);
        var cacheKeysAfterInsert = _cacheTestUtilities.GetKeys();

        // Assert
        result.Errors.Should().BeNullOrEmpty();
        // Verify cache was cleared (no Application keys should exist)
        var remainingApplicationKeys = cacheKeysAfterInsert.Where(k => k.Contains("ApplicationService")).ToList();
        remainingApplicationKeys.Should().HaveCount(0);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Application_Update_Should_Clear_Cache()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();
        await _cacheTestUtilities.DeleteAllKeyData();
        
        var cacheKeysBefore = _cacheTestUtilities.GetKeys();
        cacheKeysBefore.Should().HaveCount(0);

        var updateReq = new InsertUpdateApplicationRequest
        {
            Name = "Updated Name",
            CurrentUser = TestConstants.CurrentUser,
            Active = true
        };

        // Act
        var result = await _applicationService.Update(testRecord.ApplicationId, updateReq);
        var cacheKeysAfter = _cacheTestUtilities.GetKeys();

        // Assert
        result.Errors.Should().BeNullOrEmpty();
        cacheKeysAfter.Should().HaveCount(0); // Cache should be cleared
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Application_Delete_Should_Clear_Cache()
    {
        // Arrange
        await _SecurityTestUtilities.Application.DeleteAllRecords();
        var testRecord = await _SecurityTestUtilities.Application.CreateSingleApplicationTestRecord();
        await _cacheTestUtilities.DeleteAllKeyData();

        // Act
        var result = await _applicationService.Delete(testRecord.ApplicationId);
        var cacheKeysAfter = _cacheTestUtilities.GetKeys();

        // Assert
        result.Errors.Should().BeNullOrEmpty();
        cacheKeysAfter.Should().HaveCount(0); // Cache should be cleared
    }

    #endregion
}
```

---

## Step 14: Update SecurityTestUtilitiesManager.cs
**Location**: `Tests/ServiceTests/Security/IntegrationTests.Security/Shared/Utilities/SecurityTestUtilitiesManager.cs`

Register the new utilities class in the manager.

**Pattern**:
```csharp
public class SecurityTestUtilitiesManager : ISecurityTestUtilitiesManager
{
    private IApplicationUtilities _applicationUtilities;
    private I{EntityName}Utilities _{entityName}Utilities;  // Add new entity
    
    public SecurityTestUtilitiesManager(
        IApplicationUtilities applicationUtilities,
        I{EntityName}Utilities {entityName}Utilities)  // Add parameter
    {
        _applicationUtilities = applicationUtilities;
        _{entityName}Utilities = {entityName}Utilities;  // Store reference
    }

    public IApplicationUtilities Application
    {
        get { return _applicationUtilities; }
    }

    public I{EntityName}Utilities {EntityName}  // Add property
    {
        get { return _{entityName}Utilities; }
    }
}
```

Also update `ISecurityTestUtilitiesManager.cs`:
```csharp
public interface ISecurityTestUtilitiesManager
{
    public IApplicationUtilities Application { get; }
    public I{EntityName}Utilities {EntityName} { get; }  // Add property
}
```

---

## Step 15: Update SecurityTestBase.cs
**Location**: `Tests/ServiceTests/Security/IntegrationTests.Security/Shared/SecurityTestBase.cs`

Inject the new service and logic implementations into the test base class.

**Updates**:
1. Add protected field for new logic interface
2. Add protected field for new service interface (optional if only testing logic)
3. Inject in constructor
4. Register in `ConfigureSecurityService()` method

**Pattern**:
```csharp
public class SecurityTestBase
{
    // ... existing fields ...
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly I{EntityName}Logic _{entityName}Logic;  // Add new entity
    
    public SecurityTestBase()
    {
        // ... existing code ...
        _applicationLogic = _serviceProvider.GetService<IApplicationLogic>();
        _{entityName}Logic = _serviceProvider.GetService<I{EntityName}Logic>();  // Add injection
    }

    private ServiceCollection ConfigureSecurityService(ServiceCollection services)
    {
        // ... existing Application configuration ...

        #region {EntityName}

        services.AddTransient<I{EntityName}Service, {EntityName}Service>();
        services.AddTransient<I{EntityName}Logic, {EntityName}Logic>();

        // Configure Fluent Validation Validators
        services.AddTransient<IValidator<Filter{EntityName}LogicRequest>, Filter{EntityName}LogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdate{EntityName}Request>, InsertUpdate{EntityName}RequestValidator>();

        #endregion

        return services;
    }
}
```

Also update `ConfigureBaseDependencies()`:
```csharp
private ServiceCollection ConfigureBaseDependencies(ServiceCollection services)
{
    // ... existing code ...
    services.AddTransient<IApplicationUtilities, ApplicationUtilities>();
    services.AddTransient<I{EntityName}Utilities, {EntityName}Utilities>();  // Add new entity
    
    return services;
}
```

---

## Step 15.5: Standard Test Methods (Controller / Service / Logic)

Add the following default test methods for each entity to ensure consistent, repeatable coverage across Controller, Service, and Logic layers. Use these names and short descriptions as templates when writing tests for new entities.

**Controller Tests**
- `Entity_GetAll_Should_Return_Active_Data`: verifies only active records are returned.
- `Entity_GetAll_Should_Return_Inactive_Data_When_IncludeInactive`: verifies inactive records returned when requested.
- `Entity_GetAll_Should_Return_Zero_Records_When_None_Exist`: verifies empty result handling.
- `Entity_GetById_Should_Return_Active_Record`: verifies successful retrieval by id.
- `Entity_GetById_Should_Return_Inactive_Record_When_IncludeInactive`: verifies retrieval of inactive with flag.
- `Entity_GetById_Should_Return_NotFound_For_Missing_Record`: verifies 404 for non-existent id.
- `Entity_GetById_Should_Return_BadRequest_For_Invalid_Id`: verifies validation for invalid id inputs.
- `Entity_Filter_Should_Return_Active_Data`: verifies filtering returns active records.
- `Entity_Filter_Should_Return_Inactive_Data_When_IncludeInactive`: verifies filter include inactive behavior.
- `Entity_Filter_Should_Return_Zero_Records_When_None_Match`: verifies empty filter result handling.
- `Entity_Filter_Should_Return_BadRequest_For_Null_Or_Blank_Body`: verifies controller rejects empty/invalid filter payloads.
- `Entity_Insert_Should_Create_Record`: verifies successful insert via controller.
- `Entity_Insert_Should_Return_BadRequest_For_Null_Or_Blank_Body`: verifies controller rejects empty/invalid insert payloads.
- `Entity_Insert_Should_Return_ValidationErrors_For_Invalid_Data`: verifies required/max-length/unique validations surface to caller.
- `Entity_Update_Should_Update_Record`: verifies successful update via controller.
- `Entity_Update_Should_Return_BadRequest_For_Null_Or_Blank_Body`: verifies controller rejects empty/invalid update payloads.
- `Entity_Update_Should_Return_ValidationErrors_For_Invalid_Data`: verifies update validation errors.
- `Entity_Delete_Should_Delete_Record`: verifies delete endpoint returns expected status and record is gone.
- `Entity_Delete_Should_Return_NotFound_For_Missing_Record`: verifies deleting non-existent id returns 404.
- `Entity_Delete_Should_Return_BadRequest_For_Invalid_Id`: verifies controller validation for bad id values.

**Service Tests (Caching & Delegation)**
- `Entity_GetAll_Should_Cache_Result`: verifies GetAll populates cache for default flags.
- `Entity_GetAll_IncludeInactive_Should_Cache_Result`: verifies include-inactive cache key variant.
- `Entity_GetAll_Should_Not_Hit_Cache_When_DeleteCache_True`: verifies bypass/refresh behavior.
- `Entity_GetById_Should_Cache_Result`: verifies GetById populates cache.
- `Entity_GetById_Should_Not_Cache_When_Record_Not_Found`: verifies missing id does not create cache entry.
- `Entity_Filter_Should_Cache_Result`: verifies filtered requests use cache.
- `Entity_Insert_Should_Clear_Cache_On_Success`: verifies cache invalidation after insert.
- `Entity_Update_Should_Clear_Cache_On_Success`: verifies cache invalidation after update.
- `Entity_Delete_Should_Clear_Cache_On_Success`: verifies cache invalidation after delete.
- `Entity_Service_Should_Propagate_Logic_Errors`: verifies service returns/describes errors from logic layer unchanged.
- `Entity_Service_Should_Handle_IncludeRelated_Flag`: verifies service forwards includeRelated to logic/cache key.

**Logic Tests (Validation & Persistence)**
- `Entity_GetAll_Should_Return_Active_Data`: verifies logic returns only active by default.
- `Entity_GetAll_Should_Return_Inactive_Data_When_IncludeInactive`: verifies flag behavior.
- `Entity_GetById_Should_Return_Active_Record`: verifies successful retrieval.
- `Entity_GetById_Should_Return_Null_For_Missing_Record`: verifies null/empty response for missing id.
- `Entity_Filter_Should_Apply_All_Criteria`: verifies each filter field affects results as expected.
- `Entity_Insert_Should_Create_Record`: verifies insert returns created DTO and persistence.
- `Entity_Insert_Should_Return_ValidationErrors_For_Required_Fields`: verifies required-field validation.
- `Entity_Insert_Should_Return_ValidationErrors_For_MaxLength_Fields`: verifies length validation.
- `Entity_Insert_Should_Return_ValidationErrors_For_Unique_Constraints`: verifies unique-field constraints surface as validation errors.
- `Entity_Insert_Should_Return_Error_For_Missing_ForeignKey_Reference`: verifies FK constraints or logical checks for related entities.
- `Entity_Update_Should_Update_Record`: verifies update applies changes and persisted.
- `Entity_Update_Should_Return_ValidationErrors_For_Invalid_Update`: verifies update validation (required/unique/max-length).
- `Entity_Delete_Should_Delete_Record`: verifies deletion logic (soft-delete or remove) and subsequent GetById reflects deletion when appropriate.
- `Entity_Delete_Should_Return_Error_For_Missing_Record`: verifies deleting non-existent id returns validation error.
- `Entity_Validation_Should_Return_Specific_Error_Messages`: verify validator utilities generate expected messages (use Utilities.GetExpected*Errors).

Notes:
- Replace `Entity` in method names with the actual entity name (e.g., `ApplicationUser_GetAll_Should_Return_Active_Data`).
- Add additional tests for related-entity behaviors (e.g., foreign-key validation) when the entity references other domain entities.
- When caching is used, include tests for cache key variants (IncludeInactive, includeRelated, DeleteCache flag) and ensure cache invalidation is covered.
- For controller tests, include null/blank body tests and unsupported media-type tests if applicable (to match existing project patterns).

## Step 16: Verify Build and Compilation
**Final Step**: After completing all implementation and test files, verify that all projects compile successfully without errors.

### Build All Projects

Run the following build commands to ensure all projects compile correctly:

```bash
# Build Logic.Security
dotnet build Services/Security/Logic.Security/Logic.Security.csproj

# Build Service.Security
dotnet build Services/Security/Service.Security/Service.Security.csproj

# Build Contract.Security (if modified)
dotnet build Services/Security/Contract.Security/Contract.Security.csproj

# Build MssBase.Service (main API)
dotnet build MssBase.Service/MssBase.Service.csproj

# Build IntegrationTests.Security (test project)
dotnet build Tests/ServiceTests/Security/IntegrationTests.Security/IntegrationTests.Security.csproj

# Build complete solution
dotnet build
```

**Expected Result**: All projects should compile with 0 errors. There may be warnings (null reference warnings are common), but no errors should occur.

**Troubleshooting Common Build Errors**:
- **Missing using statements**: Ensure all namespaces are imported at the top of files
- **Missing DI registrations**: Verify ServiceExtensions.cs includes all service and validator registrations
- **Cache key constant not found**: Ensure `ICacheService.{EntityName}Service` constant is added to ICacheService.cs
- **API endpoint not found**: Verify ApiEndPoints.cs includes the new entity endpoint
- **Test utilities not injected**: Verify SecurityTestBase.cs includes the new utilities in ConfigureBaseDependencies()

---

## Summary Checklist

**Implementation Steps**:
- [ ] Database model exists (usually auto-generated)
  - [ ] `Services/Security/Data.Security/Models/{EntityName}.cs`
- [ ] DTO files created:
  - [ ] `{EntityName}Dto.cs`
  - [ ] `InsertUpdate{EntityName}Request.cs`
  - [ ] `Logic/Filter{EntityName}LogicRequest.cs`
  - [ ] `Service/Filter{EntityName}ServiceRequest.cs`
- [ ] Validators created:
  - [ ] `Validators/{EntityName}/InsertUpdate{EntityName}RequestValidator.cs`
  - [ ] `Validators/{EntityName}/Filter{EntityName}LogicRequestValidator.cs`
- [ ] Interfaces created:
  - [ ] `Contract.Security/{EntityName}/I{EntityName}Logic.cs`
  - [ ] `Contract.Security/{EntityName}/I{EntityName}Service.cs`
- [ ] Implementation classes created:
  - [ ] `Logic.Security/Logic/{EntityName}Logic.cs`
  - [ ] `Service.Security/Service/{EntityName}Service.cs`
- [ ] Controller created:
  - [ ] `MssBase.Service/Controllers/Security/{EntityName}Controller.cs`
- [ ] Converter extensions added if needed:
  - [ ] `Data.Security/Converters/{EntityName}Converters.cs`
- [ ] ServiceExtensions.cs updated:
  - [ ] Added imports for new entity types
  - [ ] Registered in `ConfigureSecurityService()` method
- [ ] ICacheService.cs updated:
  - [ ] Added cache key constant: `public const string {EntityName}Service = "{EntityName}Service"`
- [ ] ApiEndPoints.cs updated:
  - [ ] Added API endpoint: `public class {EntityName} { public const string Base = "/api/Security/{EntityName}"; }`

**Testing Steps**:
- [ ] Test utilities created:
  - [ ] `Tests/ServiceTests/Security/IntegrationTests.Security/Shared/Utilities/{EntityName}Utilities.cs`
  - [ ] `Tests/ServiceTests/Security/IntegrationTests.Security/Shared/Utilities/Contracts/I{EntityName}Utilities.cs`
- [ ] Test files created:
  - [ ] `Tests/ServiceTests/Security/IntegrationTests.Security/Controller/{EntityName}ControllerTests.cs`
  - [ ] `Tests/ServiceTests/Security/IntegrationTests.Security/Logic/{EntityName}/{EntityName}LogicTests.cs`
  - [ ] `Tests/ServiceTests/Security/IntegrationTests.Security/Service/{EntityName}ServiceTests.cs`
- [ ] SecurityTestUtilitiesManager.cs updated:
  - [ ] Added property for new utilities
  - [ ] Updated constructor to inject new utilities
  - [ ] Added getter property
- [ ] ISecurityTestUtilitiesManager.cs updated:
  - [ ] Added interface property: `public I{EntityName}Utilities {EntityName} { get; }`
- [ ] SecurityTestBase.cs updated:
  - [ ] Added necessary using statements
  - [ ] Added protected field for logic interface
  - [ ] Injected in constructor
  - [ ] Registered in `ConfigureBaseDependencies()`
  - [ ] Registered in `ConfigureSecurityService()`

**Build Verification Steps**:
- [ ] All projects compile without errors
  - [ ] Run `dotnet build Services/Security/Logic.Security/Logic.Security.csproj` ✅ 0 errors
  - [ ] Run `dotnet build Services/Security/Service.Security/Service.Security.csproj` ✅ 0 errors
  - [ ] Run `dotnet build MssBase.Service/MssBase.Service.csproj` ✅ 0 errors
  - [ ] Run `dotnet build Tests/ServiceTests/Security/IntegrationTests.Security/IntegrationTests.Security.csproj` ✅ 0 errors

---

## Notes for Future Reference

- Each entity follows the same pattern established by `Application` and `ApplicationUser`
- Use the existing code as templates - copy from Application and replace names
- Maintain consistent naming: entity name should be PascalCase everywhere
- Always implement full CRUD: GetAll, GetById, Filter, Insert, Update, Delete
- Cache keys follow the pattern: `{ServiceName}_{Operation}_{Parameters}`
- Validation messages use utility methods from `IValidatorUtilities`
- All async operations return `ErrorValidationResult<T>` or `ErrorValidationResult`
- Test utilities must implement the corresponding `I{EntityName}Utilities` interface
- Always verify builds pass with 0 errors before considering implementation complete
