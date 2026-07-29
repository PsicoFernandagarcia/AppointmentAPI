using Application.Integration.Test.Abstractions;
using Appointment.Application.AuthUseCases.Authenticate;
using Appointment.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Appointment.Application.AuthUseCases.ResetPassword;
using Appointment.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Appointment.Domain.Infrastructure;
using Appointment.Application.AuthUseCases.SendResetPassCode;

namespace Appointment.Integration.Test.AuthCase
{
    public class ResetPasswordTest(TestWebApplicationFactory factory) : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Should_Reset_Password()
        {
            var user = Utilities.UserCommon;
            var sendResetPassCommand = new SendResetPassCodeCommand { Email = user.Email };
            await HttpClient.PostAsJsonAsync("api/auth/ResetPasswordCode", sendResetPassCommand);

            var code = await Utilities.GetResetPassCode(factory, user.Email);

            var cripto = new Crypt(Options.Create(new AuthOptions { HashValue = "psicoe84ad660c4721ae0e84ad660c4721ae0fer" }));
            var loginCommand = new LoginCommand
            {
                UserName = user.Email,
                Password = cripto.EncryptStringToBytes_Aes("test"),
                TimezoneOffset = 60,
            };
            var res = await HttpClient.PostAsJsonAsync("api/auth", loginCommand);
            res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var resetPassCommand = new ResetPasswordCommand
            {
                Email = user.Email,
                Code = code,
                NewPassword =cripto.EncryptStringToBytes_Aes("test"),

            };
            res = await HttpClient.PutAsJsonAsync("api/auth/Password", resetPassCommand);
            res.StatusCode.Should().Be(HttpStatusCode.OK);


            res = await HttpClient.PostAsJsonAsync("api/auth", loginCommand);
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            await Utilities.DeleteAllModels(factory);
        }

        [Fact]
        public async Task ShouldNot_ResetPassword_BecauseOfInvalidCode()
        {
            var user = Utilities.UserCommon;

            var resetPassCommand = new ResetPasswordCommand
            {
                Email = user.Email,
                Code = 1234,
                NewPassword = "C0ij4bYrnop592iCKLDrp2YIeJujgP83oid9xt6IWtw4huOwLCBNHSnSKL3ovTms",

            };
            var res = await HttpClient.PutAsJsonAsync("api/auth/Password", resetPassCommand);
            res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var br = await res.ToBadRequestObject();
            br.Message.Should().Be("El código ingresado no es válido para ese email, intente generar uno nuevo");
            await Utilities.DeleteAllModels(factory);

        }
    }
}
