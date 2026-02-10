using Enyim.Caching.Memcached;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts;
using Shared.Service.Cache.Memcached;
using System.Text.Json;
using Tests.Shared;

namespace IntegrationTests.Shared
{
	public class MemcachedTestUtilities
	{
		private readonly AppSettingsHelper _configHelper;
		private readonly MemcachedCluster _memcachedCluster;
		private readonly IMemcachedClient _iMemcachedClient;
		private readonly MemcachedService _memcachedService;

		public MemcachedTestUtilities()
		{
			_configHelper = new AppSettingsHelper();
			_memcachedCluster = new MemcachedCluster(_configHelper.GetMemcachedEndpointsString());
			_memcachedCluster.Start();
			_iMemcachedClient = _memcachedCluster.GetClient();
			_memcachedService = new MemcachedService(_iMemcachedClient);
		}

		public async Task<IEnumerable<string>> GetKeysAsync()
		{
			var result = await _iMemcachedClient.GetWithResultAsync<string>("ALL_KEYS");
			if (!result.Success)
			{
				return [];
			}

			return JsonSerializer.Deserialize<IEnumerable<string>>(result.Value);
		}

		public void Configure(ServiceCollection services)
		{
            // var memcachedEndpoints = _configHelper.GetMemcachedEndpointsString();
            // var memcachedCluster = new MemcachedCluster(memcachedEndpoints);
            // memcachedCluster.Start();
            services.AddSingleton(_iMemcachedClient);
            services.AddScoped<ICacheService, MemcachedService>();
		}

		public async Task<T?> GetKeyData<T>(string keyName)
		{
			var result = await _iMemcachedClient.GetWithResultAsync<string>(keyName);

			if (!result.Success)
			{
				return default;
			}

			return JsonSerializer.Deserialize<T>(result.Value);
		}

		public async Task DeleteAllKeyData()
		{
			await _iMemcachedClient.FlushAll();
		}
	}
}
