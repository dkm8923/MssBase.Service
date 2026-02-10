using System.Text.Json;
using Enyim.Caching.Memcached;
using Shared.Contracts;
using Shared.Logic.Common;
using Shared.Models;

namespace Shared.Service.Cache.Memcached;

public class MemcachedService : ICacheService
{
	private readonly IMemcachedClient _client;
	private const string ALL_KEYS = "ALL_KEYS";

	public MemcachedService(IMemcachedClient memcachedClient)
	{
		_client = memcachedClient;
	}

	public async Task<ErrorValidationResult<TResponse>> GetByKeyAsync<TResponse>(bool deleteCache, string keyName, Func<Task<ErrorValidationResult<TResponse>>> asyncFunction, int expiresIn = 0)
	{
		if (deleteCache)
		{
			var deleteResult = await _client.DeleteWithResultAsync(keyName);
			// TODO: Log the OperationStatus if it is unexpected?
		}

		var getResult = await _client.GetWithResultAsync<string>(keyName);
		// TODO: Log the OperationStatus if it is unexpected?

		if (getResult.Success)
		{
			return new ErrorValidationResult<TResponse>
			{
				Response = JsonSerializer.Deserialize<TResponse>(getResult.Value)
			};
		}

		var result = await asyncFunction();

		var responseIsEmptyList = CommonUtilities.IsGenericTypeCollectionWithData(result.Response);

		if (result.Response != null && !responseIsEmptyList)
		{
			var expiration = expiresIn is 0 ? Expiration.Never : Expiration.From(TimeSpan.FromMinutes(expiresIn));
			await SetAsync(keyName, result.Response, expiration);
			await AppendToAllKeysAsync(keyName);
			// TODO: Log the OperationStatus if it is unexpected?
		}

		return result;
	}

	public async Task RemoveKeysByPatternAsync(string pattern)
	{
		var getResult = await _client.GetWithResultAsync<string>(ALL_KEYS);

		if (getResult.Success)
		{
			var arr = JsonSerializer.Deserialize<IEnumerable<string>>(getResult.Value);
			var matchingKeys = arr.Where(t => CacheUtilities.KeySatisfiesPattern(t, pattern));
			if (!matchingKeys.Any())
			{
				return;
			}
			
			// Update the ALL_KEYS list
			await SetAsync(ALL_KEYS, arr.Where(t => !matchingKeys.Contains(t)), Expiration.Never);
			// Delete the actual KVP
			var deleteTasks = matchingKeys.Select(t => _client.DeleteWithResultAsync(t));
			await Task.WhenAll(deleteTasks);
			// TODO: Log the OperationStatus if it is unexpected?
		}
	}

	private async Task SetAsync<T>(string keyName, T value, Expiration expiration)
	{
		var storeValue = JsonSerializer.Serialize(value);
		var storeResult = await _client.StoreWithResultAsync(StoreMode.Set, keyName, storeValue, expiration: expiration);
		// TODO: Log the OperationStatus if it is unexpected?
	}

	private async Task AppendToAllKeysAsync(string key)
	{
		var getResult = await _client.GetWithResultAsync<string>(ALL_KEYS);

		if (getResult.Success)
		{
			var arr = JsonSerializer.Deserialize<IEnumerable<string>>(getResult.Value);
			await SetAsync(ALL_KEYS, arr.Append(key), expiration: Expiration.Never);
		}
		else if (getResult.StatusCode == OperationStatus.KeyNotFound)
		{
			await SetAsync<IEnumerable<string>>(ALL_KEYS, [key], expiration: Expiration.Never);
		}
	}
}