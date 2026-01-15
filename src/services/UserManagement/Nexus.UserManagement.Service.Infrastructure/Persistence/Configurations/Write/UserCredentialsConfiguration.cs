using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class UserCredentialsWriteConfiguration : IEntityTypeConfiguration<UserCredentials>
    {
        public void Configure(EntityTypeBuilder<UserCredentials> builder)
        {
            builder.ToTable("UserCredentials");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, dbValue => UserId.From(dbValue))
                .ValueGeneratedNever();

            builder.Property(c => c.PasswordHash)
                .HasConversion(
                    passHash => passHash.Value,
                    dbValue => PasswordHash.Create(dbValue))
                .HasColumnName("PasswordHash")
                .IsRequired();

            builder.Property(c => c.ClientSalt)
                .HasColumnName("ClientSalt")
                .IsRequired();

            builder.Property(c => c.EncryptedDek)
                .HasColumnName("EncryptedDek")
                .IsRequired();
        }
    }
}