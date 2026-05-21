using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Application.Abstractions.Contexts
{
    public interface IWriteDbContext
    {
        public DbSet<User> Users { get; }
        public DbSet<Dek> Deks { get; }
        public DbSet<RecoveryKey> RecoveryKeys { get; }
        public DbSet<UserAuthenticator> UserAuthenticators { get; }
        public DbSet<Role> Roles { get; }
        public DbSet<UserRoles> UserRoles { get; }
        public DbSet<Status> Statuses { get; }
        public DbSet<Gender> Genders { get; }
        public DbSet<Country> Countries { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}