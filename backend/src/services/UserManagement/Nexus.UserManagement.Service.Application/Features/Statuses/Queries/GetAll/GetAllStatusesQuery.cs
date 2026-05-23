using MediatR;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Queries.GetAll
{
    public sealed record GetAllStatusesQuery() : IRequest<List<StatusResponse>>;
}