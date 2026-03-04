using Contract.Security.Application;
using Contract.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using Shared.Models;
using IntegrationTests.Shared.Utilities;

namespace IntegrationTests.Security.Shared.Utilities;

public class ApplicationUserPermissionUtilities : IApplicationUserPermissionUtilities
{
    protected readonly IApplicationUserPermissionLogic _ApplicationUserPermissionLogic;
    protected readonly IApplicationLogic _applicationLogic;
    
    public ApplicationUserPermissionUtilities(IApplicationUserPermissionLogic ApplicationUserPermissionLogic, IApplicationLogic applicationLogic) 
    {
        _ApplicationUserPermissionLogic = ApplicationUserPermissionLogic;
        _applicationLogic = applicationLogic;
    }

    /// <summary>
    /// Asynchronously clears test data from the ApplicationUserPermission and Application tables, creates a fresh test application record, 
    /// and returns the application ID for use in subsequent tests.
    /// </summary>
    // public async Task<int> ClearTestTablesAndReturnApplicationId(IApplicationUtilities applicationUtilities)
    // {
    //     await DeleteAllRecords();
    //     await applicationUtilities.DeleteAllRecords();
    //     return (await applicationUtilities.CreateTestRecords(1, true)).FirstOrDefault().ApplicationId;
    // }

    // public InsertUpdateApplicationUserPermissionRequest ConvertApplicationUserPermissionDtoToInsertUpdateRequest(ApplicationUserPermissionDto req)
    // {
    //     return new InsertUpdateApplicationUserPermissionRequest
    //     {
    //         Email = req.Email,
    //         FirstName = req.FirstName,
    //         LastName = req.LastName,
    //         DateOfBirth = req.DateOfBirth,
    //         Password = req.Password,
    //         LastLoginDate = req.LastLoginDate,
    //         LastPasswordChangeDate = req.LastPasswordChangeDate,
    //         LastLockoutDate = req.LastLockoutDate,
    //         FailedPasswordAttemptCount = req.FailedPasswordAttemptCount,
    //         Active = req.Active,
    //         ApplicationId = req.ApplicationId,
    //         CurrentUser = TestConstants.CurrentUser
    //     };
    // }

    // public InsertUpdateApplicationUserPermissionRequest CreateInsertUpdateRequestWithMaxLengthErrors()
    // {
    //     return new InsertUpdateApplicationUserPermissionRequest
    //     { 
    //         Email = LogicTestUtilities.GenerateRandomString(120) + "@test.com",
    //         FirstName = LogicTestUtilities.GenerateRandomString(65),
    //         LastName = LogicTestUtilities.GenerateRandomString(65),
    //         Password = LogicTestUtilities.GenerateRandomString(65),
    //         Active = true,
    //         ApplicationId = 1,
    //         CurrentUser = LogicTestUtilities.GenerateRandomString(65)
    //     };
    // }
    
    // public InsertUpdateApplicationUserPermissionRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true)
    // {
    //     return new InsertUpdateApplicationUserPermissionRequest
    //     {
    //         Email = LogicTestUtilities.GenerateRandomString(64) + "@test.com",
    //         FirstName = LogicTestUtilities.GenerateRandomString(32),
    //         LastName = LogicTestUtilities.GenerateRandomString(32),
    //         Password = LogicTestUtilities.GenerateRandomString(64),
    //         Active = active,
    //         ApplicationId = applicationId,
    //         CurrentUser = TestConstants.CurrentUser
    //     };
    // }

    /// <summary>
    /// Creates a single application user test record with randomized data for integration testing purposes.
    /// </summary>
    // public async Task<ApplicationUserPermissionDto> CreateSingleApplicationUserPermissionTestRecord(int applicationId, bool active = true)
    // {
    //     //create test record
    //     var insertReq = CreateInsertUpdateRequestWithRandomValues(applicationId, active);

    //     var ret = await _ApplicationUserPermissionLogic.Insert(insertReq, _applicationLogic);

    //     ret.Errors.Should().BeNullOrEmpty("Insert of application user test record failed when it should have succeeded.");

    //     return ret.Response;
    // }

    /// <summary>
    /// Creates a single application user test record with specific values provided in the request object.
    /// </summary>
    // public async Task<ApplicationUserPermissionDto> CreateSingleApplicationUserPermissionTestRecordWithSpecificValues(int applicationId, InsertUpdateApplicationUserPermissionRequest req = null)
    // {
    //     var ret = new ErrorValidationResult<ApplicationUserPermissionDto>();

