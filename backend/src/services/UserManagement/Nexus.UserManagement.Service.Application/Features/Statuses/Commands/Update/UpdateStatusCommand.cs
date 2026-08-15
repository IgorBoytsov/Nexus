using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Update
{
    public sealed record UpdateStatusCommand(Guid Id, string Name) : IRequest<Result>, ICommand;
}