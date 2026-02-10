using Service.Logger.Contracts;
using Service.Logger.Dto;

namespace Service.Logger.Models
{
    public class LoggerServiceTestStub : ILoggerService
    {
        public Task Log(InsertLoggerRequest req)
        {
            Console.WriteLine("LoggerServiceTestStub.Log()");
            return Task.CompletedTask;
        }
    }
}
