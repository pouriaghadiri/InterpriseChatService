using Application.Features.UserUseCase.Commands;
using Application.Features.UserUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Users.Validators
{
    public class RegisterUserCommandValidatorTests
    {
        private readonly RegisterUserCommandValidator _validator;

        public RegisterUserCommandValidatorTests()
        {
            _validator = new RegisterUserCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Test123!@#",
                PhoneNumber = "09123456789",
                ProfilePicture = "profile.jpg",
                Bio = "Software Developer",
                Location = "Tehran"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Last", "email@test.com", "Test123!@#", "09123456789", "First name is required.")]
        [InlineData("First", "", "email@test.com", "Test123!@#", "09123456789", "Last name is required.")]
        [InlineData("First", "Last", "", "Test123!@#", "09123456789", "Email is required.")]
        [InlineData("First", "Last", "email@test.com", "", "09123456789", "Password is required.")]
        [InlineData("First", "Last", "email@test.com", "Test123!@#", "", "Phone number is required.")]
        public void Validate_Should_Return_Invalid_When_Required_Fields_Are_Empty(
            string firstName, string lastName, string email, string password, string phoneNumber, string expectedError)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("invalid-email", "Invalid email format.")]
        [InlineData("test@", "Invalid email format.")]
        [InlineData("@test.com", "Invalid email format.")]
        public void Validate_Should_Return_Invalid_When_Email_Is_Invalid(string email, string expectedError)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                Password = "Test123!@#",
                PhoneNumber = "09123456789"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("1234567", "Password must be at least 8 characters long.")]
        [InlineData("password", "Password must contain at least one uppercase letter.")]
        [InlineData("PASSWORD", "Password must contain at least one lowercase letter.")]
        [InlineData("Password", "Password must contain at least one number.")]
        [InlineData("Password123", "Password must contain at least one special character.")]
        public void Validate_Should_Return_Invalid_When_Password_Does_Not_Meet_Requirements(
            string password, string expectedError)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = password,
                PhoneNumber = "09123456789"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("1234567890", "Phone number must start with 09 or +989 and be followed by 9 digits.")]
        [InlineData("091234567", "Phone number must start with 09 or +989 and be followed by 9 digits.")]
        [InlineData("+989123456", "Phone number must start with 09 or +989 and be followed by 9 digits.")]
        public void Validate_Should_Return_Invalid_When_PhoneNumber_Is_Invalid(
            string phoneNumber, string expectedError)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Test123!@#",
                PhoneNumber = phoneNumber
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("First", "Last", "email@test.com", "Test123!@#", "09123456789", "profile.jpg", "Bio", "Location")]
        [InlineData("First", "Last", "email@test.com", "Test123!@#", "09123456789", null, null, null)]
        public void Validate_Should_Return_Valid_When_Optional_Fields_Are_Valid_Or_Null(
            string firstName, string lastName, string email, string password, string phoneNumber,
            string profilePicture, string bio, string location)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber,
                ProfilePicture = profilePicture,
                Bio = bio,
                Location = location
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
} 