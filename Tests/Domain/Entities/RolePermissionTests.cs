using Domain.Base;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Entities
{
    public class RolePermissionTests
    {
        [Fact]
        public void RolePermission_Should_Create_With_Valid_Properties()
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
            rolePermission.Id.Should().NotBeEmpty();
            rolePermission.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void RolePermission_Should_Have_Empty_Guid_Properties_By_Default()
        {
            // Arrange & Act
            var rolePermission = new RolePermission();

            // Assert
            rolePermission.RoleId.Should().Be(Guid.Empty);
            rolePermission.PermissionId.Should().Be(Guid.Empty);
            rolePermission.DepartmentId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void RolePermission_Should_Inherit_From_Entity()
        {
            // Arrange & Act
            var rolePermission = new RolePermission();

            // Assert
            rolePermission.Should().BeAssignableTo<Entity>();
            rolePermission.Id.Should().NotBeEmpty();
            rolePermission.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void RolePermission_Should_Allow_Setting_All_Properties()
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
            rolePermission.RoleId.Should().Be(roleId);
            rolePermission.PermissionId.Should().Be(permissionId);
            rolePermission.DepartmentId.Should().Be(departmentId);
        }

        [Fact]
        public void RolePermission_Should_Support_Navigation_Properties()
        {
            // Arrange
            var rolePermission = new RolePermission();

            // Act & Assert
            // Navigation properties should be accessible (even if null)
            rolePermission.Role.Should().BeNull();
            rolePermission.Permission.Should().BeNull();
            rolePermission.Department.Should().BeNull();
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321", "11111111-1111-1111-1111-111111111111")]
        [InlineData("00000000-0000-0000-0000-000000000000", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA")]
        public void RolePermission_Should_Accept_Various_Guid_Formats(string roleIdStr, string permissionIdStr, string departmentIdStr)
        {
            // Arrange
            var roleId = Guid.Parse(roleIdStr);
            var permissionId = Guid.Parse(permissionIdStr);
            var departmentId = Guid.Parse(departmentIdStr);

            // Act
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                DepartmentId = departmentId
            };

            // Assert
            rolePermission.RoleId.Should().Be(roleId);
            rolePermission.PermissionId.Should().Be(permissionId);
            rolePermission.DepartmentId.Should().Be(departmentId);
        }
    }
}

