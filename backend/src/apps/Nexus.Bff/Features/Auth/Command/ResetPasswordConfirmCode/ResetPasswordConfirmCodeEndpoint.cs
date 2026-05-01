using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordConfirmCode
{
    public static class ResetPasswordConfirmCodeEndpoint
    {
        public static void MapConfirmCodeEmail(this IEndpointRouteBuilder app)
        {
            app.MapPost("recovery-password/confirm-code/{login}/{code}", async (
                [FromRoute] string login, 
                [FromRoute] string code,
                [FromServices] IMediator mediator) =>
            {
               var result = await mediator.Send(new ResetPasswordConfirmCodeCommand(login, code));
               
                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);

                return Results.Ok();
            });
        }
    }
}