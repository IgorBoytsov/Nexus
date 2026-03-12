using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Create
{
    public sealed record CreateStatusCommand(string Name) : IRequest<Result<Guid>>;
}