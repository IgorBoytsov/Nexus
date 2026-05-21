using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    public sealed class DekConfiguration : IEntityTypeConfiguration<Dek>
    {
        public void Configure(EntityTypeBuilder<Dek> builder)
        {
            builder.ToTable("deks");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasColumnName("id")
                .HasConversion(i => i.Value, db => DekId.From(db))
                .ValueGeneratedNever();

            builder.Property(d => d.UserId)
                .HasColumnName("user_id")
                .HasConversion(u => u.Value, db => UserId.From(db))
                .IsRequired();

            builder.Property(d => d.EncryptedValue)
                .HasColumnName("encrypted_value")
                .HasConversion(ev => ev.Value, db => EncryptedValue.Create(db))
                .IsRequired();

            builder.Property(d => d.Version)
                .HasColumnName("crypto_version")
                .HasConversion(cv => cv.Value, db => CryptoVersion.Create(db))
                .IsRequired();

            builder.Property(d => d.Type)
                .HasColumnName("dek_type")
                .HasConversion(
                    dekType => dekType.Value,
                    dbValue => DekType.FromValue(dbValue))
                .IsRequired();

            builder.Property(d => d.UpdateAt)
                .HasColumnName("update_at")
                .IsRequired();
                
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(d => d.UserId) 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}