using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rebout.Nexus.Contracts.UserManagement.v1;

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
                    request.EncryptedDek, 
                    request.EncryptionAlgorithm, 
                    request.Iterations, 
                    request.KdfType, 
                    request.Email,
                    string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone,
                    string.IsNullOrWhiteSpace(request.IdGender) ? null : Guid.Parse(request.IdGender),
                    string.IsNullOrWhiteSpace(request.IdCountry) ? null : Guid.Parse(request.IdCountry));

                var result = await mediator.Send(command, ct);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);

                return Results.Ok();
            });
        }
    }
}