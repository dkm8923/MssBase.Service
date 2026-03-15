using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using Shared.Models;
using IntegrationTests.Shared.Utilities;

namespace IntegrationTests.Security.Shared.Utilities;

public class ApplicationUserUtilities : IApplicationUserUtilities
{
    protected readonly IApplicationUserLogic _applicationUserLogic;
    protected readonly IApplicationLogic _applicationLogic;
    
    public ApplicationUserUtilities(IApplicationUserLogic applicationUserLogic, IApplicationLogic applicationLogic) 
    {
        _applicationUserLogic = applicationUserLogic;
        _applicationLogic = applicationLogic;
    }

    /// <summary>
    /// Asynchronously clears test data from the ApplicationUser and Application tables, creates a fresh test application record, 
    /// and returns the application ID for use in subsequent tests.
    /// </summary>
    public async Task<int> ClearTestTablesAndReturnApplicationId(IApplicationUtilities applicationUtilities)
    {
        await DeleteAllRecords();
        await applicationUtilities.DeleteAllRecords();
        return (await applicationUtilities.CreateActiveTestRecords(1)).FirstOrDefault().ApplicationId;
    }

    public InsertUpdateApplicationUserRequest ConvertApplicationUserDtoToInsertUpdateRequest(ApplicationUserDto req)
    {
        return new InsertUpdateApplicationUserRequest
        {
            Email = req.Email,
            FirstName = req.FirstName,
            LastName = req.LastName,
            DateOfBirth = req.DateOfBirth,
            Password = req.Password,
            LastLoginDate = req.LastLoginDate,
            LastPasswordChangeDate = req.LastPasswordChangeDate,
            LastLockoutDate = req.LastLockoutDate,
            FailedPasswordAttemptCount = req.FailedPasswordAttemptCount,
            Active = req.Active,
            ApplicationId = req.ApplicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithMaxLengthErrors()
    {
        return new InsertUpdateApplicationUserRequest
        { 
            Email = LogicTestUtilities.GenerateRandomString(120) + "@test.com",
            FirstName = LogicTestUtilities.GenerateRandomString(65),
            LastName = LogicTestUtilities.GenerateRandomString(65),
            Password = LogicTestUtilities.GenerateRandomString(65),
            Active = true,
            ApplicationId = 1,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true)
    {
        return new InsertUpdateApplicationUserRequest
        {
            Email = LogicTestUtilities.GenerateRandomString(64) + "@test.com",
            FirstName = LogicTestUtilities.GenerateRandomString(32),
            LastName = LogicTestUtilities.GenerateRandomString(32),
            Password = LogicTestUtilities.GenerateRandomString(64),
            Active = active,
            ApplicationId = applicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single application user test record with randomized data for integration testing purposes.
    /// </summary>
    public async Task<ApplicationUserDto> CreateSingleApplicationUserTestRecord(int applicationId, bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithRandomValues(applicationId, active);

        var ret = await _applicationUserLogic.Insert(insertReq, _applicationLogic);

        ret.Errors.Should().BeNullOrEmpty("Insert of application user test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Creates a single application user test record with specific values provided in the request object.
    /// </summary>
    // public async Task<ApplicationUserDto> CreateSingleApplicationUserTestRecordWithSpecificValues(int applicationId, InsertUpdateApplicationUserRequest req = null)
    // {
    //     var ret = new ErrorValidationResult<ApplicationUserDto>();

    //     if (req == null)
    //     {
    //         var insertReq = new InsertUpdateApplicationUserRequest
    //         {
    //             Email = "test@applicationuser.com",
    //             FirstName = "Test",
    //             LastName = "ApplicationUser",
    //             Active = true,
    //             ApplicationId = 1,
    //             CurrentUser = TestConstants.CurrentUser
    //         };

    //         ret = await _applicationUserLogic.Insert(insertReq);
    //     }
    //     else
    //     {
    //         req.Email = req.Email ?? "test@applicationuser.com";
    //         req.FirstName = req.FirstName ?? "Test";
    //         req.LastName = req.LastName ?? "ApplicationUser";
    //         req.ApplicationId = req.ApplicationId == 0 ? 1 : req.ApplicationId;
    //         req.CurrentUser = req.CurrentUser ?? TestConstants.CurrentUser; 
    //         ret = await _applicationUserLogic.Insert(req);
    //     }

    //     return ret.Response;
    // }

    /// <summary>
    /// Asynchronously creates a set of predefined active test application user records in the data store.
    /// </summary>
    public async Task<List<ApplicationUserDto>> CreateActiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<ApplicationUserDto>();
        var recordsToCreate = new List<InsertUpdateApplicationUserRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationUserTestRecord(applicationId, true));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined inactive test application user records in the data store.
    /// </summary>
    public async Task<List<ApplicationUserDto>> CreateInactiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<ApplicationUserDto>();
        var recordsToCreate = new List<InsertUpdateApplicationUserRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationUserTestRecord(applicationId, false));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        var recordsToDelete = await _applicationUserLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

        foreach (var record in recordsToDelete.Response)
        {
            await _applicationUserLogic.Delete(record.ApplicationUserId);
        }
    }

    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Email", new List<string> { "Email cannot exceed 128 characters!" } },
            { "FirstName", new List<string> { "FirstName cannot exceed 64 characters!" } },
            { "LastName", new List<string> { "LastName cannot exceed 64 characters!" } },
            { "Password", new List<string> { "Password cannot exceed 64 characters!" } },
            { "CurrentUser", new List<string> { "CurrentUser cannot exceed 64 characters!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUser", new List<string> { "Record does not exist for specified ApplicationUserId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Email", new List<string> { "Email is a required field!" } },
            { "ApplicationId", new List<string> { "ApplicationId is a required field!" } },
            { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedInvalidEmailFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Email", new List<string> { "Email must be in a valid format!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Email", new List<string> { "Email must be unique!" } }
        };
    }

    /// <summary>
    /// Verifies that all relevant property values of two application user records are equal.
    /// </summary>
    public void VerifyTestRecordValuesMatch(ApplicationUserDto recordA, ApplicationUserDto recordB)
    {
        recordA.ApplicationUserId.Should().Be(recordB.ApplicationUserId);
        recordA.Email.Should().Be(recordB.Email);
        recordA.FirstName.Should().Be(recordB.FirstName);
        recordA.LastName.Should().Be(recordB.LastName);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }
}