    //     if (req == null)
    //     {
    //         var insertReq = new InsertUpdateApplicationUserPermissionRequest
    //         {
    //             Email = "test@ApplicationUserPermission.com",
    //             FirstName = "Test",
    //             LastName = "ApplicationUserPermission",
    //             Active = true,
    //             ApplicationId = 1,
    //             CurrentUser = TestConstants.CurrentUser
    //         };

    //         ret = await _ApplicationUserPermissionLogic.Insert(insertReq);
    //     }
    //     else
    //     {
    //         req.Email = req.Email ?? "test@ApplicationUserPermission.com";
    //         req.FirstName = req.FirstName ?? "Test";
    //         req.LastName = req.LastName ?? "ApplicationUserPermission";
    //         req.ApplicationId = req.ApplicationId == 0 ? 1 : req.ApplicationId;
    //         req.CurrentUser = req.CurrentUser ?? TestConstants.CurrentUser; 
    //         ret = await _ApplicationUserPermissionLogic.Insert(req);
    //     }

    //     return ret.Response;
    // }

    /// <summary>
    /// Asynchronously creates a set of predefined test application user records in the data store.
    /// </summary>
    // public async Task<List<ApplicationUserPermissionDto>> CreateTestRecords(int applicationId, short numberOfRecordsToCreate = 5, bool active = true)
    // {
    //     //create test records
    //     var ret = new List<ApplicationUserPermissionDto>();
    //     var recordsToCreate = new List<InsertUpdateApplicationUserPermissionRequest>();

    //     for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
    //     {
    //         ret.Add(await CreateSingleApplicationUserPermissionTestRecord(applicationId, active));
    //     }

    //     return ret;
    // }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        var recordsToDelete = await _ApplicationUserPermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

        foreach (var record in recordsToDelete.Response)
        {
            await _ApplicationUserPermissionLogic.Delete(record.ApplicationUserPermissionId);
        }
    }

    // public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors()
    // {
    //     return new Dictionary<string, List<string>>
    //     {
    //         { "Email", new List<string> { "Email cannot exceed 128 characters!" } },
    //         { "FirstName", new List<string> { "FirstName cannot exceed 64 characters!" } },
    //         { "LastName", new List<string> { "LastName cannot exceed 64 characters!" } },
    //         { "Password", new List<string> { "Password cannot exceed 64 characters!" } },
    //         { "CurrentUser", new List<string> { "CurrentUser cannot exceed 64 characters!" } }
    //     };
    // }

    // public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors()
    // {
    //     return new Dictionary<string, List<string>>
    //     {
    //         { "ApplicationUserPermission", new List<string> { "Record does not exist for specified ApplicationUserPermissionId!" } }
    //     };
    // }

    // public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    // {
    //     return new Dictionary<string, List<string>>
    //     {
    //         { "Email", new List<string> { "Email is a required field!" } },
    //         { "ApplicationId", new List<string> { "ApplicationId is a required field!" } },
    //         { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
    //     };
    // }

    // public Dictionary<string, List<string>> GetExpectedInvalidEmailFieldErrors()
    // {
    //     return new Dictionary<string, List<string>>
    //     {
    //         { "Email", new List<string> { "Email must be in a valid format!" } }
    //     };
    // }

    // public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
    // {
    //     return new Dictionary<string, List<string>>
    //     {
    //         { "Email", new List<string> { "Email must be unique!" } }
    //     };
    // }

    /// <summary>
    /// Verifies that all relevant property values of two application user records are equal.
    /// </summary>
    // public void VerifyTestRecordValuesMatch(ApplicationUserPermissionDto recordA, ApplicationUserPermissionDto recordB)
    // {
    //     recordA.ApplicationUserPermissionId.Should().Be(recordB.ApplicationUserPermissionId);
    //     recordA.Email.Should().Be(recordB.Email);
    //     recordA.FirstName.Should().Be(recordB.FirstName);
    //     recordA.LastName.Should().Be(recordB.LastName);
    //     recordA.Active.Should().Be(recordB.Active);
    //     recordA.ApplicationId.Should().Be(recordB.ApplicationId);
    //     recordA.CreatedBy.Should().Be(recordB.CreatedBy);
    //     recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    // }
}

