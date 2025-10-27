using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Services;
using Domain.Common.ValueObjects;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Services.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Tests.UnitTest.Infrastructure.Services
{
    public class JwtTokenServiceTests
    {
        private readonly JwtSettingsDTO _jwtSettings;
        private readonly JwtTokenService _jwtTokenService;

        public JwtTokenServiceTests()
        {
            _jwtSettings = new JwtSettingsDTO
            {
                Key = "ThisIsAVeryLongSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpireMinutes = 60
            };
            _jwtTokenService = new JwtTokenService(_jwtSettings);
        }

        [Fact]
        public void GenerateToken_With_Valid_User_And_Roles_Should_Generate_Valid_Token()
        {
            // Arrange
            var user = CreateTestUser();
            var roles = new[] { "Admin", "User" };
            var permissions = new[] { "Read", "Write" };

            // Act
            var token = _jwtTokenService.GenerateToken(user, roles, out var expireDate, permissions);

            // Assert
            token.Should().NotBeNullOrEmpty();
            expireDate.Should().BeCloseTo(DateTime.Now.AddMinutes(60), TimeSpan.FromMinutes(1));
            
            // Verify token can be parsed
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            jwtToken.Should().NotBeNull();
            jwtToken.Issuer.Should().Be(_jwtSettings.Issuer);
            jwtToken.Audiences.Should().Contain(_jwtSettings.Audience);
        }

        [Fact]
        public void GenerateToken_Should_Include_User_Claims()
        {
            // Arrange
            var user = CreateTestUser();
            var roles = new[] { "Admin" };

            // Act
            var token = _jwtTokenService.GenerateToken(user, roles, out var expireDate);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
            claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == $"{user.FullName.FirstName} {user.FullName.LastName}");
            claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email.Value);
        }

        [Fact]
        public void GenerateToken_Should_Include_Role_Claims()
        {
            // Arrange
            var user = CreateTestUser();
            var roles = new[] { "Admin", "User", "Manager" };

            // Act
            var token = _jwtTokenService.GenerateToken(user, roles, out var expireDate);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
            claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Manager");
        }

        [Fact]
        public void GenerateToken_With_Empty_Roles_Should_Not_Include_Role_Claims()
        {
            // Arrange
            var user = CreateTestUser();
            var roles = new string[0];

            // Act
            var token = _jwtTokenService.GenerateToken(user, roles, out var expireDate);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
        }

        [Fact]
        public void GenerateToken_With_Null_Roles_Should_Not_Throw_Exception()
        {
            // Arrange
            var user = CreateTestUser();
            string[] roles = null;

            // Act & Assert
            var action = () => _jwtTokenService.GenerateToken(user, roles, out var expireDate);
            action.Should().NotThrow();
        }

        [Fact]
        public void GenerateToken_With_Permissions_Should_Not_Include_Permission_Claims_Currently()
        {
            // Arrange
            var user = CreateTestUser();
            var roles = new[] { "Admin" };
            var permissions = new[] { "Read", "Write", "Delete" };

            // Act
            var token = _jwtTokenService.GenerateToken(user, roles, out var expireDate, permissions);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            // Currently permissions are commented out in the implementation
            claims.Should().NotContain(c => c.Type == "permission");
        }

        [Fact]
        public void GenerateToken_Should_Set_Correct_Expiration_Time()
        {
            // Arrange
            var user = CreateTestUser();
            var roles = new[] { "Admin" };
            var customExpireMinutes = 120;
            var customJwtSettings = new JwtSettingsDTO
            {
                Key = _jwtSettings.Key,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                ExpireMinutes = customExpireMinutes
            };
            var customJwtService = new JwtTokenService(customJwtSettings);

            // Act
            var token = customJwtService.GenerateToken(user, roles, out var expireDate);

            // Assert
            expireDate.Should().BeCloseTo(DateTime.Now.AddMinutes(customExpireMinutes), TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void GenerateToken_Should_Use_Correct_Signing_Algorithm()
        {
            // Arrange
            var user = CreateTestUser();
            var roles = new[] { "Admin" };

            // Act
            var token = _jwtTokenService.GenerateToken(user, roles, out var expireDate);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            jwtToken.Header.Alg.Should().Be(SecurityAlgorithms.HmacSha256);
        }

        [Fact]
        public void GenerateToken_Should_Generate_Different_Tokens_For_Same_Input()
        {
            // Arrange
            var user = CreateTestUser();
            var roles = new[] { "Admin" };

            // Act
            var token1 = _jwtTokenService.GenerateToken(user, roles, out var expireDate1);
            var token2 = _jwtTokenService.GenerateToken(user, roles, out var expireDate2);

            // Assert
            token1.Should().NotBe(token2);
            // Expiration times should be different due to time passing
            expireDate1.Should().NotBe(expireDate2);
        }

        [Fact]
        public void GenerateToken_With_Different_Users_Should_Generate_Different_Tokens()
        {
            // Arrange
            var user1 = CreateTestUser();
            var user2 = CreateTestUser();
            var roles = new[] { "Admin" };

            // Act
            var token1 = _jwtTokenService.GenerateToken(user1, roles, out var expireDate1);
            var token2 = _jwtTokenService.GenerateToken(user2, roles, out var expireDate2);

            // Assert
            token1.Should().NotBe(token2);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(30)]
        [InlineData(60)]
        [InlineData(1440)] // 24 hours
        public void GenerateToken_With_Different_Expiration_Minutes_Should_Set_Correct_Expiration(int expireMinutes)
        {
            // Arrange
            var customJwtSettings = new JwtSettingsDTO
            {
                Key = _jwtSettings.Key,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                ExpireMinutes = expireMinutes
            };
            var customJwtService = new JwtTokenService(customJwtSettings);
            var user = CreateTestUser();
            var roles = new[] { "Admin" };

            // Act
            var token = customJwtService.GenerateToken(user, roles, out var expireDate);

            // Assert
            expireDate.Should().BeCloseTo(DateTime.Now.AddMinutes(expireMinutes), TimeSpan.FromMinutes(1));
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john.doe@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }
    }
}

