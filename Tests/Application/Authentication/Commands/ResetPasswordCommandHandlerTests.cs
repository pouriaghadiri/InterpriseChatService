using Application.Features.AuthenticationUseCase.Commands;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authentication.Commands
{
    public class ResetPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly ResetPasswordCommandHandler _handler;
        private readonly ResetPasswordCommand _request;

        public ResetPasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new ResetPasswordCommandHandler(_userRepositoryMock.Object);

            _request = new ResetPasswordCommand
            {
                Email = "john@example.com",
                NewPassword = "NewPassword123!@#",
                ConfirmPassword = "NewPassword123!@#",
                Token = "valid-reset-token"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Success_WhenValidToken()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Contain("Password has been reset successfully");
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
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPasswordsDoNotMatch()
        {
            // Arrange
            _request.ConfirmPassword = "DifferentPassword123!@#";

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Password Mismatch");
            result.Message.Should().Contain("do not match");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPasswordIsInvalid()
        {
            // Arrange
            _request.NewPassword = "weak";
            _request.ConfirmPassword = "weak";

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid Password");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("User Not Found");
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

