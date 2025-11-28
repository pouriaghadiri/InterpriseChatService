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
    public class VerifyEmailCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VerifyEmailCommandHandler _handler;
        private readonly VerifyEmailCommand _request;

        public VerifyEmailCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new VerifyEmailCommandHandler(
                _userRepositoryMock.Object,
                _cacheServiceMock.Object,
                _unitOfWorkMock.Object);

            _request = new VerifyEmailCommand
            {
                Email = "john@example.com",
                Token = "valid-verification-token"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Success_WhenValidToken()
        {
            // Arrange
            var user = CreateTestUser();
            var tokenCacheModel = new EmailVerificationTokenCacheModel
            {
                UserId = user.Id,
                Email = user.Email.Value,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(7)
            };

            _cacheServiceMock
                .Setup(service => service.GetAsync<EmailVerificationTokenCacheModel>(It.IsAny<string>()))
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
            result.Message.Should().Contain("Email has been verified successfully");
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
        public async Task Handle_Should_Return_Failure_WhenTokenIsInvalid()
        {
            // Arrange
            _cacheServiceMock
                .Setup(service => service.GetAsync<EmailVerificationTokenCacheModel>(It.IsAny<string>()))
                .ReturnsAsync((EmailVerificationTokenCacheModel)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid Token");
            result.Message.Should().Contain("Invalid or expired verification token");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            var tokenCacheModel = new EmailVerificationTokenCacheModel
            {
                UserId = Guid.NewGuid(),
                Email = "john@example.com",
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(7)
            };

            _cacheServiceMock
                .Setup(service => service.GetAsync<EmailVerificationTokenCacheModel>(It.IsAny<string>()))
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

