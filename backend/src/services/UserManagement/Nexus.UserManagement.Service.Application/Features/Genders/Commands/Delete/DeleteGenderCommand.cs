using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Delete
{
    public sealed record DeleteGenderCommand(Guid Id) : IRequest<Result>, ICommand;
}