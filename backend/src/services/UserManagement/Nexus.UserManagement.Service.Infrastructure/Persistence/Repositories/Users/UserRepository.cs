using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Shared.EntityFramework;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users
{
    internal sealed class UserRepository(UserManagementContext context) : Repository<User, UserManagementContext>(context), IUserRepository
    {
        public async Task<bool> CheckAvailableEmail(Email email) => await _entity.AnyAsync(u => u.Email == email);
    }
}