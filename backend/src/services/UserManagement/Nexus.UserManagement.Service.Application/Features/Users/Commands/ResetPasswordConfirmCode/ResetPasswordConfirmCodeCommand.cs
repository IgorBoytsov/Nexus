using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordConfirmCode
{
    public sealed record ResetPasswordConfirmCodeCommand(string Login, int Code) : IRequest<Result>;
}