using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Queries.GetAll
{
    public sealed record GetAllCountriesQuery() : IRequest<List<CountryResponse>>, IQuery;
}