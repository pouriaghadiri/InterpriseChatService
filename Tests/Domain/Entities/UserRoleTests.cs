using Domain.Base;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Entities
{
    public class UserRoleTests
    {
        [Fact]
        public void UserRole_Should_Create_With_Valid_Properties()
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
            userRole.Id.Should().NotBeEmpty();
            userRole.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UserRole_Should_Have_Empty_Guid_Properties_By_Default()
        {
            // Arrange & Act
            var userRole = new UserRole();

            // Assert
            userRole.UserId.Should().Be(Guid.Empty);
            userRole.RoleId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void UserRole_Should_Inherit_From_Entity()
        {
            // Arrange & Act
            var userRole = new UserRole();

            // Assert
            userRole.Should().BeAssignableTo<Entity>();
            userRole.Id.Should().NotBeEmpty();
            userRole.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UserRole_Should_Allow_Setting_All_Properties()
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
            userRole.UserId.Should().Be(userId);
            userRole.RoleId.Should().Be(roleId);
        }

        [Fact]
        public void UserRole_Should_Support_Navigation_Properties()
        {
            // Arrange
            var userRole = new UserRole();

            // Act & Assert
            // Navigation properties should be accessible (even if null)
            userRole.User.Should().BeNull();
            userRole.Role.Should().BeNull();
        }

        [Fact]
        public void UserRole_Should_Initialize_UserRoleInDepartments_Collection()
        {
            // Arrange & Act
            var userRole = new UserRole();

            // Assert
            userRole.UserRoleInDepartments.Should().NotBeNull();
            userRole.UserRoleInDepartments.Should().BeEmpty();
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321")]
        [InlineData("00000000-0000-0000-0000-000000000000", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        [InlineData("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA", "BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB")]
        public void UserRole_Should_Accept_Various_Guid_Formats(string userIdStr, string roleIdStr)
        {
            // Arrange
            var userId = Guid.Parse(userIdStr);
            var roleId = Guid.Parse(roleIdStr);

            // Act
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };

            // Assert
            userRole.UserId.Should().Be(userId);
            userRole.RoleId.Should().Be(roleId);
        }

        [Fact]
        public void UserRole_Should_Allow_Same_User_Multiple_Roles()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId1 = Guid.NewGuid();
            var roleId2 = Guid.NewGuid();

            // Act
            var userRole1 = new UserRole
            {
                UserId = userId,
                RoleId = roleId1
            };

            var userRole2 = new UserRole
            {
                UserId = userId,
                RoleId = roleId2
            };

            // Assert
            userRole1.UserId.Should().Be(userId);
            userRole2.UserId.Should().Be(userId);
            userRole1.RoleId.Should().NotBe(userRole2.RoleId);
            userRole1.Id.Should().NotBe(userRole2.Id);
        }

        [Fact]
        public void UserRole_Should_Allow_Same_Role_Multiple_Users()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            // Act
            var userRole1 = new UserRole
            {
                UserId = userId1,
                RoleId = roleId
            };

            var userRole2 = new UserRole
            {
                UserId = userId2,
                RoleId = roleId
            };

            // Assert
            userRole1.RoleId.Should().Be(roleId);
            userRole2.RoleId.Should().Be(roleId);
            userRole1.UserId.Should().NotBe(userRole2.UserId);
            userRole1.Id.Should().NotBe(userRole2.Id);
        }

        [Fact]
        public void UserRole_Should_Support_Collection_Operations()
        {
            // Arrange
            var userRole = new UserRole();
            var userRoleInDepartment1 = new UserRoleInDepartment();
            var userRoleInDepartment2 = new UserRoleInDepartment();

            // Act
            userRole.UserRoleInDepartments.Add(userRoleInDepartment1);
            userRole.UserRoleInDepartments.Add(userRoleInDepartment2);

            // Assert
            userRole.UserRoleInDepartments.Should().HaveCount(2);
            userRole.UserRoleInDepartments.Should().Contain(userRoleInDepartment1);
            userRole.UserRoleInDepartments.Should().Contain(userRoleInDepartment2);
        }
    }
}

