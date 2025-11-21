using Application.Features.RoleUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Role.Commands
{
    public class UpdateRoleCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateRoleCommandHandler _handler;
        private readonly UpdateRoleCommand _request;

        public UpdateRoleCommandHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateRoleCommandHandler(_unitOfWorkMock.Object, _roleRepositoryMock.Object);

            var name = EntityName.Create("UpdatedAdmin").Data;
            _request = new UpdateRoleCommand
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = "Updated administrator role"
            };
        }

        [Fact]
        public async Task Handle_Should_UpdateRole_WhenInputIsValid()
        {
            // Arrange
            var existingRole = CreateTestRole();
            existingRole.Id = _request.Id;

            _roleRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingRole);

            _roleRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _roleRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Role>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Role updated successfully.");
            _roleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRoleNotFound()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync((Role?)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Found");
            result.Message.Should().Be("Role not found.");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRoleNameAlreadyExists()
        {
            // Arrange
            var existingRole = CreateTestRole();
            existingRole.Id = _request.Id;

            _roleRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingRole);

            _roleRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Exist Error");
            result.Message.Should().Contain("already exists");
        }

        private Role CreateTestRole()
        {
            var name = EntityName.Create("Admin").Data;
            return Role.CreateRole(name, "Administrator role").Data;
        }
    }
}

