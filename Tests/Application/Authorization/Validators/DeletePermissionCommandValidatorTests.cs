using Application.Features.AuthorizationUseCase.Commands;
using Application.Features.AuthorizationUseCase.Validators;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Application.Authorization.Validators
{
    public class DeletePermissionCommandValidatorTests
    {
        private readonly DeletePermissionCommandValidator _validator;

        public DeletePermissionCommandValidatorTests()
        {
            _validator = new DeletePermissionCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_Id_Is_Valid()
        {
            // Arrange
            var command = new DeletePermissionCommand(Guid.NewGuid());

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_Id_Is_Empty()
        {
            // Arrange
            var command = new DeletePermissionCommand(Guid.Empty);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Permission ID is required.");
        }
    }
}

