using Contract.Security.Application;
using Contract.Security.User;
using Dto.Security.User;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Contract.Security;
using Data.Security;
using Data.Security.Converters;
using Microsoft.EntityFrameworkCore;
using Data.Security.Models;
using Shared.Logic;
using Dto.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData.Logic;
using static Shared.Logic.Common.Constants;
using Contract.Common.CommonRelationalData;

namespace IntegrationTests.Security.Shared.Utilities;

public class UserUtilities : IUserUtilities
{
    private readonly ISecurityConnectionStrings _connectionStrings;
    private readonly SecurityDBContextFactory _dbContextFactory;
    protected readonly IUserLogic _userLogic;
    protected readonly ICommonRelationalDataLogic _commonRelationalDataLogic;
    
    
    public UserUtilities(ISecurityConnectionStrings connectionStrings, IUserLogic userLogic, ICommonRelationalDataLogic commonRelationalDataLogic) 
    {
        _connectionStrings = connectionStrings;
        _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
        _userLogic = userLogic;
        _commonRelationalDataLogic = commonRelationalDataLogic;
    }

    public InsertUpdateUserRequest ConvertUserDtoToInsertUpdateRequest(UserDto req)
    {
        return new InsertUpdateUserRequest
        {
            Email = req.Email,
            Title = req.Title,
            FirstName = req.FirstName,
            MiddleName = req.MiddleName,
            LastName = req.LastName,
            PreferredName = req.PreferredName,
            Suffix = req.Suffix,
            DateOfBirth = req.DateOfBirth,
            TimeZone = req.TimeZone,
            Active = req.Active,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdateUserRequest CreateInsertUpdateRequestWithMaxLengthErrors()
    {
        return new InsertUpdateUserRequest
        { 
            Email = LogicTestUtilities.GenerateRandomString(120) + "@test.com",
            Title = LogicTestUtilities.GenerateRandomString(9),
            FirstName = LogicTestUtilities.GenerateRandomString(65),
            MiddleName = LogicTestUtilities.GenerateRandomString(65),
            LastName = LogicTestUtilities.GenerateRandomString(65),
            PreferredName = LogicTestUtilities.GenerateRandomString(65),
            Suffix = LogicTestUtilities.GenerateRandomString(9),
            TimeZone = LogicTestUtilities.GenerateRandomString(65),
            Active = true,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdateUserRequest CreateInsertUpdateRequestWithRandomValues(bool active = true)
    {
        return new InsertUpdateUserRequest
        {
            Email = LogicTestUtilities.GenerateRandomString(64) + "@test.com",
            Title = "Mr.",
            FirstName = LogicTestUtilities.GenerateRandomString(32),
            MiddleName = LogicTestUtilities.GenerateRandomString(32),
            LastName = LogicTestUtilities.GenerateRandomString(32),
            PreferredName = LogicTestUtilities.GenerateRandomString(32),
            Suffix = "Jr.",
            TimeZone = "EST",
            DateOfBirth = LogicTestUtilities.GetRandomDateTime(2000),
            Active = active,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single application user test record with randomized data for integration testing purposes.
    /// </summary>
    public async Task<UserDto> CreateSingleUserTestRecord(bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithRandomValues(active);

        var commonData = await GetCommonRelationalDataForUserInsertUpdateValidation();

        var ret = await _userLogic.Insert(insertReq, commonData);

        ret.Errors.Should().BeNullOrEmpty("Insert of user test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined active test application user records in the data store.
    /// </summary>
    public async Task<List<UserDto>> CreateActiveTestRecords(short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<UserDto>();
        var recordsToCreate = new List<InsertUpdateUserRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleUserTestRecord(true));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined inactive test application user records in the data store.
    /// </summary>
    public async Task<List<UserDto>> CreateInactiveTestRecords(short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<UserDto>();
        var recordsToCreate = new List<InsertUpdateUserRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleUserTestRecord(false));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined test active read-only application user records in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application to which the application users belong.</param>
    /// <param name="numberOfRecordsToCreate">The number of active read-only test records to create. Default is 5.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of created active read-only application user DTOs.</returns>
    public async Task<List<UserDto>> CreateActiveReadOnlyTestRecords(short numberOfRecordsToCreate = 5)
    {
        return await CreateReadOnlyTestRecords(true, numberOfRecordsToCreate);
    }

    /// <summary>
    /// Asynchronously creates a set of predefined test inactive read-only application user records in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application to which the application users belong.</param>
    /// <param name="numberOfRecordsToCreate">The number of inactive read-only test records to create. Default is 5.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of created inactive read-only application user DTOs.</returns>
    public async Task<List<UserDto>> CreateInactiveReadOnlyTestRecords(short numberOfRecordsToCreate = 5)
    {
        return await CreateReadOnlyTestRecords(false, numberOfRecordsToCreate);
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        using var dbContext = _dbContextFactory.CreateContextReadWrite();
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [UserLogin]; DELETE FROM [User];");
    }

    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Email", new List<string> { "Email cannot exceed 128 characters!" } },
            { "Title", new List<string> { "Title cannot exceed 8 characters!" } },
            { "FirstName", new List<string> { "FirstName cannot exceed 64 characters!" } },
            { "MiddleName", new List<string> { "MiddleName cannot exceed 64 characters!" } },
            { "LastName", new List<string> { "LastName cannot exceed 64 characters!" } },
            { "PreferredName", new List<string> { "PreferredName cannot exceed 64 characters!" } },
            { "Suffix", new List<string> { "Suffix cannot exceed 8 characters!" } },
            { "TimeZone", new List<string> { "TimeZone cannot exceed 64 characters!" } },
            { "CurrentUser", new List<string> { "CurrentUser cannot exceed 64 characters!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "User", new List<string> { "Record does not exist for specified UserId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Email", new List<string> { "Email is a required field!" } },
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

    public Dictionary<string, List<string>> GetExpectedInvalidTitleFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Title", new List<string> { "Title value is invalid! Value must come from CommonRelationalData.PersonTitle List!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedInvalidSuffixFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Suffix", new List<string> { "Suffix value is invalid! Value must come from CommonRelationalData.PersonSuffix List!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedInvalidTimeZoneFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "TimeZone", new List<string> { "TimeZone value is invalid! Value must come from CommonRelationalData.UsaTimeZone List!" } }
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
    /// Retrieves a dictionary of expected read-only field validation error messages.
    /// </summary>
    /// <returns>A dictionary where the key is the read-only field name and the value is a list of error messages associated with that read-only field.</returns>
    public Dictionary<string, List<string>> GetExpectedReadOnlyErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "User", new List<string> { "Record is read only and cannot be modified! (IE: ReadOnly property is set to true)" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedApplicationUserForeignKeyErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationUsers", new List<string> { "Record still contains child dependencies! IE: ApplicationUsers" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedChangePasswordRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "UserId", new List<string> { "UserId is a required field!" } },
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
    public void VerifyTestRecordValuesMatch(UserDto recordA, UserDto recordB)
    {
        recordA.UserId.Should().Be(recordB.UserId);
        recordA.Email.Should().Be(recordB.Email);
        recordA.Title.Should().Be(recordB.Title);
        recordA.FirstName.Should().Be(recordB.FirstName);
        recordA.MiddleName.Should().Be(recordB.MiddleName);
        recordA.LastName.Should().Be(recordB.LastName);
        recordA.PreferredName.Should().Be(recordB.PreferredName);
        recordA.Suffix.Should().Be(recordB.Suffix);
        recordA.TimeZone.Should().Be(recordB.TimeZone);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ReadOnly.Should().Be(recordB.ReadOnly);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }

    /// <summary>
    /// Verifies that the related data is included and valid on the user record based on the specified parameters.
    /// </summary>
    /// <param name="applicationUser">The user record to verify.</param>
    /// <param name="includeInactive">Indicates whether inactive related data should be included in the verification.</param>
    public void VerifyIncludeRelatedDataOnUser(UserDto user, bool includeInactive = false)
    {
        user.ApplicationUsers.Should().NotBeNull();
        user.ApplicationUsers.Count().Should().BeGreaterThan(0);
            
        foreach (var applicationUser in user.ApplicationUsers)
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

    public async Task<FilterCommonRelationalDataDto> GetCommonRelationalDataForUserInsertUpdateValidation()
    {
        var commonDataRes = await _commonRelationalDataLogic.Filter(new FilterCommonRelationalDataLogicRequest
        {
            ReferenceTypes = new List<string>
            {
                CommonRelationalDataReferenceTypes.PersonTitle,
                CommonRelationalDataReferenceTypes.PersonSuffix,
                CommonRelationalDataReferenceTypes.UsaTimeZone
            }
        });

        return commonDataRes.Response;
    }

    #region Private

    /// <summary>
    /// Asynchronously creates a set of predefined test read-only permission records in the data store.
    /// </summary>
    /// <param name="active">Indicates whether the created read-only test records should be active. Default is true.</param>
    /// <param name="numberOfRecordsToCreate">The number of read-only test records to create. Default is 5.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of created read-only permission DTOs.</returns>
    private async Task<List<UserDto>> CreateReadOnlyTestRecords(bool active = true, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<UserDto>();
        
        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            var insertReq = CreateInsertUpdateRequestWithRandomValues(active);
            var ent = insertReq.ToEntityOnInsert();
            ent.ReadOnly = true;

            ent.UserLogin = new UserLogin
            {
                Password = LogicUtilities.HashPassword(LogicTestUtilities.GenerateRandomString(16)),
                PasswordResetRequired = false
            };

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                await dbContext.Users.AddAsync(ent);
                await dbContext.SaveChangesAsync();
                ret.Add(ent.ToDto());
            }
        }

        return ret;
    }

    #endregion  
}
