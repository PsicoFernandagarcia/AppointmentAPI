using System;
using System.Linq;
using System.Text.Json.Serialization;
using Appointment.Api.Controllers;
using Appointment.Infrastructure.Configuration;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment.Host.Extensions
{
    public static class InfrastructureRegistrationExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            var connectionString = configuration.GetConnectionString("AppointmentConnection");
            services.AddDbContext<AppDbContext>(
                options => options.UseLazyLoadingProxies().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly("Appointment.Host")));
            services.AddMvc()
                .AddNewtonsoftJson()
                .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = c =>
                {
                    var errors = string.Join("  -  ", c.ModelState.Values.Where(v => v.Errors.Count > 0)
                        .SelectMany(v => v.Errors)
                        .Select(v => v.ErrorMessage));

                    return new BadRequestObjectResult(new
                    {
                        ErrorCode = "Your validation error code",
                        Message = errors
                    });
                };
            });

            services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    })
                .AddApplicationPart(typeof(AuthController).Assembly)
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssemblyContaining<Application.Application>();
                    c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
                });
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app,
            IWebHostEnvironment env,AppDbContext context)
        {
            app .UseHttpsRedirection()
                .UseRouting()
                .UseCors()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            context.Database.Migrate();
            
            return app;
        }
    }
}
