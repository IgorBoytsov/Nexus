using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed class GetUserByIdQueryHandler(IUserReadOnlyRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserAuthDataResponse>
    {
        public async Task<UserAuthDataResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
            => await userRepository.GetUserByIdAuth(request.UserId);
    }
}