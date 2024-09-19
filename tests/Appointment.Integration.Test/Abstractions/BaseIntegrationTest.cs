using Appointment.Domain.Entities;
using Google.Apis.Util;
using Google;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Appointment.Infrastructure.Configuration;

namespace Application.Integration.Test.Abstractions
{
    public class BaseIntegrationTest: IClassFixture<TestWebApplicationFactory>
    {
        public HttpClient HttpClient { get; init; }
        public BaseIntegrationTest(TestWebApplicationFactory factory)
        {
            using (var scope = factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                Utilities.InitializeDbForTests(db);
            }
            HttpClient = factory.CreateClient();
            SetAuth();
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes("AADDFIKCJMLDOCJKAADDFIKCJMLDOCJKAAfdssfdsafsdaDDFIKCJMLDOCJKAADDFIKCJMLDOCJK");
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role,string.Join(',',user.Roles.Select(x=>x.Name))),
                }),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        private User SetAuth()
        {
            var user = Utilities.UserHost;
            var token = GenerateJwtToken(user);
            HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return user;
        }

        protected User SetAuthForCommonUser()
        {
            var user = Utilities.UserCommon;
            var token = GenerateJwtToken(user);
            HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return user;
        }
    }
}
