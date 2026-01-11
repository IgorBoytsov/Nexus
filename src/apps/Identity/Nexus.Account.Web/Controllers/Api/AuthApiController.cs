using Microsoft.AspNetCore.Mvc;
using Nexus.Account.Web.Services.Http;
using Shared.Contracts.Requests;

namespace Nexus.Account.Web.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController(IAuthClient authClient) : Controller
    {
        private readonly IAuthClient _authClient = authClient;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            if (request == null)
                return BadRequest("Данные были не заполнены.");

            var result = await _authClient.Login(request);

            if (result.IsFailure)
                return Unauthorized("Неверные данные для входа.");

            return Ok(new { message = "Success" });
        }
    }
}