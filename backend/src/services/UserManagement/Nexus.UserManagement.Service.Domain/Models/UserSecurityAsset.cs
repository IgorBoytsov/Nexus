using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class UserSecurityAsset : Entity<UserSecurityAssetId>
    {
        public UserId UserId { get; private set; }
        public AssetType AssetType { get; private set; }
        public EncryptedAssetValue EncryptedValue { get; private set; }  
        public int CryptoVersion { get; private set; } 

        private UserSecurityAsset()
        {

        }

        private UserSecurityAsset(UserSecurityAssetId id, UserId userId, AssetType assetType, EncryptedAssetValue encryptedAssetValue, int cryptoVersion) : base(id)
        {
            UserId = userId;
            AssetType = assetType;
            EncryptedValue = encryptedAssetValue;
            CryptoVersion = cryptoVersion;
        }

        internal static UserSecurityAsset Create(UserId userId, AssetType assetType, EncryptedAssetValue encryptedAssetValue, int cryptoVersion)
            => new(UserSecurityAssetId.New(), userId, assetType, encryptedAssetValue, cryptoVersion);

        internal void UpdateMainDek(EncryptedAssetValue encryptedAssetValue, int cryptoVersion)
        {
            EncryptedValue = encryptedAssetValue;
            CryptoVersion = cryptoVersion;
        }
    }
}