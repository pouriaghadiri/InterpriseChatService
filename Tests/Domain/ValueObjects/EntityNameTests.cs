using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class EntityNameTests
    {
        [Fact]
        public void Create_WithValidName_ShouldCreateEntityName()
        {
            // Arrange
            var validName = "Test Entity";

            // Act
            var entityName = EntityName.Create(validName).Data;

            // Assert
            entityName.Value.Should().Be("Test Entity");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullName_ShouldReturnFailure(string invalidName)
        {
            // Act & Assert
            var result = EntityName.Create(invalidName);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Name content is required.");
            result.Message.Should().Be("Please fix the name input.");
             
        }

        [Fact]
        public void Create_WithTooLongName_ShouldReturnFailure()
        {
            // Arrange
            var tooLongName = new string('a', 101); // 101 characters

            // Act & Assert
            var result = EntityName.Create(tooLongName);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Name Content is too long");
            result.Message.Should().Be("Please fix the name input.");
             
        }

        [Fact]
        public void Create_WithMaxLengthName_ShouldCreateEntityName()
        {
            // Arrange
            var maxLengthName = new string('a', 100); // 100 characters

            // Act
            var entityName = EntityName.Create(maxLengthName).Data;

            // Assert
            entityName.Value.Should().Be(maxLengthName);
        }

        [Fact]
        public void Equals_WithSameName_ShouldReturnTrue()
        {
            // Arrange
            var name1 = EntityName.Create("Test Entity").Data;
            var name2 = EntityName.Create("Test Entity").Data;

            // Act & Assert
            name1.Should().Be(name2);
            (name1 == name2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentName_ShouldReturnFalse()
        {
            // Arrange
            var name1 = EntityName.Create("Test Entity 1").Data;
            var name2 = EntityName.Create("Test Entity 2").Data;

            // Act & Assert
            name1.Should().NotBe(name2);
            (name1 != name2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var name = EntityName.Create("Test Entity").Data;

            // Act & Assert
            name.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameName_ShouldReturnSameHashCode()
        {
            // Arrange
            var name1 = EntityName.Create("Test Entity").Data;
            var name2 = EntityName.Create("Test Entity").Data;

            // Act & Assert
            name1.GetHashCode().Should().Be(name2.GetHashCode());
        }
    }
} 