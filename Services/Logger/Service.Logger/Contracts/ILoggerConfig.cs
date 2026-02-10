namespace Service.Logger.Contracts
{
    public interface ILoggerConfig
    {
        public string ConnectionString { get; set; }
        public string RedisListName { get; set; }
    }
}
