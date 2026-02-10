using MssBase.Service.Shared;
using Contract.Security;
using Contract.Security.Application;
using Contract.Common;
using Contract.Common.CommonType;
using Contract.Common.Rate;
using Contract.Common.Unit;     
using Contract.Common.UnitDefinition;
using Contract.Common.UnitGroupColumn;
using Dto.Security.Application;
using Dto.Security.Application.Logic;
using Dto.Common.Rate;
using Dto.Common.Rate.Logic;
using Dto.Common.Unit;
using Dto.Common.Unit.Logic;
using Dto.Common.UnitDefinition;
using Dto.Common.UnitGroupColumn;
using FluentValidation;
using Logic.Security;
using Logic.Security.Logic;
using Logic.Security.Validators.Application;
using Logic.Common;
using Logic.Common.Logic;
using Logic.Common.Validators.Rate;
using Logic.Common.Validators.Unit;
using Logic.Common.Validators.UnitDefinition;
using Logic.Common.Validators.UnitGroupColumn;
using Microsoft.Extensions.Options;
using Service.Security;
using Service.Security.Service;
using Service.Common;
using Service.Common.Service;
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

        //private static void ConfigureMemcached(this IServiceCollection services, WebApplicationBuilder builder)
        //{
        //    var memcachedEndpoints = builder.Configuration.GetSection("MemcachedConfiguration").GetSection("Endpoints").Value;
        //    var memcachedCluster = new MemcachedCluster(memcachedEndpoints);
        //    memcachedCluster.Start();
        //    services.AddSingleton(memcachedCluster.GetClient());

        //    services.AddScoped<ICacheService, MemcachedService>();
        //}

        public static void ConfigureCache(this IServiceCollection services, WebApplicationBuilder builder)
        {
            if (builder.Configuration.GetSection("RedisConfiguration")?.GetSection("ConnectionString")?.Exists() is true)
            {
                ConfigureRedis(services, builder);
            }
            //else if (builder.Configuration.GetSection("MemcachedConfiguration")?.GetSection("Endpoints")?.Exists() is true)
            //{
            //    //ConfigureMemcached(services, builder);
            //}
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
            services.AddTransient<ILogicUtilities, LogicUtilities>();
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

            services.AddScoped<ICommonServiceManager, CommonServiceManager>();
            services.AddScoped<ICommonLogicManager, CommonLogicManager>();
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

            services.AddScoped<ISecurityServiceManager, SecurityServiceManager>();
            services.AddScoped<ISecurityLogicManager, SecurityLogicManager>();
        }
    }
}
