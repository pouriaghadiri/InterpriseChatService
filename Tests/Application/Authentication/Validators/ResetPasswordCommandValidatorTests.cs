using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Authentication.Validators
{
    public class ResetPasswordCommandValidatorTests
    {
        private readonly ResetPasswordCommandValidator _validator;

        public ResetPasswordCommandValidatorTests()
        {
            _validator = new ResetPasswordCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Email = "john.doe@example.com",
                Token = "valid-token",
                NewPassword = "NewPassword123!@#",
                ConfirmPassword = "NewPassword123!@#"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "token", "NewPassword123!@#", "NewPassword123!@#", "Email is required.")]
        [InlineData(null, "token", "NewPassword123!@#", "NewPassword123!@#", "Email is required.")]
        public void Validate_Should_Return_Invalid_When_Email_Is_Empty(string email, string token, string newPassword, string confirmPassword, string expectedError)
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Email = email,
                Token = token,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("john@example.com", "", "NewPassword123!@#", "NewPassword123!@#", "Reset token is required.")]
        [InlineData("john@example.com", null, "NewPassword123!@#", "NewPassword123!@#", "Reset token is required.")]
        public void Validate_Should_Return_Invalid_When_Token_Is_Empty(string email, string token, string newPassword, string confirmPassword, string expectedError)
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Email = email,
                Token = token,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("john@example.com", "token", "", "NewPassword123!@#", "New password is required.")]
        [InlineData("john@example.com", "token", null, "NewPassword123!@#", "New password is required.")]
        public void Validate_Should_Return_Invalid_When_Password_Is_Empty(string email, string token, string newPassword, string confirmPassword, string expectedError)
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Email = email,
                Token = token,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("john@example.com", "token", "Short1!", "Short1!", "Password must be at least 8 characters long.")]
        public void Validate_Should_Return_Invalid_When_Password_Is_Too_Short(string email, string token, string newPassword, string confirmPassword, string expectedError)
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Email = email,
                Token = token,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("john@example.com", "token", "NewPassword123!@#", "DifferentPassword123!@#", "Password and confirmation must match.")]
        public void Validate_Should_Return_Invalid_When_Passwords_Do_Not_Match(string email, string token, string newPassword, string confirmPassword, string expectedError)
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Email = email,
                Token = token,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
}

