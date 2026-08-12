using Contract.Security.Application;
using Contract.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using Shared.Models;
using IntegrationTests.Shared.Utilities;
using Contract.Security.Permission;
using Contract.Security.ApplicationUser;
using Contract.Security;
using Data.Security;
using Data.Security.Converters;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Security.Shared.Utilities;

public class ApplicationUserPermissionUtilities : IApplicationUserPermissionUtilities
{
    private readonly ISecurityConnectionStrings _connectionStrings;
    private readonly SecurityDBContextFactory _dbContextFactory;
    
    protected readonly IApplicationUserPermissionLogic _applicationUserPermissionLogic;
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly IApplicationUserLogic _applicationUserLogic;
    protected readonly IPermissionLogic _permissionLogic;
    
    public ApplicationUserPermissionUtilities(ISecurityConnectionStrings connectionStrings, IApplicationUserPermissionLogic applicationUserPermissionLogic, IApplicationLogic applicationLogic, IApplicationUserLogic applicationUserLogic, IPermissionLogic permissionLogic) 
    {
        _connectionStrings = connectionStrings;
        _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
        _applicationUserPermissionLogic = applicationUserPermissionLogic;
        _applicationLogic = applicationLogic;
        _applicationUserLogic = applicationUserLogic;
        _permissionLogic = permissionLogic;
    }

    public InsertUpdateApplicationUserPermissionRequest ConvertApplicationUserPermissionDtoToInsertUpdateRequest(ApplicationUserPermissionDto req)
    {
        return new InsertUpdateApplicationUserPermissionRequest
        {
            Active = req.Active,
            ApplicationId = req.ApplicationId,
            ApplicationUserId = req.ApplicationUserId,
            PermissionId = req.PermissionId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdateApplicationUserPermissionRequest CreateInsertUpdateRequestWithMaxLengthErrors(int applicationId, int applicationUserId, int permissionId)
    {
        return new InsertUpdateApplicationUserPermissionRequest
        { 
            ApplicationUserId = applicationUserId,
            PermissionId = permissionId,
            Active = true,
            ApplicationId = applicationId,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdateApplicationUserPermissionRequest CreateInsertUpdateRequestWithSpecificValues(int applicationId, int applicationUserId, int permissionId, bool active = true)
    {
        return new InsertUpdateApplicationUserPermissionRequest
        {
            ApplicationUserId = applicationUserId,
            PermissionId = permissionId,
            Active = active,
            ApplicationId = applicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single application user permission test record with specific data for integration testing purposes.
    /// </summary>
    public async Task<ApplicationUserPermissionDto> CreateSingleApplicationUserPermissionTestRecord(int applicationId, int applicationUserId, int permissionId, bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithSpecificValues(applicationId, applicationUserId, permissionId, active);

        var ret = await _applicationUserPermissionLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _permissionLogic);

        ret.Errors.Should().BeNullOrEmpty("Insert of application user permission test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined active test application user permission records in the data store.
    /// </summary>
    public async Task<List<ApplicationUserPermissionDto>> CreateActiveTestRecords(int applicationId, int applicationUserId, int permissionId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<ApplicationUserPermissionDto>();
        var recordsToCreate = new List<InsertUpdateApplicationUserPermissionRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationUserPermissionTestRecord(applicationId, applicationUserId, permissionId, true));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined inactive test application user permission records in the data store.
    /// </summary>
    public async Task<List<ApplicationUserPermissionDto>> CreateInactiveTestRecords(int applicationId, int applicationUserId, int permissionId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<ApplicationUserPermissionDto>();
        var recordsToCreate = new List<InsertUpdateApplicationUserPermissionRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationUserPermissionTestRecord(applicationId, applicationUserId, permissionId, false));
        }

        return ret;
    }
    
    /// <summary>
    /// Asynchronously creates a test active read-only application user permission record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application for the test record.</param>
    /// <param name="applicationUserId">The ID of the application user for the test record.</param>
    /// <param name="permissionId">The ID of the permission for the test record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created active read-only application user permission DTO.</returns>
    public async Task<ApplicationUserPermissionDto> CreateActiveReadOnlyTestRecord(int applicationId, int applicationUserId, int permissionId)
    {
        return await CreateReadOnlyTestRecord(applicationId, applicationUserId, permissionId, true);
    }

    /// <summary>
    /// Asynchronously creates a test inactive read-only application user permission record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application for the test record.</param>
    /// <param name="applicationUserId">The ID of the application user for the test record.</param>
    /// <param name="permissionId">The ID of the permission for the test record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created inactive read-only application user permission DTO.</returns>
    public async Task<ApplicationUserPermissionDto> CreateInactiveReadOnlyTestRecord(int applicationId, int applicationUserId, int permissionId)
    {
        return await CreateReadOnlyTestRecord(applicationId, applicationUserId, permissionId, false);
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        using var dbContext = _dbContextFactory.CreateContextReadWrite();
        await dbContext.ApplicationUserPermissions.ExecuteDeleteAsync();
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
            { "ApplicationUserPermission", new List<string> { "Record does not exist for specified ApplicationUserPermissionId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationId", new List<string> { "ApplicationId is a required field!" } },
            { "ApplicationUserId", new List<string> { "ApplicationUserId is a required field!" } },
            { "PermissionId", new List<string> { "PermissionId is a required field!" } },
            { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
        };
    }

   public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
   {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUserPermission", new List<string> { "ApplicationUserPermission must be unique!" } }
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
            { "ApplicationUserPermission", new List<string> { "Record is read only and cannot be modified! (IE: ReadOnly property is set to true)" } }
        };
    }

    /// <summary>
    /// Verifies that all relevant property values of two application user permission records are equal.
    /// </summary>
    public void VerifyTestRecordValuesMatch(ApplicationUserPermissionDto recordA, ApplicationUserPermissionDto recordB)
    {
        recordA.ApplicationUserPermissionId.Should().Be(recordB.ApplicationUserPermissionId);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.ApplicationUserId.Should().Be(recordB.ApplicationUserId);
        recordA.PermissionId.Should().Be(recordB.PermissionId);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ReadOnly.Should().Be(recordB.ReadOnly);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }

     #region Private

    /// <summary>
    /// Asynchronously creates a predefined test read-only application user permission record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application associated with the test record.</param>
    /// <param name="applicationUserId">The ID of the application user associated with the test record.</param>
    /// <param name="permissionId">The ID of the permission associated with the test record.</param>
    /// <param name="active">Indicates whether the created read-only test records should be active. Default is true.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created read-only application user permission DTO.</returns>
    private async Task<ApplicationUserPermissionDto> CreateReadOnlyTestRecord(int applicationId, int applicationUserId, int permissionId, bool active = true)
    {
        //create test records
        ApplicationUserPermissionDto ret;
        
        var insertReq = CreateInsertUpdateRequestWithSpecificValues(applicationId, applicationUserId, permissionId, active);
        var ent = insertReq.ToEntityOnInsert();
        ent.ReadOnly = true;

        using (var dbContext = _dbContextFactory.CreateContextReadWrite())
        {
            await dbContext.ApplicationUserPermissions.AddAsync(ent);
            await dbContext.SaveChangesAsync();
            ret = ent.ToDto();
        }

        return ret;
    }

    #endregion 
}

