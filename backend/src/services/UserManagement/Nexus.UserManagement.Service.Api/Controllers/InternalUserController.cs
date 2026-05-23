using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.UserInternal.Queries.GetByLoginInternal;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api.Controllers
{
    [ApiController]
    [Route("internal/api/users")]
    public class InternalUserController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("by-login/{login}")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromRoute] string login)
        {
            var command = new GetUserByLoginInternalQuery(login);

            var result = await _mediator.Send(command);

            return result.Match(
                onSuccess: Ok,
                onFailure: errors => this.MapActionResult(errors));
        }

        [HttpGet("by-id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginById([FromRoute] Guid id)
        {
            var command = new GetUserByIdQuery(id);

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
