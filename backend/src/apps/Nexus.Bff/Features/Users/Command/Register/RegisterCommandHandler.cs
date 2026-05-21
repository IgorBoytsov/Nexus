using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Crossdyne.Toolkit.Results;
using Shared.Contracts;

namespace Nexus.Bff.Features.Users.Command.Register
{
    public sealed class RegisterCommandHandler(IUserManagementService userManagementService) : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
            => _userManagementService.Register(new RegisterUserRequest(
                request.Login, 
                request.UserName, 
                request.Verifier, 
                request.SrpVersion, 
                request.ClientSalt, 
                request.EncryptedVerifierWrapKey, 
                request.CryptoVersion, 
                request.EncryptedVerifierWrapKey, 
                request.KekWrapVersion, 
                request.KekKeyId,
                request.Email, 
                request.IdGender?.ToString(),
                request.IdCountry?.ToString(),
                [.. request.RecoveryKeys.Select(rk => new RecoveryKeyData(rk.EncryptedValue, rk.CryptoVersion))]));
    }
}