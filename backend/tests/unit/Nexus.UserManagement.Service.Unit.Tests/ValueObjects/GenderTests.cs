using Nexus.UserManagement.Service.Domain.ValueObjects.Gender;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class GenderTests
    {
        #region Gender - GenderName

        [Fact]
        public void GenderName_Create_ValidName_ReturnsGenderNameWithSameValue()
        {
            var name = "ValidName";
            GenderName assetName = GenderName.Create(name);

            Assert.Equal(name, assetName.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GenderName_Create_EmptyName_ThrowsArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => GenderName.Create(value!));
        }

        [Theory]
        [InlineData("a")] // MinLength = 2
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // MaxLength = 50, Inline = 51
        public void GenderName_Create_IncorrectRange_ThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => GenderName.Create(value));
        }

        #endregion
    }
}