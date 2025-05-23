using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class PersonFullNameTests
    {
        [Fact]
        public void Create_WithValidNames_ShouldCreatePersonFullName()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";

            // Act
            var fullName = new PersonFullName(firstName, lastName);

            // Assert
            fullName.FirstName.Should().Be("John");
            fullName.LastName.Should().Be("Doe");
        }

        [Theory]
        [InlineData("", "Doe")]
        [InlineData(" ", "Doe")]
        [InlineData(null, "Doe")]
        public void Create_WithEmptyOrNullFirstName_ShouldThrowArgumentException(string firstName, string lastName)
        {
            // Act & Assert
            var action = () => new PersonFullName(firstName, lastName);
            action.Should().Throw<ArgumentException>()
                .WithMessage("First name is required.");
        }

        [Theory]
        [InlineData("John", "")]
        [InlineData("John", " ")]
        [InlineData("John", null)]
        public void Create_WithEmptyOrNullLastName_ShouldThrowArgumentException(string firstName, string lastName)
        {
            // Act & Assert
            var action = () => new PersonFullName(firstName, lastName);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Last name is required.");
        }

        [Fact]
        public void Equals_WithSameNames_ShouldReturnTrue()
        {
            // Arrange
            var fullName1 = new PersonFullName("John", "Doe");
            var fullName2 = new PersonFullName("John", "Doe");

            // Act & Assert
            fullName1.Should().Be(fullName2);
            (fullName1 == fullName2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentNames_ShouldReturnFalse()
        {
            // Arrange
            var fullName1 = new PersonFullName("John", "Doe");
            var fullName2 = new PersonFullName("Jane", "Doe");

            // Act & Assert
            fullName1.Should().NotBe(fullName2);
            (fullName1 != fullName2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var fullName1 = new PersonFullName("John", "Doe");
            var fullName2 = new PersonFullName("JOHN", "DOE");

            // Act & Assert
            fullName1.Should().Be(fullName2);
            (fullName1 == fullName2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var fullName = new PersonFullName("John", "Doe");

            // Act & Assert
            fullName.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameNames_ShouldReturnSameHashCode()
        {
            // Arrange
            var fullName1 = new PersonFullName("John", "Doe");
            var fullName2 = new PersonFullName("John", "Doe");

            // Act & Assert
            fullName1.GetHashCode().Should().Be(fullName2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WithDifferentCase_ShouldReturnSameHashCode()
        {
            // Arrange
            var fullName1 = new PersonFullName("John", "Doe");
            var fullName2 = new PersonFullName("JOHN", "DOE");

            // Act & Assert
            fullName1.GetHashCode().Should().Be(fullName2.GetHashCode());
        }
    }
} 