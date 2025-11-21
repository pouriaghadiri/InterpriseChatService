using Application.Common.CacheModels;
using Application.Features.AuthenticationUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Authentication.Commands
{
    public class ResetPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly ResetPasswordCommandHandler _handler;
        private readonly ResetPasswordCommand _request;

        public ResetPasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new ResetPasswordCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _cacheServiceMock.Object);

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
            var tokenCacheModel = new PasswordResetTokenCacheModel
            {
                UserId = user.Id,
                Email = user.Email.Value,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _cacheServiceMock
                .Setup(service => service.GetAsync<PasswordResetTokenCacheModel>(It.IsAny<string>()))
                .ReturnsAsync(tokenCacheModel);

            _userRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);

            _cacheServiceMock
                .Setup(service => service.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

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
        public async Task Handle_Should_Return_Failure_WhenTokenIsInvalid()
        {
            // Arrange
            _cacheServiceMock
                .Setup(service => service.GetAsync<PasswordResetTokenCacheModel>(It.IsAny<string>()))
                .ReturnsAsync((PasswordResetTokenCacheModel)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid Token");
            result.Message.Should().Contain("Invalid or expired reset token");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            var tokenCacheModel = new PasswordResetTokenCacheModel
            {
                UserId = Guid.NewGuid(),
                Email = "john@example.com",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _cacheServiceMock
                .Setup(service => service.GetAsync<PasswordResetTokenCacheModel>(It.IsAny<string>()))
                .ReturnsAsync(tokenCacheModel);

            _userRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(It.IsAny<Guid>()))
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

