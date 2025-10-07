using MediatR;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetAll
{
    public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserListItemDto>>
    {
        public Task<List<UserListItemDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}