using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetChangePasswordData
{
    public sealed class GetChangePasswordDataQueryHandler(IUserReadOnlyRepository userRepository) : IRequestHandler<GetChangePasswordDataQuery, Result<GetChangePasswordDataResponse>>
    {
        public async Task<Result<GetChangePasswordDataResponse>> Handle(GetChangePasswordDataQuery request, CancellationToken cancellationToken)
            => await userRepository.GetChangePasswordData(request.UserId);
    }
}