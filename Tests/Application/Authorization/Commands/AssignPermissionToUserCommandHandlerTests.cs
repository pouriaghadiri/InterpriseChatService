using Application.Features.AuthorizationUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using DepartmentEntity = Domain.Entities.Department;

namespace Tests.Application.Authorization.Commands
{
    public class AssignPermissionToUserCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AssignPermissionToUserCommandHandler _handler;
        private readonly AssignPermissionToUserCommand _request;

        public AssignPermissionToUserCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new AssignPermissionToUserCommandHandler(_unitOfWorkMock.Object);

            _request = new AssignPermissionToUserCommand
            {
                UserId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_AssignPermission_WhenInputIsValid()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Permissions.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Departments.ExistsAsync(It.IsAny<Expression<Func<DepartmentEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.UserPermissions.ExistsAsync(It.IsAny<Expression<Func<UserPermission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(uow => uow.UserPermissions.AddAsync(It.IsAny<UserPermission>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("assigned to user successfully");
            _unitOfWorkMock.Verify(uow => uow.UserPermissions.AddAsync(It.IsAny<UserPermission>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Contain("user doesn't have exist");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPermissionNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Permissions.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Contain("permission doesn't have exist");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenDepartmentNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Permissions.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Departments.ExistsAsync(It.IsAny<Expression<Func<DepartmentEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Contain("department doesn't have exist");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPermissionAlreadyAssigned()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Permissions.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Departments.ExistsAsync(It.IsAny<Expression<Func<DepartmentEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.UserPermissions.ExistsAsync(It.IsAny<Expression<Func<UserPermission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Exist Error");
            result.Message.Should().Contain("already assigned");
        }
    }
}

