using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysSet
{
    public sealed record RecoveryViaKeysSetCommand(
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
        // Recovery Keys
        List<RecoveryKeyCommandData> RecoveryKeys) : IRequest<Result>,
        IHasLogin,
        IHasEncryptedVerifier, 
        IHasSrpSalt, 
        IHasEncryptedDek;

        public record RecoveryKeyCommandData(string EncryptedValue, int CryptoVersion);
}