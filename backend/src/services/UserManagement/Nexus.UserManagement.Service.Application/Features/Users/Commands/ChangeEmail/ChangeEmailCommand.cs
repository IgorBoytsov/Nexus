using Crossdyne.Toolkit.Results;
using MediatR;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeEmail
{
    public sealed record ChangeEmailCommand(Guid UserId, string Email, string Code) : IRequest<Result<Unit>>;
}