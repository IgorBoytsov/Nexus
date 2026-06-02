using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysSet
{
    public static class RecoveryViaKeysSetEndpoint
    {
        public static void MapRecoveryViaKeysSet(this IEndpointRouteBuilder app)
        {
            app.MapPost("set-password-recovery-keys", async ( [FromBody] RecoveryViaKeysSetRequest request, [FromServices] IMediator mediator) =>
            {
                var command = new RecoveryViaKeysSetCommand(
                    request.Login,
                    request.EncryptedVerifier,
                    request.SrpSalt,
                    request.SrpVersion,
                    request.EncryptedVerifierWrapKey,
                    request.KeyWrapVersion,
                    request.AsymmetricKeyId,
                    request.EncryptedDek,
                    request.DekSalt,
                    request.CryptoVersion,
                    [.. request.RecoveryKeys.Select(x => new RecoveryKeyCommandData(x.EncryptedValue, x.CryptoVersion))]
                );

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });
        }
    }
}