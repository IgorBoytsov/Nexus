using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Create
{
    public sealed record CreateGenderCommand(string Name) : IRequest<Result<Guid>>;
}