using Microsoft.EntityFrameworkCore;
using Nexus.Authentication.Service.Application;
using Nexus.Authentication.Service.Domain.Models;
using System.Reflection;

namespace Nexus.Authentication.Service.Infrastructure.Persistence.Contexts
{
    public sealed class AuthenticationContext(DbContextOptions<AuthenticationContext> potions) : DbContext(potions), IApplicationDbContext
    {
        public DbSet<AccessData> AccessData { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}