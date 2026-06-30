using System.Security.Claims;
using Crossdyne.Toolkit.Results;
using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Infrastructure.Clients;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Nexus.Bff.Models.Responses;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;
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
                [FromServices] IFileService fileService,
                CancellationToken ct = default) =>
            {
                string? userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized(); 

                var result = await userManagementService.GetProfileInfo(userId);

                if(result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                ProfileInfoResponse profileInfoResponse = result.Value;
                S3KeyResponse? s3Key = profileInfoResponse.AvatarS3Key;

                if (s3Key == null)
                    return Results.Ok(new ProfileInfoBffResponse(profileInfoResponse.Login, profileInfoResponse.UserName, profileInfoResponse.Email, profileInfoResponse.DateRegistration, ""));

                Result<string> urlResult = await fileService.GetUrl(s3Key.Bucket, s3Key.FolderPath, s3Key.Key);

                if (urlResult.IsFailure)
                    return Results.Ok(new ProfileInfoBffResponse(profileInfoResponse.Login, profileInfoResponse.UserName, profileInfoResponse.Email, profileInfoResponse.DateRegistration, ""));

                return Results.Ok(new ProfileInfoBffResponse(profileInfoResponse.Login, profileInfoResponse.UserName, profileInfoResponse.Email, profileInfoResponse.DateRegistration, urlResult.Value)); 
            }).RequireAuthorization();

            app.MapPatch("change/avatar", async (
                [FromForm] IFormFile file, 
                [FromServices] IUserManagementService userManagementService) =>
            {
                using var stream = file.OpenReadStream();
                var result = await userManagementService.ChangeAvatar(stream, file.FileName);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            }).DisableAntiforgery().RequireAuthorization();
        }
    }
}