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
using Contract.Security;
using Data.Security;
using Data.Security.Converters;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Security.Shared.Utilities;

public class RolePermissionUtilities : IRolePermissionUtilities
{
    private readonly ISecurityConnectionStrings _connectionStrings;
    private readonly SecurityDBContextFactory _dbContextFactory;

    protected readonly IRolePermissionLogic _rolePermissionLogic;
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly IRoleLogic _roleLogic;
    protected readonly IPermissionLogic _permissionLogic;
    
    public RolePermissionUtilities(ISecurityConnectionStrings connectionStrings, IRolePermissionLogic rolePermissionLogic, IApplicationLogic applicationLogic, IRoleLogic roleLogic, IPermissionLogic permissionLogic) 
    {
        _connectionStrings = connectionStrings;
        _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
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
    /// Asynchronously creates a test active read-only role permission record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application for the test record.</param>
    /// <param name="roleId">The ID of the role for the test record.</param>
    /// <param name="permissionId">The ID of the permission for the test record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created active read-only role permission DTO.</returns>
    public async Task<RolePermissionDto> CreateActiveReadOnlyTestRecord(int applicationId, int roleId, int permissionId)
    {
        return await CreateReadOnlyTestRecord(applicationId, roleId, permissionId, true);
    }

    /// <summary>
    /// Asynchronously creates a test inactive read-only role permission record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application for the test record.</param>
    /// <param name="roleId">The ID of the role for the test record.</param>
    /// <param name="permissionId">The ID of the permission for the test record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created inactive read-only role permission DTO.</returns>
    public async Task<RolePermissionDto> CreateInactiveReadOnlyTestRecord(int applicationId, int roleId, int permissionId)
    {
        return await CreateReadOnlyTestRecord(applicationId, roleId, permissionId, false);
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        using var dbContext = _dbContextFactory.CreateContextReadWrite();
        await dbContext.RolePermissions.ExecuteDeleteAsync();
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
    /// Retrieves a dictionary of expected read-only field validation error messages.
    /// </summary>
    /// <returns>A dictionary where the key is the read-only field name and the value is a list of error messages associated with that read-only field.</returns>
    public Dictionary<string, List<string>> GetExpectedReadOnlyErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "RolePermission", new List<string> { "Record is read only and cannot be modified! (IE: ReadOnly property is set to true)" } }
        };
    }

    /// <summary>
    /// Verifies that all relevant property values of two role permission records are equal.
    /// </summary>
    public void VerifyTestRecordValuesMatch(RolePermissionDto recordA, RolePermissionDto recordB)
    {
        recordA.RolePermissionId.Should().Be(recordB.RolePermissionId);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.RoleId.Should().Be(recordB.RoleId);
        recordA.PermissionId.Should().Be(recordB.PermissionId);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ReadOnly.Should().Be(recordB.ReadOnly);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }

    #region Private

    /// <summary>
    /// Asynchronously creates a predefined test read-only role permission record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application associated with the test record.</param>
    /// <param name="roleId">The ID of the role associated with the test record.</param>
    /// <param name="permissionId">The ID of the permission associated with the test record.</param>
    /// <param name="active">Indicates whether the created read-only test records should be active. Default is true.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created read-only role permission DTO.</returns>
    private async Task<RolePermissionDto> CreateReadOnlyTestRecord(int applicationId, int roleId, int permissionId, bool active = true)
    {
        //create test records
        RolePermissionDto ret;
        
        var insertReq = CreateInsertUpdateRequestWithSpecificValues(applicationId, roleId, permissionId, active);
        var ent = insertReq.ToEntityOnInsert();
        ent.ReadOnly = true;

        using (var dbContext = _dbContextFactory.CreateContextReadWrite())
        {
            await dbContext.RolePermissions.AddAsync(ent);
            await dbContext.SaveChangesAsync();
            ret = ent.ToDto();
        }

        return ret;
    }

    #endregion 
}

