using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysSet
{
    public sealed record RecoveryViaKeysSetCommand(
        string Login,
        string Verifier,
        string ClientSalt,
        string EncryptedVerifierWrapKey,
        int CryptoVersion,
        int SrpVersion,
        string EncryptedDek, 
        int KeyWrapVersion,
        string AsymmetricKeyId,
        List<RecoveryKeyCommandData> RecoveryKeys) : IRequest<Result>,
        IHasLogin,
        IHasVerifier, 
        IHasClientSalt, 
        IHasEncryptedDek;

        public record RecoveryKeyCommandData(string EncryptedValue, int CryptoVersion);
}