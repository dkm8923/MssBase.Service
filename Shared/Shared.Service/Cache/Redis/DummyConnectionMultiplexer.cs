using StackExchange.Redis;
using StackExchange.Redis.Maintenance;
using StackExchange.Redis.Profiling;
using System.Net;

namespace Shared.Service.Cache.Redis
{
    public class DummyConnectionMultiplexer : IConnectionMultiplexer
    {
        public void Dispose() { }
        public IDatabase GetDatabase(int db = -1, object asyncState = null) => null;
        public IServer GetServer(string host, int port, object asyncState = null) => null;
        public IServer GetServer(string hostAndPort, object asyncState = null) => null;
        public IServer GetServer(EndPoint endpoint, object asyncState = null) => null;
        public IServer GetServer(RedisKey key, object? asyncState = null, CommandFlags flags = CommandFlags.None) => null;
        public EndPoint[] GetEndPoints(bool configuredOnly = false) => new EndPoint[0];
        public void GetStatus(TextWriter log = null) { }
        public void GetStatus(IBatch batch, TextWriter log = null) { }
        public void Wait(Task task) { }
        public T Wait<T>(Task<T> task) => default;
        public void WaitAll(params Task[] tasks) { }

        public void RegisterProfiler(Func<ProfilingSession?> profilingSessionProvider)
        {
            throw new NotImplementedException();
        }

        public ServerCounters GetCounters()
        {
            throw new NotImplementedException();
        }

        public int HashSlot(RedisKey key)
        {
            throw new NotImplementedException();
        }

        public ISubscriber GetSubscriber(object? asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(IPAddress host, int port)
        {
            throw new NotImplementedException();
        }

        public IServer[] GetServers()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfigureAsync(TextWriter? log = null)
        {
            throw new NotImplementedException();
        }

        public bool Configure(TextWriter? log = null)
        {
            throw new NotImplementedException();
        }

        public string GetStatus()
        {
            throw new NotImplementedException();
        }

        public void Close(bool allowCommandsToComplete = true)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(bool allowCommandsToComplete = true)
        {
            throw new NotImplementedException();
        }

        public string? GetStormLog()
        {
            throw new NotImplementedException();
        }

        public void ResetStormLog()
        {
            throw new NotImplementedException();
        }

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public int GetHashSlot(RedisKey key)
        {
            throw new NotImplementedException();
        }

        public void ExportConfiguration(Stream destination, ExportOptions options = (ExportOptions)(-1))
        {
            throw new NotImplementedException();
        }

        public void AddLibraryNameSuffix(string suffix)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public int TimeoutMilliseconds => 0;
        public long OperationCount => 0;
        public bool IsConnected => false;
        public bool IsConnecting => false;
        public string ClientName => "Dummy";
        public string Configuration => "Dummy";
        public int StormLogThreshold { get; set; }
        public bool PreserveAsyncOrder { get; set; }
        public bool IncludeDetailInExceptions { get; set; }
        public int ConnectTimeout { get; set; }
        public int SyncTimeout { get; set; }
        public bool IsDisposed => false;
        public event EventHandler<RedisErrorEventArgs> ErrorMessage;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<InternalErrorEventArgs> InternalError;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored;
        public event EventHandler<EndPointEventArgs> ConfigurationChanged;
        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast;
        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved;
        public event EventHandler<ServerMaintenanceEvent> ServerMaintenance;
        public event EventHandler<ServerMaintenanceEvent> ServerMaintenanceEnded;
        public event EventHandler<ServerMaintenanceEvent> ServerMaintenanceEvent;
    }
}
