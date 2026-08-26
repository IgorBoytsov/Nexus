using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Queries.GetAll
{
    public sealed class GetAllStatusesQueryHandler(IStatusReadOnlyRepository statusRepository) : IRequestHandler<GetAllStatusesQuery, List<StatusResponse>>
    {
        public async Task<List<StatusResponse>> Handle(GetAllStatusesQuery request, CancellationToken cancellationToken)
            => [.. await statusRepository.GetAllAsync(cancellationToken)]; 
    }
}