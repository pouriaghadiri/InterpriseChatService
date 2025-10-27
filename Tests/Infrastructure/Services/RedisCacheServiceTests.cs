using Domain.Services;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text.Json;
using Xunit;

namespace Tests.UnitTest.Infrastructure.Services
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly RedisCacheService _cacheService;

        public RedisCacheServiceTests()
        {
            _distributedCacheMock = new Mock<IDistributedCache>();
            _cacheService = new RedisCacheService(_distributedCacheMock.Object);
        }

        [Fact]
        public async Task GetAsync_With_Valid_Key_Should_Return_Deserialized_Object()
        {
            // Arrange
            var key = "test-key";
            var testObject = new TestModel { Id = 1, Name = "Test" };
            var serializedObject = JsonSerializer.Serialize(testObject, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            _distributedCacheMock
                .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serializedObject);

            // Act
            var result = await _cacheService.GetAsync<TestModel>(key);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Test");
        }

        [Fact]
        public async Task GetAsync_With_Non_Existent_Key_Should_Return_Null()
        {
            // Arrange
            var key = "non-existent-key";
            _distributedCacheMock
                .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            // Act
            var result = await _cacheService.GetAsync<TestModel>(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetAsync_With_Value_Should_Serialize_And_Store()
        {
            // Arrange
            var key = "test-key";
            var testObject = new TestModel { Id = 1, Name = "Test" };
            var expectedJson = JsonSerializer.Serialize(testObject, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            _distributedCacheMock
                .Setup(x => x.SetStringAsync(key, It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.SetAsync(key, testObject);

            // Assert
            _distributedCacheMock.Verify(
                x => x.SetStringAsync(key, expectedJson, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SetAsync_With_Expiration_Should_Set_Absolute_Expiration()
        {
            // Arrange
            var key = "test-key";
            var testObject = new TestModel { Id = 1, Name = "Test" };
            var expiration = TimeSpan.FromMinutes(10);

            _distributedCacheMock
                .Setup(x => x.SetStringAsync(key, It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.SetAsync(key, testObject, expiration);

            // Assert
            _distributedCacheMock.Verify(
                x => x.SetStringAsync(key, It.IsAny<string>(), 
                    It.Is<DistributedCacheEntryOptions>(opt => opt.AbsoluteExpiration.HasValue), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SetAsync_Without_Expiration_Should_Set_Sliding_Expiration()
        {
            // Arrange
            var key = "test-key";
            var testObject = new TestModel { Id = 1, Name = "Test" };

            _distributedCacheMock
                .Setup(x => x.SetStringAsync(key, It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.SetAsync(key, testObject);

            // Assert
            _distributedCacheMock.Verify(
                x => x.SetStringAsync(key, It.IsAny<string>(), 
                    It.Is<DistributedCacheEntryOptions>(opt => opt.SlidingExpiration.HasValue), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_Should_Call_DistributedCache_Remove()
        {
            // Arrange
            var key = "test-key";
            _distributedCacheMock
                .Setup(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.RemoveAsync(key);

            // Assert
            _distributedCacheMock.Verify(
                x => x.RemoveAsync(key, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_With_Existing_Key_Should_Return_True()
        {
            // Arrange
            var key = "existing-key";
            _distributedCacheMock
                .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync("some-value");

            // Act
            var result = await _cacheService.ExistsAsync(key);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_With_Non_Existent_Key_Should_Return_False()
        {
            // Arrange
            var key = "non-existent-key";
            _distributedCacheMock
                .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            // Act
            var result = await _cacheService.ExistsAsync(key);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetOrSetAsync_With_Cached_Value_Should_Return_Cached_Value()
        {
            // Arrange
            var key = "test-key";
            var cachedObject = new TestModel { Id = 1, Name = "Cached" };
            var serializedObject = JsonSerializer.Serialize(cachedObject, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            _distributedCacheMock
                .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serializedObject);

            // Act
            var result = await _cacheService.GetOrSetAsync(key, () => Task.FromResult(new TestModel { Id = 2, Name = "New" }));

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Cached");
        }

        [Fact]
        public async Task GetOrSetAsync_Without_Cached_Value_Should_Execute_Factory_And_Cache_Result()
        {
            // Arrange
            var key = "test-key";
            var factoryObject = new TestModel { Id = 2, Name = "Factory" };
            var expectedJson = JsonSerializer.Serialize(factoryObject, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            _distributedCacheMock
                .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);
            
            _distributedCacheMock
                .Setup(x => x.SetStringAsync(key, It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cacheService.GetOrSetAsync(key, () => Task.FromResult(factoryObject));

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(2);
            result.Name.Should().Be("Factory");
            
            _distributedCacheMock.Verify(
                x => x.SetStringAsync(key, expectedJson, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetManyAsync_Should_Return_Dictionary_With_Existing_Keys()
        {
            // Arrange
            var keys = new[] { "key1", "key2", "key3" };
            var object1 = new TestModel { Id = 1, Name = "Test1" };
            var object2 = new TestModel { Id = 2, Name = "Test2" };
            
            _distributedCacheMock
                .Setup(x => x.GetStringAsync("key1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonSerializer.Serialize(object1, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            
            _distributedCacheMock
                .Setup(x => x.GetStringAsync("key2", It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);
            
            _distributedCacheMock
                .Setup(x => x.GetStringAsync("key3", It.IsAny<CancellationToken>()))
                .ReturnsAsync(JsonSerializer.Serialize(object2, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

            // Act
            var result = await _cacheService.GetManyAsync<TestModel>(keys);

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainKey("key1");
            result.Should().ContainKey("key3");
            result["key1"].Id.Should().Be(1);
            result["key3"].Id.Should().Be(2);
        }

        [Fact]
        public async Task SetManyAsync_Should_Set_All_Values()
        {
            // Arrange
            var values = new Dictionary<string, TestModel>
            {
                ["key1"] = new TestModel { Id = 1, Name = "Test1" },
                ["key2"] = new TestModel { Id = 2, Name = "Test2" }
            };

            _distributedCacheMock
                .Setup(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.SetManyAsync(values);

            // Assert
            _distributedCacheMock.Verify(
                x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task RemoveByPatternAsync_Should_Throw_NotImplementedException()
        {
            // Arrange
            var pattern = "test-pattern*";

            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() => _cacheService.RemoveByPatternAsync(pattern));
        }

        [Fact]
        public async Task GetAsync_With_Invalid_Json_Should_Throw_JsonException()
        {
            // Arrange
            var key = "invalid-json-key";
            _distributedCacheMock
                .Setup(x => x.GetStringAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync("invalid-json");

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => _cacheService.GetAsync<TestModel>(key));
        }

        // Helper class for testing
        private class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}

