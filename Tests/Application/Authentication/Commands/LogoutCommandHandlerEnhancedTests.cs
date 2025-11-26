using Application.Common;
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
    public class LogoutCommandHandlerEnhancedTests
    {
        private readonly Mock<ICacheInvalidationService> _cacheInvalidationServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly LogoutCommandHandlerEnhanced _handler;
        private readonly LogoutCommand _request;

        public LogoutCommandHandlerEnhancedTests()
        {
            _cacheInvalidationServiceMock = new Mock<ICacheInvalidationService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _handler = new LogoutCommandHandlerEnhanced(
                _cacheInvalidationServiceMock.Object,
                _cacheServiceMock.Object,
                _httpContextAccessorMock.Object);

            _request = new LogoutCommand();
        }

        [Fact]
        public async Task Handle_Should_BlacklistToken_WhenTokenExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-jwt-token";
            var httpContext = CreateHttpContextWithUserAndToken(userId, token);

            _httpContextAccessorMock
                .Setup(accessor => accessor.HttpContext)
                .Returns(httpContext);

            _cacheInvalidationServiceMock
                .Setup(service => service.InvalidateUserCacheAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _cacheServiceMock
                .Setup(service => service.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _cacheServiceMock
                .Setup(service => service.RemoveAsync(It.IsAny<string>()))
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

            _cacheServiceMock.Verify(
                service => service.SetAsync(
                    It.Is<string>(k => k == CacheHelper.TokenBlacklistKey(token)),
                    It.IsAny<object>(),
                    It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_InvalidateAllUserCaches_WhenUserIsAuthenticated()
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

            _cacheServiceMock
                .Setup(service => service.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _cacheInvalidationServiceMock.Verify(
                service => service.InvalidateUserCacheAsync(userId, It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_RemoveRefreshTokens_WhenUserIsAuthenticated()
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

            _cacheServiceMock
                .Setup(service => service.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _cacheServiceMock.Verify(
                service => service.RemoveAsync(It.Is<string>(k => k == $"refresh_token:{userId}")),
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

        private HttpContext CreateHttpContextWithUserAndToken(Guid userId, string token)
        {
            var httpContext = CreateHttpContextWithUser(userId);
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            return httpContext;
        }
    }
}

