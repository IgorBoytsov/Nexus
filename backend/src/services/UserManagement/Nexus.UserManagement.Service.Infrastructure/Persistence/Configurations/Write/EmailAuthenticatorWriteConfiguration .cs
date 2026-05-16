using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    public sealed class EmailAuthenticatorWriteConfiguration : IEntityTypeConfiguration<EmailAuthenticator>
    {
        public void Configure(EntityTypeBuilder<EmailAuthenticator> builder)
        {
            builder.Property(x => x.Email)
                .HasColumnName("Email")
                .HasConversion(
                    e => e.HasValue ? e.Value.Value : null,
                    db => string.IsNullOrWhiteSpace(db) ? null : Email.Create(db))
                .UseCollation(PostgresConstants.COLLATION_NAME)
                .IsRequired(false);
        }
    }
}