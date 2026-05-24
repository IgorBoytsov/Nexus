using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nexus.Bff.Services;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.ChangePassword
{
    public static class ChangePasswordEndpoint
    {
        public static void MapChangePassword(this IEndpointRouteBuilder app)
        {
            app.MapPost("change-password", async (
                HttpContext httpContext, 
                [FromBody] ChangePasswordRequest request, 
                [FromServices] IMediator mediator,
                [FromServices] JwtReadService jwtReaderService) =>
            {
                var token = await httpContext.GetTokenAsync("access_token");
                var tokenData = jwtReaderService.ExtractData(token!);

                var command = new ChangePasswordCommand(Guid.Parse(tokenData.UserId), request.Verifier, request.ClientSalt, request.EncryptedDek, request.CryptoVersion, request.SrpVersion, request.EncryptedVerifierWrapKey, request.KeyWrapVersion, request.AsymmetricKeyId);

                var result = await mediator.Send(command);

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            }).RequireAuthorization();
        }
    }
}