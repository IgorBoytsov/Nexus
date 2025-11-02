using Microsoft.EntityFrameworkCore;
using Nexus.Authentication.Service.Domain.Models;

namespace Nexus.Authentication.Service.Application
{
    public interface IApplicationDbContext
    {
        DbSet<AccessData> AccessData { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}