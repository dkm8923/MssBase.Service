using MssBase.Service.Shared;
using Contract.Common;
using Contract.Common.Unit;
using Dto.Common.Unit;
using Dto.Common.Unit.Logic;
using FluentValidation;
using IntegrationTests.Common.Shared.Utilities;
using IntegrationTests.Common.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Logic.Common;
using Logic.Common.Logic;
using Logic.Common.Validators.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Service.Common;
using Service.Common.Service;
using Service.Logger.Contracts;
using Shared.Contracts.Logic.Validators;
using Shared.Logic.Validators;
using Tests.Shared;

namespace IntegrationTests.Common.Shared
{
    public class CommonTestBase
    {
        private readonly AppSettingsHelper _configHelper;
        protected readonly ServiceProvider _serviceProvider;
        protected readonly ILoggerService _loggerSvc;
        protected readonly ICommonLogicManager _CommonLogic;
        protected readonly ICommonTestUtilitiesManager _CommonTestUtilities;
        protected readonly IValidatorUtilities _validatorUtilities;

        public CommonTestBase()
        {
            //set environment variable to key off of in program.cs
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
            _configHelper = new AppSettingsHelper();

            _serviceProvider = ConfigureServices();
            
            _loggerSvc = _serviceProvider.GetService<ILoggerService>();
            _CommonLogic = _serviceProvider.GetService<ICommonLogicManager>();
            _CommonTestUtilities = _serviceProvider.GetService<ICommonTestUtilitiesManager>();
            _validatorUtilities = _serviceProvider.GetService<IValidatorUtilities>();
        }

        private ServiceProvider ConfigureServices() 
        {
            var services = new ServiceCollection();

            services = ConfigureBaseDependencies(services);
            services = ConfigureCommonService(services);

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
            services.AddTransient<ICommonTestUtilitiesManager, CommonTestUtilitiesManager>();
            //services.AddTransient<IUnitUtilities, UnitUtilities>();
            
            return services;
        }

        private ServiceCollection ConfigureCommonService(ServiceCollection services)
        {
            services.Configure<CommonConnectionStrings>(_configHelper.Configuration.GetSection("CommonConnectionStrings"));

            services.AddSingleton<ICommonConnectionStrings>(sp =>
             sp.GetRequiredService<IOptionsMonitor<CommonConnectionStrings>>().CurrentValue);

            #region Unit

            //Add Services Here...
            // services.AddTransient<IUnitService, UnitService>();
            // services.AddTransient<IUnitLogic, UnitLogic>();

            // //Configure Fluent Validation Validators
            // services.AddTransient<IValidator<FilterUnitLogicRequest>, FilterUnitLogicRequestValidator>();
            // services.AddTransient<IValidator<InsertUpdateUnitRequest>, InsertUpdateUnitRequestValidator>();

            #endregion

            //service dependencies
            services.AddTransient<ICommonLogicManager, CommonLogicManager>();
            services.AddTransient<ICommonServiceManager, CommonServiceManager>();

            return services;
        }
    }
}
