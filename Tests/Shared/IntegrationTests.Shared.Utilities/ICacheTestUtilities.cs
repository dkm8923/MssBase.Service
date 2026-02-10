using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Shared.Utilities
{
    public interface ICacheTestUtilities
    {
        public List<string> GetKeys();
        public void ConfigureCache(ServiceCollection services);
        public Task<T?> GetKeyData<T>(string keyName);
        public Task DeleteAllKeyData();
    }
}
