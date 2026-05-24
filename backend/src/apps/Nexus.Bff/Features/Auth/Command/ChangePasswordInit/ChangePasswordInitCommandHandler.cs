using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Shared.Contracts.UserManagement.Requests;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.Bff.Features.Auth.Command.ChangePasswordInit
{
    public sealed class ChangePasswordInitCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ChangePasswordInitCommand, Result<ChangePasswordInitResponse>>
    {
        public async Task<Result<ChangePasswordInitResponse>> Handle(ChangePasswordInitCommand request, CancellationToken cancellationToken)
            => await userManagementService.ChangePasswordInit(new ChangePasswordInitRequest(request.UserId.ToString()));
    }
}