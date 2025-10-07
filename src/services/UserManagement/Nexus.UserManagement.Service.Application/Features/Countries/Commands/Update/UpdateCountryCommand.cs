using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Update
{
    public sealed record UpdateCountryCommand(Guid Id, string Name) : IRequest<Result>;
}