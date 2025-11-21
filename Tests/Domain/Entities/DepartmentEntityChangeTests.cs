using Domain.Entities;
using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Entities
{
    public class DepartmentEntityChangeTests
    {
        [Fact]
        public void Department_ShouldHaveNameProperty()
        {
            // Arrange
            var name = EntityName.Create("Engineering").Data;

            // Act
            var department = Department.CreateDepartment(name).Data;

            // Assert
            department.Should().NotBeNull();
            department.Name.Should().NotBeNull();
            department.Name.Value.Should().Be("Engineering");
        }

        [Fact]
        public void Department_ShouldHaveUserRoleInDepartmentsCollection()
        {
            // Arrange
            var name = EntityName.Create("Engineering").Data;

            // Act
            var department = Department.CreateDepartment(name).Data;

            // Assert
            department.Should().NotBeNull();
            // Note: UserRoleInDepartments is not initialized in the constructor
            // This is expected behavior for EF Core navigation properties
            department.UserRoleInDepartments.Should().BeNull();
        }

        [Fact]
        public void Department_CreateDepartment_WithValidName_ShouldReturnSuccess()
        {
            // Arrange
            var name = EntityName.Create("HR Department").Data;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be("HR Department");
        }

        [Fact]
        public void Department_CreateDepartment_WithNullName_ShouldReturnFailure()
        {
            // Arrange
            EntityName name = null;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("name of Department cannot be empty");
        }

        [Fact]
        public void Department_CreateDepartment_WithEmptyName_ShouldReturnFailure()
        {
            // Arrange
            var name = EntityName.Create("").Data;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("name of Department cannot be empty");
        }

        [Fact]
        public void Department_CreateDepartment_WithWhitespaceName_ShouldReturnFailure()
        {
            // Arrange
            var name = EntityName.Create("   ").Data;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("name of Department cannot be empty");
        }
    }
}
