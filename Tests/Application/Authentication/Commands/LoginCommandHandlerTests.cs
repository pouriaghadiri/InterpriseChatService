using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Services;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using RoleEntity = Domain.Entities.Role;
using DepartmentEntity = Domain.Entities.Department;

namespace Tests.Application.Authentication.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IActiveDepartmentService> _activeDepartmentServiceMock;
        private readonly Mock<IUserRoleInDepartmentRepository> _userRoleInDepartmentRepositoryMock;
        private readonly LoginCommandHandler _handler;
        private readonly LoginCommand _request;

        public LoginCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _activeDepartmentServiceMock = new Mock<IActiveDepartmentService>();
            _userRoleInDepartmentRepositoryMock = new Mock<IUserRoleInDepartmentRepository>();

            _handler = new LoginCommandHandler(
                _userRepositoryMock.Object,
                _jwtTokenServiceMock.Object,
                _cacheServiceMock.Object,
                _activeDepartmentServiceMock.Object,
                _userRoleInDepartmentRepositoryMock.Object);

            _request = new LoginCommand
            {
                Email = "john@example.com",
                Password = "Test123!@#"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Token_WhenCredentialsAreValid()
        {
            // Arrange
            var user = CreateTestUser();
            var department = CreateTestDepartment();
            var role = CreateTestRole();
            var roles = new List<string> { "Admin" };
            var token = "test-jwt-token";
            var refreshToken = "test-refresh-token";
            var expireDate = DateTime.UtcNow.AddHours(1);
            var refreshTokenExpireDate = DateTime.UtcNow.AddDays(7);

            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            _userRoleInDepartmentRepositoryMock
                .Setup(repo => repo.GetRolesOfUserInDepartment(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<RoleEntity> { role });

            DateTime capturedExpireDate = expireDate;
            _jwtTokenServiceMock
                .Setup(service => service.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>(), out capturedExpireDate, null))
                .Returns(token);

            DateTime capturedRefreshTokenExpireDate = refreshTokenExpireDate;
            _jwtTokenServiceMock
                .Setup(service => service.GenerateRefreshToken(It.IsAny<User>(), out capturedRefreshTokenExpireDate))
                .Returns(refreshToken);

            _cacheServiceMock
                .Setup(service => service.SetAsync(It.IsAny<string>(), It.IsAny<TokenResultDTO>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _activeDepartmentServiceMock
                .Setup(service => service.SetActiveDepartmentIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Token.Should().Be(token);
            result.Data.RefreshToken.Should().Be(refreshToken);
            result.Data.ExpireTime.Should().Be(expireDate);
            result.Message.Should().Be("Logged in");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenEmailIsInvalid()
        {
            // Arrange
            _request.Email = "invalid-email";

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid");
            result.Message.Should().Be("Invalid email");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Auth");
            result.Message.Should().Be("Invalid credentials");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act - using wrong password
            _request.Password = "WrongPassword123!";
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Auth");
            result.Message.Should().Be("Invalid credentials");
        }

        [Fact]
        public async Task Handle_Should_CacheActiveDepartment_WhenUserHasActiveDepartment()
        {
            // Arrange
            var user = CreateTestUser();
            var department = CreateTestDepartment();
            user.SetActiveDepartment(department.Id);
            var role = CreateTestRole();
            var roles = new List<string> { "Admin" };
            var token = "test-jwt-token";
            var refreshToken = "test-refresh-token";
            var expireDate = DateTime.UtcNow.AddHours(1);
            var refreshTokenExpireDate = DateTime.UtcNow.AddDays(7);

            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            _userRoleInDepartmentRepositoryMock
                .Setup(repo => repo.GetRolesOfUserInDepartment(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<RoleEntity> { role });

            DateTime capturedExpireDate = expireDate;
            _jwtTokenServiceMock
                .Setup(service => service.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>(), out capturedExpireDate, null))
                .Returns(token);

            DateTime capturedRefreshTokenExpireDate = refreshTokenExpireDate;
            _jwtTokenServiceMock
                .Setup(service => service.GenerateRefreshToken(It.IsAny<User>(), out capturedRefreshTokenExpireDate))
                .Returns(refreshToken);

            _cacheServiceMock
                .Setup(service => service.SetAsync(It.IsAny<string>(), It.IsAny<TokenResultDTO>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _activeDepartmentServiceMock
                .Setup(service => service.SetActiveDepartmentIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _activeDepartmentServiceMock.Verify(
                service => service.SetActiveDepartmentIdAsync(user.Id, user.ActiveDepartmentId!.Value),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserHasNoDepartment()
        {
            // Arrange
            var user = CreateTestUser();
            user.UserRoles = new List<UserRole>(); // No roles, so no departments
            var roles = new List<string>();

            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Auth");
            result.Message.Should().Be("User has no valid department");
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;
            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;

            // Add a role with department
            var role = CreateTestRole();
            var department = CreateTestDepartment();
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                User = user,
                Role = role
            };
            userRole.UserRoleInDepartments = new List<UserRoleInDepartment>
            {
                new UserRoleInDepartment
                {
                    UserRoleId = userRole.Id,
                    DepartmentId = department.Id,
                    UserRole = userRole,
                    Department = department
                }
            };
            user.UserRoles = new List<UserRole> { userRole };

            return user;
        }

        private RoleEntity CreateTestRole()
        {
            var name = EntityName.Create("Admin").Data;
            return RoleEntity.CreateRole(name, "Administrator role").Data;
        }

        private DepartmentEntity CreateTestDepartment()
        {
            var name = EntityName.Create("IT").Data;
            return DepartmentEntity.CreateDepartment(name).Data;
        }
    }
}

