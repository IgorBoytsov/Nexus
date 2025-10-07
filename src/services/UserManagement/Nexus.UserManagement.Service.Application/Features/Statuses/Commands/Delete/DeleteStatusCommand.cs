using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Delete
{
    public sealed record DeleteStatusCommand(Guid Id) : IRequest<Result>;
}