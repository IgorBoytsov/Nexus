using Shared.Contracts.UserManagement.Responses;
using Shared.Kernel.Interfaces;

namespace Nexus.UserManagement.Service.Application.Abstractions.Repositories
{
    public interface IGenderReadOnlyRepository : IReadOnlyRepository<GenderResponse>
    {
        
    }
}