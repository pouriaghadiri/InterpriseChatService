using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class HashedPasswordTests
    {
        [Fact]
        public void Create_WithValidPassword_ShouldCreateHashedPassword()
        {
            // Arrange
            var plainPassword = "TestPassword123!";

            // Act
            var hashedPassword = new HashedPassword(plainPassword);

            // Assert
            hashedPassword.Hash.Should().NotBeNullOrEmpty();
            hashedPassword.Salt.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullPassword_ShouldThrowArgumentException(string invalidPassword)
        {
            // Act & Assert
            var action = () => new HashedPassword(invalidPassword);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Password is required.");
        }

        [Fact]
        public void CreateFromPlain_WithValidPassword_ShouldCreateHashedPassword()
        {
            // Arrange
            var plainPassword = "TestPassword123!";

            // Act
            var hashedPassword = HashedPassword.CreateFromPlain(plainPassword);

            // Assert
            hashedPassword.Hash.Should().NotBeNullOrEmpty();
            hashedPassword.Salt.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Verify_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var plainPassword = "TestPassword123!";
            var hashedPassword = new HashedPassword(plainPassword);

            // Act
            var result = hashedPassword.Verify(plainPassword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var hashedPassword = new HashedPassword("CorrectPassword123!");

            // Act
            var result = hashedPassword.Verify("WrongPassword123!");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithSamePassword_ShouldReturnTrue()
        {
            // Arrange
            var plainPassword = "TestPassword123!";
            var hashedPassword1 = new HashedPassword(plainPassword);
            var hashedPassword2 = new HashedPassword(plainPassword);

            // Act & Assert
            // Note: Even with the same plain password, the hash and salt will be different
            // due to random salt generation. Therefore, we need to manually set the same hash and salt
            // to test equality.
            var type = typeof(HashedPassword);
            var hashProp = type.GetProperty("Hash", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var saltProp = type.GetProperty("Salt", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            hashProp.SetValue(hashedPassword2, hashProp.GetValue(hashedPassword1));
            saltProp.SetValue(hashedPassword2, saltProp.GetValue(hashedPassword1));



            hashedPassword1.Should().Be(hashedPassword2);
            (hashedPassword1 == hashedPassword2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentPasswords_ShouldReturnFalse()
        {
            // Arrange
            var hashedPassword1 = new HashedPassword("Password1");
            var hashedPassword2 = new HashedPassword("Password2");

            // Act & Assert
            hashedPassword1.Should().NotBe(hashedPassword2);
            (hashedPassword1 != hashedPassword2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var hashedPassword = new HashedPassword("TestPassword123!");

            // Act & Assert
            hashedPassword.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSamePassword_ShouldReturnSameHashCode()
        {
            // Arrange
            var plainPassword = "TestPassword123!";
            var hashedPassword1 = new HashedPassword(plainPassword);
            var hashedPassword2 = new HashedPassword(plainPassword);

            // Set same hash and salt for testing hash code
            var type = typeof(HashedPassword);
            var hashProp = type.GetProperty("Hash", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var saltProp = type.GetProperty("Salt", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            hashProp.SetValue(hashedPassword2, hashProp.GetValue(hashedPassword1));
            saltProp.SetValue(hashedPassword2, saltProp.GetValue(hashedPassword1));


            // Act & Assert
            hashedPassword1.GetHashCode().Should().Be(hashedPassword2.GetHashCode());
        }
    }
} 