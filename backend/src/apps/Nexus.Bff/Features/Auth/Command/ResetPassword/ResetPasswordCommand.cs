using MediatR;
using Crossdyne.Toolkit.Results;

namespace Nexus.Bff.Features.Auth.Command.ResetPassword
{
    public sealed record ResetPasswordCommand(string Login, string Verifier, string ClientSalt, string EncryptedDek, string EncryptionAlgorithm, int Iterations, string KdfType) : IRequest<Result>;
}