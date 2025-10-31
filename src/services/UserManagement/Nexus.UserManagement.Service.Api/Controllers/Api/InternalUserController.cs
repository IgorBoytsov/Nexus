using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetByLoginInternal;

namespace Nexus.UserManagement.Service.Api.Controllers.Api
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

            return result.Match<IActionResult>(
                onSuccess: Ok,
                onFailure: errors =>
                {
                    return Unauthorized(new
                    {
                        Title = "Не валидные данные",
                        Message = result.StringMessage
                    });

                });
        }
    }
}
