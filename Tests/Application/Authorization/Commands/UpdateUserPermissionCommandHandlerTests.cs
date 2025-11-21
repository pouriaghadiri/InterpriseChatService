using Application.Features.AuthorizationUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Services;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authorization.Commands
{
    public class UpdateUserPermissionCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheInvalidationService> _cacheInvalidationServiceMock;
        private readonly UpdateUserPermissionCommandHandler _handler;
        private readonly UpdateUserPermissionCommand _request;

        public UpdateUserPermissionCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheInvalidationServiceMock = new Mock<ICacheInvalidationService>();
            _handler = new UpdateUserPermissionCommandHandler(_unitOfWorkMock.Object, _cacheInvalidationServiceMock.Object);

            _request = new UpdateUserPermissionCommand
            {
                UserId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Throw_NotImplementedException()
        {
            // Arrange - Handler currently throws NotImplementedException

            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() => _handler.Handle(_request, CancellationToken.None));
        }
    }
}

