using Application.Features.DepartmentUseCase.Commands;
using Application.Features.DepartmentUseCase.Validators;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Application.Department.Validators
{
    public class UpdateDepartmentCommandValidatorTests
    {
        private readonly UpdateDepartmentCommandValidator _validator;

        public UpdateDepartmentCommandValidatorTests()
        {
            _validator = new UpdateDepartmentCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
                Name = "UpdatedIT"
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
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.Empty,
                Name = "IT"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Department ID is required.");
        }

        [Theory]
        [InlineData("", "Department name is required.")]
        [InlineData(null, "Department name is required.")]
        public void Validate_Should_Return_Invalid_When_Name_Is_Empty(string name, string expectedError)
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedError);
        }
    }
}

