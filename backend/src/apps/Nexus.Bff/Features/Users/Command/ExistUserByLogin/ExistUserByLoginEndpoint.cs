using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Users.Command.ExistUserByLogin
{
    public static class ExistUserByLoginEndpoint
    {
        public static void MapExistUserByLogin(IEndpointRouteBuilder app)
        {
            app.MapPost("exist-user-by-login", async ([FromBody] ExistUserBuLoginRequest request, [FromServices] IMediator mediator, CancellationToken ct) =>
            {
                var command = new ExistUserByLoginCommand(request.Login);

                var result = await mediator.Send(command, ct);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });
        }
    }
}