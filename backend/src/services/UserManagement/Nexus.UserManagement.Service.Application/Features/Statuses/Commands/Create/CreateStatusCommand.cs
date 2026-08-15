using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Create
{
    public sealed record CreateStatusCommand(string Name) : IRequest<Result<Guid>>, ICommand;
}