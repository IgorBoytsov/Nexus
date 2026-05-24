using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Shared.Contracts.UserManagement.Requests;

namespace Nexus.Bff.Features.Auth.Command.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ChangePasswordCommand, Result>
    {
        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            => await userManagementService.ChangePassword(new ChangePasswordRequest(request.UserId.ToString(), request.Verifier, request.ClientSalt, request.EncryptedDek, request.CryptoVersion, request.SrpVersion, request.EncryptedVerifierWrapKey, request.KeyWrapVersion, request.AsymmetricKeyId));
    }
}