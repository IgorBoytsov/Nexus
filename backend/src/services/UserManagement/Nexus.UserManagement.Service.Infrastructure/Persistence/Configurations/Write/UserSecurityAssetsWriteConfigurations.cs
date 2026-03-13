using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using System.Text.Json;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class UserSecurityAssetsWriteConfigurations : IEntityTypeConfiguration<UserSecurityAsset>
    {
        public void Configure(EntityTypeBuilder<UserSecurityAsset> builder)
        {
            builder.ToTable("UserSecurityAssets");

            builder.HasKey(us => us.Id);

            builder.Property(us => us.Id)
                .HasConversion(
                    id => id.Value,
                    dbValue => UserSecurityAssetId.From(dbValue))
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(ua => ua.AssetType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(ua => ua.EncryptedValue)
                .HasConversion(
                    encValue => encValue.Value,
                    dbValue => EncryptedAssetValue.Create(dbValue))
                .HasColumnName("EncryptedValue")
                .IsRequired();

            builder.Property(x => x.EncryptionMetadata)
                .HasColumnName("Metadata")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<EncryptionMetadata>(v, (JsonSerializerOptions?)null))
                .IsRequired();
        }
    }
}