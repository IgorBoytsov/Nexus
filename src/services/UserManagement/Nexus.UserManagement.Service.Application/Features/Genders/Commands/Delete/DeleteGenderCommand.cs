using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Delete
{
    public sealed record DeleteGenderCommand(Guid Id) : IRequest<Result>;
}