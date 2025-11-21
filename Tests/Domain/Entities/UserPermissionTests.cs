using Domain.Entities;
using Domain.Base;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Entities
{
    public class UserPermissionTests
    {
        [Fact]
        public void UserPermission_ShouldHaveCorrectProperties()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            // Act
            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                DepartmentId = departmentId
            };

            // Assert
            userPermission.Should().NotBeNull();
            userPermission.UserId.Should().Be(userId);
            userPermission.PermissionId.Should().Be(permissionId);
            userPermission.DepartmentId.Should().Be(departmentId);
        }

        [Fact]
        public void UserPermission_ShouldHaveNavigationProperties()
        {
            // Arrange
            var userPermission = new UserPermission();

            // Act & Assert
            userPermission.User.Should().BeNull(); // Initially null
            userPermission.Permission.Should().BeNull(); // Initially null
            userPermission.Department.Should().BeNull(); // Initially null
        }

        [Fact]
        public void UserPermission_ShouldInheritFromEntity()
        {
            // Arrange & Act
            var userPermission = new UserPermission();

            // Assert
            userPermission.Should().BeAssignableTo<Entity>();
        }
    }
}
