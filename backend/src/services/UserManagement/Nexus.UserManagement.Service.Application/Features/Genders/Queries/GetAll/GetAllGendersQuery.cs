using MediatR;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Queries.GetAll
{
    public sealed record GetAllGendersQuery() : IRequest<List<GenderDto>>;
}