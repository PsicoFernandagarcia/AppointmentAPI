using Appointment.Host.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment.Host.Extensions
{
    public static class MediatRRegistrationExtzensions
    {
        public static IServiceCollection AddMediatRConfigurations(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Application).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return services;
        }
    }
}