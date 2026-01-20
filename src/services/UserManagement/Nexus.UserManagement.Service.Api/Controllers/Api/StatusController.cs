using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Create;
using Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Delete;
using Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Update;
using Nexus.UserManagement.Service.Application.Features.Statuses.Queries.GetAll;
using Shared.Contracts.UserMenagement.Requests;

namespace Nexus.UserManagement.Service.Api.Controllers.Api
{
    [ApiController]
    [Route("api/statuses")]
    public class StatusController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStatusRequest request)
        {
            var result = await _mediator.Send(new CreateStatusCommand(request.Name));

            return result.Match<IActionResult>(onSuccess: () => Ok(result.Value), onFailure: errors => BadRequest(errors));
        }

        [HttpPatch("{id}/update")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _mediator.Send(new UpdateStatusCommand(id, request.Name));

            return result.Match<IActionResult>(onSuccess: () => Ok(), onFailure: errors => BadRequest(errors));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteStatusCommand(id));

            return result.Match<IActionResult>(onSuccess: () => Ok(), onFailure: errors => BadRequest(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllStatusesQuery());

            return Ok(result);
        }
    }
}
