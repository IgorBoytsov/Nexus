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

            builder.Property(x => x.Verificator)
                .HasColumnName("SrpVerificator")
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
        }
    }
}