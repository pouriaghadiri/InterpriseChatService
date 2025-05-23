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
            var email = Email.Create(validEmail).Data;

            // Assert
            email.Value.Should().Be("test@example.com");
        }

        [Fact]
        public void Create_WithValidEmailUpperCase_ShouldCreateLowerCaseEmail()
        {
            // Arrange
            var validEmail = "TEST@EXAMPLE.COM";

            // Act
            var email = Email.Create(validEmail).Data;

            // Assert
            email.Value.Should().Be("test@example.com");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullEmail_ShouldReturnFailiure(string invalidEmail)
        {
            // Act & Assert
            var result = Email.Create(invalidEmail);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Email is required.");
            result.Message.Should().Be("Please fix the email input.");
        }

        [Theory]
        [InlineData("notanemail")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("test@.com")]
        [InlineData("test.com")]
        public void Create_WithInvalidEmailFormat_ShouldReturnFailiure(string invalidEmail)
        {
            // Act & Assert

            var result = Email.Create(invalidEmail);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Invalid email format.");
            result.Message.Should().Be("Please fix the email input.");
             
        }

        [Fact]
        public void Equals_WithSameEmail_ShouldReturnTrue()
        {
            // Arrange
            var email1 = Email.Create("test@example.com").Data;
            var email2 = Email.Create("test@example.com").Data;

            // Act & Assert
            email1.Should().Be(email2);
            (email1 == email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentEmail_ShouldReturnFalse()
        {
            // Arrange
            var email1 = Email.Create("test1@example.com").Data;
            var email2 = Email.Create("test2@example.com").Data;

            // Act & Assert
            email1.Should().NotBe(email2);
            (email1 != email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var email = Email.Create("test@example.com").Data;

            // Act & Assert
            email.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameEmail_ShouldReturnSameHashCode()
        {
            // Arrange
            var email1 = Email.Create("test@example.com").Data;
            var email2 = Email.Create("test@example.com").Data;

            // Act & Assert
            email1.GetHashCode().Should().Be(email2.GetHashCode());
        }
    }
} 