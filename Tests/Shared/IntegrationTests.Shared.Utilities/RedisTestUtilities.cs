using IntegrationTests.Shared.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts;
using Shared.Service.Cache.Redis;
using StackExchange.Redis;
using System.Text.Json;
using Tests.Shared;

namespace IntegrationTests.Shared
{
    public class RedisTestUtilities : ICacheTestUtilities
    {
        private readonly AppSettingsHelper _configHelper;

        public RedisTestUtilities()
        {
            _configHelper = new AppSettingsHelper();
        }

        private ConnectionMultiplexer RedisConnection =>
        ConnectionMultiplexer.Connect(_configHelper.GetRedisConnectionString());

        private IDatabase GetRedisConnectionDatabase() => RedisConnection.GetDatabase();

        public List<string> GetKeys()
        {
            var redisDb = GetRedisConnectionDatabase();

            return redisDb.Multiplexer.GetServer(_configHelper.GetRedisConnectionString()).Keys().Select(key => key.ToString()).ToList();
        }

        //public void ConfigureRedis(ServiceCollection services)
        //{
        //    var redisServerUrl = _configHelper.GetRedisConnectionString();

        //    IConnectionMultiplexer redisConnectionMultiplexer = null;

        //    try
        //    {
        //        redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisServerUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and continue with a dummy connection multiplexer
        //        Console.WriteLine($"Could not connect to Redis: {ex.Message}");
        //        redisConnectionMultiplexer = new DummyConnectionMultiplexer();
        //    }

        //    services.AddSingleton(redisConnectionMultiplexer);

        //    services.AddTransient<ICacheService, RedisExtensions>();
        //}

        public void ConfigureCache(ServiceCollection services)
        {
            var redisServerUrl = _configHelper.GetRedisConnectionString();

            IConnectionMultiplexer redisConnectionMultiplexer = null;

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

            services.AddTransient<ICacheService, RedisExtensions>();
        }

        public async Task<T?> GetKeyData<T>(string keyName)
        {
            var redisDb = GetRedisConnectionDatabase();

            var cachedJson = await redisDb.StringGetAsync(keyName);

            if (cachedJson.IsNullOrEmpty)
            {
                return default;
            }

            //return JsonSerializer.Deserialize<T>(cachedJson);
            return JsonSerializer.Deserialize<T>((string)cachedJson); //fix for .NET 10 upgrade
        }

        public async Task DeleteAllKeyData()
        {
            var redisDb = GetRedisConnectionDatabase();
            var availableCacheKeys = this.GetKeys();

            foreach (var key in availableCacheKeys) 
            {
                await redisDb.KeyDeleteAsync(key);
            }
        }

    }
}
