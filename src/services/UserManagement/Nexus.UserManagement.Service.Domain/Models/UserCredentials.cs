using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class UserCredentials : Entity<UserId>
    {
        public PasswordHash PasswordHash { get; private set; } = null!;
        public string ClientSalt { get; private set; } = null!;
        public string EncryptedDek { get; private set; } = null!;

        private UserCredentials() { }

        internal UserCredentials(UserId userId, PasswordHash passwordHash, string clientSalt, string encryptedDek) : base(userId)
        {
            PasswordHash = passwordHash;
            ClientSalt = clientSalt;
            EncryptedDek = encryptedDek;
        }

        internal void UpdatePassword(PasswordHash newHash) => PasswordHash = newHash;
    }
}