using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Api.Models.Requests;
using Nexus.UserManagement.Service.Application.Features.Genders.Commands.Create;
using Nexus.UserManagement.Service.Application.Features.Genders.Commands.Delete;
using Nexus.UserManagement.Service.Application.Features.Genders.Commands.Update;
using Nexus.UserManagement.Service.Application.Features.Genders.Queries.GetAll;

namespace Nexus.UserManagement.Service.Api.Controllers.Api
{
    [ApiController]
    [Route("api/genders")]
    public class GenderController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGenderRequest request)
        {
            var result = await _mediator.Send(new CreateGenderCommand(request.Name));

            return result.Match<IActionResult>(onSuccess: () => Ok(result.Value), onFailure: errors => BadRequest(errors));
        }

        [HttpPatch("{id}/update")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromBody] UpdateGenderRequest request)
        {
            var result = await _mediator.Send(new UpdateGenderCommand(id, request.Name));

            return result.Match<IActionResult>(onSuccess: () => Ok(), onFailure: errors => BadRequest(errors));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteGenderCommand(id));

            return result.Match<IActionResult>(onSuccess: () => Ok(), onFailure: errors => BadRequest(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllGendersQuery());

            return Ok(result);
        }
    }
}