using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using villa.Controllers;
using DataLayer.Models;
using RepositoryLayer.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ControllerTests
{
    public class RegistrationControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<RegistrationController>> _mockLogger;
        private readonly RegistrationController _registrationController;

        public RegistrationControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<RegistrationController>>();
            _registrationController = new RegistrationController(_mockUserRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var validCredentials = new Registration { UserName = "validUser", Password = "validPassword" };
            _mockUserRepository.Setup(repo => repo.AuthenticateAsync(validCredentials.UserName, validCredentials.Password))
                .ReturnsAsync(new Registration { UserName = "validUser" });

            // Act
            var result = await _registrationController.Login(validCredentials);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult?.Value);
            Assert.NotNull(okResult.Value.GetType().GetProperty("Token")?.GetValue(okResult.Value));
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var invalidCredentials = new Registration { UserName = "invalidUser", Password = "invalidPassword" };
            _mockUserRepository.Setup(repo => repo.AuthenticateAsync(invalidCredentials.UserName, invalidCredentials.Password))
                .ReturnsAsync((Registration)null);

            // Act
            var result = await _registrationController.Login(invalidCredentials);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task AddUser_ValidRegistration_ReturnsOk()
        {
            // Arrange
            var validRegistration = new Registration { UserName = "newUser", Password = "newPassword" };
            _mockUserRepository.Setup(repo => repo.UserExistsAsync(validRegistration.UserName))
                .ReturnsAsync(false);
            _mockUserRepository.Setup(repo => repo.EmailExistsAsync(validRegistration.Email))
                .ReturnsAsync(false);

            // Act
            var result = await _registrationController.AddUser(validRegistration);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddUser_ExistingUsername_ReturnsBadRequest()
        {
            // Arrange
            var existingUsername = "existingUser";
            var registration = new Registration { UserName = existingUsername, Password = "password" };
            _mockUserRepository.Setup(repo => repo.UserExistsAsync(existingUsername))
                .ReturnsAsync(true);

            // Act
            var result = await _registrationController.AddUser(registration);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddUser_ExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var existingEmail = "existing@example.com";
            var registration = new Registration { UserName = "newUser", Password = "password", Email = existingEmail };
            _mockUserRepository.Setup(repo => repo.UserExistsAsync(registration.UserName))
                .ReturnsAsync(false);
            _mockUserRepository.Setup(repo => repo.EmailExistsAsync(existingEmail))
                .ReturnsAsync(true);

            // Act
            var result = await _registrationController.AddUser(registration);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // Add more test methods for other controller actions

        // Remember to include tests for error cases, edge cases, and validation scenarios
    }
}
