using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Validations.Common.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangePassword
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