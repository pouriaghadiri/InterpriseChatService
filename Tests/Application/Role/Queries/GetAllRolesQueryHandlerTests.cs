using Application.Features.RoleUseCase.DTOs;
using Application.Features.RoleUseCase.Queries;
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
using RoleEntity = Domain.Entities.Role;

namespace Tests.Application.Role.Queries
{
    public class GetAllRolesQueryHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly GetAllRolesQueryHandler _handler;
        private readonly GetAllRolesQuery _request;

        public GetAllRolesQueryHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _handler = new GetAllRolesQueryHandler(_roleRepositoryMock.Object);
            _request = new GetAllRolesQuery();
        }

        [Fact]
        public async Task Handle_Should_Return_AllRoles()
        {
            // Arrange
            var roles = new List<RoleEntity>
            {
                CreateTestRole("Admin"),
                CreateTestRole("User"),
                CreateTestRole("Manager")
            };

            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(roles);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Count.Should().Be(3);
            result.Data[0].Name.Should().Be("Admin");
        }

        [Fact]
        public async Task Handle_Should_Return_EmptyList_WhenNoRoles()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<RoleEntity>());

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        private RoleEntity CreateTestRole(string name)
        {
            var entityName = EntityName.Create(name).Data;
            return RoleEntity.CreateRole(entityName, $"Description for {name}").Data;
        }
    }
}

