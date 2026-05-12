using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.ResetPassword
{
    public static class ResetPasswordEndpoint
    {
        public static void MapRecoveryAccess(this IEndpointRouteBuilder app)
        {
            app.MapPost("reset-password", async (
                [FromBody] RecoveryPasswordRequest request, 
                [FromServices] IMediator mediator) =>
            {
                var command = new ResetPasswordCommand(request.Login, request.Verifier, request.ClientSalt, request.EncryptedDek, request.CryptoVersion);
                var result = await mediator.Send(command);
                            
                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });
        }
    }
}