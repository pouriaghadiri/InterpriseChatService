using Domain.Entities;
using Domain.Base;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Entities
{
    public class UserRoleTests
    {
        [Fact]
        public void UserRole_ShouldHaveCorrectProperties()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            // Act
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };

            // Assert
            userRole.Should().NotBeNull();
            userRole.UserId.Should().Be(userId);
            userRole.RoleId.Should().Be(roleId);
            userRole.UserRoleInDepartments.Should().NotBeNull();
            userRole.UserRoleInDepartments.Should().BeEmpty();
        }

        [Fact]
        public void UserRole_ShouldHaveNavigationProperties()
        {
            // Arrange
            var userRole = new UserRole();

            // Act & Assert
            userRole.User.Should().BeNull(); // Initially null
            userRole.Role.Should().BeNull(); // Initially null
            userRole.UserRoleInDepartments.Should().NotBeNull();
        }

        [Fact]
        public void UserRole_ShouldInheritFromEntity()
        {
            // Arrange & Act
            var userRole = new UserRole();

            // Assert
            userRole.Should().BeAssignableTo<Entity>();
        }
    }
}
