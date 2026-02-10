using Shared.Contracts;
using Shared.Logic.Common;
using Shared.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Shared.Service.Cache.Redis
{
    public class RedisExtensions : ICacheService
    {
        private readonly IDatabase _redis;

        public RedisExtensions(IConnectionMultiplexer multiplexer) 
        {
            _redis = multiplexer.GetDatabase();
        }

        /// <summary>
        /// Top Level, all in one function that should be used in majority of cases for data retrieval operations. Checks if redis is connected / Removes cache key if applicable / Retrieve data from cache or db.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deleteCache">Boolean to allow for deleting redis cache instance of request if exists</param>
        /// <param name="keyName">String value for key that will be stored in Redis</param>
        /// <param name="asyncFunction">Logic that will be executed if Redis key does not exist or if Redis is unavailable</param>
        /// <param name="expiresIn">TTL for Redis Key / Value</param>
        /// <returns></returns>
        public async Task<ErrorValidationResult<TResponse>> GetByKeyAsync<TResponse>(bool deleteCache, string keyName, Func<Task<ErrorValidationResult<TResponse>>> asyncFunction, int expiresIn = RedisConstants.DefaultRedisCacheExpireMinutes)
        {
            var redisIsConnected = await CheckIfRedisConnected();
            await RemoveCacheKeyIfApplicable(deleteCache, keyName, redisIsConnected);
            return await GetDataFromCacheOrDb(redisIsConnected, keyName, asyncFunction);
        }

        /// <summary>
        /// Removes all cache keys related to sent pattern (IE: Use this when deleting all instances of keys for particular service on Insert / Update / Delete)
        /// </summary>
        /// <param name="pattern">Search string to find all available keys</param>
        /// <returns></returns>
        public async Task RemoveKeysByPatternAsync(string pattern)
        {
            var redisIsConnected = await CheckIfRedisConnected();
            var keysToDelete = await FindKeysWithPatternAsync(pattern, redisIsConnected);

            await RemoveCacheKeysIfApplicable(keysToDelete.ToList(), redisIsConnected);
        }

        /// <summary>
        /// Retrieve ValidationResponse data from Redis if applicable or Database if not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisIsConnected">Boolean indicating whether Redis is available</param>
        /// <param name="keyName">String value for key that will be stored in Redis</param>
        /// <param name="asyncFunction">Logic that will be executed if Redis key does not exist or if Redis is unavailable</param>
        /// <param name="expiresIn">TTL for Redis Key / Value</param>
        /// <returns></returns>
        private async Task<ErrorValidationResult<TResponse>> GetDataFromCacheOrDb<TResponse>(bool redisIsConnected, string keyName, Func<Task<ErrorValidationResult<TResponse>>> asyncFunction, int expiresIn = RedisConstants.DefaultRedisCacheExpireMinutes)
        {
            if (!redisIsConnected)
            {
                return await asyncFunction();
            }

            var cachedJson = await _redis.StringGetAsync(keyName);

            if (string.IsNullOrEmpty(cachedJson))
            {
                var validationResult = await asyncFunction();

                //check if response is empty collection and do not cache if the case
                var responseIsEmptyList = CommonUtilities.IsGenericTypeCollectionWithData(validationResult.Response);

                if (validationResult.Response != null && !responseIsEmptyList)
                {
                    await CreateRedisCacheKeyValue(keyName, JsonSerializer.Serialize(validationResult.Response), expiresIn);
                }

                return validationResult;
            }

            return new ErrorValidationResult<TResponse> { Response = JsonSerializer.Deserialize<TResponse>(cachedJson) };
        }

        /// <summary>
        /// Create Redis Cache Key By keyName parm
        /// </summary>
        /// <param name="keyName">String value for key that will be stored in Redis</param>
        /// <param name="json">Actual JSON that will be stored in Redis</param>
        /// <param name="expiresIn">TTL for Redis Key / Value</param>
        /// <returns></returns>
        private async Task CreateRedisCacheKeyValue(string keyName, string json, int expiresIn)
        {
            var setTask = _redis.StringSetAsync(keyName, json);
            var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromMinutes(expiresIn));
            await Task.WhenAll(setTask, expireTask);
        }

        /// <summary>
        /// Remove Redis cache key / value
        /// </summary>
        /// <param name="deleteCache">Boolean to allow for deleting redis cache instance of request if exists</param>
        /// <param name="keyName">String value for key that will be stored in Redis</param>
        /// <param name="redisIsConnected">Boolean indicating whether Redis is available</param>
        /// <returns></returns>
        private async Task RemoveCacheKeyIfApplicable(bool deleteCache, string keyName, bool redisIsConnected)
        {
            if (deleteCache && redisIsConnected)
            {
                await _redis.KeyDeleteAsync(keyName);
            }
        }

        /// <summary>
        /// Removes list of cahce keys from Redis 
        /// </summary>
        /// <param name="keys">List of Redis keys to remove</param>
        /// <param name="redisIsConnected">Boolean indicating whether Redis is available</param>
        /// <returns></returns>
        private async Task RemoveCacheKeysIfApplicable(List<string> keys, bool redisIsConnected)
        {
            if (redisIsConnected)
            {
                foreach (var key in keys)
                {
                    await _redis.KeyDeleteAsync(key);
                }
            }
        }

        /// <summary>
        /// Verify Redis is available for use
        /// </summary>
        /// <param name="timeoutMilliseconds">Allowed time for pinging Redis instance</param>
        /// <returns></returns>
        private async Task<bool> CheckIfRedisConnected(int timeoutMilliseconds = 1000)
        {
            //Task.WhenAny is used to wait for either the PingAsync task or a delay task to complete.
            //If the PingAsync task completes first, it means the ping was successful within the timeout period.
            //If the delay task completes first, it means the ping took too long, and the method returns false

            try
            {
                //Redis was down on initial app startup. Dummy multiplexor was used for DI
                if (_redis == null)
                {
                    return false;
                }

                var pingTask = _redis.PingAsync();
                var delayTask = Task.Delay(timeoutMilliseconds);
                var completedTask = await Task.WhenAny(pingTask, delayTask);
                if (completedTask == pingTask)
                {
                    // Ping completed within the timeout
                    await pingTask; // Ensure any exceptions are observed
                    return true;
                }
                else
                {
                    // Timeout
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Connection failed
                Console.WriteLine("Redis Did Not Connect: " + ex);
                return false;
            }
        }

        /// <summary>
        /// Find and return Cache Keys that match pattern (Uses Redis SCAN Command)
        /// </summary>
        /// <param name="pattern">Pattern that will be searched for against Redis key(s)</param>
        /// <returns></returns>
        private async Task<IEnumerable<string>> FindKeysWithPatternAsync(string pattern, bool redisIsConnected)
        {
            if (redisIsConnected)
            {
                var endpoints = _redis.Multiplexer.GetEndPoints();
                var server = _redis.Multiplexer.GetServer(endpoints[0]);

                var keys = new List<string>();
                var cursor = 0L;

                do
                {
                    var result = await server.ExecuteAsync("SCAN", cursor.ToString(), "MATCH", $"*{pattern}*");
                    var resultArray = (RedisResult[])result;

                    cursor = (long)resultArray[0];
                    var keysArray = (RedisKey[])resultArray[1];

                    keys.AddRange(keysArray.Select(key => key.ToString()));
                } while (cursor != 0);

                return keys;
            }

            return new List<string>();
        }
    }
}
