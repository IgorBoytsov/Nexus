using MediatR;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Queries.GetAll
{
    public sealed record GetAllStatusesQuery() : IRequest<List<StatusDto>>;
}