using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.Authentication.Service.Application.Features.Commands.Login;
using Nexus.Authentication.Service.Application.Features.Commands.LoginByToken;
using Nexus.Authentication.Service.Application.Features.Commands.Refresh;
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

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);
            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess: () => Ok(result.Value),
                onFailure: errors => BadRequest(errors));
        }

        [HttpPost("token-login")]
        public async Task<IActionResult> TokenLogin([FromBody] TokenLoginRequest request)
        {
            var command = new TokenLoginCommand(request.RefreshToken);
            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess: () => Ok(result.Value),
                onFailure: errors => BadRequest(errors));
        }
    }
}