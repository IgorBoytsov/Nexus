using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Domain.Models;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;
using Shared.EntityFramework;

namespace Nexus.Authentication.Service.Infrastructure.Persistence.Repositories.AccessDatas
{
    internal sealed class AccessDataRepository(AuthenticationContext context) : Repository<AccessData, AuthenticationContext>(context), IAccessDataRepository
    {
        
    }
}