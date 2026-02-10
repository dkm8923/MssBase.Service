using Service.Logger.Contracts;
using Service.Logger.Dto;
using StackExchange.Redis;
using System.Text.Json;

namespace Service.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly ILoggerConfig _loggerConfig;
        private readonly IConnectionMultiplexer _redisConnectionMultiplexer;
        private readonly IDatabase _redis;
        
        public LoggerService(ILoggerConfig loggerConfig)
        {
            try
            {
                _loggerConfig = loggerConfig;
                _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(_loggerConfig.ConnectionString);
                _redis = _redisConnectionMultiplexer.GetDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not connect to {_loggerConfig.ConnectionString}: {ex.Message}");
            }
        }

        public async Task Log(InsertLoggerRequest req)
        {
            try
            {
                if (_redis != null) 
                {
                    await _redis.ListLeftPushAsync(_loggerConfig.RedisListName, JsonSerializer.Serialize(req));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not connect to {_loggerConfig.ConnectionString}: {ex.Message}");
            }
        }
    }
}

