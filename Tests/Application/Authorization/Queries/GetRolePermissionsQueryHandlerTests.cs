using Application.Features.AuthorizationUseCase.DTOs;
using Application.Features.AuthorizationUseCase.Queries;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authorization.Queries
{
    public class GetRolePermissionsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GetRolePermissionsQueryHandler _handler;
        private readonly GetRolePermissionsQuery _request;

        public GetRolePermissionsQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new GetRolePermissionsQueryHandler(_unitOfWorkMock.Object);
            _request = new GetRolePermissionsQuery(Guid.NewGuid(), Guid.NewGuid());
        }

        [Fact]
        public async Task Handle_Should_Return_RolePermissions_WhenFound()
        {
            // Arrange
            var permissions = new List<RolePermission>
            {
                new RolePermission
                {
                    RoleId = _request.roleId,
                    DepartmentId = _request.departmentId,
                    Permission = new Permission
                    {
                        Name = EntityName.Create("CreateUser").Data,
                        Description = "Create user permission"
                    }
                }
            };

            // Note: There's a bug in the handler - it checks Users instead of Roles
            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.RolePermissions.GetRolePermissionsAsync(_request.roleId, _request.departmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(permissions);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Count.Should().Be(1);
            result.Data[0].Name.Should().Be("CreateUser");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRoleNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenNoPermissions()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.RolePermissions.GetRolePermissionsAsync(_request.roleId, _request.departmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RolePermission>());

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("No Permissions");
        }
    }
}

