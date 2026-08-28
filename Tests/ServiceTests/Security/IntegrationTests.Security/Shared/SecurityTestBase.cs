using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Dto.Security.Application;
using Dto.Security.Application.Logic;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using FluentValidation;
using IntegrationTests.Security.Shared.Utilities;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Logic.Security.Logic;
using Logic.Security.Validators.Application;
using Logic.Security.Validators.ApplicationUser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Service.Security.Service;
using Service.Logger.Contracts;
using Tests.Shared;
using MssBase.Service.Shared.ConnectionStrings;
using Contract.Security;
using Contract.Security.Role;
using Dto.Security.Role.Logic;
using Dto.Security.Role;
using Logic.Security.Validators.Role;
using Contract.Security.Permission;
using Dto.Security.Permission.Logic;
using Logic.Security.Validators.Permission;
using Dto.Security.Permission;
using Contract.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Logic;
using Dto.Security.ApplicationUserPermission;
using Logic.Security.Validators.ApplicationUserPermission;
using Contract.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Logic;
using Logic.Security.Validators.ApplicationUserRole;
using Dto.Security.ApplicationUserRole;
using Contract.Security.RolePermission;
using Dto.Security.RolePermission.Logic;
using Logic.Security.Validators.RolePermission;
using Dto.Security.RolePermission;
using Data.Security;
using Contract.Security.Authentication;
using Dto.Security.Authentication;
using Logic.Security.Validators.Authentication;
using Shared.Models;
using MssBase.Service.Shared.Authorization;
using Microsoft.EntityFrameworkCore;
using IntegrationTests.Shared.Models.Config;
using Contract.Security.User;
using Dto.Security.User.Logic;
using Dto.Security.User;
using Logic.Security.Validators.User;

namespace IntegrationTests.Security.Shared;

public class SecurityTestBase
{
    private readonly TestConfig _testConfig;
    private readonly ISecurityConnectionStrings _connectionStrings;
    protected readonly SecurityDBContextFactory _dbContextFactory;
    private readonly AppSettingsHelper _configHelper;
    protected readonly ServiceProvider _serviceProvider;
    protected readonly ILoggerService _loggerSvc;
    protected readonly IAuthenticationLogic _authenticationLogic;
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly IUserLogic _userLogic;
    protected readonly IApplicationUserLogic _applicationUserLogic;
    protected readonly IApplicationUserPermissionLogic _applicationUserPermissionLogic;
    protected readonly IApplicationUserRoleLogic _applicationUserRoleLogic;
    protected readonly IPermissionLogic _permissionLogic;
    protected readonly IRoleLogic _roleLogic;
    protected readonly IRolePermissionLogic _rolePermissionLogic;
    protected readonly ISecurityTestUtilitiesManager _securityTestUtilities;
    protected readonly IOptionsMonitor<AuthenticationSettingsConfig> _authenticationSettingsConfigMonitor;
    protected readonly IOptionsMonitor<JwtAuthenticationConfig> _jwtAuthenticationConfigMonitor;
    protected readonly IOptionsMonitor<PasswordValidationConfig> _passwordValidationConfigMonitor;

    public SecurityTestBase()
    {
        //set environment variable to key off of in program.cs
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        _configHelper = new AppSettingsHelper();

        _serviceProvider = ConfigureServices();
        _testConfig = _serviceProvider.GetService<IOptionsMonitor<TestConfig>>().CurrentValue;
        
        //get instance of db context factory (Should be using services for 99% of DB interactions, but their are always edge cases)
        _connectionStrings = _serviceProvider.GetService<ISecurityConnectionStrings>();
        _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);

