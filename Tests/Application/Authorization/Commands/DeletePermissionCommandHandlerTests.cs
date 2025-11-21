using Application.Features.AuthorizationUseCase.Commands;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authorization.Commands
{
    public class DeletePermissionCommandHandlerTests
    {
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly DeletePermissionCommandHandler _handler;
        private readonly DeletePermissionCommand _request;

        public DeletePermissionCommandHandlerTests()
        {
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _handler = new DeletePermissionCommandHandler(_permissionRepositoryMock.Object);

            _request = new DeletePermissionCommand(Guid.NewGuid());
        }

        [Fact]
        public async Task Handle_Should_DeletePermission_WhenInputIsValid()
        {
            // Arrange
            var existingPermission = CreateTestPermission();

            _permissionRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingPermission);

            _permissionRepositoryMock
                .Setup(repo => repo.IsPermissionInUseAsync(_request.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _permissionRepositoryMock
                .Setup(repo => repo.DeleteAsync(It.IsAny<Permission>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Permission deleted successfully");
            _permissionRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Permission>()), Times.Once);
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
        public async Task Handle_Should_Return_Failure_WhenPermissionIsInUse()
        {
            // Arrange
            var existingPermission = CreateTestPermission();

            _permissionRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingPermission);

            _permissionRepositoryMock
                .Setup(repo => repo.IsPermissionInUseAsync(_request.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Conflict");
            result.Message.Should().Contain("cannot be deleted");
        }

        private Permission CreateTestPermission()
        {
            var name = EntityName.Create("TestPermission").Data;
            return new Permission
            {
                Name = name,
                Description = "Test description"
            };
        }
    }
}

