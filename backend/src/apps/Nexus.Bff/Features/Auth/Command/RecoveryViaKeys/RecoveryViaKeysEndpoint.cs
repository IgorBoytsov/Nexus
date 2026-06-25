using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeys
{
    public static class RecoveryViaKeysEndpoint
    {
        public static void MapRecoveryViaKeysSet(this IEndpointRouteBuilder app)
        {
            app.MapPost("recovery-via-keys", async ( [FromBody] RecoveryViaKeysRequest request, [FromServices] IMediator mediator) =>
            {
                var command = new RecoveryViaKeysCommand(
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