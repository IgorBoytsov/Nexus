using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Register
{
    public sealed record RegisterCommand(
        string Login,
        string UserName,
        string Verifier, 
        string ClientSalt, 
        string EncryptedDek,
        string EncryptionAlgorithm,
        int Iterations,
        string KdfType,
        string Email, string? Phone,
        Guid? IdGender, Guid? IdCountry) : IRequest<Result>;
}