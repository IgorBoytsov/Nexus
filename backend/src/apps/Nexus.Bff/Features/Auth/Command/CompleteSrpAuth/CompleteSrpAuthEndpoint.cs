using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Authentication.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.CompleteSrpAuth
{
    public static class CompleteSrpAuthEndpoint
    {
        public static void MapCompleteSrpAuth(this IEndpointRouteBuilder app)
        {
            app.MapPost("srp/complete", async (
                HttpContext httpContext,
                [FromBody] CompleteSrpRequest request, 
                [FromServices] IMediator mediator, 
                CancellationToken ct) =>
            {
                var command = new CompleteSrpAuthCommand(request.TempAuthToken);
                var result = await mediator.Send(command, ct);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                var userSession = result.Value;

                var claims = new List<Claim> 
                { 
                    new(ClaimTypes.NameIdentifier, userSession.UserId),
                    new(ClaimTypes.Name, userSession.Login),
                    new("SessionId", userSession.SessionId) 
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, 
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                    AllowRefresh = true 
                };

                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return Results.Ok();
            });
        }
    }
}