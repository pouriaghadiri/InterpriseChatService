using Application.Features.AuthorizationUseCase.Commands;
using Application.Features.AuthorizationUseCase.Validators;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Application.Authorization.Validators
{
    public class UpdatePermissionCommandValidatorTests
    {
        private readonly UpdatePermissionCommandValidator _validator;

        public UpdatePermissionCommandValidatorTests()
        {
            _validator = new UpdatePermissionCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new UpdatePermissionCommand
            {
                Id = Guid.NewGuid(),
                Name = "UpdateUser",
                Description = "Permission to update users"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_Id_Is_Empty()
        {
            // Arrange
            var command = new UpdatePermissionCommand
            {
                Id = Guid.Empty,
                Name = "UpdateUser",
                Description = "Description"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Permission ID is required.");
        }

        [Theory]
        [InlineData("", "Description", "Permission name is required.")]
        [InlineData(null, "Description", "Permission name is required.")]
        public void Validate_Should_Return_Invalid_When_Name_Is_Empty(string name, string description, string expectedError)
        {
            // Arrange
            var command = new UpdatePermissionCommand
            {
                Id = Guid.NewGuid(),
                Name = name,
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

