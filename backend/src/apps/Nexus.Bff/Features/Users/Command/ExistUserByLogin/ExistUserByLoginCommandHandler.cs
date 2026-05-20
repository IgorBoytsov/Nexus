using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Shared.Contracts;

namespace Nexus.Bff.Features.Users.Command.ExistUserByLogin
{
    public sealed class ExistUserByLoginCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ExistUserByLoginCommand, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(ExistUserByLoginCommand request, CancellationToken cancellationToken)
            => await _userManagementService.ExistUserByLogin(new ExistUserBuLoginRequest(request.Login));
    }
}