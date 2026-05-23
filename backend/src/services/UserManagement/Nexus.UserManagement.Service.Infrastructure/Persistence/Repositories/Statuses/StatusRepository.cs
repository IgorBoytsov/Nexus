using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Shared.EntityFramework;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Statuses
{
    internal sealed class StatusRepository(UserManagementContext context) : Repository<Status, UserManagementContext>(context), IStatusRepository
    {
        
    }
}