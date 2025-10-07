using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Application.Abstractions.Models;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Read
{
    internal sealed class GenderReadConfiguration : IEntityTypeConfiguration<GenderView>
    {
        public void Configure(EntityTypeBuilder<GenderView> builder)
        {
            builder.ToView("V_Genders");
            builder.HasKey(g => g.Id);
        }
    }
}