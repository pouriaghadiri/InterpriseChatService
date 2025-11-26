using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.UserUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Users.Validators
{
    public class ChangePasswordUserCommandValidatorTests
    {
        private readonly ChangePasswordUserCommandValidator _validator;

        public ChangePasswordUserCommandValidatorTests()
        {
            _validator = new ChangePasswordUserCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new ChangePasswordUserCommand
            {
                CurrentPassword = "CurrentPass123!",
                NewPassword = "NewPass123!@#",
                NewPasswordConfirm = "NewPass123!@#"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "NewPass123!@#", "NewPass123!@#", "Current password is required.")]
        [InlineData("CurrentPass123!", "", "NewPass123!@#", "New password is required.")]
        [InlineData("CurrentPass123!", "NewPass123!@#", "", "Confirm password does not match the new password.")]
        public void Validate_Should_Return_Invalid_When_Required_Fields_Are_Empty(
            string currentPassword, string newPassword, string newPasswordConfirm, string expectedError)
        {
            // Arrange
            var command = new ChangePasswordUserCommand
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                NewPasswordConfirm = newPasswordConfirm
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("12345", "NewPass123!@#", "NewPass123!@#", "Current password must be at least 6 characters long.")]
        public void Validate_Should_Return_Invalid_When_CurrentPassword_Is_Too_Short(
            string currentPassword, string newPassword, string newPasswordConfirm, string expectedError)
        {
            // Arrange
            var command = new ChangePasswordUserCommand
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                NewPasswordConfirm = newPasswordConfirm
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("CurrentPass123!", "1234567", "1234567", "New password must be at least 8 characters long.")]
        [InlineData("CurrentPass123!", "newpass123!@#", "newpass123!@#", "New password must contain at least one uppercase letter.")]
        [InlineData("CurrentPass123!", "NEWPASS123!@#", "NEWPASS123!@#", "New password must contain at least one lowercase letter.")]
        [InlineData("CurrentPass123!", "NewPass!@#", "NewPass!@#", "New password must contain at least one number.")]
        [InlineData("CurrentPass123!", "NewPass123", "NewPass123", "New password must contain at least one special character.")]
        public void Validate_Should_Return_Invalid_When_NewPassword_Does_Not_Meet_Requirements(
            string currentPassword, string newPassword, string newPasswordConfirm, string expectedError)
        {
            // Arrange
            var command = new ChangePasswordUserCommand
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                NewPasswordConfirm = newPasswordConfirm
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("CurrentPass123!", "NewPass123!@#", "DifferentPass123!@#", "Confirm password does not match the new password.")]
        public void Validate_Should_Return_Invalid_When_Passwords_Do_Not_Match(
            string currentPassword, string newPassword, string newPasswordConfirm, string expectedError)
        {
            // Arrange
            var command = new ChangePasswordUserCommand
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                NewPasswordConfirm = newPasswordConfirm
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
} 