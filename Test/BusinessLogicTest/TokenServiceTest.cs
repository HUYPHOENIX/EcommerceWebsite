// Tests/BussinessLogic.Tests/Services/TokenServiceTests.cs
using Xunit;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BussinessLogic.Services;
using SharedViewModel.DTOs;
using Microsoft.Extensions.Configuration;

namespace BussinessLogic.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenService _service;

        private const string ValidKey = "this_is_a_very_long_secret_key_for_jwt_tokens_minimum_32_characters_required";
        private const string ValidIssuer = "TestIssuer";
        private const string ValidAudience = "TestAudience";

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(x => x["Authorization:Key"]).Returns(ValidKey);
            _mockConfiguration.Setup(x => x["Authorization:Issuer"]).Returns(ValidIssuer);
            _mockConfiguration.Setup(x => x["Authorization:Audience"]).Returns(ValidAudience);

            _service = new TokenService(_mockConfiguration.Object);
        }

        #region GenerateAccessTokenAsync - Valid Cases

        [Fact]
        public async Task GenerateAccessTokenAsync_WithValidRequest_ReturnsValidToken()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            // Act
            var token = await _service.GenerateAccessTokenAsync(request);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(token));
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_TokenContainsAllRequiredClaims()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-789",
                Email = "jane@example.com",
                FirstName = "Jane",
                Roles = new List<string>()
            };

            // Act
            var token = await _service.GenerateAccessTokenAsync(request);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);

            // ✅ Check all required claims
            Assert.NotNull(jwtToken.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub && c.Value == request.UserId));

            Assert.NotNull(jwtToken.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Email && c.Value == request.Email));

            Assert.NotNull(jwtToken.Claims.FirstOrDefault(c =>
    c.Type == JwtRegisteredClaimNames.GivenName &&
    c.Value == request.FirstName));

            Assert.NotNull(jwtToken.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Jti));
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_TokenWithSingleRole_IncludesRoleClaim()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            // Act
            var token = await _service.GenerateAccessTokenAsync(request);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();
            Assert.Single(roleClaims);
            Assert.Equal("Customer", roleClaims[0].Value);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_TokenWithMultipleRoles_IncludesAllRoleClaims()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string> { "Admin", "Customer" }
            };

            // Act
            var token = await _service.GenerateAccessTokenAsync(request);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            foreach (var claim in jwtToken.Claims)
            Assert.NotNull(jwtToken);
            var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();
            Assert.Equal(2, roleClaims.Count);
            Assert.Contains(roleClaims, c => c.Value == "Admin");
            Assert.Contains(roleClaims, c => c.Value == "Customer");
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_TokenExpiryIs15Minutes()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            // ✅ Get NOW, add 15 minutes
            var now = DateTime.UtcNow;
            var expectedExpiry = now.AddMinutes(15);
            var expectedExpirySeconds = ((DateTimeOffset)expectedExpiry).ToUnixTimeSeconds();

            // Act
            var token = await _service.GenerateAccessTokenAsync(request);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var actualExpirySeconds = ((DateTimeOffset)jwtToken.ValidTo).ToUnixTimeSeconds();

            Assert.Equal(expectedExpirySeconds, actualExpirySeconds);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_TokenContainsIssuerAndAudience()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            // Act
            var token = await _service.GenerateAccessTokenAsync(request);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            Assert.Equal(ValidIssuer, jwtToken.Issuer);
            Assert.Contains(ValidAudience, jwtToken.Audiences);
        }

        #endregion

        #region GenerateAccessTokenAsync - Validation Error Cases

        [Fact]
        public async Task GenerateAccessTokenAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.GenerateAccessTokenAsync(null)
            );
            Assert.Contains("Request không được trống", ex.Message);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_WithNullUserId_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = null,
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.GenerateAccessTokenAsync(request)
            );
            Assert.Contains("không được trống", ex.Message);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_WithNullEmail_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = null,
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.GenerateAccessTokenAsync(request)
            );
            Assert.Contains("không được trống", ex.Message);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_WithNullFirstName_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = null,
                Roles = new List<string> { "Customer" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.GenerateAccessTokenAsync(request)
            );
            Assert.Contains("không được trống", ex.Message);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_WithNullRoles_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = null
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.GenerateAccessTokenAsync(request)
            );
            Assert.Contains("không được trống", ex.Message);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_WithEmptyRoles_GeneratesTokenWithoutRoleClaims()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string>()  // Empty list
            };

            // Act
            var token = await _service.GenerateAccessTokenAsync(request);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            Assert.Empty(roleClaims);
        }

        #endregion

        #region GenerateAccessTokenAsync - Token Uniqueness

        [Fact]
        public async Task GenerateAccessTokenAsync_DifferentCallsGenerateDifferentTokens()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            // Act
            var token1 = await _service.GenerateAccessTokenAsync(request);
            var token2 = await _service.GenerateAccessTokenAsync(request);

            // Assert
            Assert.NotEqual(token1, token2); // Different due to unique JTI
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_TokensForDifferentUsersAreDifferent()
        {
            // Arrange
            var request1 = new TokenGenerationRequest
            {
                UserId = "user-111",
                Email = "user1@example.com",
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            var request2 = new TokenGenerationRequest
            {
                UserId = "user-222",
                Email = "user2@example.com",
                FirstName = "Jane",
                Roles = new List<string> { "Customer" }
            };

            // Act
            var token1 = await _service.GenerateAccessTokenAsync(request1);
            var token2 = await _service.GenerateAccessTokenAsync(request2);

            // Assert
            Assert.NotEqual(token1, token2);

            // Verify different user IDs in tokens
            var handler = new JwtSecurityTokenHandler();
            var jwt1 = handler.ReadToken(token1) as JwtSecurityToken;
            var jwt2 = handler.ReadToken(token2) as JwtSecurityToken;

            var sub1 = jwt1.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
            var sub2 = jwt2.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

            Assert.NotEqual(sub1, sub2);
        }

        #endregion

        #region GenerateAccessTokenAsync - Configuration Tests

        [Fact]
        public async Task GenerateAccessTokenAsync_UsesConfigurationValues()
        {
            // Arrange
            var request = new TokenGenerationRequest
            {
                UserId = "user-123",
                Email = "user@example.com",
                FirstName = "John",
                Roles = new List<string> { "Customer" }
            };

            // Act
            await _service.GenerateAccessTokenAsync(request);

            // Assert - Verify configuration was used
            _mockConfiguration.Verify(x => x["Authorization:Key"], Times.Once);
            _mockConfiguration.Verify(x => x["Authorization:Issuer"], Times.Once);
            _mockConfiguration.Verify(x => x["Authorization:Audience"], Times.Once);
        }

        #endregion
    }
}