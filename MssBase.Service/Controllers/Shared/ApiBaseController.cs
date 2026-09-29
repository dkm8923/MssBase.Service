using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MssBase.Service.Controllers.Shared
{
    public abstract class ApiBaseController : ControllerBase
    {
        private readonly ILogger _logger;

        protected ApiBaseController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        protected ObjectResult HandleControllerException(HttpContext context, Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception processing {Method} {Path} (TraceId: {TraceId})",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            return Problem(statusCode: 500, title: "An unexpected error occurred.");
        }
    }
}
