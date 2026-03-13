using MediatR;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetAll
{
    public sealed record GetAllUsersQuery() : IRequest<List<UserListItemDto>>;
}