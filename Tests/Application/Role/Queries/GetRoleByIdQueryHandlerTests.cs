using Application.Features.RoleUseCase.DTOs;
using Application.Features.RoleUseCase.Queries;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Role.Queries
{
    public class GetRoleByIdQueryHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly GetRoleByIdQueryHandler _handler;
        private readonly GetRoleByIdQuery _request;

        public GetRoleByIdQueryHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _handler = new GetRoleByIdQueryHandler(_roleRepositoryMock.Object);
            _request = new GetRoleByIdQuery(Guid.NewGuid());
        }

        [Fact]
        public async Task Handle_Should_Return_Role_WhenFound()
        {
            // Arrange
            var role = CreateTestRole();
            role.Id = _request.Id;

            _roleRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(role);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(_request.Id);
            result.Data.Name.Should().Be("Admin");
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

        private Role CreateTestRole()
        {
            var name = EntityName.Create("Admin").Data;
            return Role.CreateRole(name, "Administrator role").Data;
        }
    }
}

