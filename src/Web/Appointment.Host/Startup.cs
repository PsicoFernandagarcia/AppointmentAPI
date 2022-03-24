using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appointment.Host.Extensions;
using Appointment.Infrastructure.Configuration;

namespace Appointment.Host
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;

        }

        public void ConfigureServices(IServiceCollection services) =>
            services.AddServices()
                .AddOptions(Configuration)
                .AddAuth(Configuration)
                .AddInfrastructure(Configuration)
                .AddVersioningConfigurations()
                .AddHttpClientRegistration()
                .AddHealthChecksConfigurations(Configuration)
                .AddSwaggerConfigurations()
                .AddMediatRConfigurations();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext context) =>
            app.UseMiddleware(env)
                .UseInfrastructure(env,context)
                .UseSwaggerConfigurations(env)
                .UseHealthCheckConfigurations()
                .UseCorsConfigurations();
    }
}
