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
        public EncryptionMetadata EncryptionMetadata { get; private set; } 

        private UserSecurityAsset()
        {

        }

        private UserSecurityAsset(UserSecurityAssetId id, UserId userId, AssetType assetType, EncryptedAssetValue encryptedAssetValue, EncryptionMetadata encryptionMetadata) : base(id)
        {
            UserId = userId;
            AssetType = assetType;
            EncryptedValue = encryptedAssetValue;
            EncryptionMetadata = encryptionMetadata;
        }

        internal static UserSecurityAsset Create(UserId userId, AssetType assetType, EncryptedAssetValue encryptedAssetValue, EncryptionMetadata encryptionMetadata)
            => new(UserSecurityAssetId.New(), userId, assetType, encryptedAssetValue, encryptionMetadata);

    }
}