using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Infrastructure.Clients;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Users
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("register", async (
                [FromBody] RegisterUserRequest request, 
                [FromServices] IUserManagementService userManagementService, 
                CancellationToken ct) =>
            {
                var result = await userManagementService.Register(new RegisterUserRequest(
                    request.Login,
                    request.UserName, 
                    request.Email, 
                    request.IdGender?.ToString(),
                    request.IdCountry?.ToString(),
                    request.EncryptedVerifier, 
                    request.SrpSalt, 
                    request.SrpVersion, 
                    request.EncryptedVerifierWrapKey,
                    request.KeyWrapVersion, 
                    request.AsymmetricKeyId,
                    request.EncryptedDek, 
                    request.DekSalt, 
                    request.CryptoVersion,
                    [.. request.RecoveryKeys.Select(rk => new RecoveryKeyData(rk.EncryptedValue, rk.CryptoVersion))]));

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });

            app.MapGet("exist/user/login", async (
                [FromQuery] string login, 
                [FromServices] IUserManagementService userManagementService, 
                CancellationToken ct) =>
            {
                var result = await userManagementService.ExistUserByLogin(login);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });

            app.MapGet("public/key", async ([FromServices] IAuthClient authClient) =>
            {
                var result = await authClient.GetPublicKey();

                return Results.Ok(new
                {
                    publicKey = result.Value
                });
            });
        }
    }
}