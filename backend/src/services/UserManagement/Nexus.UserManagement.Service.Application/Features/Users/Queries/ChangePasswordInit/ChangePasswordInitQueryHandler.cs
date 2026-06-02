using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.ChangePasswordInit
{
    public sealed class ChangePasswordInitQueryHandler(IUserReadOnlyRepository userRepository) : IRequestHandler<ChangePasswordInitQuery, Result<ChangePasswordInitResponse>>
    {
        public async Task<Result<ChangePasswordInitResponse>> Handle(ChangePasswordInitQuery request, CancellationToken cancellationToken)
            => await userRepository.ChangePasswordInit(request.UserId);
    }
}