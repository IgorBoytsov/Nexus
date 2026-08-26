using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Responses;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo
{
    public sealed class GetPublicEncryptionInfoQueryHandler(
        IUserReadOnlyRepository userRepository) : IRequestHandler<GetPublicEncryptionInfoQuery, Result<PublicEncryptionInfoResponse>>
    {
        public async Task<Result<PublicEncryptionInfoResponse>> Handle(GetPublicEncryptionInfoQuery request, CancellationToken cancellationToken)
            => await userRepository.GetPublicEncryptionInfoResponse(request.Login);
    }
}