using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysSet
{
    public sealed class RecoveryViaKeysSetCommandHandler(IUserManagementService userManagementService) : IRequestHandler<RecoveryViaKeysSetCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(RecoveryViaKeysSetCommand request, CancellationToken cancellationToken)
            => await _userManagementService.SetRecoveryKeys(
                new RecoveryViaKeysSetRequest(
                    request.Login,
                    request.EncryptedVerifier, 
                    request.SrpSalt, 
                    request.SrpVersion, 
                    request.EncryptedVerifierWrapKey, 
                    request.KeyWrapVersion, 
                    request.AsymmetricKeyId, 
                    request.EncryptedDek, 
                    request.DekSalt,
                    request.CryptoVersion,
                    [.. request.RecoveryKeys.Select(x => new RecoveryKeyRequestData(x.EncryptedValue, x.CryptoVersion))]));
    }
}