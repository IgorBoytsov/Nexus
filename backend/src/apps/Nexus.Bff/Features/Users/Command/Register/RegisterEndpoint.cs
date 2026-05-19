using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Users.Command.Register
{
    public static class RegisterEndpoint
    {
        public static void MapRegister(this IEndpointRouteBuilder app)
        {
            app.MapPost("register", async ([FromBody] RegisterUserRequest request, [FromServices] IMediator mediator, CancellationToken ct) =>
            {
                var command = new RegisterCommand(
                    request.Login, 
                    request.UserName, 
                    request.Verifier, 
                    request.ClientSalt, 
                    request.EncryptedVerifierWrapKey, 
                    request.CryptoVersion,
                    request.SrpVersion,
                    request.Email,
                    request.EncryptedVerifierWrapKey, request.KeyWrapVersion, request.AsymmetricKeyId,
                    string.IsNullOrWhiteSpace(request.IdGender) ? null : Guid.Parse(request.IdGender),
                    string.IsNullOrWhiteSpace(request.IdCountry) ? null : Guid.Parse(request.IdCountry),
                    [.. request.RecoveryKeys.Select(rk => new RecoveryKeyCommandData(rk.EncryptedValue, rk.CryptoVersion))]);

                var result = await mediator.Send(command, ct);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });
        }
    }
}