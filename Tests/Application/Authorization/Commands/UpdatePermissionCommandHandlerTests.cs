using Application.Features.AuthorizationUseCase.Commands;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authorization.Commands
{
    public class UpdatePermissionCommandHandlerTests
    {
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly UpdatePermissionCommandHandler _handler;
        private readonly UpdatePermissionCommand _request;

        public UpdatePermissionCommandHandlerTests()
        {
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _handler = new UpdatePermissionCommandHandler(_permissionRepositoryMock.Object);

            _request = new UpdatePermissionCommand
            {
                Id = Guid.NewGuid(),
                Name = "UpdateUser",
                Description = "Permission to update users"
            };
        }

        [Fact]
        public async Task Handle_Should_UpdatePermission_WhenInputIsValid()
        {
            // Arrange
            var existingPermission = CreateTestPermission();
            existingPermission.Id = _request.Id;

            _permissionRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingPermission);

            _permissionRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _permissionRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Permission>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Permission updated successfully");
            _permissionRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Permission>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPermissionNotFound()
        {
            // Arrange
            _permissionRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync((Permission?)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Found");
            result.Message.Should().Be("Permission not found");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPermissionNameAlreadyExists()
        {
            // Arrange
            var existingPermission = CreateTestPermission();
            existingPermission.Id = _request.Id;

            _permissionRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingPermission);

            _permissionRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Conflict");
            result.Message.Should().Contain("already exists");
        }

        private Permission CreateTestPermission()
        {
            var name = EntityName.Create("TestPermission").Data;
            return new Permission
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = "Test description"
            };
        }
    }
}

