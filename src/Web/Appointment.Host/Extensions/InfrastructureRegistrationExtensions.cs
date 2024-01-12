using Appointment.Api.Controllers;
using Appointment.Infrastructure.Configuration;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Appointment.Domain;

namespace Appointment.Host.Extensions
{
    public static class InfrastructureRegistrationExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            var connectionString = configuration.GetConnectionString("AppointmentConnection");
            services.AddDbContext<AppDbContext>(
                options =>
                {
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly("Appointment.Host"));

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
            services.AddCache();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    })
                    .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = c =>
                    {
                        var errors = string.Join("  -  ", c.ModelState.Values.Where(v => v.Errors.Count > 0)
                            .SelectMany(v => v.Errors)
                            .Select(v => v.ErrorMessage));

                        return new BadRequestObjectResult(new
                        {
                            ErrorCode = HttpStatusCode.BadRequest,
                            Message = errors
                        });
                    };
                })
                .AddApplicationPart(typeof(AuthController).Assembly);

                
            services.AddSwaggerConfigurations()
                    .AddFluentValidationAutoValidation()
                    .AddFluentValidationClientsideAdapters()
                    .AddValidatorsFromAssemblyContaining<Application.Application>();

            return services;
        }

        public static IServiceCollection AddCache(this IServiceCollection services)
            => services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder => builder.NoCache());
                options.AddPolicy(CacheKeys.UsersPolicy, builder => 
                        builder.Expire(TimeSpan.FromMinutes(15))
                               .Tag(CacheKeys.Users)
                               .AddPolicy<OutputCachePolicy>()
                        );

                options.AddPolicy(CacheKeys.PaymentsPolicy, builder =>
                                       builder.Expire(TimeSpan.FromMinutes(15))
                                              .Tag(CacheKeys.Payments)
                                              .AddPolicy<OutputCachePolicy>()
                                       );

                options.AddPolicy(CacheKeys.AvailabilitiesPolicy, builder =>
                                      builder.Expire(TimeSpan.FromMinutes(15))
                                             .Tag(CacheKeys.Availabilities)
                                             .AddPolicy<OutputCachePolicy>()
                                      );

                options.AddPolicy(CacheKeys.AppointmentsPolicy, builder =>
                                      builder.Expire(TimeSpan.FromMinutes(15))
                                             .Tag(CacheKeys.Appointments)
                                             .AddPolicy<OutputCachePolicy>()
                                      );
            });

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app,
            IWebHostEnvironment env, AppDbContext context)
        {
            app.UseHttpsRedirection()
                .UseRouting()
                .UseCors()
                .UseAuthentication()
                .UseAuthorization()
                .UseOutputCache()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                ;
            context.Database.Migrate();

            return app;
        }
    }

    public class OutputCachePolicy : IOutputCachePolicy
    {

        public static readonly OutputCachePolicy Instance = new();

        public OutputCachePolicy()
        {
        }

        ValueTask IOutputCachePolicy.CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            var attemptOutputCaching = AttemptOutputCaching(context);
            context.EnableOutputCaching = true;
            context.AllowCacheLookup = attemptOutputCaching;
            context.AllowCacheStorage = attemptOutputCaching;
            context.AllowLocking = true;

            // Vary by any query by default
            context.CacheVaryByRules.QueryKeys = "*";
            return ValueTask.CompletedTask;
        }
        // this never gets hit when Authorization is present
        ValueTask IOutputCachePolicy.ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            return ValueTask.CompletedTask;
        }

        ValueTask IOutputCachePolicy.ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
        {
            var response = context.HttpContext.Response;
            context.AllowCacheStorage = true;

            return ValueTask.CompletedTask;
        }

        private static bool AttemptOutputCaching(OutputCacheContext context)
        {
            // Check if the current request fulfills the requirements to be cached

            var request = context.HttpContext.Request;

            // Verify the method
            if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
            {
                return false;
            }

            // Verify existence of authorization headers
            //if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) || request.HttpContext.User?.Identity?.IsAuthenticated == true)
            //{
            //    return false;
            //}
            return true;
        }
    }
}
