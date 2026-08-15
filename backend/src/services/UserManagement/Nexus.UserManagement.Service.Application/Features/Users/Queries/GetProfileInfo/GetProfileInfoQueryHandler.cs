using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo
{
    public sealed class GetProfileInfoQueryHandler(IUserReadOnlyRepository userRepository) : IRequestHandler<GetProfileInfoQuery, Result<ProfileInfoResponse>>
    {
        public async Task<Result<ProfileInfoResponse>> Handle(GetProfileInfoQuery request, CancellationToken cancellationToken)
            => await userRepository.GetProfileInfo(request.UserId);
    }
}