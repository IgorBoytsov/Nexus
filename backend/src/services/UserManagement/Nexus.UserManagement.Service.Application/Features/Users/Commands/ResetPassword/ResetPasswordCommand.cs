using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Validations.Common.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword
{
    public sealed record ResetPasswordCommand(string Login, string Verifier, string ClientSalt, string EncryptedDek, int CryptoVersion) : IRequest<Result>,
    IHasLogin,
    IHasVerifier, 
    IHasClientSalt, 
    IHasEncryptedDek;
}