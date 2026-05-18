using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.Register;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordConfirmCode;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo;
using Crossdyne.Toolkit.Results;
using Shared.Contracts;
using System.Security.Claims;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api.Controllers
{
    public sealed record RecoveryAccessRequest(string Login, string Email, string NewPassword);

    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public UserController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var command = new RegisterCommand(
                request.Login, request.UserName, 
                request.Verifier, request.ClientSalt, request.EncryptedVerifierWrapKey, 
                request.CryptoVersion, request.SrpVersion, 
                request.EncryptedVerifierWrapKey, request.KeyWrapVersion, request.AsymmetricKeyId, 
                request.Email, 
                string.IsNullOrWhiteSpace(request.IdGender) ? null : Guid.Parse(request.IdGender), 
                string.IsNullOrWhiteSpace(request.IdCountry) ? null : Guid.Parse(request.IdCountry));

            var result = await _mediator.Send(command);

            return result.Match(
                onSuccess: () => Ok(new { Message = "Регистрация прошла успешно!" }),
                onFailure: errors => this.MapActionResult(errors));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("User ID не найден в токене.");

            if (!Guid.TryParse(userIdString, out var userId))
                return BadRequest("Не верный User ID формат.");

            var result = await _mediator.Send(new GetUserByIdQuery(userId));

            return result.Match<IActionResult>(
                onSuccess: () => Ok(result.Value),
                onFailure: errors =>
                {
                    if (errors.Any(e => e.Code == ErrorCode.Server))
                    {
                        var serverError = errors.FirstOrDefault(e => e.Code == ErrorCode.Save);
                        return StatusCode(StatusCodes.Status500InternalServerError, new
                        {
                            Tittle = "Внутренняя ошибка сервера",
                            Details = serverError?.Message,
                        });
                    }
                    return BadRequest(result.StringMessage);
                });
        }

        [HttpGet("public-encryption-info/{login}")]
        public async Task<IActionResult> GetPublicEncryptionInnfo([FromRoute] string login)
        {
            var result = await _mediator.Send(new GetPublicEncryptionInfoQuery(login));

            return result.Match<IActionResult>(
                onSuccess: () => Ok(result.Value),
                onFailure: errors =>
                {
                    if (errors.Any(e => e.Code == ErrorCode.Server))
                    {
                        var serverError = errors.FirstOrDefault(e => e.Code == ErrorCode.Save);
                        return StatusCode(StatusCodes.Status500InternalServerError, new
                        {
                            Tittle = "Внутренняя ошибка сервера",
                            Details = serverError?.Message,
                        });
                    }
                    return BadRequest(result.StringMessage);
                });
        }

        [HttpGet("profile-info/{userId}")]
        public async Task<IActionResult> GetProfileInfo([FromRoute] string userId)
        {
            var result = await _mediator.Send(new GetProfileInfoQuery(Guid.Parse(userId)));

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [HttpPost("recovery-password/send-code/{login}")]
        public async Task<IActionResult> SendConfirmCodeEmail(string login)
        {
            var command = new ResetPasswordSendCodeCommand(login);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok();
        }

        [HttpPost("recovery-password/confirm-code/{login}/{code}")]
        public async Task<IActionResult> ConfirmCodeEmail(string login, string code)
        {
            var command = new ResetPasswordConfirmCodeCommand(login, code);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok();
        }
        
        [HttpPost("recovery-password")]
        public async Task<IActionResult> RecoveryPassword([FromBody] RecoveryPasswordRequest request)
        {
            var command = new ResetPasswordCommand(request.Login, request.Verifier, request.ClientSalt, request.EncryptedDek, request.CryptoVersion);
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
                return this.MapActionResult(result.Errors);
                
            return Ok();
        }
    }
}