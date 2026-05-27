using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Query.RecoveryViaKeysInit
{
    public static class RecoveryViaKeysInitEndpoint
    {
        public static void MapRecoveryViaKeysInit(this IEndpointRouteBuilder app)
        {
            app.MapGet("init-recovery-keys", async ([FromQuery] string login, [FromServices] IMediator mediator) =>
            {
                var command = new RecoveryViaKeysInitQuery(login);

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            });
        }
    }
}