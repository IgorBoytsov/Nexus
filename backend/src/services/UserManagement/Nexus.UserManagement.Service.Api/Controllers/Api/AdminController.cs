using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Api.Models.Requests.Admin;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.RegisterAdmin;

namespace Nexus.UserManagement.Service.Api.Controllers.Api
{
    [ApiController]
    [Route("api/admin/users")]
    //[Authorize(Roles = "Admin, SuperAdmin")]
    public class AdminController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserByAdminRequest request)
        {
            var command = new RegisterAdminCommand(
                request.Login,
                request.UserName,
                request.Password,
                request.Email,
                request.Phone,
                request.IdGender,
                request.IdCountry);

            var result = await _mediator.Send(command);

            return result.Match<IActionResult>(
                onSuccess: Ok,
                onFailure: errors => Conflict(errors)
            );
        }

    }
}