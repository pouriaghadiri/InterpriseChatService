using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using FluentAssertions;
using Xunit;
using Domain.Common.ValueObjects;

namespace Tests.UnitTest.Persistence.Repositories
{
    public class UserRoleInDepartmentRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRoleInDepartmentRepository _repository;

        public UserRoleInDepartmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new UserRoleInDepartmentRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_UserRoleInDepartment_To_Context()
        {
            // Arrange
            var userRoleInDepartment = CreateTestUserRoleInDepartment();

            // Act
            await _repository.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Assert
            var savedUserRoleInDepartment = await _context.UserRoleInDepartments.FindAsync(userRoleInDepartment.Id);
            savedUserRoleInDepartment.Should().NotBeNull();
            savedUserRoleInDepartment.Id.Should().Be(userRoleInDepartment.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_UserRoleInDepartment()
        {
            // Arrange
            var userRoleInDepartment = CreateTestUserRoleInDepartment();
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(userRoleInDepartment.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userRoleInDepartment.Id);
            result.UserRoleId.Should().Be(userRoleInDepartment.UserRoleId);
            result.DepartmentId.Should().Be(userRoleInDepartment.DepartmentId);
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
        public async Task ExistsAsync_With_Existing_UserRoleInDepartment_Should_Return_True()
        {
            // Arrange
            var userRoleInDepartment = CreateTestUserRoleInDepartment();
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(urid => urid.Id == userRoleInDepartment.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_UserRoleInDepartment_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(urid => urid.Id == nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_With_UserRole_And_Department_Predicate_Should_Work_Correctly()
        {
            // Arrange
            var userRoleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var userRoleInDepartment = CreateTestUserRoleInDepartment(userRoleId, departmentId);
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(urid => urid.UserRoleId == userRoleId && urid.DepartmentId == departmentId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var userRoleInDepartment = CreateTestUserRoleInDepartment();
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.ExistsAsync(urid => urid.Id == userRoleInDepartment.Id, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetRolesOfUserInDepartment_Should_Return_Roles_With_Includes()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var role1 = CreateTestRole("Role 1");
            var role2 = CreateTestRole("Role 2");
            
            var userRole1 = CreateTestUserRole(userId, role1.Id);
            var userRole2 = CreateTestUserRole(userId, role2.Id);
            
            var userRoleInDepartment1 = CreateTestUserRoleInDepartment(userRole1.Id, departmentId);
            var userRoleInDepartment2 = CreateTestUserRoleInDepartment(userRole2.Id, departmentId);

            await _context.Roles.AddRangeAsync(role1, role2);
            await _context.UserRoles.AddRangeAsync(userRole1, userRole2);
            await _context.UserRoleInDepartments.AddRangeAsync(userRoleInDepartment1, userRoleInDepartment2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRolesOfUserInDepartment(userId, departmentId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(r => r.Name.Value == "Role 1");
            result.Should().Contain(r => r.Name.Value == "Role 2");
        }

        [Fact]
        public async Task GetRolesOfUserInDepartment_With_No_Roles_Should_Return_Empty_List()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetRolesOfUserInDepartment(userId, departmentId);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRolesOfUserInDepartment_Should_Filter_By_User_And_Department()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var departmentId1 = Guid.NewGuid();
            var departmentId2 = Guid.NewGuid();
            
            var role1 = CreateTestRole("Role 1");
            var role2 = CreateTestRole("Role 2");
            var role3 = CreateTestRole("Role 3");
            
            var userRole1 = CreateTestUserRole(userId1, role1.Id);
            var userRole2 = CreateTestUserRole(userId1, role2.Id);
            var userRole3 = CreateTestUserRole(userId2, role3.Id);
            
            var userRoleInDepartment1 = CreateTestUserRoleInDepartment(userRole1.Id, departmentId1);
            var userRoleInDepartment2 = CreateTestUserRoleInDepartment(userRole2.Id, departmentId2);
            var userRoleInDepartment3 = CreateTestUserRoleInDepartment(userRole3.Id, departmentId1);

            await _context.Roles.AddRangeAsync(role1, role2, role3);
            await _context.UserRoles.AddRangeAsync(userRole1, userRole2, userRole3);
            await _context.UserRoleInDepartments.AddRangeAsync(userRoleInDepartment1, userRoleInDepartment2, userRoleInDepartment3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRolesOfUserInDepartment(userId1, departmentId1);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Value.Should().Be("Role 1");
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_UserRoleInDepartment_With_All_Properties()
        {
            // Arrange
            var userRoleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var userRoleInDepartment = CreateTestUserRoleInDepartment(userRoleId, departmentId);
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(userRoleInDepartment.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userRoleInDepartment.Id);
            result.UserRoleId.Should().Be(userRoleId);
            result.DepartmentId.Should().Be(departmentId);
            result.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321")]
        [InlineData("00000000-0000-0000-0000-000000000000", "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        [InlineData("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA", "BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB")]
        public async Task ExistsAsync_With_Different_Guid_Combinations_Should_Work(string userRoleIdStr, string departmentIdStr)
        {
            // Arrange
            var userRoleId = Guid.Parse(userRoleIdStr);
            var departmentId = Guid.Parse(departmentIdStr);
            var userRoleInDepartment = CreateTestUserRoleInDepartment(userRoleId, departmentId);
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(urid => urid.UserRoleId == userRoleId && urid.DepartmentId == departmentId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddAsync_Should_Handle_Multiple_UserRoleInDepartments()
        {
            // Arrange
            var userRoleInDepartment1 = CreateTestUserRoleInDepartment();
            var userRoleInDepartment2 = CreateTestUserRoleInDepartment();
            var userRoleInDepartment3 = CreateTestUserRoleInDepartment();

            // Act
            await _repository.AddAsync(userRoleInDepartment1);
            await _repository.AddAsync(userRoleInDepartment2);
            await _repository.AddAsync(userRoleInDepartment3);
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.UserRoleInDepartments.CountAsync();
            count.Should().Be(3);
        }

        [Fact]
        public async Task GetRolesOfUserInDepartment_With_Multiple_Users_And_Departments_Should_Work_Correctly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var role1 = CreateTestRole("Admin Role");
            var role2 = CreateTestRole("User Role");
            
            var userRole1 = CreateTestUserRole(userId, role1.Id);
            var userRole2 = CreateTestUserRole(userId, role2.Id);
            
            var userRoleInDepartment1 = CreateTestUserRoleInDepartment(userRole1.Id, departmentId);
            var userRoleInDepartment2 = CreateTestUserRoleInDepartment(userRole2.Id, departmentId);

            await _context.Roles.AddRangeAsync(role1, role2);
            await _context.UserRoles.AddRangeAsync(userRole1, userRole2);
            await _context.UserRoleInDepartments.AddRangeAsync(userRoleInDepartment1, userRoleInDepartment2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRolesOfUserInDepartment(userId, departmentId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(r => r.Name.Value == "Admin Role");
            result.Should().Contain(r => r.Name.Value == "User Role");
        }

        private UserRoleInDepartment CreateTestUserRoleInDepartment(Guid? userRoleId = null, Guid? departmentId = null)
        {
            return new UserRoleInDepartment
            {
                UserRoleId = userRoleId ?? Guid.NewGuid(),
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

        private Role CreateTestRole(string name = "Test Role")
        {
            var entityName = EntityName.Create(name).Data;
            return Role.CreateRole(entityName, "Test Description").Data;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

