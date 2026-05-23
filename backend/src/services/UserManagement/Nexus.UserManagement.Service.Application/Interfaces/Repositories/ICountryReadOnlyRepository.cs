using Shared.Contracts.UserManagement.Responses;
using Shared.Kernel.Interfaces;

namespace Nexus.UserManagement.Service.Application.Interfaces.Repositories
{
    public interface ICountryReadOnlyRepository : IReadOnlyRepository<CountryResponse>
    {
        
    }
}