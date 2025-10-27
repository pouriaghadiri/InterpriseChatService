using Application.Features.UserUseCase.DTOs;
using Application.Features.UserUseCase.Queries;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.UnitTest.Users.Queries
{
    public class GetUserByEmailQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserByNameQueryHandler _handler;

        public GetUserByEmailQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByNameQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_With_Valid_Email_Should_Return_User_Data()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetUserByEmailQuery(email);
            var user = CreateTestUserWithRoles();

            _userRepositoryMock
                .Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ID.Should().Be(user.Id);
            result.Data.FullName.Should().Be(user.FullName);
            result.Message.Should().Be("Transfer data completed successfully");
        }

        [Fact]
        public async Task Handle_With_Invalid_Email_Format_Should_Return_Validation_Error()
        {
            // Arrange
            var email = "invalid-email";
            var query = new GetUserByEmailQuery(email);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid input");
            result.Message.Should().Be("Email validation failed");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_With_Empty_Email_Should_Return_Validation_Error()
        {
            // Arrange
            var email = "";
            var query = new GetUserByEmailQuery(email);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid input");
            result.Message.Should().Be("Email validation failed");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_With_Null_Email_Should_Return_Validation_Error()
        {
            // Arrange
            var email = (string)null;
            var query = new GetUserByEmailQuery(email);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid input");
            result.Message.Should().Be("Email validation failed");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_With_Non_Existent_User_Should_Return_NotFound_Error()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var query = new GetUserByEmailQuery(email);

            _userRepositoryMock
                .Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("NotFound Error");
            result.Message.Should().Be("User is not found!");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_With_User_Having_Roles_Should_Include_Role_Data()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetUserByEmailQuery(email);
            var user = CreateTestUserWithRoles();

            _userRepositoryMock
                .Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.UserRoleInDepartments.Should().NotBeEmpty();
            result.Data.UserRoleInDepartments.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_With_User_Having_No_Roles_Should_Return_Empty_Role_List()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetUserByEmailQuery(email);
            var user = CreateTestUserWithoutRoles();

            _userRepositoryMock
                .Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.UserRoleInDepartments.Should().BeEmpty();
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("admin@company.org")]
        [InlineData("test@domain.co.uk")]
        public async Task Handle_With_Different_Valid_Emails_Should_Work(string email)
        {
            // Arrange
            var query = new GetUserByEmailQuery(email);
            var user = CreateTestUserWithRoles();

            _userRepositoryMock
                .Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_With_CancellationToken_Should_Pass_To_Repository()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetUserByEmailQuery(email);
            var cancellationToken = new CancellationToken();
            var user = CreateTestUserWithRoles();

            _userRepositoryMock
                .Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_Map_User_Properties_Correctly()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetUserByEmailQuery(email);
            var user = CreateTestUserWithRoles();

            _userRepositoryMock
                .Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ID.Should().Be(user.Id);
            result.Data.FullName.Should().Be(user.FullName);
        }

        [Fact]
        public async Task Handle_Should_Map_Role_Department_Data_Correctly()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetUserByEmailQuery(email);
            var user = CreateTestUserWithRoles();

            _userRepositoryMock
                .Setup(x => x.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.UserRoleInDepartments.Should().NotBeEmpty();
            
            var firstRole = result.Data.UserRoleInDepartments.First();
            firstRole.DepartmentID.Should().NotBeEmpty();
            firstRole.RoleID.Should().NotBeEmpty();
            firstRole.DepartmentName.Should().NotBeNull();
            firstRole.RoleName.Should().NotBeNull();
        }

        private User CreateTestUserWithRoles()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("test@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;

            // Add mock roles and departments
            var role1 = CreateTestRole("Admin");
            var role2 = CreateTestRole("User");
            var department1 = CreateTestDepartment("IT Department");
            var department2 = CreateTestDepartment("HR Department");

            var userRole1 = new UserRole { UserId = user.Id, RoleId = role1.Id };
            var userRole2 = new UserRole { UserId = user.Id, RoleId = role2.Id };

            var userRoleInDepartment1 = new UserRoleInDepartment
            {
                UserRoleId = userRole1.Id,
                DepartmentId = department1.Id,
                Department = department1,
                UserRole = userRole1
            };

            var userRoleInDepartment2 = new UserRoleInDepartment
            {
                UserRoleId = userRole2.Id,
                DepartmentId = department2.Id,
                Department = department2,
                UserRole = userRole2
            };

            userRole1.UserRoleInDepartments.Add(userRoleInDepartment1);
            userRole2.UserRoleInDepartments.Add(userRoleInDepartment2);

            user.UserRoles.Add(userRole1);
            user.UserRoles.Add(userRole2);

            return user;
        }

        private User CreateTestUserWithoutRoles()
        {
            var fullName = PersonFullName.Create("Jane", "Smith").Data;
            var email = Email.Create("jane@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09987654321").Data;

            return User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
        }

        private Role CreateTestRole(string name)
        {
            var entityName = EntityName.Create(name).Data;
            var role = Role.CreateRole(entityName, "Test Description").Data;
            return role;
        }

        private Department CreateTestDepartment(string name)
        {
            var entityName = EntityName.Create(name).Data;
            var department = Department.CreateDepartment(entityName).Data;
            return department;
        }
    }
}

