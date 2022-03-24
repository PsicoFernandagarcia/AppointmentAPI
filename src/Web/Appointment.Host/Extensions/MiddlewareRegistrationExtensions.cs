using Appointment.Host.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Appointment.Host.Extensions
{
    public static class MiddlewareRegistrationExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
            return app;
        }

    }
}