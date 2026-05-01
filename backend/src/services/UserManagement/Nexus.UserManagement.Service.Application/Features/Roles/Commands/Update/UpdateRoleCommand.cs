using MediatR;
using Crossdyne.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Update
{
    public sealed record UpdateRoleCommand(Guid Id, string Name) : IRequest<Result>;
}