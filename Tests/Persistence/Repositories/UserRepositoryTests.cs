using Domain.Common.ValueObjects;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Persistence.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_User_To_Context()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            await _repository.AddAsync(user);
            await _context.SaveChangesAsync();

            // Assert
            var savedUser = await _context.Users.FindAsync(user.Id);
            savedUser.Should().NotBeNull();
            savedUser.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Return_User_With_Includes()
        {
            // Arrange
            var user = CreateTestUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.UserRoles.Should().NotBeNull();
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
        public async Task GetbyEmailAsync_Should_Return_User_With_Includes()
        {
            // Arrange
            var user = CreateTestUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyEmailAsync(user.Email);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Email.Value.Should().Be(user.Email.Value);
            result.UserRoles.Should().NotBeNull();
        }

        [Fact]
        public async Task GetbyEmailAsync_With_Non_Existent_Email_Should_Return_Null()
        {
            // Arrange
            var nonExistentEmail = Email.Create("nonexistent@example.com").Data;

            // Act
            var result = await _repository.GetbyEmailAsync(nonExistentEmail);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_With_Existing_User_Should_Return_True()
        {
            // Arrange
            var user = CreateTestUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(u => u.Id == user.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_User_Should_Return_False()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(u => u.Id == nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_With_Email_Predicate_Should_Work_Correctly()
        {
            // Arrange
            var user = CreateTestUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(u => u.Email.Value == user.Email.Value);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetbyEmailAsync_Should_Handle_Case_Insensitive_Email()
        {
            // Arrange
            var user = CreateTestUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var upperCaseEmail = Email.Create(user.Email.Value.ToUpper()).Data;

            // Act
            var result = await _repository.GetbyEmailAsync(upperCaseEmail);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetbyIdAsync_Should_Include_UserRoles_And_Related_Data()
        {
            // Arrange
            var user = CreateTestUser();
            var role = CreateTestRole();
            var department = CreateTestDepartment();
            var userRole = new UserRole { UserId = user.Id, RoleId = role.Id };
            var userRoleInDepartment = new UserRoleInDepartment 
            { 
                UserRoleId = userRole.Id, 
                DepartmentId = department.Id 
            };

            await _context.Users.AddAsync(user);
            await _context.Roles.AddAsync(role);
            await _context.Departments.AddAsync(department);
            await _context.UserRoles.AddAsync(userRole);
            await _context.UserRoleInDepartments.AddAsync(userRoleInDepartment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.UserRoles.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user@domain.org")]
        [InlineData("admin@company.co.uk")]
        public async Task GetbyEmailAsync_With_Different_Email_Formats_Should_Work(string emailAddress)
        {
            // Arrange
            var email = Email.Create(emailAddress).Data;
            var user = CreateTestUserWithEmail(email);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetbyEmailAsync(email);

            // Assert
            result.Should().NotBeNull();
            result.Email.Value.Should().Be(emailAddress.ToLower());
        }

        [Fact]
        public async Task ExistsAsync_With_CancellationToken_Should_Work()
        {
            // Arrange
            var user = CreateTestUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _repository.ExistsAsync(u => u.Id == user.Id, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john.doe@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }

        private User CreateTestUserWithEmail(Email email)
        {
            var fullName = PersonFullName.Create("Test", "User").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }

        private Role CreateTestRole()
        {
            var name = EntityName.Create("Test Role").Data;
            return Role.CreateRole(name, "Test Description").Data;
        }

        private Department CreateTestDepartment()
        {
            var name = EntityName.Create("Test Department").Data;
            return Department.CreateDepartment(name).Data;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

