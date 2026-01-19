using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class UserCredentials : Entity<UserId>
    {
        public string Verifier { get; private set; } = null!;
        public string ClientSalt { get; private set; } = null!;
        public string EncryptedDek { get; private set; } = null!;

        private UserCredentials() { }

        internal UserCredentials(UserId userId, string verifier, string clientSalt, string encryptedDek) : base(userId)
        {
            Verifier = verifier;
            ClientSalt = clientSalt;
            EncryptedDek = encryptedDek;
        }

        internal void UpdateVerifier(string verifier) => Verifier = verifier;
    }
}