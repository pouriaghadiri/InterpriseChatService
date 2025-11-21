using Application.Common.DTOs;
using Application.Features.AuthenticationUseCase.Commands;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authentication.Commands
{
    public class ForgotPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly EmailSettingsDTO _emailSettings;
        private readonly ForgotPasswordCommandHandler _handler;
        private readonly ForgotPasswordCommand _request;

        public ForgotPasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailSettings = new EmailSettingsDTO
            {
                BaseUrl = "https://localhost:7065"
            };
            _handler = new ForgotPasswordCommandHandler(
                _userRepositoryMock.Object,
                _cacheServiceMock.Object,
                _emailServiceMock.Object,
                _emailSettings);

            _request = new ForgotPasswordCommand
            {
                Email = "john@example.com"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Success_WhenEmailIsValid()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            _cacheServiceMock
                .Setup(service => service.GetAsync<string>(It.IsAny<string>()))
                .ReturnsAsync((string)null);

            _cacheServiceMock
                .Setup(service => service.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            _emailServiceMock
                .Setup(service => service.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("password reset link has been sent");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenEmailIsInvalid()
        {
            // Arrange
            _request.Email = "invalid-email";

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid Email");
            result.Message.Should().Contain("valid email address");
        }

        [Fact]
        public async Task Handle_Should_Return_Success_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            // For security, should return success even if user doesn't exist
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("password reset link has been sent");
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;
            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }
    }
}

