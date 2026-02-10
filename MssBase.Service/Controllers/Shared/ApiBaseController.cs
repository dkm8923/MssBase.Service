using Microsoft.AspNetCore.Mvc;
using Service.Logger.Contracts;
using Service.Logger.Dto;

namespace MssBase.Service.Controllers.Shared
{
    public abstract class ApiBaseController : ControllerBase
    {
        private ILoggerService _loggerSvc;

        protected ILoggerService loggerSvc => _loggerSvc ?? (HttpContext.RequestServices.GetService<ILoggerService>());

        protected async Task LogControllerException(HttpContext context, Exception ex)
        {
            var fullUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            var errorMsg = $"Action: {context.Request.Method}, URL: {fullUrl}, Exception: {ex.Message}";
            loggerSvc.Log(new InsertLoggerRequest { ApplicationMessage = errorMsg });
        }

        protected ObjectResult HandleControllerException(HttpContext context, Exception ex)
        {
            LogControllerException(context, ex);
            return StatusCode(500, ex.Message);
        }
    }
}
