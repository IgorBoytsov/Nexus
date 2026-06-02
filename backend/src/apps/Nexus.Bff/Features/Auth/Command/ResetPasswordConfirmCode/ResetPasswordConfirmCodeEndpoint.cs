using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.UserManagement.Requests;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordConfirmCode
{
    public static class ResetPasswordConfirmCodeEndpoint
    {
        public static void MapConfirmCodeEmail(this IEndpointRouteBuilder app)
        {
            app.MapPost("recovery-password/confirm-code/{login}", async (
                [FromRoute] string login, 
                [FromBody] ConfirmCodeRequest request,
                [FromServices] IMediator mediator) =>
            {
               var result = await mediator.Send(new ResetPasswordConfirmCodeCommand(login, request.Code));
               
                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });
        }
    }
}