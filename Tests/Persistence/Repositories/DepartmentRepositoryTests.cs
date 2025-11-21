using Domain.Common.ValueObjects;
using Domain.Entities; 
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Xunit;

namespace Tests.UnitTest.Persistence.Repositories
{
    public class DepartmentRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DepartmentRepository _repository;

        public DepartmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new DepartmentRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Department_To_Context()
        {
            // Arrange
            var department = CreateTestDepartment();

            // Act
            await _repository.AddAsync(department);
            await _context.SaveChangesAsync();

            // Assert
            var savedDepartment = await _context.Departments.FindAsync(department.Id);
            savedDepartment.Should().NotBeNull();
            savedDepartment.Id.Should().Be(department.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_Department()
        {
            // Arrange
            var department = CreateTestDepartment();
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(department.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(department.Id);
            result.Name.Value.Should().Be(department.Name.Value);
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
        public async Task GetAllAsync_Should_Return_All_Departments()
        {
            // Arrange
            var department1 = CreateTestDepartment("Department 1");
            var department2 = CreateTestDepartment("Department 2");
            var department3 = CreateTestDepartment("Department 3");

            await _context.Departments.AddRangeAsync(department1, department2, department3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(d => d.Id == department1.Id);
            result.Should().Contain(d => d.Id == department2.Id);
            result.Should().Contain(d => d.Id == department3.Id);
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
        public async Task ExistsAsync_With_Existing_Department_Should_Return_True()
        {
            // Arrange
            var department = CreateTestDepartment();
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(d => d.Id == department.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_Department_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(d => d.Id == nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_With_Name_Predicate_Should_Work_Correctly()
        {
            // Arrange
            var department = CreateTestDepartment("Engineering Department");
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(d => d.Name.Value == "Engineering Department");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Department()
        {
            // Arrange
            var department = CreateTestDepartment("Original Name");
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            var newName = EntityName.Create("Updated Name").Data;
            department.Name = newName;

            // Act
            await _repository.UpdateAsync(department);
            await _context.SaveChangesAsync();

            // Assert
            var updatedDepartment = await _context.Departments.FindAsync(department.Id);
            updatedDepartment.Should().NotBeNull();
            updatedDepartment.Name.Value.Should().Be("Updated Name");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Department()
        {
            // Arrange
            var department = CreateTestDepartment();
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(department);
            await _context.SaveChangesAsync();

            // Assert
            var deletedDepartment = await _context.Departments.FindAsync(department.Id);
            deletedDepartment.Should().BeNull();
        }

        [Fact]
        public async Task IsDepartmentInUseAsync_With_Department_In_Use_Should_Return_True()
        {
            // Arrange
            var department = CreateTestDepartment();
            var userRole = CreateTestUserRole();
            var userRoleInDepartment = new UserRoleInDepartment
            {
                UserRoleId = userRole.Id,
                DepartmentId = department.Id
            };

            await _context.Departments.AddAsync(department);
            await _context.UserRoles.AddAsync(userRole);
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsDepartmentInUseAsync(department.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsDepartmentInUseAsync_With_Department_Not_In_Use_Should_Return_False()
        {
            // Arrange
            var department = CreateTestDepartment();
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsDepartmentInUseAsync(department.Id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsDepartmentInUseAsync_With_Non_Existent_Department_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.IsDepartmentInUseAsync(nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsDepartmentInUseAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var department = CreateTestDepartment();
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.IsDepartmentInUseAsync(department.Id, cancellationToken);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("Engineering Department")]
        [InlineData("Human Resources")]
        [InlineData("Marketing Team")]
        [InlineData("IT Support")]
        public async Task ExistsAsync_With_Different_Department_Names_Should_Work(string departmentName)
        {
            // Arrange
            var department = CreateTestDepartment(departmentName);
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(d => d.Name.Value == departmentName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var department = CreateTestDepartment();
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.ExistsAsync(d => d.Id == department.Id, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        private Department CreateTestDepartment(string name = "Test Department")
        {
            var entityName = EntityName.Create(name).Data;
            return Department.CreateDepartment(entityName).Data;
        }

        private UserRole CreateTestUserRole()
        {
            return new UserRole
            {
                UserId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

