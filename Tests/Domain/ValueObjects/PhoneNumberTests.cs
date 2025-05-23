using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class PhoneNumberTests
    {
        [Fact]
        public void Create_WithValidPhoneNumber_ShouldReturnSuccess()
        {
            // Arrange
            var validPhone = "09123456789";

            // Act
            var result = PhoneNumber.Create(validPhone);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Value.Should().Be("09123456789");
            result.Message.Should().Be("Phone number created successfully");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullPhoneNumber_ShouldReturnFailure(string invalidPhone)
        {
            // Act
            var result = PhoneNumber.Create(invalidPhone);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Phone number is required.");
            result.Message.Should().Be("Please fix the phone number input.");
        }

        [Theory]
        [InlineData("1234567890")]     // Missing 09 prefix
        [InlineData("091234567")]      // Too short
        [InlineData("0912345678901")]  // Too long
        [InlineData("09abcdefghi")]    // Contains letters
        [InlineData("09 12345678")]    // Contains space
        public void Create_WithInvalidPhoneNumberFormat_ShouldReturnFailure(string invalidPhone)
        {
            // Act
            var result = PhoneNumber.Create(invalidPhone);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Invalid phone number format.");
            result.Message.Should().Be("Please fix the phone number input.");
        }

        [Fact]
        public void Equals_WithSamePhoneNumber_ShouldReturnTrue()
        {
            // Arrange
            var phone1 = PhoneNumber.Create("09123456789").Data;
            var phone2 = PhoneNumber.Create("09123456789").Data;

            // Act & Assert
            phone1.Should().Be(phone2);
            (phone1 == phone2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentPhoneNumber_ShouldReturnFalse()
        {
            // Arrange
            var phone1 = PhoneNumber.Create("09123456789").Data;
            var phone2 = PhoneNumber.Create("09987654321").Data;

            // Act & Assert
            phone1.Should().NotBe(phone2);
            (phone1 != phone2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithUpperCasePhoneNumber_ShouldReturnTrue()
        {
            // Arrange
            var phone1 = PhoneNumber.Create("09123456789").Data;
            var phone2 = PhoneNumber.Create("09123456789").Data;

            // Act & Assert
            phone1.Should().Be(phone2);
            (phone1 == phone2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var phone = PhoneNumber.Create("09123456789").Data;

            // Act & Assert
            phone!.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSamePhoneNumber_ShouldReturnSameHashCode()
        {
            // Arrange
            var phone1 = PhoneNumber.Create("09123456789").Data;
            var phone2 = PhoneNumber.Create("09123456789").Data;

            // Act & Assert
            phone1!.GetHashCode().Should().Be(phone2!.GetHashCode());
        }
    }
} 