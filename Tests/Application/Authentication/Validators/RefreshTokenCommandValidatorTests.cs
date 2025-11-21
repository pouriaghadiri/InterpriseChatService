using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Authentication.Validators
{
    public class RefreshTokenCommandValidatorTests
    {
        private readonly RefreshTokenCommandValidator _validator;

        public RefreshTokenCommandValidatorTests()
        {
            _validator = new RefreshTokenCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_Token_Is_Valid()
        {
            // Arrange
            var command = new RefreshTokenCommand
            {
                RefreshToken = "valid-refresh-token"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Refresh token is required.")]
        [InlineData(null, "Refresh token is required.")]
        public void Validate_Should_Return_Invalid_When_Token_Is_Empty(string token, string expectedError)
        {
            // Arrange
            var command = new RefreshTokenCommand
            {
                RefreshToken = token
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
}

