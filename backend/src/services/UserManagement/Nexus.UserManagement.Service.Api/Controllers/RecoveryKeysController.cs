using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeys;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetRecoveryKeys;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class RecoveryKeysController(IMediator mediator) : Controller
    {
        [HttpGet("{login}/recovery-keys")]
        public async Task<IActionResult> GetRecoveryKeys([FromRoute] string login)
        {
            var command = new GetRecoveryKeysQuery(login);

            var result = await mediator.Send(command);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            var response = new RecoveryViaKeysPayloadResponse(result.Value.RecoveryKeys);

            return Ok(response);
        }

       [HttpPost("{login}/recovery-keys")]
        public async Task<IActionResult> SetRecoveryKeys([FromRoute] string login, [FromBody] RecoveryViaKeysRequest request)
        {
            var command = new RecoveryViaKeysCommand(
                request.Login, 
                request.EncryptedVerifier, 
                request.SrpSalt, 
                request.SrpVersion, 
                request.EncryptedVerifierWrapKey, 
                request.KeyWrapVersion, 
                request.AsymmetricKeyId, 
                request.EncryptedDek, 
                request.DekSalt,
                request.CryptoVersion, 
                [.. request.RecoveryKeys.Select(x => new RecoveryKeyCommandData(x.EncryptedValue, x.CryptoVersion))]);
        
            var result = await mediator.Send(command);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok();
        }
    }
}