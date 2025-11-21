using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Persistence.Repositories
{
    public class UserRoleRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRoleRepository _repository;

        public UserRoleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new UserRoleRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_UserRole_To_Context()
        {
            // Arrange
            var userRole = CreateTestUserRole();

            // Act
            await _repository.AddAsync(userRole);
            await _context.SaveChangesAsync();

            // Assert
            var savedUserRole = await _context.UserRoles.FindAsync(userRole.Id);
            savedUserRole.Should().NotBeNull();
            savedUserRole.Id.Should().Be(userRole.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_UserRole()
        {
            // Arrange
            var userRole = CreateTestUserRole();
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(userRole.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userRole.Id);
            result.UserId.Should().Be(userRole.UserId);
            result.RoleId.Should().Be(userRole.RoleId);
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
        public async Task ExistsAsync_With_Existing_UserRole_Should_Return_True()
        {
            // Arrange
            var userRole = CreateTestUserRole();
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(ur => ur.Id == userRole.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_UserRole_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(ur => ur.Id == nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_With_User_And_Role_Predicate_Should_Work_Correctly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var userRole = CreateTestUserRole(userId, roleId);
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var userRole = CreateTestUserRole();
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.ExistsAsync(ur => ur.Id == userRole.Id, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_UserRole_With_All_Properties()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var userRole = CreateTestUserRole(userId, roleId);
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(userRole.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userRole.Id);
            result.UserId.Should().Be(userId);
            result.RoleId.Should().Be(roleId);
            result.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321")]
        [InlineData("00000000-0000-0000-0000-000000000000", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        [InlineData("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA", "BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB")]
        public async Task ExistsAsync_With_Different_Guid_Combinations_Should_Work(string userIdStr, string roleIdStr)
        {
            // Arrange
            var userId = Guid.Parse(userIdStr);
            var roleId = Guid.Parse(roleIdStr);
            var userRole = CreateTestUserRole(userId, roleId);
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddAsync_Should_Handle_Multiple_UserRoles()
        {
            // Arrange
            var userRole1 = CreateTestUserRole();
            var userRole2 = CreateTestUserRole();
            var userRole3 = CreateTestUserRole();

            // Act
            await _repository.AddAsync(userRole1);
            await _repository.AddAsync(userRole2);
            await _repository.AddAsync(userRole3);
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.UserRoles.CountAsync();
            count.Should().Be(3);
        }

        [Fact]
        public async Task ExistsAsync_With_Complex_Predicate_Should_Work()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId1 = Guid.NewGuid();
            var roleId2 = Guid.NewGuid();
            
            var userRole1 = CreateTestUserRole(userId, roleId1);
            var userRole2 = CreateTestUserRole(userId, roleId2);
            
            await _context.UserRoles.AddRangeAsync(userRole1, userRole2);
            await _context.SaveChangesAsync();

            // Act
            var result1 = await _repository.ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId1);
            var result2 = await _repository.ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId2);
            var result3 = await _repository.ExistsAsync(ur => ur.UserId == userId);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
        }

        private UserRole CreateTestUserRole(Guid? userId = null, Guid? roleId = null)
        {
            return new UserRole
            {
                UserId = userId ?? Guid.NewGuid(),
                RoleId = roleId ?? Guid.NewGuid()
            };
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

