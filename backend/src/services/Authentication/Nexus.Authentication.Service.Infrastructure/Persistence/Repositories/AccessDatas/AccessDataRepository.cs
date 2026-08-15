using Microsoft.EntityFrameworkCore;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Domain.Models;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;
using Shared.EntityFramework;

namespace Nexus.Authentication.Service.Infrastructure.Persistence.Repositories.AccessDatas
{
    internal sealed class AccessDataRepository(AuthenticationContext context) : Repository<AccessData, AuthenticationContext>(context), IAccessDataRepository
    {
        public async Task<int> CloseSessions(Guid userId, DateTime eventDateTime)
            => await _entity.Where(a => a.UserId == userId && a.CreationDate < eventDateTime).ExecuteDeleteAsync();
    }
}