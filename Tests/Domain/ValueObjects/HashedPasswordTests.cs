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
            var hashedPassword = HashedPassword.Create(plainPassword).Data;

            // Assert
            hashedPassword.Hash.Should().NotBeNullOrEmpty();
            hashedPassword.Salt.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullPassword_ShouldReturnFailure(string invalidPassword)
        {
            // Act & Assert

            var result = HashedPassword.Create(invalidPassword);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Password is required.");
            result.Message.Should().Be("Please fix the password input."); 
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
            var hashedPassword = HashedPassword.Create(plainPassword).Data;

            // Act
            var result = hashedPassword.Verify(plainPassword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var hashedPassword = HashedPassword.Create("CorrectPassword123!").Data;

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
            var hashedPassword1 = HashedPassword.Create(plainPassword).Data;
            var hashedPassword2 = HashedPassword.Create(plainPassword).Data;

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
            var hashedPassword1 = HashedPassword.Create("Password1").Data;
            var hashedPassword2 = HashedPassword.Create("Password2").Data;

            // Act & Assert
            hashedPassword1.Should().NotBe(hashedPassword2);
            (hashedPassword1 != hashedPassword2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var hashedPassword = HashedPassword.Create("TestPassword123!").Data;

            // Act & Assert
            hashedPassword.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSamePassword_ShouldReturnSameHashCode()
        {
            // Arrange
            var plainPassword = "TestPassword123!";
            var hashedPassword1 = HashedPassword.Create(plainPassword).Data;
            var hashedPassword2 = HashedPassword.Create(plainPassword).Data;

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