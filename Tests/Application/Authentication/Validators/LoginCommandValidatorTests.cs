using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Authentication.Validators
{
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _validator;

        public LoginCommandValidatorTests()
        {
            _validator = new LoginCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "john.doe@example.com",
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Password123", "Email is required.")]
        [InlineData(null, "Password123", "Email is required.")]
        public void Validate_Should_Return_Invalid_When_Email_Is_Empty(string email, string password, string expectedError)
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = email,
                Password = password
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("invalid-email", "Password123", "Please provide a valid email address.")]
        [InlineData("test@", "Password123", "Please provide a valid email address.")]
        [InlineData("@test.com", "Password123", "Please provide a valid email address.")]
        public void Validate_Should_Return_Invalid_When_Email_Format_Is_Invalid(string email, string password, string expectedError)
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = email,
                Password = password
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("john@example.com", "", "Password is required.")]
        [InlineData("john@example.com", null, "Password is required.")]
        public void Validate_Should_Return_Invalid_When_Password_Is_Empty(string email, string password, string expectedError)
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = email,
                Password = password
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("john@example.com", "12345", "Password must be at least 6 characters long.")]
        public void Validate_Should_Return_Invalid_When_Password_Is_Too_Short(string email, string password, string expectedError)
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = email,
                Password = password
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
}

