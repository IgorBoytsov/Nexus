using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Gender;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class GenderWriteConfiguration : IEntityTypeConfiguration<Gender>
    {
        public void Configure(EntityTypeBuilder<Gender> builder)
        {
            builder.ToTable("Genders");

            builder.HasKey(x => x.Id);

            builder.Property(c => c.Id)
              .HasColumnName("Id")
              .ValueGeneratedNever();

            builder.Property(u => u.Name)
              .HasColumnName("Name")
              .HasConversion(
                  genderName => genderName.Value,
                  dbValue => GenderName.Create(dbValue))
              .HasMaxLength(GenderName.MAX_LENGTH)
              .UseCollation(PostgresConstants.COLLATION_NAME)
              .IsRequired();
        }
    }
}