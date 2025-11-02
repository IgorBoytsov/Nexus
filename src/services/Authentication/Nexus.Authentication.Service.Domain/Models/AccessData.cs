namespace Nexus.Authentication.Service.Domain.Models
{
    public sealed class AccessData
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string RefreshToken { get; private set; } = null!;
        public string AccessToken { get; private set; } = null!;
        public DateTime CreationDate { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool IsUsed { get; private set; }
        public bool IsRevoked { get; private set; }

        private AccessData() { }

        private AccessData(Guid userId, string refreshToken, string accessToken, DateTime creationDate, DateTime expiryDate, bool isUsed, bool isRevoked)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RefreshToken = refreshToken;
            AccessToken = accessToken;
            CreationDate = creationDate;
            ExpiryDate = expiryDate;
            IsUsed = isUsed;
            IsRevoked = isRevoked;
        }

        public static AccessData Create(Guid userId, string refreshToken, string accessToken, DateTime creationDate, DateTime expiryDate, bool isUsed, bool isRevoked)
        {
            return new AccessData(userId, refreshToken, accessToken, creationDate, expiryDate, isUsed, isRevoked);
        }
    }
}