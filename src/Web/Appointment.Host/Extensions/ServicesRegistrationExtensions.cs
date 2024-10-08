using Appointment.Api.Infrastructure;
using Appointment.Application.SendEmailUseCase;
using Appointment.Domain.Interfaces;
using Appointment.Infrastructure.Configuration;
using Appointment.Infrastructure.Repositories;
using Appointment.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment.Host.Extensions
{
    public static class ServicesRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(new HealthCheckCircuitBreakerService());
            services.AddSingleton<IServiceAccountSingleton,ServiceAccountSingleton>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IAppointmentRepository, AppointmentRepository>();
            services.AddTransient<IAvailabilityRepository, AvailabilityRepository>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
            services.AddTransient<IBlogRepository, BlogRepository>();
            services.AddTransient<IEmailSender, Sender>();
            services.AddTransient<ICrypt, Crypt>();
            return services;
        }
    }

}
