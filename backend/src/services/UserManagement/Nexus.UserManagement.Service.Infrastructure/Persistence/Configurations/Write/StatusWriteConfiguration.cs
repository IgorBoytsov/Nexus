using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Status;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class StatusWriteConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.ToTable("Statuses");

            builder.HasKey(r => r.Id);

            builder.Property(c => c.Id)
              .HasColumnName("Id")
              .ValueGeneratedNever();

            builder.Property(r => r.Name)
                .HasColumnName("Name")
                .HasConversion(
                    roleName => roleName.Value,
                    dbValue => StatusName.Create(dbValue))
                .UseCollation(PostgresConstants.COLLATION_NAME)
                .IsRequired();
        }
    }
}