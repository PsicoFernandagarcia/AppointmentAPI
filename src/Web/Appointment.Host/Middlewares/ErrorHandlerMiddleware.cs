using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Appointment.Host.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public ErrorHandlerMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment,
            ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                LogRequest(context, ex);
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = 500;
                await response.WriteAsync(JsonSerializer.Serialize(new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://httpstatuses.com/500",
                    Title = ex.Message,
                    Detail = _webHostEnvironment.EnvironmentName == "Development" ? ex.StackTrace : string.Empty,
                    Instance = response.HttpContext.Request.Path
                }));
            }
        }
        private void LogRequest(HttpContext context, Exception ex)
        {
            _logger.LogError(@"Http Request Global Exception
                                   Schema:{RequestScheme}
                                   Host: {RequestHost}
                                   Path: {RequestPath}
                                   QueryString: {RequestQueryString} 
                                   Exception: {@ExceptionDetail}",
                                   context.Request.Scheme,
                                   context.Request.Host,
                                   context.Request.Path,
                                   context.Request.QueryString,
                                   FlattenException(ex));
        }

        private string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}
