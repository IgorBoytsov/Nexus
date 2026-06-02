using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Services;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Query.ChangePasswordInit
{
    public static class ChangePasswordInitEndpoint
    {
        public static void MapChangePassword(this IEndpointRouteBuilder app)
        {
            app.MapGet("change-password-init", async (
                HttpContext httpContext, 
                [FromServices] IMediator mediator,
                [FromServices] JwtReadService jwtReadService) =>
            {
                var token = await httpContext.GetTokenAsync("access_token");
                var tokenData = jwtReadService.ExtractData(token!);

                var command = new ChangePasswordInitQuery(Guid.Parse(tokenData.UserId));

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}