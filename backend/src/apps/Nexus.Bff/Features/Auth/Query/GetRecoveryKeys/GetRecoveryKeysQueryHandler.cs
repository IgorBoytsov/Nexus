using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts;

namespace Nexus.Bff.Features.Auth.Query.GetRecoveryKeys
{
    public sealed class GetRecoveryKeysQueryHandler(IUserManagementService userManagementService) : IRequestHandler<GetRecoveryKeysQuery, Result<RecoveryViaKeysPayloadResponse>>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result<RecoveryViaKeysPayloadResponse>> Handle(GetRecoveryKeysQuery request, CancellationToken cancellationToken)
            => await _userManagementService.GetRecoveryKeys(new RecoveryViaKeysGetPayloadRequest(request.Login));
    }
}