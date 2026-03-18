using Contract.Security.Application;
using Contract.Security.RolePermission;
using Dto.Security.RolePermission;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using Shared.Models;
using IntegrationTests.Shared.Utilities;
using Contract.Security.Permission;
using Contract.Security.Role;

namespace IntegrationTests.Security.Shared.Utilities;

public class RolePermissionUtilities : IRolePermissionUtilities
{
    protected readonly IRolePermissionLogic _rolePermissionLogic;
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly IRoleLogic _roleLogic;
    protected readonly IPermissionLogic _permissionLogic;
    
    public RolePermissionUtilities(IRolePermissionLogic rolePermissionLogic, IApplicationLogic applicationLogic, IRoleLogic roleLogic, IPermissionLogic permissionLogic) 
    {
        _rolePermissionLogic = rolePermissionLogic;
        _applicationLogic = applicationLogic;
        _roleLogic = roleLogic;
        _permissionLogic = permissionLogic;
    }

    public InsertUpdateRolePermissionRequest ConvertRolePermissionDtoToInsertUpdateRequest(RolePermissionDto req)
    {
        return new InsertUpdateRolePermissionRequest
        {
            Active = req.Active,
            ApplicationId = req.ApplicationId,
            RoleId = req.RoleId,
            PermissionId = req.PermissionId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdateRolePermissionRequest CreateInsertUpdateRequestWithMaxLengthErrors(int applicationId, int roleId, int permissionId)
    {
        return new InsertUpdateRolePermissionRequest
        { 
            RoleId = roleId,
            PermissionId = permissionId,
            Active = true,
            ApplicationId = applicationId,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdateRolePermissionRequest CreateInsertUpdateRequestWithSpecificValues(int applicationId, int roleId, int permissionId, bool active = true)
    {
        return new InsertUpdateRolePermissionRequest
        {
            RoleId = roleId,
            PermissionId = permissionId,
            Active = active,
            ApplicationId = applicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single application user permission test record with specific data for integration testing purposes.
    /// </summary>
    public async Task<RolePermissionDto> CreateSingleRolePermissionTestRecord(int applicationId, int roleId, int permissionId, bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithSpecificValues(applicationId, roleId, permissionId, active);

        var ret = await _rolePermissionLogic.Insert(insertReq, _applicationLogic, _roleLogic, _permissionLogic);

        ret.Errors.Should().BeNullOrEmpty("Insert of role permission test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined active test application user permission records in the data store.
    /// </summary>
    public async Task<List<RolePermissionDto>> CreateActiveTestRecords(int applicationId, int roleId, int permissionId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<RolePermissionDto>();
        var recordsToCreate = new List<InsertUpdateRolePermissionRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleRolePermissionTestRecord(applicationId, roleId, permissionId, true));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined inactive test application user permission records in the data store.
    /// </summary>
    public async Task<List<RolePermissionDto>> CreateInactiveTestRecords(int applicationId, int roleId, int permissionId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<RolePermissionDto>();
        var recordsToCreate = new List<InsertUpdateRolePermissionRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleRolePermissionTestRecord(applicationId, roleId, permissionId, false));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        var recordsToDelete = await _rolePermissionLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

        foreach (var record in recordsToDelete.Response)
        {
            await _rolePermissionLogic.Delete(record.RolePermissionId);
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
            { "RolePermission", new List<string> { "Record does not exist for specified RolePermissionId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationId", new List<string> { "ApplicationId is a required field!" } },
            { "RoleId", new List<string> { "RoleId is a required field!" } },
            { "PermissionId", new List<string> { "PermissionId is a required field!" } },
            { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
        };
    }

   public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
   {
        return new Dictionary<string, List<string>>
        {
            { "RolePermission", new List<string> { "RolePermission must be unique!" } }
        };
   }

    /// <summary>
    /// Verifies that all relevant property values of two application user permission records are equal.
    /// </summary>
    public void VerifyTestRecordValuesMatch(RolePermissionDto recordA, RolePermissionDto recordB)
    {
        recordA.RolePermissionId.Should().Be(recordB.RolePermissionId);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.RoleId.Should().Be(recordB.RoleId);
        recordA.PermissionId.Should().Be(recordB.PermissionId);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }
}

