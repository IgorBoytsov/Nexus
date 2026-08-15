using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.DomainModels
{
    public class RecoveryKeyTests
    {
        [Fact]
        public void RecoveryKey_Create_ValidValues_ReturnRecoveryKey()
        {
            UserId userId = UserId.New();
            EncryptedValue encryptedRecoveryKey = EncryptedValue.Create(Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 }));
            CryptoVersion cryptoVersion = CryptoVersion.Create(1);
            KeyHint keyHint = KeyHint.Create("env_v1");

            RecoveryKey recoveryKey = RecoveryKey.Create(userId, encryptedRecoveryKey, cryptoVersion, keyHint);

            Assert.True(Guid.TryParse(recoveryKey.Id.Value.ToString(), out var _));
            Assert.Equal(userId, recoveryKey.UserId);
            Assert.Equal(encryptedRecoveryKey, recoveryKey.EncryptedValue);
            Assert.Equal(cryptoVersion, recoveryKey.Version);
            Assert.Equal(keyHint, recoveryKey.KeyHint);
        }

        [Fact]
        public void MarkAsUsed_Mark_Correct_IsUserTrue()
        {
            RecoveryKey recoveryKey = RecoveryKey.Create(
                UserId.New(), 
                EncryptedValue.Create(Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 })), 
                CryptoVersion.Create(1), 
                KeyHint.Create("env_v1"));

            recoveryKey.MarkAsUsed();

            Assert.True(recoveryKey.IsUsed);
        }
    }
}