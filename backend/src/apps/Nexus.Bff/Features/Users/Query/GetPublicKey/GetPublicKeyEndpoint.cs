using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Nexus.Bff.Features.Users.Query.GetPublicKey
{
    public static class GetPublicKeyEndpoint
    {
        public static void MapGetPublicKey(this IEndpointRouteBuilder app)
        {
            app.MapGet("public-key", async ([FromServices] IMediator mediator) =>
            {
                var query = new GetPublicKeyQuery();
                var result = await mediator.Send(query);

                return Results.Ok(new
                {
                    publicKey = result.Value
                });
            });
        }
    }
}