using Domain.Common.ValueObjects;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Persistence.Repositories
{
    public class RoleRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleRepository _repository;

        public RoleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new RoleRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Role_To_Context()
        {
            // Arrange
            var role = CreateTestRole();

            // Act
            await _repository.AddAsync(role);
            await _context.SaveChangesAsync();

            // Assert
            var savedRole = await _context.Roles.FindAsync(role.Id);
            savedRole.Should().NotBeNull();
            savedRole.Id.Should().Be(role.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_Role()
        {
            // Arrange
            var role = CreateTestRole();
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(role.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(role.Id);
            result.Name.Value.Should().Be(role.Name.Value);
            result.Description.Should().Be(role.Description);
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
        public async Task GetAllAsync_Should_Return_All_Roles()
        {
            // Arrange
            var role1 = CreateTestRole("Role 1", "Description 1");
            var role2 = CreateTestRole("Role 2", "Description 2");
            var role3 = CreateTestRole("Role 3", "Description 3");

            await _context.Roles.AddRangeAsync(role1, role2, role3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(r => r.Id == role1.Id);
            result.Should().Contain(r => r.Id == role2.Id);
            result.Should().Contain(r => r.Id == role3.Id);
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
        public async Task ExistsAsync_With_Existing_Role_Should_Return_True()
        {
            // Arrange
            var role = CreateTestRole();
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(r => r.Id == role.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_Role_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(r => r.Id == nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_With_Name_Predicate_Should_Work_Correctly()
        {
            // Arrange
            var role = CreateTestRole("Admin Role", "Administrator role");
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(r => r.Name.Value == "Admin Role");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Role()
        {
            // Arrange
            var role = CreateTestRole("Original Name", "Original Description");
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            var newName = EntityName.Create("Updated Name").Data;
            role.Name = newName;
            role.Description = "Updated Description";

            // Act
            await _repository.UpdateAsync(role);
            await _context.SaveChangesAsync();

            // Assert
            var updatedRole = await _context.Roles.FindAsync(role.Id);
            updatedRole.Should().NotBeNull();
            updatedRole.Name.Value.Should().Be("Updated Name");
            updatedRole.Description.Should().Be("Updated Description");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Role()
        {
            // Arrange
            var role = CreateTestRole();
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(role);
            await _context.SaveChangesAsync();

            // Assert
            var deletedRole = await _context.Roles.FindAsync(role.Id);
            deletedRole.Should().BeNull();
        }

        [Fact]
        public async Task IsRoleInUseAsync_With_Role_In_Use_Should_Return_True()
        {
            // Arrange
            var role = CreateTestRole();
            var userRole = new UserRole
            {
                UserId = Guid.NewGuid(),
                RoleId = role.Id
            };

            await _context.Roles.AddAsync(role);
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsRoleInUseAsync(role.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsRoleInUseAsync_With_Role_Not_In_Use_Should_Return_False()
        {
            // Arrange
            var role = CreateTestRole();
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsRoleInUseAsync(role.Id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsRoleInUseAsync_With_Non_Existent_Role_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.IsRoleInUseAsync(nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsRoleInUseAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var role = CreateTestRole();
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.IsRoleInUseAsync(role.Id, cancellationToken);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("Admin", "Administrator role")]
        [InlineData("User", "Regular user role")]
        [InlineData("Manager", "Management role")]
        [InlineData("Guest", "Guest user role")]
        public async Task ExistsAsync_With_Different_Role_Names_Should_Work(string roleName, string description)
        {
            // Arrange
            var role = CreateTestRole(roleName, description);
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(r => r.Name.Value == roleName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var role = CreateTestRole();
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.ExistsAsync(r => r.Id == role.Id, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_Role_With_All_Properties()
        {
            // Arrange
            var role = CreateTestRole("Test Role", "Test Description");
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(role.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(role.Id);
            result.Name.Value.Should().Be("Test Role");
            result.Description.Should().Be("Test Description");
            result.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        private Role CreateTestRole(string name = "Test Role", string description = "Test Description")
        {
            var entityName = EntityName.Create(name).Data;
            return Role.CreateRole(entityName, description).Data;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

