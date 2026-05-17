using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class SrpAuthenticator : UserAuthenticator
    {
        public Login? Login { get; private set; }
        public Verificator? Verificator { get; private set; }
        public Salt? Salt { get; private set; }

        private SrpAuthenticator()
        {
            
        }

        private SrpAuthenticator(UserId userId, Login login, Verificator verificator, Salt salt) 
        : base(UserAuthenticatorId.New(), userId, UserAuthenticatorType.SRP)
        {
            Login = login;
            Verificator = verificator;
            Salt = salt;
        }

        public static SrpAuthenticator Create(UserId userId, Login login, Verificator verificator, Salt salt)
        {
            return new SrpAuthenticator(userId, login, verificator, salt);
        }

        internal void Update(Login login, Verificator verifier, Salt salt)
        {
            Login = login;
            Verificator = verifier;
            Salt = salt;
        }

        public Verificator GetVerificator() => Verificator ?? throw new InvalidOperationException("SRP Verificator not initialized");
    }
}