using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.Bff.Infrastructure.Clients.UserManagement;

namespace Nexus.Bff.Features.Profile.Query.Info
{
    public sealed class GetProfileInfoQueryHandler(IUserManagementService userManagementService) : IRequestHandler<GetProfileInfoQuery, Result<ProfileInfoResponse>>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result<ProfileInfoResponse>> Handle(GetProfileInfoQuery request, CancellationToken cancellationToken)
            => await _userManagementService.GetProfileInfo(request.UserId);
    }
}