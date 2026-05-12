using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;

namespace Nexus.Bff.Features.Users.Command.Register
{
    public sealed class RegisterCommandHandler(IUserManagementService userManagementService) : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
            => _userManagementService.Register(new Shared.Contracts.RegisterUserRequest(request.Login, request.UserName, request.Verifier, request.ClientSalt, request.EncryptedDek, request.CryptoVersion, request.Email, request.Phone, request.IdGender?.ToString(), request.IdCountry?.ToString()));
    }
}