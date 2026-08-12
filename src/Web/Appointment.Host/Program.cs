using Appointment.Host.Schedule;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Appointment.Host
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .Build())
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext())
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    webBuilder.ConfigureAppConfiguration((builderContext, config) =>
                    {
                        var env = builderContext.HostingEnvironment;
                        config.AddJsonFile("./appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"./appsettings.{env.EnvironmentName}.json", optional: true,
                                reloadOnChange: true)
                            .AddJsonFile($"./configs/appsettings-prod.json", optional: true,
                                reloadOnChange: true)
                            .AddEnvironmentVariables();
                    }).UseStartup<Startup>();
                })
            .ConfigureServices(services => { services.AddHostedService<BackgroundWorker>(); })
            ;
    }
}
