using System.Net;

namespace PerfomanceLogger.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(
            RequestDelegate next, 
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ExceptionHandling(context, ex);
            }
        }

        public async Task ExceptionHandling(HttpContext context, Exception ex)
        {
            switch (ex)
            {
                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogWarning(ex.Message);
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(ex.Message);
                    break;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { Success=false, Message=ex.Message });
        }

    }
}
