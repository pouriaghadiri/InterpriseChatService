using Domain.Common.ValueObjects;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Entities
{
    public class RoleTests
    {
        [Fact]
        public void CreateRole_Should_Return_Success_When_Name_Is_Valid()
        {
            // Arrange
            var name = EntityName.Create("Admin").Data;
            var description = "Administrator role with full access";

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(name);
            result.Data.Description.Should().Be(description);
            result.Message.Should().Be("New Role created successfuly.");
        }

        [Fact]
        public void CreateRole_Should_Return_Success_When_Description_Is_Null()
        {
            // Arrange
            var name = EntityName.Create("User").Data;
            string description = null;

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(name);
            result.Data.Description.Should().BeNull();
        }

        [Fact]
        public void CreateRole_Should_Return_Success_When_Description_Is_Empty()
        {
            // Arrange
            var name = EntityName.Create("Guest").Data;
            string description = "";

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(name);
            result.Data.Description.Should().Be("");
        }

        [Fact]
        public void CreateRole_Should_Return_Failure_When_Name_Is_Null()
        {
            // Arrange
            EntityName name = null;
            var description = "Some description";

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of role cannot be empty!");
        }

        [Fact]
        public void CreateRole_Should_Return_Failure_When_Name_Is_Empty()
        {
            // Arrange
            var name = EntityName.Create("").Data;
            var description = "Some description";

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of role cannot be empty!");
        }

        [Fact]
        public void CreateRole_Should_Return_Failure_When_Name_Is_Whitespace()
        {
            // Arrange
            var name = EntityName.Create("   ").Data;
            var description = "Some description";

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of role cannot be empty!");
        }

        [Theory]
        [InlineData("Admin", "Full system access")]
        [InlineData("User", "Standard user access")]
        [InlineData("Guest", "Limited access")]
        [InlineData("Manager", "Department management access")]
        [InlineData("Supervisor", "Team supervision access")]
        [InlineData("Analyst", "Data analysis access")]
        public void CreateRole_Should_Return_Success_With_Different_Valid_Names_And_Descriptions(string roleName, string description)
        {
            // Arrange
            var name = EntityName.Create(roleName).Data;

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be(roleName);
            result.Data.Description.Should().Be(description);
        }

        [Fact]
        public void CreateRole_Should_Create_Role_With_Correct_Properties()
        {
            // Arrange
            var roleName = "System Administrator";
            var description = "Full system administration privileges";
            var name = EntityName.Create(roleName).Data;

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be(roleName);
            result.Data.Description.Should().Be(description);
            result.Data.Id.Should().NotBeEmpty();
            result.Data.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void CreateRole_Should_Return_Different_Ids_For_Different_Roles()
        {
            // Arrange
            var name1 = EntityName.Create("Role 1").Data;
            var name2 = EntityName.Create("Role 2").Data;
            var description = "Test description";

            // Act
            var result1 = Role.CreateRole(name1, description);
            var result2 = Role.CreateRole(name2, description);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Theory]
        [InlineData("Admin", null)]
        [InlineData("User", "")]
        [InlineData("Guest", "   ")]
        public void CreateRole_Should_Handle_Various_Description_Values(string roleName, string description)
        {
            // Arrange
            var name = EntityName.Create(roleName).Data;

            // Act
            var result = Role.CreateRole(name, description);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be(roleName);
            result.Data.Description.Should().Be(description);
        }

        #region Duplication Tests

        [Fact]
        public void CreateRole_Should_Create_Roles_With_Same_Name_As_Different_Entities()
        {
            // Arrange
            var name1 = EntityName.Create("Admin").Data;
            var name2 = EntityName.Create("Admin").Data;
            var description = "Administrator role";

            // Act
            var result1 = Role.CreateRole(name1, description);
            var result2 = Role.CreateRole(name2, description);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Domain entities can be created with same name - duplication validation happens at application layer
            result1.Data.Name.Value.Should().Be("Admin");
            result2.Data.Name.Value.Should().Be("Admin");
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void CreateRole_Should_Create_Roles_With_Same_Name_But_Different_Descriptions()
        {
            // Arrange
            var name1 = EntityName.Create("Manager").Data;
            var name2 = EntityName.Create("Manager").Data;
            var description1 = "Department manager role";
            var description2 = "Project manager role";

            // Act
            var result1 = Role.CreateRole(name1, description1);
            var result2 = Role.CreateRole(name2, description2);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Both roles should be created successfully with same name but different descriptions
            result1.Data.Name.Value.Should().Be("Manager");
            result2.Data.Name.Value.Should().Be("Manager");
            result1.Data.Description.Should().Be(description1);
            result2.Data.Description.Should().Be(description2);
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void CreateRole_Should_Handle_Case_Sensitive_Name_Comparison()
        {
            // Arrange
            var name1 = EntityName.Create("Admin").Data;
            var name2 = EntityName.Create("admin").Data;
            var description = "Administrator role";

            // Act
            var result1 = Role.CreateRole(name1, description);
            var result2 = Role.CreateRole(name2, description);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Different case names are treated as different entities at domain level
            result1.Data.Name.Value.Should().Be("Admin");
            result2.Data.Name.Value.Should().Be("admin");
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void CreateRole_Should_Handle_Whitespace_Differences_In_Name()
        {
            // Arrange
            var name1 = EntityName.Create("Admin").Data;
            var name2 = EntityName.Create(" Admin ").Data;
            var description = "Administrator role";

            // Act
            var result1 = Role.CreateRole(name1, description);
            var result2 = Role.CreateRole(name2, description);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Different whitespace names are treated as different entities at domain level
            result1.Data.Name.Value.Should().Be("Admin");
            result2.Data.Name.Value.Should().Be(" Admin ");
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void CreateRole_Should_Allow_Same_Name_With_Different_Instances()
        {
            // Arrange
            var roleName = "Administrator";
            var description = "System administrator role";
            var name1 = EntityName.Create(roleName).Data;
            var name2 = EntityName.Create(roleName).Data;

            // Act
            var result1 = Role.CreateRole(name1, description);
            var result2 = Role.CreateRole(name2, description);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Both should be created successfully - duplication prevention is handled at application layer
            result1.Data.Name.Value.Should().Be(roleName);
            result2.Data.Name.Value.Should().Be(roleName);
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        #endregion
    }
} 