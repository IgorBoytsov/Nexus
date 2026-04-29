using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Services;

namespace Nexus.Bff.Features.Profile.Query.Info
{
    public static class GetProfileInfoQueryEndpoint
    {
        public static void MapRegister(this IEndpointRouteBuilder app)
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
                    return Results.BadRequest(result.Errors);

                return Results.Ok(result.Value); 
            }).RequireAuthorization();
        }   
    }
}