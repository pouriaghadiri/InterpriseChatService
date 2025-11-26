using Application.Features.AuthenticationUseCase.Commands;
using Domain.Base;
using Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authentication.Commands
{
    public class LogoutCommandHandlerTests
    {
        private readonly Mock<ICacheInvalidationService> _cacheInvalidationServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly LogoutCommandHandler _handler;
        private readonly LogoutCommand _request;

        public LogoutCommandHandlerTests()
        {
            _cacheInvalidationServiceMock = new Mock<ICacheInvalidationService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _handler = new LogoutCommandHandler(
                _cacheInvalidationServiceMock.Object,
                _httpContextAccessorMock.Object);

            _request = new LogoutCommand();
        }

        [Fact]
        public async Task Handle_Should_InvalidateCache_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var httpContext = CreateHttpContextWithUser(userId);

            _httpContextAccessorMock
                .Setup(accessor => accessor.HttpContext)
                .Returns(httpContext);

            _cacheInvalidationServiceMock
                .Setup(service => service.InvalidateUserCacheAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Logged out successfully");
            _cacheInvalidationServiceMock.Verify(
                service => service.InvalidateUserCacheAsync(userId, It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFoundInToken()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal();

            _httpContextAccessorMock
                .Setup(accessor => accessor.HttpContext)
                .Returns(httpContext);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Unauthorized");
            result.Message.Should().Be("User not found in token");
            _cacheInvalidationServiceMock.Verify(
                service => service.InvalidateUserCacheAsync(It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenHttpContextIsNull()
        {
            // Arrange
            _httpContextAccessorMock
                .Setup(accessor => accessor.HttpContext)
                .Returns((HttpContext?)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Unauthorized");
            result.Message.Should().Be("User not found in token");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserIdClaimIsInvalid()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            httpContext.User = new ClaimsPrincipal(identity);

            _httpContextAccessorMock
                .Setup(accessor => accessor.HttpContext)
                .Returns(httpContext);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Unauthorized");
            result.Message.Should().Be("User not found in token");
        }

        private HttpContext CreateHttpContextWithUser(Guid userId)
        {
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Test");
            httpContext.User = new ClaimsPrincipal(identity);
            return httpContext;
        }
    }
}

