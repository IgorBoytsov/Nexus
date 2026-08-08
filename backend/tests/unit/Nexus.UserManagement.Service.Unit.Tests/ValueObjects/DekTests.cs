using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Shared.Kernel.Exceptions;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class DekTests
    {
        #region Dek - DekId

        [Fact]
        public void DekId_From_ValidGuid_ReturnsDekIdWithSameValue()
        {
            var guid = Guid.NewGuid();
            DekId dekId = DekId.From(guid);

            Assert.NotEqual(Guid.Empty, dekId.Value);
            Assert.Equal(guid, dekId.Value);
        }

        [Fact]
        public void DekId_New_ReturnsNonEmptyDekId()
        {
            DekId dekId = DekId.New();

            Assert.NotEqual(Guid.Empty, dekId.Value);
        }

        [Fact]
        public void DekId_From_EmptyGuid_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DekId.From(Guid.Empty));
        }

        #endregion

        #region  Dek - DekType

        [Fact]
        public void DekType_FromName_ValidName_ReturnDekType()
        {
            string name = DekType.Main.Name;
            DekType dekType = DekType.FromName(name);

            Assert.Equal(name, dekType.Name);
        }

        [Fact]
        public void DekType_FromName_NotExistDekType_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DekType.FromName("Not_Exist_Name"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void DekType_FromName_IncorrectName_ThrowArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => DekType.FromName(value!));
        }

        [Fact]
        public void DekType_FromValue_ValidValue_ReturnDekType()
        {
            int value = DekType.Main.Value;
            DekType dekType = DekType.FromValue(value);

            Assert.Equal(value, dekType.Value);
        }

        [Fact]
        public void DekType_FromValue_NotExistDekType_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DekType.FromValue(99999));
        }

        #endregion

        #region Dek - KeyHint

        [Fact]
        public void KeyHint_Create_ValidValue_ReturnsKeyHint()
        {
            string name = "Valid".ToUpper();
            KeyHint keyHint = KeyHint.Create(name);

            Assert.Equal(name, keyHint.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void KeyHint_Create_IncorrectValue_ThrowsDomainException(string? value)
        {
            Assert.Throws<DomainException>(() => KeyHint.Create(value!));
        }

        #endregion
    }
}