        _loggerSvc = _serviceProvider.GetService<ILoggerService>();
        _authenticationSettingsConfigMonitor = _serviceProvider.GetService<IOptionsMonitor<AuthenticationSettingsConfig>>();
        _jwtAuthenticationConfigMonitor = _serviceProvider.GetService<IOptionsMonitor<JwtAuthenticationConfig>>();
        _passwordValidationConfigMonitor = _serviceProvider.GetService<IOptionsMonitor<PasswordValidationConfig>>();
        _authenticationLogic = _serviceProvider.GetService<IAuthenticationLogic>();
        _applicationLogic = _serviceProvider.GetService<IApplicationLogic>();
        _userLogic = _serviceProvider.GetService<IUserLogic>(); 
        _applicationUserLogic = _serviceProvider.GetService<IApplicationUserLogic>();
        _applicationUserPermissionLogic = _serviceProvider.GetService<IApplicationUserPermissionLogic>();
        _applicationUserRoleLogic = _serviceProvider.GetService<IApplicationUserRoleLogic>();
        _permissionLogic = _serviceProvider.GetService<IPermissionLogic>();
        _roleLogic = _serviceProvider.GetService<IRoleLogic>();
        _rolePermissionLogic = _serviceProvider.GetService<IRolePermissionLogic>();
        _securityTestUtilities = _serviceProvider.GetService<ISecurityTestUtilitiesManager>();
    }

    protected async Task ClearAllSecurityTestTableData()
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateContextReadWrite();
        
            if (_testConfig.DatabaseType == "SqlServer")
            {
                var query = @"
                    DELETE FROM [ApplicationUserPermission];
                    DELETE FROM [ApplicationUserRole];
                    DELETE FROM [RolePermission];
                    DELETE FROM [Role];
                    DELETE FROM [Permission];
                    DELETE FROM [UserLogin];
                    DELETE FROM [User];
                    DELETE FROM [ApplicationUser_Log_Login];
                    DELETE FROM [ApplicationUser_Log_ChangePassword];
                    DELETE FROM [ApplicationUserLogin];
                    DELETE FROM [ApplicationUser];
                    DELETE FROM [Application];
                    DELETE FROM [AuditLog];
                ";
                await dbContext.Database.ExecuteSqlRawAsync(query);
            }
            else
            {
                await dbContext.ApplicationUserPermissions.ExecuteDeleteAsync();
                await dbContext.ApplicationUserRoles.ExecuteDeleteAsync();
                await dbContext.RolePermissions.ExecuteDeleteAsync();
                await dbContext.Roles.ExecuteDeleteAsync();
                await dbContext.Permissions.ExecuteDeleteAsync();
                await dbContext.UserLogins.ExecuteDeleteAsync();
                await dbContext.Users.ExecuteDeleteAsync();
                await dbContext.ApplicationUserLogins.ExecuteDeleteAsync();
                await dbContext.ApplicationUsers.ExecuteDeleteAsync();
                await dbContext.Applications.ExecuteDeleteAsync();
                await dbContext.AuditLogs.ExecuteDeleteAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected async Task DropDatabaseAndRecreate()
    {
        using (var dbContext = _dbContextFactory.CreateContextReadWrite())
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }

    protected async Task<ApplicationUserDto> CreateTestUserWithPermissions(int applicationId, AssignRoleRequest req)
    {
        try
        {
            var testUser = await _applicationUserLogic.Insert(new InsertUpdateApplicationUserRequest { 
                Active = true, 
                ApplicationId = applicationId, 
                Email = TestConstants.DefaultTestUserEmail, 
                FirstName = "Bob", 
                LastName = "Smith", 
                DateOfBirth = new DateTime(1987, 2, 12),
                CurrentUser = TestConstants.CurrentUser 
            }, _applicationLogic);

            if (testUser.Response != null)
            {
                //change password for users so they can be used for authentication testing...
                await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { ApplicationUserId = testUser.Response.ApplicationUserId, NewPassword = TestConstants.DefaultTestUserPassword, CurrentUser = TestConstants.CurrentUser });
            }

            var applicationUserId = testUser.Response.ApplicationUserId;

            //Create default application user permissions for user
            await CreateDefaultApplicationUserPermissionsForTestUser(applicationId, applicationUserId);

            if (req.ApplicationAdmin || req.ApplicationReadOnly)
            {
                var applicationRolesWithPermissions = await CreateDefaultApplicationRolesWithPermissions(applicationId);
                var roleId = req.ApplicationAdmin ? applicationRolesWithPermissions.Where(x => x.Name == UserApiRoles.ApplicationAdmin).FirstOrDefault().RoleId 
                    : applicationRolesWithPermissions.Where(x => x.Name == UserApiRoles.ApplicationReadOnly).FirstOrDefault().RoleId;

                await AssignRoleToUser(applicationId, applicationUserId, roleId);
            }

            if (req.ApplicationUserAdmin || req.ApplicationUserReadOnly)
            {
                var applicationUserRolesWithPermissions = await CreateDefaultApplicationUserRolesWithPermissions(applicationId);
                var roleId = req.ApplicationUserAdmin ? applicationUserRolesWithPermissions.Where(x => x.Name == UserApiRoles.ApplicationUserAdmin).FirstOrDefault().RoleId 
                    : applicationUserRolesWithPermissions.Where(x => x.Name == UserApiRoles.ApplicationUserReadOnly).FirstOrDefault().RoleId;

                await AssignRoleToUser(applicationId, applicationUserId, roleId);
            }

            if (req.ApplicationUserPermissionAdmin || req.ApplicationUserPermissionReadOnly)
            {
                var applicationUserPermissionRolesWithPermissions = await CreateDefaultApplicationUserPermissionRolesWithPermissions(applicationId);
                var roleId = req.ApplicationUserPermissionAdmin ? applicationUserPermissionRolesWithPermissions.Where(x => x.Name == UserApiRoles.ApplicationUserPermissionAdmin).FirstOrDefault().RoleId 
                    : applicationUserPermissionRolesWithPermissions.Where(x => x.Name == UserApiRoles.ApplicationUserPermissionReadOnly).FirstOrDefault().RoleId;

                await AssignRoleToUser(applicationId, applicationUserId, roleId);
            }

            if (req.ApplicationUserRoleAdmin || req.ApplicationUserRoleReadOnly)
            {
                var applicationUserRoleRolesWithPermissions = await CreateDefaultApplicationUserRoleRolesWithPermissions(applicationId);
                var roleId = req.ApplicationUserRoleAdmin ? applicationUserRoleRolesWithPermissions.Where(x => x.Name == UserApiRoles.ApplicationUserRoleAdmin).FirstOrDefault().RoleId 
                    : applicationUserRoleRolesWithPermissions.Where(x => x.Name == UserApiRoles.ApplicationUserRoleReadOnly).FirstOrDefault().RoleId;

                await AssignRoleToUser(applicationId, applicationUserId, roleId);
            }

            if (req.PermissionAdmin || req.PermissionReadOnly)
            {
                var permissionRolesWithPermissions = await CreateDefaultPermissionRolesWithPermissions(applicationId);
                var roleId = req.PermissionAdmin ? permissionRolesWithPermissions.Where(x => x.Name == UserApiRoles.PermissionAdmin).FirstOrDefault().RoleId 
                    : permissionRolesWithPermissions.Where(x => x.Name == UserApiRoles.PermissionReadOnly).FirstOrDefault().RoleId;

                await AssignRoleToUser(applicationId, applicationUserId, roleId);
            }

            if (req.RoleAdmin || req.RoleReadOnly)
            {
                var roleRolesWithPermissions = await CreateDefaultRoleRolesWithPermissions(applicationId);
                var roleId = req.RoleAdmin ? roleRolesWithPermissions.Where(x => x.Name == UserApiRoles.RoleAdmin).FirstOrDefault().RoleId 
                    : roleRolesWithPermissions.Where(x => x.Name == UserApiRoles.RoleReadOnly).FirstOrDefault().RoleId;

                await AssignRoleToUser(applicationId, applicationUserId, roleId);
            }

            if (req.RolePermissionAdmin || req.RolePermissionReadOnly)
            {
                var roleRolesWithPermissions = await CreateDefaultRolePermissionRolesWithPermissions(applicationId);
                var roleId = req.RolePermissionAdmin ? roleRolesWithPermissions.Where(x => x.Name == UserApiRoles.RolePermissionAdmin).FirstOrDefault().RoleId 
                    : roleRolesWithPermissions.Where(x => x.Name == UserApiRoles.RolePermissionReadOnly).FirstOrDefault().RoleId;

                await AssignRoleToUser(applicationId, applicationUserId, roleId);
            }

            var ret = await _applicationUserLogic.GetById(testUser.Response.ApplicationUserId, new BaseLogicGet { IncludeRelated = true });

            return ret.Response;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private async Task<List<ApplicationUserPermissionDto>> CreateDefaultApplicationUserPermissionsForTestUser(int applicationId, int applicationUserId)
    {
        var testPermissions = new List<InsertUpdatePermissionRequest>();
        testPermissions.Add(new InsertUpdatePermissionRequest { 
            Active = true, 
            ApplicationId = applicationId, 
            Name = "Test Permission 1", 
            Description = "Test Permission 1 for Test User",
            CurrentUser = TestConstants.CurrentUser
        });

        testPermissions.Add(new InsertUpdatePermissionRequest { 
            Active = true, 
            ApplicationId = applicationId, 
            Name = "Test Permission 2", 
            Description = "Test Permission 2 for Test User",
            CurrentUser = TestConstants.CurrentUser
        });

        testPermissions.Add(new InsertUpdatePermissionRequest { 
            Active = true, 
            ApplicationId = applicationId, 
            Name = "Test Permission 3", 
            Description = "Test Permission 3 for Test User",
            CurrentUser = TestConstants.CurrentUser
        });

        testPermissions.Add(new InsertUpdatePermissionRequest { 
            Active = true, 
            ApplicationId = applicationId, 
            Name = "Test Permission 4", 
            Description = "Test Permission 4 for Test User",
            CurrentUser = TestConstants.CurrentUser
        });

        testPermissions.Add(new InsertUpdatePermissionRequest { 
            Active = true, 
            ApplicationId = applicationId, 
            Name = "Test Permission 5", 
            Description = "Test Permission 5 for Test User",
            CurrentUser = TestConstants.CurrentUser
        });

        var createdPermissions = await CreatePermissions(testPermissions);

        var createdApplicationUserPermissions = new List<ApplicationUserPermissionDto>();
        
        foreach (var permission in createdPermissions)
        {
            var req = new InsertUpdateApplicationUserPermissionRequest { 
                Active = true, 
                ApplicationId = applicationId, 
                ApplicationUserId = applicationUserId, 
                PermissionId = permission.PermissionId,
                CurrentUser = TestConstants.CurrentUser
            };

            var aup = await _applicationUserPermissionLogic.Insert(req, _applicationLogic, _applicationUserLogic, _permissionLogic);
            createdApplicationUserPermissions.Add(aup.Response);
        }
        
        return createdApplicationUserPermissions;

    }

    private async Task<List<RoleDto>> CreateDefaultApplicationRolesWithPermissions(int applicationId)
    {
        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();
        
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationRead, Description = "Allows for retrieving all Application data in a read only state" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationInsert, Description = "Allows for creating new Application data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUpdate, Description = "Allows for updating existing Application data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationDelete, Description = "Allows for deleting Application data" });

        var createdPermissions = await CreatePermissions(permissionsToCreate);

        var adminRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.ApplicationAdmin, Description = "Full Access to all Application Functionality." });
        var readOnlyRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.ApplicationReadOnly, Description = "ReadOnly Access to Application Functionality." });
        var readOnlyPermissions = createdPermissions.Where(x => x.Name == UserApiPermissions.ApplicationRead).ToList();

        // create admin role permissions
        await CreateRolePermissions(applicationId, createdPermissions, adminRole.RoleId);

        // create readonly role permissions
        await CreateRolePermissions(applicationId, readOnlyPermissions, readOnlyRole.RoleId);
        
        var ret = await _roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { adminRole.RoleId, readOnlyRole.RoleId }, IncludeRelated = true });
        return ret.Response.ToList();
    }

    private async Task<List<RoleDto>> CreateDefaultApplicationUserRolesWithPermissions(int applicationId)
    {
        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();
        
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserRead, Description = "Allows for retrieving all ApplicationUser data in a read only state" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserInsert, Description = "Allows for creating new ApplicationUser data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserUpdate, Description = "Allows for updating existing ApplicationUser data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserDelete, Description = "Allows for deleting ApplicationUser data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserResetPassword, Description = "Allows for resetting ApplicationUser password" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserChangePassword, Description = "Allows for changing ApplicationUser password" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserPasswordChangeHistoryRead, Description = "Allows for reading ApplicationUser password change history" });

        var createdPermissions = await CreatePermissions(permissionsToCreate);

        var adminRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.ApplicationUserAdmin, Description = "Full Access to all ApplicationUser Functionality." });
        var readOnlyRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.ApplicationUserReadOnly, Description = "ReadOnly Access to all ApplicationUser Functionality." });
        var readOnlyPermissions = createdPermissions.Where(x => x.Name == UserApiPermissions.ApplicationUserRead).ToList();

        // create admin role permissions
        await CreateRolePermissions(applicationId, createdPermissions, adminRole.RoleId);

        // create readonly role permissions
        await CreateRolePermissions(applicationId, readOnlyPermissions, readOnlyRole.RoleId);
        
        var ret = await _roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { adminRole.RoleId, readOnlyRole.RoleId }, IncludeRelated = true });
        return ret.Response.ToList();
    }

    private async Task<List<RoleDto>> CreateDefaultApplicationUserPermissionRolesWithPermissions(int applicationId)
    {
        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();
        
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserPermissionRead, Description = "Allows for retrieving all ApplicationUserPermission data in a read only state" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserPermissionInsert, Description = "Allows for creating new ApplicationUserPermission data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserPermissionUpdate, Description = "Allows for updating existing ApplicationUserPermission data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserPermissionDelete, Description = "Allows for deleting ApplicationUserPermission data" });

        var createdPermissions = await CreatePermissions(permissionsToCreate);

        var adminRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.ApplicationUserPermissionAdmin, Description = "Full Access to all ApplicationUserPermission Functionality." });
        var readOnlyRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.ApplicationUserPermissionReadOnly, Description = "ReadOnly Access to all ApplicationUserPermission Functionality." });
        var readOnlyPermissions = createdPermissions.Where(x => x.Name == UserApiPermissions.ApplicationUserPermissionRead).ToList();

        // create admin role permissions
        await CreateRolePermissions(applicationId, createdPermissions, adminRole.RoleId);

        // create readonly role permissions
        await CreateRolePermissions(applicationId, readOnlyPermissions, readOnlyRole.RoleId);
        
        var ret = await _roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { adminRole.RoleId, readOnlyRole.RoleId }, IncludeRelated = true });
        return ret.Response.ToList();
    }

    private async Task<List<RoleDto>> CreateDefaultApplicationUserRoleRolesWithPermissions(int applicationId)
    {
        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();
        
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserRoleRead, Description = "Allows for retrieving all ApplicationUserRole data in a read only state" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserRoleInsert, Description = "Allows for creating new ApplicationUserRole data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserRoleUpdate, Description = "Allows for updating existing ApplicationUserRole data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.ApplicationUserRoleDelete, Description = "Allows for deleting ApplicationUserRole data" });

        var createdPermissions = await CreatePermissions(permissionsToCreate);

        var adminRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.ApplicationUserRoleAdmin, Description = "Full Access to all ApplicationUserRole Functionality." });
        var readOnlyRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.ApplicationUserRoleReadOnly, Description = "ReadOnly Access to all ApplicationUserRole Functionality." });
        var readOnlyPermissions = createdPermissions.Where(x => x.Name == UserApiPermissions.ApplicationUserRoleRead).ToList();

        // create admin role permissions
        await CreateRolePermissions(applicationId, createdPermissions, adminRole.RoleId);

        // create readonly role permissions
        await CreateRolePermissions(applicationId, readOnlyPermissions, readOnlyRole.RoleId);
        
        var ret = await _roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { adminRole.RoleId, readOnlyRole.RoleId }, IncludeRelated = true });
        return ret.Response.ToList();
    }

    private async Task<List<RoleDto>> CreateDefaultPermissionRolesWithPermissions(int applicationId)
    {
        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();
        
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.PermissionRead, Description = "Allows for retrieving all Permission data in a read only state" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.PermissionInsert, Description = "Allows for creating new Permission data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.PermissionUpdate, Description = "Allows for updating existing Permission data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.PermissionDelete, Description = "Allows for deleting Permission data" });

        var createdPermissions = await CreatePermissions(permissionsToCreate);

        var adminRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.PermissionAdmin, Description = "Full Access to all Permission Functionality." });
        var readOnlyRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.PermissionReadOnly, Description = "ReadOnly Access to all Permission Functionality." });
        var readOnlyPermissions = createdPermissions.Where(x => x.Name == UserApiPermissions.PermissionRead).ToList();

        // create admin role permissions
        await CreateRolePermissions(applicationId, createdPermissions, adminRole.RoleId);

        // create readonly role permissions
        await CreateRolePermissions(applicationId, readOnlyPermissions, readOnlyRole.RoleId);
        
        var ret = await _roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { adminRole.RoleId, readOnlyRole.RoleId }, IncludeRelated = true });
        return ret.Response.ToList();
    }

    private async Task<List<RoleDto>> CreateDefaultRoleRolesWithPermissions(int applicationId)
    {
        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();
        
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.RoleRead, Description = "Allows for retrieving all Role data in a read only state" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.RoleInsert, Description = "Allows for creating new Role data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.RoleUpdate, Description = "Allows for updating existing Role data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.RoleDelete, Description = "Allows for deleting Role data" });

        var createdPermissions = await CreatePermissions(permissionsToCreate);

        var adminRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.RoleAdmin, Description = "Full Access to all Role Functionality." });
        var readOnlyRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.RoleReadOnly, Description = "ReadOnly Access to all Role Functionality." });
        var readOnlyPermissions = createdPermissions.Where(x => x.Name == UserApiPermissions.RoleRead).ToList();

        // create admin role permissions
        await CreateRolePermissions(applicationId, createdPermissions, adminRole.RoleId);

        // create readonly role permissions
        await CreateRolePermissions(applicationId, readOnlyPermissions, readOnlyRole.RoleId);
        
        var ret = await _roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { adminRole.RoleId, readOnlyRole.RoleId }, IncludeRelated = true });
        return ret.Response.ToList();
    }

    private async Task<List<RoleDto>> CreateDefaultRolePermissionRolesWithPermissions(int applicationId)
    {
        var permissionsToCreate = new List<InsertUpdatePermissionRequest>();
        
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.RolePermissionRead, Description = "Allows for retrieving all RolePermission data in a read only state" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.RolePermissionInsert, Description = "Allows for creating new RolePermission data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.RolePermissionUpdate, Description = "Allows for updating existing RolePermission data" });
        permissionsToCreate.Add(new InsertUpdatePermissionRequest { Active = true, ApplicationId = applicationId, Name = UserApiPermissions.RolePermissionDelete, Description = "Allows for deleting RolePermission data" });

        var createdPermissions = await CreatePermissions(permissionsToCreate);

        var adminRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.RolePermissionAdmin, Description = "Full Access to all RolePermission Functionality." });
        var readOnlyRole = await CreateRole(new InsertUpdateRoleRequest { Active = true, ApplicationId = applicationId, Name = UserApiRoles.RolePermissionReadOnly, Description = "ReadOnly Access to all RolePermission Functionality." });
        var readOnlyPermissions = createdPermissions.Where(x => x.Name == UserApiPermissions.RolePermissionRead).ToList();

        // create admin role permissions
        await CreateRolePermissions(applicationId, createdPermissions, adminRole.RoleId);

        // create readonly role permissions
        await CreateRolePermissions(applicationId, readOnlyPermissions, readOnlyRole.RoleId);
        
        var ret = await _roleLogic.Filter(new FilterRoleLogicRequest { RoleIds = new List<int> { adminRole.RoleId, readOnlyRole.RoleId }, IncludeRelated = true });
        return ret.Response.ToList();
    }

    protected async Task<SecurityTestData> ArrangeApplicationTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        ret.ActiveApplications = await _securityTestUtilities.Application.CreateActiveTestRecords(1);
        ret.InactiveApplications = await _securityTestUtilities.Application.CreateInactiveTestRecords(1);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeReadOnlyApplicationTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        ret.ActiveApplications = await _securityTestUtilities.Application.CreateActiveReadOnlyTestRecords(1);
        ret.InactiveApplications = await _securityTestUtilities.Application.CreateInactiveReadOnlyTestRecords(1);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeApplicationTestDataWithRelatedData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        ret.ActiveApplications = await _securityTestUtilities.Application.CreateActiveTestRecords(1);
        ret.InactiveApplications = await _securityTestUtilities.Application.CreateInactiveTestRecords(1);

        foreach (var activeApplication in ret.ActiveApplications)
        {
            ret.ActiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(activeApplication.ApplicationId, 1);
            ret.ActivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(activeApplication.ApplicationId);
            ret.ActiveRoles = await _securityTestUtilities.Role.CreateActiveTestRecords(activeApplication.ApplicationId, 1);

            foreach (var activeRole in ret.ActiveRoles)
            {
                foreach (var activePermission in ret.ActivePermissions)
                {
                    ret.ActiveRolePermissions.Add(await CreateRolePermission(new InsertUpdateRolePermissionRequest { Active = true, ApplicationId = activeApplication.ApplicationId, RoleId = activeRole.RoleId, PermissionId = activePermission.PermissionId }));
                }
            }

            var activePermissionsForApplicationUser = await _securityTestUtilities.Permission.CreateActiveTestRecords(activeApplication.ApplicationId, 1);
            foreach (var activePermission in activePermissionsForApplicationUser)
            {
                foreach (var activeApplicationUser in ret.ActiveApplicationUsers)
                {
                    ret.ActiveApplicationUserPermissions.Add(await CreateApplicationUserPermission(new InsertUpdateApplicationUserPermissionRequest { Active = true, ApplicationId = activeApplication.ApplicationId, ApplicationUserId = activeApplicationUser.ApplicationUserId, PermissionId = activePermission.PermissionId }));
                }

                ret.ActivePermissions.Add(activePermission);
            }
        }

        foreach (var inactiveApplication in ret.InactiveApplications)
        {
            ret.InactiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(inactiveApplication.ApplicationId, 1);
            ret.InactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(inactiveApplication.ApplicationId, 1);
            ret.InactiveRoles = await _securityTestUtilities.Role.CreateInactiveTestRecords(inactiveApplication.ApplicationId, 1);

            foreach (var inactiveRole in ret.InactiveRoles)
            {
                foreach (var inactivePermission in ret.InactivePermissions)
                {
                    ret.InactiveRolePermissions.Add(await CreateRolePermission(new InsertUpdateRolePermissionRequest { Active = false, ApplicationId = inactiveApplication.ApplicationId, RoleId = inactiveRole.RoleId, PermissionId = inactivePermission.PermissionId }));
                }
            }

            var inactivePermissionsForApplicationUser = await _securityTestUtilities.Permission.CreateInactiveTestRecords(inactiveApplication.ApplicationId, 1);
            foreach (var inactivePermission in inactivePermissionsForApplicationUser)
            {
                foreach (var inactiveApplicationUser in ret.InactiveApplicationUsers)
                {
                    ret.InactiveApplicationUserPermissions.Add(await CreateApplicationUserPermission(new InsertUpdateApplicationUserPermissionRequest { Active = false, ApplicationId = inactiveApplication.ApplicationId, ApplicationUserId = inactiveApplicationUser.ApplicationUserId, PermissionId = inactivePermission.PermissionId }));
                }

                ret.InactivePermissions.Add(inactivePermission);
            }
        }

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeUserTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        ret.ActiveUsers = await _securityTestUtilities.User.CreateActiveTestRecords(1);
        ret.InactiveUsers = await _securityTestUtilities.User.CreateInactiveTestRecords(1);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeReadOnlyUserTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        ret.ActiveUsers = await _securityTestUtilities.User.CreateActiveReadOnlyTestRecords(1);
        ret.InactiveUsers = await _securityTestUtilities.User.CreateInactiveReadOnlyTestRecords(1);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeApplicationUserTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

        ret.ActiveApplications.Add(application);

        ret.ActiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1);
        ret.InactiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(application.ApplicationId, 1);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeReadOnlyApplicationUserTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        ret.ActiveApplications.Add(application);

        ret.ActiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
        ret.InactiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

        return ret;
    }

    protected async Task<ErrorValidationResult> ArrangeApplicationUserPasswordChangeHistoryTestData(int applicationUserId)
    {
        var passwordChangeResponse = await _applicationUserLogic.ChangePassword(new ChangePasswordRequest { ApplicationUserId = applicationUserId, NewPassword = TestConstants.DefaultTestUserPassword + "1", CurrentUser = TestConstants.CurrentUser });
        return passwordChangeResponse;
    }

    protected async Task<SecurityTestData> ArrangeApplicationUserTestDataWithRelatedData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        
        ret.ActiveApplications.Add(application);

        ret.ActiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId, 1);
        ret.InactiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(application.ApplicationId, 1);

        ret.ActivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1);
        ret.InactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, 1);

        ret.ActiveRoles = await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1);
        ret.InactiveRoles = await _securityTestUtilities.Role.CreateInactiveTestRecords(application.ApplicationId, 1);

        //create role and user permissions with active permissions
        foreach (var activePermission in ret.ActivePermissions)
        {
            foreach (var role in ret.ActiveRoles)
            {
                ret.ActiveRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, activePermission.PermissionId));
            }

            ret.ActiveApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, ret.ActiveApplicationUsers.FirstOrDefault().ApplicationUserId, activePermission.PermissionId));
        }

        //create role and user permissions with inactive permissions
        foreach (var inactivePermission in ret.InactivePermissions)
        {
            foreach (var role in ret.InactiveRoles)
            {
                ret.InactiveRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, inactivePermission.PermissionId, false));
            }

            ret.InactiveApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, ret.InactiveApplicationUsers.FirstOrDefault().ApplicationUserId, inactivePermission.PermissionId, false));
        }

        //attach active role(s) to active user(s)
        foreach (var activeRole in ret.ActiveRoles)
        {
            ret.ActiveApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, ret.ActiveApplicationUsers.FirstOrDefault().ApplicationUserId, activeRole.RoleId));
        }

        //attach inactive role(s) to inactive user(s)
        foreach (var inactiveRole in ret.InactiveRoles)
        {
            ret.InactiveApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, ret.InactiveApplicationUsers.FirstOrDefault().ApplicationUserId, inactiveRole.RoleId, false));
        }

        return ret;
    }

    protected async Task<SecurityTestData> ArrangePermissionTestData(short numberOfActivePermissionsToCreate = 5, short numberOfInactivePermissionsToCreate = 5)
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

        ret.ActiveApplications.Add(application);

        ret.ActivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, numberOfActivePermissionsToCreate);
        ret.InactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, numberOfInactivePermissionsToCreate);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeReadOnlyPermissionTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        ret.ActiveApplications.Add(application);

        ret.ActivePermissions = await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
        ret.InactivePermissions = await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

        return ret;
    }


    protected async Task<SecurityTestData> ArrangeRoleTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        ret.ActiveApplications.Add(application);

        ret.ActiveRoles = await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1);
        ret.InactiveRoles = await _securityTestUtilities.Role.CreateInactiveTestRecords(application.ApplicationId, 1);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeReadOnlyRoleTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        ret.ActiveApplications.Add(application);

        ret.ActiveRoles = await _securityTestUtilities.Role.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
        ret.InactiveRoles = await _securityTestUtilities.Role.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeRoleTestDataWithRelatedData()
    {
        var ret = await ArrangeRoleTestData();

        var application = ret.ActiveApplications.FirstOrDefault();

        var activeRolePermissions = new List<RolePermissionDto>();
        var inactiveRolePermissions = new List<RolePermissionDto>();

        var activePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1);
        var inactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, 1);
        
        foreach (var activeRole in ret.ActiveRoles)
        {
            //create active RolePermission record(s)
            foreach (var activePermission in activePermissions) 
            {
                activeRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, activeRole.RoleId, activePermission.PermissionId));
            }

            //create inactive RolePermission record(s)
            foreach (var inactivePermission in inactivePermissions) 
            {
                inactiveRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, activeRole.RoleId, inactivePermission.PermissionId, false));
            }
        }

        foreach (var inactiveRole in ret.InactiveRoles)
        {
            //create inactive RolePermission record(s)
            foreach (var inactivePermission in inactivePermissions) 
            {
                inactiveRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, inactiveRole.RoleId, inactivePermission.PermissionId, false));
            }
        }
        
        ret.ActiveRolePermissions = activeRolePermissions;
        ret.InactiveRolePermissions = inactiveRolePermissions;

        ret.ActivePermissions = activePermissions;
        ret.InactivePermissions = inactivePermissions;

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeApplicationUserPermissionTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        ret.ActiveApplications.Add(application);
        
        var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
        var activePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1);
        var inactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, 1);

        var activeApplicationUserPermissions = new List<ApplicationUserPermissionDto>();
        var inactiveApplicationUserPermissions = new List<ApplicationUserPermissionDto>();

        //create active ApplicationUserPermission records
        foreach (var activePermission in activePermissions) 
        {
            activeApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activePermission.PermissionId));
        }

        //create inactive ApplicationUserPermission records
        foreach (var inactivePermission in inactivePermissions) 
        {
            inactiveApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, inactivePermission.PermissionId, false));
        }

        ret.ActiveApplicationUserPermissions = activeApplicationUserPermissions;
        ret.InactiveApplicationUserPermissions = inactiveApplicationUserPermissions;

        ret.ActivePermissions = activePermissions;
        ret.InactivePermissions = inactivePermissions;

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeReadOnlyApplicationUserPermissionTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = (await _securityTestUtilities.Application.CreateActiveReadOnlyTestRecords(1)).First();
        ret.ActiveApplications.Add(application);
        
        var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecords(application.ApplicationId)).First();
        var activePermissions = await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
        var inactivePermissions = await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

        var activeApplicationUserPermissions = new List<ApplicationUserPermissionDto>();
        var inactiveApplicationUserPermissions = new List<ApplicationUserPermissionDto>();

        //create active ApplicationUserPermission records
        foreach (var activePermission in activePermissions) 
        {
            activeApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activePermission.PermissionId));
        }

        //create inactive ApplicationUserPermission records
        foreach (var inactivePermission in inactivePermissions) 
        {
            inactiveApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateInactiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, inactivePermission.PermissionId));
        }

        ret.ActiveApplicationUserPermissions = activeApplicationUserPermissions;
        ret.InactiveApplicationUserPermissions = inactiveApplicationUserPermissions;

        ret.ActivePermissions = activePermissions;
        ret.InactivePermissions = inactivePermissions;

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeApplicationUserRoleTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();

        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        ret.ActiveApplications.Add(application);
        
        var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
        var activeRoles = await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId, 1);
        var inactiveRoles = await _securityTestUtilities.Role.CreateInactiveTestRecords(application.ApplicationId, 1);

        var activeApplicationUserRoles = new List<ApplicationUserRoleDto>();
        var inactiveApplicationUserRoles = new List<ApplicationUserRoleDto>();

        //create active ApplicationUserRole records
        foreach (var activeRole in activeRoles) 
        {
            activeApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activeRole.RoleId));
        }

        //create inactive ApplicationUserRole records
        foreach (var inactiveRole in inactiveRoles) 
        {
            inactiveApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, inactiveRole.RoleId, false));
        }

        ret.ActiveApplicationUserRoles = activeApplicationUserRoles;
        ret.InactiveApplicationUserRoles = inactiveApplicationUserRoles;

        ret.ActiveRoles = activeRoles;
        ret.InactiveRoles = inactiveRoles;

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeReadOnlyApplicationUserRoleTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();

        var application = (await _securityTestUtilities.Application.CreateActiveReadOnlyTestRecords(1)).First();
        ret.ActiveApplications.Add(application);
        
        var applicationUser = (await _securityTestUtilities.ApplicationUser.CreateActiveReadOnlyTestRecords(application.ApplicationId)).First();
        var activeRoles = await _securityTestUtilities.Role.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
        var inactiveRoles = await _securityTestUtilities.Role.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

        var activeApplicationUserRoles = new List<ApplicationUserRoleDto>();
        var inactiveApplicationUserRoles = new List<ApplicationUserRoleDto>();

        //create active ApplicationUserRole records
        foreach (var activeRole in activeRoles) 
        {
            activeApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateActiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activeRole.RoleId));
        }

        //create inactive ApplicationUserRole records
        foreach (var inactiveRole in inactiveRoles) 
        {
            inactiveApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateInactiveReadOnlyTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, inactiveRole.RoleId));
        }

        ret.ActiveApplicationUserRoles = activeApplicationUserRoles;
        ret.InactiveApplicationUserRoles = inactiveApplicationUserRoles;

        ret.ActiveRoles = activeRoles;
        ret.InactiveRoles = inactiveRoles;

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeRolePermissionTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        ret.ActiveApplications.Add(application);
        
        var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
        var activePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId, 1);
        var inactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId, 1);

        var activeRolePermissions = new List<RolePermissionDto>();
        var inactiveRolePermissions = new List<RolePermissionDto>();

        //create 5 active RolePermission records
        foreach (var activePermission in activePermissions) 
        {
            activeRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, activePermission.PermissionId));
        }

        //create 5 inactive RolePermission records
        foreach (var inactivePermission in inactivePermissions) 
        {
            inactiveRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, role.RoleId, inactivePermission.PermissionId, false));
        }

        ret.ActiveRolePermissions = activeRolePermissions;
        ret.InactiveRolePermissions = inactiveRolePermissions;

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeReadOnlyRolePermissionTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = (await _securityTestUtilities.Application.CreateActiveReadOnlyTestRecords(1)).First();
        ret.ActiveApplications.Add(application);
        
        var role = (await _securityTestUtilities.Role.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1)).First();
        var activePermissions = await _securityTestUtilities.Permission.CreateActiveReadOnlyTestRecords(application.ApplicationId, 1);
        var inactivePermissions = await _securityTestUtilities.Permission.CreateInactiveReadOnlyTestRecords(application.ApplicationId, 1);

        var activeRolePermissions = new List<RolePermissionDto>();
        var inactiveRolePermissions = new List<RolePermissionDto>();

        //create 5 active RolePermission records
        foreach (var activePermission in activePermissions) 
        {
            activeRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateActiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, activePermission.PermissionId));
        }

        //create 5 inactive RolePermission records
        foreach (var inactivePermission in inactivePermissions) 
        {
            inactiveRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateInactiveReadOnlyTestRecord(application.ApplicationId, role.RoleId, inactivePermission.PermissionId));
        }

        ret.ActiveRolePermissions = activeRolePermissions;
        ret.InactiveRolePermissions = inactiveRolePermissions;

        return ret;
    }
    
    protected async Task<SecurityTestData> ArrangeSecurityTestData()
    {
        var securityTestDataRet = new SecurityTestData();

        await ClearAllSecurityTestTableData();

        //create test applications
        var activeApplications = await _securityTestUtilities.Application.CreateActiveTestRecords();
        var inactiveApplications = await _securityTestUtilities.Application.CreateInactiveTestRecords();
        
        var activeApplicationUsers = new List<ApplicationUserDto>();
        var inactiveApplicationUsers = new List<ApplicationUserDto>();

        var activePermissions = new List<PermissionDto>();
        var inactivePermissions = new List<PermissionDto>();

        var activeRoles = new List<RoleDto>();
        var inactiveRoles = new List<RoleDto>();
        
        var activeApplicationUserPermissions = new List<ApplicationUserPermissionDto>();
        var inactiveApplicationUserPermissions = new List<ApplicationUserPermissionDto>();

        var activeApplicationUserRoles = new List<ApplicationUserRoleDto>();
        var inactiveApplicationUserRoles = new List<ApplicationUserRoleDto>();

        var activeRolePermissions = new List<RolePermissionDto>();
        var inactiveRolePermissions = new List<RolePermissionDto>();

        foreach (var activeApplication in activeApplications)
        {
            //create test active application users
            var activeApplicationUserRes = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(activeApplication.ApplicationId);
            activeApplicationUserRes.ForEach(r => activeApplicationUsers.Add(r));

            //create test inactive application users
            var inactiveApplicationUserRes = await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(activeApplication.ApplicationId);
            inactiveApplicationUserRes.ForEach(r => inactiveApplicationUsers.Add(r));

            //create test active permissions
            var activePermissionRes = await _securityTestUtilities.Permission.CreateActiveTestRecords(activeApplication.ApplicationId);
            activePermissionRes.ForEach(r => activePermissions.Add(r));

            //create test inactive permissions
            var inactivePermissionRes = await _securityTestUtilities.Permission.CreateInactiveTestRecords(activeApplication.ApplicationId);
            inactivePermissionRes.ForEach(r => inactivePermissions.Add(r));

            //create test active roles
            var activeRoleRes = await _securityTestUtilities.Role.CreateActiveTestRecords(activeApplication.ApplicationId);
            activeRoleRes.ForEach(r => activeRoles.Add(r));

            //create test inactive roles
            var inactiveRoleRes = await _securityTestUtilities.Role.CreateInactiveTestRecords(activeApplication.ApplicationId);
            inactiveRoleRes.ForEach(r => inactiveRoles.Add(r));

            //create test active application user permissions
            foreach (var activePermission in activePermissionRes)
            {
                foreach (var activeApplicationUser in activeApplicationUserRes)
                {
                    activeApplicationUserPermissions.AddRange(await _securityTestUtilities.ApplicationUserPermission.CreateActiveTestRecords(activeApplication.ApplicationId, activeApplicationUser.ApplicationUserId, activePermission.PermissionId, 1));
                }
            }

            //create test inactive application user permissions
            foreach (var inactivePermission in inactivePermissionRes)
            {
                foreach (var inactiveApplicationUser in inactiveApplicationUserRes)
                {
                    inactiveApplicationUserPermissions.AddRange(await _securityTestUtilities.ApplicationUserPermission.CreateInactiveTestRecords(activeApplication.ApplicationId, inactiveApplicationUser.ApplicationUserId, inactivePermission.PermissionId, 1));
                }
            }

            //create test active application user roles
            foreach (var activeRole in activeRoleRes)
            {
                foreach (var activeApplicationUser in activeApplicationUserRes)
                {
                    activeApplicationUserRoles.AddRange(await _securityTestUtilities.ApplicationUserRole.CreateActiveTestRecords(activeApplication.ApplicationId, activeApplicationUser.ApplicationUserId, activeRole.RoleId, 1));
                }
            }

            //create test inactive application user roles
            foreach (var inactiveRole in inactiveRoleRes)
            {
                foreach (var inactiveApplicationUser in inactiveApplicationUserRes)
                {
                    inactiveApplicationUserRoles.AddRange(await _securityTestUtilities.ApplicationUserRole.CreateInactiveTestRecords(activeApplication.ApplicationId, inactiveApplicationUser.ApplicationUserId, inactiveRole.RoleId, 1));
                }
            }

            //create test active role permissions
            foreach (var activeRole in activeRoleRes)
            {
                foreach (var activePermission in activePermissionRes)
                {
                    activeRolePermissions.AddRange(await _securityTestUtilities.RolePermission.CreateActiveTestRecords(activeApplication.ApplicationId, activeRole.RoleId, activePermission.PermissionId, 1));
                }
            }

            //create test inactive role permissions
            foreach (var inactiveRole in inactiveRoleRes)
            {
                foreach (var inactivePermission in inactivePermissionRes)
                {
                    inactiveRolePermissions.AddRange(await _securityTestUtilities.RolePermission.CreateInactiveTestRecords(activeApplication.ApplicationId, inactiveRole.RoleId, inactivePermission.PermissionId, 1));
                }
            }
        }

        foreach (var inactiveApplication in inactiveApplications)
        {
            //create test inactive application users
            var inactiveApplicationUserRes = await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(inactiveApplication.ApplicationId);
            inactiveApplicationUserRes.ForEach(r => inactiveApplicationUsers.Add(r));

            //create test inactive permissions
            var inactivePermissionRes = await _securityTestUtilities.Permission.CreateInactiveTestRecords(inactiveApplication.ApplicationId);
            inactivePermissionRes.ForEach(r => inactivePermissions.Add(r));

            //create test inactive roles
            var inactiveRoleRes = await _securityTestUtilities.Role.CreateInactiveTestRecords(inactiveApplication.ApplicationId);
            inactiveRoleRes.ForEach(r => inactiveRoles.Add(r));

            //create test inactive application user permissions
            foreach (var inactivePermission in inactivePermissionRes)
            {
                foreach (var inactiveApplicationUser in inactiveApplicationUserRes)
                {
                    inactiveApplicationUserPermissions.AddRange(await _securityTestUtilities.ApplicationUserPermission.CreateInactiveTestRecords(inactiveApplication.ApplicationId, inactiveApplicationUser.ApplicationUserId, inactivePermission.PermissionId, 1));
                }
            }

            //create test inactive application user roles
            foreach (var inactiveRole in inactiveRoleRes)
            {
                foreach (var inactiveApplicationUser in inactiveApplicationUserRes)
                {
                    inactiveApplicationUserRoles.AddRange(await _securityTestUtilities.ApplicationUserRole.CreateInactiveTestRecords(inactiveApplication.ApplicationId, inactiveApplicationUser.ApplicationUserId, inactiveRole.RoleId, 1));
                }
            }

            //create test inactive role permissions
            foreach (var inactiveRole in inactiveRoleRes)
            {
                foreach (var inactivePermission in inactivePermissionRes)
                {
                    inactiveRolePermissions.AddRange(await _securityTestUtilities.RolePermission.CreateInactiveTestRecords(inactiveApplication.ApplicationId, inactiveRole.RoleId, inactivePermission.PermissionId, 1));
                }
            }
        }
        
        securityTestDataRet.ActiveApplications = activeApplications;
        securityTestDataRet.InactiveApplications = inactiveApplications;
        securityTestDataRet.ActiveApplicationUsers = activeApplicationUsers;
        securityTestDataRet.InactiveApplicationUsers = inactiveApplicationUsers;
        securityTestDataRet.ActivePermissions = activePermissions;
        securityTestDataRet.InactivePermissions = inactivePermissions;
        securityTestDataRet.ActiveRoles = activeRoles;
        securityTestDataRet.InactiveRoles = inactiveRoles;
        securityTestDataRet.ActiveApplicationUserPermissions = activeApplicationUserPermissions;
        securityTestDataRet.InactiveApplicationUserPermissions = inactiveApplicationUserPermissions;
        securityTestDataRet.ActiveApplicationUserRoles = activeApplicationUserRoles;
        securityTestDataRet.InactiveApplicationUserRoles = inactiveApplicationUserRoles;
        securityTestDataRet.ActiveRolePermissions = activeRolePermissions;
        securityTestDataRet.InactiveRolePermissions = inactiveRolePermissions;

        return securityTestDataRet;
    }

    private ServiceProvider ConfigureServices() 
    {
        var services = new ServiceCollection();

        services = ConfigureBaseDependencies(services);
        services = ConfigureSecurityService(services);

        return services.BuildServiceProvider();
    }

    private ServiceCollection ConfigureBaseDependencies(ServiceCollection services)
    {
        // ICacheService Setup
        new RedisTestUtilities().ConfigureCache(services);
        
        services.AddTransient<ICacheTestUtilities, RedisTestUtilities>();

        //configure logger service
        //services.AddSingleton<ILoggerService, LoggerServiceTestStub>();

        //unit testing dependencies
        services.AddTransient<ISecurityTestUtilitiesManager, SecurityTestUtilitiesManager>();
        services.AddTransient<IAuthenticationUtilities, AuthenticationUtilities>();
        services.AddTransient<IApplicationUtilities, ApplicationUtilities>();
        services.AddTransient<IUserUtilities, UserUtilities>();
        services.AddTransient<IApplicationUserUtilities, ApplicationUserUtilities>();
        services.AddTransient<IRoleUtilities, RoleUtilities>();
        services.AddTransient<IPermissionUtilities, PermissionUtilities>();
        services.AddTransient<IApplicationUserPermissionUtilities, ApplicationUserPermissionUtilities>();
        services.AddTransient<IApplicationUserRoleUtilities, ApplicationUserRoleUtilities>();
        services.AddTransient<IRolePermissionUtilities, RolePermissionUtilities>();
        
        return services;
    }

    private ServiceCollection ConfigureSecurityService(ServiceCollection services)
    {
        services.Configure<AuthenticationSettingsConfig>(_configHelper.Configuration.GetSection("AuthenticationSettingsConfiguration"));
        services.Configure<JwtAuthenticationConfig>(_configHelper.Configuration.GetSection("JwtAuthConfiguration"));
        services.Configure<PasswordValidationConfig>(_configHelper.Configuration.GetSection("PasswordValidationConfiguration"));
        services.Configure<SecurityConnectionStrings>(_configHelper.Configuration.GetSection("SecurityConnectionStrings"));

        services.Configure<TestConfig>(_configHelper.Configuration.GetSection("TestConfiguration"));

        services.AddSingleton<ISecurityConnectionStrings>(sp =>
            sp.GetRequiredService<IOptionsMonitor<SecurityConnectionStrings>>().CurrentValue);

        #region Application

        services.AddTransient<IApplicationService, ApplicationService>();
        services.AddTransient<IApplicationLogic, ApplicationLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterApplicationLogicRequest>, FilterApplicationLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateApplicationRequest>, InsertUpdateApplicationRequestValidator>();

        #endregion

        #region User

        //services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserLogic, UserLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterUserLogicRequest>, FilterUserLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateUserRequest>, InsertUpdateUserRequestValidator>();
        //services.AddTransient<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();

        #endregion

        #region ApplicationUser

        services.AddTransient<IApplicationUserService, ApplicationUserService>();
        services.AddTransient<IApplicationUserLogic, ApplicationUserLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterApplicationUserLogicRequest>, FilterApplicationUserLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateApplicationUserRequest>, InsertUpdateApplicationUserRequestValidator>();
        services.AddTransient<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();

        #endregion

        #region ApplicationUserPermission

        services.AddTransient<IApplicationUserPermissionService, ApplicationUserPermissionService>();
        services.AddTransient<IApplicationUserPermissionLogic, ApplicationUserPermissionLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterApplicationUserPermissionLogicRequest>, FilterApplicationUserPermissionLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateApplicationUserPermissionRequest>, InsertUpdateApplicationUserPermissionRequestValidator>();

        #endregion

        #region ApplicationUserRole

        services.AddTransient<IApplicationUserRoleService, ApplicationUserRoleService>();
        services.AddTransient<IApplicationUserRoleLogic, ApplicationUserRoleLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterApplicationUserRoleLogicRequest>, FilterApplicationUserRoleLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateApplicationUserRoleRequest>, InsertUpdateApplicationUserRoleRequestValidator>();

        #endregion

        #region Authentication

        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IAuthenticationLogic, AuthenticationLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
        services.AddTransient<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();
        services.AddTransient<IValidator<RevokeTokenRequest>, RevokeTokenRequestValidator>();
        services.AddTransient<IValidator<ForgotPasswordRequest>, ForgotPasswordRequestValidator>();

        #endregion

        #region Role

        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IRoleLogic, RoleLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterRoleLogicRequest>, FilterRoleLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateRoleRequest>, InsertUpdateRoleRequestValidator>();

        #endregion

        #region Permission

        services.AddTransient<IPermissionService, PermissionService>();
        services.AddTransient<IPermissionLogic, PermissionLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterPermissionLogicRequest>, FilterPermissionLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdatePermissionRequest>, InsertUpdatePermissionRequestValidator>();

        #endregion

        #region RolePermission

        services.AddTransient<IRolePermissionService, RolePermissionService>();
        services.AddTransient<IRolePermissionLogic, RolePermissionLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterRolePermissionLogicRequest>, FilterRolePermissionLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateRolePermissionRequest>, InsertUpdateRolePermissionRequestValidator>();

        #endregion

        return services;
    }

    #region models

    protected record AssignRoleRequest
    {
        public bool ApplicationAdmin { get; set; } = false;
        public bool ApplicationReadOnly { get; set; } = false;
        public bool ApplicationUserAdmin { get; set; } = false;
        public bool ApplicationUserReadOnly { get; set; } = false;
        public bool ApplicationUserPermissionAdmin { get; set; } = false;
        public bool ApplicationUserPermissionReadOnly { get; set; } = false;
        public bool ApplicationUserRoleAdmin { get; set; } = false;
        public bool ApplicationUserRoleReadOnly { get; set; } = false;
        public bool PermissionAdmin { get; set; } = false;
        public bool PermissionReadOnly { get; set; } = false;
        public bool RoleAdmin { get; set; } = false;
        public bool RoleReadOnly { get; set; } = false;
        public bool RolePermissionAdmin { get; set; } = false;  
        public bool RolePermissionReadOnly { get; set; } = false;
    }

    #endregion

    #region utils

    private async Task<List<PermissionDto>> CreatePermissions(List<InsertUpdatePermissionRequest> permissionsToCreate)
    {
        var createdPermissions = new List<PermissionDto>();

        foreach (var permission in permissionsToCreate)
        {
            permission.CurrentUser = TestConstants.CurrentUser;
            var result = await _permissionLogic.Insert(permission, _applicationLogic);
            createdPermissions.Add(result.Response);
        }

        return createdPermissions;
    }

    private async Task<RoleDto> CreateRole(InsertUpdateRoleRequest req)
    {
        req.CurrentUser = TestConstants.CurrentUser;
        var result = await _roleLogic.Insert(req, _applicationLogic);
        return result.Response;
    }

    private async Task CreateRolePermissions(int applicationId, List<PermissionDto> permissions, int roleId)
    {
        foreach (var permission in permissions)
        {
            await CreateRolePermission(new InsertUpdateRolePermissionRequest { Active = true, ApplicationId = applicationId, RoleId = roleId, PermissionId = permission.PermissionId });
        }
    }

    private async Task<RolePermissionDto> CreateRolePermission(InsertUpdateRolePermissionRequest req)
    {
        req.CurrentUser = TestConstants.CurrentUser;
        var result = await _rolePermissionLogic.Insert(req, _applicationLogic, _roleLogic, _permissionLogic);
        return result.Response;
    }

    private async Task<ApplicationUserPermissionDto> CreateApplicationUserPermission(InsertUpdateApplicationUserPermissionRequest req)
    {
        req.CurrentUser = TestConstants.CurrentUser;
        var result = await _applicationUserPermissionLogic.Insert(req, _applicationLogic, _applicationUserLogic, _permissionLogic);
        return result.Response;
    }

    private async Task<ApplicationUserRoleDto> CreateApplicationUserRole(InsertUpdateApplicationUserRoleRequest req)
    {
        req.CurrentUser = TestConstants.CurrentUser;
        var result = await _applicationUserRoleLogic.Insert(req, _applicationLogic, _applicationUserLogic, _roleLogic);
        return result.Response;
    }

    private async Task AssignRoleToUser(int applicationId, int applicationUserId, int roleId)
    {
        await CreateApplicationUserRole(new InsertUpdateApplicationUserRoleRequest { Active = true, ApplicationId = applicationId, ApplicationUserId = applicationUserId, RoleId = roleId });
    }

    #endregion
}
