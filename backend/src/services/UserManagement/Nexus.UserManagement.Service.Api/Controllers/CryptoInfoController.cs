using Crossdyne.Toolkit.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo;

namespace Nexus.UserManagement.Service.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public sealed class CryptoInfoController(IMediator mediator) : Controller
    {
       [HttpGet("{login}/crypto/public")]
        public async Task<IActionResult> GetPublicEncryptionInfo([FromRoute] string login)
        {
            var result = await mediator.Send(new GetPublicEncryptionInfoQuery(login));

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