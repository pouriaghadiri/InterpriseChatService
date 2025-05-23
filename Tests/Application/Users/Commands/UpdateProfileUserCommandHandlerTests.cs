using Application.Features.UserUseCase.Commands;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using Domain.Base;

namespace Tests.UnitTest.Users.Commands
{
    public class UpdateProfileUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateProfileUserCommandHandler _handler;
        private readonly UpdateProfileUserCommand _request;

        public UpdateProfileUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateProfileUserCommandHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object);

            _request = new UpdateProfileUserCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "09115100001",
                Bio = "Developer",
                Location = "Tehran",
                ProfilePicture = "profile.jpg"
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
            result.Title.Should().Be("NotFound Error");
            result.Message.Should().Be("This user is not valid!");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenInvalidPhoneNumber()
        {
            // Arrange
            var createdFullName = PersonFullName.Create("John", "Doe");
            var createdEmail = Email.Create("john@example.com");
            var createdPassword = HashedPassword.Create("password");
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

            _request.PhoneNumber = "invalid";

            _userRepositoryMock.Setup(x => x.GetbyIdAsync(_request.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid input");
            result.Message.Should().Be("Phone number validation failed!");
        }

        [Fact]
        public async Task Handle_Should_UpdateProfile_WhenInputIsValid()
        {
            // Arrange
            var createdFullName = PersonFullName.Create("John", "Doe");
            var createdEmail = Email.Create("john@example.com");
            var createdPassword = HashedPassword.Create("password");
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
            result.Message.Should().Be("User profile Updated successfully.");
        }
    }
} 