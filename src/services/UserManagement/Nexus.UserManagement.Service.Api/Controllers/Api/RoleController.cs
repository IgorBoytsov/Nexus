using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Roles.Commands.Create;
using Nexus.UserManagement.Service.Application.Features.Roles.Commands.Delete;
using Nexus.UserManagement.Service.Application.Features.Roles.Commands.Update;
using Nexus.UserManagement.Service.Application.Features.Roles.Queries.GetAll;
using Shared.Contracts.UserMenagement.Requests;

namespace Nexus.UserManagement.Service.Api.Controllers.Api
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            var result = await _mediator.Send(new CreateRoleCommand(request.Name));

            return result.Match<IActionResult>(onSuccess: () => Ok(result.Value), onFailure: errors => BadRequest(errors));
        }

        [HttpPatch("{id}/update")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromBody] UpdateRoleRequest request)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(id, request.Name));

            return result.Match<IActionResult>(onSuccess: () => Ok(), onFailure: errors => BadRequest(errors));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));

            return result.Match<IActionResult>(onSuccess: () => Ok(), onFailure: errors => BadRequest(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());

            return Ok(result);
        }
    }
}
