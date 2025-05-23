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
            var fullName = PersonFullName.Create(firstName, lastName).Data;

            // Assert
            fullName.FirstName.Should().Be("John");
            fullName.LastName.Should().Be("Doe");
        }

        [Theory]
        [InlineData("", "Doe")]
        [InlineData(" ", "Doe")]
        [InlineData(null, "Doe")]
        public void Create_WithEmptyOrNullFirstName_ShouldReturnFailure(string firstName, string lastName)
        {
            // Act & Assert

            var result = PersonFullName.Create(firstName, lastName);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Firstname is required.");
            result.Message.Should().Be("Please fix the name input."); 
        }

        [Theory]
        [InlineData("John", "")]
        [InlineData("John", " ")]
        [InlineData("John", null)]
        public void Create_WithEmptyOrNullLastName_ShouldReturnFailure(string firstName, string lastName)
        {
            // Act & Assert

            var result = PersonFullName.Create(firstName, lastName);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Lastname is required.");
            result.Message.Should().Be("Please fix the name input."); 
        }

        [Fact]
        public void Equals_WithSameNames_ShouldReturnTrue()
        {
            // Arrange
            var fullName1 = PersonFullName.Create("John", "Doe").Data;
            var fullName2 = PersonFullName.Create("John", "Doe").Data;

            // Act & Assert
            fullName1.Should().Be(fullName2);
            (fullName1 == fullName2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentNames_ShouldReturnFalse()
        {
            // Arrange
            var fullName1 = PersonFullName.Create("John", "Doe").Data;
            var fullName2 = PersonFullName.Create("Jane", "Doe").Data;

            // Act & Assert
            fullName1.Should().NotBe(fullName2);
            (fullName1 != fullName2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var fullName1 = PersonFullName.Create("John", "Doe").Data;
            var fullName2 = PersonFullName.Create("JOHN", "DOE").Data;

            // Act & Assert
            fullName1.Should().Be(fullName2);
            (fullName1 == fullName2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var fullName = PersonFullName.Create("John", "Doe").Data;

            // Act & Assert
            fullName.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameNames_ShouldReturnSameHashCode()
        {
            // Arrange
            var fullName1 = PersonFullName.Create("John", "Doe").Data;
            var fullName2 = PersonFullName.Create("John", "Doe").Data;

            // Act & Assert
            fullName1.GetHashCode().Should().Be(fullName2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WithDifferentCase_ShouldReturnSameHashCode()
        {
            // Arrange
            var fullName1 = PersonFullName.Create("John", "Doe").Data;
            var fullName2 = PersonFullName.Create("JOHN", "DOE").Data;

            // Act & Assert
            fullName1.GetHashCode().Should().Be(fullName2.GetHashCode());
        }
    }
} 