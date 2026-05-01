using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Quantropic.Toolkit.Results;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordSendCode
{
    public sealed class ResetPasswordSendCodeCommandHandler(IUserManagementService userManagement) : IRequestHandler<ResetPasswordSendCodeCommand, Result>
    {
        private readonly IUserManagementService _userManagement = userManagement;

        public async Task<Result> Handle(ResetPasswordSendCodeCommand request, CancellationToken cancellationToken)
            => await _userManagement.SendConfirmCodeEmail(request.Login);
    }
}