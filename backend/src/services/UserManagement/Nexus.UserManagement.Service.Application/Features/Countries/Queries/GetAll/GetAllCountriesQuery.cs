using MediatR;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Queries.GetAll
{
    public sealed record GetAllCountriesQuery() : IRequest<List<CountryResponse>>;
}