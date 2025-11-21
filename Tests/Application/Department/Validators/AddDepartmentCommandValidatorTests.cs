using Application.Features.DepartmentUseCase.Commands;
using Application.Features.DepartmentUseCase.Validators;
using FluentAssertions;
using Xunit;

namespace Tests.Application.Department.Validators
{
    public class AddDepartmentCommandValidatorTests
    {
        private readonly AddDepartmentCommandValidator _validator;

        public AddDepartmentCommandValidatorTests()
        {
            _validator = new AddDepartmentCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_Name_Is_Valid()
        {
            // Arrange
            var command = new AddDepartmentCommand
            {
                Name = "IT"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Department name is required.")]
        [InlineData(null, "Department name is required.")]
        public void Validate_Should_Return_Invalid_When_Name_Is_Empty(string name, string expectedError)
        {
            // Arrange
            var command = new AddDepartmentCommand
            {
                Name = name
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
            var command = new AddDepartmentCommand
            {
                Name = new string('A', 51) // 51 characters
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage.Contains("must not exceed 50 characters"));
        }
    }
}

