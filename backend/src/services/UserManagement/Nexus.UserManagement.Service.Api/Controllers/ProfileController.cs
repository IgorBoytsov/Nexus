using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.ExistByLogin;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api.Controllers
{    
    [ApiController]
    [Route("api/v1/users")]
    public class ProfileController(IMediator mediator) : Controller
    {
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("User ID не найден в токене.");

            if (!Guid.TryParse(userIdString, out var userId))
                return BadRequest("Не верный User ID формат.");

            var result = await mediator.Send(new GetUserByIdQuery(Guid.Parse(userIdString)));

            return Ok(result);
        }

        
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetProfileInfo([FromRoute] string userId)
        {
            var result = await mediator.Send(new GetProfileInfoQuery(Guid.Parse(userId)));

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [HttpGet("{login}/exists")]
        public async Task<IActionResult> ExistUserByLogin([FromRoute] string login)
        {
            var command = new ExistUserByLoginQuery(login);

            var result = await mediator.Send(command);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok();
        }
    }
}