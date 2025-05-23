using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_WithValidEmail_ShouldCreateEmail()
        {
            // Arrange
            var validEmail = "test@example.com";

            // Act
            var email = new Email(validEmail);

            // Assert
            email.Value.Should().Be("test@example.com");
        }

        [Fact]
        public void Create_WithValidEmailUpperCase_ShouldCreateLowerCaseEmail()
        {
            // Arrange
            var validEmail = "TEST@EXAMPLE.COM";

            // Act
            var email = new Email(validEmail);

            // Assert
            email.Value.Should().Be("test@example.com");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            var action = () => new Email(invalidEmail);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Email is required.");
        }

        [Theory]
        [InlineData("notanemail")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("test@.com")]
        [InlineData("test.com")]
        public void Create_WithInvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            var action = () => new Email(invalidEmail);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Invalid email format.");
        }

        [Fact]
        public void Equals_WithSameEmail_ShouldReturnTrue()
        {
            // Arrange
            var email1 = new Email("test@example.com");
            var email2 = new Email("test@example.com");

            // Act & Assert
            email1.Should().Be(email2);
            (email1 == email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentEmail_ShouldReturnFalse()
        {
            // Arrange
            var email1 = new Email("test1@example.com");
            var email2 = new Email("test2@example.com");

            // Act & Assert
            email1.Should().NotBe(email2);
            (email1 != email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var email = new Email("test@example.com");

            // Act & Assert
            email.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameEmail_ShouldReturnSameHashCode()
        {
            // Arrange
            var email1 = new Email("test@example.com");
            var email2 = new Email("test@example.com");

            // Act & Assert
            email1.GetHashCode().Should().Be(email2.GetHashCode());
        }
    }
} 