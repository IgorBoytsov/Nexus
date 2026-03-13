using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class UserAuthenticator : Entity<UserAuthenticatorId>
    {
        public UserId UserId { get; private set; }
        public UserAuthenticatorType Method { get; private set; }
        public IdentityIdentifier Identifier { get; private set; } 
        public CredentialBlob? CredentialData { get; private set; } 
        public string? Salt { get; private set; }

        private UserAuthenticator()
        {
            
        }

        private UserAuthenticator(UserAuthenticatorId id, UserId userId, UserAuthenticatorType method, IdentityIdentifier identifier, CredentialBlob? creditialData, string? salt) : base(id)
        {
            UserId = userId;
            Method = method;
            Identifier = identifier;
            CredentialData = creditialData;
            Salt = salt;
        }

        internal static UserAuthenticator Create(UserId userId,UserAuthenticatorType method, IdentityIdentifier identifier, CredentialBlob? creditialData, string? salt)
            => new(UserAuthenticatorId.New(), userId, method, identifier, creditialData, salt);
    }
}