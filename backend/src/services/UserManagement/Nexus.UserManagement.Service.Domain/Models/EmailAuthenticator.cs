using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class EmailAuthenticator : UserAuthenticator
    {
        public Email? Email { get; private set; }

        private EmailAuthenticator()
        {
            
        }

        private EmailAuthenticator(UserId userId, Email email) : base(UserAuthenticatorId.New(), userId, Enums.UserAuthenticatorType.Email)
        {
            Email = email;
        }

        public static EmailAuthenticator Create(UserId userId, Email email)
        {
            return new EmailAuthenticator(userId, email);
        }

        public void Update(Email email)
        {
            Email = email;
        }
    }
}