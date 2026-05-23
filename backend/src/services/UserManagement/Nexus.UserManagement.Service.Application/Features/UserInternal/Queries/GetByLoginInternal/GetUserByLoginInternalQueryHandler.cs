using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;

namespace Nexus.UserManagement.Service.Application.Features.UserInternal.Queries.GetByLoginInternal
{
    public sealed class GetUserByLoginInternalQueryHandler(IUserReadOnlyRepository userRepository) : IRequestHandler<GetUserByLoginInternalQuery, Result<UserAuthDataResponse>>
    {
        public async Task<Result<UserAuthDataResponse>> Handle(GetUserByLoginInternalQuery request, CancellationToken cancellationToken)
            => await userRepository.GetUserByLoginAuth(request.Login);
    }
}