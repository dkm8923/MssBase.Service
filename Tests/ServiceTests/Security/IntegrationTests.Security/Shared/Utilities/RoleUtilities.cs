using Contract.Security.Application;
using Contract.Security.Role;
using Dto.Security.Role;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using Shared.Models;
using IntegrationTests.Shared.Utilities;

namespace IntegrationTests.Security.Shared.Utilities;

public class RoleUtilities : IRoleUtilities
{
    protected readonly IRoleLogic _RoleLogic;
    protected readonly IApplicationLogic _applicationLogic;
    
    public RoleUtilities(IRoleLogic RoleLogic, IApplicationLogic applicationLogic) 
    {
        _RoleLogic = RoleLogic;
        _applicationLogic = applicationLogic;
    }

    /// <summary>
    /// Asynchronously clears test data from the Role and Application tables, creates a fresh test application record, 
    /// and returns the application ID for use in subsequent tests.
    /// </summary>
    public async Task<int> ClearTestTablesAndReturnApplicationId(IApplicationUtilities applicationUtilities)
    {
        await DeleteAllRecords();
        await applicationUtilities.DeleteAllRecords();
        return (await applicationUtilities.CreateTestRecords(1, true)).FirstOrDefault().ApplicationId;
    }

    public InsertUpdateRoleRequest ConvertRoleDtoToInsertUpdateRequest(RoleDto req)
    {
        return new InsertUpdateRoleRequest
        {
            Active = req.Active,
            Name = req.Name,
            Description = req.Description,
            ApplicationId = req.ApplicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdateRoleRequest CreateInsertUpdateRequestWithMaxLengthErrors()
    {
        return new InsertUpdateRoleRequest
        { 
            Name = LogicTestUtilities.GenerateRandomString(120),
            Description = LogicTestUtilities.GenerateRandomString(65),
            Active = true,
            ApplicationId = 1,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdateRoleRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true)
    {
        return new InsertUpdateRoleRequest
        {
            Name = LogicTestUtilities.GenerateRandomString(64),
            Description = LogicTestUtilities.GenerateRandomString(32),
            Active = active,
            ApplicationId = applicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single role test record with randomized data for integration testing purposes.
    /// </summary>
    public async Task<RoleDto> CreateSingleRoleTestRecord(int applicationId, bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithRandomValues(applicationId, active);

        var ret = await _RoleLogic.Insert(insertReq, _applicationLogic);

        ret.Errors.Should().BeNullOrEmpty("Insert of role test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined test role records in the data store.
    /// </summary>
    public async Task<List<RoleDto>> CreateTestRecords(int applicationId, short numberOfRecordsToCreate = 5, bool active = true)
    {
        //create test records
        var ret = new List<RoleDto>();
        var recordsToCreate = new List<InsertUpdateRoleRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleRoleTestRecord(applicationId, active));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        var recordsToDelete = await _RoleLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

        foreach (var record in recordsToDelete.Response)
        {
            await _RoleLogic.Delete(record.RoleId);
        }
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

    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Role", new List<string> { "Record does not exist for specified RoleId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Name", new List<string> { "Name is a required field!" } },
            { "ApplicationId", new List<string> { "ApplicationId is a required field!" } },
            { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Name", new List<string> { "Name must be unique!" } }
        };
    }

    /// <summary>
    /// Verifies that all relevant property values of two role records are equal.
    /// </summary>
    public void VerifyTestRecordValuesMatch(RoleDto recordA, RoleDto recordB)
    {
        recordA.RoleId.Should().Be(recordB.RoleId);
        recordA.Name.Should().Be(recordB.Name);
        recordA.Description.Should().Be(recordB.Description);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }
}
