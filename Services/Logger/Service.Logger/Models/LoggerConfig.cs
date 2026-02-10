using Service.Logger.Contracts;

namespace Service.Logger.Models
{
    public record LoggerConfig : ILoggerConfig
    {
        public string ConnectionString { get; set; }
        public string RedisListName { get; set; }
    }
}
