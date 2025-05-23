using Application.Features.UserUseCase.Commands;
using Application.Features.UserUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Users.Validators
{
    public class UpdateProfileUserCommandValidatorTests
    {
        private readonly UpdateProfileUserCommandValidator _validator; 

        public UpdateProfileUserCommandValidatorTests()
        {
            _validator = new UpdateProfileUserCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new UpdateProfileUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
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
        [InlineData("", "Last", "09123456789", "First name is required.")]
        [InlineData("First", "", "09123456789", "Last name is required.")]
        [InlineData("First", "Last", "", "Phone number is required.")]
        public void Validate_Should_Return_Invalid_When_Required_Fields_Are_Empty(
            string firstName, string lastName, string phoneNumber, string expectedError)
        {
            // Arrange
            var command = new UpdateProfileUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
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
            var command = new UpdateProfileUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = phoneNumber
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData("First", "Last", "09123456789", "profile.jpg", "Bio", "Location")]
        [InlineData("First", "Last", "09123456789", "", "", "")]
        public void Validate_Should_Return_Valid_When_Optional_Fields_Are_Valid_Or_Empty(
            string firstName, string lastName, string phoneNumber,
            string profilePicture, string bio, string location)
        {
            // Arrange
            var command = new UpdateProfileUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
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



        public static IEnumerable<object[]> ExceedLengthTestData =>
            new List<object[]>
            {
        new object[] { "First", "Last", "09123456789", new string('a', 201), "", "", "Profile picture URL must not exceed 200 characters." },
        new object[] { "First", "Last", "09123456789", "", new string('a', 501), "", "Bio must not exceed 500 characters." },
        new object[] { "First", "Last", "09123456789", "", "", new string('a', 101), "Location must not exceed 100 characters." }
            };

        [Theory]
        [MemberData(nameof(ExceedLengthTestData))]
        public void Validate_Should_Return_Invalid_When_Optional_Fields_Exceed_Length_Limits(
            string firstName, string lastName, string phoneNumber,
            string profilePicture, string bio, string location, string expectedError)
        {
            // Arrange
            var command = new UpdateProfileUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                ProfilePicture = profilePicture,
                Bio = bio,
                Location = location
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
}
