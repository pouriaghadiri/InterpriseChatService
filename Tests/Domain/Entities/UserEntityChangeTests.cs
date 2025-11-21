using Domain.Entities;
using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Entities
{
    public class UserEntityChangeTests
    {
        [Fact]
        public void User_ShouldHaveActiveDepartmentIdProperty()
        {
            // Arrange
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            // Act
            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;

            // Assert
            user.Should().NotBeNull();
            user.ActiveDepartmentId.Should().BeNull(); // Should be nullable initially
        }

        [Fact]
        public void User_SetActiveDepartment_WithValidDepartmentId_ShouldReturnSuccess()
        {
            // Arrange
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;
            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
            var departmentId = Guid.NewGuid();

            // Act
            var result = user.SetActiveDepartment(departmentId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            user.ActiveDepartmentId.Should().Be(departmentId);
        }

        [Fact]
        public void User_SetActiveDepartment_WithEmptyDepartmentId_ShouldReturnFailure()
        {
            // Arrange
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;
            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;

            // Act
            var result = user.SetActiveDepartment(Guid.Empty);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Department ID cannot be empty");
        }

        [Fact]
        public void User_ShouldHaveUserRolesCollection()
        {
            // Arrange
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            // Act
            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;

            // Assert
            user.Should().NotBeNull();
            user.UserRoles.Should().NotBeNull();
            user.UserRoles.Should().BeEmpty();
        }

        [Fact]
        public void User_ShouldHaveActiveDepartmentNavigationProperty()
        {
            // Arrange
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            // Act
            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;

            // Assert
            user.Should().NotBeNull();
            user.ActiveDepartment.Should().BeNull(); // Should be nullable initially
        }
    }
}
