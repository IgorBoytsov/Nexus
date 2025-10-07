using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Queries.GetAll
{
    public sealed class GetAllRolesQueryHandler(IReadDbContext readContext) : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IReadDbContext _readContext = readContext;

        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
            => await _readContext.RolesView
                .Select(r => new RoleDto(r.Id.ToString(), r.Name))
                    .ToListAsync(cancellationToken);
    }
}