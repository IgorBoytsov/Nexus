using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Authentication.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.SrpChallenge
{
    public static class GetSrpChallengeEndpoint
    {
        public static void MapGetSrpChallenge(this IEndpointRouteBuilder app)
        {
            app.MapPost("srp/challenge", async ([FromBody] SrpChallengeRequest request, [FromServices] IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetSrpChallengeCommand(request.Login), ct);

                if(result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            });
        }
    }
}