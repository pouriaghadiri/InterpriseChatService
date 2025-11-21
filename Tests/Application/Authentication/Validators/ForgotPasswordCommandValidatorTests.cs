using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Authentication.Validators
{
    public class ForgotPasswordCommandValidatorTests
    {
        private readonly ForgotPasswordCommandValidator _validator;

        public ForgotPasswordCommandValidatorTests()
        {
            _validator = new ForgotPasswordCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_Email_Is_Valid()
        {
            // Arrange
            var command = new ForgotPasswordCommand
            {
                Email = "john.doe@example.com"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Email is required.")]
        [InlineData(null, "Email is required.")]
        public void Validate_Should_Return_Invalid_When_Email_Is_Empty(string email, string expectedError)
        {
            // Arrange
            var command = new ForgotPasswordCommand
            {
                Email = email
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("invalid-email", "Please provide a valid email address.")]
        [InlineData("test@", "Please provide a valid email address.")]
        [InlineData("@test.com", "Please provide a valid email address.")]
        public void Validate_Should_Return_Invalid_When_Email_Format_Is_Invalid(string email, string expectedError)
        {
            // Arrange
            var command = new ForgotPasswordCommand
            {
                Email = email
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
}

