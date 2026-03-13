using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Update
{
    public sealed record UpdateGenderCommand(Guid Id, string Name) : IRequest<Result>;
}