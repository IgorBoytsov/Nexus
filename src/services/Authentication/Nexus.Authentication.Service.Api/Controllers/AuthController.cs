using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.Authentication.Service.Application.Features.Commands.LoginByToken;
using Nexus.Authentication.Service.Application.Features.Commands.Refresh;
using Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge;
using Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof;
using Shared.Contracts.Authentication.Requests;
using Shared.Kernel.Results;

namespace Nexus.Authentication.Service.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("srp/challenge")]
        public async Task<IActionResult> GetChallenge([FromBody] SrpChallengeRequest request)
        {
            var command = new GetSrpChallengeCommand(request.Login);
            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess: () => Ok(result.Value),
                onFailure: errors => BadRequest(new { Title = "Ошибка инициализации входа", Errors = errors }));
        }

        [HttpPost("srp/verify")]
        public async Task<IActionResult> VerifyProof([FromBody] SrpVerifyRequest request)
        {
            var command = new VerifySrpProofCommand(request.Login, request.A, request.M1);
            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess: () => Ok(result.Value),
                onFailure: errors =>
                {
                    var error = errors.FirstOrDefault();
                    if (error!.Code == ErrorCode.InvalidPassword || error.Code == ErrorCode.NotFound)
                        return Unauthorized(new { Title = "Ошибка аутентификации", Detail = error.Message });

                    return BadRequest(new { Title = "Ошибка проверки", Errors = errors });
                });
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