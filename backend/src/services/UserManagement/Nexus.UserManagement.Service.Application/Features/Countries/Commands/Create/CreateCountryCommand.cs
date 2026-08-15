using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Create
{
    public sealed record CreateCountryCommand(string Name) : IRequest<Result<Guid>>, ICommand;
}