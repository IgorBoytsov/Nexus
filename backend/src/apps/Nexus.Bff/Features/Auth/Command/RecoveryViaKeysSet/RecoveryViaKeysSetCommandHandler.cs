using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Shared.Contracts;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysSet
{
    public sealed class RecoveryViaKeysSetCommandHandler(IUserManagementService userManagementService) : IRequestHandler<RecoveryViaKeysSetCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(RecoveryViaKeysSetCommand request, CancellationToken cancellationToken)
            => await _userManagementService.RecoveryViaKeysSet(
                new RecoveryViaKeysSetRequest(
                    request.Login, 
                    request.Verifier, 
                    request.ClientSalt, 
                    request.EncryptedVerifierWrapKey, 
                    request.CryptoVersion, 
                    request.SrpVersion, 
                    request.EncryptedDek, 
                    request.KeyWrapVersion, 
                    request.AsymmetricKeyId,
                    [.. request.RecoveryKeys.Select(x => new RecoveryKeyRequestData(x.EncryptedValue, x.CryptoVersion))]));
    }
}