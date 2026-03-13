using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Queries.GetAll
{
    public sealed class GetAllCountriesQueryHandler(IReadDbContext readContext) : IRequestHandler<GetAllCountriesQuery, List<CountryDTO>>
    {
        private readonly IReadDbContext _readContext = readContext;

        public async Task<List<CountryDTO>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
            => await _readContext.CountriesView
                .Select(c => new CountryDTO(c.Id.ToString(), c.Name))
                    .ToListAsync(cancellationToken);
    }
}