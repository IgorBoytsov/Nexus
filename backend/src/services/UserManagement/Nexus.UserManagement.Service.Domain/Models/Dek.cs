using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class Dek : AggregateRoot<DekId>
    {
        public UserId UserId { get; private set; }
        public EncryptedValue EncryptedValue { get; private set; }
        public CryptoVersion Version { get; private set; }
        public DekType Type { get; private set; }
        public DateTimeOffset UpdateAt { get; private set; }

        private Dek()
        {
            
        }

        private Dek(UserId userId, EncryptedValue encryptedValue, CryptoVersion version, DekType type) : base(DekId.New())
        {
            UserId = userId;
            EncryptedValue = encryptedValue;
            Version = version;
            Type = type;
            UpdateAt = DateTimeOffset.UtcNow;
        }

        public static Dek Create(UserId userId, EncryptedValue encryptedValue, CryptoVersion version, DekType type)
        {
            return new Dek(userId, encryptedValue, version, type);
        }

        public void Rotate(EncryptedValue encryptedValue, CryptoVersion version)
        {
            EncryptedValue = encryptedValue;
            Version = version;
            UpdateAt = DateTimeOffset.UtcNow;
        }
    }
}