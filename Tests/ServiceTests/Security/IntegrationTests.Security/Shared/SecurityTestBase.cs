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

namespace IntegrationTests.Security.Shared;

public class SecurityTestBase
{
    private readonly ISecurityConnectionStrings _connectionStrings;
    protected readonly SecurityDBContextFactory _dbContextFactory;
    private readonly AppSettingsHelper _configHelper;
    protected readonly ServiceProvider _serviceProvider;
    protected readonly ILoggerService _loggerSvc;
    protected readonly IAuthenticationLogic _authenticationLogic;
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly IApplicationUserLogic _applicationUserLogic;
    protected readonly IApplicationUserPermissionLogic _applicationUserPermissionLogic;
    protected readonly IApplicationUserRoleLogic _applicationUserRoleLogic;
    protected readonly IPermissionLogic _permissionLogic;
    protected readonly IRoleLogic _roleLogic;
    protected readonly IRolePermissionLogic _rolePermissionLogic;
    protected readonly ISecurityTestUtilitiesManager _securityTestUtilities;
    protected readonly IOptionsMonitor<AuthenticationSettingsConfig> _authenticationSettingsConfigMonitor;
    
    public SecurityTestBase()
    {
        //set environment variable to key off of in program.cs
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        _configHelper = new AppSettingsHelper();

        _serviceProvider = ConfigureServices();
        
        //get instance of db context factory (Should be using services for 99% of DB interactions, but their are always edge cases)
        _connectionStrings = _serviceProvider.GetService<ISecurityConnectionStrings>();
        _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);

        _loggerSvc = _serviceProvider.GetService<ILoggerService>();
        _authenticationSettingsConfigMonitor = _serviceProvider.GetService<IOptionsMonitor<AuthenticationSettingsConfig>>();
        _authenticationLogic = _serviceProvider.GetService<IAuthenticationLogic>();
        _applicationLogic = _serviceProvider.GetService<IApplicationLogic>();
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
        await _securityTestUtilities.ApplicationUserPermission.DeleteAllRecords();
        await _securityTestUtilities.ApplicationUserRole.DeleteAllRecords();
        await _securityTestUtilities.RolePermission.DeleteAllRecords();
        await _securityTestUtilities.Role.DeleteAllRecords();
        await _securityTestUtilities.Permission.DeleteAllRecords();
        await _securityTestUtilities.ApplicationUser.DeleteAllRecords();
        await _securityTestUtilities.Application.DeleteAllRecords();
    }

    protected async Task DropDatabaseAndRecreate()
    {
        using (var dbContext = _dbContextFactory.CreateContextReadWrite())
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }

    protected async Task<SecurityTestData> ArrangeApplicationUserTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

        ret.ActiveApplications.Add(application);

        ret.ActiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateActiveTestRecords(application.ApplicationId);;
        ret.InactiveApplicationUsers = await _securityTestUtilities.ApplicationUser.CreateInactiveTestRecords(application.ApplicationId);

        return ret;
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

        ret.ActivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
        ret.InactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId);

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

    protected async Task<SecurityTestData> ArrangePermissionTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();

        ret.ActiveApplications.Add(application);

        ret.ActivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);;
        ret.InactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeRoleTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        ret.ActiveApplications.Add(application);

        ret.ActiveRoles = await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId);;
        ret.InactiveRoles = await _securityTestUtilities.Role.CreateInactiveTestRecords(application.ApplicationId);

        return ret;
    }

    protected async Task<SecurityTestData> ArrangeRoleTestDataWithRelatedData()
    {
        var ret = await ArrangeRoleTestData();

        var application = ret.ActiveApplications.FirstOrDefault();

        var activeRolePermissions = new List<RolePermissionDto>();
        var inactiveRolePermissions = new List<RolePermissionDto>();

        var activePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
        var inactivePermissions = await _securityTestUtilities.Permission.CreateInactiveTestRecords(application.ApplicationId);
        
        foreach (var activeRole in ret.ActiveRoles)
        {
            //create 5 active RolePermission records
            foreach (var activePermission in activePermissions) 
            {
                activeRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, activeRole.RoleId, activePermission.PermissionId));
            }

            //create 5 inactive RolePermission records
            foreach (var inactivePermission in inactivePermissions) 
            {
                inactiveRolePermissions.Add(await _securityTestUtilities.RolePermission.CreateSingleRolePermissionTestRecord(application.ApplicationId, activeRole.RoleId, inactivePermission.PermissionId, false));
            }
        }

        foreach (var inactiveRole in ret.InactiveRoles)
        {
            //create 5 inactive RolePermission records
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
        var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
        var activePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
        var inactivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);

        var activeApplicationUserPermissions = new List<ApplicationUserPermissionDto>();
        var inactiveApplicationUserPermissions = new List<ApplicationUserPermissionDto>();

        //create 5 active ApplicationUserPermission records
        foreach (var activePermission in activePermissions) 
        {
            activeApplicationUserPermissions.Add(await _securityTestUtilities.ApplicationUserPermission.CreateSingleApplicationUserPermissionTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activePermission.PermissionId));
        }

        //create 5 inactive ApplicationUserPermission records
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

    protected async Task<SecurityTestData> ArrangeApplicationUserRoleTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        var applicationUser = await _securityTestUtilities.ApplicationUser.CreateSingleApplicationUserTestRecord(application.ApplicationId);
        var activeRoles = await _securityTestUtilities.Role.CreateActiveTestRecords(application.ApplicationId);
        var inactiveRoles = await _securityTestUtilities.Role.CreateInactiveTestRecords(application.ApplicationId);

        var activeApplicationUserRoles = new List<ApplicationUserRoleDto>();
        var inactiveApplicationUserRoles = new List<ApplicationUserRoleDto>();

        //create 5 active ApplicationUserRole records
        foreach (var activeRole in activeRoles) 
        {
            activeApplicationUserRoles.Add(await _securityTestUtilities.ApplicationUserRole.CreateSingleApplicationUserRoleTestRecord(application.ApplicationId, applicationUser.ApplicationUserId, activeRole.RoleId));
        }

        //create 5 inactive ApplicationUserRole records
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

    protected async Task<SecurityTestData> ArrangeRolePermissionTestData()
    {
        // Arrange
        var ret = new SecurityTestData();
        await ClearAllSecurityTestTableData();
        var application = await _securityTestUtilities.Application.CreateSingleApplicationTestRecord();
        var role = await _securityTestUtilities.Role.CreateSingleRoleTestRecord(application.ApplicationId);
        var activePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);
        var inactivePermissions = await _securityTestUtilities.Permission.CreateActiveTestRecords(application.ApplicationId);

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
        
        services.Configure<SecurityConnectionStrings>(_configHelper.Configuration.GetSection("SecurityConnectionStrings"));

        services.AddSingleton<ISecurityConnectionStrings>(sp =>
            sp.GetRequiredService<IOptionsMonitor<SecurityConnectionStrings>>().CurrentValue);

        #region Application

        services.AddTransient<IApplicationService, ApplicationService>();
        services.AddTransient<IApplicationLogic, ApplicationLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterApplicationLogicRequest>, FilterApplicationLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateApplicationRequest>, InsertUpdateApplicationRequestValidator>();

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
}
