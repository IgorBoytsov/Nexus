using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class UserAuthenticatorsWriteConfigurations : IEntityTypeConfiguration<UserAuthenticator>
    {
        public void Configure(EntityTypeBuilder<UserAuthenticator> builder)
        {
            builder.ToTable("UserAuthenticators");

            builder.HasKey(ua => ua.Id);

            builder.Property(ua => ua.Id)
                .HasConversion(
                    id => id.Value,
                    dbValue => UserAuthenticatorId.From(dbValue))
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(ua => ua.UserId)
                .HasConversion(
                    id => id.Value,
                    dbValue => UserId.From(dbValue))
                .IsRequired();

            builder.Property(ua => ua.Method)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(ua => ua.Identifier)
                .HasConversion(
                    identifier => identifier.Value,
                    dbValue => IdentityIdentifier.Create(dbValue))
                .HasColumnName("Identifier")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(ua => ua.CredentialData)
                .HasConversion(
                     credData => credData.HasValue ? credData.Value.Value : null,
                     dbValue => string.IsNullOrEmpty(dbValue) ? null : CredentialBlob.Create(dbValue))
                .HasColumnName("CredentialData")
                .IsRequired(false);

            builder.Property(ua => ua.Salt)
                .HasMaxLength(256)
                .IsRequired(false);
        }
    }
}
