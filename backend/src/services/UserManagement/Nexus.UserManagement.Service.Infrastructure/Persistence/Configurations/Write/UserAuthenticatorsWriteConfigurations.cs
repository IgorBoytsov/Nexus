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
            builder.ToTable("user_authenticators");

            builder.HasKey(ua => ua.Id);

            builder.Property(ua => ua.Id)
                .HasConversion(
                    id => id.Value,
                    dbValue => UserAuthenticatorId.From(dbValue))
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(ua => ua.UserId)
                .HasColumnName("user_id")
                .HasConversion(
                    id => id.Value,
                    dbValue => UserId.From(dbValue))
                .IsRequired();

            builder.HasDiscriminator<UserAuthenticatorType>(nameof(UserAuthenticator.Method))
                .HasValue<SrpAuthenticator>(UserAuthenticatorType.SRP)
                .HasValue<EmailAuthenticator>(UserAuthenticatorType.Email);

            builder.Property(ua => ua.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(ua => ua.LastUsedAt)
                .HasColumnName("last_used_at")
                .IsRequired(false);

            builder.Property(ua => ua.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
