using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Update
{
    public sealed record UpdateGenderCommand(Guid Id, string Name) : IRequest<Result>, ICommand;
}