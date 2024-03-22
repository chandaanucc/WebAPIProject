using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using villa.Controllers;
using RepositoryLayer.Interfaces;
using DataLayer.Models;

namespace Villa.Tests.ControllerTests
{
    public class ImageControllerTests
    {
        [Fact]
        public async Task UploadImage_ValidFile_AddsImageSuccessfully()
        {
            // Arrange
            var mockImageRepository = new Mock<IImageRepository>();
            var mockLogger = new Mock<ILogger<ImageController>>();

            // Set up a mock form file
            var formFileMock = new Mock<IFormFile>();

            // Set up a mock image file content (e.g., a byte array)
            byte[] imageData = new byte[] { /* Your image byte data here */ };

            // Set up the stream of the image content
            var imageStream = new MemoryStream(imageData);

            // Set up the form file properties
            formFileMock.Setup(f => f.OpenReadStream()).Returns(imageStream);
            formFileMock.Setup(f => f.FileName).Returns("test_image.jpg"); // Example filename

            // Set up the controller with the mock repository and logger
            var controller = new ImageController(mockImageRepository.Object, mockLogger.Object);

            // Act
            var result = await controller.UploadImage(formFileMock.Object, "test_username", "test_comment");

            // Assert
            Assert.NotNull(result);
            // Add more assertions as needed
        }

        [Fact]
        public async Task GetImageByUserName_ExistingUsername_ReturnsImageSuccessfully()
        {
            // Arrange
            var mockImageRepository = new Mock<IImageRepository>();
            var mockLogger = new Mock<ILogger<ImageController>>();

            // Set up the controller with the mock repository and logger
            var controller = new ImageController(mockImageRepository.Object, mockLogger.Object);

            // Act
            var result = await controller.GetImageByUserName("test_username");

            // Assert
            Assert.NotNull(result);
            // Add more assertions as needed
        }

        [Fact]
        public async Task DeleteImageByUserName_ExistingUsername_DeletesImageSuccessfully()
        {
            // Arrange
            var mockImageRepository = new Mock<IImageRepository>();
            var mockLogger = new Mock<ILogger<ImageController>>();

            // Set up the controller with the mock repository and logger
            var controller = new ImageController(mockImageRepository.Object, mockLogger.Object);

            // Act
            var result = await controller.DeleteImageByUserName("test_username");

            // Assert
            Assert.NotNull(result);
            // Add more assertions as needed
        }
    }
}
