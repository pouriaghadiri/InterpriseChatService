using Application.Features.RoleUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using RoleEntity = Domain.Entities.Role; 

namespace Tests.Application.Role.Commands
{
    public class DeleteRoleCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteRoleCommandHandler _handler;
        private readonly DeleteRoleCommand _request;

        public DeleteRoleCommandHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteRoleCommandHandler(_unitOfWorkMock.Object, _roleRepositoryMock.Object);

            _request = new DeleteRoleCommand(Guid.NewGuid());
        }

        [Fact]
        public async Task Handle_Should_DeleteRole_WhenInputIsValid()
        {
            // Arrange
            var existingRole = CreateTestRole();

            _roleRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingRole);

            _roleRepositoryMock
                .Setup(repo => repo.IsRoleInUseAsync(_request.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _roleRepositoryMock
                .Setup(repo => repo.DeleteAsync(It.IsAny<RoleEntity>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Role deleted successfully.");
            _roleRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<RoleEntity>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRoleNotFound()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync((RoleEntity?)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Found");
            result.Message.Should().Be("Role not found.");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRoleIsInUse()
        {
            // Arrange
            var existingRole = CreateTestRole();

            _roleRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingRole);

            _roleRepositoryMock
                .Setup(repo => repo.IsRoleInUseAsync(_request.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("In Use Error");
            result.Message.Should().Contain("assigned to users");
        }

        private RoleEntity CreateTestRole()
        {
            var name = EntityName.Create("Admin").Data;
            return RoleEntity.CreateRole(name, "Administrator role").Data;
        }
    }
}

