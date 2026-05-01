using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Quantropic.Toolkit.Results;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordConfirmCode
{
    public sealed class ResetPasswordConfirmCodeCommandHandler(IUserManagementService userManagementService) : IRequestHandler<ResetPasswordConfirmCodeCommand, Result>
    {
        private IUserManagementService _userManagementService = userManagementService;

        public async Task<Result> Handle(ResetPasswordConfirmCodeCommand request, CancellationToken cancellationToken)
            => await _userManagementService.ConfirmCodeEmail(request.Login, request.Code);
    }
}