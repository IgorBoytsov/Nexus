using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Create
{
    public sealed record CreateGenderCommand(string Name) : IRequest<Result<Guid>>;
}