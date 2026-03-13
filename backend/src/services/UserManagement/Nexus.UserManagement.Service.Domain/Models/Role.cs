using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class Role : Entity<RoleId>
    {
        public RoleName Name { get; set; } = null!;

        private Role() { }

        public Role(RoleId id, RoleName name) : base(id) => Name = name;

        public static Role Create(string name)
        {
            var roleNeame = RoleName.Create(name);

            return new Role(RoleId.New(), roleNeame);
        }

        public void UpdateName(RoleName roleName)
        {
            if (Name != roleName)
                Name = roleName;
        }
    }
}