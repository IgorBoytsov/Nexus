using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Create
{
    public sealed record CreateRoleCommand(string Name) : IRequest<Result<Guid>>, ICommand;
}