using Microsoft.Extensions.Configuration;

namespace Tests.Shared
{
    public class AppSettingsHelper
    {
        public IConfiguration Configuration { get; }

        public AppSettingsHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.IntegrationTest.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public string GetSetting(string key)
        {
            return Configuration[key];
        }

        public string GetRedisConnectionString()
        {
            return Configuration.GetSection("RedisConfiguration")["ConnectionString"];
        }
    }
}
