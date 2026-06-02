using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts.UserManagement.Requests;

namespace Nexus.Bff.Features.Auth.Command.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ChangePasswordCommand, Result>
    {
        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            => await userManagementService.ChangePassword(new ChangePasswordRequest(
                request.UserId.ToString(), 
                request.EncryptedVerifier, 
                request.SrpSalt, 
                request.SrpVersion, 
                request.EncryptedVerifierWrapKey, 
                request.KeyWrapVersion,
                request.AsymmetricKeyId, 
                request.EncryptedDek, 
                request.DekSalt,
                request.CryptoVersion));
    }
}