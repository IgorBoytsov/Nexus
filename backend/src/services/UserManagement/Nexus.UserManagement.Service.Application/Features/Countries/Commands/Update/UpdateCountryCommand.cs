using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Update
{
    public sealed record UpdateCountryCommand(Guid Id, string Name) : IRequest<Result>, ICommand;
}