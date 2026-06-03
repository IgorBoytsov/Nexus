using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Features.Auth.Query.GetChangePasswordData
{
    public sealed class GetChangePasswordDataQueryHandler(IUserManagementService userManagementService) : IRequestHandler<GetChangePasswordDataQuery, Result<GetChangePasswordDataResponse>>
    {
        public async Task<Result<GetChangePasswordDataResponse>> Handle(GetChangePasswordDataQuery request, CancellationToken cancellationToken)
            => await userManagementService.GetChangePasswordData(new GetChangePasswordDataRequest(request.UserId.ToString()));
    }
}