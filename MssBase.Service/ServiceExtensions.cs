using MssBase.Service.Shared;
using Contract.Security;
using Contract.Security.Application;
using Contract.Security.ApplicationUser;
//using Contract.Common;
using Dto.Security.Application;
using Dto.Security.Application.Logic;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using FluentValidation;
using Logic.Security;
using Logic.Security.Logic;
using Logic.Security.Validators.Application;
using Logic.Security.Validators.ApplicationUser;
//using Logic.Common;
using Microsoft.Extensions.Options;
using Service.Security;
using Service.Security.Service;
//using Service.Common;
using Service.Logger;
using Service.Logger.Contracts;
using Service.Logger.Models;
using Shared.Contracts;
using Shared.Contracts.Logic;
using Shared.Contracts.Logic.Validators;
using Shared.Logic;
using Shared.Logic.Validators;
using Shared.Service.Cache.Redis;
using StackExchange.Redis;
using MssBase.Service.Shared.ConnectionStrings;
using Contract.Common;
using Contract.Security.Role;
using Contract.Security.Permission;
using Dto.Security.Role.Logic;
using Dto.Security.Role;
using Dto.Security.Permission.Logic;
using Dto.Security.Permission;
using Logic.Security.Validators.Role;
using Logic.Security.Validators.Permission;
using Contract.Security.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission.Logic;
using Logic.Security.Validators.ApplicationUserPermission;
using Dto.Security.ApplicationUserPermission;

namespace MssBase.Service
{
    public static class ServiceExtensions
    {
        private static void ConfigureRedis(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var redisServerUrl = builder.Configuration.GetSection("RedisConfiguration")?.GetSection("ConnectionString").Value;

            IConnectionMultiplexer redisConnectionMultiplexer;
            try
            {
                redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisServerUrl);
            }
            catch (Exception ex)
            {
                // Log the exception and continue with a dummy connection multiplexer
                Console.WriteLine($"Could not connect to Redis: {ex.Message}");
                redisConnectionMultiplexer = new DummyConnectionMultiplexer();
            }

            services.AddSingleton(redisConnectionMultiplexer);

            services.AddScoped<ICacheService, RedisExtensions>();
        }

        public static void ConfigureCache(this IServiceCollection services, WebApplicationBuilder builder)
        {
            if (builder.Configuration.GetSection("RedisConfiguration")?.GetSection("ConnectionString")?.Exists() is true)
            {
                ConfigureRedis(services, builder);
            }
            else
            {
                throw new Exception("Cache Configuration not found");
            }
        }

        private static void ConfigureLoggerService(this IServiceCollection services, WebApplicationBuilder builder, string environmentName)
        {
            if (environmentName == "IntegrationTest")
            {
                // Apply test-specific configuration or services
                services.AddSingleton<ILoggerService, LoggerServiceTestStub>();
            }
            else
            {
                services.Configure<LoggerConfig>(builder.Configuration.GetSection("LoggerConfiguration"));

                services.AddSingleton<ILoggerConfig>(sp =>
                    sp.GetRequiredService<IOptionsMonitor<LoggerConfig>>().CurrentValue);

                services.AddSingleton<ILoggerService, LoggerService>();
            }
        }

        public static void ConfigureBaseDependencies(this IServiceCollection services, WebApplicationBuilder builder, string environmentName)
        {
            ConfigureLoggerService(services, builder, environmentName);

            //configure shared utility logic
            services.AddTransient<IValidatorUtilities, ValidatorUtilities>();
        }

        public static void ConfigureCommonService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<CommonConnectionStrings>(builder.Configuration.GetSection("CommonConnectionStrings"));

            services.AddSingleton<ICommonConnectionStrings>(sp =>
             sp.GetRequiredService<IOptionsMonitor<CommonConnectionStrings>>().CurrentValue);

            // #region CommonType

            // services.AddScoped<ICommonTypeService, CommonTypeService>();
            // services.AddScoped<ICommonTypeLogic, CommonTypeLogic>();

            // #endregion
        }

        public static void ConfigureSecurityService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<SecurityConnectionStrings>(builder.Configuration.GetSection("SecurityConnectionStrings"));

            services.AddSingleton<ISecurityConnectionStrings>(sp =>
             sp.GetRequiredService<IOptionsMonitor<SecurityConnectionStrings>>().CurrentValue);

            #region Application

            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IApplicationLogic, ApplicationLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterApplicationLogicRequest>, FilterApplicationLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateApplicationRequest>, InsertUpdateApplicationRequestValidator>();

            #endregion

            #region ApplicationUser

            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IApplicationUserLogic, ApplicationUserLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterApplicationUserLogicRequest>, FilterApplicationUserLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateApplicationUserRequest>, InsertUpdateApplicationUserRequestValidator>();

            #endregion

            #region ApplicationUserPermission

            services.AddScoped<IApplicationUserPermissionService, ApplicationUserPermissionService>();
            services.AddScoped<IApplicationUserPermissionLogic, ApplicationUserPermissionLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterApplicationUserPermissionLogicRequest>, FilterApplicationUserPermissionLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateApplicationUserPermissionRequest>, InsertUpdateApplicationUserPermissionRequestValidator>();

            #endregion

            #region Role

            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleLogic, RoleLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterRoleLogicRequest>, FilterRoleLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateRoleRequest>, InsertUpdateRoleRequestValidator>();

            #endregion

            #region Permission

            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IPermissionLogic, PermissionLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterPermissionLogicRequest>, FilterPermissionLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdatePermissionRequest>, InsertUpdatePermissionRequestValidator>();

            #endregion
        }
    }
}
