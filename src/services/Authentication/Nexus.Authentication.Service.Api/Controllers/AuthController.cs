using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.Authentication.Service.Application.Features.Commands.Login;
using Shared.Contracts.Requests;

namespace Nexus.Authentication.Service.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var command = new LoginCommand(request.Login, request.Password);

            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess:() => Ok(result.Value),
                onFailure: errors => BadRequest(errors));
        }
    }
}