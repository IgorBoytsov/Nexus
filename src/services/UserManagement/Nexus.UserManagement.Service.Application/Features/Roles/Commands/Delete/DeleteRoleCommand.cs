using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Delete
{
    public sealed record DeleteRoleCommand(Guid Id) : IRequest<Result>;
}