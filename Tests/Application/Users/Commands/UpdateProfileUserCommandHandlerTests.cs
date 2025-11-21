using Application.Features.UserUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.UnitTest.Users.Commands
{
    public class UpdateProfileUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateProfileUserCommandHandler _handler;

        public UpdateProfileUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateProfileUserCommandHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_With_Valid_Data_Should_Update_User_Profile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "09123456789",
                Bio = "Updated Bio",
                Location = "Updated Location",
                ProfilePicture = "updated-profile.jpg"
            };

            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("User profile Updated successfully.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_With_Non_Existent_User_Should_Return_NotFound_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "09123456789"
            };

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("NotFound Error");
            result.Message.Should().Be("This user is not valid!");
        }

        [Fact]
        public async Task Handle_With_Invalid_FirstName_Should_Return_Validation_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "", // Invalid empty first name
                LastName = "Doe",
                PhoneNumber = "09123456789"
            };

            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid input");
            result.Message.Should().Be("Fullname validation failed");
        }

        [Fact]
        public async Task Handle_With_Invalid_LastName_Should_Return_Validation_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "John",
                LastName = "", // Invalid empty last name
                PhoneNumber = "09123456789"
            };

            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid input");
            result.Message.Should().Be("Fullname validation failed");
        }

        [Fact]
        public async Task Handle_With_Invalid_PhoneNumber_Should_Return_Validation_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "invalid-phone" // Invalid phone number
            };

            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid input");
            result.Message.Should().Be("Phone number validation failed!");
        }

        [Fact]
        public async Task Handle_With_Valid_Optional_Fields_Should_Update_Successfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "09123456789",
                Bio = "Software Developer",
                Location = "Tehran, Iran",
                ProfilePicture = "profile.jpg"
            };

            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("User profile Updated successfully.");
        }

        [Fact]
        public async Task Handle_With_Null_Optional_Fields_Should_Update_Successfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "09123456789",
                Bio = null,
                Location = null,
                ProfilePicture = null
            };

            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("User profile Updated successfully.");
        }

        [Fact]
        public async Task Handle_With_User_Update_Failure_Should_Return_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "09123456789"
            };

            var user = CreateMockUserWithUpdateFailure();
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Update Error");
            result.Message.Should().Be("Failed to update user profile");
        }

        [Theory]
        [InlineData("John", "Doe", "09123456789")]
        [InlineData("Jane", "Smith", "09987654321")]
        [InlineData("Ali", "Rezaei", "09111111111")]
        public async Task Handle_With_Different_Valid_Names_Should_Update_Successfully(string firstName, string lastName, string phoneNumber)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };

            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("User profile Updated successfully.");
        }

        [Fact]
        public async Task Handle_With_CancellationToken_Should_Pass_To_SaveChanges()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateProfileUserCommand
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "09123456789"
            };

            var user = CreateTestUser();
            var cancellationToken = new CancellationToken();
            
            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(cancellationToken))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("Original", "User").Data;
            var email = Email.Create("original@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09111111111").Data;

            return User.RegisterUser(fullName, email, password, phone, "original.jpg", "Original Bio", "Original Location").Data;
        }

        private User CreateMockUserWithUpdateFailure()
        {
            var user = new Mock<User>();
            user.Setup(x => x.UpdateUserProfile(
                It.IsAny<PersonFullName>(),
                It.IsAny<Email>(),
                It.IsAny<PhoneNumber>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(MessageDTO.Failure("Update Error", null, "Failed to update user profile"));
            
            return user.Object;
        }
    }
}