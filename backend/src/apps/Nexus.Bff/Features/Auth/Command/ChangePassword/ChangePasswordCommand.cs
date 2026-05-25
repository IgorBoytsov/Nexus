using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.ChangePassword
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