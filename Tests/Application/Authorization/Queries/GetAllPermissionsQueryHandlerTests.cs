using Application.Features.AuthorizationUseCase.DTOs;
using Application.Features.AuthorizationUseCase.Queries;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authorization.Queries
{
    public class GetAllPermissionsQueryHandlerTests
    {
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly GetAllPermissionsQueryHandler _handler;
        private readonly GetAllPermissionsQuery _request;

        public GetAllPermissionsQueryHandlerTests()
        {
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _handler = new GetAllPermissionsQueryHandler(_permissionRepositoryMock.Object);
            _request = new GetAllPermissionsQuery();
        }

        [Fact]
        public async Task Handle_Should_Return_AllPermissions()
        {
            // Arrange
            var permissions = new List<Permission>
            {
                CreateTestPermission("CreateUser"),
                CreateTestPermission("UpdateUser"),
                CreateTestPermission("DeleteUser")
            };

            _permissionRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(permissions);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Count.Should().Be(3);
            result.Data[0].Name.Should().Be("CreateUser");
        }

        [Fact]
        public async Task Handle_Should_Return_EmptyList_WhenNoPermissions()
        {
            // Arrange
            _permissionRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Permission>());

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        private Permission CreateTestPermission(string name)
        {
            var entityName = EntityName.Create(name).Data;
            return new Permission
            {
                Name = entityName,
                Description = $"Description for {name}"
            };
        }
    }
}

