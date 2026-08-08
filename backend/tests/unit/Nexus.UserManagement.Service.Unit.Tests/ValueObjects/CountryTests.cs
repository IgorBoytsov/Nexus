using Nexus.UserManagement.Service.Domain.ValueObjects.Country;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class CountryTests
    {
        #region Country - CountryName

        [Fact]
        public void CountryName_Create_ValidName_ReturnsCountryNameWithSameValue()
        {
            var name = "ValidName";
            CountryName assetName = CountryName.Create(name);

            Assert.Equal(name, assetName.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CountryName_Create_EmptyName_ThrowsArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => CountryName.Create(value!));
        }

        [Theory]
        [InlineData("a")] // MinLength = 2
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // MaxLength = 100, Inline = 105
        public void CountryName_Create_IncorrectRange_ThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => CountryName.Create(value));
        }

        #endregion
    }
}