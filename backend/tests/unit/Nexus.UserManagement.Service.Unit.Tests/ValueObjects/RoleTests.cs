using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class RoleTests
    {
        #region Role - RoleId

        [Fact]
        public void RoleId_From_ValidGuid_ReturnsRoleIdWithSameValue()
        {
            var guid = Guid.NewGuid();
            RoleId roleId = RoleId.From(guid);

            Assert.NotEqual(Guid.Empty, roleId.Value);
            Assert.Equal(guid, roleId.Value);
        }

        [Fact]
        public void RoleId_New_ReturnsNonEmptyRoleId()
        {
            RoleId roleId = RoleId.New();

            Assert.NotEqual(Guid.Empty, roleId.Value);
        }

        [Fact]
        public void RoleId_From_EmptyGuid_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => RoleId.From(Guid.Empty));
        }

        #endregion

        #region Role - RoleName

        [Fact]
        public void RoleName_Create_ValidName_ReturnsRoleNameWithSameValue()
        {
            var name = "ValidName";
            RoleName roleName = RoleName.Create(name);

            Assert.Equal(name, roleName.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RoleName_Create_EmptyName_ThrowsArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => RoleName.Create(value!));
        }

        #endregion
    }
}