using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Queries.GetAll
{
    public sealed class GetAllStatusesQueryHandler(IReadDbContext readContext) : IRequestHandler<GetAllStatusesQuery, List<StatusDto>>
    {
        private readonly IReadDbContext _readContext = readContext;

        public async Task<List<StatusDto>> Handle(GetAllStatusesQuery request, CancellationToken cancellationToken)
            => await _readContext.StatusesView
                .Select(s => new StatusDto(s.Id.ToString(), s.Name))
                    .ToListAsync(cancellationToken);
    }
}