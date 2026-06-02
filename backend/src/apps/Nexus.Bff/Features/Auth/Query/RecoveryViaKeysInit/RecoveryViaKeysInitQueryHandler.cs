using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients.UserManagement;
using Shared.Contracts;

namespace Nexus.Bff.Features.Auth.Query.RecoveryViaKeysInit
{
    public sealed class RecoveryViaKeysInitQueryHandler(IUserManagementService userManagementService) : IRequestHandler<RecoveryViaKeysInitQuery, Result<RecoveryViaKeysPayloadResponse>>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result<RecoveryViaKeysPayloadResponse>> Handle(RecoveryViaKeysInitQuery request, CancellationToken cancellationToken)
            => await _userManagementService.InitRecoveryKeys(new RecoveryViaKeysGetPayloadRequest(request.Login));
    }
}