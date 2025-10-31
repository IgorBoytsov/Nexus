using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Api.Models.Requests;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.Login;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryAccess;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.Register;
using Shared.Contracts.Requests;
using Shared.Kernel.Results;

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
            var command = new RegisterCommand(request.Login, request.UserName, request.Password, request.Email, request.Phone, request.IdGender, request.IdCountry);

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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var command = new LoginUserCommand(request.Password, request.Login, request.Email);

            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess: user =>
                {
                    return Ok(user);
                },
                onFailure: errors =>
                {
                    return Unauthorized(new
                    {
                        Title = "Не валидные данные",
                        Message = result.StringMessage
                    });

                });
        }

        [HttpPost("recovery-access")]
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
    }
}