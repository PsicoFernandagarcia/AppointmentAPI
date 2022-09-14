using Appointment.Api.Infrastructure.HttpResponses;
using Appointment.Application.AppointmentUseCases.AddAppointment;
using Appointment.Application.AppointmentUseCases.AddAppointmentByHost;
using Appointment.Application.AppointmentUseCases.CancelAppointment;
using Appointment.Application.AppointmentUseCases.GetAppointmentsByFilter;
using Appointment.Application.AppointmentUseCases.GetMyAppointment;
using Appointment.Application.AppointmentUseCases.GetYearInformation;
using Appointment.Application.AppointmentUseCases.HasAnyAppointment;
using Appointment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appointment.Api.Controllers
{
    [Route("Api/Appointments")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateAppointmentCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(result.Error);
            return Ok();
        }

        [HttpPost("HostAssignments")]
        public async Task<IActionResult> HostAssignments([FromBody] CreateAppointmentByHostCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(result.Error);
            return Ok();
        }


        [HttpGet("Mine")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(IEnumerable<Domain.Entities.Appointment>), 200)]
        public async Task<IActionResult> GetHost([FromQuery] GetMyAppointmentsQuery query)
        {
            query.UserId = int.Parse(User.Identity.Name);
            return (await _mediator.Send(query)).ToHttpResponse();
        }

        [HttpPost("{id}/Cancel")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentsCommand command)
        {
            command.AppointmentId = id;
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(result.Error);
            return Ok();
        }

        [HttpGet()]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(IEnumerable<Domain.Entities.AppointmentDto>), 200)]
        public async Task<IActionResult> Get([FromQuery] GetAppointmentsByFilterQuery query)
        {
            query.UserId = int.Parse(User.Identity.Name);
            return (await _mediator.Send(query)).ToHttpResponse();
        }

        [HttpGet("Any")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(IEnumerable<Domain.Entities.AppointmentDto>), 200)]
        public async Task<IActionResult> Get([FromQuery] HasAnyAppointmentQuery query)
        {
            if (query.PatientId <= 0)
                query.PatientId = int.Parse(User.Identity.Name);
            return (await _mediator.Send(query)).ToHttpResponse();
        }

        [HttpGet("Information")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(IEnumerable<AppointmentYearInformationDto>), 200)]
        public async Task<IActionResult> GetYearInformation([FromQuery] GetYearInformationQuery query)
        {
            return (await _mediator.Send(query)).ToHttpResponse();
        }
    }
}