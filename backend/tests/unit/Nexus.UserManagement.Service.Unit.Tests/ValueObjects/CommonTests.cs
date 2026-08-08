using Nexus.UserManagement.Service.Domain.ValueObjects.Common;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.ValueObjects
{
    public class CommonTests
    {
        #region Asset - S3Key

        private const string Bucket = "TestBucket";
        private static readonly IReadOnlyCollection<string> Folders = ["user", "avatars"];
        private const string PathFolderString = "user/avatars"; 
        private const string FileName = "avatar.png";
        private const string CorrectStringS3KeyWithFolders = $"{Bucket}:{PathFolderString}/{FileName}";
        private const string CorrectStringS3KeyWithOutFolders = $"{Bucket}:{FileName}";

        [Theory]
        [InlineData(Bucket, new string[] { "user", "avatars" }, FileName)]
        [InlineData(Bucket, new string[] { }, FileName)]
        public void S3Key_Create_CorrectValues_ReturnS3Key(string bucket, string[] folders, string fileName)
        {
            S3Key s3Key = S3Key.Create(bucket, [.. folders], fileName);

            Assert.Equal(Bucket, s3Key.Bucket);
            Assert.True(Guid.TryParse(s3Key.FileName.Split('.').First(), out var _));
            
            if (folders.Count() > 0)
            {
                Assert.Equal(Folders, s3Key.Folders);
                Assert.Equal(PathFolderString, s3Key.FolderPath);
            }
            else
            {
                Assert.Empty(s3Key.Folders);
                Assert.Equal("", s3Key.FolderPath);
            }
        }

        [Theory]
        [InlineData("", new string[] { "user", "avatars" }, FileName)]
        [InlineData(Bucket, null, FileName)]
        [InlineData(Bucket, new string[] { "user", "avatars" }, "")]
        [InlineData(null, new string[] { "user", "avatars" }, FileName)]
        [InlineData(Bucket, new string[] { "user", "avatars" }, null)]
        public void S3Key_Create_EmptyValue_ThrowArgumentNullException(string? bucket, string[]? folders, string? fileName)
        {
            Assert.Throws<ArgumentException>(() => S3Key.Create(bucket!, folders!, fileName!));
        }

        [Fact]
        public void S3Key_Create_InvalidFileName_ThrowFormatException()
        {
            Assert.Throws<FormatException>(() => S3Key.Create(Bucket, [.. Folders], "fileName-without-extension"));
        }

        [Theory]
        [InlineData(CorrectStringS3KeyWithFolders)]
        [InlineData(CorrectStringS3KeyWithOutFolders)]
        public void S3Key_Restore_CorrectValues_ReturnS3Key(string s3KeyString)
        {
            S3Key s3Key = S3Key.Restore(s3KeyString);

            Assert.Equal(Bucket, s3Key.Bucket);
            Assert.Equal(s3KeyString, s3Key.Value);
            Assert.Equal(FileName.ToLowerInvariant(), s3Key.FileName);

            if (s3Key.Folders.Count() > 0)
                Assert.Equal(PathFolderString, s3Key.FolderPath);
        }

        [Theory]
        [InlineData("Incorrect")]
        [InlineData("/folders/user")]
        [InlineData("file.exe")]
        public void S3Key_Restore_IncorrectS3KeyString_ThrowFormatException(string value)
        {
            Assert.Throws<FormatException>(() => S3Key.Restore(value));
        }

        [Fact]
        public void S3Key_Restore_S3KeyStringWithOutSeparator_ThrowFormatException()
        {
            Assert.Throws<FormatException>(() => S3Key.Restore($"{Bucket}/{PathFolderString}/avatar.png"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void S3Key_Restore_S3KeyStringEmpty_ThrowArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => S3Key.Restore(value!));
        }

        [Fact]
        public void S3Key_GetObjectKey_WithFolders_ReturnCorrectValue()
        {
            S3Key s3Key = S3Key.Create(Bucket, [.. Folders], FileName);

            string fileName = s3Key.FileName;
            string objectKey = s3Key.GetObjectKey();

            Assert.Equal($"{PathFolderString}/{fileName}", objectKey);
        }

        [Fact]
        public void S3Key_GetObjectKey_WithOutFolders_ReturnCorrectValue()
        {
            S3Key s3Key = S3Key.Create(Bucket, [], FileName);

            string fileName = s3Key.FileName;
            string objectKey = s3Key.GetObjectKey();

            Assert.Equal(fileName, objectKey);
        }

        [Fact]
        public void S3Key_FolderPathProp_WithFolders_ReturnCorrectPath()
        {
            S3Key s3Key = S3Key.Create(Bucket, [.. Folders], FileName);

            Assert.Equal(PathFolderString, s3Key.FolderPath);
        }

        [Fact]
        public void S3Key_FolderPathProp_WithOutFolders_ReturnCorrectPath()
        {
            S3Key s3Key = S3Key.Create(Bucket, [], FileName);

            Assert.Equal("", s3Key.FolderPath);
        }

        #endregion
    }
}