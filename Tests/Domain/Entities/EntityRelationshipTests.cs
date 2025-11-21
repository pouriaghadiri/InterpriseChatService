using Domain.Entities;
using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Entities
{
    public class EntityRelationshipTests
    {
        [Fact]
        public void UserRole_ShouldLinkUserAndRole()
        {
            // Arrange
            var user = CreateTestUser();
            var role = CreateTestRole();
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };

            // Act & Assert
            userRole.UserId.Should().Be(user.Id);
            userRole.RoleId.Should().Be(role.Id);
        }

        [Fact]
        public void UserRoleInDepartment_ShouldLinkUserRoleAndDepartment()
        {
            // Arrange
            var user = CreateTestUser();
            var role = CreateTestRole();
            var department = CreateTestDepartment();
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            var userRoleInDepartment = new UserRoleInDepartment
            {
                UserRoleId = userRole.Id,
                DepartmentId = department.Id
            };

            // Act & Assert
            userRoleInDepartment.UserRoleId.Should().Be(userRole.Id);
            userRoleInDepartment.DepartmentId.Should().Be(department.Id);
        }

        [Fact]
        public void UserPermission_ShouldLinkUserPermissionAndDepartment()
        {
            // Arrange
            var user = CreateTestUser();
            var permission = CreateTestPermission();
            var department = CreateTestDepartment();
            var userPermission = new UserPermission
            {
                UserId = user.Id,
                PermissionId = permission.Id,
                DepartmentId = department.Id
            };

            // Act & Assert
            userPermission.UserId.Should().Be(user.Id);
            userPermission.PermissionId.Should().Be(permission.Id);
            userPermission.DepartmentId.Should().Be(department.Id);
        }

        [Fact]
        public void RolePermission_ShouldLinkRolePermissionAndDepartment()
        {
            // Arrange
            var role = CreateTestRole();
            var permission = CreateTestPermission();
            var department = CreateTestDepartment();
            var rolePermission = new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id,
                DepartmentId = department.Id
            };

            // Act & Assert
            rolePermission.RoleId.Should().Be(role.Id);
            rolePermission.PermissionId.Should().Be(permission.Id);
            rolePermission.DepartmentId.Should().Be(department.Id);
        }

        [Fact]
        public void User_WithActiveDepartment_ShouldHaveCorrectRelationship()
        {
            // Arrange
            var user = CreateTestUser();
            var department = CreateTestDepartment();

            // Act
            var result = user.SetActiveDepartment(department.Id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.ActiveDepartmentId.Should().Be(department.Id);
        }

        [Fact]
        public void User_WithMultipleRolesInDifferentDepartments_ShouldWork()
        {
            // Arrange
            var user = CreateTestUser();
            var role1 = CreateTestRole();
            var role2 = CreateTestRole();
            var department1 = CreateTestDepartment();
            var department2 = CreateTestDepartment();

            // Act
            var userRole1 = new UserRole
            {
                UserId = user.Id,
                RoleId = role1.Id
            };
            var userRole2 = new UserRole
            {
                UserId = user.Id,
                RoleId = role2.Id
            };
            var userRoleInDept1 = new UserRoleInDepartment
            {
                UserRoleId = userRole1.Id,
                DepartmentId = department1.Id
            };
            var userRoleInDept2 = new UserRoleInDepartment
            {
                UserRoleId = userRole2.Id,
                DepartmentId = department2.Id
            };

            // Assert
            userRoleInDept1.DepartmentId.Should().Be(department1.Id);
            userRoleInDept2.DepartmentId.Should().Be(department2.Id);
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("Test", "User").Data;
            var email = Email.Create("test@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;
            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Test User", "Test City").Data;
        }

        private Role CreateTestRole()
        {
            var name = EntityName.Create("TestRole").Data;
            return Role.CreateRole(name, "Test Role Description").Data;
        }

        private Department CreateTestDepartment()
        {
            var name = EntityName.Create("TestDepartment").Data;
            return Department.CreateDepartment(name).Data;
        }

        private Permission CreateTestPermission()
        {
            var name = EntityName.Create("TestPermission").Data;
            return Permission.CreatePermission(name, "Test Permission Description").Data;
        }
    }
}
