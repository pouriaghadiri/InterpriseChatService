using Domain.Common.ValueObjects;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Entities
{
    public class PermissionTests
    {
        [Fact]
        public void CreatePermission_Should_Return_Success_When_Name_Is_Valid()
        {
            // Arrange
            var name = EntityName.Create("User Management").Data;
            var description = "Permission to manage users";

            // Act
            var result = Permission.CreatePermission(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(name);
            result.Data.Description.Should().Be(description);
            result.Message.Should().Be("New Permission created successfuly.");
        }

        [Fact]
        public void CreatePermission_Should_Return_Success_When_Description_Is_Null()
        {
            // Arrange
            var name = EntityName.Create("User Management").Data;
            string description = null;

            // Act
            var result = Permission.CreatePermission(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(name);
            result.Data.Description.Should().BeNull();
        }

        [Fact]
        public void CreatePermission_Should_Return_Success_When_Description_Is_Empty()
        {
            // Arrange
            var name = EntityName.Create("User Management").Data;
            var description = "";

            // Act
            var result = Permission.CreatePermission(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(name);
            result.Data.Description.Should().Be("");
        }

        [Fact]
        public void CreatePermission_Should_Return_Failure_When_Name_Is_Null()
        {
            // Arrange
            EntityName name = null;
            var description = "Permission to manage users";

            // Act
            var result = Permission.CreatePermission(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of Permission cannot be empty!");
        }

        [Fact]
        public void CreatePermission_Should_Return_Failure_When_Name_Is_Empty()
        {
            // Arrange
            var name = EntityName.Create("").Data;
            var description = "Permission to manage users";

            // Act
            var result = Permission.CreatePermission(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of Permission cannot be empty!");
        }

        [Fact]
        public void CreatePermission_Should_Return_Failure_When_Name_Is_Whitespace()
        {
            // Arrange
            var name = EntityName.Create("   ").Data;
            var description = "Permission to manage users";

            // Act
            var result = Permission.CreatePermission(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of Permission cannot be empty!");
        }

        [Theory]
        [InlineData("User Management", "Permission to manage users")]
        [InlineData("Department View", "Permission to view departments")]
        [InlineData("Role Assignment", "Permission to assign roles")]
        [InlineData("Permission Control", "Permission to control permissions")]
        [InlineData("System Admin", "Full system administration access")]
        public void CreatePermission_Should_Return_Success_With_Different_Valid_Names(string permissionName, string description)
        {
            // Arrange
            var name = EntityName.Create(permissionName).Data;

            // Act
            var result = Permission.CreatePermission(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be(permissionName);
            result.Data.Description.Should().Be(description);
        }

        [Fact]
        public void CreatePermission_Should_Create_Permission_With_Correct_Properties()
        {
            // Arrange
            var permissionName = "Advanced User Management";
            var description = "Advanced permission to manage users with extended capabilities";
            var name = EntityName.Create(permissionName).Data;

            // Act
            var result = Permission.CreatePermission(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be(permissionName);
            result.Data.Description.Should().Be(description);
            result.Data.Id.Should().NotBeEmpty();
            result.Data.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void CreatePermission_Should_Return_Different_Ids_For_Different_Permissions()
        {
            // Arrange
            var name1 = EntityName.Create("Permission 1").Data;
            var name2 = EntityName.Create("Permission 2").Data;
            var description = "Test permission";

            // Act
            var result1 = Permission.CreatePermission(name1, description);
            var result2 = Permission.CreatePermission(name2, description);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void CreatePermission_Should_Create_Permissions_With_Same_Name_As_Different_Entities()
        {
            // Arrange
            var name1 = EntityName.Create("User Management").Data;
            var name2 = EntityName.Create("User Management").Data;
            var description = "Permission to manage users";

            // Act
            var result1 = Permission.CreatePermission(name1, description);
            var result2 = Permission.CreatePermission(name2, description);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Domain entities can be created with same name - duplication validation happens at application layer
            result1.Data.Name.Value.Should().Be("User Management");
            result2.Data.Name.Value.Should().Be("User Management");
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }
    }
}

