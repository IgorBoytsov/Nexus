using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class Role : AggregateRoot<RoleId>
    {
        public RoleName Name { get; set; } = null!;

        private Role() { }

        private Role(RoleId id, RoleName name) : base(id) => Name = name;

        public static Role Create(string name)
        {
            var roleName = RoleName.Create(name);

            return new Role(RoleId.New(), roleName);
        }

        public void UpdateName(RoleName roleName)
        {
            if (Name != roleName)
                Name = roleName;
        }
    }
}