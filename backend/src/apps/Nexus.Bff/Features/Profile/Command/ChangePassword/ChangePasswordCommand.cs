using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.Bff.Features.Profile.Command.ChangePassword
{
    public sealed record ChangePasswordCommand(
        Guid UserId,
        // Srp
        string EncryptedVerifier, 
        string SrpSalt, 
        int SrpVersion, 
        string EncryptedVerifierWrapKey, 
        int KeyWrapVersion, 
        string AsymmetricKeyId,
        // Dek
        string EncryptedDek,
        string DekSalt,
        int CryptoVersion) : IRequest<Result>,
        IHasGuidUserId,
        IHasEncryptedVerifier, 
        IHasSrpSalt, 
        IHasEncryptedDek;
}