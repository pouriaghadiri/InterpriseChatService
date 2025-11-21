using Application.Features.DepartmentUseCase.Commands;
using Application.Features.DepartmentUseCase.Validators;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Application.Department.Validators
{
    public class DeleteDepartmentCommandValidatorTests
    {
        private readonly DeleteDepartmentCommandValidator _validator;

        public DeleteDepartmentCommandValidatorTests()
        {
            _validator = new DeleteDepartmentCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_Id_Is_Valid()
        {
            // Arrange
            var command = new DeleteDepartmentCommand(Guid.NewGuid());

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_Id_Is_Empty()
        {
            // Arrange
            var command = new DeleteDepartmentCommand(Guid.Empty);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Department ID is required.");
        }
    }
}

