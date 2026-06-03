using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Services;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Query.GetChangePasswordData
{
    public static class ChangePasswordInitEndpoint
    {
        public static void MapChangePassword(this IEndpointRouteBuilder app)
        {
            app.MapGet("change-password", async (
                HttpContext httpContext, 
                [FromServices] IMediator mediator,
                [FromServices] JwtReadService jwtReadService) =>
            {
                var token = await httpContext.GetTokenAsync("access_token");
                var tokenData = jwtReadService.ExtractData(token!);

                var command = new GetChangePasswordDataQuery(Guid.Parse(tokenData.UserId));

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}