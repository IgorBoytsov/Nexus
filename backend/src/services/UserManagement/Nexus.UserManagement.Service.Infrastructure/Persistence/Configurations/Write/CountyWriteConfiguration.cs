using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Country;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class CountyWriteConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("countries");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
              .HasColumnName("id")
              .ValueGeneratedNever();

            builder.Property(c => c.Name)
              .HasColumnName("name")
              .HasConversion(
                  genderName => genderName.Value,
                  dbValue => CountryName.Create(dbValue))
              .HasMaxLength(CountryName.MAX_LENGTH)
              .UseCollation(PostgresConstants.COLLATION_NAME)
              .IsRequired();
        }
    }
}