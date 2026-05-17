using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Enums;
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

            builder.HasDiscriminator<UserAuthenticatorType>(nameof(UserAuthenticator.Method))
                .HasValue<SrpAuthenticator>(UserAuthenticatorType.SRP)
                .HasValue<EmailAuthenticator>(UserAuthenticatorType.Email);

            builder.Property(ua => ua.CreatedAt)
                .HasColumnName(nameof(UserAuthenticator.CreatedAt))
                .IsRequired();

            builder.Property(ua => ua.LastUsedAt)
                .HasColumnName(nameof(UserAuthenticator.LastUsedAt))
                .IsRequired(false);

            builder.Property(ua => ua.IsActive)
                .HasColumnName(nameof(UserAuthenticator.IsActive))
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
