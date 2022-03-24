using System.Threading.Tasks;
using Appointment.Api.Infrastructure.HttpResponses;
using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Application.AvailabilityUseCases.CreateAvailability;
using Appointment.Application.AvailabilityUseCases.GetAvailability;
using Appointment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Appointment.Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class AvailabilitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AvailabilitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Post([FromBody] CreateAvailabilityCommand command)
        {
            command.HostId = int.Parse(User.Identity.Name);
            return (await _mediator.Send(command)).ToHttpResponse();
        }
        

        [HttpPost("bulk")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> PostMany([FromBody] CreateAvailabilitiesCommand command)
        {
            command.HostId = int.Parse(User.Identity.Name);
            foreach (var availability in command.Availabilities)
            {
                availability.HostId = command.HostId;
            }
            return (await _mediator.Send(command)).ToHttpResponse();
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get( [FromQuery]GetAvailabilityQuery query)
            => (await _mediator.Send(query)).ToHttpResponse();

        [HttpGet("mine")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> GetMine([FromQuery] GetMyAvailabilityQuery query)
        {
            var queryToSend = new GetAvailabilityQuery();
            queryToSend.HostId= int.Parse(User.Identity.Name);
            queryToSend.DateTo = query.DateTo;
            queryToSend.DateFrom = query.DateFrom;
            return (await _mediator.Send(queryToSend)).ToHttpResponse();
        }

    }
}