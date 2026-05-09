using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Extensions;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordSendCode
{
    public static class ResetPasswordSendCodeEndpoint
    {
        public static void MapSendConfirmCodeEmail(this IEndpointRouteBuilder app)
        {
            app.MapPost("recovery-password/send-confirm-code/{login}", async (
                [FromRoute] string login, 
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(new ResetPasswordSendCodeCommand(login));

                if (result.IsFailure)
                    return result.Errors.MapToMinimalApiResult();

                return Results.Ok();
            });
        }
    }
}