using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Api.Models.Requests;
using Nexus.UserManagement.Service.Application.Features.Countries.Commands.Create;
using Nexus.UserManagement.Service.Application.Features.Countries.Commands.Delete;
using Nexus.UserManagement.Service.Application.Features.Countries.Commands.Update;
using Nexus.UserManagement.Service.Application.Features.Countries.Queries.GetAll;

namespace Nexus.UserManagement.Service.Api.Controllers.Api
{
    [ApiController]
    [Route("api/countries")]
    public sealed class CountryController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCountryRequest request)
        {
            var result = await _mediator.Send(new CreateCountryCommand(request.Name));

            return result.Match<IActionResult>(onSuccess: () => Ok(result.Value), onFailure: errors => BadRequest(errors));
        }

        [HttpPatch("{id}/update")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, [FromBody] UpdateCountryRequest request)
        {
            var result = await _mediator.Send(new UpdateCountryCommand(id, request.Name));

            return result.Match<IActionResult>(onSuccess: () => Ok(), onFailure: errors => BadRequest(errors));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteCountryCommand(id));

            return result.Match<IActionResult>(onSuccess: () => Ok(), onFailure: errors => BadRequest(errors));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCountriesQuery());

            return Ok(result);
        }
    }
}