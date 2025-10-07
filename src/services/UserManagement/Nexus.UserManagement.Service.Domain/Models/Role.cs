using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class Role : Entity<Guid>
    {
        public RoleName Name { get; set; } = null!;

        private Role() { }

        public Role(Guid id, RoleName name) : base(id) => Name = name;

        public static Role Create(string name)
        {
            var roleNeame = RoleName.Create(name);

            return new Role(Guid.NewGuid(), roleNeame);
        }

        public void UpdateName(RoleName roleName)
        {
            if (Name != roleName)
                Name = roleName;
        }
    }
}