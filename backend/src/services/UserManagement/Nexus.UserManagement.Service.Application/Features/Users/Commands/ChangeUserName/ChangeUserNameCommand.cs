using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.Validation.Abstractions;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeUserName
{
    public sealed record ChangeUserNameCommand(Guid UserId, string UserName) : IRequest<Result<Unit>>, IHasUserName;
}