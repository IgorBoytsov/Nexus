using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Queries.GetAll
{
    public sealed class GetAllGendersQueryHandler(IGenderReadOnlyRepository genderRepository) : IRequestHandler<GetAllGendersQuery, List<GenderResponse>>
    {
        public async Task<List<GenderResponse>> Handle(GetAllGendersQuery request, CancellationToken cancellationToken)
            => [.. await genderRepository.GetAllAsync(cancellationToken)];
    }
}