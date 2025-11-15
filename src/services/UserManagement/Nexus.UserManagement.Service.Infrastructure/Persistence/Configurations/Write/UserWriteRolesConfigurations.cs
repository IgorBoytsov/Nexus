using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class UserWriteRolesConfigurations : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {
            builder.ToTable("UserRoles");
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Property(c => c.UserId)
              .HasColumnName("UserId")
              .HasConversion(roleId => roleId.Value, dbValue => UserId.From(dbValue))
              .ValueGeneratedNever();

            builder.Property(c => c.RoleId)
              .HasColumnName("RoleId")
              .HasConversion(roleId => roleId.Value, dbValue => RoleId.From(dbValue))
              .ValueGeneratedNever();

            builder.HasOne<User>()
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Role>()
                .WithMany() 
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}