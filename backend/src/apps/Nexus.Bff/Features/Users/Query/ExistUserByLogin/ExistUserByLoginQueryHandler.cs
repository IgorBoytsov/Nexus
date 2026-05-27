using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Shared.Contracts;

namespace Nexus.Bff.Features.Users.Query.ExistUserByLogin
{
    public sealed class ExistUserByLoginCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ExistUserByLoginQuery, Result>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(ExistUserByLoginQuery request, CancellationToken cancellationToken)
            => await _userManagementService.ExistUserByLogin(new ExistUserBuLoginRequest(request.Login));
    }
}