namespace Nexus.Authentication.Service.Application.Abstractions.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void Dispose();
    }
}