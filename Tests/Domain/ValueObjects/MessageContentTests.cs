using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.ValueObjects
{
    public class MessageContentTests
    {
        [Fact]
        public void Create_WithValidContent_ShouldCreateMessageContent()
        {
            // Arrange
            var validContent = "Test message content";

            // Act
            var messageContent = MessageContent.Create(validContent).Data;

            // Assert
            messageContent.Value.Should().Be("Test message content");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullContent_ShouldReturnFailure(string invalidContent)
        {
            // Act & Assert

            var result = MessageContent.Create(invalidContent);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Message content is required.");
            result.Message.Should().Be("Please fix the message input."); 
        }

        [Fact]
        public void Create_WithTooLongContent_ShouldReturnFailure()
        {
            // Arrange
            var tooLongContent = new string('a', 1501); // 1501 characters

            // Act & Assert

            var result = MessageContent.Create(tooLongContent);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().Contain("Message Content is too long");
            result.Message.Should().Be("Please fix the message input."); 
        }

        [Fact]
        public void Create_WithMaxLengthContent_ShouldCreateMessageContent()
        {
            // Arrange
            var maxLengthContent = new string('a', 1500); // 1500 characters

            // Act
            var messageContent = MessageContent.Create(maxLengthContent).Data;

            // Assert
            messageContent.Value.Should().Be(maxLengthContent);
        }

        [Fact]
        public void Equals_WithSameContent_ShouldReturnTrue()
        {
            // Arrange
            var content1 = MessageContent.Create("Test message").Data;
            var content2 = MessageContent.Create("Test message").Data;

            // Act & Assert
            content1.Should().Be(content2);
            (content1 == content2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentContent_ShouldReturnFalse()
        {
            // Arrange
            var content1 = MessageContent.Create("Test message 1").Data;
            var content2 = MessageContent.Create("Test message 2").Data;

            // Act & Assert
            content1.Should().NotBe(content2);
            (content1 != content2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var content = MessageContent.Create("Test message").Data;

            // Act & Assert
            content.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameContent_ShouldReturnSameHashCode()
        {
            // Arrange
            var content1 = MessageContent.Create("Test message").Data;
            var content2 = MessageContent.Create("Test message").Data;

            // Act & Assert
            content1.GetHashCode().Should().Be(content2.GetHashCode());
        }
    }
} 