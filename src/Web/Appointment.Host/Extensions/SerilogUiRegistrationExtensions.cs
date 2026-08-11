using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Ui.Core.Extensions;
using Serilog.Ui.MySqlProvider.Extensions;
using Serilog.Ui.Web.Extensions;

namespace Appointment.Host.Extensions
{
    public static class SerilogUiRegistrationExtensions
    {
        public static IServiceCollection AddSerilogUiServices(this IServiceCollection services, IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("AppointmentConnection");
            services.AddSerilogUi(options =>
            {
                options.UseMySqlServer(opts => opts
                                      .WithConnectionString(cs)
                                      .WithTable("Logs")
                                      );
            });

            return services;
        }

        public static IApplicationBuilder UseSerilogUiDashboard(this IApplicationBuilder app)
        {
            app.UseSerilogUi();
            return app;
        }
    }
}
