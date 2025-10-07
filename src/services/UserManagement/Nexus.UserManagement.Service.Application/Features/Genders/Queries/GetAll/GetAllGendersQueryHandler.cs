using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Queries.GetAll
{
    public sealed class GetAllGendersQueryHandler(IReadDbContext readContext) : IRequestHandler<GetAllGendersQuery, List<GenderDto>>
    {
        private readonly IReadDbContext _readContext = readContext;

        public async Task<List<GenderDto>> Handle(GetAllGendersQuery request, CancellationToken cancellationToken)
            => await _readContext.GendersView.
                Select(g => new GenderDto(g.Id.ToString(), g.Name))
                    .ToListAsync(cancellationToken);
    }
}