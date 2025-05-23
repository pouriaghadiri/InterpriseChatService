using Domain.Common.ValueObjects;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void RegisterUser_Should_Return_Success_When_All_Fields_Are_Valid()
        {
            // Arrange
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john.doe@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            // Act
            var result = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran");

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.FullName.Should().Be(fullName);
            result.Data.Email.Should().Be(email);
            result.Data.HashedPassword.Should().Be(password);
            result.Data.Phone.Should().Be(phone);
            result.Data.ProfilePicture.Should().Be("profile.jpg");
            result.Data.Bio.Should().Be("Developer");
            result.Data.Location.Should().Be("Tehran");
        }

        [Theory]
        [InlineData(null, "Doe", "john@example.com", "Test123!@#", "09123456789", "FullName name cannot be empty")]
        [InlineData("John", null, "john@example.com", "Test123!@#", "09123456789", "FullName name cannot be empty")]
        [InlineData("John", "Doe", null, "Test123!@#", "09123456789", "Email cannot be empty")]
        [InlineData("John", "Doe", "john@example.com", "Test123!@#", null, "Phone number cannot be empty")]
        public void RegisterUser_Should_Return_Failure_When_Required_Fields_Are_Empty(
            string firstName, string lastName, string email, string password, string phone, string expectedError)
        {
            // Arrange
            var fullName = firstName != null && lastName != null 
                ? PersonFullName.Create(firstName, lastName).Data 
                : null;
            var emailObj = email != null ? Email.Create(email).Data : null;
            var passwordObj = password != null ? HashedPassword.Create(password).Data : null;
            var phoneObj = phone != null ? PhoneNumber.Create(phone).Data : null;

            // Act
            var result = User.RegisterUser(fullName, emailObj, passwordObj, phoneObj, null, null, null);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be(expectedError);
        }

        [Fact]
        public void UpdateUserProfile_Should_Return_Success_When_All_Fields_Are_Valid()
        {
            // Arrange
            var user = CreateValidUser();
            var newFullName = PersonFullName.Create("Jane", "Smith").Data;
            var newPhone = PhoneNumber.Create("09123456789").Data;

            // Act
            var result = user.UpdateUserProfile(newFullName, user.Email, newPhone, "New Bio", "New Location", "new-profile.jpg");

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            user.FullName.Should().Be(newFullName);
            user.Phone.Should().Be(newPhone);
            user.Bio.Should().Be("New Bio");
            user.Location.Should().Be("New Location");
            user.ProfilePicture.Should().Be("new-profile.jpg");
        }

        [Theory]
        [InlineData(null, "Smith", "09123456789", "FullName name cannot be empty")]
        [InlineData("Jane", null, "09123456789", "FullName name cannot be empty")]
        [InlineData("Jane", "Smith", null, "Phone number cannot be empty")]
        public void UpdateUserProfile_Should_Return_Failure_When_Required_Fields_Are_Empty(
            string firstName, string lastName, string phone, string expectedError)
        {
            // Arrange
            var user = CreateValidUser();
            var newFullName = firstName != null && lastName != null 
                ? PersonFullName.Create(firstName, lastName).Data 
                : null;
            var newPhone = phone != null ? PhoneNumber.Create(phone).Data : null;

            // Act
            var result = user.UpdateUserProfile(newFullName, user.Email, newPhone, null, null, null);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be(expectedError);
        }

        [Fact]
        public void ChangePassword_Should_Return_Success_When_Password_Is_Valid()
        {
            // Arrange
            var user = CreateValidUser();
            var currentPassword = "Test123!@#";
            var newPassword = "NewPass123!@#";

            // Act
            var result = user.ChangePassword(currentPassword, newPassword);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            user.HashedPassword.Verify(newPassword).Should().BeTrue();
        }

        [Theory]
        [InlineData("WrongPass123!@#", "NewPass123!@#", "Current password is incorrect")]
        [InlineData("Test123!@#", "short", "New password cannot be empty or less than 6 charachters")]
        public void ChangePassword_Should_Return_Failure_When_Input_Is_Invalid(
            string currentPassword, string newPassword, string expectedError)
        {
            // Arrange
            var user = CreateValidUser();

            // Act
            var result = user.ChangePassword(currentPassword, newPassword);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be(expectedError);
        }

        private User CreateValidUser()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john.doe@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }
    }
} 