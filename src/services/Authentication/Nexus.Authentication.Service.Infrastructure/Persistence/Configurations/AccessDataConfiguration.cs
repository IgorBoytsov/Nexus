using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Authentication.Service.Domain.Models;

namespace Nexus.Authentication.Service.Infrastructure.Persistence.Configurations
{
    internal sealed class AccessDataConfiguration : IEntityTypeConfiguration<AccessData>
    {
        public void Configure(EntityTypeBuilder<AccessData> builder)
        {
            builder.ToTable("AccessData");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.RefreshToken).IsRequired();
            builder.Property(x => x.AccessToken).IsRequired();
            builder.Property(x => x.CreationDate).IsRequired();
            builder.Property(x => x.ExpiryDate).IsRequired();
            builder.Property(x => x.IsUsed).IsRequired();
            builder.Property(x => x.IsRevoked).IsRequired();
        }
    }
}