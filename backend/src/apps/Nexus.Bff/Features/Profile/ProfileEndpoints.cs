using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Profile
{
    public static class ProfileEndpoints
    {
        public static void MapProfileEndpoints(this IEndpointRouteBuilder app)
        {
            #region Смена пароля

            app.MapGet("password/change/init", async (
                HttpContext httpContext, 
                [FromServices] IUserManagementService userManagementService) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    return Results.Unauthorized();

                var result = await userManagementService.GetChangePasswordData(new GetChangePasswordDataRequest(userId));

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value);
            }).RequireAuthorization();

            app.MapPost("password/change", async (
                HttpContext httpContext, 
                [FromBody] ChangePasswordRequest request, 
                [FromServices] IUserManagementService userManagementService) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    return Results.Unauthorized();

                var result = await userManagementService.ChangePassword(new ChangePasswordRequest(
                userId, 
                request.EncryptedVerifier, 
                request.SrpSalt, 
                request.SrpVersion, 
                request.EncryptedVerifierWrapKey, 
                request.KeyWrapVersion,
                request.AsymmetricKeyId, 
                request.EncryptedDek, 
                request.DekSalt,
                request.CryptoVersion));

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            }).RequireAuthorization();

            #endregion

            app.MapGet("/profile", async (
                HttpContext httpContext, 
                [FromServices] IUserManagementService userManagementService, 
                CancellationToken ct = default) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized(); 

                var result = await userManagementService.GetProfileInfo(userId);

                if(result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok(result.Value); 
            }).RequireAuthorization();
        }
    }
}