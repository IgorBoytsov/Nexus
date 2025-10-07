using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Delete
{
    public sealed record DeleteCountryCommand(Guid Id) : IRequest<Result>;
}