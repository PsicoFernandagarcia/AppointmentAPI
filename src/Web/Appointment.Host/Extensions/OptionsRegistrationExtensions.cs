using Appointment.Domain.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment.Host.Extensions
{
    public static class OptionsRegistrationExtensions
    {
        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddOptions<ConnectionOptions>().Bind(Configuration.GetSection(ConnectionOptions.SECTION));
            services.AddOptions<AuthOptions>().Bind(Configuration.GetSection(AuthOptions.SECTION));
            services.AddOptions<EmailOptions>().Bind(Configuration.GetSection(EmailOptions.SECTION));
            services.AddOptions<CalendarConfigOptions>().Bind(Configuration.GetSection(CalendarConfigOptions.SECTION));
            return services;
        }
    }
}
