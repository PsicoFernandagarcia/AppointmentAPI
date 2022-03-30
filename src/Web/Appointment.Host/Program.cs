using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Appointment.Host
{
    public class Program
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
                            .AddEnvironmentVariables();
                    }).UseStartup<Startup>();

                    webBuilder.ConfigureKestrel((context, serverOptions) =>
                    {
                        var kestrelSection = context.Configuration.GetSection("Kestrel");

                        serverOptions.Configure(kestrelSection)
                        .Endpoint("HTTPS", listenOptions =>
                        {
                        });
                    });
                });
    }
}
