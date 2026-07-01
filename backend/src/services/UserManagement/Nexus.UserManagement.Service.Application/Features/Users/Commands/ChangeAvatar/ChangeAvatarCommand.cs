using Crossdyne.Toolkit.Results;
using MediatR;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeAvatar
{
    public sealed record ChangeAvatarCommand(Guid UserId, Stream File, string FileName) : IRequest<Result<Unit>>;
}