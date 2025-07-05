using Domain.Common.ValueObjects;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTest.Domain.Entities
{
    public class DepartmentTests
    {
        [Fact]
        public void CreateDepartment_Should_Return_Success_When_Name_Is_Valid()
        {
            // Arrange
            var name = EntityName.Create("Engineering Department").Data;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be(name);
            result.Message.Should().Be("New Department created successfuly.");
        }

        [Fact]
        public void CreateDepartment_Should_Return_Failure_When_Name_Is_Null()
        {
            // Arrange
            EntityName name = null;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of Department cannot be empty!");
        }

        [Fact]
        public void CreateDepartment_Should_Return_Failure_When_Name_Is_Empty()
        {
            // Arrange
            var name = EntityName.Create("").Data;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of Department cannot be empty!");
        }

        [Fact]
        public void CreateDepartment_Should_Return_Failure_When_Name_Is_Whitespace()
        {
            // Arrange
            var name = EntityName.Create("   ").Data;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("name of Department cannot be empty!");
        }

        [Theory]
        [InlineData("HR Department")]
        [InlineData("Marketing Team")]
        [InlineData("Sales Division")]
        [InlineData("IT Support")]
        [InlineData("Finance Department")]
        [InlineData("Operations Team")]
        public void CreateDepartment_Should_Return_Success_With_Different_Valid_Names(string departmentName)
        {
            // Arrange
            var name = EntityName.Create(departmentName).Data;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be(departmentName);
            //result.Data.UserRoleInDepartments.Should().NotBeNull();
        }

        [Fact]
        public void CreateDepartment_Should_Create_Department_With_Correct_Properties()
        {
            // Arrange
            var departmentName = "Research & Development";
            var name = EntityName.Create(departmentName).Data;

            // Act
            var result = Department.CreateDepartment(name);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Value.Should().Be(departmentName);
            result.Data.Id.Should().NotBeEmpty();
            result.Data.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void CreateDepartment_Should_Return_Different_Ids_For_Different_Departments()
        {
            // Arrange
            var name1 = EntityName.Create("Department 1").Data;
            var name2 = EntityName.Create("Department 2").Data;

            // Act
            var result1 = Department.CreateDepartment(name1);
            var result2 = Department.CreateDepartment(name2);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        #region Duplication Tests

        [Fact]
        public void CreateDepartment_Should_Create_Departments_With_Same_Name_As_Different_Entities()
        {
            // Arrange
            var name1 = EntityName.Create("IT Department").Data;
            var name2 = EntityName.Create("IT Department").Data;

            // Act
            var result1 = Department.CreateDepartment(name1);
            var result2 = Department.CreateDepartment(name2);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Domain entities can be created with same name - duplication validation happens at application layer
            result1.Data.Name.Value.Should().Be("IT Department");
            result2.Data.Name.Value.Should().Be("IT Department");
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void CreateDepartment_Should_Handle_Case_Sensitive_Name_Comparison()
        {
            // Arrange
            var name1 = EntityName.Create("IT Department").Data;
            var name2 = EntityName.Create("it department").Data;

            // Act
            var result1 = Department.CreateDepartment(name1);
            var result2 = Department.CreateDepartment(name2);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Different case names are treated as different entities at domain level
            result1.Data.Name.Value.Should().Be("IT Department");
            result2.Data.Name.Value.Should().Be("it department");
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void CreateDepartment_Should_Handle_Whitespace_Differences_In_Name()
        {
            // Arrange
            var name1 = EntityName.Create("IT Department").Data;
            var name2 = EntityName.Create(" IT Department ").Data;

            // Act
            var result1 = Department.CreateDepartment(name1);
            var result2 = Department.CreateDepartment(name2);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Different whitespace names are treated as different entities at domain level
            result1.Data.Name.Value.Should().Be("IT Department");
            result2.Data.Name.Value.Should().Be(" IT Department ");
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        [Fact]
        public void CreateDepartment_Should_Allow_Same_Name_With_Different_Instances()
        {
            // Arrange
            var departmentName = "Engineering Department";
            var name1 = EntityName.Create(departmentName).Data;
            var name2 = EntityName.Create(departmentName).Data;

            // Act
            var result1 = Department.CreateDepartment(name1);
            var result2 = Department.CreateDepartment(name2);

            // Assert
            result1.Should().NotBeNull();
            result1.IsSuccess.Should().BeTrue();
            result2.Should().NotBeNull();
            result2.IsSuccess.Should().BeTrue();
            
            // Both should be created successfully - duplication prevention is handled at application layer
            result1.Data.Name.Value.Should().Be(departmentName);
            result2.Data.Name.Value.Should().Be(departmentName);
            result1.Data.Id.Should().NotBe(result2.Data.Id);
        }

        #endregion
    }
} 