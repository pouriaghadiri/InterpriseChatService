using Application.Features.RoleUseCase.Commands;
using Application.Features.RoleUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Role.Validators
{
    public class AddRoleCommandValidatorTests
    {
        private readonly AddRoleCommandValidator _validator;

        public AddRoleCommandValidatorTests()
        {
            _validator = new AddRoleCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new AddRoleCommand
            {
                Name = "Admin",
                Description = "Administrator role"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Description", "Role name is required.")]
        [InlineData(null, "Description", "Role name is required.")]
        public void Validate_Should_Return_Invalid_When_Name_Is_Empty(string name, string description, string expectedError)
        {
            // Arrange
            var command = new AddRoleCommand
            {
                Name = name,
                Description = description
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_Name_Is_Too_Long()
        {
            // Arrange
            var command = new AddRoleCommand
            {
                Name = new string('A', 51), // 51 characters
                Description = "Description"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage.Contains("must not exceed 50 characters"));
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_Description_Is_Optional()
        {
            // Arrange
            var command = new AddRoleCommand
            {
                Name = "Admin",
                Description = null
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}

