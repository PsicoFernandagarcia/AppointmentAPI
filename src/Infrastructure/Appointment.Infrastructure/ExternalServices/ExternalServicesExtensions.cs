using Appointment.Domain.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Refit;
using System;

namespace Appointment.Infrastructure.ExternalServices
{
    public static class ExternalServicesExtensions
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration Configuration)
        {
            var agoraURI = Configuration.GetSection("Agora:uri").Value;
            services
               .AddRefitClient<IAgoraClient>()
               .ConfigureHttpClient(c => c.BaseAddress = new Uri(agoraURI));
            return services;
        }
    }
}
