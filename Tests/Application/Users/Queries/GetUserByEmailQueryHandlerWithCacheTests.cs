using Application.Features.UserUseCase.Queries;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Moq;
using Xunit;
using FluentAssertions;
using Domain.Base;

namespace Tests.UnitTest.Users.Queries
{
    public class GetUserByEmailQueryHandlerWithCacheTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly GetUserByEmailQueryHandlerWithCache _handler;
        private readonly GetUserByEmailQuery _request;

        public GetUserByEmailQueryHandlerWithCacheTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new GetUserByEmailQueryHandlerWithCache(_userRepositoryMock.Object, _cacheServiceMock.Object);

            _request = new GetUserByEmailQuery("john@example.com");
        }

        [Fact]
        public async Task Handle_Should_Return_User_From_Cache_When_Available()
        {
            // Arrange
            var userDto = CreateTestUserDTO();
            var cacheKey = $"user:email:{_request.Email}";
            
            _cacheServiceMock.Setup(x => x.GetAsync<Application.Features.UserUseCase.DTOs.UserDTO>(cacheKey))
                .ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ID.Should().Be(userDto.ID);
            
            // Verify cache was checked
            _cacheServiceMock.Verify(x => x.GetAsync<Application.Features.UserUseCase.DTOs.UserDTO>(cacheKey), Times.Once);
            
            // Verify repository was not called
            _userRepositoryMock.Verify(x => x.GetbyEmailAsync(It.IsAny<Email>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_User_From_Database_When_Not_In_Cache()
        {
            // Arrange
            var user = CreateTestUser();
            var cacheKey = $"user:email:{_request.Email}";
            
            _cacheServiceMock.Setup(x => x.GetAsync<Application.Features.UserUseCase.DTOs.UserDTO>(cacheKey))
                .ReturnsAsync((Application.Features.UserUseCase.DTOs.UserDTO)null);
            _userRepositoryMock.Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);
            _cacheServiceMock.Setup(x => x.SetAsync(cacheKey, It.IsAny<Application.Features.UserUseCase.DTOs.UserDTO>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ID.Should().Be(user.Id);
            
            // Verify cache was checked first
            _cacheServiceMock.Verify(x => x.GetAsync<Application.Features.UserUseCase.DTOs.UserDTO>(cacheKey), Times.Once);
            
            // Verify repository was called
            _userRepositoryMock.Verify(x => x.GetbyEmailAsync(It.IsAny<Email>()), Times.Once);
            
            // Verify user was cached
            _cacheServiceMock.Verify(x => x.SetAsync(cacheKey, It.IsAny<Application.Features.UserUseCase.DTOs.UserDTO>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            var cacheKey = $"user:email:{_request.Email}";
            
            _cacheServiceMock.Setup(x => x.GetAsync<Application.Features.UserUseCase.DTOs.UserDTO>(cacheKey))
                .ReturnsAsync((Application.Features.UserUseCase.DTOs.UserDTO)null);
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
        public async Task Handle_Should_Use_Correct_Cache_Key()
        {
            // Arrange
            var user = CreateTestUser();
            var cacheKey = $"user:email:{_request.Email}";
            
            _cacheServiceMock.Setup(x => x.GetAsync<Application.Features.UserUseCase.DTOs.UserDTO>(It.IsAny<string>()))
                .ReturnsAsync((Application.Features.UserUseCase.DTOs.UserDTO)null);
            _userRepositoryMock.Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);
            _cacheServiceMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<Application.Features.UserUseCase.DTOs.UserDTO>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(_request, CancellationToken.None);

            // Assert
            _cacheServiceMock.Verify(x => x.GetAsync<Application.Features.UserUseCase.DTOs.UserDTO>(cacheKey), Times.Once);
            _cacheServiceMock.Verify(x => x.SetAsync(cacheKey, It.IsAny<Application.Features.UserUseCase.DTOs.UserDTO>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;
            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }

        private Application.Features.UserUseCase.DTOs.UserDTO CreateTestUserDTO()
        {
            return new Application.Features.UserUseCase.DTOs.UserDTO
            {
                ID = Guid.NewGuid(),
                FullName = PersonFullName.Create("John", "Doe").Data,
                UserRoleInDepartments = new List<Application.Features.UserUseCase.DTOs.UserRoleInDepartmentDTO>()
            };
        }
    }
}
