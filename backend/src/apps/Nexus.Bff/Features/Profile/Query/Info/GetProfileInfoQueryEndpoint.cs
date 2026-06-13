using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Profile.Query.Info
{
    public static class GetProfileInfoQueryEndpoint
    {
        public static void MapProfileInfo(this IEndpointRouteBuilder app)
        {
            app.MapGet("/profile", async (
                HttpContext httpContext, 
                [FromServices] IMediator mediator, 
                CancellationToken ct = default) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized(); 

                var result = await mediator.Send(new GetProfileInfoQuery(userId!), ct);

                if(result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value); 
            }).RequireAuthorization();
        }   
    }
}