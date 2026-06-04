using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.Register;
using Shared.Contracts;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class RegistrationController(IMediator mediator) : Controller
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var command = new RegisterCommand(
                request.Login,
                request.UserName, 
                request.Email, 
                string.IsNullOrWhiteSpace(request.IdGender) ? null : Guid.Parse(request.IdGender), 
                string.IsNullOrWhiteSpace(request.IdCountry) ? null : Guid.Parse(request.IdCountry),
                request.EncryptedVerifier, 
                request.SrpSalt,
                request.SrpVersion, 
                request.EncryptedVerifierWrapKey, 
                request.KeyWrapVersion, 
                request.AsymmetricKeyId, 
                request.EncryptedDek, 
                request.DekSalt, 
                request.CryptoVersion, 
                [.. request.RecoveryKeys.Select(rk => new RecoveryKeyCommandData(rk.EncryptedValue, rk.CryptoVersion))]);

            var result = await mediator.Send(command);

            return result.Match(
                onSuccess: () => Ok(new { Message = "Регистрация прошла успешно!" }),
                onFailure: errors => this.MapActionResult(errors));
        }
    }
}