using Appointment.Api.Infrastructure.HttpResponses;
using Appointment.Application.AuthUseCases.Authenticate;
using Appointment.Application.AuthUseCases.AuthenticateExternal;
using Appointment.Application.AuthUseCases.ResetPassword;
using Appointment.Application.AuthUseCases.SendResetPassCode;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Appointment.Api.Controllers
{
    [Route("Api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOutputCacheStore _cache;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(LoginResult), 200)]
        public async Task<IActionResult> Authenticate([FromBody] LoginCommand login)
            => (await _mediator.Send(login)).ToHttpResponse();

        [HttpPost("External")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(LoginResult), 200)]
        public async Task<IActionResult> AuthenticateExternal([FromBody] LoginExternalCommand login)
            => (await _mediator.Send(login)).ToHttpResponse();


        [HttpPost("ResetPasswordCode")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> GenerateResetPassCode([FromBody] SendResetPassCodeCommand resetPassCode)
           => (await _mediator.Send(resetPassCode)).ToHttpResponse();


        [HttpPut("Password")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> ResetPass([FromBody] ResetPasswordCommand resetPass)
           => (await _mediator.Send(resetPass)).ToHttpResponse();

    }
}
