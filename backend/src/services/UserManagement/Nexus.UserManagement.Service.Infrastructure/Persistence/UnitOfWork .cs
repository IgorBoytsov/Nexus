using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence
{
    public class UnitOfWork(UserManagementContext context) : IUnitOfWork
    {
        private readonly UserManagementContext _context = context;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}