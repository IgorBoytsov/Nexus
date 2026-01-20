using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Account.Web.Services.Http;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.Authentication.Responses;
using System.Security.Claims;

namespace Nexus.Account.Web.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController(IAuthClient authClient) : Controller
    {
        private readonly IAuthClient _authClient = authClient;

        [HttpPost("challenge")]
        [AllowAnonymous]
        public async Task<IActionResult> Step1([FromBody] SrpChallengeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Login))
                return BadRequest("Логин обязателен");

            var result = await _authClient.GetSrpChallenge(request);

            if (result.IsFailure)
                return NotFound("Пользователь не найден");

            return Ok(result.Value);
        }

        [HttpPost("verify")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step2([FromBody] SrpVerifyRequest request)
        {
            var result = await _authClient.VerifySrpProof(request);

            if (result.IsFailure)
                return Unauthorized($"Неверные данные для входа (ZKP proof failed): {result.StringMessage}");

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

        [HttpPost("login-by-token")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginByToken()
        {
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var result = await _authClient.LoginByToken(new TokenLoginRequest(refreshToken));

            if (result.IsFailure)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Unauthorized();
            }

            var tokens = result.Value!;

            var claimsIdentity = new ClaimsIdentity(User.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();
            authProperties.StoreTokens(
                [
                     new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken },
                     new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken }
                ]);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return Ok();
        }

        [HttpGet("public-key")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicKey()
        {
            var result = await _authClient.GetPublicKey();

            if (result.IsFailure)
                return StatusCode(500, "Ошибка получения настроек безопасности");

            return Ok(new { PublicKey = result.Value });
        }

        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}