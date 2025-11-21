using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using FluentAssertions;
using Xunit;
using Domain.Common.ValueObjects;

namespace Tests.UnitTest.Persistence.Repositories
{
    public class UserPermissionRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserPermissionRepository _repository;

        public UserPermissionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new UserPermissionRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_UserPermission_To_Context()
        {
            // Arrange
            var userPermission = CreateTestUserPermission();

            // Act
            await _repository.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            // Assert
            var savedUserPermission = await _context.UserPermissions.FindAsync(userPermission.Id);
            savedUserPermission.Should().NotBeNull();
            savedUserPermission.Id.Should().Be(userPermission.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_UserPermission()
        {
            // Arrange
            var userPermission = CreateTestUserPermission();
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(userPermission.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userPermission.Id);
            result.UserId.Should().Be(userPermission.UserId);
            result.PermissionId.Should().Be(userPermission.PermissionId);
            result.DepartmentId.Should().Be(userPermission.DepartmentId);
        }

        [Fact]
        public async Task GetbyIdAsync_With_Non_Existent_Id_Should_Return_Null()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetbyIdAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_With_Existing_UserPermission_Should_Return_True()
        {
            // Arrange
            var userPermission = CreateTestUserPermission();
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(up => up.Id == userPermission.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_UserPermission_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(up => up.Id == nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_With_User_And_Permission_Predicate_Should_Work_Correctly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var userPermission = CreateTestUserPermission(userId, permissionId, departmentId);
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(up => up.UserId == userId && up.PermissionId == permissionId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var userPermission = CreateTestUserPermission();
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.ExistsAsync(up => up.Id == userPermission.Id, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserPermissionsAsync_Should_Return_User_Permissions_With_Includes()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var permission = CreateTestPermission();
            var userPermission = CreateTestUserPermission(userId, permission.Id, departmentId);

            await _context.Permissions.AddAsync(permission);
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserPermissionsAsync(userId, departmentId);

            // Assert
            result.Should().HaveCount(1);
            result.First().Permission.Should().NotBeNull();
            result.First().Permission.Name.Value.Should().Be(permission.Name.Value);
        }

        [Fact]
        public async Task GetUserPermissionsAsync_With_No_Permissions_Should_Return_Empty_List()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetUserPermissionsAsync(userId, departmentId);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserPermissionsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.GetUserPermissionsAsync(userId, departmentId, cancellationToken);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUserPermissionsAsync_Should_Return_Combined_Permissions()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            
            // Create permissions
            var userPermission = CreateTestPermission("User Permission");
            var rolePermission = CreateTestPermission("Role Permission");
            
            // Create user permission
            var userPerm = CreateTestUserPermission(userId, userPermission.Id, departmentId);
            
            // Create role permission
            var rolePerm = CreateTestRolePermission(roleId, rolePermission.Id, departmentId);
            
            // Create user role
            var userRole = CreateTestUserRole(userId, roleId);
            var userRoleInDepartment = CreateTestUserRoleInDepartment(userRole.Id, departmentId);

            await _context.Permissions.AddRangeAsync(userPermission, rolePermission);
            await _context.UserPermissions.AddAsync(userPerm);
            await _context.RolePermissions.AddAsync(rolePerm);
            await _context.UserRoles.AddAsync(userRole);
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUserPermissionsAsync(userId, departmentId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain("User Permission");
            result.Should().Contain("Role Permission");
        }

        [Fact]
        public async Task GetAllUserPermissionsAsync_With_Only_User_Permissions_Should_Return_User_Permissions()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var permission = CreateTestPermission("User Permission");
            var userPermission = CreateTestUserPermission(userId, permission.Id, departmentId);

            await _context.Permissions.AddAsync(permission);
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUserPermissionsAsync(userId, departmentId);

            // Assert
            result.Should().HaveCount(1);
            result.Should().Contain("User Permission");
        }

        [Fact]
        public async Task GetAllUserPermissionsAsync_With_Only_Role_Permissions_Should_Return_Role_Permissions()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var permission = CreateTestPermission("Role Permission");
            var rolePermission = CreateTestRolePermission(roleId, permission.Id, departmentId);
            var userRole = CreateTestUserRole(userId, roleId);
            var userRoleInDepartment = CreateTestUserRoleInDepartment(userRole.Id, departmentId);

            await _context.Permissions.AddAsync(permission);
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.UserRoles.AddAsync(userRole);
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUserPermissionsAsync(userId, departmentId);

            // Assert
            result.Should().HaveCount(1);
            result.Should().Contain("Role Permission");
        }

        [Fact]
        public async Task GetAllUserPermissionsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.GetAllUserPermissionsAsync(userId, departmentId, cancellationToken);

            // Assert
            result.Should().BeEmpty();
        }

        private UserPermission CreateTestUserPermission(Guid? userId = null, Guid? permissionId = null, Guid? departmentId = null)
        {
            return new UserPermission
            {
                UserId = userId ?? Guid.NewGuid(),
                PermissionId = permissionId ?? Guid.NewGuid(),
                DepartmentId = departmentId ?? Guid.NewGuid()
            };
        }

        private Permission CreateTestPermission(string name = "Test Permission")
        {
            var entityName = EntityName.Create(name).Data;
            return Permission.CreatePermission(entityName, "Test Description").Data;
        }

        private RolePermission CreateTestRolePermission(Guid? roleId = null, Guid? permissionId = null, Guid? departmentId = null)
        {
            return new RolePermission
            {
                RoleId = roleId ?? Guid.NewGuid(),
                PermissionId = permissionId ?? Guid.NewGuid(),
                DepartmentId = departmentId ?? Guid.NewGuid()
            };
        }

        private UserRole CreateTestUserRole(Guid? userId = null, Guid? roleId = null)
        {
            return new UserRole
            {
                UserId = userId ?? Guid.NewGuid(),
                RoleId = roleId ?? Guid.NewGuid()
            };
        }

        private UserRoleInDepartment CreateTestUserRoleInDepartment(Guid? userRoleId = null, Guid? departmentId = null)
        {
            return new UserRoleInDepartment
            {
                UserRoleId = userRoleId ?? Guid.NewGuid(),
                DepartmentId = departmentId ?? Guid.NewGuid()
            };
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

