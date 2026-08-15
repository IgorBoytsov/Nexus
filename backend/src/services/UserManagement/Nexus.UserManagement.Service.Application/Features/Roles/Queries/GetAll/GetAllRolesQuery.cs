using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Queries.GetAll
{
    public sealed record GetAllRolesQuery() : IRequest<List<RoleResponse>>, IQuery;
}