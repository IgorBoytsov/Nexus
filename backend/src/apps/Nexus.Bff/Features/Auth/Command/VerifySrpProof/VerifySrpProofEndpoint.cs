using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Authentication.Requests;
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

                return Results.Ok(new 
                {
                     M2 = verifierResponse.M2, 
                     TempAuthToken = verifierResponse.TempAuthToken 
                });
            });
        }
    }
}