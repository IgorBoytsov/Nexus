using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Delete
{
    public sealed record DeleteStatusCommand(Guid Id) : IRequest<Result>, ICommand;
}