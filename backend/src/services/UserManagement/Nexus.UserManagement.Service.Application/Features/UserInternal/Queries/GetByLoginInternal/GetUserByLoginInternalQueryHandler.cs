using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.UserInternal.Queries.GetByLoginInternal
{
    public sealed class GetUserByLoginInternalQueryHandler(IUserReadOnlyRepository userRepository) : IRequestHandler<GetUserByLoginInternalQuery, Result<UserAuthDataResponse>>
    {
        public async Task<Result<UserAuthDataResponse>> Handle(GetUserByLoginInternalQuery request, CancellationToken cancellationToken)
            => await userRepository.GetUserByLoginAuth(request.Login);
    }
}