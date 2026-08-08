using Nexus.UserManagement.Service.Domain.Exceptions;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class UserAuthenticatorTests
    {
        #region UserAuthenticator - AsymmetricKeyId

        [Fact]
        public void AsymmetricKeyId_Create_ValidGuid_ReturnsAsymmetricKeyIdWithSameValue()
        {
            var value = "env_v1";
            AsymmetricKeyId recoveryKeyId = AsymmetricKeyId.Create(value);

            Assert.Equal(value, recoveryKeyId.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AsymmetricKeyId_Create_EmptyValue_ThrowsEmptyValueException(string? value)
        {
            Assert.Throws<EmptyValueException>(() => AsymmetricKeyId.Create(value!));
        }

        #endregion
        
        #region UserAuthenticator - CredentialBlob

        [Fact]
        public void CredentialBlob_Create_ValidValue_ReturnCredentialBlob()
        {
            byte[] encryptedData = { 1, 2, 3, 4, 5 };
            string base64 = Convert.ToBase64String(encryptedData);
            CredentialBlob blob = CredentialBlob.Create(base64);
            byte[] decryptedData = Convert.FromBase64String(blob.Value);

            Assert.Equal(encryptedData, decryptedData);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CredentialBlob_Create_EmptyValue_ThrowEmptyValueException(string? value)
        {
            Assert.Throws<EmptyValueException>(() => CredentialBlob.Create(value!));
        }

        #endregion
        
        #region UserAuthenticator - CryptoVersion

        [Fact]
        public void CryptoVersion_Create_ValidValue_ReturnCryptoVersion()
        {
            int version = 1;
            CryptoVersion cryptoVersion = CryptoVersion.Create(1);

            Assert.Equal(version, cryptoVersion.Value);
        }

        [Fact]
        public void CryptoVersion_Create_NegativeValue_ThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CryptoVersion.Create(-1));
        }

        #endregion
        
        #region UserAuthenticator - IdentityIdentifier

        [Fact]
        public void IdentityIdentifier_Create_ValidValue_ReturnIdentityIdentifier()
        {
            string encryptedVerifier = "fNERGUGRFNMUJFJEKFNGJG";
            IdentityIdentifier identityIdentifier = IdentityIdentifier.Create(encryptedVerifier);

            Assert.Equal(encryptedVerifier, identityIdentifier.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void IdentityIdentifier_Create_EmptyValue_ThrowEmptyValueException(string? value)
        {
            Assert.Throws<EmptyValueException>(() => IdentityIdentifier.Create(value!));
        }

        #endregion
        
        #region UserAuthenticator - Salt

        [Fact]
        public void Salt_Create_ValidValue_ReturnIdentityIdentifier()
        {
            string encryptedVerifier = "fNERGUGRFNMUJFJEKFNGJG";
            Salt salt = Salt.Create(encryptedVerifier);

            Assert.Equal(encryptedVerifier, salt.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Salt_Create_EmptyValue_ThrowEmptyValueException(string? value)
        {
            Assert.Throws<EmptyValueException>(() => Salt.Create(value!));
        }

        #endregion
        
        #region UserAuthenticator - SrpVersion

        [Fact]
        public void SrpVersion_Create_ValidValue_ReturnSrpVersion()
        {
            int version = 1;
            SrpVersion srpVersion = SrpVersion.Create(1);

            Assert.Equal(version, srpVersion.Value);
        }

        [Fact]
        public void SrpVersion_Create_NegativeValue_ThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CryptoVersion.Create(-1));
        }

        #endregion
        
        #region UserAuthenticator - UserAuthenticatorId
        
        [Fact]
        public void UserAuthenticatorId_From_ValidGuid_ReturnsUserAuthenticatorIdWithSameValue()
        {
            var value = Guid.NewGuid();
            UserAuthenticatorId userAuthenticatorId = UserAuthenticatorId.From(value);

            Assert.Equal(value, userAuthenticatorId.Value);
        }

        [Fact]
        public void UserAuthenticatorId_Create_EmptyGuid_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => UserAuthenticatorId.From(Guid.Empty));
        }

        #endregion
        
        #region UserAuthenticator - Verificator

        [Fact]
        public void Verificator_Create_ValidValue_ReturnVerificator()
        {
            string verificatorBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });
            Verificator salt = Verificator.Create(verificatorBase64);

            Assert.Equal(verificatorBase64, salt.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Verificator_Create_EmptyValue_ThrowEmptyValueException(string? value)
        {
            Assert.Throws<EmptyValueException>(() => Salt.Create(value!));
        }

        #endregion
    }
}