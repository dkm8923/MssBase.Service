using MssBase.Service.Shared;
using Contract.Security;
using Contract.Security.Application;
using Dto.Security.Application;
using Dto.Security.Application.Logic;
using FluentValidation;
using IntegrationTests.Security.Shared.Utilities;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Logic.Security;
using Logic.Security.Logic;
using Logic.Security.Validators.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Service.Security;
using Service.Security.Service;
using Service.Logger.Contracts;
using Shared.Contracts.Logic.Validators;
using Shared.Logic.Validators;
using Tests.Shared;
using Shared.Contracts;

namespace IntegrationTests.Security.Shared;

public class SecurityTestBase
    {
        private readonly AppSettingsHelper _configHelper;
        protected readonly ServiceProvider _serviceProvider;
        protected readonly ILoggerService _loggerSvc;
        protected readonly ISecurityLogicManager _SecurityLogic;
        protected readonly ISecurityTestUtilitiesManager _SecurityTestUtilities;
        protected readonly IValidatorUtilities _validatorUtilities;

        public SecurityTestBase()
        {
            //set environment variable to key off of in program.cs
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
            _configHelper = new AppSettingsHelper();

            _serviceProvider = ConfigureServices();
            
            _loggerSvc = _serviceProvider.GetService<ILoggerService>();
            _SecurityLogic = _serviceProvider.GetService<ISecurityLogicManager>();
            _SecurityTestUtilities = _serviceProvider.GetService<ISecurityTestUtilitiesManager>();
            _validatorUtilities = _serviceProvider.GetService<IValidatorUtilities>();
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
            //new MemcachedTestUtilities().Configure(services);

            services.AddTransient<ICacheTestUtilities, RedisTestUtilities>();

            //configure logger service
            //services.AddSingleton<ILoggerService, LoggerServiceTestStub>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidatorUtilities, ValidatorUtilities>();

            //unit testing dependencies
            services.AddTransient<ISecurityTestUtilitiesManager, SecurityTestUtilitiesManager>();
            services.AddTransient<IApplicationUtilities, ApplicationUtilities>();
            
            return services;
        }

        private ServiceCollection ConfigureSecurityService(ServiceCollection services)
        {
            services.Configure<DatabaseConnectionStrings>(_configHelper.Configuration.GetSection("SecurityConnectionStrings"));

            services.AddSingleton<IDatabaseConnectionStrings>(sp =>
                sp.GetRequiredService<IOptionsMonitor<DatabaseConnectionStrings>>().CurrentValue);

            #region Application

            //Add Services Here...
            services.AddTransient<IApplicationService, ApplicationService>();
            services.AddTransient<IApplicationLogic, ApplicationLogic>();

            //Configure Fluent Validation Validators
            services.AddTransient<IValidator<FilterApplicationLogicRequest>, FilterApplicationLogicRequestValidator>();
            services.AddTransient<IValidator<InsertUpdateApplicationRequest>, InsertUpdateApplicationRequestValidator>();

            #endregion

            //service dependencies
            services.AddTransient<ISecurityLogicManager, SecurityLogicManager>();
            services.AddTransient<ISecurityServiceManager, SecurityServiceManager>();

            return services;
        }
    }
