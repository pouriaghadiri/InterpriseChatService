using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Services;
using Domain.Base;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authentication.Commands
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly RefreshTokenCommandHandler _handler;
        private readonly RefreshTokenCommand _request;

        public RefreshTokenCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _handler = new RefreshTokenCommandHandler(_userRepositoryMock.Object, _jwtTokenServiceMock.Object);

            _request = new RefreshTokenCommand
            {
                RefreshToken = "valid-refresh-token"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_NotImplemented_WhenCalled()
        {
            // Arrange - Handler currently returns "Not Implemented"

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Implemented");
            result.Message.Should().Contain("not yet implemented");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRefreshTokenIsEmpty()
        {
            // Arrange
            _request.RefreshToken = "";

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }
    }
}

