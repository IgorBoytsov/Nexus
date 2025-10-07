using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Create
{
    public sealed record CreateRoleCommand(string Name) : IRequest<Result<Guid>>;
}