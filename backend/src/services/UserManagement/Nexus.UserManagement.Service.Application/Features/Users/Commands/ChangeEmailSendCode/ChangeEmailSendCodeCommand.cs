using Crossdyne.Toolkit.Results;
using MediatR;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeEmailSendCode
{
    public sealed record ChangeEmailSendCodeCommand(Guid UserId, string Email) : IRequest<Result<Unit>>;
}