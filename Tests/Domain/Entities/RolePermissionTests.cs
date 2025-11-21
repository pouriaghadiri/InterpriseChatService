using Domain.Entities;
using Domain.Base;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Entities
{
    public class RolePermissionTests
    {
        [Fact]
        public void RolePermission_ShouldHaveCorrectProperties()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            // Act
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                DepartmentId = departmentId
            };

            // Assert
            rolePermission.Should().NotBeNull();
            rolePermission.RoleId.Should().Be(roleId);
            rolePermission.PermissionId.Should().Be(permissionId);
            rolePermission.DepartmentId.Should().Be(departmentId);
        }

        [Fact]
        public void RolePermission_ShouldHaveNavigationProperties()
        {
            // Arrange
            var rolePermission = new RolePermission();

            // Act & Assert
            rolePermission.Role.Should().BeNull(); // Initially null
            rolePermission.Permission.Should().BeNull(); // Initially null
            rolePermission.Department.Should().BeNull(); // Initially null
        }

        [Fact]
        public void RolePermission_ShouldInheritFromEntity()
        {
            // Arrange & Act
            var rolePermission = new RolePermission();

            // Assert
            rolePermission.Should().BeAssignableTo<Entity>();
        }
    }
}
