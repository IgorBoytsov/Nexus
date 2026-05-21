using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Shared.Contracts;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysInit
{
    public sealed class RecoveryViaKeysInitCommandHandler(IUserManagementService userManagementService) : IRequestHandler<RecoveryViaKeysInitCommand, Result<RecoveryViaKeysPayloadResponse>>
    {
        private readonly IUserManagementService _userManagementService = userManagementService;

        public async Task<Result<RecoveryViaKeysPayloadResponse>> Handle(RecoveryViaKeysInitCommand request, CancellationToken cancellationToken)
            => await _userManagementService.RecoveryViaKeys(new RecoveryViaKeysGetPayloadRequest(request.Login));
    }
}