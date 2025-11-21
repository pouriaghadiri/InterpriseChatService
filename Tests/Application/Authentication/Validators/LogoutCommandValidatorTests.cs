using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Authentication.Validators
{
    public class LogoutCommandValidatorTests
    {
        private readonly LogoutCommandValidator _validator;

        public LogoutCommandValidatorTests()
        {
            _validator = new LogoutCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_Command_Is_Empty()
        {
            // Arrange
            var command = new LogoutCommand();

            // Act
            var result = _validator.Validate(command);

            // Assert
            // Logout command doesn't require validation
            result.IsValid.Should().BeTrue();
        }
    }
}

