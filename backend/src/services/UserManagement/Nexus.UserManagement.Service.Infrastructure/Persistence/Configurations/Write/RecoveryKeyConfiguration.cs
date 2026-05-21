using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.RecoveryKeys;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    public sealed class RecoveryKeyConfiguration : IEntityTypeConfiguration<RecoveryKey>
    {
        public void Configure(EntityTypeBuilder<RecoveryKey> builder)
        {
            builder.ToTable("recovery_keys");
            builder.HasKey(rk => rk.Id);

            builder.Property(rk => rk.Id)
                .HasColumnName("id")
                .HasConversion(i => i.Value, db => RecoveryKeyId.From(db))
                .ValueGeneratedNever();
                
            builder.Property(rk => rk.UserId)
                .HasColumnName("user_id")
                .HasConversion(u => u.Value, db => UserId.From(db))
                .IsRequired(true);
                
            builder.Property(rk => rk.EncryptedValue)
                .HasColumnName("encrypted_value")
                .HasConversion(ev => ev.Value, db => EncryptedValue.Create(db))
                .IsRequired(true);

            builder.Property(rk => rk.Version)
                .HasColumnName("crypto_version")
                .HasConversion(cv => cv.Value, db => CryptoVersion.Create(db))
                .IsRequired(true);
       
            builder.Property(rk => rk.KeyHint)
                .HasColumnName("key_hint")
                .HasConversion(kh => kh.Value, db => KeyHint.Create(db))
                .IsRequired(true);

            builder.Property(rk => rk.IsUsed)
                .HasColumnName("is_used")
                .HasDefaultValue(false);

            builder.Property(rk => rk.UsedAt)
                .HasColumnName("used_at")
                .IsRequired(false);
                
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(d => d.UserId) 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}