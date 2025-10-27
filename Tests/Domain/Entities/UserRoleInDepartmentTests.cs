using Domain.Base;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Entities
{
    public class UserRoleInDepartmentTests
    {
        [Fact]
        public void UserRoleInDepartment_Should_Create_With_Valid_Properties()
        {
            // Arrange
            var userRoleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            // Act
            var userRoleInDepartment = new UserRoleInDepartment
            {
                UserRoleId = userRoleId,
                DepartmentId = departmentId
            };

            // Assert
            userRoleInDepartment.Should().NotBeNull();
            userRoleInDepartment.UserRoleId.Should().Be(userRoleId);
            userRoleInDepartment.DepartmentId.Should().Be(departmentId);
            userRoleInDepartment.Id.Should().NotBeEmpty();
            userRoleInDepartment.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UserRoleInDepartment_Should_Have_Empty_Guid_Properties_By_Default()
        {
            // Arrange & Act
            var userRoleInDepartment = new UserRoleInDepartment();

            // Assert
            userRoleInDepartment.UserRoleId.Should().Be(Guid.Empty);
            userRoleInDepartment.DepartmentId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void UserRoleInDepartment_Should_Inherit_From_Entity()
        {
            // Arrange & Act
            var userRoleInDepartment = new UserRoleInDepartment();

            // Assert
            userRoleInDepartment.Should().BeAssignableTo<Entity>();
            userRoleInDepartment.Id.Should().NotBeEmpty();
            userRoleInDepartment.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UserRoleInDepartment_Should_Allow_Setting_All_Properties()
        {
            // Arrange
            var userRoleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            // Act
            var userRoleInDepartment = new UserRoleInDepartment
            {
                UserRoleId = userRoleId,
                DepartmentId = departmentId
            };

            // Assert
            userRoleInDepartment.UserRoleId.Should().Be(userRoleId);
            userRoleInDepartment.DepartmentId.Should().Be(departmentId);
        }

        [Fact]
        public void UserRoleInDepartment_Should_Support_Navigation_Properties()
        {
            // Arrange
            var userRoleInDepartment = new UserRoleInDepartment();

            // Act & Assert
            // Navigation properties should be accessible (even if null)
            userRoleInDepartment.UserRole.Should().BeNull();
            userRoleInDepartment.Department.Should().BeNull();
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321")]
        [InlineData("00000000-0000-0000-0000-000000000000", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        [InlineData("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA", "BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB")]
        public void UserRoleInDepartment_Should_Accept_Various_Guid_Formats(string userRoleIdStr, string departmentIdStr)
        {
            // Arrange
            var userRoleId = Guid.Parse(userRoleIdStr);
            var departmentId = Guid.Parse(departmentIdStr);

            // Act
            var userRoleInDepartment = new UserRoleInDepartment
            {
                UserRoleId = userRoleId,
                DepartmentId = departmentId
            };

            // Assert
            userRoleInDepartment.UserRoleId.Should().Be(userRoleId);
            userRoleInDepartment.DepartmentId.Should().Be(departmentId);
        }

        [Fact]
        public void UserRoleInDepartment_Should_Allow_Same_UserRole_Multiple_Departments()
        {
            // Arrange
            var userRoleId = Guid.NewGuid();
            var departmentId1 = Guid.NewGuid();
            var departmentId2 = Guid.NewGuid();

            // Act
            var userRoleInDepartment1 = new UserRoleInDepartment
            {
                UserRoleId = userRoleId,
                DepartmentId = departmentId1
            };

            var userRoleInDepartment2 = new UserRoleInDepartment
            {
                UserRoleId = userRoleId,
                DepartmentId = departmentId2
            };

            // Assert
            userRoleInDepartment1.UserRoleId.Should().Be(userRoleId);
            userRoleInDepartment2.UserRoleId.Should().Be(userRoleId);
            userRoleInDepartment1.DepartmentId.Should().NotBe(userRoleInDepartment2.DepartmentId);
            userRoleInDepartment1.Id.Should().NotBe(userRoleInDepartment2.Id);
        }

        [Fact]
        public void UserRoleInDepartment_Should_Allow_Same_Department_Multiple_UserRoles()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var userRoleId1 = Guid.NewGuid();
            var userRoleId2 = Guid.NewGuid();

            // Act
            var userRoleInDepartment1 = new UserRoleInDepartment
            {
                UserRoleId = userRoleId1,
                DepartmentId = departmentId
            };

            var userRoleInDepartment2 = new UserRoleInDepartment
            {
                UserRoleId = userRoleId2,
                DepartmentId = departmentId
            };

            // Assert
            userRoleInDepartment1.DepartmentId.Should().Be(departmentId);
            userRoleInDepartment2.DepartmentId.Should().Be(departmentId);
            userRoleInDepartment1.UserRoleId.Should().NotBe(userRoleInDepartment2.UserRoleId);
            userRoleInDepartment1.Id.Should().NotBe(userRoleInDepartment2.Id);
        }

        [Fact]
        public void UserRoleInDepartment_Should_Represent_UserRole_Department_Association()
        {
            // Arrange
            var userRoleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            // Act
            var userRoleInDepartment = new UserRoleInDepartment
            {
                UserRoleId = userRoleId,
                DepartmentId = departmentId
            };

            // Assert
            // This entity represents the many-to-many relationship between UserRole and Department
            userRoleInDepartment.UserRoleId.Should().Be(userRoleId);
            userRoleInDepartment.DepartmentId.Should().Be(departmentId);
            
            // Each instance should have a unique ID
            userRoleInDepartment.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void UserRoleInDepartment_Should_Support_Entity_Lifecycle_Methods()
        {
            // Arrange
            var userRoleInDepartment = new UserRoleInDepartment
            {
                UserRoleId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid()
            };

            // Act & Assert
            // Test entity lifecycle methods inherited from Entity
            userRoleInDepartment.IsActive.Should().BeTrue();
            userRoleInDepartment.IsDeleted.Should().BeFalse();
            userRoleInDepartment.IsArchived.Should().BeFalse();
        }
    }
}

