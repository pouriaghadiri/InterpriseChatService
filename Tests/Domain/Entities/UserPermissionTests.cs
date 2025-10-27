using Domain.Base;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Entities
{
    public class UserPermissionTests
    {
        [Fact]
        public void UserPermission_Should_Create_With_Valid_Properties()
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
            userPermission.Id.Should().NotBeEmpty();
            userPermission.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UserPermission_Should_Have_Empty_Guid_Properties_By_Default()
        {
            // Arrange & Act
            var userPermission = new UserPermission();

            // Assert
            userPermission.UserId.Should().Be(Guid.Empty);
            userPermission.PermissionId.Should().Be(Guid.Empty);
            userPermission.DepartmentId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void UserPermission_Should_Inherit_From_Entity()
        {
            // Arrange & Act
            var userPermission = new UserPermission();

            // Assert
            userPermission.Should().BeAssignableTo<Entity>();
            userPermission.Id.Should().NotBeEmpty();
            userPermission.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UserPermission_Should_Allow_Setting_All_Properties()
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
            userPermission.UserId.Should().Be(userId);
            userPermission.PermissionId.Should().Be(permissionId);
            userPermission.DepartmentId.Should().Be(departmentId);
        }

        [Fact]
        public void UserPermission_Should_Support_Navigation_Properties()
        {
            // Arrange
            var userPermission = new UserPermission();

            // Act & Assert
            // Navigation properties should be accessible (even if null)
            userPermission.User.Should().BeNull();
            userPermission.Permission.Should().BeNull();
            userPermission.Department.Should().BeNull();
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321", "11111111-1111-1111-1111-111111111111")]
        [InlineData("00000000-0000-0000-0000-000000000000", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA")]
        public void UserPermission_Should_Accept_Various_Guid_Formats(string userIdStr, string permissionIdStr, string departmentIdStr)
        {
            // Arrange
            var userId = Guid.Parse(userIdStr);
            var permissionId = Guid.Parse(permissionIdStr);
            var departmentId = Guid.Parse(departmentIdStr);

            // Act
            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                DepartmentId = departmentId
            };

            // Assert
            userPermission.UserId.Should().Be(userId);
            userPermission.PermissionId.Should().Be(permissionId);
            userPermission.DepartmentId.Should().Be(departmentId);
        }

        [Fact]
        public void UserPermission_Should_Allow_Same_User_Multiple_Permissions()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var permissionId1 = Guid.NewGuid();
            var permissionId2 = Guid.NewGuid();

            // Act
            var userPermission1 = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId1,
                DepartmentId = departmentId
            };

            var userPermission2 = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId2,
                DepartmentId = departmentId
            };

            // Assert
            userPermission1.UserId.Should().Be(userId);
            userPermission2.UserId.Should().Be(userId);
            userPermission1.PermissionId.Should().NotBe(userPermission2.PermissionId);
            userPermission1.Id.Should().NotBe(userPermission2.Id);
        }

        [Fact]
        public void UserPermission_Should_Allow_Same_Permission_Multiple_Users()
        {
            // Arrange
            var permissionId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            // Act
            var userPermission1 = new UserPermission
            {
                UserId = userId1,
                PermissionId = permissionId,
                DepartmentId = departmentId
            };

            var userPermission2 = new UserPermission
            {
                UserId = userId2,
                PermissionId = permissionId,
                DepartmentId = departmentId
            };

            // Assert
            userPermission1.PermissionId.Should().Be(permissionId);
            userPermission2.PermissionId.Should().Be(permissionId);
            userPermission1.UserId.Should().NotBe(userPermission2.UserId);
            userPermission1.Id.Should().NotBe(userPermission2.Id);
        }
    }
}

