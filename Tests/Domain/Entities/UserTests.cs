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

        #region User Name Duplication Tests

        [Fact]
        public void RegisterUser_Should_Allow_Users_With_Same_Name_But_Different_Ids()
        {
            // Arrange
            var fullName1 = PersonFullName.Create("John", "Doe").Data;
            var fullName2 = PersonFullName.Create("John", "Doe").Data;
            var email1 = Email.Create("john.doe@example.com").Data;
            var email2 = Email.Create("john.doe2@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone1 = PhoneNumber.Create("09123456789").Data;
            var phone2 = PhoneNumber.Create("09123456788").Data;

            // Act
            var result1 = User.RegisterUser(fullName1, email1, password, phone1, "profile1.jpg", "Developer", "Tehran");
            var result2 = User.RegisterUser(fullName2, email2, password, phone2, "profile2.jpg", "Developer", "Tehran");

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();

            // Users can have the same name but must have different IDs
            result1.Data.FullName.FirstName.Should().Be("John");
            result1.Data.FullName.LastName.Should().Be("Doe");
            result2.Data.FullName.FirstName.Should().Be("John");
            result2.Data.FullName.LastName.Should().Be("Doe");
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void RegisterUser_Should_Allow_Users_With_Same_Name_But_Different_Emails()
        {
            // Arrange
            var fullName = PersonFullName.Create("Jane", "Smith").Data;
            var email1 = Email.Create("jane.smith@example.com").Data;
            var email2 = Email.Create("jane.smith2@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone1 = PhoneNumber.Create("09123456789").Data;
            var phone2 = PhoneNumber.Create("09123456788").Data;

            // Act
            var result1 = User.RegisterUser(fullName, email1, password, phone1, "profile1.jpg", "Developer", "Tehran");
            var result2 = User.RegisterUser(fullName, email2, password, phone2, "profile2.jpg", "Developer", "Tehran");

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();

            // Users can have the same name but different emails and IDs
            result1.Data.FullName.Should().Be(fullName);
            result2.Data.FullName.Should().Be(fullName);
            result1.Data.Email.Should().Be(email1);
            result2.Data.Email.Should().Be(email2);
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        #endregion

        #region AssignRoleToUser Tests

        [Fact]
        public void AssignRoleToUser_Should_Return_Success_When_Valid_Role_And_Department()
        {
            // Arrange
            var user = CreateValidUser();
            var role = Role.CreateRole(EntityName.Create("Admin").Data, "Administrator role").Data;
            var department = Department.CreateDepartment(EntityName.Create("IT").Data).Data;

            // Act
            var result = user.AssignRoleToUser(role, department);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Role assigned to user in department.");
            user.UserRoles.Should().HaveCount(1);
            user.UserRoles.First().Role.Should().Be(role);
            user.UserRoles.First().UserRoleInDepartments.Should().HaveCount(1);
            user.UserRoles.First().UserRoleInDepartments.First().Department.Should().Be(department);
        }

        [Fact]
        public void AssignRoleToUser_Should_Return_Failure_When_Role_Is_Null()
        {
            // Arrange
            var user = CreateValidUser();
            Role role = null;
            var department = Department.CreateDepartment(EntityName.Create("IT").Data).Data;

            // Act
            var result = user.AssignRoleToUser(role, department);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Role is required!");
        }

        [Fact]
        public void AssignRoleToUser_Should_Return_Failure_When_Department_Is_Null()
        {
            // Arrange
            var user = CreateValidUser();
            var role = Role.CreateRole(EntityName.Create("Admin").Data, "Administrator role").Data;
            Department department = null;

            // Act
            var result = user.AssignRoleToUser(role, department);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Department is required!");
        }

        [Fact]
        public void AssignRoleToUser_Should_Return_Failure_When_Role_Already_Assigned_To_Same_Department()
        {
            // Arrange
            var user = CreateValidUser();
            var role = Role.CreateRole(EntityName.Create("Admin").Data, "Administrator role").Data;
            var department = Department.CreateDepartment(EntityName.Create("IT").Data).Data;

            // Act - Assign role first time
            var firstResult = user.AssignRoleToUser(role, department);
            var secondResult = user.AssignRoleToUser(role, department);

            // Assert
            firstResult.Should().NotBeNull();
            firstResult.IsSuccess.Should().BeTrue();
            secondResult.Should().NotBeNull();
            secondResult.IsSuccess.Should().BeFalse();
            secondResult.Message.Should().Be("User already has this role in the specified department.");
        }

        [Fact]
        public void AssignRoleToUser_Should_Add_New_UserRole_When_Role_Not_Already_Assigned()
        {
            // Arrange
            var user = CreateValidUser();
            var role1 = Role.CreateRole(EntityName.Create("Admin").Data, "Administrator role").Data;
            var role2 = Role.CreateRole(EntityName.Create("User").Data, "Standard user role").Data;
            var department = Department.CreateDepartment(EntityName.Create("IT").Data).Data;

            // Act
            var result1 = user.AssignRoleToUser(role1, department);
            var result2 = user.AssignRoleToUser(role2, department);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            user.UserRoles.Should().HaveCount(2);
            user.UserRoles.Should().Contain(ur => ur.Role == role1);
            user.UserRoles.Should().Contain(ur => ur.Role == role2);
        }

        [Fact]
        public void AssignRoleToUser_Should_Handle_Multiple_Roles_To_Same_Department()
        {
            // Arrange
            var user = CreateValidUser();
            var role1 = Role.CreateRole(EntityName.Create("Admin").Data, "Administrator role").Data;
            var role2 = Role.CreateRole(EntityName.Create("User").Data, "Standard user role").Data;
            var department = Department.CreateDepartment(EntityName.Create("IT").Data).Data;

            // Act
            var result1 = user.AssignRoleToUser(role1, department);
            var result2 = user.AssignRoleToUser(role2, department);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            user.UserRoles.Should().HaveCount(2);
            user.UserRoles.Should().Contain(ur => ur.Role == role1);
            user.UserRoles.Should().Contain(ur => ur.Role == role2);
        }


        [Fact]
        public void AssignRoleToUser_Should_Add_New_UserRole_And_Department_When_Not_Exist()
        {
            // Arrange
            var user = CreateValidUser();
            var role = Role.CreateRole(EntityName.Create("Admin").Data!, "Admin Role").Data!;
            var department = Department.CreateDepartment(EntityName.Create("IT").Data!).Data!;

            // Act
            var result = user.AssignRoleToUser(role, department);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.UserRoles.Should().HaveCount(1);
            user.UserRoles.First().UserRoleInDepartments.Should().ContainSingle()
                .Which.Department.Should().Be(department);
        }

        [Fact]
        public void AssignRoleToUser_Should_Reuse_UserRole_When_Role_Already_Exists_In_Another_Department()
        {
            // Arrange
            var user = CreateValidUser();
            var role = Role.CreateRole(EntityName.Create("Admin").Data!, "Admin Role").Data!;
            var dept1 = Department.CreateDepartment(EntityName.Create("IT").Data!).Data!;
            var dept2 = Department.CreateDepartment(EntityName.Create("HR").Data!).Data!;

            // Act
            var result1 = user.AssignRoleToUser(role, dept1);
            var result2 = user.AssignRoleToUser(role, dept2);

            // Assert
            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue();

            user.UserRoles.Should().HaveCount(1); // Same Role, reused
            user.UserRoles.First().UserRoleInDepartments.Should().HaveCount(2);
        }

        [Fact]
        public void AssignRoleToUser_Should_Fail_When_Already_Assigned_To_Same_Department()
        {
            // Arrange
            var user = CreateValidUser();
            var role = Role.CreateRole(EntityName.Create("Admin").Data!, "Admin Role").Data!;
             
            var dept = Department.CreateDepartment(EntityName.Create("IT").Data!).Data!;

            user.AssignRoleToUser(role, dept); // First assignment

            // Act
            var result = user.AssignRoleToUser(role, dept); // Duplicate

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("already has this role");
        }

        [Fact]
        public void AssignRoleToUser_Should_Fail_When_Role_Is_Null()
        {
            // Arrange
            var user = CreateValidUser();
            var dept = Department.CreateDepartment(EntityName.Create("IT").Data!).Data!;

            // Act
            var result = user.AssignRoleToUser(null!, dept);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Role is required!");
        }

        [Fact]
        public void AssignRoleToUser_Should_Fail_When_Department_Is_Null()
        {
            // Arrange
            var user = CreateValidUser();
            var role = Role.CreateRole(EntityName.Create("Admin").Data!, "Admin Role").Data!;

            // Act
            var result = user.AssignRoleToUser(role, null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Department is required!");
        }

        #endregion
    }
}