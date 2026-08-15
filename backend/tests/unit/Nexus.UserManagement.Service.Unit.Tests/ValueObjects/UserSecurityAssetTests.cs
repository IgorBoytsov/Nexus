using Nexus.UserManagement.Service.Domain.Exceptions;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class UserSecurityAssetTests
    {        
        #region UserAuthenticator - UserSecurityAssetId

        [Fact]
        public void UserSecurityAssetId_From_ValidGuid_ReturnUserSecurityAssetIdWithSameValue()
        {
            var value = Guid.NewGuid();
            UserSecurityAssetId userSecurityAssetId = UserSecurityAssetId.From(value);

            Assert.Equal(value, userSecurityAssetId.Value);
        }

        [Fact]
        public void UserSecurityAssetId_Create_GuidEmpty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => UserSecurityAssetId.From(Guid.Empty));
        }

        #endregion
        #region UserSecurityAsset - EncryptedValue

        [Fact]
        public void EncryptedValue_Create_ValidValue_ReturnEncryptedValue()
        {
            string encryptedValueBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5});
            EncryptedValue encryptedValue = EncryptedValue.Create(encryptedValueBase64);

            Assert.Equal(encryptedValueBase64, encryptedValue.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void EncryptedValue_Create_EmptyValue_ThrowEmptyValueException(string? value)
        {
            Assert.Throws<EmptyValueException>(() => EncryptedValue.Create(value!));
        }

        #endregion
    }
}