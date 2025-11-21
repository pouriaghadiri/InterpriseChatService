using Application.Features.RoleUseCase.Commands;
using Application.Features.RoleUseCase.Validators;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Application.Role.Validators
{
    public class DeleteRoleCommandValidatorTests
    {
        private readonly DeleteRoleCommandValidator _validator;

        public DeleteRoleCommandValidatorTests()
        {
            _validator = new DeleteRoleCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_Id_Is_Valid()
        {
            // Arrange
            var command = new DeleteRoleCommand(Guid.NewGuid());

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_Id_Is_Empty()
        {
            // Arrange
            var command = new DeleteRoleCommand(Guid.Empty);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Role ID is required.");
        }
    }
}

