using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangePassword;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordConfirmCode;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.ChangePasswordInit;
using Shared.Contracts;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class PasswordResetController(IMediator mediator) : Controller
    {
        [HttpPost("{login}/password/reset/init")]
        public async Task<IActionResult> InitPasswordReset([FromRoute] string login)
        {
            var command = new ResetPasswordSendCodeCommand(login);
            var result = await mediator.Send(command);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok();
        }

        [HttpPost("{login}/password/reset/confirm")]
        public async Task<IActionResult> ConfirmPasswordReset([FromRoute] string login, [FromBody] ConfirmCodeRequest request)
        {
            var command = new ResetPasswordConfirmCodeCommand(login, request.Code);
            var result = await mediator.Send(command);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok();
        }

        [HttpPost("{login}/password/reset/complete")]
        public async Task<IActionResult> CompletePasswordReset([FromRoute] string login, [FromBody] RecoveryPasswordRequest request)
        {
            var command = new ResetPasswordCommand(
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

       [HttpGet("{userId:guid}/password/change/init")]
        public async Task<IActionResult> InitPasswordChange([FromRoute] Guid userId)
        {
            var command = new ChangePasswordInitQuery(userId);

            var result = await mediator.Send(command);
            
             if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok(result.Value);
        }

        [HttpPost("{userId:guid}/password")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid userId, [FromBody] ChangePasswordRequest request)
        {
            var command = new ChangePasswordCommand(
                userId, 
                request.EncryptedVerifier, 
                request.SrpSalt, 
                request.SrpVersion, 
                request.EncryptedVerifierWrapKey, 
                request.KeyWrapVersion, 
                request.AsymmetricKeyId, 
                request.EncryptedDek,
                request.DekSalt,
                request.CryptoVersion);
            
            var result = await mediator.Send(command);
            
            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok();
        }
    }
}