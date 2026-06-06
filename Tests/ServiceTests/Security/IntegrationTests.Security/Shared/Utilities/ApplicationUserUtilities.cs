using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Dto.Security.ApplicationUser;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using Shared.Models;
using IntegrationTests.Shared.Utilities;
using Contract.Security;

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

    public InsertUpdateApplicationUserRequest ConvertApplicationUserDtoToInsertUpdateRequest(ApplicationUserDto req)
    {
        return new InsertUpdateApplicationUserRequest
        {
            Email = req.Email,
            FirstName = req.FirstName,
            LastName = req.LastName,
            DateOfBirth = req.DateOfBirth,
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
            DateOfBirth = LogicTestUtilities.GetRandomDateTime(2000),
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
        var applicationUserIdsToDelete = recordsToDelete.Response.Select(x => x.ApplicationUserId).ToList();
        applicationUserIdsToDelete.ForEach(async id => await _applicationUserLogic.Delete(id));
    }

    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Email", new List<string> { "Email cannot exceed 128 characters!" } },
            { "FirstName", new List<string> { "FirstName cannot exceed 64 characters!" } },
            { "LastName", new List<string> { "LastName cannot exceed 64 characters!" } },
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
            { "Email", new List<string> { "Invalid email address!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Email", new List<string> { "Email must be unique!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedApplicationUserPermissionForeignKeyErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUserPermissions", new List<string> { "Record still contains child dependencies! IE: ApplicationUserPermissions" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedApplicationUserRoleForeignKeyErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUserRoles", new List<string> { "Record still contains child dependencies! IE: ApplicationUserRoles" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedChangePasswordRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUserId", new List<string> { "ApplicationUserId is a required field!" } },
            { "NewPassword", new List<string> { "NewPassword is a required field!" } },
            { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedChangePasswordInvalidPasswordErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ChangePassword", new List<string> { "New password must be different from the old password!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedChangePasswordMinMaxLengthErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "NewPassword", new List<string> { "NewPassword must be between 12 and 128 characters!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedChangePasswordUpperCaseRequiredErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "NewPassword", new List<string> { "NewPassword must contain at least one uppercase letter!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedChangePasswordLowerCaseRequiredErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "NewPassword", new List<string> { "NewPassword must contain at least one lowercase letter!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedChangePasswordSpecialCharacterRequiredErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "NewPassword", new List<string> { "NewPassword must contain at least one special character!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedChangePasswordNumberRequiredErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "NewPassword", new List<string> { "NewPassword must contain at least one number!" } }
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

    /// <summary>
    /// Verifies that the related data is included and valid on the application user record based on the specified parameters.
    /// </summary>
    /// <param name="applicationUser">The application user record to verify.</param>
    /// <param name="includeInactive">Indicates whether inactive related data should be included in the verification.</param>
    public void VerifyIncludeRelatedDataOnApplicationUser(ApplicationUserDto applicationUser, bool includeInactive = false)
    {
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
}
