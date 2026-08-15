using Crossdyne.Toolkit.Results;
using MediatR;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.Bff.Features.Auth.Command.Logout
{
    public sealed record LogoutCommand(string SessionId): IRequest<Result<Unit>>;
}