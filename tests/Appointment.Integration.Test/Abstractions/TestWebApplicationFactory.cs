using Appointment.Host;
using Appointment.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Testcontainers.MySql;

namespace Application.Integration.Test.Abstractions
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MySqlContainer _dbContainer = new MySqlBuilder()
            .WithImage("mysql:latest")
            .WithDatabase("AppointmentDb")
            .WithUsername("test")
            .WithPassword("test")
            .WithCleanUp(true)
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseMySql(_dbContainer.GetConnectionString(),
                                     ServerVersion.AutoDetect(_dbContainer.GetConnectionString()),
                                     b => b.MigrationsAssembly("Appointment.Host")
                                     );
                });
                services.AddAuthentication()
                .AddJwtBearer("Integrations", x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("AADDFIKCJMLDOCJKAADDFIKCJMLDOCJKAAfdssfdsafsdaDDFIKCJMLDOCJKAADDFIKCJMLDOCJK")),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                });
                services.AddAuthorizationBuilder()
                    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                         .RequireAuthenticatedUser()
                         .AddAuthenticationSchemes("Integrations")
                         .Build());
            });
        }
        public Task InitializeAsync()
        {
            return _dbContainer.StartAsync();
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return _dbContainer.StopAsync();
        }
    }
}
