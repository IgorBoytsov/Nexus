using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts
{
    public sealed class UserManagementContext(DbContextOptions<UserManagementContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Dek> Deks { get; set; } = null!;
        public DbSet<RecoveryKey> RecoveryKeys { get; set; } = null!;
        public DbSet<UserAuthenticator> UserAuthenticators { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRoles> UserRoles { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;
        public DbSet<Gender> Genders { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}