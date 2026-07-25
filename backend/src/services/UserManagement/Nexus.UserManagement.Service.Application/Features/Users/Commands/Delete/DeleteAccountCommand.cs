using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Delete
{
    public sealed record DeleteAccountCommand(Guid UserId) : IRequest<Result<Unit>>, ICommand;
}