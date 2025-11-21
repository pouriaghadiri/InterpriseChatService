using Application.Features.AuthorizationUseCase.DTOs;
using Application.Features.AuthorizationUseCase.Queries;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authorization.Queries
{
    public class GetPermissionByIdQueryHandlerTests
    {
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly GetPermissionByIdQueryHandler _handler;
        private readonly GetPermissionByIdQuery _request;

        public GetPermissionByIdQueryHandlerTests()
        {
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _handler = new GetPermissionByIdQueryHandler(_permissionRepositoryMock.Object);
            _request = new GetPermissionByIdQuery(Guid.NewGuid());
        }

        [Fact]
        public async Task Handle_Should_Return_Permission_WhenFound()
        {
            // Arrange
            var permission = CreateTestPermission();
            permission.Id = _request.Id;

            _permissionRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(permission);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(_request.Id);
            result.Data.Name.Should().Be("TestPermission");
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

