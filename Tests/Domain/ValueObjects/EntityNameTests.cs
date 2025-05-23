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
            var entityName = new EntityName(validName);

            // Assert
            entityName.Value.Should().Be("Test Entity");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullName_ShouldThrowArgumentException(string invalidName)
        {
            // Act & Assert
            var action = () => new EntityName(invalidName);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Name content is required.");
        }

        [Fact]
        public void Create_WithTooLongName_ShouldThrowArgumentException()
        {
            // Arrange
            var tooLongName = new string('a', 101); // 101 characters

            // Act & Assert
            var action = () => new EntityName(tooLongName);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Name Content is too long");
        }

        [Fact]
        public void Create_WithMaxLengthName_ShouldCreateEntityName()
        {
            // Arrange
            var maxLengthName = new string('a', 100); // 100 characters

            // Act
            var entityName = new EntityName(maxLengthName);

            // Assert
            entityName.Value.Should().Be(maxLengthName);
        }

        [Fact]
        public void Equals_WithSameName_ShouldReturnTrue()
        {
            // Arrange
            var name1 = new EntityName("Test Entity");
            var name2 = new EntityName("Test Entity");

            // Act & Assert
            name1.Should().Be(name2);
            (name1 == name2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentName_ShouldReturnFalse()
        {
            // Arrange
            var name1 = new EntityName("Test Entity 1");
            var name2 = new EntityName("Test Entity 2");

            // Act & Assert
            name1.Should().NotBe(name2);
            (name1 != name2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var name = new EntityName("Test Entity");

            // Act & Assert
            name.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameName_ShouldReturnSameHashCode()
        {
            // Arrange
            var name1 = new EntityName("Test Entity");
            var name2 = new EntityName("Test Entity");

            // Act & Assert
            name1.GetHashCode().Should().Be(name2.GetHashCode());
        }
    }
} 