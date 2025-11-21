using Application.Features.AuthorizationUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authorization.Commands
{
    public class AssignPermissionToRoleCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AssignPermissionToRoleCommandHandler _handler;
        private readonly AssignPermissionToRoleCommand _request;

        public AssignPermissionToRoleCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new AssignPermissionToRoleCommandHandler(_unitOfWorkMock.Object);

            _request = new AssignPermissionToRoleCommand
            {
                RoleId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_AssignPermission_WhenInputIsValid()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Roles.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Permissions.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Departments.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.RolePermissions.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.RolePermission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(uow => uow.RolePermissions.AddAsync(It.IsAny<Domain.Entities.RolePermission>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("assigned to role successfully");
            _unitOfWorkMock.Verify(uow => uow.RolePermissions.AddAsync(It.IsAny<Domain.Entities.RolePermission>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRoleNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Roles.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Contain("role doesn't have exist");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPermissionAlreadyAssigned()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Roles.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Permissions.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Departments.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.RolePermissions.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.RolePermission, bool>>>(), It.IsAny<CancellationToken>()))
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

