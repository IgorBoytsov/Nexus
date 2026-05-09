using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Genders.Commands.Create;
using Nexus.UserManagement.Service.Application.Features.Genders.Commands.Delete;
using Nexus.UserManagement.Service.Application.Features.Genders.Commands.Update;
using Nexus.UserManagement.Service.Application.Features.Genders.Queries.GetAll;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api.Controllers
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

            return result.Match<IActionResult>(onSuccess: () => Ok(result.Value), onFailure: this.MapActionResult);
        }

        [HttpPatch("{id}/update")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromBody] UpdateGenderRequest request)
        {
            var result = await _mediator.Send(new UpdateGenderCommand(id, request.Name));

            return result.Match<IActionResult>(onSuccess: Ok, onFailure: this.MapActionResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteGenderCommand(id));

            return result.Match<IActionResult>(onSuccess: Ok, onFailure: this.MapActionResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllGendersQuery());

            return Ok(result);
        }
    }
}