using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.Authentication.Service.Infrastructure.Persistence
{
    internal class UnitOfWork(AuthenticationContext context) : IUnitOfWork
    {
        private readonly AuthenticationContext _context = context;

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