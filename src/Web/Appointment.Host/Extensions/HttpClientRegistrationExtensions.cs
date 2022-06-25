using Appointment.Api.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Appointment.Host.Extensions
{
    public static class HttpClientRegistrationExtensions
    {
        private static ILogger<Startup> _logger;
        private static HealthCheckCircuitBreakerService _healthCheckCircuitBreakerService;

        public static IServiceCollection AddHttpClientRegistration(this IServiceCollection services)
        {
            _logger = services.BuildServiceProvider(true).GetRequiredService<ILogger<Startup>>();

            _healthCheckCircuitBreakerService = services.BuildServiceProvider().GetRequiredService<HealthCheckCircuitBreakerService>();

            return services;

        }

        private static void OnHalfOpen()
        {
            _logger.LogInformation($"CircuitBreaker OnHalfOpen");
        }

        private static void OnReset()
        {
            _logger.LogInformation($"CircuitBreaker OnReset");

            _healthCheckCircuitBreakerService.Healthy();
        }

    }
}
