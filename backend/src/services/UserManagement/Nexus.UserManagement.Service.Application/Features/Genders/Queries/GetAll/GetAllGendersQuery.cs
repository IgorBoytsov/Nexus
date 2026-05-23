using MediatR;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Queries.GetAll
{
    public sealed record GetAllGendersQuery() : IRequest<List<GenderResponse>>;
}