using System.Net.Quic;
using Crossdyne.Toolkit.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.UserManagement.Service.Api.Extensions;
using Nexus.UserManagement.Service.Api.Models;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetDek;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo;
using Shared.Web.Extensions;

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

        [HttpGet("private/crypto/dek")]
        [Authorize]
        public async Task<IActionResult> Dek()
        {
            Result<ExtractData> extractResult = this.ExtractCredentials(User, out IActionResult actionResult);
            
            if (extractResult.IsFailure)
                return actionResult;

            var query = new GetDekQuery(extractResult.Value.UserId);
            var result = await mediator.Send(query);

            if (result.IsFailure)
                return this.MapActionResult(result.Errors);

            return Ok(result.Value);
        }
    }
}