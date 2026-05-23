using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.RecoveryKeys;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Shared.Kernel.Errors;
using Shared.Kernel.Exceptions;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class RecoveryKey : Entity<RecoveryKeyId>
    {
        public UserId UserId { get; private set; }
        public EncryptedValue EncryptedValue { get; private set; }
        public CryptoVersion Version { get; private set; }
        public KeyHint KeyHint { get; private set; }
        public bool IsUsed {get; private set; }
        public DateTimeOffset? UsedAt { get; private set; }

        private RecoveryKey()
        {
            
        }

        private RecoveryKey(UserId userId, EncryptedValue encryptedValue, CryptoVersion version, KeyHint keyHint) : base(RecoveryKeyId.New())
        {
            UserId = userId;
            EncryptedValue = encryptedValue;
            Version = version;
            KeyHint = keyHint;
            IsUsed = false;
        }

        internal static RecoveryKey Create(UserId userId, EncryptedValue encryptedValue, CryptoVersion version, KeyHint keyHint)
        {
            return new RecoveryKey(userId, encryptedValue, version, keyHint);
        }

        internal void MarkAsUsed()
        {
            if (IsUsed)
                throw new DomainException(new Error(AppErrors.AlreadyUsed, "This recovery key has already been used."));

            IsUsed = true;
            UsedAt = DateTimeOffset.UtcNow;
        }
    }
}