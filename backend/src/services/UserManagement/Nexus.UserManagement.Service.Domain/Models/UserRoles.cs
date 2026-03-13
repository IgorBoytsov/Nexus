using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class UserRoles
    {
        public UserId UserId { get; private set; }
        public RoleId RoleId { get; private set; }

        private UserRoles() { }

        private UserRoles(UserId userId, RoleId roleId) 
        {
            UserId = userId;
            RoleId = roleId;
        }

        public static UserRoles Create(Guid userId, Guid roleId) => new UserRoles(UserId.From(userId), RoleId.From(roleId));
    }
}