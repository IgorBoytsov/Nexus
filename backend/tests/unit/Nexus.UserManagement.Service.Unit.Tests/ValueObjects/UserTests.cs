using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class UserTests
    {
        #region User - UserId

        [Fact]
        public void UserId_From_ValidGuid_ReturnsUserIdWithSameValue()
        {
            var guid = Guid.NewGuid();
            UserId recoveryKeyId = UserId.From(guid);

            Assert.NotEqual(Guid.Empty, recoveryKeyId.Value);
            Assert.Equal(guid, recoveryKeyId.Value);
        }

        [Fact]
        public void UserId_New_ReturnsNonEmptyUserId()
        {
            UserId dekId = UserId.New();

            Assert.NotEqual(Guid.Empty, dekId.Value);
        }

        [Fact]
        public void UserId_From_EmptyGuid_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => UserId.From(Guid.Empty));
        }

        #endregion

        #region User - Email

        [Fact]
        public void Email_Create_ValidValue_ReturnEmail()
        {
            string value = "test@test.com";
            Email email = Email.Create(value);

            Assert.Equal(value, email.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Email_Create_EmptyValue_ThrowArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => Email.Create(value!));
        }

        [Theory]
        [InlineData("@test.ru")]
        [InlineData("test")]
        [InlineData(".ru")]
        public void Email_Create_IncorrectFormatValue_ThrowArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => Email.Create(value!));
        }

        #endregion

        #region User - Login

        [Fact]
        public void Login_Create_ValidValue_ReturnLogin()
        {
            string value = "login";
            Login login = Login.Create(value);

            Assert.Equal(value, login.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Login_EmptyValue_ThrowArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => Login.Create(value!));
        }

        [Theory]
        [InlineData("lo-gin")]
        [InlineData("логин")]
        [InlineData("❤️")]
        [InlineData("!@#$FFF")]
        public void Login_IncorrectValue_ThrowArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => Login.Create(value));
        }

        [Theory]
        [InlineData("ab")] // MinValue = 3
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // MaxValue = 50. Inline = 51
        public void Login_RangeIncorrect_ThrownArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => Login.Create(value));
        }

        #endregion

        #region User - UserName

        [Fact]
        public void UserName_Create_ValidName_ReturnsUserNameWithSameValue()
        {
            var name = "ValidName";
            UserName roleName = UserName.Create(name);

            Assert.Equal(name, roleName.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void UserName_Create_EmptyName_ThrowsArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => UserName.Create(value!));
        }

        [Theory]
        [InlineData("b")] // MinValue = 2
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // MaxValue = 100. Inline = 101
        public void UserName_RangeIncorrect_ThrownArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => UserName.Create(value));
        }

        #endregion
    }
}