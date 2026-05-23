using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Queries.GetAll
{
    public sealed class GetAllCountriesQueryHandler(ICountryReadOnlyRepository countryRepository) : IRequestHandler<GetAllCountriesQuery, List<CountryResponse>>
    {
        public async Task<List<CountryResponse>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
            => [.. await countryRepository.GetAllAsync(cancellationToken)];      
    }
}