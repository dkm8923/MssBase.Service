using Contract.Security.Application;
using Contract.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using Shared.Models;
using IntegrationTests.Shared.Utilities;
using Contract.Security.Role;
using Contract.Security.ApplicationUser;

namespace IntegrationTests.Security.Shared.Utilities;

public class ApplicationUserRoleUtilities : IApplicationUserRoleUtilities
{
    protected readonly IApplicationUserRoleLogic _applicationUserRoleLogic;
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly IApplicationUserLogic _applicationUserLogic;
    protected readonly IRoleLogic _roleLogic;
    
    public ApplicationUserRoleUtilities(IApplicationUserRoleLogic applicationUserRoleLogic, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IRoleLogic roleLogic) 
    {
        _applicationUserRoleLogic = applicationUserRoleLogic;
        _applicationLogic = applicationLogic;
        _applicationUserLogic = applicationUserLogic;
        _roleLogic = roleLogic;
    }

    public InsertUpdateApplicationUserRoleRequest ConvertApplicationUserRoleDtoToInsertUpdateRequest(ApplicationUserRoleDto req)
    {
        return new InsertUpdateApplicationUserRoleRequest
        {
            Active = req.Active,
            ApplicationId = req.ApplicationId,
            ApplicationUserId = req.ApplicationUserId,
            RoleId = req.RoleId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdateApplicationUserRoleRequest CreateInsertUpdateRequestWithMaxLengthErrors(int applicationId, int applicationUserId, int roleId)
    {
        return new InsertUpdateApplicationUserRoleRequest
        { 
            ApplicationUserId = applicationUserId,
            RoleId = roleId,
            Active = true,
            ApplicationId = applicationId,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdateApplicationUserRoleRequest CreateInsertUpdateRequestWithSpecificValues(int applicationId, int applicationUserId, int roleId, bool active = true)
    {
        return new InsertUpdateApplicationUserRoleRequest
        {
            ApplicationUserId = applicationUserId,
            RoleId = roleId,
            Active = active,
            ApplicationId = applicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single application user role test record with specific data for integration testing purposes.
    /// </summary>
    public async Task<ApplicationUserRoleDto> CreateSingleApplicationUserRoleTestRecord(int applicationId, int applicationUserId, int roleId, bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithSpecificValues(applicationId, applicationUserId, roleId, active);

        var ret = await _applicationUserRoleLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _roleLogic);

        ret.Errors.Should().BeNullOrEmpty("Insert of application user role test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined active test application user role records in the data store.
    /// </summary>
    public async Task<List<ApplicationUserRoleDto>> CreateActiveTestRecords(int applicationId, int applicationUserId, int roleId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<ApplicationUserRoleDto>();
        var recordsToCreate = new List<InsertUpdateApplicationUserRoleRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationUserRoleTestRecord(applicationId, applicationUserId, roleId, true));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined inactive test application user role records in the data store.
    /// </summary>
    public async Task<List<ApplicationUserRoleDto>> CreateInactiveTestRecords(int applicationId, int applicationUserId, int roleId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<ApplicationUserRoleDto>();
        var recordsToCreate = new List<InsertUpdateApplicationUserRoleRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationUserRoleTestRecord(applicationId, applicationUserId, roleId, false));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        var recordsToDelete = await _applicationUserRoleLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

        foreach (var record in recordsToDelete.Response)
        {
            await _applicationUserRoleLogic.Delete(record.ApplicationUserRoleId);
        }
    }

    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "CurrentUser", new List<string> { "CurrentUser cannot exceed 64 characters!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUserRole", new List<string> { "Record does not exist for specified ApplicationUserRoleId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationId", new List<string> { "ApplicationId is a required field!" } },
            { "ApplicationUserId", new List<string> { "ApplicationUserId is a required field!" } },
            { "RoleId", new List<string> { "RoleId is a required field!" } },
            { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
        };
    }

   public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
   {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUserRole", new List<string> { "ApplicationUserRole must be unique!" } }
        };
   }

    /// <summary>
    /// Verifies that all relevant property values of two application user role records are equal.
    /// </summary>
    public void VerifyTestRecordValuesMatch(ApplicationUserRoleDto recordA, ApplicationUserRoleDto recordB)
    {
        recordA.ApplicationUserRoleId.Should().Be(recordB.ApplicationUserRoleId);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.ApplicationUserId.Should().Be(recordB.ApplicationUserId);
        recordA.RoleId.Should().Be(recordB.RoleId);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }
}

