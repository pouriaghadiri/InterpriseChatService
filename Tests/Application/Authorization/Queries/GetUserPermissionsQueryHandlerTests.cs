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
    public class GetUserPermissionsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GetUserPermissionsQueryHandler _handler;
        private readonly GetUserPermissionsQuery _request;

        public GetUserPermissionsQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new GetUserPermissionsQueryHandler(_unitOfWorkMock.Object);
            _request = new GetUserPermissionsQuery(Guid.NewGuid(), Guid.NewGuid());
        }

        [Fact]
        public async Task Handle_Should_Return_UserPermissions_WhenFound()
        {
            // Arrange
            var permissions = new List<UserPermission>
            {
                new UserPermission
                {
                    UserId = _request.userId,
                    DepartmentId = _request.departmentId,
                    Permission = new Permission
                    {
                        Name = EntityName.Create("CreateUser").Data,
                        Description = "Create user permission"
                    }
                }
            };

            _unitOfWorkMock.Setup(uow => uow.Users.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.UserPermissions.GetUserPermissionsAsync(_request.userId, _request.departmentId, It.IsAny<CancellationToken>()))
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
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
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
            _unitOfWorkMock.Setup(uow => uow.UserPermissions.GetUserPermissionsAsync(_request.userId, _request.departmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserPermission>());

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("No Permissions");
        }
    }
}

