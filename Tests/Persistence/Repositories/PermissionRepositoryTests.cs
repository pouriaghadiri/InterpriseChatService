using Domain.Common.ValueObjects;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Persistence.Repositories
{
    public class PermissionRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionRepository _repository;

        public PermissionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new PermissionRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Permission_To_Context()
        {
            // Arrange
            var permission = CreateTestPermission();

            // Act
            await _repository.AddAsync(permission);
            await _context.SaveChangesAsync();

            // Assert
            var savedPermission = await _context.Permissions.FindAsync(permission.Id);
            savedPermission.Should().NotBeNull();
            savedPermission.Id.Should().Be(permission.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_Permission()
        {
            // Arrange
            var permission = CreateTestPermission();
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(permission.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(permission.Id);
            result.Name.Value.Should().Be(permission.Name.Value);
            result.Description.Should().Be(permission.Description);
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
        public async Task GetAllAsync_Should_Return_All_Permissions()
        {
            // Arrange
            var permission1 = CreateTestPermission("Permission 1", "Description 1");
            var permission2 = CreateTestPermission("Permission 2", "Description 2");
            var permission3 = CreateTestPermission("Permission 3", "Description 3");

            await _context.Permissions.AddRangeAsync(permission1, permission2, permission3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(p => p.Id == permission1.Id);
            result.Should().Contain(p => p.Id == permission2.Id);
            result.Should().Contain(p => p.Id == permission3.Id);
        }

        [Fact]
        public async Task GetAllAsync_With_Empty_Database_Should_Return_Empty_List()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ExistsAsync_With_Existing_Permission_Should_Return_True()
        {
            // Arrange
            var permission = CreateTestPermission();
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(p => p.Id == permission.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_Permission_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(p => p.Id == nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_With_Name_Predicate_Should_Work_Correctly()
        {
            // Arrange
            var permission = CreateTestPermission("User Management", "Permission to manage users");
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(p => p.Name.Value == "User Management");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Permission()
        {
            // Arrange
            var permission = CreateTestPermission("Original Name", "Original Description");
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            var newName = EntityName.Create("Updated Name").Data;
            permission.Name = newName;
            permission.Description = "Updated Description";

            // Act
            await _repository.UpdateAsync(permission);
            await _context.SaveChangesAsync();

            // Assert
            var updatedPermission = await _context.Permissions.FindAsync(permission.Id);
            updatedPermission.Should().NotBeNull();
            updatedPermission.Name.Value.Should().Be("Updated Name");
            updatedPermission.Description.Should().Be("Updated Description");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Permission()
        {
            // Arrange
            var permission = CreateTestPermission();
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(permission);
            await _context.SaveChangesAsync();

            // Assert
            var deletedPermission = await _context.Permissions.FindAsync(permission.Id);
            deletedPermission.Should().BeNull();
        }

        [Fact]
        public async Task IsPermissionInUseAsync_With_Permission_In_RolePermissions_Should_Return_True()
        {
            // Arrange
            var permission = CreateTestPermission();
            var rolePermission = new RolePermission
            {
                RoleId = Guid.NewGuid(),
                PermissionId = permission.Id,
                DepartmentId = Guid.NewGuid()
            };

            await _context.Permissions.AddAsync(permission);
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsPermissionInUseAsync(permission.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsPermissionInUseAsync_With_Permission_In_UserPermissions_Should_Return_True()
        {
            // Arrange
            var permission = CreateTestPermission();
            var userPermission = new UserPermission
            {
                UserId = Guid.NewGuid(),
                PermissionId = permission.Id,
                DepartmentId = Guid.NewGuid()
            };

            await _context.Permissions.AddAsync(permission);
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsPermissionInUseAsync(permission.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsPermissionInUseAsync_With_Permission_Not_In_Use_Should_Return_False()
        {
            // Arrange
            var permission = CreateTestPermission();
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsPermissionInUseAsync(permission.Id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsPermissionInUseAsync_With_Non_Existent_Permission_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.IsPermissionInUseAsync(nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsPermissionInUseAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var permission = CreateTestPermission();
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.IsPermissionInUseAsync(permission.Id, cancellationToken);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("User Management", "Permission to manage users")]
        [InlineData("Department View", "Permission to view departments")]
        [InlineData("Role Assignment", "Permission to assign roles")]
        [InlineData("Permission Control", "Permission to control permissions")]
        public async Task ExistsAsync_With_Different_Permission_Names_Should_Work(string permissionName, string description)
        {
            // Arrange
            var permission = CreateTestPermission(permissionName, description);
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(p => p.Name.Value == permissionName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var permission = CreateTestPermission();
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.ExistsAsync(p => p.Id == permission.Id, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_Permission_With_All_Properties()
        {
            // Arrange
            var permission = CreateTestPermission("Test Permission", "Test Description");
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(permission.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(permission.Id);
            result.Name.Value.Should().Be("Test Permission");
            result.Description.Should().Be("Test Description");
            result.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        private Permission CreateTestPermission(string name = "Test Permission", string description = "Test Description")
        {
            var entityName = EntityName.Create(name).Data;
            return Permission.CreatePermission(entityName, description).Data;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

