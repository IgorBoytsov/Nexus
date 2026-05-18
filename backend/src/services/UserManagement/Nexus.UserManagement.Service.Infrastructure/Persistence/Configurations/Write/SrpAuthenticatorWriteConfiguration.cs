using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    public sealed class SrpAuthenticatorWriteConfiguration : IEntityTypeConfiguration<SrpAuthenticator>
    {
        public void Configure(EntityTypeBuilder<SrpAuthenticator> builder)
        {

            builder.Property(x => x.Login)
                .HasColumnName("SrpLogin")
                .HasConversion(
                    l => l.HasValue ? l.Value.Value : null,
                    db => string.IsNullOrWhiteSpace(db) ? null : Login.Create(db))
                .IsRequired(false);

            builder.Property(x => x.EncryptedVerifier)
                .HasColumnName("SrpEncryptedVerifier")
                .HasConversion(
                    v => v.HasValue ? v.Value.Value : null,
                    db => string.IsNullOrWhiteSpace(db) ? null : Verificator.Create(db))
                .IsRequired(false);

            builder.Property(x => x.Salt)
                .HasColumnName("SrpSalt")
                .HasConversion(
                    s => s.HasValue ? s!.Value.Value : null,
                    db => string.IsNullOrWhiteSpace(db) ? null : Salt.Create(db))
                .IsRequired(false);

            builder.Property(x => x.SrpVersion)
                .HasColumnName("SrpVersion")
                .HasConversion(
                    v => v.HasValue ? v.Value.Value : (int?)null,
                    db => db.HasValue ? SrpVersion.Create(db.Value) : null)
                .IsRequired(false);

            builder.Property(x => x.EncryptedVerifierWrapKey)
                .HasColumnName("SrpEncryptedVerifierWrapKey")
                .HasConversion(
                    evw => evw.HasValue ? evw.Value.Value : null,
                    db => string.IsNullOrWhiteSpace(db) ? null : CredentialBlob.Create(db))
                .IsRequired(false);

            builder.Property(x => x.KeyWrapVersion)
                .HasColumnName("SrpKeyWrapVersion")
                .HasConversion(
                    kv => kv.HasValue ? kv.Value.Value : (int?)null, 
                    db => db == null ? null : CryptoVersion.Create(db.Value))
                .IsRequired(false);

            builder.Property(x => x.AsymmetricKeyId)
                .HasColumnName("SrpAsymmetricKeyId")
                .HasConversion(
                    ak => ak.HasValue ? ak.Value.Value : null,
                    db => string.IsNullOrWhiteSpace(db) ? null : AsymmetricKeyId.Create(db))
                .IsRequired(false);
        }
    }
}