using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Users.Query.ExistUserByLogin
{
    public static class ExistUserByLoginEndpoint
    {
        public static void MapExistUserByLogin(IEndpointRouteBuilder app)
        {
            app.MapGet("exist-user-by-login", async ([FromQuery] string login, [FromServices] IMediator mediator, CancellationToken ct) =>
            {
                var command = new ExistUserByLoginQuery(login);

                var result = await mediator.Send(command, ct);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });
        }
    }
}