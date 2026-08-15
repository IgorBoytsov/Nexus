using Nexus.UserManagement.Service.Domain.ValueObjects.Status;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class StatusTests
    {
        #region Status - StatusName

        [Fact]
        public void StatusName_Create_ValidName_ReturnsStatusNameWithSameValue()
        {
            var name = "ValidName";
            StatusName statusName = StatusName.Create(name);

            Assert.Equal(name, statusName.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void StatusName_Create_EmptyName_ThrowsArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => StatusName.Create(value!));
        }

        #endregion
    }
}