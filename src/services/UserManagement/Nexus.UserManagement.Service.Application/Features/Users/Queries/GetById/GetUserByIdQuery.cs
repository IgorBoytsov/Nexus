using MediatR;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed record GetUserByIdQuery() : IRequest<UserDto>;
}