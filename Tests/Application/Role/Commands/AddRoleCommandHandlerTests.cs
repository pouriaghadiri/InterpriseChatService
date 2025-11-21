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
using RoleEntity = Domain.Entities.Role;

namespace Tests.Application.Role.Commands
{
    public class AddRoleCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AddRoleCommandHandler _handler;
        private readonly AddRoleCommand _request;

        public AddRoleCommandHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new AddRoleCommandHandler(_unitOfWorkMock.Object, _roleRepositoryMock.Object);

            _request = new AddRoleCommand
            {
                Name = "Admin",
                Description = "Administrator role"
            };
        }

        [Fact]
        public async Task Handle_Should_CreateRole_WhenInputIsValid()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoleEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _roleRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<RoleEntity>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Role added successfully.");
            _roleRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<RoleEntity>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRoleNameIsInvalid()
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
        public async Task Handle_Should_Return_Failure_WhenRoleNameAlreadyExists()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<RoleEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Exist Error");
            result.Message.Should().Contain("already exists");
        }
    }
}

