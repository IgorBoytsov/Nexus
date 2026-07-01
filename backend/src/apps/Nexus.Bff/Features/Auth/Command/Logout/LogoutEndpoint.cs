using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.Logout
{
    public static class LogoutEndpoint
    {
        public static void MapLogout(this IEndpointRouteBuilder app)
        {
            app.MapPost("logout", async (
                HttpContext context, 
                [FromServices] IMediator mediator) =>
            {
                var sessionId = context.User.FindFirst("SessionId")?.Value;

                // if (string.IsNullOrWhiteSpace(sessionId))
                //     return Results.Unauthorized();
                    
                var command = new LogoutCommand(sessionId!);
                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Results.Ok();
            }).RequireAuthorization();
        }
    }
}