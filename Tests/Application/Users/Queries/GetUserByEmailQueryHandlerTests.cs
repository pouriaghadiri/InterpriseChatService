using Application.Features.UserUseCase.Queries;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using Domain.Base;

namespace Tests.UnitTest.Users.Queries
{
    public class GetUserByEmailQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserByNameQueryHandler _handler;
        private readonly GetUserByEmailQuery _request;

        public GetUserByEmailQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByNameQueryHandler(_userRepositoryMock.Object);

            _request = new GetUserByEmailQuery("john@example.com");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("NotFound Error");
            result.Message.Should().Be("User is not found!");
        }

        [Fact]
        public async Task Handle_Should_Return_User_WhenUserExists()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock.Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ID.Should().Be(user.Id);
            result.Data.FullName.FirstName.Should().Be(user.FullName.FirstName);
            result.Data.FullName.LastName.Should().Be(user.FullName.LastName);
        }

        [Fact]
        public async Task Handle_Should_Call_Repository_With_Correct_Email()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock.Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            await _handler.Handle(_request, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(x => x.GetbyEmailAsync(It.Is<Email>(e => e.Value == _request.Email)), Times.Once);
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
