using Appointment.Api.Infrastructure.HttpResponses;
using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Application.GCalendar.GetEvents;
using Appointment.Application.UsersUseCase.GetUserByRole;
using Appointment.Domain;
using Appointment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appointment.Api.Controllers
{
    [Route("Api/Calendars")]
    [ApiController]
    public class CalendarsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CalendarsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        [HttpGet("Events")]
        public async Task<IActionResult> GetEvents([FromQuery]GetEventsQuery query)
            => (await _mediator.Send(query)).ToHttpResponse();

    }
}
