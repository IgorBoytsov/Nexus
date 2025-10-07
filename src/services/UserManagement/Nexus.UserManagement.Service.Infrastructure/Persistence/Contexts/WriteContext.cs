using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts
{
    public sealed class WriteContext(DbContextOptions<WriteContext> options) : DbContext(options), IWriteDbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;
        public DbSet<Gender> Genders { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CountyWriteConfiguration());
            modelBuilder.ApplyConfiguration(new GenderWriteConfiguration());
            modelBuilder.ApplyConfiguration(new RoleWriteConfiguration());
            modelBuilder.ApplyConfiguration(new StatusWriteConfiguration());
            modelBuilder.ApplyConfiguration(new UserWriteConfiguration());
        }
    }
}