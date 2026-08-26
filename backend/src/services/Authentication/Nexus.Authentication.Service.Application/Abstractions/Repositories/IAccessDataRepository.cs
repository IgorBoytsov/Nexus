using Nexus.Authentication.Service.Domain.Models;
using Shared.Kernel.Interfaces;

namespace Nexus.Authentication.Service.Application.Abstractions.Repositories
{
    public interface IAccessDataRepository : IRepository<AccessData>
    {
        Task<int> CloseSessions(Guid userId, DateTime eventDateTime);
    }
}