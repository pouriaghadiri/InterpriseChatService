using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Queries;
using Domain.Base;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authentication.Queries
{
    public class GetUserProfileQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly GetUserProfileQueryHandler _handler;
        private readonly GetUserProfileQuery _request;

        public GetUserProfileQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _handler = new GetUserProfileQueryHandler(_userRepositoryMock.Object, _httpContextAccessorMock.Object);
            _request = new GetUserProfileQuery();
        }

        [Fact]
        public async Task Handle_Should_Return_Unauthorized_WhenUserNotFoundInToken()
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
            result.Message.Should().Contain("Unable to identify user from token");
        }
    }
}

