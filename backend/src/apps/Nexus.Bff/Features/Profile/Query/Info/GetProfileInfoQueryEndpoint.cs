using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Services;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Profile.Query.Info
{
    public static class GetProfileInfoQueryEndpoint
    {
        public static void MapProfileInfo(this IEndpointRouteBuilder app)
        {
            app.MapGet("/profile", async (
                HttpContext httpContext, 
                [FromServices] JwtReadService jwtReadService, 
                [FromServices] IMediator mediator, 
                CancellationToken ct = default) =>
            {
                var token = await httpContext.GetTokenAsync("access_token");
                var tokenData = jwtReadService.ExtractData(token!);
                var result = await mediator.Send(new GetProfileInfoQuery(tokenData.UserId), ct);

                if(result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value); 
            }).RequireAuthorization();
        }   
    }
}