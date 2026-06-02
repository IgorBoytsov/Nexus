using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.Bff.Infrastructure.Clients.UserManagement;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordSendCode
{
    public sealed class ResetPasswordSendCodeCommandHandler(IUserManagementService userManagement) : IRequestHandler<ResetPasswordSendCodeCommand, Result>
    {
        private readonly IUserManagementService _userManagement = userManagement;

        public async Task<Result> Handle(ResetPasswordSendCodeCommand request, CancellationToken cancellationToken)
            => await _userManagement.InitPasswordReset(request.Login);
    }
}