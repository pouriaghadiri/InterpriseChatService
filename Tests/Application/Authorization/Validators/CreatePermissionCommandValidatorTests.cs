using Application.Features.AuthorizationUseCase.Commands;
using Application.Features.AuthorizationUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Authorization.Validators
{
    public class CreatePermissionCommandValidatorTests
    {
        private readonly CreatePermissionCommandValidator _validator;

        public CreatePermissionCommandValidatorTests()
        {
            _validator = new CreatePermissionCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new CreatePermissionCommand
            {
                Name = "CreateUser",
                Description = "Permission to create users"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Description", "Permission name is required.")]
        [InlineData(null, "Description", "Permission name is required.")]
        public void Validate_Should_Return_Invalid_When_Name_Is_Empty(string name, string description, string expectedError)
        {
            // Arrange
            var command = new CreatePermissionCommand
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
            var command = new CreatePermissionCommand
            {
                Name = new string('A', 101), // 101 characters
                Description = "Description"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage.Contains("must not exceed 100 characters"));
        }

        [Theory]
        [InlineData("", "Permission description is required.")]
        [InlineData(null, "Permission description is required.")]
        public void Validate_Should_Return_Invalid_When_Description_Is_Empty(string description, string expectedError)
        {
            // Arrange
            var command = new CreatePermissionCommand
            {
                Name = "CreateUser",
                Description = description
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
}

