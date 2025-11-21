using Application.Features.AuthorizationUseCase.Commands;
using Application.Features.AuthorizationUseCase.Validators;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Application.Authorization.Validators
{
    public class AssignPermissionToUserCommandValidatorTests
    {
        private readonly AssignPermissionToUserCommandValidator _validator;

        public AssignPermissionToUserCommandValidatorTests()
        {
            _validator = new AssignPermissionToUserCommandValidator();
        }

        [Fact]
        public void Validate_Should_Return_Valid_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new AssignPermissionToUserCommand
            {
                UserId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid()
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_UserId_Is_Empty()
        {
            // Arrange
            var command = new AssignPermissionToUserCommand
            {
                UserId = Guid.Empty,
                PermissionId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid()
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "User ID is required.");
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_PermissionId_Is_Empty()
        {
            // Arrange
            var command = new AssignPermissionToUserCommand
            {
                UserId = Guid.NewGuid(),
                PermissionId = Guid.Empty,
                DepartmentId = Guid.NewGuid()
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Permission ID is required.");
        }

        [Fact]
        public void Validate_Should_Return_Invalid_When_DepartmentId_Is_Empty()
        {
            // Arrange
            var command = new AssignPermissionToUserCommand
            {
                UserId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                DepartmentId = Guid.Empty
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "Department ID is required.");
        }
    }
}

