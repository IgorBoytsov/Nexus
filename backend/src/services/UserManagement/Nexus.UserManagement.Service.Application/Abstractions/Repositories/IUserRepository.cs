using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Kernel.Interfaces;

namespace Nexus.UserManagement.Service.Application.Abstractions.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> CheckAvailableEmail(Email email);
    }
}