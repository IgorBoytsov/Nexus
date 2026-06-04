using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts;
using Nexus.Bff.Infrastructure.Clients.UserManagement;

namespace Nexus.Bff.Features.Auth.Command.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            => await _userManagementService.ResetPasswordComplete(new ResetPasswordCompleteRequest(
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
                [.. request.RecoveryKeys.Select(x => new RecoveryKeysRequestData(x.EncryptedValue, x.CryptoVersion))]));
    }
}