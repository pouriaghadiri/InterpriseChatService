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
    public class CreatePermissionCommandHandlerTests
    {
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly CreatePermissionCommandHandler _handler;
        private readonly CreatePermissionCommand _request;

        public CreatePermissionCommandHandlerTests()
        {
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _handler = new CreatePermissionCommandHandler(_permissionRepositoryMock.Object);

            _request = new CreatePermissionCommand
            {
                Name = "CreateUser",
                Description = "Permission to create users"
            };
        }

        [Fact]
        public async Task Handle_Should_CreatePermission_WhenInputIsValid()
        {
            // Arrange
            _permissionRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _permissionRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Permission>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Permission created successfully.");
            _permissionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Permission>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPermissionNameIsInvalid()
        {
            // Arrange
            _request.Name = "";

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPermissionNameAlreadyExists()
        {
            // Arrange
            _permissionRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Exists");
            result.Message.Should().Contain("already exists");
        }
    }
}

