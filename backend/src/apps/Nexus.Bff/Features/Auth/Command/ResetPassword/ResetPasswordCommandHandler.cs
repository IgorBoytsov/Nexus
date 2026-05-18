using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Crossdyne.Toolkit.Results;
using Shared.Contracts;

namespace Nexus.Bff.Features.Auth.Command.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            => await _userManagementService.RecoveryPassword(new RecoveryPasswordRequest(request.Login, request.Verifier, request.ClientSalt, request.EncryptedDek, request.CryptoVersion, request.SrpVersion, request.EncryptedVerifierWrapKey, request.KeyWrapVersion, request.AsymmetricKeyId));
    }
}