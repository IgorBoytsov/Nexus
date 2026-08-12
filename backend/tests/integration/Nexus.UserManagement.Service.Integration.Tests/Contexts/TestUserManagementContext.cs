using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.UserManagement.Service.Integration.Tests.Contexts
{
    public class TestUserManagementContext(DbContextOptions<UserManagementContext> options) : UserManagementContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserManagementContext).Assembly);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.GetCollation() == PostgresConstants.COLLATION_NAME)
                    {
                        property.SetCollation("NOCASE");
                    }
                }
            }
        }
    }
}