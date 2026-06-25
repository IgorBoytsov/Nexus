using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.ResetPassword
{
    public static class ResetPasswordEndpoint
    {
        public static void MapRecoveryAccess(this IEndpointRouteBuilder app)
        {
            app.MapPost("reset-password", async (
                [FromBody] ResetPasswordCompleteRequest request, 
                [FromServices] IMediator mediator) =>
            {
                var command = new ResetPasswordCommand(
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
                    [.. request.RecoveryKeys.Select(x => new RecoveryKeyCommandData(x.EncryptedValue, x.CryptoVersion))]);
                    
                var result = await mediator.Send(command);
                            
                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });
        }
    }
}