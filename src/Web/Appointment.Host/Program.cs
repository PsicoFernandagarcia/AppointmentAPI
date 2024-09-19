using Appointment.Host.Schedule;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Appointment.Host
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
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
