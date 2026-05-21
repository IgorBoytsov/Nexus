using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public abstract class UserAuthenticator : AggregateRoot<UserAuthenticatorId>
    {
        public UserId UserId { get; private set; }
        public UserAuthenticatorType Method { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? LastUsedAt { get; protected set; } 
        public bool IsActive { get; protected set; } = true;

        protected UserAuthenticator()
        {
            
        }

        protected UserAuthenticator(UserAuthenticatorId id, UserId userId, UserAuthenticatorType method) : base(id)
        {
            UserId = userId;
            Method = method;
        }

        public void MarkUsed() => LastUsedAt = DateTime.UtcNow;
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}