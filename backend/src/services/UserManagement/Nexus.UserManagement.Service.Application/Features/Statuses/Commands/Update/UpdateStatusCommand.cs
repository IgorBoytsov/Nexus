using MediatR;
using Crossdyne.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Update
{
    public sealed record UpdateStatusCommand(Guid Id, string Name) : IRequest<Result>;
}