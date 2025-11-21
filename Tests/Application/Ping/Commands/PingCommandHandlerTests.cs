using Application.Features.PingUserCase;
using Application.Features.UserUseCase;
using Domain.Base.Interface;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Ping.Commands
{
    public class PingCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PingCommandHandler _handler;
        private readonly PingCommand _request;

        public PingCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new PingCommandHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object);
            _request = new PingCommand();
        }

        [Fact]
        public async Task Handle_Should_Return_Pong()
        {
            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().Be("Pong");
        }

        [Fact]
        public async Task Handle_Should_Complete_Successfully()
        {
            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("Pong");
        }
    }
}

