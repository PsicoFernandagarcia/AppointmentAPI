using Application.Integration.Test.Abstractions;
using Appointment.Application.AuthUseCases.Authenticate;
using Appointment.Domain.Entities;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Appointment.Application.AuthUseCases.ResetPassword;
using Appointment.Application.AuthUseCases.CreateUser;

namespace Appointment.Integration.Test.AuthCase
{
    public class CreateUserTest(TestWebApplicationFactory factory) : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Should_Create_User()
        {

            var command = new CreateUserCommand
            {
                Email = "create-user-test@gmail.com",
                UserName = "create-user-test@gmail.com",
                LastName = "LN",
                Name = "Test",
                Password = "25jUkAkc9nh4RUpBsnrbCoWT10F4QxOSgu060zyA98Xg+H10OvRw7WXMMIqmi6rcqf+faYegU5Rgzfy7vMvhYQ==",
                TimezoneOffset = 60,
            };
            var res = await HttpClient.PostAsJsonAsync("api/users", command);
            res.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginCommand = new LoginCommand
            {
                UserName = command.Email,
                Password = command.Password,
                TimezoneOffset = command.TimezoneOffset,
            };

            res = await HttpClient.PostAsJsonAsync("api/auth", loginCommand);
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            await Utilities.DeleteAllModels(factory);
        }

        [Fact]
        public async Task Should_Not_Create_User_Because_Already_Exists()
        {

            var command = new CreateUserCommand
            {
                Email = "test@gmail.com",
                UserName = "test@gmail.com",
                LastName = "LN",
                Name = "Test",
                Password = "C0ij4bYrnop592iCKLDrp2YIeJujgP83oid9xt6IWtw4huOwLCBNHSnSKL3ovTms",
                TimezoneOffset = 60,
            };
            var res = await HttpClient.PostAsJsonAsync("api/users", command);
            res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}
