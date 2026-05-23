namespace Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void Dispose();
    }
}