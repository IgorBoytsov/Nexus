using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Requests;
using Nexus.Bff.Infrastructure.Clients.UserManagement;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordConfirmCode
{
    public sealed class ResetPasswordConfirmCodeCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ResetPasswordConfirmCodeCommand, Result>
    {
        private IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(ResetPasswordConfirmCodeCommand request, CancellationToken cancellationToken)
            => await _userManagementService.ResetPasswordConfirm(request.Login, new ResetPasswordConfirmCodeRequest(request.Code));
    }
}