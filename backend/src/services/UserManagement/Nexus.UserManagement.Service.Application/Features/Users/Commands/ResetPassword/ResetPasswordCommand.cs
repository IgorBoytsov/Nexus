using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword
{
    public sealed record ResetPasswordCommand(string Login, string Verifier, string ClientSalt, string EncryptedDek, string EncryptionAlgorithm, int Iterations, string KdfType) : IRequest<Result>;
}