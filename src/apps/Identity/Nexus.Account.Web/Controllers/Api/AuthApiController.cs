using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Nexus.Account.Web.Services.Http;
using Shared.Contracts.Requests;
using Shared.Contracts.Responses;
using System.Security.Claims;

namespace Nexus.Account.Web.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController(IAuthClient authClient) : Controller
    {
        private readonly IAuthClient _authClient = authClient;

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            if (request == null)
                return BadRequest("Данные были не заполнены.");

            var result = await _authClient.Login(request);

            if (result.IsFailure)
                return Unauthorized("Неверные данные для входа.");

            AuthResponse tokens = result.Value!;

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, request.Login)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();
            authProperties.StoreTokens(
            [
                new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken },
                new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken }
            ]);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return Ok(new { message = "Success" });
        }

        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}