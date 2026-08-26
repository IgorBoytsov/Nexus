using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Nexus.UserManagement.Service.Application.Abstractions.Validators;
using Shared.Abstractions.Validations;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeys
{
    public sealed record RecoveryViaKeysCommand(
        string Login,
        // Srp
        string EncryptedVerifier,
        string SrpSalt,
        int SrpVersion,
        int SrpCryptoVersion,
        string EncryptedVerifierWrapKey,
        int KeyWrapVersion,
        string AsymmetricKeyId,
        // Dek
        string EncryptedDek,
        string DekSalt, 
        int CryptoVersion,
        // Recovery Keys
        List<RecoveryKeyCommandData> RecoveryKeys) : IRequest<Result>, ICommand,
        IHasLogin,
        IHasEncryptedVerifier, 
        IHasSrpSalt, 
        IHasEncryptedDek;

        public record RecoveryKeyCommandData(string EncryptedValue, int CryptoVersion);
}