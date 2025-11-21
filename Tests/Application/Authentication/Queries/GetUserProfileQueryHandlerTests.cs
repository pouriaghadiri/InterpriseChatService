using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Queries;
using Domain.Base;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authentication.Queries
{
    public class GetUserProfileQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserProfileQueryHandler _handler;
        private readonly GetUserProfileQuery _request;

        public GetUserProfileQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserProfileQueryHandler(_userRepositoryMock.Object);
            _request = new GetUserProfileQuery();
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
    }
}

