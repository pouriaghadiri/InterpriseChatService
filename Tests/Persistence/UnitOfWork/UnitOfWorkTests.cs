using Domain.Base.Interface;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using PersistenceUnitOfWork = Persistence.UnitOfWork.UnitOfWork;
using FluentAssertions;
using Moq;
using Xunit;
using Domain.Common.ValueObjects;
using Domain.Entities;

namespace Tests.UnitTest.Persistence.UnitOfWork
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly PersistenceUnitOfWork _unitOfWork;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly Mock<IRolePermissionRepository> _rolePermissionRepositoryMock;
        private readonly Mock<IUserPermissionRepository> _userPermissionRepositoryMock;
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserRoleInDepartmentRepository> _userRoleInDepartmentRepositoryMock;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _rolePermissionRepositoryMock = new Mock<IRolePermissionRepository>();
            _userPermissionRepositoryMock = new Mock<IUserPermissionRepository>();
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userRoleInDepartmentRepositoryMock = new Mock<IUserRoleInDepartmentRepository>();

            _unitOfWork = new PersistenceUnitOfWork(
                _context,
                _roleRepositoryMock.Object,
                _permissionRepositoryMock.Object,
                _rolePermissionRepositoryMock.Object,
                _userPermissionRepositoryMock.Object,
                _departmentRepositoryMock.Object,
                _userRepositoryMock.Object,
                _userRoleInDepartmentRepositoryMock.Object
            );
        }

        [Fact]
        public void UnitOfWork_Should_Have_All_Repository_Properties()
        {
            // Assert
            _unitOfWork.Roles.Should().NotBeNull();
            _unitOfWork.Permissions.Should().NotBeNull();
            _unitOfWork.RolePermissions.Should().NotBeNull();
            _unitOfWork.UserPermissions.Should().NotBeNull();
            _unitOfWork.Departments.Should().NotBeNull();
            _unitOfWork.Users.Should().NotBeNull();
            _unitOfWork.userRoleInDepartment.Should().NotBeNull();
        }

        [Fact]
        public void UnitOfWork_Should_Implement_IUnitOfWork_Interface()
        {
            // Assert
            _unitOfWork.Should().BeAssignableTo<IUnitOfWork>();
        }

        [Fact]
        public async Task SaveChangesAsync_Should_Call_Context_SaveChangesAsync()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Assert
            result.Should().Be(0); // InMemory database returns 0 for empty context
        }

        [Fact]
        public async Task SaveChangesAsync_Without_CancellationToken_Should_Use_Default()
        {
            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task BeginTransactionAsync_Should_Throw_NotImplementedException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() => _unitOfWork.BeginTransactionAsync());
        }

        [Fact]
        public async Task CommitTransactionAsync_Should_Throw_NotImplementedException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() => _unitOfWork.CommitTransactionAsync());
        }

        [Fact]
        public async Task RollBackTransactionAsync_Should_Throw_NotImplementedException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() => _unitOfWork.RollBackTransactionAsync());
        }

        [Fact]
        public void UnitOfWork_Should_Use_Provided_Repositories()
        {
            // Assert
            _unitOfWork.Roles.Should().Be(_roleRepositoryMock.Object);
            _unitOfWork.Permissions.Should().Be(_permissionRepositoryMock.Object);
            _unitOfWork.RolePermissions.Should().Be(_rolePermissionRepositoryMock.Object);
            _unitOfWork.UserPermissions.Should().Be(_userPermissionRepositoryMock.Object);
            _unitOfWork.Departments.Should().Be(_departmentRepositoryMock.Object);
            _unitOfWork.Users.Should().Be(_userRepositoryMock.Object);
            _unitOfWork.userRoleInDepartment.Should().Be(_userRoleInDepartmentRepositoryMock.Object);
        }

        [Fact]
        public async Task SaveChangesAsync_With_Data_Should_Return_Correct_Count()
        {
            // Arrange
            var user = CreateTestUser();
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Add some data

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().Be(0); // No changes to save
        }

        [Fact]
        public async Task SaveChangesAsync_With_New_Data_Should_Return_Correct_Count()
        {
            // Arrange
            var user = CreateTestUser();
            _context.Users.Add(user);

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().Be(1); // One entity added
        }

        [Fact]
        public void UnitOfWork_Constructor_Should_Initialize_All_Repositories()
        {
            // Assert
            _unitOfWork.Roles.Should().NotBeNull();
            _unitOfWork.Permissions.Should().NotBeNull();
            _unitOfWork.RolePermissions.Should().NotBeNull();
            _unitOfWork.UserPermissions.Should().NotBeNull();
            _unitOfWork.Departments.Should().NotBeNull();
            _unitOfWork.Users.Should().NotBeNull();
            _unitOfWork.userRoleInDepartment.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_Should_Handle_Concurrent_Access()
        {
            // Arrange
            var tasks = new List<Task<int>>();
            
            // Act
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(_unitOfWork.SaveChangesAsync());
            }
            
            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().AllBeEquivalentTo(0);
        }

        [Fact]
        public async Task SaveChangesAsync_With_Exception_Should_Propagate_Exception()
        {
            // Arrange
            var contextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new InvalidOperationException("Database error"));

            var unitOfWorkWithMockContext = new PersistenceUnitOfWork(
                contextMock.Object,
                _roleRepositoryMock.Object,
                _permissionRepositoryMock.Object,
                _rolePermissionRepositoryMock.Object,
                _userPermissionRepositoryMock.Object,
                _departmentRepositoryMock.Object,
                _userRepositoryMock.Object,
                _userRoleInDepartmentRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWorkWithMockContext.SaveChangesAsync());
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("Test", "User").Data;
            var email = Email.Create("test@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

