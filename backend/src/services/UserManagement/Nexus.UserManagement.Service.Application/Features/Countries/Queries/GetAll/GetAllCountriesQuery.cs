using MediatR;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Queries.GetAll
{
    public sealed record GetAllCountriesQuery() : IRequest<List<CountryDTO>>;
}