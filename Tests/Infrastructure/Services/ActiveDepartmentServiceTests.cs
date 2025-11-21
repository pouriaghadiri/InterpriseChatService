using Application.Common;
using Application.Common.CacheModels;
using Domain.Base;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using FluentAssertions;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace Tests.UnitTest.Infrastructure.Services
{
    public class ActiveDepartmentServiceTests
    {
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly ActiveDepartmentService _activeDepartmentService;

        public ActiveDepartmentServiceTests()
        {
            _cacheServiceMock = new Mock<ICacheService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _activeDepartmentService = new ActiveDepartmentService(_cacheServiceMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetActiveDepartmentIdAsync_With_Cached_Data_Should_Return_Cached_DepartmentId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
            var cachedData = new ActiveDepartmentCacheModel { DepartmentId = departmentId };

            _cacheServiceMock
                .Setup(x => x.GetAsync<ActiveDepartmentCacheModel>(cacheKey))
                .ReturnsAsync(cachedData);

            // Act
            var result = await _activeDepartmentService.GetActiveDepartmentIdAsync(userId);

            // Assert
            result.Should().Be(departmentId);
            _userRepositoryMock.Verify(x => x.GetbyIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetActiveDepartmentIdAsync_Without_Cached_Data_Should_Get_From_Database_And_Cache()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
            var user = CreateMockUser(userId, departmentId);

            _cacheServiceMock
                .Setup(x => x.GetAsync<ActiveDepartmentCacheModel>(cacheKey))
                .ReturnsAsync((ActiveDepartmentCacheModel)null);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _cacheServiceMock
                .Setup(x => x.SetAsync(cacheKey, It.IsAny<ActiveDepartmentCacheModel>(), CacheHelper.Expiration.User))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _activeDepartmentService.GetActiveDepartmentIdAsync(userId);

            // Assert
            result.Should().Be(departmentId);
            _cacheServiceMock.Verify(x => x.SetAsync(cacheKey, It.IsAny<ActiveDepartmentCacheModel>(), CacheHelper.Expiration.User), Times.Once);
        }

        [Fact]
        public async Task GetActiveDepartmentIdAsync_With_User_Without_ActiveDepartment_Should_Return_Null()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
            var user = CreateMockUser(userId, null);

            _cacheServiceMock
                .Setup(x => x.GetAsync<ActiveDepartmentCacheModel>(cacheKey))
                .ReturnsAsync((ActiveDepartmentCacheModel)null);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _activeDepartmentService.GetActiveDepartmentIdAsync(userId);

            // Assert
            result.Should().BeNull();
            _cacheServiceMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task GetActiveDepartmentIdAsync_With_Non_Existent_User_Should_Return_Null()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);

            _cacheServiceMock
                .Setup(x => x.GetAsync<ActiveDepartmentCacheModel>(cacheKey))
                .ReturnsAsync((ActiveDepartmentCacheModel)null);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _activeDepartmentService.GetActiveDepartmentIdAsync(userId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetActiveDepartmentIdAsync_With_Valid_User_Should_Update_Database_And_Cache()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
            var user = CreateMockUser(userId, null);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _cacheServiceMock
                .Setup(x => x.SetAsync(cacheKey, It.IsAny<ActiveDepartmentCacheModel>(), CacheHelper.Expiration.User))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _activeDepartmentService.SetActiveDepartmentIdAsync(userId, departmentId);

            // Assert
            result.Should().BeTrue();
            user.ActiveDepartmentId.Should().Be(departmentId);
            _cacheServiceMock.Verify(x => x.SetAsync(cacheKey, It.IsAny<ActiveDepartmentCacheModel>(), CacheHelper.Expiration.User), Times.Once);
        }

        [Fact]
        public async Task SetActiveDepartmentIdAsync_With_Non_Existent_User_Should_Return_False()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _activeDepartmentService.SetActiveDepartmentIdAsync(userId, departmentId);

            // Assert
            result.Should().BeFalse();
            _cacheServiceMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task SetActiveDepartmentIdAsync_With_Exception_Should_Return_False()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _activeDepartmentService.SetActiveDepartmentIdAsync(userId, departmentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveActiveDepartmentIdAsync_Should_Remove_From_Cache_And_Update_Database()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
            var user = CreateMockUser(userId, departmentId);

            _cacheServiceMock
                .Setup(x => x.RemoveAsync(cacheKey))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _activeDepartmentService.RemoveActiveDepartmentIdAsync(userId);

            // Assert
            result.Should().BeTrue();
            user.ActiveDepartmentId.Should().BeNull();
            _cacheServiceMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once);
        }

        [Fact]
        public async Task RemoveActiveDepartmentIdAsync_With_Non_Existent_User_Should_Still_Remove_From_Cache()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);

            _cacheServiceMock
                .Setup(x => x.RemoveAsync(cacheKey))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _activeDepartmentService.RemoveActiveDepartmentIdAsync(userId);

            // Assert
            result.Should().BeTrue();
            _cacheServiceMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once);
        }

        [Fact]
        public async Task RemoveActiveDepartmentIdAsync_With_Exception_Should_Return_False()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _cacheServiceMock
                .Setup(x => x.RemoveAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Cache error"));

            // Act
            var result = await _activeDepartmentService.RemoveActiveDepartmentIdAsync(userId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task HasActiveDepartmentAsync_Should_Check_Cache_Existence()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);

            _cacheServiceMock
                .Setup(x => x.ExistsAsync(cacheKey))
                .ReturnsAsync(true);

            // Act
            var result = await _activeDepartmentService.HasActiveDepartmentAsync(userId);

            // Assert
            result.Should().BeTrue();
            _cacheServiceMock.Verify(x => x.ExistsAsync(cacheKey), Times.Once);
        }

        [Fact]
        public async Task HasActiveDepartmentAsync_With_Non_Existent_Cache_Should_Return_False()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);

            _cacheServiceMock
                .Setup(x => x.ExistsAsync(cacheKey))
                .ReturnsAsync(false);

            // Act
            var result = await _activeDepartmentService.HasActiveDepartmentAsync(userId);

            // Assert
            result.Should().BeFalse();
        }

        private User CreateMockUser(Guid userId, Guid? activeDepartmentId)
        {
            var user = new Mock<User>();
            user.Setup(x => x.Id).Returns(userId);
            user.SetupProperty(x => x.ActiveDepartmentId, activeDepartmentId);
            user.Setup(x => x.SetActiveDepartment(It.IsAny<Guid>()))
                .Returns(MessageDTO.Success("Success", "Department set successfully"));
            return user.Object;
        }
    }
}

