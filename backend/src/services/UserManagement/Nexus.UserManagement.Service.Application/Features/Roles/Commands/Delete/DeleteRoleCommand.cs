using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Delete
{
    public sealed record DeleteRoleCommand(Guid Id) : IRequest<Result>, ICommand;
}