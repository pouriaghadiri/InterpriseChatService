using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Authentication.Validators
{
    public class VerifyEmailCommandValidatorTests
    {
        private readonly VerifyEmailCommandValidator _validator;

        public VerifyEmailCommandValidatorTests()
        {
            _validator = new VerifyEmailCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new VerifyEmailCommand
            {
                Email = "john.doe@example.com",
                Token = "valid-verification-token"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "token", "Email is required.")]
        [InlineData(null, "token", "Email is required.")]
        public void Validate_Should_Return_Invalid_When_Email_Is_Empty(string email, string token, string expectedError)
        {
            // Arrange
            var command = new VerifyEmailCommand
            {
                Email = email,
                Token = token
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("invalid-email", "token", "Please provide a valid email address.")]
        public void Validate_Should_Return_Invalid_When_Email_Format_Is_Invalid(string email, string token, string expectedError)
        {
            // Arrange
            var command = new VerifyEmailCommand
            {
                Email = email,
                Token = token
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("john@example.com", "", "Verification token is required.")]
        [InlineData("john@example.com", null, "Verification token is required.")]
        public void Validate_Should_Return_Invalid_When_Token_Is_Empty(string email, string token, string expectedError)
        {
            // Arrange
            var command = new VerifyEmailCommand
            {
                Email = email,
                Token = token
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
}

