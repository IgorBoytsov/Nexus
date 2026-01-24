using Microsoft.AspNetCore.Mvc;
using Nexus.Account.Web.Services.Http;

namespace Nexus.Account.Web.Controllers.Api
{
    [Route("api/users")]
    [ApiController]
    public class UserMenagementApiController(IUserMenagementClient userMenagementClient) : Controller
    {
        private readonly IUserMenagementClient _userMenagementClient = userMenagementClient;
        
        [HttpGet("public-encryption-info")]
        public async Task<IActionResult> GetPublicEncryptionInfo([FromRoute] string login)
        {
            var result = await _userMenagementClient.GetPublicEncryptionInfo(login);

            if (result.IsFailure)
                return NotFound("Данные не найдены");

            return Ok(result.Value);
        }
    }
}