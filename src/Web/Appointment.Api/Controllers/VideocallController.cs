using Appointment.Api.Infrastructure.HttpResponses;
using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Application.Videocall;
using Appointment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Appointment.Api.Controllers
{
    [Route("Api/Videocall")]
    [ApiController]
    [Authorize]
    public class VideocallController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VideocallController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Token([FromQuery]GetTokenQuery query)
        {
            return (await _mediator.Send(query)).ToHttpResponse();
        }
    }
}
