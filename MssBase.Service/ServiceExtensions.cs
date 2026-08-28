using MssBase.Service.Shared;
using Contract.Security;
using Contract.Security.Application;
using Contract.Security.ApplicationUser;
using Contract.Common;
using Dto.Security.Application;
using Dto.Security.Application.Logic;
using Dto.Security.ApplicationUser;
using Dto.Security.ApplicationUser.Logic;
using FluentValidation;
using Logic.Security;
using Logic.Security.Logic;
using Logic.Security.Validators.Application;
using Logic.Security.Validators.ApplicationUser;
using Logic.Common;
using Microsoft.Extensions.Options;
using Service.Security;
using Service.Security.Service;
using Service.Common;
using Service.Logger;
using Service.Logger.Contracts;
using Service.Logger.Models;
using Shared.Contracts;
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
using Contract.Security.ApplicationUserRole;
using Dto.Security.ApplicationUserRole.Logic;
using Logic.Security.Validators.ApplicationUserRole;
using Dto.Security.ApplicationUserRole;
using Contract.Security.RolePermission;
using Dto.Security.RolePermission.Logic;
using Logic.Security.Validators.RolePermission;
using Dto.Security.RolePermission;
using Dto.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Shared.JsonConverters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using MssBase.Service.Shared.FluentValidation;
using Contract.Security.Authentication;
using Logic.Security.Validators.Authentication;
using Microsoft.AspNetCore.Authorization;
using MssBase.Service.Shared.Authorization;
using Contract.Common.CommonRelationalData;
using Service.Common.Service;
using Logic.Common.Logic;
using Contract.Security.User;
using Dto.Security.User.Logic;
using Dto.Security.User;
using Logic.Security.Validators.User;

namespace MssBase.Service
{
    public static class ServiceExtensions
    {
        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));
        }

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

        public static void ConfigureAuthenticationSettings(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<AuthenticationSettingsConfig>(builder.Configuration.GetSection("AuthenticationSettingsConfiguration"));
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<JwtAuthenticationConfig>(builder.Configuration.GetSection("JwtAuthConfiguration"));

            builder.Services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptionsMonitor<JwtAuthenticationConfig>>((jwtBearerOptions, jwtConfigMonitor) =>
                {
                    var jwtConfig = jwtConfigMonitor.CurrentValue;
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.ValidIssuer,
                        ValidAudience = jwtConfig.ValidAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.IssuerSigningKey))
                    };
                });
        }

        public static void ConfigurePasswordValidationSettings(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<PasswordValidationConfig>(builder.Configuration.GetSection("PasswordValidationConfiguration"));
        }

        public static void AddPermissionAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }

        public static void ConfigureControllers(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(config =>
            {
                config.Filters.Add(new ProducesAttribute("application/json"));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableDateTimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableBoolToFalseJsonConverter());
            });
        }

        public static void ConfigureCors(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };

            builder.Services.AddCors(options =>
{
                options.AddPolicy("AppPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services, WebApplicationBuilder builder, string environmentName)
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

        public static void ConfigureCommonService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<CommonConnectionStrings>(builder.Configuration.GetSection("CommonConnectionStrings"));

            services.AddSingleton<ICommonConnectionStrings>(sp =>
             sp.GetRequiredService<IOptionsMonitor<CommonConnectionStrings>>().CurrentValue);

            #region CommonRelationalData

            services.AddScoped<ICommonRelationalDataService, CommonRelationalDataService>();
            services.AddScoped<ICommonRelationalDataLogic, CommonRelationalDataLogic>();

            #endregion
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

            #region User

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserLogic, UserLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterUserLogicRequest>, FilterUserLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateUserRequest>, InsertUpdateUserRequestValidator>();
            services.AddTransient<IValidator<Dto.Security.User.ChangePasswordRequest>, Logic.Security.Validators.User.ChangePasswordRequestValidator>();

            #endregion

            #region ApplicationUser

            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IApplicationUserLogic, ApplicationUserLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterApplicationUserLogicRequest>, FilterApplicationUserLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateApplicationUserRequest>, InsertUpdateApplicationUserRequestValidator>();
            services.AddTransient<IValidator<Dto.Security.ApplicationUser.ChangePasswordRequest>, Logic.Security.Validators.ApplicationUser.ChangePasswordRequestValidator>();

            #endregion

            #region ApplicationUserPermission

            services.AddScoped<IApplicationUserPermissionService, ApplicationUserPermissionService>();
            services.AddScoped<IApplicationUserPermissionLogic, ApplicationUserPermissionLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterApplicationUserPermissionLogicRequest>, FilterApplicationUserPermissionLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateApplicationUserPermissionRequest>, InsertUpdateApplicationUserPermissionRequestValidator>();

            #endregion

            #region ApplicationUserRole

            services.AddScoped<IApplicationUserRoleService, ApplicationUserRoleService>();
            services.AddScoped<IApplicationUserRoleLogic, ApplicationUserRoleLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterApplicationUserRoleLogicRequest>, FilterApplicationUserRoleLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateApplicationUserRoleRequest>, InsertUpdateApplicationUserRoleRequestValidator>();

            #endregion

            #region Authentication

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAuthenticationLogic, AuthenticationLogic>();
            
            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
            services.AddTransient<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
            services.AddTransient<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();
            services.AddTransient<IValidator<RevokeTokenRequest>, RevokeTokenRequestValidator>();
            services.AddTransient<IValidator<ForgotPasswordRequest>, ForgotPasswordRequestValidator>();

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

            #region RolePermission

            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<IRolePermissionLogic, RolePermissionLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterRolePermissionLogicRequest>, FilterRolePermissionLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateRolePermissionRequest>, InsertUpdateRolePermissionRequestValidator>();

            #endregion
        }

        public static void ConfigureFluentValidationAutoValidation(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddFluentValidationAutoValidation(configuration =>
            {
                // Disable the built-in .NET model (data annotations) validation.
                configuration.DisableBuiltInModelValidation = true;

                // Only validate controllers decorated with the `AutoValidation` attribute.
                configuration.ValidationStrategy = ValidationStrategy.Annotations;

                // Enable validation for parameters bound from `BindingSource.Body` binding sources.
                configuration.EnableBodyBindingSourceAutomaticValidation = true;

                // Enable validation for parameters bound from `BindingSource.Form` binding sources.
                configuration.EnableFormBindingSourceAutomaticValidation = true;

                // Enable validation for parameters bound from `BindingSource.Query` binding sources.
                configuration.EnableQueryBindingSourceAutomaticValidation = true;

                // Enable validation for parameters bound from `BindingSource.Path` binding sources.
                configuration.EnablePathBindingSourceAutomaticValidation = true;

                // Enable validation for parameters bound from 'BindingSource.Custom' binding sources.
                configuration.EnableCustomBindingSourceAutomaticValidation = true;

                // Replace the default result factory with a custom implementation.
                configuration.OverrideDefaultResultFactoryWith<FluentValidationCustomResultFactory>();
            });
        }
    }
}
