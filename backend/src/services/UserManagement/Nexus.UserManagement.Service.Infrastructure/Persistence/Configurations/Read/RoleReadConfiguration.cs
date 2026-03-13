using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Application.Abstractions.Models;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Read
{
    internal sealed class RoleReadConfiguration : IEntityTypeConfiguration<RoleView>
    {
        public void Configure(EntityTypeBuilder<RoleView> builder)
        {
            builder.ToView("V_Roles");
            builder.HasKey(r => r.Id);
        }
    }
}