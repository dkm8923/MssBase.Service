using Service.Logger.Dto;

namespace Service.Logger.Contracts
{
    public interface ILoggerService
    {
        public Task Log(InsertLoggerRequest req);
    }
}
