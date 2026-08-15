using Nexus.UserManagement.Service.Domain.ValueObjects.RecoveryKeys;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class RecoveryKeyTests
    {
        #region RecoveryKey - RecoveryKeyId

        [Fact]
        public void RecoveryKeyId_From_ValidGuid_ReturnsRecoveryKeyIdWithSameValue()
        {
            var guid = Guid.NewGuid();
            RecoveryKeyId recoveryKeyId = RecoveryKeyId.From(guid);

            Assert.NotEqual(Guid.Empty, recoveryKeyId.Value);
            Assert.Equal(guid, recoveryKeyId.Value);
        }

        [Fact]
        public void RecoveryKeyId_New_ReturnsNonEmptyRecoveryKeyId()
        {
            RecoveryKeyId dekId = RecoveryKeyId.New();

            Assert.NotEqual(Guid.Empty, dekId.Value);
        }

        [Fact]
        public void RecoveryKeyId_From_EmptyGuid_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => RecoveryKeyId.From(Guid.Empty));
        }

        #endregion
    }
}