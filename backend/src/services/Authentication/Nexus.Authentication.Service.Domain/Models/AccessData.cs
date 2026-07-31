using Shared.Kernel.Primitives;

namespace Nexus.Authentication.Service.Domain.Models
{
    public sealed class AccessData : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public string RefreshTokenHash { get; private set; } = null!;
        public DateTime CreationDate { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool IsUsed { get; private set; }
        public bool IsRevoked { get; private set; }

        private AccessData() { }

        private AccessData(Guid userId, string refreshTokenHash, DateTime creationDate, DateTime expiryDate, bool isUsed, bool isRevoked)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RefreshTokenHash = refreshTokenHash;
            CreationDate = creationDate;
            ExpiryDate = expiryDate;
            IsUsed = isUsed;
            IsRevoked = isRevoked;
        }

        public static AccessData Create(Guid userId, string refreshTokenHash, DateTime creationDate, DateTime expiryDate, bool isUsed, bool isRevoked)
        {
            return new AccessData(userId, refreshTokenHash, creationDate, expiryDate, isUsed, isRevoked);
        }

        public void MarkAsUsed() => IsUsed = true;
    }
}