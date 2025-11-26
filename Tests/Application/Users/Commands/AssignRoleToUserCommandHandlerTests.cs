using Application.Features.UserUseCase.Commands;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Moq;
using Xunit;
using FluentAssertions;
using Domain.Base;
using RoleEntity = Domain.Entities.Role;
using DepartmentEntity = Domain.Entities.Department;

namespace Tests.UnitTest.Users.Commands
{
    public class AssignRoleToUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
        private readonly Mock<IUserRoleInDepartmentRepository> _userRoleInDepartmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheInvalidationService> _cacheInvalidationServiceMock;
        private readonly AssignRoleToUserCommandHandler _handler;
        private readonly AssignRoleToUserCommand _request;

        public AssignRoleToUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
            _userRoleInDepartmentRepositoryMock = new Mock<IUserRoleInDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheInvalidationServiceMock = new Mock<ICacheInvalidationService>();
            
            _handler = new AssignRoleToUserCommandHandler(
                _roleRepositoryMock.Object,
                _userRepositoryMock.Object,
                _departmentRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _userRoleRepositoryMock.Object,
                _userRoleInDepartmentRepositoryMock.Object,
                _cacheInvalidationServiceMock.Object);

            var user = CreateTestUser();
            var role = CreateTestRole();
            var department = CreateTestDepartment();
            
            _request = new AssignRoleToUserCommand
            {
                User = user,
                Role = role,
                Department = department
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.User.Id))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Be("The selected user doesn't have exist!");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenDepartmentNotFound()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.User.Id))
                .ReturnsAsync(user);
            _departmentRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DepartmentEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Be("The selected department doesn't have exist!");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRoleNotFound()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.User.Id))
                .ReturnsAsync(user);
            _departmentRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DepartmentEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _roleRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<RoleEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Be("The selected role doesn't have exist!");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserAlreadyHasRoleInDepartment()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.User.Id))
                .ReturnsAsync(user);
            _departmentRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DepartmentEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _roleRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<RoleEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Mock that user already has this role in this department
            _userRoleInDepartmentRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserRoleInDepartment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Exist Error");
            result.Message.Should().Be("This Assignment is exist!");
        }

        [Fact]
        public async Task Handle_Should_AssignRole_WhenInputIsValid()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.User.Id))
                .ReturnsAsync(user);
            _departmentRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DepartmentEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _roleRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<RoleEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Mock that user doesn't have this role in this department
            _userRoleInDepartmentRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<UserRoleInDepartment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Title.Should().Be("Created");
            result.Message.Should().Be("Assigned role to user successfully complleted.");
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;
            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }

        private RoleEntity CreateTestRole()
            {
            var name = EntityName.Create("Admin").Data;
            return RoleEntity.CreateRole(name, "Administrator role").Data;
        }

        private DepartmentEntity CreateTestDepartment()
            {
            var name = EntityName.Create("Engineering").Data;
            return DepartmentEntity.CreateDepartment(name).Data;
        }
    }
}
