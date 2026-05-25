using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Validations.Common.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangePassword
{
    public sealed record ChangePasswordCommand(
        Guid UserId,
        string Verifier, 
        string ClientSalt, 
        string EncryptedDek,
        int CryptoVersion, 
        int SrpVersion, 
        string EncryptedVerifierWrapKey, 
        int KeyWrapVersion, 
        string AsymmetricKeyId) : IRequest<Result>,
        IHasGuidUserId,
        IHasVerifier, 
        IHasClientSalt, 
        IHasEncryptedDek;
}