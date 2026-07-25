using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Create
{
    public sealed record CreateGenderCommand(string Name) : IRequest<Result<Guid>>, ICommand;
}