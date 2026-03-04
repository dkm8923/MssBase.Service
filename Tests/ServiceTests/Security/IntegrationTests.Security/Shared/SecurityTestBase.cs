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

namespace IntegrationTests.Security.Shared;

public class SecurityTestBase
{
    private readonly AppSettingsHelper _configHelper;
    protected readonly ServiceProvider _serviceProvider;
    protected readonly ILoggerService _loggerSvc;
    protected readonly IApplicationLogic _applicationLogic;
    protected readonly IApplicationUserLogic _applicationUserLogic;
    protected readonly IApplicationUserPermissionLogic _applicationUserPermissionLogic;
    protected readonly IPermissionLogic _permissionLogic;
    protected readonly IRoleLogic _roleLogic;
    protected readonly ISecurityTestUtilitiesManager _securityTestUtilities;
    
    public SecurityTestBase()
    {
        //set environment variable to key off of in program.cs
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        _configHelper = new AppSettingsHelper();

        _serviceProvider = ConfigureServices();
        
        _loggerSvc = _serviceProvider.GetService<ILoggerService>();
        _applicationLogic = _serviceProvider.GetService<IApplicationLogic>();
        _applicationUserLogic = _serviceProvider.GetService<IApplicationUserLogic>();
        _applicationUserPermissionLogic = _serviceProvider.GetService<IApplicationUserPermissionLogic>();
        _permissionLogic = _serviceProvider.GetService<IPermissionLogic>();
        _roleLogic = _serviceProvider.GetService<IRoleLogic>();
        _securityTestUtilities = _serviceProvider.GetService<ISecurityTestUtilitiesManager>();
    }

    protected async Task ClearAllSecurityTestTableData()
    {
        //await _securityTestUtilities.ApplicationUserPermission.DeleteAllRecords();
        //await _securityTestUtilities.RolePermission.DeleteAllRecords();
        await _securityTestUtilities.Role.DeleteAllRecords();
        await _securityTestUtilities.Permission.DeleteAllRecords();
        await _securityTestUtilities.ApplicationUser.DeleteAllRecords();
        await _securityTestUtilities.Application.DeleteAllRecords();
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
        services.AddTransient<IApplicationUtilities, ApplicationUtilities>();
        services.AddTransient<IApplicationUserUtilities, ApplicationUserUtilities>();
        services.AddTransient<IRoleUtilities, RoleUtilities>();
        services.AddTransient<IPermissionUtilities, PermissionUtilities>();
        services.AddTransient<IApplicationUserPermissionUtilities, ApplicationUserPermissionUtilities>();
        
        return services;
    }

    private ServiceCollection ConfigureSecurityService(ServiceCollection services)
    {
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

        #endregion

        #region ApplicationUserPermission

        services.AddTransient<IApplicationUserPermissionService, ApplicationUserPermissionService>();
        services.AddTransient<IApplicationUserPermissionLogic, ApplicationUserPermissionLogic>();

        //Configure Fluent Validation Validators
        services.AddTransient<IValidator<FilterApplicationUserPermissionLogicRequest>, FilterApplicationUserPermissionLogicRequestValidator>();
        services.AddTransient<IValidator<InsertUpdateApplicationUserPermissionRequest>, InsertUpdateApplicationUserPermissionRequestValidator>();

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

        //service dependencies
        
        return services;
    }
}
