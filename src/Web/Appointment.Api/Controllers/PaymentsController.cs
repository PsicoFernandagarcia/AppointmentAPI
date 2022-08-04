using Appointment.Api.Infrastructure.HttpResponses;
using Appointment.Application.PaymentUseCases.AddPayment;
using Appointment.Application.PaymentUseCases.GetLatestsPaymentsByHost;
using Appointment.Application.PaymentUseCases.GetPaymentsFromPatientByHost;
using Appointment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Appointment.Api.Controllers
{
    [Route("Api/Payments")]
    [ApiController]
    //[Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Post(CreatePaymentCommand command)
        {
            if (command.HostId == 0)
                command.HostId = int.Parse(User.Identity.Name);
            return (await _mediator.Send(command)).ToHttpResponse();
        }

        [HttpGet("Latest")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> GetLatests([FromQuery] GetLatestsPaymentsByHostQuery query)
        {
            if (query.HostId == 0)
                query.HostId = int.Parse(User.Identity.Name);
            return (await _mediator.Send(query)).ToHttpResponse();
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get([FromQuery] GetPaymentsFromPatientByHostQuery query)
        {
            if (query.HostId == 0)
                query.HostId = int.Parse(User.Identity.Name);
            return (await _mediator.Send(query)).ToHttpResponse();
        }

    }
}
