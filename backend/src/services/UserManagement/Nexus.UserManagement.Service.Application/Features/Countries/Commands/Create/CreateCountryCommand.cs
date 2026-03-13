using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Create
{
    public sealed record CreateCountryCommand(string Name) : IRequest<Result<Guid>>;
}