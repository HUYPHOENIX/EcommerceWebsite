// Tests/BussinessLogic.Tests/Services/AuthServiceTests.cs
using Xunit;
using Moq;
using BussinessLogic.Entities;
using BussinessLogic.Services;
using SharedViewModel.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace BussinessLogic.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockUserManager = CreateMockUserManager();
            _mockTokenService = new Mock<ITokenService>();
            _service = new AuthService(_mockTokenService.Object, _mockUserManager.Object);
        }

        private static Mock<UserManager<User>> CreateMockUserManager()
        {
            var userStore = new Mock<IUserStore<User>>();

            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(x => x.Value).Returns(new IdentityOptions());

            var userValidators = new List<IUserValidator<User>>();
            var passwordValidators = new List<IPasswordValidator<User>>();

            var userManager = new Mock<UserManager<User>>(
                userStore.Object,
                options.Object,
                new PasswordHasher<User>(),
                userValidators,
                passwordValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                new Mock<ILogger<UserManager<User>>>().Object
            );

            return userManager;
        }

        #region LoginCustomerAsync Tests

        [Fact]
        public async Task LoginCustomerAsync_WithValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "customer@example.com",
                Password = "Password123"
            };

            var user = new User
            {
                Id = "user-123",
                Email = "customer@example.com",
                NormalizedEmail = "CUSTOMER@EXAMPLE.COM",
                FirstName = "John",
                LastName = "Doe",
                UserName = "customer@example.com"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync("customer@example.com"))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(user, "Password123"))
                .ReturnsAsync(true);

            _mockUserManager
                .Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Customer" });

            _mockTokenService
                .Setup(t => t.GenerateAccessTokenAsync(It.IsAny<TokenGenerationRequest>()))
                .ReturnsAsync("valid-jwt-token");

            // Act
            var result = await _service.LoginCustomerAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Login thành công", result.Message);
            Assert.Equal("valid-jwt-token", result.AccessToken);
            Assert.Contains("Customer", result.Roles);
        }

        [Fact]
        public async Task LoginCustomerAsync_WithEmptyEmail_ReturnsFalse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "",
                Password = "Password123"
            };

            // Act
            var result = await _service.LoginCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email không được trống", result.Message);
        }

        [Fact]
        public async Task LoginCustomerAsync_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "customer@example.com",
                Password = ""
            };

            // Act
            var result = await _service.LoginCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Password không được trống", result.Message);
        }

        [Fact]
        public async Task LoginCustomerAsync_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "invalid-email",
                Password = "Password123"
            };

            // Act
            var result = await _service.LoginCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email không hợp lệ", result.Message);
        }

        [Fact]
        public async Task LoginCustomerAsync_WithNonExistentEmail_ReturnsFalse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync("nonexistent@example.com"))
                .ReturnsAsync(null as User);

            // Act
            var result = await _service.LoginCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email hoặc password không đúng", result.Message);
        }

        [Fact]
        public async Task LoginCustomerAsync_WithWrongPassword_ReturnsFalse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "customer@example.com",
                Password = "WrongPassword123"
            };

            var user = new User
            {
                Id = "user-123",
                Email = "customer@example.com",
                NormalizedEmail = "CUSTOMER@EXAMPLE.COM",
                FirstName = "John",
                LastName = "Doe",
                UserName = "customer@example.com"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync("customer@example.com"))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(user, "WrongPassword123"))
                .ReturnsAsync(false);

            // Act
            var result = await _service.LoginCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email hoặc password không đúng", result.Message);
        }

        #endregion

        #region LoginAdminAsync Tests

        [Fact]
        public async Task LoginAdminAsync_WithAdminRole_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "admin@example.com",
                Password = "AdminPassword123"
            };

            var user = new User
            {
                Id = "user-456",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                FirstName = "Admin",
                LastName = "User",
                UserName = "admin@example.com"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync("admin@example.com"))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(user, "AdminPassword123"))
                .ReturnsAsync(true);

            _mockUserManager
                .Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            _mockTokenService
                .Setup(t => t.GenerateAccessTokenAsync(It.IsAny<TokenGenerationRequest>()))
                .ReturnsAsync("admin-jwt-token");

            // Act
            var result = await _service.LoginAdminAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Login thành công", result.Message);
            Assert.Contains("Admin", result.Roles);
        }

        [Fact]
        public async Task LoginAdminAsync_WithoutAdminRole_ReturnsFalse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "customer@example.com",
                Password = "Password123"
            };

            var user = new User
            {
                Id = "user-123",
                Email = "customer@example.com",
                NormalizedEmail = "CUSTOMER@EXAMPLE.COM",
                FirstName = "John",
                LastName = "Doe",
                UserName = "customer@example.com"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync("customer@example.com"))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(user, "Password123"))
                .ReturnsAsync(true);

            _mockUserManager
                .Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Customer" });

            // Act
            var result = await _service.LoginAdminAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("không có quyền 'Admin'", result.Message);
        }

        #endregion

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "NewPassword123",
                FirstName = "John",
                LastName = "Doe"
            };

            var user = new User
            {
                Id = "user-789",
                Email = "newuser@example.com",
                NormalizedEmail = "NEWUSER@EXAMPLE.COM",
                FirstName = "John",
                LastName = "Doe",
                UserName = "newuser@example.com"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync("newuser@example.com"))
                .ReturnsAsync(null as User);

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<User>(), "NewPassword123"))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "Customer"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Đăng ký thành công", result.Message);
            Assert.Contains("Customer", result.Roles);
            _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<User>(), "NewPassword123"), Times.Once);
            _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), "Customer"), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyEmail_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "",
                Password = "NewPassword123",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Tất cả các field không được để trống", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Tất cả các field không được để trống", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyFirstName_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "NewPassword123",
                FirstName = "",
                LastName = "Doe"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Tất cả các field không được để trống", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyLastName_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "NewPassword123",
                FirstName = "John",
                LastName = ""
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Tất cả các field không được để trống", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "invalid-email",
                Password = "NewPassword123",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email không hợp lệ", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithWeakPassword_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "weak",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("ít nhất 8 ký tự", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithPasswordMissingUppercase_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("chữ hoa", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithPasswordMissingLowercase_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "PASSWORD123",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("chữ thường", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithPasswordMissingDigit_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "PasswordNoDigit",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("số", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ReturnsFalse()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "existing@example.com",
                Password = "NewPassword123",
                FirstName = "John",
                LastName = "Doe"
            };

            var existingUser = new User
            {
                Id = "user-111",
                Email = "existing@example.com",
                NormalizedEmail = "EXISTING@EXAMPLE.COM",
                UserName = "existing@example.com"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync("existing@example.com"))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email đã được sử dụng", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WhenCreateUserFails_ReturnsFalseWithErrorMessage()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "NewPassword123",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync("newuser@example.com"))
                .ReturnsAsync(null as User);

            var identityError = new IdentityError { Description = "User creation failed" };
            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Lỗi đăng ký", result.Message);
        }

        #endregion

        #region Email Validation Tests

        [Theory]
        [InlineData("valid@example.com")]
        [InlineData("user.name@example.co.uk")]
        [InlineData("test123@test.org")]
        public async Task ValidEmail_WithValidFormats_PassesValidation(string email)
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = email,
                Password = "Password123"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(email))
                .ReturnsAsync(null as User);

            // Act
            var result = await _service.LoginCustomerAsync(request);

            // Assert - Should not fail on email format
            Assert.False(result.IsSuccess);
            Assert.NotEqual("Email không hợp lệ", result.Message);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        public async Task InvalidEmail_WithInvalidFormats_ReturnsFalse(string email)
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = email,
                Password = "Password123"
            };

            // Act
            var result = await _service.LoginCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email không hợp lệ", result.Message);
        }

        #endregion
    }
}