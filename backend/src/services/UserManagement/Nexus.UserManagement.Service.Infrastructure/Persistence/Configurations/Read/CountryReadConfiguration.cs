using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Application.Abstractions.Models;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Read
{
    internal sealed class CountryReadConfiguration : IEntityTypeConfiguration<CountryView>
    {
        public void Configure(EntityTypeBuilder<CountryView> builder)
        {
            builder.ToView("V_Countries");
            builder.HasKey(c => c.Id);
        }
    }
}