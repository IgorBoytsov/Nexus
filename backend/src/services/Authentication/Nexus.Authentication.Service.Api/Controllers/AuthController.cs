using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.Authentication.Service.Application.Features.Commands.Refresh;
using Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge;
using Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Web.Extensions;

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

            return result.Match(
                onSuccess: () => Ok(result.Value),
                onFailure: errors => this.MapActionResult(errors));
        }

        [HttpPost("srp/verify")]
        public async Task<IActionResult> VerifyProof([FromBody] SrpVerifyRequest request)
        {
            var command = new VerifySrpProofCommand(request.Login, request.A, request.M1);
            var result = await _mediator.Send(command);

            return result.Match(
                onSuccess: () => Ok(result.Value),
                onFailure: errors => this.MapActionResult(errors));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] Shared.Contracts.Authentication.Requests.RefreshTokensRequest request)
        {
            var command = new RefreshTokenCommand(request.RefreshToken, request.AccessToken);
            var result = await _mediator.Send(command);

            return result.Match(
                onSuccess: () => Ok(result.Value),
                onFailure: errors => this.MapActionResult(errors));
        }
    }
}