using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public static class VerifySrpProofEndpoint
    {
        public static void MapVerifySrpProof(this IEndpointRouteBuilder app)
        {
            app.MapPost("srp/verify", async (
                HttpContext httpContext,
                [FromBody] SrpVerifyRequest request, 
                [FromServices] IMediator mediator, 
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new VerifySrpProofCommand(request.Login, request.A, request.M1), ct);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                var verifierResponse = result.Value;

                var claims = new List<Claim> 
                { 
                    new(ClaimTypes.NameIdentifier, verifierResponse.UserId),
                    new(ClaimTypes.Name, verifierResponse.Login),
                    new("SessionId", verifierResponse.SessionId) 
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

                return Results.Ok(new { M2 = result.Value!.M2 });
            });
        }
    }
}