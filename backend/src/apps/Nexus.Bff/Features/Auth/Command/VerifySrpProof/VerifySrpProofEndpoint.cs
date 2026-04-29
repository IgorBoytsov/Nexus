using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Services;
using Rebout.Nexus.Contracts.Authentication.v1;

namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public static class VerifySrpProofEndpoint
    {
        public static void MapVerifySrpProof(this IEndpointRouteBuilder app)
        {
            app.MapPost("srp/verify", async (
                HttpContext httpContext,
                [FromServices] JwtReadService jwtReader,
                [FromBody] SrpVerifyRequest request, 
                [FromServices] IMediator mediator, 
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new VerifySrpProofCommand(request.Login, request.A, request.M1), ct);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);

                var tokens = result.Value;

                var jwtData = jwtReader.ExtractData(tokens!.AccessToken);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, jwtData.Login),
                    new(ClaimTypes.NameIdentifier, jwtData.UserId)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();
                authProperties.StoreTokens(
                [
                    new AuthenticationToken { Name = "access_token", Value = tokens!.AccessToken },
                    new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken }        
                ]);

                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return Results.Ok(new { M2 = result.Value!.M2 });
            });
        }
    }
}