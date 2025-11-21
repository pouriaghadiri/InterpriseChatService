using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using FluentAssertions;
using Xunit;
using Domain.Common.ValueObjects;

namespace Tests.UnitTest.Persistence.Repositories
{
    public class RolePermissionRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly RolePermissionRepository _repository;

        public RolePermissionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new RolePermissionRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_RolePermission_To_Context()
        {
            // Arrange
            var rolePermission = CreateTestRolePermission();

            // Act
            await _repository.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            // Assert
            var savedRolePermission = await _context.RolePermissions.FindAsync(rolePermission.Id);
            savedRolePermission.Should().NotBeNull();
            savedRolePermission.Id.Should().Be(rolePermission.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_RolePermission()
        {
            // Arrange
            var rolePermission = CreateTestRolePermission();
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(rolePermission.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(rolePermission.Id);
            result.RoleId.Should().Be(rolePermission.RoleId);
            result.PermissionId.Should().Be(rolePermission.PermissionId);
            result.DepartmentId.Should().Be(rolePermission.DepartmentId);
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
        public async Task ExistsAsync_With_Existing_RolePermission_Should_Return_True()
        {
            // Arrange
            var rolePermission = CreateTestRolePermission();
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(rp => rp.Id == rolePermission.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_RolePermission_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(rp => rp.Id == nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_With_Role_And_Permission_Predicate_Should_Work_Correctly()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var rolePermission = CreateTestRolePermission(roleId, permissionId, departmentId);
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var rolePermission = CreateTestRolePermission();
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.ExistsAsync(rp => rp.Id == rolePermission.Id, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetRolePermissionsAsync_Should_Return_Role_Permissions_With_Includes()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var permission = CreateTestPermission();
            var rolePermission = CreateTestRolePermission(roleId, permission.Id, departmentId);

            await _context.Permissions.AddAsync(permission);
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRolePermissionsAsync(roleId, departmentId);

            // Assert
            result.Should().HaveCount(1);
            result.First().Permission.Should().NotBeNull();
            result.First().Permission.Name.Value.Should().Be(permission.Name.Value);
        }

        [Fact]
        public async Task GetRolePermissionsAsync_With_No_Permissions_Should_Return_Empty_List()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetRolePermissionsAsync(roleId, departmentId);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRolePermissionsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.GetRolePermissionsAsync(roleId, departmentId, cancellationToken);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRolePermissionsAsync_Should_Filter_By_Role_And_Department()
        {
            // Arrange
            var roleId1 = Guid.NewGuid();
            var roleId2 = Guid.NewGuid();
            var departmentId1 = Guid.NewGuid();
            var departmentId2 = Guid.NewGuid();
            
            var permission1 = CreateTestPermission("Permission 1");
            var permission2 = CreateTestPermission("Permission 2");
            var permission3 = CreateTestPermission("Permission 3");
            
            var rolePermission1 = CreateTestRolePermission(roleId1, permission1.Id, departmentId1);
            var rolePermission2 = CreateTestRolePermission(roleId1, permission2.Id, departmentId2);
            var rolePermission3 = CreateTestRolePermission(roleId2, permission3.Id, departmentId1);

            await _context.Permissions.AddRangeAsync(permission1, permission2, permission3);
            await _context.RolePermissions.AddRangeAsync(rolePermission1, rolePermission2, rolePermission3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRolePermissionsAsync(roleId1, departmentId1);

            // Assert
            result.Should().HaveCount(1);
            result.First().Permission.Name.Value.Should().Be("Permission 1");
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_RolePermission_With_All_Properties()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var rolePermission = CreateTestRolePermission(roleId, permissionId, departmentId);
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(rolePermission.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(rolePermission.Id);
            result.RoleId.Should().Be(roleId);
            result.PermissionId.Should().Be(permissionId);
            result.DepartmentId.Should().Be(departmentId);
            result.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321", "11111111-1111-1111-1111-111111111111")]
        [InlineData("00000000-0000-0000-0000-000000000000", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA")]
        public async Task ExistsAsync_With_Different_Guid_Combinations_Should_Work(string roleIdStr, string permissionIdStr, string departmentIdStr)
        {
            // Arrange
            var roleId = Guid.Parse(roleIdStr);
            var permissionId = Guid.Parse(permissionIdStr);
            var departmentId = Guid.Parse(departmentIdStr);
            var rolePermission = CreateTestRolePermission(roleId, permissionId, departmentId);
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.DepartmentId == departmentId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddAsync_Should_Handle_Multiple_RolePermissions()
        {
            // Arrange
            var rolePermission1 = CreateTestRolePermission();
            var rolePermission2 = CreateTestRolePermission();
            var rolePermission3 = CreateTestRolePermission();

            // Act
            await _repository.AddAsync(rolePermission1);
            await _repository.AddAsync(rolePermission2);
            await _repository.AddAsync(rolePermission3);
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.RolePermissions.CountAsync();
            count.Should().Be(3);
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

        private Permission CreateTestPermission(string name = "Test Permission")
        {
            var entityName = EntityName.Create(name).Data;
            return Permission.CreatePermission(entityName, "Test Description").Data;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

