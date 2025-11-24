using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using Domain.Base;
using Application.Features.AuthenticationUseCase.Commands;

namespace Tests.UnitTest.Users.Commands
{
    public class ChangePasswordUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ChangePasswordUserCommandHandler _handler;
        private readonly ChangePasswordUserCommand _request;

        public ChangePasswordUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ChangePasswordUserCommandHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object);

            _request = new ChangePasswordUserCommand
            {
                Id = Guid.NewGuid(),
                CurrentPassword = "currentPass",
                NewPassword = "newPass",
                NewPasswordConfirm = "newPass"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.Id))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Found Error");
            result.Message.Should().Be("This user is not valid!");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenCurrentPasswordIncorrect()
        {
            // Arrange
            var createdFullName = PersonFullName.Create("John", "Doe");
            var createdEmail = Email.Create("john@example.com");
            var createdPassword = HashedPassword.Create("correctPass");
            var phoneResult = PhoneNumber.Create("09115100000");
            var user = User.RegisterUser(
                createdFullName.Data,
                createdEmail.Data,
                createdPassword.Data,
                phoneResult.Data,
                null,
                null,
                null
            ).Data;

            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Error");
            result.Message.Should().Be("The current password is incorrect.");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPasswordsDontMatch()
        {
            // Arrange
            var createdFullName = PersonFullName.Create("John", "Doe");
            var createdEmail = Email.Create("john@example.com");
            var createdPassword = HashedPassword.Create("currentPass");
            var phoneResult = PhoneNumber.Create("09115100000");
            var user = User.RegisterUser(
                createdFullName.Data,
                createdEmail.Data,
                createdPassword.Data,
                phoneResult.Data,
                null,
                null,
                null
            ).Data;

            _request.NewPasswordConfirm = "differentPass";

            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Error");
            result.Message.Should().Be("The new password and confirmation password do not match.");
        }

        [Fact]
        public async Task Handle_Should_ChangePassword_WhenInputIsValid()
        {
            // Arrange
            var createdFullName = PersonFullName.Create("John", "Doe");
            var createdEmail = Email.Create("john@example.com");
            var createdPassword = HashedPassword.Create("currentPass");
            var phoneResult = PhoneNumber.Create("09115100000");
            var user = User.RegisterUser(
                createdFullName.Data,
                createdEmail.Data,
                createdPassword.Data,
                phoneResult.Data,
                null,
                null,
                null
            ).Data;

            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.Id))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Title.Should().Be("Updated");
            result.Message.Should().Be("Password changed successfully.");
        }
    }
} 