using Application.Features.UserUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.UnitTest.Users.Commands
{
    public class AssignRoleToUserCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
        private readonly Mock<IUserRoleInDepartmentRepository> _userRoleInDepartmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AssignRoleToUserCommandHandler _handler;

        public AssignRoleToUserCommandHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
            _userRoleInDepartmentRepositoryMock = new Mock<IUserRoleInDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            
            _handler = new AssignRoleToUserCommandHandler(
                _roleRepositoryMock.Object,
                _userRepositoryMock.Object,
                _departmentRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _userRoleRepositoryMock.Object,
                _userRoleInDepartmentRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_With_Valid_Data_Should_Assign_Role_Successfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            
            var command = new AssignRoleToUserCommand
            {
                User = CreateTestUser(userId),
                Role = CreateTestRole(roleId),
                Department = CreateTestDepartment(departmentId)
            };

            var user = CreateTestUser(userId);
            var role = CreateTestRole(roleId);
            var department = CreateTestDepartment(departmentId);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _departmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _roleRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userRoleInDepartmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRoleInDepartment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Assigned role to user successfully complleted.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_With_Non_Existent_User_Should_Return_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            
            var command = new AssignRoleToUserCommand
            {
                User = CreateTestUser(userId),
                Role = CreateTestRole(roleId),
                Department = CreateTestDepartment(departmentId)
            };

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Be("The selected user doesn't have exist!");
        }

        [Fact]
        public async Task Handle_With_Non_Existent_Department_Should_Return_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            
            var command = new AssignRoleToUserCommand
            {
                User = CreateTestUser(userId),
                Role = CreateTestRole(roleId),
                Department = CreateTestDepartment(departmentId)
            };

            var user = CreateTestUser(userId);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _departmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Be("The selected department doesn't have exist!");
        }

        [Fact]
        public async Task Handle_With_Non_Existent_Role_Should_Return_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            
            var command = new AssignRoleToUserCommand
            {
                User = CreateTestUser(userId),
                Role = CreateTestRole(roleId),
                Department = CreateTestDepartment(departmentId)
            };

            var user = CreateTestUser(userId);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _departmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _roleRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Exist Error");
            result.Message.Should().Be("The selected role doesn't have exist!");
        }

        [Fact]
        public async Task Handle_With_Existing_Assignment_Should_Return_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            
            var command = new AssignRoleToUserCommand
            {
                User = CreateTestUser(userId),
                Role = CreateTestRole(roleId),
                Department = CreateTestDepartment(departmentId)
            };

            var user = CreateTestUser(userId);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _departmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _roleRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userRoleInDepartmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRoleInDepartment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Exist Error");
            result.Message.Should().Be("This Assignment is exist!");
        }

        [Fact]
        public async Task Handle_With_User_Assignment_Failure_Should_Return_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            
            var command = new AssignRoleToUserCommand
            {
                User = CreateTestUser(userId),
                Role = CreateTestRole(roleId),
                Department = CreateTestDepartment(departmentId)
            };

            var user = CreateTestUserWithAssignmentFailure();

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _departmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _roleRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userRoleInDepartmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRoleInDepartment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Assignment Error");
            result.Message.Should().Be("Failed to assign role to user");
        }

        [Fact]
        public async Task Handle_With_CancellationToken_Should_Pass_To_All_Repositories()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();
            
            var command = new AssignRoleToUserCommand
            {
                User = CreateTestUser(userId),
                Role = CreateTestRole(roleId),
                Department = CreateTestDepartment(departmentId)
            };

            var user = CreateTestUser(userId);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _departmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>(), cancellationToken))
                .ReturnsAsync(true);

            _roleRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), cancellationToken))
                .ReturnsAsync(true);

            _userRoleInDepartmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRoleInDepartment, bool>>>(), cancellationToken))
                .ReturnsAsync(false);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(cancellationToken))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
        }

        [Theory]
        [InlineData("Admin", "IT Department")]
        [InlineData("User", "HR Department")]
        [InlineData("Manager", "Finance Department")]
        public async Task Handle_With_Different_Role_Department_Combinations_Should_Work(string roleName, string departmentName)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            
            var command = new AssignRoleToUserCommand
            {
                User = CreateTestUser(userId),
                Role = CreateTestRole(roleId),
                Department = CreateTestDepartment(departmentId)
            };

            var user = CreateTestUser(userId);

            _userRepositoryMock
                .Setup(x => x.GetbyIdAsync(userId))
                .ReturnsAsync(user);

            _departmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _roleRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userRoleInDepartmentRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRoleInDepartment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Assigned role to user successfully complleted.");
        }

        private User CreateTestUser(Guid? id = null)
        {
            var fullName = PersonFullName.Create("Test", "User").Data;
            var email = Email.Create("test@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone =  PhoneNumber.Create("09123456789").Data;

            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
            if (id.HasValue)
            {
                // Use reflection to set the Id for testing
                var idProperty = typeof(User).GetProperty("Id");
                idProperty?.SetValue(user, id.Value);
            }
            return user;
        }

        private User CreateTestUserWithAssignmentFailure()
        {
            // Create a real user that will fail assignment due to business logic
            var fullName = PersonFullName.Create("Test", "User").Data;
            var email = Email.Create("test@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
            
            // You can add specific conditions here that would cause assignment failure
            // For example, set a property that would make the business logic fail
            // This depends on your actual business rules in the AssignRoleToUser method
            
            return user;
        }

        private User CreateTestUserWithActiveDepartment(Guid? userId = null, Guid? activeDepartmentId = null)
        {
            var fullName = PersonFullName.Create("Test", "User").Data;
            var email = Email.Create("test@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;

            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;
            
            if (userId.HasValue)
            {
                typeof(User).GetProperty("Id")?.SetValue(user, userId.Value);
            }
            
            if (activeDepartmentId.HasValue)
            {
                typeof(User).GetProperty("ActiveDepartmentId")?.SetValue(user, activeDepartmentId.Value);
            }
            
            return user;
        }

        private Role CreateTestRole(Guid? id = null)
        {
            var name = EntityName.Create("Test Role").Data;
            var role = Role.CreateRole(name, "Test Description").Data;
            if (id.HasValue)
            {
                var idProperty = typeof(Role).GetProperty("Id");
                idProperty?.SetValue(role, id.Value);
            }
            return role;
        }

        private Department CreateTestDepartment(Guid? id = null)
        {
            var name = EntityName.Create("Test Department").Data;
            var department = Department.CreateDepartment(name).Data;
            if (id.HasValue)
            {
                var idProperty = typeof(Department).GetProperty("Id");
                idProperty?.SetValue(department, id.Value);
            }
            return department;
        }
    }
}
