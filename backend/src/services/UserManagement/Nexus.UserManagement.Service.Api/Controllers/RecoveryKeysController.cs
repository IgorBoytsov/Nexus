using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeysSet;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.RecoveryViaKeysInit;
using Shared.Contracts;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class RecoveryKeysController(IMediator mediator) : Controller
    {
        [HttpGet("{login}/recovery-keys/init")]
        public async Task<IActionResult> InitRecoveryKeys([FromRoute] string login)
        {
            var command = new RecoveryViaKeysInitQuery(login);

            var result = await mediator.Send(command);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            var response = new RecoveryViaKeysPayloadResponse(result.Value.RecoveryKeys);

            return Ok(response);
        }

       [HttpPost("{login}/recovery-keys")]
        public async Task<IActionResult> SetRecoveryKeys([FromRoute] string login, [FromBody] RecoveryViaKeysSetRequest request)
        {
            var command = new RecoveryViaKeysSetCommand(
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