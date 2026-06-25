using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts.UserManagement.Requests;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeys
{
    public sealed class RecoveryViaKeysCommandHandler(IUserManagementService userManagementService) : IRequestHandler<RecoveryViaKeysCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(RecoveryViaKeysCommand request, CancellationToken cancellationToken)
            => await _userManagementService.RecoveryKeys(
                new RecoveryViaKeysRequest(
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