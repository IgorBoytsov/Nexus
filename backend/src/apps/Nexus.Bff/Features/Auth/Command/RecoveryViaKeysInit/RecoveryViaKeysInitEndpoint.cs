using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysInit
{
    public static class RecoveryViaKeysInitEndpoint
    {
        public static void MapRecoveryViaKeysInit(this IEndpointRouteBuilder app)
        {
            app.MapPost("init-recovery-keys", async ( [FromBody] RecoveryViaKeysGetPayloadRequest request, [FromServices] IMediator mediator) =>
            {
                var command = new RecoveryViaKeysInitCommand(request.Login);

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            });
        }
    }
}