using Domain.Entities;
using Domain.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Entities
{
    public class RoleEntityChangeTests
    {
        [Fact]
        public void Role_ShouldHaveNameProperty()
        {
            // Arrange
            var name = EntityName.Create("Admin").Data;

            // Act
            var role = Role.CreateRole(name, "Administrator role").Data;

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().NotBeNull();
            role.Name.Value.Should().Be("Admin");
        }

        [Fact]
        public void Role_ShouldHaveDescriptionProperty()
        {
            // Arrange
            var name = EntityName.Create("Admin").Data;
            var description = "Administrator role";

            // Act
            var role = Role.CreateRole(name, description).Data;

            // Assert
            role.Should().NotBeNull();
            role.Description.Should().Be(description);
        }

        [Fact]
        public void Role_ShouldHaveCorrectStructure()
        {
            // Arrange
            var name = EntityName.Create("Admin").Data;

            // Act
            var role = Role.CreateRole(name, "Administrator role").Data;

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().NotBeNull();
            role.Description.Should().NotBeNull();
        }

        [Fact]
        public void Role_CreateRole_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var name = EntityName.Create("Manager").Data;
            var description = "Manager role";

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be("Manager");
            result.Data.Description.Should().Be(description);
        }

        [Fact]
        public void Role_CreateRole_WithNullName_ShouldReturnFailure()
        {
            // Arrange
            EntityName name = null;
            var description = "Manager role";

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("name of role cannot be empty");
        }

        [Fact]
        public void Role_CreateRole_WithEmptyDescription_ShouldReturnSuccess()
        {
            // Arrange
            var name = EntityName.Create("Manager").Data;
            var description = "";

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Description.Should().Be(description);
        }
    }
}
