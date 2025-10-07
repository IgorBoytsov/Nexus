using MediatR;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Queries.GetAll
{
    public sealed record GetAllRolesQuery() : IRequest<List<RoleDto>>;
}