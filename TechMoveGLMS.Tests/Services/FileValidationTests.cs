using Microsoft.AspNetCore.Http;
using System.Text;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class FileValidationTests
    {
        // Helper method to create a fake uploaded file
        private IFormFile CreateFakeFile(string fileName, string content = "fake content")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }

        private bool IsValidPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLower();
            return extension == ".pdf";
        }

        [Fact]
        public void PdfFile_IsValid_ReturnsTrue()
        {
            // Arrange
            var file = CreateFakeFile("agreement.pdf");

            // Act
            bool result = IsValidPdf(file);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ExeFile_IsInvalid_ReturnsFalse()
        {
            // Arrange
            var file = CreateFakeFile("malware.exe");

            // Act
            bool result = IsValidPdf(file);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WordFile_IsInvalid_ReturnsFalse()
        {
            // Arrange
            var file = CreateFakeFile("document.docx");

            // Act
            bool result = IsValidPdf(file);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ImageFile_IsInvalid_ReturnsFalse()
        {
            // Arrange
            var file = CreateFakeFile("photo.jpg");

            // Act
            bool result = IsValidPdf(file);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void NullFile_IsInvalid_ReturnsFalse()
        {
            // Arrange
            IFormFile file = null;

            // Act
            bool result = IsValidPdf(file);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpperCasePdfExtension_IsValid_ReturnsTrue()
        {
            // Arrange — test .PDF uppercase
            var file = CreateFakeFile("AGREEMENT.PDF");

            // Act
            bool result = IsValidPdf(file);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TextFile_IsInvalid_ReturnsFalse()
        {
            // Arrange
            var file = CreateFakeFile("notes.txt");

            // Act
            bool result = IsValidPdf(file);

            // Assert
            Assert.False(result);
        }
    }
}