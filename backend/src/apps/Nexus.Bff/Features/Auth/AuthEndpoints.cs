using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Infrastructure.Clients;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            #region Проверка аутентификации

            app.MapGet("auth/status", () => Results.Ok(new { isAuthenticated = true })).RequireAuthorization();

            #endregion

            #region SRP

            app.MapPost("srp/challenge", async (
                [FromBody] SrpChallengeRequest request, 
                [FromServices] IAuthClient authClient, 
                CancellationToken ct) =>
            {
                var result = await authClient.GetSrpChallenge(new SrpChallengeRequest(request.Login.ToLowerInvariant()));

                if(result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            });

            #endregion

            #region Восстановление доступа через ключи

            app.MapGet("recovery/keys", async (
                [FromQuery] string login, 
                [FromServices] IUserManagementService userManagementService) =>
            {
                var result = await userManagementService.GetRecoveryKeys(login);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            });

            app.MapPost("recovery/keys/password/change", async (
                [FromBody] RecoveryViaKeysRequest request, 
                [FromServices] IUserManagementService userManagementService) =>
            {
                var result = await userManagementService.RecoveryKeys(new RecoveryViaKeysRequest(
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
                    [.. request.RecoveryKeys.Select(x => new RecoveryKeyRequestData(x.EncryptedValue, x.CryptoVersion))]));
                    
                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });

            #endregion

            #region Сброс пароля

            app.MapPost("password/reset/send-confirm-code/{login}", async (
                [FromRoute] string login, 
                [FromServices] IUserManagementService userManagementService) =>
            {
                var result = await userManagementService.ResetPasswordSendCode(login);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });

            app.MapPost("password/reset/confirm-code/{login}", async (
                [FromRoute] string login, 
                [FromBody] ResetPasswordConfirmCodeRequest request,
                [FromServices] IUserManagementService userManagementService) =>
            {
               var result = await userManagementService.ResetPasswordConfirm(login, request.Code);
               
                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });

            app.MapPost("password/reset", async (
                [FromBody] ResetPasswordCompleteRequest request, 
                [FromServices] IUserManagementService userManagementService) =>
            {
                var result = await userManagementService.ResetPasswordComplete(new ResetPasswordCompleteRequest(
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
                    [.. request.RecoveryKeys.Select(x => new RecoveryKeysRequestData(x.EncryptedValue, x.CryptoVersion))]));
                            
                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });

            #endregion

        }
    }
}