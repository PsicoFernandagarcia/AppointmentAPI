using Appointment.Host.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment.Host.Extensions
{
    public static class MediatRRegistrationExtzensions
    {
        public static IServiceCollection AddMediatRConfigurations(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Application.Application));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return services;
        }
    }
}