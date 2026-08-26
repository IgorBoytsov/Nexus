using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Shared.EntityFramework;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Roles
{
    internal sealed class RoleRepository(UserManagementContext context) : Repository<Role, UserManagementContext>(context), IRoleRepository
    {
        
    }
}