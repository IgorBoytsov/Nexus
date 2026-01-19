using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Api.Models.Requests;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryAccess;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.Register;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo;
using Shared.Kernel.Results;
using System.Security.Claims;

namespace Nexus.UserManagement.Service.Api.Controllers.Api
{
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
            var command = new RegisterCommand(request.Login, request.UserName, request.Verifier, request.ClientSalt, request.EncryptedDek, request.Email, request.Phone, request.IdGender, request.IdCountry);

            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess: () => Ok(new { Message = "Регистрация прошла успешно!" }),
                onFailure: errors =>
                {
                    if (errors.Any(e => e.Code == ErrorCode.Save))
                    {
                        var serverError = errors.FirstOrDefault(e => e.Code == ErrorCode.Save);
                        return StatusCode(StatusCodes.Status500InternalServerError, new
                        {
                            Tittle = "Внутренняя ошибка сервера",
                            Details = serverError?.Message,
                        });
                    }

                    if (errors.Any(e => e.Code == ErrorCode.Conflict))
                    {
                        var conflictError = errors.FirstOrDefault(e => e.Code == ErrorCode.Conflict);
                        return Conflict(new
                        {
                            Title = "Конфликт данных",
                            Detail = conflictError?.Message
                        });
                    }

                    return BadRequest(new
                    {
                        Title = "Произошла ошибка",
                        Errors = errors.Select(e => new
                        {
                            e.Code,
                            e.Message
                        })
                    });
                });
        }

        [HttpPost("recovery-access")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryAccess([FromBody] RecoveryAccessRequest request)
        {
            var command = new RecoveryAccessCommand(request.Login, request.Email, request.NewPassword);

            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess: Ok,
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
    }
}