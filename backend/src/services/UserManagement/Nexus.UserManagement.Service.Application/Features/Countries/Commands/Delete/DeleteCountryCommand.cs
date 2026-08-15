using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Delete
{
    public sealed record DeleteCountryCommand(Guid Id) : IRequest<Result>, ICommand;
}