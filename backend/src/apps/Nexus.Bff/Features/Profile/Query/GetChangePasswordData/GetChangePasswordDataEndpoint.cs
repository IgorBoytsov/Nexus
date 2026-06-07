using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Profile.Query.GetChangePasswordData
{
    public static class ChangePasswordInitEndpoint
    {
        public static void MapChangePassword(this IEndpointRouteBuilder app)
        {
            app.MapGet("change-password", async (
                HttpContext httpContext, 
                [FromServices] IMediator mediator) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    return Results.Unauthorized();

                var command = new GetChangePasswordDataQuery(Guid.Parse(userId));

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}