using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Features.Auth.Query.ChangePasswordInit
{
    public sealed class ChangePasswordInitQueryHandler(IUserManagementService userManagementService) : IRequestHandler<ChangePasswordInitQuery, Result<ChangePasswordInitResponse>>
    {
        public async Task<Result<ChangePasswordInitResponse>> Handle(ChangePasswordInitQuery request, CancellationToken cancellationToken)
            => await userManagementService.InitPasswordChange(new ChangePasswordInitRequest(request.UserId.ToString()));
    }
}