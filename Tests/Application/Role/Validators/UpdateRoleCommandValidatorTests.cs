using Application.Features.RoleUseCase.Commands;
using Application.Features.RoleUseCase.Validators;
using Domain.Common.ValueObjects;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Application.Role.Validators
{
    public class UpdateRoleCommandValidatorTests
    {
        private readonly UpdateRoleCommandValidator _validator;

        public UpdateRoleCommandValidatorTests()
        {
            _validator = new UpdateRoleCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var name = EntityName.Create("UpdatedAdmin").Data;
            var command = new UpdateRoleCommand
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = "Updated administrator role"
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
            var name = EntityName.Create("Admin").Data;
            var command = new UpdateRoleCommand
            {
                Id = Guid.Empty,
                Name = name,
                Description = "Description"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Role ID is required.");
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_Name_Is_Null()
        {
            // Arrange
            var command = new UpdateRoleCommand
            {
                Id = Guid.NewGuid(),
                Name = null,
                Description = "Description"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Role name is required.");
        }
    }
}

