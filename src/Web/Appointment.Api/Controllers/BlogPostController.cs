using Appointment.Api.Infrastructure.HttpResponses;
using Appointment.Application.BlogPostUseCases.CreateBlogPost;
using Appointment.Application.BlogPostUseCases.GetBlogPost;
using Appointment.Domain.Dtos;
using Appointment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Appointment.Api.Controllers
{
    [Route("Api/Blog")]
    [ApiController]
    [Authorize]
    public class BlogPostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlogPostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(BlogPost), 200)]
        [Authorize(Roles = "HOST")]
        public async Task<IActionResult> Post([FromBody] CreateBlogPostCommand command)
        {
            return (await _mediator.Send(command)).ToHttpResponse();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(BlogPostDto), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromRoute]GetBlogPostByIdQuery query)
        {
            return (await _mediator.Send(query)).ToHttpResponse();
        }

        [HttpGet()]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(BlogPostDto), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetBlogPostsQuery query)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "COMMON";
            query.ShowAll = query.ShowAll && userRole == "HOST";
            return (await _mediator.Send(query)).ToHttpResponse();
        }



    }
}
