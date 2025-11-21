using Domain.Entities;
using Domain.Base;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Entities
{
    public class UserRoleInDepartmentTests
    {
        [Fact]
        public void UserRoleInDepartment_ShouldHaveCorrectProperties()
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
        }

        [Fact]
        public void UserRoleInDepartment_ShouldHaveNavigationProperties()
        {
            // Arrange
            var userRoleInDepartment = new UserRoleInDepartment();

            // Act & Assert
            userRoleInDepartment.UserRole.Should().BeNull(); // Initially null
            userRoleInDepartment.Department.Should().BeNull(); // Initially null
        }

        [Fact]
        public void UserRoleInDepartment_ShouldInheritFromEntity()
        {
            // Arrange & Act
            var userRoleInDepartment = new UserRoleInDepartment();

            // Assert
            userRoleInDepartment.Should().BeAssignableTo<Entity>();
        }
        }
}
