using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Application.Abstractions.Models;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Read
{
    internal sealed class StatusReadConfiguration : IEntityTypeConfiguration<StatusView>
    {
        public void Configure(EntityTypeBuilder<StatusView> builder)
        {
            builder.ToView("V_Statuses");
            builder.HasKey(s => s.Id);
        }
    }
}