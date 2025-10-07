using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class RoleWriteConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            builder.Property(c => c.Id)
              .HasColumnName("Id")
              .ValueGeneratedNever();

            builder.Property(r => r.Name)
                .HasColumnName("Name")
                .HasConversion(
                    roleName => roleName.Value,
                    dbValue => RoleName.Create(dbValue))
                .UseCollation(PostgresConstants.COLLATION_NAME)
                .IsRequired();
        }
    }
}