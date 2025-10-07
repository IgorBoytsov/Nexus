using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Application.Abstractions.Models;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Read
{
    internal sealed class UserReadConfiguration : IEntityTypeConfiguration<UserView>
    {
        public void Configure(EntityTypeBuilder<UserView> builder)
        {
            builder.ToView("V_Users");
            builder.HasKey(u => u.Id);
        }
    }
}