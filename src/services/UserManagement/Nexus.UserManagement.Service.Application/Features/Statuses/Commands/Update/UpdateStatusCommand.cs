using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Update
{
    public sealed record UpdateStatusCommand(Guid Id, string Name) : IRequest<Result>;
}