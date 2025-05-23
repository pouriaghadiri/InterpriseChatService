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
            var messageContent = new MessageContent(validContent);

            // Assert
            messageContent.Value.Should().Be("Test message content");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullContent_ShouldThrowArgumentException(string invalidContent)
        {
            // Act & Assert
            var action = () => new MessageContent(invalidContent);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Message content is required.");
        }

        [Fact]
        public void Create_WithTooLongContent_ShouldThrowArgumentException()
        {
            // Arrange
            var tooLongContent = new string('a', 1501); // 1501 characters

            // Act & Assert
            var action = () => new MessageContent(tooLongContent);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Message Content is too long");
        }

        [Fact]
        public void Create_WithMaxLengthContent_ShouldCreateMessageContent()
        {
            // Arrange
            var maxLengthContent = new string('a', 1500); // 1500 characters

            // Act
            var messageContent = new MessageContent(maxLengthContent);

            // Assert
            messageContent.Value.Should().Be(maxLengthContent);
        }

        [Fact]
        public void Equals_WithSameContent_ShouldReturnTrue()
        {
            // Arrange
            var content1 = new MessageContent("Test message");
            var content2 = new MessageContent("Test message");

            // Act & Assert
            content1.Should().Be(content2);
            (content1 == content2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentContent_ShouldReturnFalse()
        {
            // Arrange
            var content1 = new MessageContent("Test message 1");
            var content2 = new MessageContent("Test message 2");

            // Act & Assert
            content1.Should().NotBe(content2);
            (content1 != content2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var content = new MessageContent("Test message");

            // Act & Assert
            content.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameContent_ShouldReturnSameHashCode()
        {
            // Arrange
            var content1 = new MessageContent("Test message");
            var content2 = new MessageContent("Test message");

            // Act & Assert
            content1.GetHashCode().Should().Be(content2.GetHashCode());
        }
    }
} 