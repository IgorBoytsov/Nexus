using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts.UserManagement.Requests;

namespace Nexus.Bff.Features.Users.Command.Register
{
    public sealed class RegisterCommandHandler(IUserManagementService userManagementService) : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
            => _userManagementService.Register(new RegisterUserRequest(
                request.Login,
                request.UserName, 
                request.Email, 
                request.IdGender?.ToString(),
                request.IdCountry?.ToString(),
                request.EncryptedVerifier, 
                request.SrpSalt, 
                request.SrpVersion, 
                request.EncryptedVerifierWrapKey,
                request.KeyWrapVersion, 
                request.AsymmetricKeyId,
                request.EncryptedDek, 
                request.DekSalt, 
                request.CryptoVersion,
                [.. request.RecoveryKeys.Select(rk => new RecoveryKeyData(rk.EncryptedValue, rk.CryptoVersion))]));
    }
}