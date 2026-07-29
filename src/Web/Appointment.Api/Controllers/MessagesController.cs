using Appointment.Api.Infrastructure.HttpResponses;
using Appointment.Application.MessageUseCases.CreateMessage;
using Appointment.Application.MessageUseCases.DeleteMessage;
using Appointment.Application.MessageUseCases.GetMessage;
using Appointment.Application.MessageUseCases.UpdateMessage;
using Appointment.Domain;
using Appointment.Domain.Dtos;
using Appointment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Appointment.Api.Controllers
{
    [Route("Api/Messages")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(MessageDto), 200)]
        [Authorize(Roles = "HOST")]
        public async Task<IActionResult> Post([FromBody] CreateMessageCommand command)
        {
            return (await _mediator.Send(command)).ToHttpResponse();
        }

        [HttpPut]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(MessageDto), 200)]
        [Authorize(Roles = "HOST")]
        public async Task<IActionResult> Put([FromBody] UpdateMessageCommand command)
        {
            return (await _mediator.Send(command)).ToHttpResponse();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(MessageDto), 200)]
        [Authorize(Roles = "HOST")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return (await _mediator.Send(new DeleteMessageCommand(id))).ToHttpResponse();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(MessageDto), 200)]
        [OutputCache(PolicyName = CacheKeys.MessagesPolicy)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return (await _mediator.Send(new GetMessageByIdQuery(id))).ToHttpResponse();
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(IEnumerable<MessageDto>), 200)]
        [OutputCache(PolicyName = CacheKeys.MessagesPolicy)]
        public async Task<IActionResult> Get([FromQuery] GetMessagesQuery query)
        {
            query ??= new GetMessagesQuery();
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "COMMON";
            query.ShowAll = query.ShowAll && userRole == "HOST";
            return (await _mediator.Send(query)).ToHttpResponse();
        }
    }
}
