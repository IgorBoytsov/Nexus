using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword
{
    public sealed record ResetPasswordCommand(
        string Login, 
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
        int CryptoVersion, 
        // RecoveryKeys
        List<RecoveryKeyCommandData> RecoveryKeys) : IRequest<Result>,
        IHasLogin,
        IHasEncryptedVerifier, 
        IHasSrpSalt, 
        IHasEncryptedDek;

    public record RecoveryKeyCommandData(string EncryptedValue, int CryptoVersion);
}