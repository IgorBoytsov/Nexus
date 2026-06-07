using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Profile.Command.ChangePassword
{
    public static class ChangePasswordEndpoint
    {
        public static void MapChangePassword(this IEndpointRouteBuilder app)
        {
            app.MapPost("change-password", async (
                HttpContext httpContext, 
                [FromBody] ChangePasswordRequest request, 
                [FromServices] IMediator mediator) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    return Results.Unauthorized();

                var command = new ChangePasswordCommand(
                    Guid.Parse(userId),
                    request.EncryptedVerifier, 
                    request.SrpSalt, 
                    request.SrpVersion, 
                    request.EncryptedVerifierWrapKey, 
                    request.KeyWrapVersion, 
                    request.AsymmetricKeyId, 
                    request.EncryptedDek, 
                    request.DekSalt,
                    request.CryptoVersion);

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            }).RequireAuthorization();
        }
    }
}