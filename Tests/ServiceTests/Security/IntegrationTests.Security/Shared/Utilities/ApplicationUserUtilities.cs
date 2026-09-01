using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Contract.Security;
using Data.Security;
using Data.Security.Converters;
using Microsoft.EntityFrameworkCore;
using Contract.Security.User;

namespace IntegrationTests.Security.Shared.Utilities;

public class ApplicationUserUtilities : IApplicationUserUtilities
{
    private readonly ISecurityConnectionStrings _connectionStrings;
    private readonly SecurityDBContextFactory _dbContextFactory;
    
    protected readonly IApplicationUserLogic _applicationUserLogic;
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly IUserLogic _userLogic;
    
    public ApplicationUserUtilities(ISecurityConnectionStrings connectionStrings, IApplicationUserLogic applicationUserPermissionLogic, IApplicationLogic applicationLogic, IUserLogic userLogic) 
    {
        _connectionStrings = connectionStrings;
        _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
        _applicationUserLogic = applicationUserPermissionLogic;
        _applicationLogic = applicationLogic;
        _userLogic = userLogic;
    }

    public InsertUpdateApplicationUserRequest ConvertApplicationUserDtoToInsertUpdateRequest(ApplicationUserDto req)
    {
        return new InsertUpdateApplicationUserRequest
        {
            Active = req.Active,
            ApplicationId = req.ApplicationId,
            UserId = req.UserId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithMaxLengthErrors(int applicationId, int userId)
    {
        return new InsertUpdateApplicationUserRequest
        { 
            UserId = userId,
            Active = true,
            ApplicationId = applicationId,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdateApplicationUserRequest CreateInsertUpdateRequestWithSpecificValues(int applicationId, int userId, bool active = true)
    {
        return new InsertUpdateApplicationUserRequest
        {
            UserId = userId,
            Active = active,
            ApplicationId = applicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single application user permission test record with specific data for integration testing purposes.
    /// </summary>
    public async Task<ApplicationUserDto> CreateSingleApplicationUserTestRecord(int applicationId, int userId, bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithSpecificValues(applicationId, userId, active);

        var ret = await _applicationUserLogic.Insert(insertReq, _applicationLogic, _applicationUserLogic, _userLogic);

        ret.Errors.Should().BeNullOrEmpty("Insert of application user permission test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined active test application user permission records in the data store.
    /// </summary>
    public async Task<List<ApplicationUserDto>> CreateActiveTestRecords(int applicationId, int userId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<ApplicationUserDto>();
        var recordsToCreate = new List<InsertUpdateApplicationUserRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationUserTestRecord(applicationId, userId, true));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined inactive test application user permission records in the data store.
    /// </summary>
    public async Task<List<ApplicationUserDto>> CreateInactiveTestRecords(int applicationId, int userId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<ApplicationUserDto>();
        var recordsToCreate = new List<InsertUpdateApplicationUserRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationUserTestRecord(applicationId, userId, false));
        }

        return ret;
    }
    
    /// <summary>
    /// Asynchronously creates a test active read-only application user permission record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application for the test record.</param>
    /// <param name="applicationUserId">The ID of the application user for the test record.</param>
    /// <param name="userId">The ID of the permission for the test record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created active read-only application user permission DTO.</returns>
    public async Task<ApplicationUserDto> CreateActiveReadOnlyTestRecord(int applicationId, int userId)
    {
        return await CreateReadOnlyTestRecord(applicationId, userId, true);
    }

    /// <summary>
    /// Asynchronously creates a test inactive read-only application user permission record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application for the test record.</param>
    /// <param name="applicationUserId">The ID of the application user for the test record.</param>
    /// <param name="userId">The ID of the permission for the test record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created inactive read-only application user permission DTO.</returns>
    public async Task<ApplicationUserDto> CreateInactiveReadOnlyTestRecord(int applicationId, int userId)
    {
        return await CreateReadOnlyTestRecord(applicationId, userId, false);
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        using var dbContext = _dbContextFactory.CreateContextReadWrite();
        await dbContext.ApplicationUsers.ExecuteDeleteAsync();
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
            { "ApplicationUser", new List<string> { "Record does not exist for specified ApplicationUserId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationId", new List<string> { "ApplicationId is a required field!" } },
            { "UserId", new List<string> { "UserId is a required field!" } },
            { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
        };
    }

   public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
   {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUser", new List<string> { "ApplicationUser must be unique!" } }
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
            { "ApplicationUser", new List<string> { "Record is read only and cannot be modified! (IE: ReadOnly property is set to true)" } }
        };
    }

    /// <summary>
    /// Verifies that all relevant property values of two application user permission records are equal.
    /// </summary>
    public void VerifyTestRecordValuesMatch(ApplicationUserDto recordA, ApplicationUserDto recordB)
    {
        recordA.ApplicationUserId.Should().Be(recordB.ApplicationUserId);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.ApplicationUserId.Should().Be(recordB.ApplicationUserId);
        recordA.UserId.Should().Be(recordB.UserId);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ReadOnly.Should().Be(recordB.ReadOnly);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }

    /// <summary>
    /// Verifies that the related data is included and valid on the application user record based on the specified parameters.
    /// </summary>
    /// <param name="applicationUser">The application user record to verify.</param>
    /// <param name="includeInactive">Indicates whether inactive related data should be included in the verification.</param>
    public void VerifyIncludeRelatedDataOnApplicationUser(ApplicationUserDto applicationUser, bool includeInactive = false)
    {
        applicationUser.Application.Should().NotBeNull();
        
        applicationUser.ApplicationUserPermissions.Should().NotBeNull();
        applicationUser.ApplicationUserPermissions.Count().Should().BeGreaterThan(0);
            
        foreach (var permission in applicationUser.ApplicationUserPermissions)
        {
            permission.Permission.Should().NotBeNull();
            
            if (!includeInactive)
            {
                permission.Permission.Active.Should().BeTrue();
            }
        }

        applicationUser.ApplicationUserRoles.Should().NotBeNull();
        applicationUser.ApplicationUserRoles.Count().Should().BeGreaterThan(0);

        foreach (var applicationUserRole in applicationUser.ApplicationUserRoles)
        {
            applicationUserRole.Role.Should().NotBeNull();
            applicationUserRole.Role.RolePermissions.Should().NotBeNull();
            applicationUserRole.Role.RolePermissions.Count().Should().BeGreaterThan(0);

            foreach (var rolePermission in applicationUserRole.Role.RolePermissions)
            {
                rolePermission.Permission.Should().NotBeNull();
                
                if (!includeInactive)
                {
                    rolePermission.Permission.Active.Should().BeTrue();
                }
            }
        }
    }

     #region Private

    /// <summary>
    /// Asynchronously creates a predefined test read-only application user record in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application associated with the test record.</param>
    /// <param name="userId">The ID of the user associated with the test record.</param>
    /// <param name="active">Indicates whether the created read-only test records should be active. Default is true.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created read-only application user DTO.</returns>
    private async Task<ApplicationUserDto> CreateReadOnlyTestRecord(int applicationId, int userId, bool active = true)
    {
        //create test records
        ApplicationUserDto ret;
        
        var insertReq = CreateInsertUpdateRequestWithSpecificValues(applicationId, userId, active);
        var ent = insertReq.ToEntityOnInsert();
        ent.ReadOnly = true;

        using (var dbContext = _dbContextFactory.CreateContextReadWrite())
        {
            await dbContext.ApplicationUsers.AddAsync(ent);
            await dbContext.SaveChangesAsync();
            ret = ent.ToDto();
        }

        return ret;
    }

    #endregion 
}

